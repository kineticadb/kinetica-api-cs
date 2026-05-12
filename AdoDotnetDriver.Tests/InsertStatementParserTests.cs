using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    /// <summary>
    /// Unit tests for the InsertStatementParser class.
    /// These tests do not require a Kinetica connection.
    /// </summary>
    public class InsertStatementParserTests
    {
        [Fact]
        public void TestBasicParsing()
        {
            // Test basic INSERT parsing
            var sql = "INSERT INTO my_table (id, name, value) VALUES (1, 'test', 3.14)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("my_table", result.TableName);
            Assert.Equal(3, result.ColumnNames.Count);
            Assert.Equal("id", result.ColumnNames[0]);
            Assert.Equal("name", result.ColumnNames[1]);
            Assert.Equal("value", result.ColumnNames[2]);
            Assert.Equal(3, result.Values.Count);
            Assert.Equal(1, result.Values[0]);
            Assert.Equal("test", result.Values[1]);
            Assert.Equal(3.14, result.Values[2]);
        }

        [Fact]
        public void TestSchemaQualifiedTable()
        {
            var sql = "INSERT INTO schema.table_name (col1, col2) VALUES (100, 'hello')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("schema.table_name", result.TableName);
            Assert.Equal(2, result.ColumnNames.Count);
        }

        [Fact]
        public void TestNullValues()
        {
            var sql = "INSERT INTO my_table (id, name) VALUES (1, NULL)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(2, result.Values.Count);
            Assert.Equal(1, result.Values[0]);
            Assert.Null(result.Values[1]);
        }

        [Fact]
        public void TestQuotedStrings()
        {
            var sql = "INSERT INTO my_table (id, name) VALUES (1, 'it''s a test')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("it's a test", result.Values[1]);
        }

        [Fact]
        public void TestBooleanValues()
        {
            var sql = "INSERT INTO my_table (id, flag1, flag2) VALUES (1, TRUE, FALSE)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(true, result.Values[1]);
            Assert.Equal(false, result.Values[2]);
        }

        [Fact]
        public void TestNonInsertStatement()
        {
            var sql = "SELECT * FROM my_table";
            Assert.False(InsertStatementParser.TryParse(sql, out var result));
            Assert.Null(result);
        }

        [Fact]
        public void TestUpdateStatement()
        {
            var sql = "UPDATE my_table SET name = 'test' WHERE id = 1";
            Assert.False(InsertStatementParser.TryParse(sql, out var result));
            Assert.Null(result);
        }

        [Fact]
        public void TestDeleteStatement()
        {
            var sql = "DELETE FROM my_table WHERE id = 1";
            Assert.False(InsertStatementParser.TryParse(sql, out var result));
            Assert.Null(result);
        }

        [Fact]
        public void TestNegativeNumbers()
        {
            var sql = "INSERT INTO my_table (id, value) VALUES (-10, -3.14)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(-10, result.Values[0]);
            Assert.Equal(-3.14, result.Values[1]);
        }

        [Fact]
        public void TestLargeNumbers()
        {
            var sql = "INSERT INTO my_table (id, big_value) VALUES (1, 9223372036854775807)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(9223372036854775807L, result.Values[1]);
        }

        [Fact]
        public void TestEmptyString()
        {
            var sql = "INSERT INTO my_table (id, name) VALUES (1, '')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("", result.Values[1]);
        }

        [Fact]
        public void TestStringWithCommas()
        {
            var sql = "INSERT INTO my_table (id, name) VALUES (1, 'a, b, c')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("a, b, c", result.Values[1]);
        }

        [Fact]
        public void TestCaseInsensitiveKeywords()
        {
            var sql = "insert into my_table (id, name) values (1, 'test')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("my_table", result.TableName);
        }

        [Fact]
        public void TestMixedCaseKeywords()
        {
            var sql = "Insert Into my_table (id, name) Values (1, 'test')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("my_table", result.TableName);
        }

        [Fact]
        public void TestDoubleQuotedStrings()
        {
            var sql = "INSERT INTO my_table (id, name) VALUES (1, \"double quoted\")";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("double quoted", result.Values[1]);
        }

        [Fact]
        public void TestEscapedDoubleQuotes()
        {
            var sql = "INSERT INTO my_table (id, name) VALUES (1, \"it\"\"s quoted\")";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("it\"s quoted", result.Values[1]);
        }

        [Fact]
        public void TestScientificNotation()
        {
            var sql = "INSERT INTO my_table (id, value) VALUES (1, 1.5e10)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(1.5e10, result.Values[1]);
        }

        [Fact]
        public void TestNegativeScientificNotation()
        {
            var sql = "INSERT INTO my_table (id, value) VALUES (1, -2.5e-5)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(-2.5e-5, result.Values[1]);
        }

        [Fact]
        public void TestMultipleNullValues()
        {
            var sql = "INSERT INTO my_table (a, b, c, d) VALUES (NULL, NULL, NULL, NULL)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(4, result.Values.Count);
            Assert.All(result.Values, v => Assert.Null(v));
        }

        [Fact]
        public void TestNullCaseInsensitive()
        {
            var sql = "INSERT INTO my_table (a, b, c) VALUES (null, Null, NULL)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(3, result.Values.Count);
            Assert.All(result.Values, v => Assert.Null(v));
        }

        [Fact]
        public void TestBooleanCaseInsensitive()
        {
            var sql = "INSERT INTO my_table (a, b, c, d) VALUES (true, True, TRUE, false)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(true, result.Values[0]);
            Assert.Equal(true, result.Values[1]);
            Assert.Equal(true, result.Values[2]);
            Assert.Equal(false, result.Values[3]);
        }

        [Fact]
        public void TestWhitespaceHandling()
        {
            var sql = "INSERT INTO   my_table   (  id  ,  name  )   VALUES   (  1  ,  'test'  )";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("my_table", result.TableName);
            Assert.Equal(2, result.ColumnNames.Count);
            Assert.Equal(1, result.Values[0]);
            Assert.Equal("test", result.Values[1]);
        }

        [Fact]
        public void TestNewlinesInStatement()
        {
            var sql = @"INSERT INTO my_table
                        (id, name, value)
                        VALUES
                        (1, 'test', 3.14)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("my_table", result.TableName);
            Assert.Equal(3, result.Values.Count);
        }

        [Fact]
        public void TestStringWithParentheses()
        {
            var sql = "INSERT INTO my_table (id, name) VALUES (1, 'value (with parens)')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("value (with parens)", result.Values[1]);
        }

        [Fact]
        public void TestZeroValues()
        {
            var sql = "INSERT INTO my_table (id, int_val, float_val) VALUES (0, 0, 0.0)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(0, result.Values[0]);
            Assert.Equal(0, result.Values[1]);
            Assert.Equal(0.0, result.Values[2]);
        }

        [Fact]
        public void TestIntegerOverflow_ReturnsLong()
        {
            var sql = "INSERT INTO my_table (id, big_int) VALUES (1, 2147483648)"; // Int32.MaxValue + 1
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(2147483648L, result.Values[1]);
            Assert.IsType<long>(result.Values[1]);
        }

        [Fact]
        public void TestIntegerFitsInInt32()
        {
            var sql = "INSERT INTO my_table (id, small_int) VALUES (1, 2147483647)"; // Int32.MaxValue
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(2147483647, result.Values[1]);
            Assert.IsType<int>(result.Values[1]);
        }

        [Fact]
        public void TestSingleColumn()
        {
            var sql = "INSERT INTO my_table (id) VALUES (42)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Single(result.ColumnNames);
            Assert.Equal("id", result.ColumnNames[0]);
            Assert.Single(result.Values);
            Assert.Equal(42, result.Values[0]);
        }

        [Fact]
        public void TestManyColumns()
        {
            var sql = "INSERT INTO my_table (a, b, c, d, e, f, g, h, i, j) VALUES (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(10, result.ColumnNames.Count);
            Assert.Equal(10, result.Values.Count);
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i + 1, result.Values[i]);
            }
        }

        [Fact]
        public void TestStringWithSpecialChars()
        {
            var sql = "INSERT INTO my_table (id, data) VALUES (1, 'Special: @#$%^&*!')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("Special: @#$%^&*!", result.Values[1]);
        }

        [Fact]
        public void TestStringWithNewlines()
        {
            // Note: The newline is inside the quoted string
            var sql = "INSERT INTO my_table (id, data) VALUES (1, 'line1\nline2')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("line1\nline2", result.Values[1]);
        }

        [Fact]
        public void TestDecimalNumbers()
        {
            var sql = "INSERT INTO my_table (id, price) VALUES (1, 99.99)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(99.99, result.Values[1]);
        }

        [Fact]
        public void TestLeadingDecimal()
        {
            var sql = "INSERT INTO my_table (id, value) VALUES (1, .5)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal(0.5, result.Values[1]);
        }

        [Fact]
        public void TestSchemaQualifiedTableWithUnderscore()
        {
            // Kinetica supports schema.table format
            var sql = "INSERT INTO my_schema.my_table_name (id) VALUES (1)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("my_schema.my_table_name", result.TableName);
        }

        [Fact]
        public void TestUnderscoreInTableAndColumnNames()
        {
            var sql = "INSERT INTO my_table_name (column_one, column_two) VALUES (1, 2)";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("my_table_name", result.TableName);
            Assert.Equal("column_one", result.ColumnNames[0]);
            Assert.Equal("column_two", result.ColumnNames[1]);
        }

        [Fact]
        public void TestNumericColumnNames()
        {
            var sql = "INSERT INTO my_table (col1, col2, col3) VALUES ('a', 'b', 'c')";
            Assert.True(InsertStatementParser.TryParse(sql, out var result));
            Assert.NotNull(result);
            Assert.Equal("col1", result.ColumnNames[0]);
            Assert.Equal("col2", result.ColumnNames[1]);
            Assert.Equal("col3", result.ColumnNames[2]);
        }
    }
}

