using System;
using System.Data;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    public class SQLParserTests
    {
        private readonly SQLParser _parser = new SQLParser();

        [Theory]
        [InlineData("SELECT * FROM users", ParsedCommandType.Select)]
        [InlineData("select id from users", ParsedCommandType.Select)]
        [InlineData("  SELECT id FROM users", ParsedCommandType.Select)]
        public void Parse_DetectsSelectCommand(string sql, ParsedCommandType expected)
        {
            var result = _parser.Parse(sql, new KineticaParameterCollection());
            Assert.Equal(expected, result.CommandType);
        }

        [Theory]
        [InlineData("INSERT INTO users VALUES (1, 'test')", ParsedCommandType.Insert)]
        [InlineData("insert into users (id, name) values (1, 'test')", ParsedCommandType.Insert)]
        public void Parse_DetectsInsertCommand(string sql, ParsedCommandType expected)
        {
            var result = _parser.Parse(sql, new KineticaParameterCollection());
            Assert.Equal(expected, result.CommandType);
        }

        [Theory]
        [InlineData("UPDATE users SET name = 'test'", ParsedCommandType.Update)]
        [InlineData("update users set name = 'test' where id = 1", ParsedCommandType.Update)]
        public void Parse_DetectsUpdateCommand(string sql, ParsedCommandType expected)
        {
            var result = _parser.Parse(sql, new KineticaParameterCollection());
            Assert.Equal(expected, result.CommandType);
        }

        [Theory]
        [InlineData("DELETE FROM users", ParsedCommandType.Delete)]
        [InlineData("delete from users where id = 1", ParsedCommandType.Delete)]
        public void Parse_DetectsDeleteCommand(string sql, ParsedCommandType expected)
        {
            var result = _parser.Parse(sql, new KineticaParameterCollection());
            Assert.Equal(expected, result.CommandType);
        }

        [Theory]
        [InlineData("CREATE TABLE users (id INT, name VARCHAR)", ParsedCommandType.CreateTable)]
        [InlineData("create table users (id int)", ParsedCommandType.CreateTable)]
        public void Parse_DetectsCreateTableCommand(string sql, ParsedCommandType expected)
        {
            var result = _parser.Parse(sql, new KineticaParameterCollection());
            Assert.Equal(expected, result.CommandType);
        }

        [Theory]
        [InlineData("DROP TABLE users", ParsedCommandType.DropTable)]
        [InlineData("drop table if exists users", ParsedCommandType.DropTable)]
        public void Parse_DetectsDropTableCommand(string sql, ParsedCommandType expected)
        {
            var result = _parser.Parse(sql, new KineticaParameterCollection());
            Assert.Equal(expected, result.CommandType);
        }

        [Fact]
        public void Parse_SubstitutesStringParameter()
        {
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "John", DbType = DbType.String });

            var result = _parser.Parse("SELECT * FROM users WHERE name = @name", parameters);

            Assert.Equal("SELECT * FROM users WHERE name = 'John'", result.FinalSql);
        }

        [Fact]
        public void Parse_SubstitutesIntegerParameter()
        {
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 123, DbType = DbType.Int32 });

            var result = _parser.Parse("SELECT * FROM users WHERE id = @id", parameters);

            Assert.Equal("SELECT * FROM users WHERE id = 123", result.FinalSql);
        }

        [Fact]
        public void Parse_SubstitutesBooleanParameter_True()
        {
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@active", Value = true, DbType = DbType.Boolean });

            var result = _parser.Parse("SELECT * FROM users WHERE active = @active", parameters);

            Assert.Equal("SELECT * FROM users WHERE active = TRUE", result.FinalSql);
        }

        [Fact]
        public void Parse_SubstitutesBooleanParameter_False()
        {
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@active", Value = false, DbType = DbType.Boolean });

            var result = _parser.Parse("SELECT * FROM users WHERE active = @active", parameters);

            Assert.Equal("SELECT * FROM users WHERE active = FALSE", result.FinalSql);
        }

        [Fact]
        public void Parse_SubstitutesDateTimeParameter()
        {
            var parameters = new KineticaParameterCollection();
            var date = new DateTime(2024, 1, 15, 10, 30, 45);
            parameters.Add(new KineticaParameter { ParameterName = "@date", Value = date, DbType = DbType.DateTime });

            var result = _parser.Parse("SELECT * FROM events WHERE created_at = @date", parameters);

            Assert.Equal("SELECT * FROM events WHERE created_at = '2024-01-15 10:30:45.000'", result.FinalSql);
        }

        [Fact]
        public void Parse_SubstitutesNullParameter()
        {
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@value", Value = null, DbType = DbType.String });

            var result = _parser.Parse("SELECT * FROM users WHERE value = @value", parameters);

            Assert.Equal("SELECT * FROM users WHERE value = NULL", result.FinalSql);
        }

        [Fact]
        public void Parse_EscapesSingleQuotesInStrings()
        {
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "O'Brien", DbType = DbType.String });

            var result = _parser.Parse("SELECT * FROM users WHERE name = @name", parameters);

            Assert.Equal("SELECT * FROM users WHERE name = 'O''Brien'", result.FinalSql);
        }

        [Fact]
        public void Parse_SubstitutesMultipleParameters()
        {
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 1, DbType = DbType.Int32 });
            parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "John", DbType = DbType.String });

            var result = _parser.Parse("SELECT * FROM users WHERE id = @id AND name = @name", parameters);

            Assert.Equal("SELECT * FROM users WHERE id = 1 AND name = 'John'", result.FinalSql);
        }

        [Fact]
        public void Parse_HandlesParameterWithoutAtSymbol()
        {
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "id", Value = 1, DbType = DbType.Int32 });

            var result = _parser.Parse("SELECT * FROM users WHERE id = @id", parameters);

            Assert.Equal("SELECT * FROM users WHERE id = 1", result.FinalSql);
        }

        [Fact]
        public void Parse_PreservesOriginalSql()
        {
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 1, DbType = DbType.Int32 });

            var result = _parser.Parse("SELECT * FROM users WHERE id = @id", parameters);

            Assert.Equal("SELECT * FROM users WHERE id = @id", result.OriginalSql);
        }

        #region INSERT FROM FILE Tests

        [Theory]
        [InlineData("INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\"", ParsedCommandType.InsertFromFile)]
        [InlineData("insert into schema.users select * from file.\"/data/users.csv\"", ParsedCommandType.InsertFromFile)]
        [InlineData("INSERT INTO users (id, name) SELECT id, name FROM FILE.\"/data/users.csv\"", ParsedCommandType.InsertFromFile)]
        [InlineData("INSERT INTO users SELECT * FROM 'kifs://data/users.csv'", ParsedCommandType.InsertFromFile)]
        public void Parse_DetectsInsertFromFileCommand(string sql, ParsedCommandType expected)
        {
            var result = _parser.Parse(sql, new KineticaParameterCollection());
            Assert.Equal(expected, result.CommandType);
        }

        [Fact]
        public void Parse_InsertFromFile_ExtractsTableName()
        {
            var result = _parser.Parse("INSERT INTO my_table SELECT * FROM FILE.\"/data/test.csv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal("my_table", result.InsertFromFile.TableName);
        }

        [Fact]
        public void Parse_InsertFromFile_ExtractsSchemaQualifiedTableName()
        {
            var result = _parser.Parse("INSERT INTO my_schema.my_table SELECT * FROM FILE.\"/data/test.csv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal("my_schema.my_table", result.InsertFromFile.TableName);
        }

        [Fact]
        public void Parse_InsertFromFile_ExtractsFilePath()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal("/data/users.csv", result.InsertFromFile.FilePath);
            Assert.Null(result.InsertFromFile.KifsPath);
            Assert.False(result.InsertFromFile.IsKifsPath);
        }

        [Fact]
        public void Parse_InsertFromFile_ExtractsKifsPath()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM 'kifs://data/users.csv'", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal("kifs://data/users.csv", result.InsertFromFile.KifsPath);
            Assert.Null(result.InsertFromFile.FilePath);
            Assert.True(result.InsertFromFile.IsKifsPath);
        }

        [Fact]
        public void Parse_InsertFromFile_ExtractsSelectAllColumns()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.True(result.InsertFromFile.SelectAll);
            Assert.Empty(result.InsertFromFile.SelectColumns);
        }

        [Fact]
        public void Parse_InsertFromFile_ExtractsSelectColumnList()
        {
            var result = _parser.Parse("INSERT INTO users SELECT id, name, email FROM FILE.\"/data/users.csv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.False(result.InsertFromFile.SelectAll);
            Assert.Equal(3, result.InsertFromFile.SelectColumns.Count);
            Assert.Contains("id", result.InsertFromFile.SelectColumns);
            Assert.Contains("name", result.InsertFromFile.SelectColumns);
            Assert.Contains("email", result.InsertFromFile.SelectColumns);
        }

        [Fact]
        public void Parse_InsertFromFile_ExtractsInsertColumnList()
        {
            var result = _parser.Parse("INSERT INTO users (id, name) SELECT id, name FROM FILE.\"/data/users.csv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(2, result.InsertFromFile.InsertColumns.Count);
            Assert.Contains("id", result.InsertFromFile.InsertColumns);
            Assert.Contains("name", result.InsertFromFile.InsertColumns);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsBatchSize()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (BATCH_SIZE = 5000)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(5000, result.InsertFromFile.Options.BatchSize);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsDelimiter()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (DELIMITER = '|')",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal('|', result.InsertFromFile.Options.Delimiter);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsHeader()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (HEADER = FALSE)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.False(result.InsertFromFile.Options.HasHeader);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsSkipAndLimit()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (SKIP = 10, LIMIT = 1000)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(10, result.InsertFromFile.Options.Skip);
            Assert.Equal(1000, result.InsertFromFile.Options.Limit);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsNullString()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (NULL = 'NULL')",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal("NULL", result.InsertFromFile.Options.NullString);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsInitialClear()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (INITIAL_CLEAR = TRUE)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.True(result.InsertFromFile.Options.InitialClear);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsErrorMode()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (ON_ERROR = SKIP)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileErrorMode.Skip, result.InsertFromFile.Options.ErrorMode);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsTruncateStrings()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (TRUNCATE_STRINGS = TRUE)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.True(result.InsertFromFile.Options.TruncateStrings);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsUpdateOnExistingPk()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (UPDATE_ON_EXISTING_PK = TRUE)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.True(result.InsertFromFile.Options.UpdateOnExistingPk);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsIgnoreExistingPk()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (IGNORE_EXISTING_PK = TRUE)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.True(result.InsertFromFile.Options.IgnoreExistingPk);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_MultipleOptions()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (BATCH_SIZE = 5000, DELIMITER = '|', SKIP = 1, HEADER = TRUE, ON_ERROR = SKIP)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(5000, result.InsertFromFile.Options.BatchSize);
            Assert.Equal('|', result.InsertFromFile.Options.Delimiter);
            Assert.Equal(1, result.InsertFromFile.Options.Skip);
            Assert.True(result.InsertFromFile.Options.HasHeader);
            Assert.Equal(FileErrorMode.Skip, result.InsertFromFile.Options.ErrorMode);
        }

        [Fact]
        public void Parse_InsertFromFile_AutoDetectsPsvDelimiter()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.psv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal('|', result.InsertFromFile.Options.Delimiter);
        }

        [Fact]
        public void Parse_InsertFromFile_AutoDetectsTsvDelimiter()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.tsv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal('\t', result.InsertFromFile.Options.Delimiter);
        }

        [Fact]
        public void Parse_InsertFromFile_DefaultsToCommaDelimiter()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(',', result.InsertFromFile.Options.Delimiter);
        }

        #endregion

        #region File Format Detection Tests

        [Fact]
        public void Parse_InsertFromFile_DetectsParquetFormat()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.parquet\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.Parquet, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_DetectsParquetFormat_PqtExtension()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.pqt\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.Parquet, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_DetectsJsonFormat()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.json\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.Json, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_DetectsJsonlFormat()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.jsonl\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.Json, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_DetectsAvroFormat()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.avro\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.Avro, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_DetectsShapefileFormat()
        {
            var result = _parser.Parse("INSERT INTO geodata SELECT * FROM FILE.\"/data/points.shp\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.Shapefile, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_DetectsDelimitedTextForCsv()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\"", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.DelimitedText, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsFormat()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.dat\" WITH OPTIONS (FORMAT = PARQUET)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.Parquet, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_ExtractsFileType()
        {
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.dat\" WITH OPTIONS (FILE_TYPE = JSON)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.Json, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_WithOptions_FormatOverridesExtension()
        {
            // Even though the file is .csv, explicit FORMAT option takes precedence
            var result = _parser.Parse(
                "INSERT INTO users SELECT * FROM FILE.\"/data/users.csv\" WITH OPTIONS (FORMAT = PARQUET)",
                new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.Equal(FileFormat.Parquet, result.InsertFromFile.Options.Format);
        }

        [Fact]
        public void Parse_InsertFromFile_KifsParquet()
        {
            var result = _parser.Parse("INSERT INTO users SELECT * FROM 'kifs://data/users.parquet'", new KineticaParameterCollection());

            Assert.NotNull(result.InsertFromFile);
            Assert.True(result.InsertFromFile.IsKifsPath);
            Assert.Equal(FileFormat.Parquet, result.InsertFromFile.Options.Format);
        }

        #endregion
    }
}
