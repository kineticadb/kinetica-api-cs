using System;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests
{
    /// <summary>
    /// Async tests for boolean type handling in Kinetica.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncBooleanTypeTests
    {
        [Fact]
        public async Task Boolean_StoredAsInteger_ReadsCorrectlyAsync()
        {
            using var ctx = new TestContext("async_bool_stored");
            var tableName = ctx.QualifiedTable("bool_type_test");

            // Setup
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, active BOOLEAN, PRIMARY KEY(id))");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} VALUES (1, TRUE), (2, FALSE)");

            // Query
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} ORDER BY id");

            // Assert
            Assert.Equal(2, response.total_number_of_records);
            Assert.Equal(2, response.data.Count);

            // Check first record (id=1, active=TRUE)
            var record1 = response.data[0];
            Assert.True(record1.TryGetValue("id", out var id1));
            Assert.True(record1.TryGetValue("active", out var active1));

            // Boolean columns are stored as integers (0 or 1)
            Assert.Equal(1, Convert.ToInt32(id1));
            Assert.Equal(1, Convert.ToInt32(active1));  // TRUE = 1
            Assert.True(Convert.ToBoolean(active1));     // Should convert correctly

            // Check second record (id=2, active=FALSE)
            var record2 = response.data[1];
            Assert.True(record2.TryGetValue("id", out var id2));
            Assert.True(record2.TryGetValue("active", out var active2));

            Assert.Equal(2, Convert.ToInt32(id2));
            Assert.Equal(0, Convert.ToInt32(active2));  // FALSE = 0
            Assert.False(Convert.ToBoolean(active2));    // Should convert correctly
        }

        [Fact]
        public async Task Boolean_TypeInfo_ShowsIntWithBooleanPropertyAsync()
        {
            using var ctx = new TestContext("async_bool_typeinfo");
            var tableName = ctx.QualifiedTable("bool_type_test");

            // Setup
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, active BOOLEAN, PRIMARY KEY(id))");

            // Get type info
            var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);

            // Find the 'active' column
            KineticaType.Column? activeColumn = null;
            foreach (var col in ktype.getColumns())
            {
                if (col.getName() == "active")
                {
                    activeColumn = col;
                    break;
                }
            }

            Assert.NotNull(activeColumn);

            // Check that it's an INT type
            Assert.Equal(KineticaType.Column.ColumnType.INT, activeColumn.getType());

            // Check that it has the "boolean" property
            var props = activeColumn.getProperties();
            Assert.Contains(ColumnProperty.BOOLEAN, props);
        }
    }
}
