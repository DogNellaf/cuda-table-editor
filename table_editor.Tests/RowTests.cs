using table_editor.classes;
using Xunit;

namespace table_editor.Tests
{
    public class RowTests
    {
        [Fact]
        public void DefaultConstructor_EmptyRow()
        {
            var row = new Row();
            Assert.Empty(row);
        }

        [Fact]
        public void Constructor_IndexAndColumns_CorrectCellCount()
        {
            var row = new Row(1, 4);
            Assert.Equal(4, row.Count);
        }

        [Fact]
        public void Constructor_FirstCellIsRowNumber()
        {
            var row = new Row(5, 3);
            Assert.Equal("5", row[0].Value);
        }

        [Fact]
        public void Constructor_DataCellsAreEmpty()
        {
            var row = new Row(1, 3);
            Assert.Equal(string.Empty, row[1].Value);
            Assert.Equal(string.Empty, row[2].Value);
        }

        [Fact]
        public void ToString_ProducesCorrectCsvFormat()
        {
            var row = new Row(1, 3);
            row[1].Value = "hello";
            row[2].Value = "42";

            string csv = row.ToString();
            Assert.Equal("1;hello;42;\n", csv);
        }

        [Fact]
        public void ToString_EmptyRow_IsNewline()
        {
            var row = new Row();
            Assert.Equal("\n", row.ToString());
        }

        [Fact]
        public void ToString_AllCellsSeparatedBySemicolon()
        {
            var row = new Row(2, 4);
            string csv = row.ToString();
            // 4 cells each followed by ';', then '\n'
            Assert.Equal(5, csv.Length - csv.Replace(";", "").Length);
        }
    }
}
