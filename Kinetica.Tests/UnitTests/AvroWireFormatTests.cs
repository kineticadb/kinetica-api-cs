using System;
using System.Linq;
using Xunit;

namespace Kinetica.Tests.UnitTests
{
    /// <summary>
    /// Wire-format pinning tests for Avro encoding.
    ///
    /// These tests verify the exact binary encoding of Avro data to ensure
    /// backward compatibility. Any changes to the wire format will break these
    /// tests, which is intentional - wire format changes require careful consideration.
    ///
    /// Test vectors are pinned (hardcoded expected bytes) to detect regressions.
    /// </summary>
    [Trait("Category", "WireFormat")]
    public class AvroWireFormatTests
    {
        #region Varint Encoding Tests

        /// <summary>
        /// Tests Avro varint zig-zag encoding for positive integers.
        /// Avro uses zig-zag encoding: (n << 1) ^ (n >> 63)
        /// Then encodes as base-128 varint with continuation bits.
        /// </summary>
        [Theory]
        [InlineData(0, new byte[] { 0x00 })]                    // 0 -> zig-zag: 0 -> 0x00
        [InlineData(1, new byte[] { 0x02 })]                    // 1 -> zig-zag: 2 -> 0x02
        [InlineData(-1, new byte[] { 0x01 })]                   // -1 -> zig-zag: 1 -> 0x01
        [InlineData(2, new byte[] { 0x04 })]                    // 2 -> zig-zag: 4 -> 0x04
        [InlineData(-2, new byte[] { 0x03 })]                   // -2 -> zig-zag: 3 -> 0x03
        [InlineData(63, new byte[] { 0x7E })]                   // 63 -> zig-zag: 126 -> 0x7E
        [InlineData(64, new byte[] { 0x80, 0x01 })]             // 64 -> zig-zag: 128 -> needs 2 bytes
        [InlineData(-64, new byte[] { 0x7F })]                  // -64 -> zig-zag: 127 -> 0x7F
        [InlineData(127, new byte[] { 0xFE, 0x01 })]            // 127 -> zig-zag: 254 -> 0xFE,0x01
        [InlineData(-128, new byte[] { 0xFF, 0x01 })]           // -128 -> zig-zag: 255 -> 0xFF,0x01
        [InlineData(8191, new byte[] { 0xFE, 0x7F })]           // 8191 -> needs 2 bytes
        [InlineData(8192, new byte[] { 0x80, 0x80, 0x01 })]     // 8192 -> needs 3 bytes
        [InlineData(int.MaxValue, new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x0F })] // Max int
        [InlineData(int.MinValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })] // Min int
        public void AvroVarint_EncodesCorrectly(int value, byte[] expectedBytes)
        {
            // Arrange
            byte[] buffer = new byte[10];
            int position = 0;

            // Act
            position = kinetica.AvroEncoding.WriteVarInt(buffer, position, value);

            // Assert
            byte[] actualBytes = buffer.Take(position).ToArray();
            Assert.Equal(expectedBytes, actualBytes);
        }

        /// <summary>
        /// Tests Avro varint encoding for long values.
        /// </summary>
        [Theory]
        [InlineData(0L, new byte[] { 0x00 })]
        [InlineData(1L, new byte[] { 0x02 })]
        [InlineData(-1L, new byte[] { 0x01 })]
        [InlineData(long.MaxValue, new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
        [InlineData(long.MinValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
        public void AvroVarintLong_EncodesCorrectly(long value, byte[] expectedBytes)
        {
            // Arrange
            byte[] buffer = new byte[10];
            int position = 0;

            // Act
            position = kinetica.AvroEncoding.WriteVarLong(buffer, position, value);

            // Assert
            byte[] actualBytes = buffer.Take(position).ToArray();
            Assert.Equal(expectedBytes, actualBytes);
        }

        #endregion

        #region Simple Type Encoding Tests

        /// <summary>
        /// Test record for simple types.
        /// </summary>
        public class SimpleRecord
        {
            public int IntField { get; set; }
            public long LongField { get; set; }
            public float FloatField { get; set; }
            public double DoubleField { get; set; }
            public bool BoolField { get; set; }
            public string StringField { get; set; } = "";
        }

        /// <summary>
        /// Tests encoding of a simple record with all basic types.
        /// Wire format: int(varint) + long(varint) + float(4 bytes LE) + double(8 bytes LE) + bool(1 byte) + string(varint len + UTF-8)
        /// </summary>
        [Fact]
        public void SimpleRecord_WithBasicValues_EncodesCorrectly()
        {
            // Arrange: Create a record with known values
            var record = new SimpleRecord
            {
                IntField = 42,          // zig-zag: 84 -> 0x54
                LongField = 1000,       // zig-zag: 2000 -> 0xD0, 0x0F
                FloatField = 3.14f,     // IEEE 754 single precision LE
                DoubleField = 2.718,    // IEEE 754 double precision LE
                BoolField = true,       // 0x01
                StringField = "test"    // len=4: 0x08 + "test"
            };

            // Expected bytes:
            // Int: 42 -> 0x54
            // Long: 1000 -> 0xD0, 0x0F
            // Float: 3.14f -> 0xC3, 0xF5, 0x48, 0x40 (little-endian)
            // Double: 2.718 -> 0x58, 0x39, 0xB4, 0xC8, 0x76, 0xBE, 0x05, 0x40 (little-endian)
            // Bool: true -> 0x01
            // String: "test" -> length 4 -> 0x08, then UTF-8 bytes 0x74, 0x65, 0x73, 0x74
            byte[] expectedBytes =
            {
                0x54,                                           // int: 42
                0xD0, 0x0F,                                     // long: 1000
                0xC3, 0xF5, 0x48, 0x40,                        // float: 3.14
                0x58, 0x39, 0xB4, 0xC8, 0x76, 0xBE, 0x05, 0x40, // double: 2.718
                0x01,                                           // bool: true
                0x08, 0x74, 0x65, 0x73, 0x74                   // string: "test"
            };

            // Create KineticaType for the record
            var ktype = CreateSimpleRecordType();

            // Act
            var encoder = kinetica.DirectAvroEncoder<SimpleRecord>.GetOrCreate(ktype);
            byte[] actualBytes = encoder.Encode(record);

            // Assert
            Assert.Equal(expectedBytes, actualBytes);
        }

        /// <summary>
        /// Tests encoding of a record with zero/empty values.
        /// </summary>
        [Fact]
        public void SimpleRecord_WithZeroValues_EncodesCorrectly()
        {
            // Arrange
            var record = new SimpleRecord
            {
                IntField = 0,
                LongField = 0,
                FloatField = 0.0f,
                DoubleField = 0.0,
                BoolField = false,
                StringField = ""
            };

            // Expected bytes:
            // Int: 0 -> 0x00
            // Long: 0 -> 0x00
            // Float: 0.0f -> 0x00, 0x00, 0x00, 0x00
            // Double: 0.0 -> 0x00 * 8
            // Bool: false -> 0x00
            // String: "" -> length 0 -> 0x00
            byte[] expectedBytes =
            {
                0x00,                                           // int: 0
                0x00,                                           // long: 0
                0x00, 0x00, 0x00, 0x00,                        // float: 0.0
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // double: 0.0
                0x00,                                           // bool: false
                0x00                                            // string: ""
            };

            var ktype = CreateSimpleRecordType();

            // Act
            var encoder = kinetica.DirectAvroEncoder<SimpleRecord>.GetOrCreate(ktype);
            byte[] actualBytes = encoder.Encode(record);

            // Assert
            Assert.Equal(expectedBytes, actualBytes);
        }

        /// <summary>
        /// Tests encoding of negative integers.
        /// </summary>
        [Fact]
        public void SimpleRecord_WithNegativeValues_EncodesCorrectly()
        {
            // Arrange
            var record = new SimpleRecord
            {
                IntField = -42,         // zig-zag: 83 -> 0x53
                LongField = -1000,      // zig-zag: 1999 -> 0xCF, 0x0F
                FloatField = -3.14f,
                DoubleField = -2.718,
                BoolField = false,
                StringField = ""
            };

            // Expected bytes for negative values
            byte[] expectedBytes =
            {
                0x53,                                           // int: -42
                0xCF, 0x0F,                                     // long: -1000
                0xC3, 0xF5, 0x48, 0xC0,                        // float: -3.14
                0x58, 0x39, 0xB4, 0xC8, 0x76, 0xBE, 0x05, 0xC0, // double: -2.718
                0x00,                                           // bool: false
                0x00                                            // string: ""
            };

            var ktype = CreateSimpleRecordType();

            // Act
            var encoder = kinetica.DirectAvroEncoder<SimpleRecord>.GetOrCreate(ktype);
            byte[] actualBytes = encoder.Encode(record);

            // Assert
            Assert.Equal(expectedBytes, actualBytes);
        }

        /// <summary>
        /// Tests string encoding with UTF-8 multi-byte characters.
        /// </summary>
        [Fact]
        public void String_WithUTF8_EncodesCorrectly()
        {
            // Arrange
            var record = new SimpleRecord
            {
                StringField = "Hello™" // ™ is UTF-8: 0xE2 0x84 0xA2 (3 bytes)
            };

            // "Hello" = 5 bytes + "™" = 3 bytes = 8 bytes total
            // Length prefix: 8 -> zig-zag: 16 -> 0x10
            byte[] expectedStringBytes =
            {
                0x10,                                           // length: 8
                0x48, 0x65, 0x6C, 0x6C, 0x6F,                  // "Hello"
                0xE2, 0x84, 0xA2                               // "™"
            };

            // Full record with defaults for other fields
            byte[] expectedBytes =
            {
                0x00,                                           // int: 0
                0x00,                                           // long: 0
                0x00, 0x00, 0x00, 0x00,                        // float: 0.0
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // double: 0.0
                0x00,                                           // bool: false
                0x10, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xE2, 0x84, 0xA2 // "Hello™"
            };

            var ktype = CreateSimpleRecordType();

            // Act
            var encoder = kinetica.DirectAvroEncoder<SimpleRecord>.GetOrCreate(ktype);
            byte[] actualBytes = encoder.Encode(record);

            // Assert
            Assert.Equal(expectedBytes, actualBytes);
        }

        #endregion

        #region Nullable Type Encoding Tests

        /// <summary>
        /// Test record with nullable fields.
        /// </summary>
        public class NullableRecord
        {
            public int? NullableInt { get; set; }
            public long? NullableLong { get; set; }
            public string? NullableString { get; set; }
        }

        /// <summary>
        /// Tests encoding of nullable fields with null values.
        /// Avro union encoding: null is union index 0, non-null is union index 1 (followed by value).
        /// </summary>
        [Fact]
        public void NullableRecord_WithNulls_EncodesCorrectly()
        {
            // Arrange
            var record = new NullableRecord
            {
                NullableInt = null,
                NullableLong = null,
                NullableString = null
            };

            // Expected: Each null field is encoded as union index 0 (null branch)
            // Union index is encoded as varint: 0 -> 0x00
            byte[] expectedBytes =
            {
                0x00,  // NullableInt: union index 0 (null)
                0x00,  // NullableLong: union index 0 (null)
                0x00   // NullableString: union index 0 (null)
            };

            var ktype = CreateNullableRecordType();

            // Act
            var encoder = kinetica.DirectAvroEncoder<NullableRecord>.GetOrCreate(ktype);
            byte[] actualBytes = encoder.Encode(record);

            // Assert
            Assert.Equal(expectedBytes, actualBytes);
        }

        /// <summary>
        /// Tests encoding of nullable fields with non-null values.
        /// </summary>
        [Fact]
        public void NullableRecord_WithValues_EncodesCorrectly()
        {
            // Arrange
            var record = new NullableRecord
            {
                NullableInt = 42,
                NullableLong = 1000,
                NullableString = "test"
            };

            // Expected: Each non-null field is union index 1 (non-null branch) followed by the value
            // Union index 1 -> 0x02 (varint encoded)
            byte[] expectedBytes =
            {
                0x02, 0x54,                         // NullableInt: union index 1, value 42
                0x02, 0xD0, 0x0F,                   // NullableLong: union index 1, value 1000
                0x02, 0x08, 0x74, 0x65, 0x73, 0x74  // NullableString: union index 1, "test"
            };

            var ktype = CreateNullableRecordType();

            // Act
            var encoder = kinetica.DirectAvroEncoder<NullableRecord>.GetOrCreate(ktype);
            byte[] actualBytes = encoder.Encode(record);

            // Assert
            Assert.Equal(expectedBytes, actualBytes);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a KineticaType for SimpleRecord.
        /// </summary>
        private kinetica.KineticaType CreateSimpleRecordType()
        {
            string schemaJson = @"{
                ""type"": ""record"",
                ""name"": ""SimpleRecord"",
                ""fields"": [
                    {""name"": ""IntField"", ""type"": ""int""},
                    {""name"": ""LongField"", ""type"": ""long""},
                    {""name"": ""FloatField"", ""type"": ""float""},
                    {""name"": ""DoubleField"", ""type"": ""double""},
                    {""name"": ""BoolField"", ""type"": ""boolean""},
                    {""name"": ""StringField"", ""type"": ""string""}
                ]
            }";

            return new kinetica.KineticaType(schemaJson);
        }

        /// <summary>
        /// Creates a KineticaType for NullableRecord.
        /// </summary>
        private kinetica.KineticaType CreateNullableRecordType()
        {
            string schemaJson = @"{
                ""type"": ""record"",
                ""name"": ""NullableRecord"",
                ""fields"": [
                    {""name"": ""NullableInt"", ""type"": [""null"", ""int""]},
                    {""name"": ""NullableLong"", ""type"": [""null"", ""long""]},
                    {""name"": ""NullableString"", ""type"": [""null"", ""string""]}
                ]
            }";

            return new kinetica.KineticaType(schemaJson);
        }

        #endregion

        #region Regression Tests

        /// <summary>
        /// Regression test: Large varint values (ensures multi-byte encoding).
        /// </summary>
        [Theory]
        [InlineData(16383, new byte[] { 0xFE, 0xFF, 0x01 })]    // 3 bytes
        [InlineData(2097151, new byte[] { 0xFE, 0xFF, 0xFF, 0x01 })]  // 4 bytes (0x1FFFFF)
        [InlineData(2097152, new byte[] { 0x80, 0x80, 0x80, 0x02 })] // 4 bytes (0x200000)
        public void LargeVarint_EncodesWithMultipleBytes(int value, byte[] expectedBytes)
        {
            // Arrange
            byte[] buffer = new byte[10];

            // Act
            int position = kinetica.AvroEncoding.WriteVarInt(buffer, 0, value);

            // Assert
            byte[] actualBytes = buffer.Take(position).ToArray();
            Assert.Equal(expectedBytes, actualBytes);
        }

        /// <summary>
        /// Regression test: Empty string encoding.
        /// </summary>
        [Fact]
        public void EmptyString_EncodesAsZeroLength()
        {
            // Arrange
            var record = new SimpleRecord { StringField = "" };
            var ktype = CreateSimpleRecordType();

            // Act
            var encoder = kinetica.DirectAvroEncoder<SimpleRecord>.GetOrCreate(ktype);
            byte[] encoded = encoder.Encode(record);

            // Assert: Last byte should be 0x00 (empty string length)
            Assert.Equal(0x00, encoded[^1]);
        }

        /// <summary>
        /// Regression test: Very long string (tests buffer resizing).
        /// </summary>
        [Fact]
        public void LongString_EncodesCorrectly()
        {
            // Arrange: String of 1000 'A' characters
            var longString = new string('A', 1000);
            var record = new SimpleRecord { StringField = longString };
            var ktype = CreateSimpleRecordType();

            // Act
            var encoder = kinetica.DirectAvroEncoder<SimpleRecord>.GetOrCreate(ktype);
            byte[] encoded = encoder.Encode(record);

            // Assert: Check that length is encoded correctly
            // 1000 -> zig-zag: 2000 -> 0xD0, 0x0F
            // Find the string part (after all fixed fields)
            // int(0)=1 byte + long(0)=1 byte + float(0.0)=4 bytes + double(0.0)=8 bytes + bool(false)=1 byte = 15 bytes
            int stringStart = 15;
            Assert.Equal(0xD0, encoded[stringStart]);
            Assert.Equal(0x0F, encoded[stringStart + 1]);

            // Verify total length: stringStart + 2 (length varint) + 1000 (content)
            Assert.Equal(stringStart + 2 + 1000, encoded.Length);
        }

        #endregion
    }
}
