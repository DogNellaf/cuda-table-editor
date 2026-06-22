using table_editor.classes;
using Xunit;

namespace table_editor.Tests
{
    public class TableModelTests
    {
        // ── Construction ──────────────────────────────────────────────────────────

        [Fact]
        public void Constructor_Empty_ZeroRowsZeroColumns()
        {
            var m = new TableModel(0, 0);
            Assert.Equal(0, m.RowsCount);
            Assert.Equal(1, m.ColumnsCount); // always has row-number column
            Assert.Empty(m.Rows);
        }

        [Fact]
        public void Constructor_RowsAndColumns_CorrectCounts()
        {
            var m = new TableModel(5, 3);
            Assert.Equal(5, m.RowsCount);
            Assert.Equal(4, m.ColumnsCount); // 3 data + 1 row-number
            Assert.Equal(5, m.Rows.Count);
        }

        [Fact]
        public void Constructor_RowNumbersAreSequential()
        {
            var m = new TableModel(3, 2);
            Assert.Equal("1", m.Rows[0][0].Value);
            Assert.Equal("2", m.Rows[1][0].Value);
            Assert.Equal("3", m.Rows[2][0].Value);
        }

        [Fact]
        public void Constructor_EachRowHasCorrectCellCount()
        {
            var m = new TableModel(2, 4);
            foreach (var row in m.Rows)
                Assert.Equal(5, row.Count); // 4 data + 1 row-number
        }

        // ── AddRow ────────────────────────────────────────────────────────────────

        [Fact]
        public void AddRow_IncreasesRowsCount()
        {
            var m = new TableModel(2, 2);
            m.AddRow();
            Assert.Equal(3, m.RowsCount);
            Assert.Equal(3, m.Rows.Count);
        }

        [Fact]
        public void AddRow_NewRowHasCorrectIndex()
        {
            var m = new TableModel(2, 2);
            var row = m.AddRow();
            Assert.Equal("3", row[0].Value);
        }

        [Fact]
        public void AddRow_NewRowHasCorrectCellCount()
        {
            var m = new TableModel(1, 3);
            var row = m.AddRow();
            Assert.Equal(4, row.Count);
        }

        // ── AddColumn ─────────────────────────────────────────────────────────────

        [Fact]
        public void AddColumn_IncreasesColumnsCount()
        {
            var m = new TableModel(2, 2);
            m.AddColumn();
            Assert.Equal(4, m.ColumnsCount);
        }

        [Fact]
        public void AddColumn_AllRowsGetNewCell()
        {
            var m = new TableModel(3, 2);
            m.AddColumn();
            foreach (var row in m.Rows)
                Assert.Equal(4, row.Count);
        }

        // ── DeleteLastRow ─────────────────────────────────────────────────────────

        [Fact]
        public void DeleteLastRow_DecreasesRowsCount()
        {
            var m = new TableModel(3, 2);
            m.DeleteLastRow();
            Assert.Equal(2, m.RowsCount);
            Assert.Equal(2, m.Rows.Count);
        }

        [Fact]
        public void DeleteLastRow_OnEmptyTable_DoesNotThrow()
        {
            var m = new TableModel(0, 0);
            m.DeleteLastRow(); // must not throw
            Assert.Equal(0, m.RowsCount);
        }

        [Fact]
        public void DeleteLastRow_RemovesLastRow()
        {
            var m = new TableModel(3, 2);
            m.Rows[2][1].Value = "marker";
            m.DeleteLastRow();
            Assert.Equal(2, m.Rows.Count);
            Assert.Equal("1", m.Rows[0][0].Value);
            Assert.Equal("2", m.Rows[1][0].Value);
        }

        // ── DeleteLastColumn ──────────────────────────────────────────────────────

        [Fact]
        public void DeleteLastColumn_DecreasesColumnsCount()
        {
            var m = new TableModel(2, 3);
            m.DeleteLastColumn();
            Assert.Equal(3, m.ColumnsCount);
        }

        [Fact]
        public void DeleteLastColumn_AllRowsShrink()
        {
            var m = new TableModel(2, 3);
            m.DeleteLastColumn();
            foreach (var row in m.Rows)
                Assert.Equal(3, row.Count);
        }

        [Fact]
        public void DeleteLastColumn_WhenOnlyRowNumberColumn_DoesNothing()
        {
            var m = new TableModel(2, 0);
            m.DeleteLastColumn();
            Assert.Equal(1, m.ColumnsCount);
            foreach (var row in m.Rows)
                Assert.Single(row);
        }

        // ── CSV round-trip ────────────────────────────────────────────────────────

        [Fact]
        public void GetCsv_ThenParseCsv_PreservesData()
        {
            var original = new TableModel(3, 3);
            original.Rows[0][1].Value = "Alpha";
            original.Rows[1][2].Value = "42";
            original.Rows[2][3].Value = "3.14";

            var csv = original.GetCsv();
            var restored = new TableModel(csv);

            Assert.Equal(3, restored.RowsCount);
            Assert.Equal(4, restored.ColumnsCount);
            Assert.Equal("Alpha", restored.Rows[0][1].Value);
            Assert.Equal("42", restored.Rows[1][2].Value);
            Assert.Equal("3.14", restored.Rows[2][3].Value);
        }

        [Fact]
        public void GetCsv_RowNumbersArePersisted()
        {
            var m = new TableModel(3, 2);
            var csv = m.GetCsv();
            var restored = new TableModel(csv);

            Assert.Equal("1", restored.Rows[0][0].Value);
            Assert.Equal("2", restored.Rows[1][0].Value);
            Assert.Equal("3", restored.Rows[2][0].Value);
        }

        [Fact]
        public void CsvConstructor_EmptyString_EmptyTable()
        {
            var m = new TableModel(string.Empty);
            Assert.Equal(0, m.RowsCount);
        }

        [Fact]
        public void CsvConstructor_SkipsBlankLines()
        {
            var csv = "1;A;\n\n2;B;\n";
            var m = new TableModel(csv);
            Assert.Equal(2, m.RowsCount);
        }

        // ── Multi-operation scenarios ─────────────────────────────────────────────

        [Fact]
        public void AddThenDeleteRow_ReturnsToPreviousCount()
        {
            var m = new TableModel(2, 2);
            m.AddRow();
            m.DeleteLastRow();
            Assert.Equal(2, m.RowsCount);
        }

        [Fact]
        public void AddThenDeleteColumn_ReturnsToPreviousCount()
        {
            var m = new TableModel(2, 2);
            m.AddColumn();
            m.DeleteLastColumn();
            Assert.Equal(3, m.ColumnsCount);
        }
    }
}
