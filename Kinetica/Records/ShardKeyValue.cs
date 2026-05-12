using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace kinetica.Records;

/// <summary>
/// Represents the type of a shard key value.
    /// Equivalent to Rust's ShardKeyValue enum variants.
    /// </summary>
    public enum ShardKeyValueType
    {
        /// <summary>Null value</summary>
        Null,
        /// <summary>32-bit signed integer</summary>
        Int,
        /// <summary>64-bit signed integer</summary>
        Long,
        /// <summary>32-bit IEEE 754 float</summary>
        Float,
        /// <summary>64-bit IEEE 754 double</summary>
        Double,
        /// <summary>UTF-8 string (also used for char(N) types)</summary>
        String,
        /// <summary>Date in YYYY-MM-DD format</summary>
        Date,
        /// <summary>DateTime in YYYY-MM-DD HH:MM:SS.mmm format</summary>
        DateTime,
        /// <summary>Time in HH:MM:SS.mmm format</summary>
        Time,
        /// <summary>IPv4 address in dotted-quad format (e.g., "192.168.1.100")</summary>
        Ipv4,
        /// <summary>Decimal as string (e.g., "12345.6789")</summary>
        Decimal,
        /// <summary>Boolean value</summary>
        Boolean,
        /// <summary>8-bit signed integer (-128 to 127)</summary>
        Int8,
        /// <summary>16-bit signed integer (-32768 to 32767)</summary>
        Int16,
        /// <summary>Timestamp as milliseconds since Unix epoch</summary>
        Timestamp,
        /// <summary>UUID in standard format (e.g., "550e8400-e29b-41d4-a716-446655440000")</summary>
        Uuid,
        /// <summary>Unsigned long as string (0 to 18446744073709551615)</summary>
        Ulong
    }

    /// <summary>
    /// A typed value for shard key computation.
    /// Equivalent to Rust's ShardKeyValue enum.
    ///
    /// This struct provides type-safe storage for shard key values without
    /// requiring boxing or string parsing at routing time.
    /// </summary>
    public readonly struct ShardKeyValue : IEquatable<ShardKeyValue>
    {
        private readonly ShardKeyValueType _type;
        private readonly long _intValue;
        private readonly double _floatValue;
        private readonly string? _stringValue;

        private ShardKeyValue(ShardKeyValueType type, long intValue = 0, double floatValue = 0, string? stringValue = null)
        {
            _type = type;
            _intValue = intValue;
            _floatValue = floatValue;
            _stringValue = stringValue;
        }

        /// <summary>Gets the type of this shard key value.</summary>
        public ShardKeyValueType Type => _type;

        /// <summary>Returns true if this value is null.</summary>
        public bool IsNull => _type == ShardKeyValueType.Null;

        #region Factory Methods (Equivalent to Rust enum variants)

        /// <summary>Creates a null shard key value.</summary>
        public static ShardKeyValue Null() => new(ShardKeyValueType.Null);

        /// <summary>Creates a 32-bit integer shard key value.</summary>
        public static ShardKeyValue Int(int value) => new(ShardKeyValueType.Int, value);

        /// <summary>Creates a 64-bit integer shard key value.</summary>
        public static ShardKeyValue Long(long value) => new(ShardKeyValueType.Long, value);

        /// <summary>Creates a 32-bit float shard key value.</summary>
        public static ShardKeyValue Float(float value) => new(ShardKeyValueType.Float, floatValue: value);

        /// <summary>Creates a 64-bit double shard key value.</summary>
        public static ShardKeyValue Double(double value) => new(ShardKeyValueType.Double, floatValue: value);

        /// <summary>Creates a string shard key value.</summary>
        public static ShardKeyValue String(string value) => new(ShardKeyValueType.String, stringValue: value);

        /// <summary>Creates a date shard key value (YYYY-MM-DD format).</summary>
        public static ShardKeyValue Date(string value) => new(ShardKeyValueType.Date, stringValue: value);

        /// <summary>Creates a datetime shard key value (YYYY-MM-DD HH:MM:SS.mmm format).</summary>
        public static ShardKeyValue DateTime(string value) => new(ShardKeyValueType.DateTime, stringValue: value);

        /// <summary>Creates a time shard key value (HH:MM:SS.mmm format).</summary>
        public static ShardKeyValue Time(string value) => new(ShardKeyValueType.Time, stringValue: value);

        /// <summary>Creates an IPv4 shard key value (dotted-quad format).</summary>
        public static ShardKeyValue Ipv4(string value) => new(ShardKeyValueType.Ipv4, stringValue: value);

        /// <summary>Creates a decimal shard key value (string representation).</summary>
        public static ShardKeyValue Decimal(string value) => new(ShardKeyValueType.Decimal, stringValue: value);

        /// <summary>Creates a boolean shard key value.</summary>
        public static ShardKeyValue Boolean(bool value) => new(ShardKeyValueType.Boolean, value ? 1 : 0);

        /// <summary>Creates an 8-bit integer shard key value.</summary>
        public static ShardKeyValue Int8(sbyte value) => new(ShardKeyValueType.Int8, value);

        /// <summary>Creates a 16-bit integer shard key value.</summary>
        public static ShardKeyValue Int16(short value) => new(ShardKeyValueType.Int16, value);

        /// <summary>Creates a timestamp shard key value (milliseconds since Unix epoch).</summary>
        public static ShardKeyValue Timestamp(long value) => new(ShardKeyValueType.Timestamp, value);

        /// <summary>Creates a UUID shard key value (standard format).</summary>
        public static ShardKeyValue Uuid(string value) => new(ShardKeyValueType.Uuid, stringValue: value);

        /// <summary>Creates an unsigned long shard key value (string representation).</summary>
        public static ShardKeyValue Ulong(string value) => new(ShardKeyValueType.Ulong, stringValue: value);

        #endregion

        #region Value Accessors

        /// <summary>Gets the value as a 32-bit integer.</summary>
        /// <exception cref="InvalidOperationException">If the value type is not Int, Int8, Int16, or Boolean.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AsInt()
        {
            return _type switch
            {
                ShardKeyValueType.Int or ShardKeyValueType.Int8 or ShardKeyValueType.Int16 or ShardKeyValueType.Boolean
                    => (int)_intValue,
                _ => throw new InvalidOperationException($"Cannot convert {_type} to Int")
            };
        }

        /// <summary>Gets the value as a 64-bit integer.</summary>
        /// <exception cref="InvalidOperationException">If the value type is not numeric.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long AsLong()
        {
            return _type switch
            {
                ShardKeyValueType.Long or ShardKeyValueType.Timestamp or
                ShardKeyValueType.Int or ShardKeyValueType.Int8 or ShardKeyValueType.Int16 or ShardKeyValueType.Boolean
                    => _intValue,
                _ => throw new InvalidOperationException($"Cannot convert {_type} to Long")
            };
        }

        /// <summary>Gets the value as a 32-bit float.</summary>
        /// <exception cref="InvalidOperationException">If the value type is not Float.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float AsFloat()
        {
            return _type switch
            {
                ShardKeyValueType.Float => (float)_floatValue,
                _ => throw new InvalidOperationException($"Cannot convert {_type} to Float")
            };
        }

        /// <summary>Gets the value as a 64-bit double.</summary>
        /// <exception cref="InvalidOperationException">If the value type is not Double or Float.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double AsDouble()
        {
            return _type switch
            {
                ShardKeyValueType.Double or ShardKeyValueType.Float => _floatValue,
                _ => throw new InvalidOperationException($"Cannot convert {_type} to Double")
            };
        }

        /// <summary>Gets the value as a string.</summary>
        /// <exception cref="InvalidOperationException">If the value type is not a string-based type.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? AsString()
        {
            return _type switch
            {
                ShardKeyValueType.String or ShardKeyValueType.Date or ShardKeyValueType.DateTime or
                ShardKeyValueType.Time or ShardKeyValueType.Ipv4 or ShardKeyValueType.Decimal or
                ShardKeyValueType.Uuid or ShardKeyValueType.Ulong
                    => _stringValue,
                ShardKeyValueType.Null => null,
                _ => throw new InvalidOperationException($"Cannot convert {_type} to String")
            };
        }

        /// <summary>Gets the value as a boolean.</summary>
        /// <exception cref="InvalidOperationException">If the value type is not Boolean.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AsBool()
        {
            return _type switch
            {
                ShardKeyValueType.Boolean => _intValue != 0,
                _ => throw new InvalidOperationException($"Cannot convert {_type} to Boolean")
            };
        }

        /// <summary>Tries to get the value as an integer.</summary>
        public bool TryGetInt(out int value)
        {
            if (_type is ShardKeyValueType.Int or ShardKeyValueType.Int8 or ShardKeyValueType.Int16 or ShardKeyValueType.Boolean)
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
            if (_type is ShardKeyValueType.Long or ShardKeyValueType.Timestamp or
                ShardKeyValueType.Int or ShardKeyValueType.Int8 or ShardKeyValueType.Int16 or ShardKeyValueType.Boolean)
            {
                value = _intValue;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>Tries to get the value as a string.</summary>
        public bool TryGetString(out string? value)
        {
            if (_type is ShardKeyValueType.String or ShardKeyValueType.Date or ShardKeyValueType.DateTime or
                ShardKeyValueType.Time or ShardKeyValueType.Ipv4 or ShardKeyValueType.Decimal or
                ShardKeyValueType.Uuid or ShardKeyValueType.Ulong or ShardKeyValueType.Null)
            {
                value = _stringValue;
                return true;
            }
            value = default;
            return false;
        }

        #endregion

        #region Equality and Hashing

        public bool Equals(ShardKeyValue other)
        {
            if (_type != other._type) return false;

            return _type switch
            {
                ShardKeyValueType.Null => true,
                ShardKeyValueType.Int or ShardKeyValueType.Long or ShardKeyValueType.Int8 or
                ShardKeyValueType.Int16 or ShardKeyValueType.Boolean or ShardKeyValueType.Timestamp
                    => _intValue == other._intValue,
                ShardKeyValueType.Float or ShardKeyValueType.Double
                    => _floatValue == other._floatValue,
                _ => string.Equals(_stringValue, other._stringValue, StringComparison.Ordinal)
            };
        }

        public override bool Equals(object? obj) => obj is ShardKeyValue other && Equals(other);

        public override int GetHashCode()
        {
            return _type switch
            {
                ShardKeyValueType.Null => 0,
                ShardKeyValueType.Int or ShardKeyValueType.Long or ShardKeyValueType.Int8 or
                ShardKeyValueType.Int16 or ShardKeyValueType.Boolean or ShardKeyValueType.Timestamp
                    => HashCode.Combine(_type, _intValue),
                ShardKeyValueType.Float or ShardKeyValueType.Double
                    => HashCode.Combine(_type, _floatValue),
                _ => HashCode.Combine(_type, _stringValue)
            };
        }

        public static bool operator ==(ShardKeyValue left, ShardKeyValue right) => left.Equals(right);
        public static bool operator !=(ShardKeyValue left, ShardKeyValue right) => !left.Equals(right);

        #endregion

        public override string ToString()
        {
            return _type switch
            {
                ShardKeyValueType.Null => "null",
                ShardKeyValueType.Int or ShardKeyValueType.Long or ShardKeyValueType.Int8 or
                ShardKeyValueType.Int16 or ShardKeyValueType.Timestamp
                    => _intValue.ToString(),
                ShardKeyValueType.Boolean => (_intValue != 0).ToString(),
                ShardKeyValueType.Float or ShardKeyValueType.Double
                    => _floatValue.ToString(),
                _ => _stringValue ?? "null"
            };
        }
    }

    /// <summary>
    /// Collection of shard key column names and values.
    /// Equivalent to Rust's ShardKeyValues (SmallVec&lt;[(&amp;'static str, ShardKeyValue); 4]&gt;).
    ///
    /// This struct is optimized for small collections (up to 4 shard key columns)
    /// which is the common case.
    /// </summary>
    public readonly struct ShardKeyValues : IReadOnlyList<(string Name, ShardKeyValue Value)>
    {
        private readonly (string Name, ShardKeyValue Value)[]? _values;

        /// <summary>Creates an empty ShardKeyValues collection.</summary>
        public static ShardKeyValues Empty => new(Array.Empty<(string, ShardKeyValue)>());

        /// <summary>Creates a ShardKeyValues collection from the specified values.</summary>
        public ShardKeyValues(params (string Name, ShardKeyValue Value)[] values)
        {
            _values = values;
        }

        /// <summary>Creates a ShardKeyValues collection with a single value.</summary>
        public static ShardKeyValues Single(string name, ShardKeyValue value)
            => new((name, value));

        /// <summary>Creates a ShardKeyValues collection with two values.</summary>
        public static ShardKeyValues Two(string name1, ShardKeyValue value1, string name2, ShardKeyValue value2)
            => new((name1, value1), (name2, value2));

        /// <summary>Creates a ShardKeyValues collection with three values.</summary>
        public static ShardKeyValues Three(
            string name1, ShardKeyValue value1,
            string name2, ShardKeyValue value2,
            string name3, ShardKeyValue value3)
            => new((name1, value1), (name2, value2), (name3, value3));

        /// <summary>Creates a ShardKeyValues collection with four values.</summary>
        public static ShardKeyValues Four(
            string name1, ShardKeyValue value1,
            string name2, ShardKeyValue value2,
            string name3, ShardKeyValue value3,
            string name4, ShardKeyValue value4)
            => new((name1, value1), (name2, value2), (name3, value3), (name4, value4));

        /// <summary>Gets the number of shard key values.</summary>
        public int Count => _values?.Length ?? 0;

        /// <summary>Gets the shard key value at the specified index.</summary>
        public (string Name, ShardKeyValue Value) this[int index]
        {
            get
            {
                if (_values == null || index < 0 || index >= _values.Length)
                    throw new IndexOutOfRangeException();
                return _values[index];
            }
        }

        /// <summary>Gets an enumerator for the shard key values.</summary>
        public IEnumerator<(string Name, ShardKeyValue Value)> GetEnumerator()
        {
            if (_values == null)
                yield break;

            foreach (var value in _values)
                yield return value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
