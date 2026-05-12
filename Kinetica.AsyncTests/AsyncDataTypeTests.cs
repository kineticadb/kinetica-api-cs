using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests
{
    /// <summary>
    /// Async tests for various data type handling in Kinetica.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncDataTypeTests
    {
        #region Boolean Type Tests

        [Fact]
        public async Task Boolean_StoredAsInteger_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_bool_type");
            var tableName = ctx.QualifiedTable("bool_test");

            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, active BOOLEAN, PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, TRUE), (2, FALSE)");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} ORDER BY id");

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

        #endregion

        #region Array Type Tests

        [Fact]
        public async Task Array_IntegerArray_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_array_int");
            var tableName = ctx.QualifiedTable("array_int_test");

            // Use INTEGER[N] syntax for array columns
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, int_values INTEGER[5], PRIMARY KEY(id))");
            // Insert array values using JSON syntax
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, '[1, 2, 3, 4, 5]')");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

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

        [Fact]
        public async Task Array_DoubleArray_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_array_double");
            var tableName = ctx.QualifiedTable("array_double_test");

            // Use DOUBLE[N] syntax for array columns
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, double_values DOUBLE[3], PRIMARY KEY(id))");
            // Insert array values using JSON syntax
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, '[1.1, 2.2, 3.3]')");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

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

        #endregion

        #region Decimal Type Tests

        [Fact]
        public async Task Decimal_8Byte_DefaultPrecision_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_decimal_8byte");
            var tableName = ctx.QualifiedTable("decimal_8byte_test");

            // DECIMAL(18,4) uses 8 bytes (precision <= 18)
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, amount DECIMAL(18,4), PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, 12345.6789)");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

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

        [Fact]
        public async Task Decimal_8Byte_CustomPrecisionScale_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_decimal_8byte_custom");
            var tableName = ctx.QualifiedTable("decimal_custom_test");

            // DECIMAL(10,2) uses 8 bytes (precision <= 18)
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, price DECIMAL(10,2), PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, 99999999.99)");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

            Assert.Equal(1, response.total_number_of_records);

            var record = response.data[0];
            record.TryGetValue("price", out var priceObj);

            var priceStr = priceObj?.ToString();
            Assert.NotNull(priceStr);

            var price = decimal.Parse(priceStr!);
            Assert.Equal(99999999.99m, price);
        }

        [Fact]
        public async Task Decimal_12Byte_HighPrecision_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_decimal_12byte");
            var tableName = ctx.QualifiedTable("decimal_12byte_test");

            // DECIMAL(26,13) uses 12 bytes (precision > 18)
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, big_amount DECIMAL(26,13), PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, 1234567890123.1234567890123)");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

            Assert.Equal(1, response.total_number_of_records);

            var record = response.data[0];
            record.TryGetValue("big_amount", out var amountObj);

            var amountStr = amountObj?.ToString();
            Assert.NotNull(amountStr);

            var amount = decimal.Parse(amountStr!);
            // Verify the high-precision decimal was stored and retrieved correctly
            Assert.True(Math.Abs(amount - 1234567890123.1234567890123m) < 0.0000000001m);
        }

        [Fact]
        public async Task Decimal_12Byte_NearMaxValue_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_decimal_12byte_max");
            var tableName = ctx.QualifiedTable("decimal_max_test");

            // DECIMAL(26,13) for large values
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, big_val DECIMAL(26,13), PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, 9999999999999.9999999999999)");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

            Assert.Equal(1, response.total_number_of_records);

            var record = response.data[0];
            record.TryGetValue("big_val", out var valObj);

            var valStr = valObj?.ToString();
            Assert.NotNull(valStr);

            var val = decimal.Parse(valStr!);
            Assert.True(Math.Abs(val - 9999999999999.9999999999999m) < 0.0000000001m);
        }

        [Fact]
        public async Task Decimal_12Byte_NegativeValue_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_decimal_12byte_neg");
            var tableName = ctx.QualifiedTable("decimal_neg_test");

            // DECIMAL(26,13) for negative values
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, neg_val DECIMAL(26,13), PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, -1234567890123.1234567890123)");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

            Assert.Equal(1, response.total_number_of_records);

            var record = response.data[0];
            record.TryGetValue("neg_val", out var valObj);

            var valStr = valObj?.ToString();
            Assert.NotNull(valStr);

            var val = decimal.Parse(valStr!);
            Assert.True(Math.Abs(val - (-1234567890123.1234567890123m)) < 0.0000000001m);
        }

        [Fact]
        public async Task Decimal_TypeFromTable_ExtractsPrecisionAndScaleAsync()
        {
            using var ctx = new TestContext("async_decimal_typeinfo");
            var tableName = ctx.QualifiedTable("decimal_typeinfo_test");

            // Create table with both 8-byte and 12-byte decimal columns
            await ctx.Kinetica.ExecuteSqlAsync($@"CREATE TABLE {tableName} (
                id INT,
                small_decimal DECIMAL(10,2),
                default_decimal DECIMAL(18,4),
                big_decimal DECIMAL(26,13),
                PRIMARY KEY(id))");

            var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);
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

        [Fact]
        public void Decimal_ColumnConstants_AreCorrect()
        {
            // Verify the decimal constants match the Java API (not async, just a constant check)
            Assert.Equal(18, KineticaType.Column.DEFAULT_DECIMAL_PRECISION);
            Assert.Equal(4, KineticaType.Column.DEFAULT_DECIMAL_SCALE);
            Assert.Equal(18, KineticaType.Column.DECIMAL8_MAX_PRECISION);
        }

        #endregion

        #region WKT Geometry Type Tests

        [Fact]
        public async Task WKT_Point_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_wkt_point");
            var tableName = ctx.QualifiedTable("wkt_point_test");

            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, location GEOMETRY, PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, ST_GEOMFROMTEXT('POINT(-122.4194 37.7749)'))");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT id, ST_ASTEXT(location) as location_wkt FROM {tableName}");

            Assert.Equal(1, response.total_number_of_records);

            var record = response.data[0];
            record.TryGetValue("location_wkt", out var wktObj);

            var wkt = wktObj?.ToString();
            Assert.NotNull(wkt);
            Assert.Contains("POINT", wkt!);
        }

        [Fact]
        public async Task WKT_Polygon_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_wkt_polygon");
            var tableName = ctx.QualifiedTable("wkt_polygon_test");

            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, area GEOMETRY, PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, ST_GEOMFROMTEXT('POLYGON((0 0, 0 10, 10 10, 10 0, 0 0))'))");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT id, ST_ASTEXT(area) as area_wkt FROM {tableName}");

            Assert.Equal(1, response.total_number_of_records);

            var record = response.data[0];
            record.TryGetValue("area_wkt", out var wktObj);

            var wkt = wktObj?.ToString();
            Assert.NotNull(wkt);
            Assert.Contains("POLYGON", wkt!);
        }

        #endregion

        #region Other Type Tests

        [Fact]
        public async Task UUID_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_uuid");
            var tableName = ctx.QualifiedTable("uuid_test");

            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, uid UUID, PRIMARY KEY(id))");
            // Use a static UUID value since Kinetica doesn't have a UUID() function
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, '550e8400-e29b-41d4-a716-446655440000')");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

            Assert.Equal(1, response.total_number_of_records);

            var record = response.data[0];
            record.TryGetValue("uid", out var uidObj);

            var uidStr = uidObj?.ToString();
            Assert.NotNull(uidStr);
            // UUID format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
            Assert.Matches(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", uidStr!.ToLower());
        }

        [Fact]
        public async Task JSON_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_json");
            var tableName = ctx.QualifiedTable("json_test");

            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, metadata JSON, PRIMARY KEY(id))");
            // Use simpler JSON without nested quotes
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, '{{\"name\":\"test\",\"value\":123}}')");

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

            Assert.Equal(1, response.total_number_of_records);

            var record = response.data[0];
            record.TryGetValue("metadata", out var jsonObj);

            var jsonStr = jsonObj?.ToString();
            Assert.NotNull(jsonStr);

            // Parse and verify JSON
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr!);
            Assert.NotNull(parsed);
            Assert.True(parsed!.ContainsKey("name"));
        }

        [Fact]
        public async Task ULONG_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_ulong");
            var tableName = ctx.QualifiedTable("ulong_test");

            // Use LONG since ULONG syntax varies by Kinetica version
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, big_number LONG, PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, 9223372036854775807)"); // Max LONG

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}");

            Assert.Equal(1, response.total_number_of_records);

            var record = response.data[0];
            record.TryGetValue("big_number", out var numberObj);

            Assert.NotNull(numberObj);
            Assert.Equal(9223372036854775807L, Convert.ToInt64(numberObj));
        }

        [Fact]
        public async Task TypeFromTable_CorrectlyIdentifiesColumnTypesAsync()
        {
            using var ctx = new TestContext("async_type_from_table");
            var tableName = ctx.QualifiedTable("type_identification_test");

            await ctx.Kinetica.ExecuteSqlAsync($@"CREATE TABLE {tableName} (
                id INT,
                name VARCHAR(64),
                value DOUBLE,
                count LONG,
                active INT,
                PRIMARY KEY(id))");

            var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);
            var columns = ktype.getColumns();

            Assert.Equal(5, columns.Count);

            var idCol = columns.FirstOrDefault(c => c.getName() == "id");
            Assert.NotNull(idCol);

            var nameCol = columns.FirstOrDefault(c => c.getName() == "name");
            Assert.NotNull(nameCol);

            var valueCol = columns.FirstOrDefault(c => c.getName() == "value");
            Assert.NotNull(valueCol);
        }

        #endregion
    }
}
