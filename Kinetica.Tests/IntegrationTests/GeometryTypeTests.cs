using Xunit;
using Kinetica.Tests.Common;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for geometry type handling in dynamic schemas.
    ///
    /// These tests verify that geometry columns returned by spatial SQL functions
    /// (ST_GeomFromText, ST_Intersection, ST_Simplify, etc.) are correctly parsed
    /// as STRING columns with WKT property.
    ///
    /// Previously, geometry types would throw: KineticaException: Unknown data type/property: geometry
    /// </summary>
    [Trait("Category", "Integration")]
    public class GeometryTypeTests
    {
        #region Basic Geometry Type Tests

        /// <summary>
        /// Tests that ST_GeomFromText returns a geometry column that can be parsed.
        /// This is the most basic geometry function test.
        /// </summary>
        [Fact]
        public void TestGeometryFromText_Point()
        {
            using var ctx = new TestContext("geom_point");

            // Execute SQL with ST_GeomFromText - returns geometry type
            var sql = "SELECT ST_GeomFromText('POINT(-122.4194 37.7749)') as geom";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            // Should return 1 record
            Assert.Equal(1, resp.total_number_of_records);
            Assert.Single(resp.data);

            // Verify the geometry value is returned as WKT string
            var record = resp.data[0];
            Assert.True(record.TryGetValue("geom", out object? geomObj), "Response should contain 'geom' column");

            var geomValue = geomObj?.ToString();
            Assert.NotNull(geomValue);
            Assert.Contains("POINT", geomValue!, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tests ST_GeomFromText with a LINESTRING geometry.
        /// </summary>
        [Fact]
        public void TestGeometryFromText_LineString()
        {
            using var ctx = new TestContext("geom_line");

            var sql = "SELECT ST_GeomFromText('LINESTRING(0 0, 10 10, 20 25)') as line_geom";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];
            var geomValue = record["line_geom"]?.ToString();
            Assert.NotNull(geomValue);
            Assert.Contains("LINESTRING", geomValue!, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tests ST_GeomFromText with a POLYGON geometry.
        /// </summary>
        [Fact]
        public void TestGeometryFromText_Polygon()
        {
            using var ctx = new TestContext("geom_poly");

            var sql = "SELECT ST_GeomFromText('POLYGON((0 0, 10 0, 10 10, 0 10, 0 0))') as poly_geom";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];
            var geomValue = record["poly_geom"]?.ToString();
            Assert.NotNull(geomValue);
            Assert.Contains("POLYGON", geomValue!, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Geometry Column from Table Tests

        /// <summary>
        /// Tests querying a GEOMETRY column from a table.
        /// </summary>
        [Fact]
        public void TestGeometryColumnFromTable()
        {
            using var ctx = new TestContext("geom_table");

            var tableName = ctx.QualifiedTable("locations");

            // Create table with geometry column
            ctx.Kinetica.executeSql($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    name VARCHAR(64),
                    location GEOMETRY,
                    PRIMARY KEY (id)
                )");

            // Insert some geometry data
            ctx.Kinetica.executeSql($@"
                INSERT INTO {tableName} VALUES
                (1, 'San Francisco', ST_GeomFromText('POINT(-122.4194 37.7749)')),
                (2, 'New York', ST_GeomFromText('POINT(-74.0060 40.7128)')),
                (3, 'Los Angeles', ST_GeomFromText('POINT(-118.2437 34.0522)'))
            ");

            // Query the table - should return geometry as WKT
            var resp = ctx.Kinetica.executeSql($"SELECT id, name, location FROM {tableName} ORDER BY id", 0, -9999);

            Assert.Equal(3, resp.total_number_of_records);

            // Verify each record has geometry data
            foreach (var record in resp.data)
            {
                Assert.True(record.TryGetValue("location", out object? locationObj), "Response should contain 'location' column");
                var location = locationObj?.ToString();
                Assert.NotNull(location);
                Assert.Contains("POINT", location!, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Tests ST_AsText on a geometry column.
        /// </summary>
        [Fact]
        public void TestStAsText()
        {
            using var ctx = new TestContext("geom_astext");

            var tableName = ctx.QualifiedTable("places");

            ctx.Kinetica.executeSql($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    geom GEOMETRY,
                    PRIMARY KEY (id)
                )");

            ctx.Kinetica.executeSql($@"
                INSERT INTO {tableName} VALUES
                (1, ST_GeomFromText('POINT(1.5 2.5)'))
            ");

            // Use ST_AsText to explicitly convert to WKT
            var resp = ctx.Kinetica.executeSql($"SELECT id, ST_AsText(geom) as wkt FROM {tableName}", 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];
            var wkt = record["wkt"]?.ToString();
            Assert.NotNull(wkt);
            Assert.Contains("POINT", wkt!, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("1.5", wkt!);
            Assert.Contains("2.5", wkt!);
        }

        #endregion

        #region Spatial Function Tests

        /// <summary>
        /// Tests ST_Intersection which returns a geometry type.
        /// This was one of the functions that caused the original issue.
        /// </summary>
        [Fact]
        public void TestStIntersection()
        {
            using var ctx = new TestContext("geom_intersect");

            // Two overlapping polygons
            var sql = @"
                SELECT ST_Intersection(
                    ST_GeomFromText('POLYGON((0 0, 10 0, 10 10, 0 10, 0 0))'),
                    ST_GeomFromText('POLYGON((5 5, 15 5, 15 15, 5 15, 5 5))')
                ) as intersection_geom";

            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];
            var geomValue = record["intersection_geom"]?.ToString();
            Assert.NotNull(geomValue);
            // Intersection of two squares should be a polygon
            Assert.Contains("POLYGON", geomValue!, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tests ST_Union which returns a geometry type.
        /// </summary>
        [Fact]
        public void TestStUnion()
        {
            using var ctx = new TestContext("geom_union");

            var sql = @"
                SELECT ST_Union(
                    ST_GeomFromText('POLYGON((0 0, 5 0, 5 5, 0 5, 0 0))'),
                    ST_GeomFromText('POLYGON((3 3, 8 3, 8 8, 3 8, 3 3))')
                ) as union_geom";

            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];
            var geomValue = record["union_geom"]?.ToString();
            Assert.NotNull(geomValue);
            Assert.Contains("POLYGON", geomValue!, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tests ST_Buffer which returns a geometry type.
        /// </summary>
        [Fact]
        public void TestStBuffer()
        {
            using var ctx = new TestContext("geom_buffer");

            var sql = "SELECT ST_Buffer(ST_GeomFromText('POINT(0 0)'), 1.0) as buffer_geom";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];
            var geomValue = record["buffer_geom"]?.ToString();
            Assert.NotNull(geomValue);
            // Buffer around a point creates a polygon (circle approximation)
            Assert.Contains("POLYGON", geomValue!, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tests ST_Centroid which returns a geometry type.
        /// </summary>
        [Fact]
        public void TestStCentroid()
        {
            using var ctx = new TestContext("geom_centroid");

            var sql = "SELECT ST_Centroid(ST_GeomFromText('POLYGON((0 0, 10 0, 10 10, 0 10, 0 0))')) as centroid_geom";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];
            var geomValue = record["centroid_geom"]?.ToString();
            Assert.NotNull(geomValue);
            // Centroid of a square is a point
            Assert.Contains("POINT", geomValue!, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tests ST_Simplify which returns a geometry type.
        /// This was one of the functions mentioned in the original issue.
        /// </summary>
        [Fact]
        public void TestStSimplify()
        {
            using var ctx = new TestContext("geom_simplify");

            // Create a complex linestring and simplify it
            var sql = @"
                SELECT ST_Simplify(
                    ST_GeomFromText('LINESTRING(0 0, 1 0.1, 2 -0.1, 3 0, 4 0.1, 5 0)'),
                    0.5
                ) as simplified_geom";

            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];
            var geomValue = record["simplified_geom"]?.ToString();
            Assert.NotNull(geomValue);
            Assert.Contains("LINESTRING", geomValue!, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Multiple Geometry Columns Tests

        /// <summary>
        /// Tests a query that returns multiple geometry columns.
        /// </summary>
        [Fact]
        public void TestMultipleGeometryColumns()
        {
            using var ctx = new TestContext("geom_multi");

            var sql = @"
                SELECT
                    ST_GeomFromText('POINT(0 0)') as point_geom,
                    ST_GeomFromText('LINESTRING(0 0, 10 10)') as line_geom,
                    ST_GeomFromText('POLYGON((0 0, 5 0, 5 5, 0 5, 0 0))') as poly_geom";

            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];

            // Verify all three geometry columns exist and contain correct types
            Assert.Contains("POINT", record["point_geom"]?.ToString()!, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("LINESTRING", record["line_geom"]?.ToString()!, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("POLYGON", record["poly_geom"]?.ToString()!, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tests a query mixing geometry and non-geometry columns.
        /// </summary>
        [Fact]
        public void TestMixedGeometryAndOtherColumns()
        {
            using var ctx = new TestContext("geom_mixed");

            var tableName = ctx.QualifiedTable("cities");

            ctx.Kinetica.executeSql($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    name VARCHAR(64),
                    population BIGINT,
                    location GEOMETRY,
                    founded DATE,
                    PRIMARY KEY (id)
                )");

            ctx.Kinetica.executeSql($@"
                INSERT INTO {tableName} VALUES
                (1, 'Boston', 675647, ST_GeomFromText('POINT(-71.0589 42.3601)'), '1630-09-07')
            ");

            var resp = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 0, -9999);

            Assert.Equal(1, resp.total_number_of_records);

            var record = resp.data[0];

            // Verify all column types are handled correctly
            Assert.Equal(1, Convert.ToInt32(record["id"]));
            Assert.Equal("Boston", record["name"]?.ToString());
            Assert.Equal(675647L, Convert.ToInt64(record["population"]));
            Assert.Contains("POINT", record["location"]?.ToString()!, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(record["founded"]);
        }

        #endregion

        #region KineticaType.fromDynamicSchema Tests

        /// <summary>
        /// Tests that KineticaType correctly parses geometry columns from executeSql response.
        /// This verifies the fromDynamicSchema fix is working end-to-end.
        /// </summary>
        [Fact]
        public void TestKineticaTypeFromDynamicSchema_WithGeometry()
        {
            using var ctx = new TestContext("geom_dtype");

            // This query returns a geometry column
            var sql = "SELECT 1 as id, ST_GeomFromText('POINT(0 0)') as geom";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            // The response should be parseable without throwing
            // "Unknown data type/property: geometry"
            Assert.Equal(1, resp.total_number_of_records);
            Assert.NotEmpty(resp.data);

            // Verify the data is accessible
            var record = resp.data[0];
            Assert.Equal(1, Convert.ToInt32(record["id"]));
            Assert.Contains("POINT", record["geom"]?.ToString()!, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Null Geometry Tests

        /// <summary>
        /// Tests handling of NULL geometry values.
        /// </summary>
        [Fact]
        public void TestNullGeometry()
        {
            using var ctx = new TestContext("geom_null");

            var tableName = ctx.QualifiedTable("nullable_geom");

            ctx.Kinetica.executeSql($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    geom GEOMETRY,
                    PRIMARY KEY (id)
                )");

            // Insert a record with NULL geometry
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, geom) VALUES (1, NULL)");

            // Insert a record with actual geometry
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} VALUES (2, ST_GeomFromText('POINT(1 1)'))");

            var resp = ctx.Kinetica.executeSql($"SELECT * FROM {tableName} ORDER BY id", 0, -9999);

            Assert.Equal(2, resp.total_number_of_records);

            // First record should have null geometry
            var record1 = resp.data[0];
            Assert.True(record1["geom"] == null || string.IsNullOrEmpty(record1["geom"]?.ToString()));

            // Second record should have geometry
            var record2 = resp.data[1];
            Assert.Contains("POINT", record2["geom"]?.ToString()!, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
