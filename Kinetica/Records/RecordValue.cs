using System.Runtime.CompilerServices;

namespace kinetica.Records;

/// <summary>
/// Represents the type of a record value.
/// Equivalent to Rust's RecordValue enum variants.
/// </summary>
public enum RecordValueType
{
    /// <summary>Null value</summary>
    Null,
    /// <summary>32-bit signed integer (also used for boolean, int8, int16)</summary>
    Int,
    /// <summary>64-bit signed integer (also used for timestamp)</summary>
    Long,
    /// <summary>32-bit IEEE 754 float</summary>
    Float,
    /// <summary>64-bit IEEE 754 double</summary>
    Double,
    /// <summary>UTF-8 string (also used for date, time, datetime, char, uuid, ipv4, decimal)</summary>
    String,
    /// <summary>Raw bytes</summary>
    Bytes
}

/// <summary>
/// A typed value that can be stored in a GenericRecord.
/// Equivalent to Rust's RecordValue enum.
///
/// This struct provides type-safe storage for record field values.
/// </summary>
public readonly struct RecordValue : IEquatable<RecordValue>
{
    private readonly RecordValueType _type;
    private readonly long _intValue;
    private readonly double _floatValue;
    private readonly object? _objectValue; // string or byte[]

    private RecordValue(RecordValueType type, long intValue = 0, double floatValue = 0, object? objectValue = null)
    {
        _type = type;
        _intValue = intValue;
        _floatValue = floatValue;
        _objectValue = objectValue;
    }

    /// <summary>Gets the type of this record value.</summary>
    public RecordValueType Type => _type;

    /// <summary>Returns true if this value is null.</summary>
    public bool IsNull => _type == RecordValueType.Null;

    #region Factory Methods

    /// <summary>Creates a null record value.</summary>
    public static RecordValue Null() => new(RecordValueType.Null);

    /// <summary>Creates a 32-bit integer record value.</summary>
    public static RecordValue Int(int value) => new(RecordValueType.Int, value);

    /// <summary>Creates a 64-bit integer record value.</summary>
    public static RecordValue Long(long value) => new(RecordValueType.Long, value);

    /// <summary>Creates a 32-bit float record value.</summary>
    public static RecordValue Float(float value) => new(RecordValueType.Float, floatValue: value);

    /// <summary>Creates a 64-bit double record value.</summary>
    public static RecordValue Double(double value) => new(RecordValueType.Double, floatValue: value);

    /// <summary>Creates a string record value.</summary>
    public static RecordValue String(string value) => new(RecordValueType.String, objectValue: value);

    /// <summary>Creates a bytes record value.</summary>
    public static RecordValue Bytes(byte[] value) => new(RecordValueType.Bytes, objectValue: value);

    /// <summary>Creates a boolean record value (stored as int).</summary>
    public static RecordValue Boolean(bool value) => new(RecordValueType.Int, value ? 1 : 0);

    #endregion

    #region Implicit Conversions

    /// <summary>Implicitly converts an <c>int</c> value to a <see cref="RecordValue"/>.</summary>
    public static implicit operator RecordValue(int value) => Int(value);
    /// <summary>Implicitly converts a <c>long</c> value to a <see cref="RecordValue"/>.</summary>
    public static implicit operator RecordValue(long value) => Long(value);
    /// <summary>Implicitly converts a <c>float</c> value to a <see cref="RecordValue"/>.</summary>
    public static implicit operator RecordValue(float value) => Float(value);
    /// <summary>Implicitly converts a <c>double</c> value to a <see cref="RecordValue"/>.</summary>
    public static implicit operator RecordValue(double value) => Double(value);
    /// <summary>Implicitly converts a <c>string</c> value to a <see cref="RecordValue"/>. A null input becomes a null record value.</summary>
    public static implicit operator RecordValue(string value) => value == null ? Null() : String(value);
    /// <summary>Implicitly converts a <c>byte[]</c> value to a <see cref="RecordValue"/>. A null input becomes a null record value.</summary>
    public static implicit operator RecordValue(byte[] value) => value == null ? Null() : Bytes(value);
    /// <summary>Implicitly converts a <c>bool</c> value to a <see cref="RecordValue"/>.</summary>
    public static implicit operator RecordValue(bool value) => Boolean(value);

    // Nullable conversions
    /// <summary>Implicitly converts an <c>int?</c> value to a <see cref="RecordValue"/>. A null input becomes a null record value.</summary>
    public static implicit operator RecordValue(int? value) => value.HasValue ? Int(value.Value) : Null();
    /// <summary>Implicitly converts a <c>long?</c> value to a <see cref="RecordValue"/>. A null input becomes a null record value.</summary>
    public static implicit operator RecordValue(long? value) => value.HasValue ? Long(value.Value) : Null();
    /// <summary>Implicitly converts a <c>float?</c> value to a <see cref="RecordValue"/>. A null input becomes a null record value.</summary>
    public static implicit operator RecordValue(float? value) => value.HasValue ? Float(value.Value) : Null();
    /// <summary>Implicitly converts a <c>double?</c> value to a <see cref="RecordValue"/>. A null input becomes a null record value.</summary>
    public static implicit operator RecordValue(double? value) => value.HasValue ? Double(value.Value) : Null();
    /// <summary>Implicitly converts a <c>bool?</c> value to a <see cref="RecordValue"/>. A null input becomes a null record value.</summary>
    public static implicit operator RecordValue(bool? value) => value.HasValue ? Boolean(value.Value) : Null();

    #endregion

    #region Value Accessors

    /// <summary>Gets the value as a 32-bit integer.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int? AsInt()
    {
        return _type switch
        {
            RecordValueType.Int => (int)_intValue,
            RecordValueType.Null => null,
            _ => throw new InvalidOperationException($"Cannot convert {_type} to Int")
        };
    }

    /// <summary>Gets the value as a 64-bit integer.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long? AsLong()
    {
        return _type switch
        {
            RecordValueType.Long => _intValue,
            RecordValueType.Int => _intValue,
            RecordValueType.Null => null,
            _ => throw new InvalidOperationException($"Cannot convert {_type} to Long")
        };
    }

    /// <summary>Gets the value as a 32-bit float.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float? AsFloat()
    {
        return _type switch
        {
            RecordValueType.Float => (float)_floatValue,
            RecordValueType.Null => null,
            _ => throw new InvalidOperationException($"Cannot convert {_type} to Float")
        };
    }

    /// <summary>Gets the value as a 64-bit double.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? AsDouble()
    {
        return _type switch
        {
            RecordValueType.Double => _floatValue,
            RecordValueType.Float => _floatValue,
            RecordValueType.Null => null,
            _ => throw new InvalidOperationException($"Cannot convert {_type} to Double")
        };
    }

    /// <summary>Gets the value as a string.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? AsString()
    {
        return _type switch
        {
            RecordValueType.String => (string?)_objectValue,
            RecordValueType.Null => null,
            _ => throw new InvalidOperationException($"Cannot convert {_type} to String")
        };
    }

    /// <summary>Gets the value as a byte array.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[]? AsBytes()
    {
        return _type switch
        {
            RecordValueType.Bytes => (byte[]?)_objectValue,
            RecordValueType.Null => null,
            _ => throw new InvalidOperationException($"Cannot convert {_type} to Bytes")
        };
    }

    /// <summary>Gets the value as a boolean (from int).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool? AsBool()
    {
        return _type switch
        {
            RecordValueType.Int => _intValue != 0,
            RecordValueType.Null => null,
            _ => throw new InvalidOperationException($"Cannot convert {_type} to Boolean")
        };
    }

    #endregion

    #region TryGet Methods

    /// <summary>Tries to get the value as an integer.</summary>
    public bool TryGetInt(out int value)
    {
        if (_type == RecordValueType.Int)
        {
            value = (int)_intValue;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>Tries to get the value as a long.</summary>
    public bool TryGetLong(out long value)
    {
        if (_type is RecordValueType.Long or RecordValueType.Int)
        {
            value = _intValue;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>Tries to get the value as a float.</summary>
    public bool TryGetFloat(out float value)
    {
        if (_type == RecordValueType.Float)
        {
            value = (float)_floatValue;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>Tries to get the value as a double.</summary>
    public bool TryGetDouble(out double value)
    {
        if (_type is RecordValueType.Double or RecordValueType.Float)
        {
            value = _floatValue;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>Tries to get the value as a string.</summary>
    public bool TryGetString(out string? value)
    {
        if (_type == RecordValueType.String)
        {
            value = (string?)_objectValue;
            return true;
        }
        if (_type == RecordValueType.Null)
        {
            value = null;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>Tries to get the value as bytes.</summary>
    public bool TryGetBytes(out byte[]? value)
    {
        if (_type == RecordValueType.Bytes)
        {
            value = (byte[]?)_objectValue;
            return true;
        }
        if (_type == RecordValueType.Null)
        {
            value = null;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>Tries to get the value as a boolean.</summary>
    public bool TryGetBool(out bool value)
    {
        if (_type == RecordValueType.Int)
        {
            value = _intValue != 0;
            return true;
        }
        value = default;
        return false;
    }

    #endregion

    #region Equality and Hashing

    /// <inheritdoc/>
    public bool Equals(RecordValue other)
    {
        if (_type != other._type) return false;

        return _type switch
        {
            RecordValueType.Null => true,
            RecordValueType.Int or RecordValueType.Long => _intValue == other._intValue,
            RecordValueType.Float or RecordValueType.Double => _floatValue == other._floatValue,
            RecordValueType.String => string.Equals((string?)_objectValue, (string?)other._objectValue, StringComparison.Ordinal),
            RecordValueType.Bytes => ByteArrayEquals((byte[]?)_objectValue, (byte[]?)other._objectValue),
            _ => false
        };
    }

    private static bool ByteArrayEquals(byte[]? a, byte[]? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null || b == null) return false;
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is RecordValue other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return _type switch
        {
            RecordValueType.Null => 0,
            RecordValueType.Int or RecordValueType.Long => HashCode.Combine(_type, _intValue),
            RecordValueType.Float or RecordValueType.Double => HashCode.Combine(_type, _floatValue),
            RecordValueType.String => HashCode.Combine(_type, _objectValue),
            RecordValueType.Bytes => HashCode.Combine(_type, ((byte[]?)_objectValue)?.Length ?? 0),
            _ => 0
        };
    }

    /// <summary>Determines whether two <see cref="RecordValue"/> instances are equal.</summary>
    public static bool operator ==(RecordValue left, RecordValue right) => left.Equals(right);
    /// <summary>Determines whether two <see cref="RecordValue"/> instances are not equal.</summary>
    public static bool operator !=(RecordValue left, RecordValue right) => !left.Equals(right);

    #endregion

    /// <inheritdoc/>
    public override string ToString()
    {
        return _type switch
        {
            RecordValueType.Null => "null",
            RecordValueType.Int or RecordValueType.Long => _intValue.ToString(),
            RecordValueType.Float or RecordValueType.Double => _floatValue.ToString(),
            RecordValueType.String => $"\"{_objectValue}\"",
            RecordValueType.Bytes => $"bytes[{((byte[]?)_objectValue)?.Length ?? 0}]",
            _ => "unknown"
        };
    }
}
