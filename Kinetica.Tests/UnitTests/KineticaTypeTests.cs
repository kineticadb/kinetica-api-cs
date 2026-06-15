using Xunit;
using kinetica;

namespace Kinetica.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for KineticaType, particularly the fromDynamicSchema method.
    /// These tests verify correct parsing of dynamic schemas returned by /execute/sql.
    /// </summary>
    [Trait("Category", "Unit")]
    public class KineticaTypeTests
    {
        #region Geometry Type Tests

        /// <summary>
        /// Tests that the "geometry" column type returned by spatial SQL functions
        /// (ST_GeomFromText, ST_Intersection, etc.) is correctly mapped to STRING with WKT property.
        /// This was previously throwing: KineticaException: Unknown data type/property: geometry
        /// </summary>
        [Fact]
        public void FromDynamicSchema_GeometryType_MapsToStringWithWktProperty()
        {
            // Arrange: Simulate dynamic schema returned by /execute/sql with a geometry column
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""int""}},
                    {""name"": ""column_2"", ""type"": {""type"": ""array"", ""items"": ""string""}}
                ]
            }";

            // Column headers and types as returned by Kinetica for:
            // SELECT id, ST_GeomFromText('POINT(0 0)') as geom FROM table
            object[] columnHeaders = new object[] { "id", "geom" };
            object[] columnTypes = new object[] { "int", "geometry" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            Assert.NotNull(ktype);
            Assert.Equal(2, ktype.getColumnCount());

            // Check the geometry column
            var geomColumn = ktype.getColumn("geom");
            Assert.NotNull(geomColumn);
            Assert.Equal(KineticaType.Column.ColumnType.STRING, geomColumn.getType());

            // Verify WKT property is added
            var properties = geomColumn.getProperties();
            Assert.Contains(ColumnProperty.WKT, properties);
        }

        /// <summary>
        /// Tests geometry column with nullable attribute.
        /// </summary>
        [Fact]
        public void FromDynamicSchema_NullableGeometryType_MapsCorrectly()
        {
            // Arrange: Schema where column_2 (geometry) is nullable
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""int""}},
                    {""name"": ""column_2"", ""type"": {""type"": ""array"", ""items"": [""string"", ""null""]}}
                ]
            }";

            object[] columnHeaders = new object[] { "id", "geom" };
            object[] columnTypes = new object[] { "int", "geometry" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            var geomColumn = ktype.getColumn("geom");
            Assert.NotNull(geomColumn);
            Assert.Equal(KineticaType.Column.ColumnType.STRING, geomColumn.getType());
            Assert.Contains(ColumnProperty.WKT, geomColumn.getProperties());
            Assert.True(geomColumn.isNullable());
        }

        /// <summary>
        /// Tests that multiple geometry columns in a single query are all handled correctly.
        /// Example: SELECT ST_Intersection(a, b) as intersect, ST_Union(a, b) as union_geom FROM ...
        /// </summary>
        [Fact]
        public void FromDynamicSchema_MultipleGeometryColumns_AllMapCorrectly()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""string""}},
                    {""name"": ""column_2"", ""type"": {""type"": ""array"", ""items"": ""string""}},
                    {""name"": ""column_3"", ""type"": {""type"": ""array"", ""items"": ""string""}}
                ]
            }";

            object[] columnHeaders = new object[] { "intersection_geom", "union_geom", "simplified_geom" };
            object[] columnTypes = new object[] { "geometry", "geometry", "geometry" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            Assert.Equal(3, ktype.getColumnCount());

            foreach (var colName in columnHeaders.Cast<string>())
            {
                var column = ktype.getColumn(colName);
                Assert.Equal(KineticaType.Column.ColumnType.STRING, column.getType());
                Assert.Contains(ColumnProperty.WKT, column.getProperties());
            }
        }

        #endregion

        #region WKT Type Tests (existing functionality)

        /// <summary>
        /// Tests that explicit "wkt" column type works correctly (existing behavior).
        /// </summary>
        [Fact]
        public void FromDynamicSchema_ExplicitWktType_MapsToStringWithWktProperty()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""int""}},
                    {""name"": ""column_2"", ""type"": {""type"": ""array"", ""items"": ""string""}}
                ]
            }";

            object[] columnHeaders = new object[] { "id", "location" };
            object[] columnTypes = new object[] { "int", "wkt" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            var wktColumn = ktype.getColumn("location");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, wktColumn.getType());
            Assert.Contains(ColumnProperty.WKT, wktColumn.getProperties());
        }

        #endregion

        #region Other Column Type Tests

        /// <summary>
        /// Tests that standard column types still work correctly after geometry changes.
        /// </summary>
        [Fact]
        public void FromDynamicSchema_StandardTypes_MapCorrectly()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""int""}},
                    {""name"": ""column_2"", ""type"": {""type"": ""array"", ""items"": ""long""}},
                    {""name"": ""column_3"", ""type"": {""type"": ""array"", ""items"": ""float""}},
                    {""name"": ""column_4"", ""type"": {""type"": ""array"", ""items"": ""double""}},
                    {""name"": ""column_5"", ""type"": {""type"": ""array"", ""items"": ""string""}},
                    {""name"": ""column_6"", ""type"": {""type"": ""array"", ""items"": ""bytes""}}
                ]
            }";

            object[] columnHeaders = new object[] { "int_col", "long_col", "float_col", "double_col", "string_col", "bytes_col" };
            object[] columnTypes = new object[] { "int", "long", "float", "double", "string", "bytes" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            Assert.Equal(6, ktype.getColumnCount());
            Assert.Equal(KineticaType.Column.ColumnType.INT, ktype.getColumn("int_col").getType());
            Assert.Equal(KineticaType.Column.ColumnType.LONG, ktype.getColumn("long_col").getType());
            Assert.Equal(KineticaType.Column.ColumnType.FLOAT, ktype.getColumn("float_col").getType());
            Assert.Equal(KineticaType.Column.ColumnType.DOUBLE, ktype.getColumn("double_col").getType());
            Assert.Equal(KineticaType.Column.ColumnType.STRING, ktype.getColumn("string_col").getType());
            Assert.Equal(KineticaType.Column.ColumnType.BYTES, ktype.getColumn("bytes_col").getType());
        }

        /// <summary>
        /// Tests that string-based property types (date, time, datetime, etc.) work correctly.
        /// </summary>
        [Fact]
        public void FromDynamicSchema_StringPropertyTypes_MapCorrectly()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""string""}},
                    {""name"": ""column_2"", ""type"": {""type"": ""array"", ""items"": ""string""}},
                    {""name"": ""column_3"", ""type"": {""type"": ""array"", ""items"": ""string""}},
                    {""name"": ""column_4"", ""type"": {""type"": ""array"", ""items"": ""string""}},
                    {""name"": ""column_5"", ""type"": {""type"": ""array"", ""items"": ""string""}}
                ]
            }";

            object[] columnHeaders = new object[] { "date_col", "time_col", "datetime_col", "uuid_col", "json_col" };
            object[] columnTypes = new object[] { "date", "time", "datetime", "uuid", "json" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            Assert.Equal(5, ktype.getColumnCount());

            // All should be STRING type with their respective properties
            var dateCol = ktype.getColumn("date_col");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, dateCol.getType());
            Assert.Contains(ColumnProperty.DATE, dateCol.getProperties());

            var timeCol = ktype.getColumn("time_col");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, timeCol.getType());
            Assert.Contains(ColumnProperty.TIME, timeCol.getProperties());

            var datetimeCol = ktype.getColumn("datetime_col");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, datetimeCol.getType());
            Assert.Contains(ColumnProperty.DATETIME, datetimeCol.getProperties());

            var uuidCol = ktype.getColumn("uuid_col");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, uuidCol.getType());
            Assert.Contains(ColumnProperty.UUID, uuidCol.getProperties());

            var jsonCol = ktype.getColumn("json_col");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, jsonCol.getType());
            Assert.Contains(ColumnProperty.JSON, jsonCol.getProperties());
        }

        /// <summary>
        /// Tests decimal type with precision and scale.
        /// </summary>
        [Fact]
        public void FromDynamicSchema_DecimalWithPrecisionScale_MapsCorrectly()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""string""}}
                ]
            }";

            object[] columnHeaders = new object[] { "amount" };
            object[] columnTypes = new object[] { "decimal(19,4)" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            var decimalCol = ktype.getColumn("amount");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, decimalCol.getType());
            Assert.Contains("decimal(19,4)", decimalCol.getProperties());
        }

        /// <summary>
        /// Tests array type handling.
        /// </summary>
        [Fact]
        public void FromDynamicSchema_ArrayType_MapsCorrectly()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""string""}}
                ]
            }";

            object[] columnHeaders = new object[] { "tags" };
            object[] columnTypes = new object[] { "array(int,10)" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            var arrayCol = ktype.getColumn("tags");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, arrayCol.getType());
            Assert.Contains("array(int,10)", arrayCol.getProperties());
        }

        /// <summary>
        /// Tests vector type handling.
        /// </summary>
        [Fact]
        public void FromDynamicSchema_VectorType_MapsCorrectly()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""bytes""}}
                ]
            }";

            object[] columnHeaders = new object[] { "embedding" };
            object[] columnTypes = new object[] { "vector(1024)" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            var vectorCol = ktype.getColumn("embedding");
            Assert.Equal(KineticaType.Column.ColumnType.BYTES, vectorCol.getType());
            Assert.Contains("vector(1024)", vectorCol.getProperties());
        }

        #endregion

        #region Error Handling Tests

        /// <summary>
        /// Tests that unknown column types still throw an exception.
        /// </summary>
        [Fact]
        public void FromDynamicSchema_UnknownType_ThrowsException()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""string""}}
                ]
            }";

            object[] columnHeaders = new object[] { "unknown_col" };
            object[] columnTypes = new object[] { "unknown_type_xyz" };

            // Act & Assert
            var exception = Assert.Throws<KineticaException>(() =>
                KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes));

            Assert.Contains("Unknown data type/property", exception.Message);
            Assert.Contains("unknown_type_xyz", exception.Message);
        }

        /// <summary>
        /// Tests that mismatched column headers and types throw an exception.
        /// </summary>
        [Fact]
        public void FromDynamicSchema_MismatchedArrayLengths_ThrowsException()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": []
            }";

            object[] columnHeaders = new object[] { "col1", "col2" };
            object[] columnTypes = new object[] { "int" };  // Only one type for two headers

            // Act & Assert
            var exception = Assert.Throws<KineticaException>(() =>
                KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes));

            Assert.Contains("not of the same length", exception.Message);
        }

        #endregion

        #region Mixed Geometry and Regular Columns Test

        /// <summary>
        /// Tests a realistic SQL query result with mixed column types including geometry.
        /// Simulates: SELECT id, name, ST_GeomFromText(location) as geom, created_at FROM locations
        /// </summary>
        [Fact]
        public void FromDynamicSchema_MixedColumnsWithGeometry_MapsCorrectly()
        {
            // Arrange
            string dynamicSchemaJson = @"{
                ""type"": ""record"",
                ""name"": ""dynamic_schema"",
                ""fields"": [
                    {""name"": ""column_1"", ""type"": {""type"": ""array"", ""items"": ""int""}},
                    {""name"": ""column_2"", ""type"": {""type"": ""array"", ""items"": ""string""}},
                    {""name"": ""column_3"", ""type"": {""type"": ""array"", ""items"": ""string""}},
                    {""name"": ""column_4"", ""type"": {""type"": ""array"", ""items"": ""string""}}
                ]
            }";

            object[] columnHeaders = new object[] { "id", "name", "geom", "created_at" };
            object[] columnTypes = new object[] { "int", "string", "geometry", "datetime" };

            // Act
            var ktype = KineticaType.fromDynamicSchema(dynamicSchemaJson, columnHeaders, columnTypes);

            // Assert
            Assert.Equal(4, ktype.getColumnCount());

            // id column
            var idCol = ktype.getColumn("id");
            Assert.Equal(KineticaType.Column.ColumnType.INT, idCol.getType());

            // name column
            var nameCol = ktype.getColumn("name");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, nameCol.getType());
            Assert.Empty(nameCol.getProperties().Where(p => p == ColumnProperty.WKT));

            // geometry column - should have WKT property
            var geomCol = ktype.getColumn("geom");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, geomCol.getType());
            Assert.Contains(ColumnProperty.WKT, geomCol.getProperties());

            // datetime column
            var datetimeCol = ktype.getColumn("created_at");
            Assert.Equal(KineticaType.Column.ColumnType.STRING, datetimeCol.getType());
            Assert.Contains(ColumnProperty.DATETIME, datetimeCol.getProperties());
        }

        #endregion
    }
}
