using Newtonsoft.Json;
using Xunit;
using kinetica;

namespace Kinetica.Tests
{
    /// <summary>
    /// Tests for various data type handling in Kinetica.
    /// </summary>
    [Trait("Category", "Integration")]
    public class DataTypeTests
    {
        private const string ConnectionUrl = "http://localhost:9191";
        private const string Username = "admin";
        private const string Password = "secret";
        private kinetica.Kinetica _kdb = null!;

        private kinetica.Kinetica GetConnection()
        {
            if (_kdb == null)
            {
                _kdb = new kinetica.Kinetica(ConnectionUrl, new kinetica.Kinetica.Options()
                {
                    Username = Username,
                    Password = Password
                });
            }
            return _kdb;
        }

        #region Boolean Type Tests

        [Fact]
        public void Boolean_StoredAsInteger_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "bool_type_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, active BOOLEAN, PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, TRUE), (2, FALSE)");

                var response = kdb.executeSql($"SELECT * FROM {tableName} ORDER BY id");

                Assert.Equal(2, response.total_number_of_records);

                var record1 = response.data[0];
                record1.TryGetValue("active", out var active1);
                Assert.Equal(1, Convert.ToInt32(active1));
                Assert.True(Convert.ToBoolean(active1));

                var record2 = response.data[1];
                record2.TryGetValue("active", out var active2);
                Assert.Equal(0, Convert.ToInt32(active2));
                Assert.False(Convert.ToBoolean(active2));
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        #endregion

        #region Array Type Tests

        [Fact]
        public void Array_IntegerArray_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "array_int_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // Use INTEGER[N] syntax for array columns
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, int_values INTEGER[5], PRIMARY KEY(id))");
                // Insert array values using JSON syntax
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, '[1, 2, 3, 4, 5]')");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("int_values", out var valuesObj);

                // Array is stored as JSON string
                var valuesStr = valuesObj?.ToString();
                Assert.NotNull(valuesStr);

                // Parse the JSON array
                var values = JsonConvert.DeserializeObject<int[]>(valuesStr!);
                Assert.NotNull(values);
                Assert.Equal(5, values!.Length);
                Assert.Equal(new[] { 1, 2, 3, 4, 5 }, values);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        [Fact]
        public void Array_DoubleArray_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "array_double_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // Use DOUBLE[N] syntax for array columns
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, double_values DOUBLE[3], PRIMARY KEY(id))");
                // Insert array values using JSON syntax
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, '[1.1, 2.2, 3.3]')");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("double_values", out var valuesObj);

                var valuesStr = valuesObj?.ToString();
                Assert.NotNull(valuesStr);

                var values = JsonConvert.DeserializeObject<double[]>(valuesStr!);
                Assert.NotNull(values);
                Assert.Equal(3, values!.Length);
                Assert.Equal(1.1, values[0], 5);
                Assert.Equal(2.2, values[1], 5);
                Assert.Equal(3.3, values[2], 5);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        [Fact(Skip = "VARCHAR arrays require specific Kinetica version support - test with CHAR16 instead")]
        public void Array_StringArray_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "array_string_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // Note: VARCHAR arrays may not be supported in all Kinetica versions
                // Use CHAR16[N] for fixed-length string arrays
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, str_tags CHAR16[3], PRIMARY KEY(id))");
                // Insert array values using JSON syntax
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, '[\"red\", \"green\", \"blue\"]')");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("str_tags", out var tagsObj);

                var tagsStr = tagsObj?.ToString();
                Assert.NotNull(tagsStr);

                var tags = JsonConvert.DeserializeObject<string[]>(tagsStr!);
                Assert.NotNull(tags);
                Assert.Equal(3, tags!.Length);
                // CHAR16 pads with spaces, so trim them
                var trimmedTags = tags!.Select(t => t?.Trim()).ToArray();
                Assert.Equal(new[] { "red", "green", "blue" }, trimmedTags);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        #endregion

        #region Decimal Type Tests

        [Fact]
        public void Decimal_8Byte_DefaultPrecision_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "decimal_8byte_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // DECIMAL(18,4) uses 8 bytes (precision <= 18)
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, amount DECIMAL(18,4), PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, 12345.6789)");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("amount", out var amountObj);

                // Decimal is stored as a string
                var amountStr = amountObj?.ToString();
                Assert.NotNull(amountStr);

                var amount = decimal.Parse(amountStr!);
                // DECIMAL(18,4) - 4 decimal places
                Assert.Equal(12345.6789m, amount);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        [Fact]
        public void Decimal_8Byte_CustomPrecisionScale_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "decimal_8byte_custom_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // DECIMAL(10,2) uses 8 bytes (precision <= 18)
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, price DECIMAL(10,2), PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, 99999999.99)");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("price", out var priceObj);

                var priceStr = priceObj?.ToString();
                Assert.NotNull(priceStr);

                var price = decimal.Parse(priceStr!);
                Assert.Equal(99999999.99m, price);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        [Fact]
        public void Decimal_12Byte_HighPrecision_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "decimal_12byte_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // DECIMAL(26,13) uses 12 bytes (precision > 18)
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, big_amount DECIMAL(26,13), PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, 1234567890123.1234567890123)");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("big_amount", out var amountObj);

                var amountStr = amountObj?.ToString();
                Assert.NotNull(amountStr);

                var amount = decimal.Parse(amountStr!);
                // Verify the high-precision decimal was stored and retrieved correctly
                Assert.True(Math.Abs(amount - 1234567890123.1234567890123m) < 0.0000000001m);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        [Fact]
        public void Decimal_12Byte_NearMaxValue_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "decimal_12byte_max_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // DECIMAL(26,13) for large values
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, big_val DECIMAL(26,13), PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, 9999999999999.9999999999999)");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("big_val", out var valObj);

                var valStr = valObj?.ToString();
                Assert.NotNull(valStr);

                var val = decimal.Parse(valStr!);
                Assert.True(Math.Abs(val - 9999999999999.9999999999999m) < 0.0000000001m);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        [Fact]
        public void Decimal_12Byte_NegativeValue_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "decimal_12byte_neg_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // DECIMAL(26,13) for negative values
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, neg_val DECIMAL(26,13), PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, -1234567890123.1234567890123)");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("neg_val", out var valObj);

                var valStr = valObj?.ToString();
                Assert.NotNull(valStr);

                var val = decimal.Parse(valStr!);
                Assert.True(Math.Abs(val - (-1234567890123.1234567890123m)) < 0.0000000001m);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        [Fact]
        public void Decimal_TypeFromTable_ExtractsPrecisionAndScale()
        {
            var kdb = GetConnection();
            const string tableName = "decimal_typeinfo_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // Create table with both 8-byte and 12-byte decimal columns
                kdb.executeSql($@"CREATE TABLE {tableName} (
                    id INT,
                    small_decimal DECIMAL(10,2),
                    default_decimal DECIMAL(18,4),
                    big_decimal DECIMAL(26,13),
                    PRIMARY KEY(id))");

                var ktype = KineticaType.fromTable(kdb, tableName);
                var columns = ktype.getColumns();

                // Check small_decimal (8-byte, precision=10, scale=2)
                var smallDecCol = columns.FirstOrDefault(c => c.getName() == "small_decimal");
                Assert.NotNull(smallDecCol);
                Assert.True(smallDecCol!.isDecimal());
                Assert.Equal(10, smallDecCol.getDecimalPrecision());
                Assert.Equal(2, smallDecCol.getDecimalScale());
                Assert.Equal(8, smallDecCol.getDecimalByteSize());

                // Check default_decimal (8-byte, precision=18, scale=4)
                var defaultDecCol = columns.FirstOrDefault(c => c.getName() == "default_decimal");
                Assert.NotNull(defaultDecCol);
                Assert.True(defaultDecCol!.isDecimal());
                Assert.Equal(18, defaultDecCol.getDecimalPrecision());
                Assert.Equal(4, defaultDecCol.getDecimalScale());
                Assert.Equal(8, defaultDecCol.getDecimalByteSize());

                // Check big_decimal (12-byte, precision=26, scale=13)
                var bigDecCol = columns.FirstOrDefault(c => c.getName() == "big_decimal");
                Assert.NotNull(bigDecCol);
                Assert.True(bigDecCol!.isDecimal());
                Assert.Equal(26, bigDecCol.getDecimalPrecision());
                Assert.Equal(13, bigDecCol.getDecimalScale());
                Assert.Equal(12, bigDecCol.getDecimalByteSize());
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        [Fact]
        public void Decimal_ColumnConstants_AreCorrect()
        {
            // Verify the decimal constants match the Java API
            Assert.Equal(18, KineticaType.Column.DEFAULT_DECIMAL_PRECISION);
            Assert.Equal(4, KineticaType.Column.DEFAULT_DECIMAL_SCALE);
            Assert.Equal(18, KineticaType.Column.DECIMAL8_MAX_PRECISION);
        }

        #endregion

        #region WKT Geometry Type Tests

        [Fact]
        public void WKT_Point_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "wkt_point_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, location GEOMETRY, PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, ST_GEOMFROMTEXT('POINT(-122.4194 37.7749)'))");

                var response = kdb.executeSql($"SELECT id, ST_ASTEXT(location) as location_wkt FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("location_wkt", out var wktObj);

                var wkt = wktObj?.ToString();
                Assert.NotNull(wkt);
                Assert.Contains("POINT", wkt!, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("-122.4194", wkt!);
                Assert.Contains("37.7749", wkt!);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        [Fact]
        public void WKT_Polygon_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "wkt_polygon_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, boundary GEOMETRY, PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, ST_GEOMFROMTEXT('POLYGON((0 0, 10 0, 10 10, 0 10, 0 0))'))");

                var response = kdb.executeSql($"SELECT id, ST_ASTEXT(boundary) as boundary_wkt FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("boundary_wkt", out var wktObj);

                var wkt = wktObj?.ToString();
                Assert.NotNull(wkt);
                Assert.Contains("POLYGON", wkt!, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        #endregion

        #region UUID Type Tests

        [Fact]
        public void UUID_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "uuid_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, uuid_val UUID, PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, '550e8400-e29b-41d4-a716-446655440000')");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("uuid_val", out var uuidObj);

                var uuidStr = uuidObj?.ToString();
                Assert.NotNull(uuidStr);

                var uuid = Guid.Parse(uuidStr!);
                Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), uuid);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        #endregion

        #region JSON Type Tests

        [Fact]
        public void JSON_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "json_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, data JSON, PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, '{{\"name\": \"John\", \"age\": 30}}')");

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("data", out var dataObj);

                var jsonStr = dataObj?.ToString();
                Assert.NotNull(jsonStr);

                dynamic? json = JsonConvert.DeserializeObject(jsonStr!);
                Assert.NotNull(json);
                Assert.Equal("John", (string)json!.name);
                Assert.Equal(30, (int)json!.age);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        #endregion

        #region ULONG Type Tests

        [Fact]
        public void ULONG_ReadsCorrectly()
        {
            var kdb = GetConnection();
            const string tableName = "ulong_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // Use UNSIGNED BIGINT syntax for unsigned long
                kdb.executeSql($"CREATE TABLE {tableName} (id INT, big_val UNSIGNED BIGINT, PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {tableName} VALUES (1, 18446744073709551615)");  // Max ULONG

                var response = kdb.executeSql($"SELECT * FROM {tableName}");

                Assert.Equal(1, response.total_number_of_records);

                var record = response.data[0];
                record.TryGetValue("big_val", out var valObj);

                var valStr = valObj?.ToString();
                Assert.NotNull(valStr);

                var val = ulong.Parse(valStr!);
                Assert.Equal(ulong.MaxValue, val);
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        #endregion

        #region Type Information Tests

        [Fact]
        public void TypeFromTable_CorrectlyIdentifiesColumnTypes()
        {
            var kdb = GetConnection();
            const string tableName = "type_info_test";

            try
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
                // Use correct Kinetica SQL syntax for all types
                kdb.executeSql($@"
                    CREATE TABLE {tableName} (
                        id INT,
                        is_active BOOLEAN,
                        score DECIMAL(10,2),
                        int_tags INTEGER[3],
                        metadata JSON,
                        unique_id UUID,
                        PRIMARY KEY(id)
                    )");

                var ktype = KineticaType.fromTable(kdb, tableName);
                var columns = ktype.getColumns();

                // Check boolean column
                var boolCol = columns.FirstOrDefault(c => c.getName() == "is_active");
                Assert.NotNull(boolCol);
                Assert.Equal(KineticaType.Column.ColumnType.INT, boolCol!.getType());
                Assert.Contains(ColumnProperty.BOOLEAN, boolCol.getProperties());

                // Check decimal column
                var decimalCol = columns.FirstOrDefault(c => c.getName() == "score");
                Assert.NotNull(decimalCol);
                Assert.Equal(KineticaType.Column.ColumnType.STRING, decimalCol!.getType());
                Assert.Contains(decimalCol.getProperties(), p => p.StartsWith("decimal", StringComparison.OrdinalIgnoreCase));

                // Check array column
                var arrayCol = columns.FirstOrDefault(c => c.getName() == "int_tags");
                Assert.NotNull(arrayCol);
                Assert.Equal(KineticaType.Column.ColumnType.STRING, arrayCol!.getType());
                Assert.Contains(arrayCol.getProperties(), p => p.StartsWith("array", StringComparison.OrdinalIgnoreCase));

                // Check JSON column
                var jsonCol = columns.FirstOrDefault(c => c.getName() == "metadata");
                Assert.NotNull(jsonCol);
                Assert.Equal(KineticaType.Column.ColumnType.STRING, jsonCol!.getType());
                Assert.Contains(ColumnProperty.JSON, jsonCol.getProperties());

                // Check UUID column
                var uuidCol = columns.FirstOrDefault(c => c.getName() == "unique_id");
                Assert.NotNull(uuidCol);
                Assert.Equal(KineticaType.Column.ColumnType.STRING, uuidCol!.getType());
                Assert.Contains(ColumnProperty.UUID, uuidCol.getProperties());
            }
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            }
        }

        #endregion
    }
}
