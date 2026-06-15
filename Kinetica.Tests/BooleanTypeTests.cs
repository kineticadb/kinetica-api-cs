using Xunit;
using kinetica;

namespace Kinetica.Tests
{
    /// <summary>
    /// Tests for boolean type handling in Kinetica.
    /// </summary>
    [Trait("Category", "Integration")]
    public class BooleanTypeTests
    {
        private const string ConnectionUrl = "http://localhost:9191";
        private const string Username = "admin";
        private const string Password = "secret";
        private const string TestTableName = "bool_type_test";

        [Fact]
        public void Boolean_StoredAsInteger_ReadsCorrectly()
        {
            var kdb = new kinetica.Kinetica(ConnectionUrl, new kinetica.Kinetica.Options()
            {
                Username = Username,
                Password = Password
            });

            try
            {
                // Setup
                try { kdb.executeSql($"DROP TABLE IF EXISTS {TestTableName}"); } catch { }
                kdb.executeSql($"CREATE TABLE {TestTableName} (id INT, active BOOLEAN, PRIMARY KEY(id))");
                kdb.executeSql($"INSERT INTO {TestTableName} VALUES (1, TRUE), (2, FALSE)");

                // Query
                var response = kdb.executeSql($"SELECT * FROM {TestTableName} ORDER BY id");

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
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {TestTableName}"); } catch { }
            }
        }

        [Fact]
        public void Boolean_TypeInfo_ShowsIntWithBooleanProperty()
        {
            var kdb = new kinetica.Kinetica(ConnectionUrl, new kinetica.Kinetica.Options()
            {
                Username = Username,
                Password = Password
            });

            try
            {
                // Setup
                try { kdb.executeSql($"DROP TABLE IF EXISTS {TestTableName}"); } catch { }
                kdb.executeSql($"CREATE TABLE {TestTableName} (id INT, active BOOLEAN, PRIMARY KEY(id))");

                // Get type info
                var ktype = KineticaType.fromTable(kdb, TestTableName);

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
            finally
            {
                try { kdb.executeSql($"DROP TABLE IF EXISTS {TestTableName}"); } catch { }
            }
        }
    }
}
