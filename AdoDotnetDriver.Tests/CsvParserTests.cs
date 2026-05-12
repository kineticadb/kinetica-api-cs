using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    public class CsvParserTests
    {
        [Fact]
        public void ParseLine_SimpleCsvLine()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("a,b,c");

            Assert.Equal(3, result.Length);
            Assert.Equal("a", result[0]);
            Assert.Equal("b", result[1]);
            Assert.Equal("c", result[2]);
        }

        [Fact]
        public void ParseLine_QuotedFields()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("\"hello\",\"world\",test");

            Assert.Equal(3, result.Length);
            Assert.Equal("hello", result[0]);
            Assert.Equal("world", result[1]);
            Assert.Equal("test", result[2]);
        }

        [Fact]
        public void ParseLine_QuotedFieldsWithComma()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("\"hello, world\",test,value");

            Assert.Equal(3, result.Length);
            Assert.Equal("hello, world", result[0]);
            Assert.Equal("test", result[1]);
            Assert.Equal("value", result[2]);
        }

        [Fact]
        public void ParseLine_EscapedQuotes()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("\"hello \"\"world\"\"\",test");

            Assert.Equal(2, result.Length);
            Assert.Equal("hello \"world\"", result[0]);
            Assert.Equal("test", result[1]);
        }

        [Fact]
        public void ParseLine_EmptyFields()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("a,,c,");

            Assert.Equal(4, result.Length);
            Assert.Equal("a", result[0]);
            Assert.Equal("", result[1]);
            Assert.Equal("c", result[2]);
            Assert.Equal("", result[3]);
        }

        [Fact]
        public void ParseLine_PipeSeparated()
        {
            var parser = new CsvParser(delimiter: '|');
            var result = parser.ParseLine("a|b|c");

            Assert.Equal(3, result.Length);
            Assert.Equal("a", result[0]);
            Assert.Equal("b", result[1]);
            Assert.Equal("c", result[2]);
        }

        [Fact]
        public void ParseLine_TabSeparated()
        {
            var parser = new CsvParser(delimiter: '\t');
            var result = parser.ParseLine("a\tb\tc");

            Assert.Equal(3, result.Length);
            Assert.Equal("a", result[0]);
            Assert.Equal("b", result[1]);
            Assert.Equal("c", result[2]);
        }

        [Fact]
        public void ParseLine_CustomQuoteCharacter()
        {
            var parser = new CsvParser(quote: '\'');
            var result = parser.ParseLine("'hello, world',test");

            Assert.Equal(2, result.Length);
            Assert.Equal("hello, world", result[0]);
            Assert.Equal("test", result[1]);
        }

        [Fact]
        public void ParseLine_EscapeCharacter()
        {
            var parser = new CsvParser(escape: '\\');
            var result = parser.ParseLine("hello\\,world,test");

            Assert.Equal(2, result.Length);
            Assert.Equal("hello,world", result[0]);
            Assert.Equal("test", result[1]);
        }

        [Fact]
        public void ParseLine_NumericValues()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("123,45.67,-89,0");

            Assert.Equal(4, result.Length);
            Assert.Equal("123", result[0]);
            Assert.Equal("45.67", result[1]);
            Assert.Equal("-89", result[2]);
            Assert.Equal("0", result[3]);
        }

        [Fact]
        public void ParseLine_MixedContent()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("John,\"Doe, Jr.\",123,\"456.78\",true,");

            Assert.Equal(6, result.Length);
            Assert.Equal("John", result[0]);
            Assert.Equal("Doe, Jr.", result[1]);
            Assert.Equal("123", result[2]);
            Assert.Equal("456.78", result[3]);
            Assert.Equal("true", result[4]);
            Assert.Equal("", result[5]);
        }

        [Fact]
        public void ParseLine_SingleField()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("single_value");

            Assert.Single(result);
            Assert.Equal("single_value", result[0]);
        }

        [Fact]
        public void ParseLine_EmptyLine()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("");

            Assert.Single(result);
            Assert.Equal("", result[0]);
        }

        [Fact]
        public void ParseLine_WhitespacePreserved()
        {
            var parser = new CsvParser();
            var result = parser.ParseLine("  a  ,  b  ,c");

            Assert.Equal(3, result.Length);
            Assert.Equal("  a  ", result[0]);
            Assert.Equal("  b  ", result[1]);
            Assert.Equal("c", result[2]);
        }

        [Fact]
        public void ParseLine_NewlineInQuotedField()
        {
            var parser = new CsvParser();
            // Note: This would typically come from a multi-line reader
            // For single-line parsing, the newline is preserved
            var result = parser.ParseLine("\"hello\nworld\",test");

            Assert.Equal(2, result.Length);
            Assert.Equal("hello\nworld", result[0]);
            Assert.Equal("test", result[1]);
        }
    }
}
