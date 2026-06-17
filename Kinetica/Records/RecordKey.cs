using System.Runtime.CompilerServices;
using System.Text;

namespace kinetica.Records;

/// <summary>
/// A binary key used for shard routing.
/// Equivalent to Rust's RecordKey.
///
/// This class holds a binary-encoded key buffer and computes a routing hash
/// using MurmurHash3 for consistent shard distribution.
/// </summary>
public sealed class RecordKey
{
    private readonly byte[] _buffer;
    private int _position;
    private long _routingHash;
    private bool _isValid;

    /// <summary>
    /// Creates a new RecordKey with the specified buffer size.
    /// </summary>
    public RecordKey(int bufferSize)
    {
        _buffer = new byte[bufferSize];
        _position = 0;
        _routingHash = 0;
        _isValid = true;
    }

    /// <summary>
    /// Returns true if this key is valid (no null values were added).
    /// </summary>
    public bool IsValid => _isValid && _position > 0;

    /// <summary>
    /// Gets the routing hash (must call ComputeHash first).
    /// </summary>
    public long RoutingHash => _routingHash;

    /// <summary>
    /// Gets the hash code for stripe distribution.
    /// </summary>
    public long HashCode() => _routingHash;

    /// <summary>
    /// Routes this key to a worker index using the routing table.
    /// </summary>
    public int Route(IList<int> routingTable)
    {
        if (routingTable == null || routingTable.Count == 0)
            return 0;

        // Use modulo to find the slot in routing table
        int slot = (int)(Math.Abs(_routingHash) % routingTable.Count);
        int rank = routingTable[slot];

        // Convert 1-based rank to 0-based worker index
        return rank > 0 ? rank - 1 : 0;
    }

    /// <summary>
    /// Invalidates this key (e.g., when a null value is encountered).
    /// </summary>
    public void Invalidate()
    {
        _isValid = false;
    }

    /// <summary>
    /// Computes the routing hash from the buffer contents.
    /// </summary>
    public void ComputeHash()
    {
        if (_position > 0)
        {
            _routingHash = MurmurHash3.Hash128(_buffer, 0, _position);
        }
    }

    #region Add Methods

    /// <summary>Adds an 8-bit integer to the key.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddInt8(sbyte value)
    {
        EnsureCapacity(1);
        _buffer[_position++] = (byte)value;
    }

    /// <summary>Adds a 16-bit integer to the key (little-endian).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddInt16(short value)
    {
        EnsureCapacity(2);
        _buffer[_position++] = (byte)value;
        _buffer[_position++] = (byte)(value >> 8);
    }

    /// <summary>Adds a 32-bit integer to the key (little-endian).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddInt(int value)
    {
        EnsureCapacity(4);
        _buffer[_position++] = (byte)value;
        _buffer[_position++] = (byte)(value >> 8);
        _buffer[_position++] = (byte)(value >> 16);
        _buffer[_position++] = (byte)(value >> 24);
    }

    /// <summary>Adds a 64-bit integer to the key (little-endian).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddLong(long value)
    {
        EnsureCapacity(8);
        _buffer[_position++] = (byte)value;
        _buffer[_position++] = (byte)(value >> 8);
        _buffer[_position++] = (byte)(value >> 16);
        _buffer[_position++] = (byte)(value >> 24);
        _buffer[_position++] = (byte)(value >> 32);
        _buffer[_position++] = (byte)(value >> 40);
        _buffer[_position++] = (byte)(value >> 48);
        _buffer[_position++] = (byte)(value >> 56);
    }

    /// <summary>Adds a 32-bit float to the key.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddFloat(float value)
    {
        var bits = BitConverter.SingleToInt32Bits(value);
        AddInt(bits);
    }

    /// <summary>Adds a 64-bit double to the key.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddDouble(double value)
    {
        var bits = BitConverter.DoubleToInt64Bits(value);
        AddLong(bits);
    }

    /// <summary>Adds a string to the key (via MurmurHash3).</summary>
    public void AddString(string value)
    {
        if (value == null)
        {
            Invalidate();
            return;
        }

        var hash = MurmurHash3.Hash128(Encoding.UTF8.GetBytes(value));
        AddLong(hash);
    }

    /// <summary>Adds a fixed-length char to the key.</summary>
    public void AddCharN(string value, int length)
    {
        if (value == null)
        {
            Invalidate();
            return;
        }

        EnsureCapacity(length);

        var bytes = Encoding.UTF8.GetBytes(value);
        int copyLen = Math.Min(bytes.Length, length);

        Array.Copy(bytes, 0, _buffer, _position, copyLen);

        // Pad with zeros if string is shorter than expected length
        for (int i = copyLen; i < length; i++)
        {
            _buffer[_position + i] = 0;
        }

        _position += length;
    }

    /// <summary>Adds a date string (YYYY-MM-DD) to the key.</summary>
    public void AddDate(string value)
    {
        if (value == null)
        {
            Invalidate();
            return;
        }

        if (!TryParseDate(value, out int year, out int month, out int day))
        {
            // Fall back to string hash
            AddString(value);
            return;
        }

        // Pack date as integer: year * 10000 + month * 100 + day
        // Then store as 4-byte hash
        int packed = year * 10000 + month * 100 + day;
        AddInt(packed);
    }

    /// <summary>Adds a datetime string (YYYY-MM-DD HH:MM:SS.mmm) to the key.</summary>
    public void AddDateTime(string value)
    {
        if (value == null)
        {
            Invalidate();
            return;
        }

        if (!TryParseDateTime(value, out int year, out int month, out int day,
            out int hour, out int minute, out int second, out int millis))
        {
            // Fall back to string hash
            AddString(value);
            return;
        }

        // Pack datetime into 8 bytes using bit-packing
        // Similar to Rust implementation
        long packed = 0;
        packed |= ((long)(year - 1900) & 0x3FF) << 54;  // 10 bits for year
        packed |= ((long)month & 0xF) << 50;           // 4 bits for month
        packed |= ((long)day & 0x1F) << 45;            // 5 bits for day
        packed |= ((long)hour & 0x1F) << 40;           // 5 bits for hour
        packed |= ((long)minute & 0x3F) << 34;         // 6 bits for minute
        packed |= ((long)second & 0x3F) << 28;         // 6 bits for second
        packed |= ((long)millis & 0x3FF) << 18;        // 10 bits for millis

        AddLong(packed);
    }

    /// <summary>Adds a time string (HH:MM:SS.mmm) to the key.</summary>
    public void AddTime(string value)
    {
        if (value == null)
        {
            Invalidate();
            return;
        }

        if (!TryParseTime(value, out int hour, out int minute, out int second, out int millis))
        {
            // Fall back to string hash
            AddString(value);
            return;
        }

        // Pack time into 4 bytes
        int packed = (hour * 3600000) + (minute * 60000) + (second * 1000) + millis;
        AddInt(packed);
    }

    /// <summary>Adds an IPv4 address string (dotted-quad) to the key.</summary>
    public void AddIpv4(string value)
    {
        if (value == null)
        {
            Invalidate();
            return;
        }

        if (!TryParseIpv4(value, out uint ip))
        {
            // Fall back to string hash
            AddString(value);
            return;
        }

        AddInt((int)ip);
    }

    /// <summary>Adds a decimal string to the key.</summary>
    public void AddDecimal(string value, int precision, int scale)
    {
        if (value == null)
        {
            Invalidate();
            return;
        }

        // For simplicity, use string hash for decimal
        // A full implementation would parse and encode the decimal value
        AddString(value);
    }

    /// <summary>Adds a UUID string to the key.</summary>
    public void AddUuid(string value)
    {
        if (value == null)
        {
            Invalidate();
            return;
        }

        // Hash the UUID string
        AddString(value);
    }

    #endregion

    #region Parsing Helpers

    private static bool TryParseDate(string value, out int year, out int month, out int day)
    {
        year = month = day = 0;

        if (value.Length < 10)
            return false;

        // YYYY-MM-DD
        if (int.TryParse(value.AsSpan(0, 4), out year) &&
            int.TryParse(value.AsSpan(5, 2), out month) &&
            int.TryParse(value.AsSpan(8, 2), out day))
        {
            return true;
        }

        return false;
    }

    private static bool TryParseDateTime(string value,
        out int year, out int month, out int day,
        out int hour, out int minute, out int second, out int millis)
    {
        year = month = day = hour = minute = second = millis = 0;

        if (value.Length < 19)
            return false;

        // YYYY-MM-DD HH:MM:SS
        if (!TryParseDate(value, out year, out month, out day))
            return false;

        if (int.TryParse(value.AsSpan(11, 2), out hour) &&
            int.TryParse(value.AsSpan(14, 2), out minute) &&
            int.TryParse(value.AsSpan(17, 2), out second))
        {
            // Try to parse milliseconds if present
            if (value.Length > 20 && value[19] == '.')
            {
                var msSpan = value.AsSpan(20);
                int msLen = Math.Min(msSpan.Length, 3);
                if (int.TryParse(msSpan.Slice(0, msLen), out millis))
                {
                    // Normalize to 3 digits
                    while (msLen < 3) { millis *= 10; msLen++; }
                }
            }
            return true;
        }

        return false;
    }

    private static bool TryParseTime(string value,
        out int hour, out int minute, out int second, out int millis)
    {
        hour = minute = second = millis = 0;

        if (value.Length < 8)
            return false;

        // HH:MM:SS
        if (int.TryParse(value.AsSpan(0, 2), out hour) &&
            int.TryParse(value.AsSpan(3, 2), out minute) &&
            int.TryParse(value.AsSpan(6, 2), out second))
        {
            // Try to parse milliseconds if present
            if (value.Length > 9 && value[8] == '.')
            {
                var msSpan = value.AsSpan(9);
                int msLen = Math.Min(msSpan.Length, 3);
                if (int.TryParse(msSpan.Slice(0, msLen), out millis))
                {
                    while (msLen < 3) { millis *= 10; msLen++; }
                }
            }
            return true;
        }

        return false;
    }

    private static bool TryParseIpv4(string value, out uint ip)
    {
        ip = 0;

        var parts = value.Split('.');
        if (parts.Length != 4)
            return false;

        for (int i = 0; i < 4; i++)
        {
            if (!byte.TryParse(parts[i], out byte b))
                return false;
            ip = (ip << 8) | b;
        }

        return true;
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCapacity(int additional)
    {
        if (_position + additional > _buffer.Length)
        {
            throw new InvalidOperationException(
                $"RecordKey buffer overflow: need {_position + additional} bytes, have {_buffer.Length}");
        }
    }
}

/// <summary>
/// MurmurHash3 implementation for 128-bit hashing.
/// </summary>
internal static class MurmurHash3
{
    private const ulong C1 = 0x87c37b91114253d5UL;
    private const ulong C2 = 0x4cf5ad432745937fUL;

    /// <summary>
    /// Computes a 128-bit MurmurHash3 and returns the lower 64 bits.
    /// </summary>
    public static long Hash128(byte[] data)
    {
        return Hash128(data, 0, data.Length);
    }

    /// <summary>
    /// Computes a 128-bit MurmurHash3 and returns the lower 64 bits.
    /// </summary>
    public static long Hash128(byte[] data, int offset, int length, uint seed = 0)
    {
        ulong h1 = seed;
        ulong h2 = seed;

        int nblocks = length / 16;

        // Body
        for (int i = 0; i < nblocks; i++)
        {
            ulong k1 = GetBlock64(data, offset + i * 16);
            ulong k2 = GetBlock64(data, offset + i * 16 + 8);

            k1 *= C1;
            k1 = RotateLeft(k1, 31);
            k1 *= C2;
            h1 ^= k1;

            h1 = RotateLeft(h1, 27);
            h1 += h2;
            h1 = h1 * 5 + 0x52dce729;

            k2 *= C2;
            k2 = RotateLeft(k2, 33);
            k2 *= C1;
            h2 ^= k2;

            h2 = RotateLeft(h2, 31);
            h2 += h1;
            h2 = h2 * 5 + 0x38495ab5;
        }

        // Tail
        int tail = offset + nblocks * 16;
        int remaining = length - nblocks * 16;

        ulong k1_tail = 0;
        ulong k2_tail = 0;

        switch (remaining)
        {
            case 15: k2_tail ^= (ulong)data[tail + 14] << 48; goto case 14;
            case 14: k2_tail ^= (ulong)data[tail + 13] << 40; goto case 13;
            case 13: k2_tail ^= (ulong)data[tail + 12] << 32; goto case 12;
            case 12: k2_tail ^= (ulong)data[tail + 11] << 24; goto case 11;
            case 11: k2_tail ^= (ulong)data[tail + 10] << 16; goto case 10;
            case 10: k2_tail ^= (ulong)data[tail + 9] << 8; goto case 9;
            case 9:
                k2_tail ^= data[tail + 8];
                k2_tail *= C2;
                k2_tail = RotateLeft(k2_tail, 33);
                k2_tail *= C1;
                h2 ^= k2_tail;
                goto case 8;
            case 8: k1_tail ^= (ulong)data[tail + 7] << 56; goto case 7;
            case 7: k1_tail ^= (ulong)data[tail + 6] << 48; goto case 6;
            case 6: k1_tail ^= (ulong)data[tail + 5] << 40; goto case 5;
            case 5: k1_tail ^= (ulong)data[tail + 4] << 32; goto case 4;
            case 4: k1_tail ^= (ulong)data[tail + 3] << 24; goto case 3;
            case 3: k1_tail ^= (ulong)data[tail + 2] << 16; goto case 2;
            case 2: k1_tail ^= (ulong)data[tail + 1] << 8; goto case 1;
            case 1:
                k1_tail ^= data[tail];
                k1_tail *= C1;
                k1_tail = RotateLeft(k1_tail, 31);
                k1_tail *= C2;
                h1 ^= k1_tail;
                break;
        }

        // Finalization
        h1 ^= (ulong)length;
        h2 ^= (ulong)length;

        h1 += h2;
        h2 += h1;

        h1 = FMix64(h1);
        h2 = FMix64(h2);

        h1 += h2;
        // h2 += h1; // Not needed since we only return h1

        return (long)h1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong GetBlock64(byte[] data, int offset)
    {
        return BitConverter.ToUInt64(data, offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong RotateLeft(ulong x, int r)
    {
        return (x << r) | (x >> (64 - r));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong FMix64(ulong k)
    {
        k ^= k >> 33;
        k *= 0xff51afd7ed558ccdUL;
        k ^= k >> 33;
        k *= 0xc4ceb9fe1a85ec53UL;
        k ^= k >> 33;
        return k;
    }
}
