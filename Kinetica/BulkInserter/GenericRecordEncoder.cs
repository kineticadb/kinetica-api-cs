using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;
using Avro;
using kinetica.Records;

namespace kinetica;

/// <summary>
/// High-performance Avro encoder for GenericRecord instances.
///
/// This encoder reads values directly from GenericRecord's RecordValue[] array
/// and writes Avro binary format without intermediate object allocations.
/// </summary>
internal sealed class GenericRecordEncoder
{
    private readonly Schema.Type[] _fieldTypes;
    private readonly bool[] _fieldNullable;
    private readonly int[] _nullIndex; // Index of null in union (0 or 1)
    private readonly int _fieldCount;

    // Thread-local buffer for encoding
    [ThreadStatic]
    private static byte[]? _threadLocalBuffer;

    // Cache of encoders by schema
    private static readonly ConcurrentDictionary<string, GenericRecordEncoder> _encoderCache = new();

    /// <summary>
    /// Gets or creates a GenericRecordEncoder for the specified KineticaType.
    /// </summary>
    public static GenericRecordEncoder GetOrCreate(KineticaType ktype)
    {
        var schemaString = ktype.getSchemaString();
        return _encoderCache.GetOrAdd(schemaString, _ => new GenericRecordEncoder(ktype));
    }

    private GenericRecordEncoder(KineticaType ktype)
    {
        var schema = ktype.getSchema() as RecordSchema
            ?? throw new ArgumentException("Invalid schema: not a record schema");

        _fieldCount = schema.Fields.Count;
        _fieldTypes = new Schema.Type[_fieldCount];
        _fieldNullable = new bool[_fieldCount];
        _nullIndex = new int[_fieldCount];

        for (int i = 0; i < _fieldCount; i++)
        {
            var field = schema.Fields[i];
            var fieldSchema = field.Schema;

            // Handle union types (nullable)
            if (fieldSchema is UnionSchema unionSchema)
            {
                _fieldNullable[i] = true;

                // Find the positions of null and non-null types in the union
                // Kinetica typically uses ["type", "null"] (non-null first, null second)
                // But we need to handle both orderings
                int nullIdx = -1;
                int nonNullIdx = -1;

                for (int j = 0; j < unionSchema.Schemas.Count; j++)
                {
                    var s = unionSchema.Schemas[j];
                    if (s.Tag == Schema.Type.Null)
                    {
                        nullIdx = j;
                    }
                    else
                    {
                        nonNullIdx = j;
                        _fieldTypes[i] = s.Tag;
                    }
                }

                _nullIndex[i] = nullIdx;
            }
            else
            {
                _fieldNullable[i] = false;
                _fieldTypes[i] = fieldSchema.Tag;
                _nullIndex[i] = -1;
            }
        }
    }

    /// <summary>
    /// Encodes a single GenericRecord to Avro binary format.
    /// </summary>
    public byte[] Encode(GenericRecord record)
    {
        var values = record.GetValues();
        var buffer = GetBuffer();
        int position = 0;

        for (int i = 0; i < _fieldCount; i++)
        {
            var value = values[i];
            var fieldType = _fieldTypes[i];
            var nullable = _fieldNullable[i];

            AvroEncoding.EnsureCapacity(ref buffer, position, 32);

            if (nullable)
            {
                var nullIdx = _nullIndex[i];
                var nonNullIdx = nullIdx == 0 ? 1 : 0;

                if (value.IsNull)
                {
                    // Write null union index (zig-zag encoded)
                    position = AvroEncoding.WriteVarInt(buffer, position, nullIdx);
                    continue;
                }
                else
                {
                    // Write non-null union index (zig-zag encoded)
                    position = AvroEncoding.WriteVarInt(buffer, position, nonNullIdx);
                }
            }

            position = EncodeValue(buffer, position, value, fieldType);
        }

        // Copy to exact-size result
        var result = new byte[position];
        Buffer.BlockCopy(buffer, 0, result, 0, position);
        return result;
    }

    /// <summary>
    /// Encodes multiple GenericRecords in parallel using thread-local buffers.
    /// </summary>
    public byte[][] EncodeMany(IReadOnlyList<GenericRecord> records)
    {
        var count = records.Count;
        var results = new byte[count][];

        if (count < 100)
        {
            // Sequential for small batches (< 100 records)
            for (int i = 0; i < count; i++)
            {
                results[i] = Encode(records[i]);
            }
        }
        else
        {
            // Parallel for large batches (>= 100 records)
            // Each thread uses its own thread-local buffer for efficiency
            System.Threading.Tasks.Parallel.For(0, count, i =>
            {
                results[i] = Encode(records[i]);
            });
        }

        return results;
    }

    /// <summary>
    /// Encodes multiple GenericRecords to a list of byte arrays.
    /// Uses parallel encoding for batches of 100+ records.
    /// </summary>
    public List<byte[]> EncodeManyAsList(IReadOnlyList<GenericRecord> records)
    {
        return new List<byte[]>(EncodeMany(records));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int EncodeValue(byte[] buffer, int position, RecordValue value, Schema.Type fieldType)
    {
        switch (fieldType)
        {
            case Schema.Type.Int:
                return AvroEncoding.WriteVarInt(buffer, position, value.AsInt() ?? 0);

            case Schema.Type.Long:
                return AvroEncoding.WriteVarLong(buffer, position, value.AsLong() ?? 0);

            case Schema.Type.Float:
                var floatVal = value.AsFloat() ?? 0f;
                AvroEncoding.EnsureCapacity(ref buffer, position, 4);
                var floatBytes = BitConverter.GetBytes(floatVal);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(floatBytes);
                Buffer.BlockCopy(floatBytes, 0, buffer, position, 4);
                return position + 4;

            case Schema.Type.Double:
                var doubleVal = value.AsDouble() ?? 0.0;
                AvroEncoding.EnsureCapacity(ref buffer, position, 8);
                var doubleBytes = BitConverter.GetBytes(doubleVal);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(doubleBytes);
                Buffer.BlockCopy(doubleBytes, 0, buffer, position, 8);
                return position + 8;

            case Schema.Type.Boolean:
                buffer[position++] = (byte)(value.AsBool() == true ? 1 : 0);
                return position;

            case Schema.Type.String:
                var strVal = value.AsString() ?? string.Empty;
                var strBytes = Encoding.UTF8.GetBytes(strVal);
                AvroEncoding.EnsureCapacity(ref buffer, position, strBytes.Length + 10);
                position = AvroEncoding.WriteVarLong(buffer, position, strBytes.Length);
                Buffer.BlockCopy(strBytes, 0, buffer, position, strBytes.Length);
                return position + strBytes.Length;

            case Schema.Type.Bytes:
                var bytesVal = value.AsBytes() ?? Array.Empty<byte>();
                AvroEncoding.EnsureCapacity(ref buffer, position, bytesVal.Length + 10);
                position = AvroEncoding.WriteVarLong(buffer, position, bytesVal.Length);
                Buffer.BlockCopy(bytesVal, 0, buffer, position, bytesVal.Length);
                return position + bytesVal.Length;

            default:
                // For unsupported types, write empty string
                buffer[position++] = 0;
                return position;
        }
    }

    private static byte[] GetBuffer()
    {
        var buffer = _threadLocalBuffer;
        if (buffer == null)
        {
            buffer = new byte[4096];
            _threadLocalBuffer = buffer;
        }
        return buffer;
    }
}
