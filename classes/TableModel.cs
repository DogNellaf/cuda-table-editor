using System;
using System.Collections.Generic;
using System.Text;

namespace table_editor.classes
{
    public class TableModel
    {
        public int RowsCount { get; private set; }
        public int ColumnsCount { get; private set; }
        public List<Row> Rows { get; } = new();

        public TableModel(int rowsCount, int columnsCount)
        {
            ColumnsCount = columnsCount + 1; // column 0 is the row-number column
            RowsCount = rowsCount;

            for (int i = 1; i <= RowsCount; i++)
                Rows.Add(new Row(i, ColumnsCount));
        }

        public TableModel(string csv)
        {
            foreach (var line in csv.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cells = line.Split(';');
                int cellCount = cells.Length - 1; // trailing ';' produces an empty last element
                if (cellCount <= 0) continue;

                if (ColumnsCount == 0)
                    ColumnsCount = cellCount;

                var row = new Row();
                for (int j = 0; j < Math.Min(ColumnsCount, cellCount); j++)
                    row.Add(new Cell(cells[j]));

                Rows.Add(row);
                RowsCount++;
            }
        }

        public Row AddRow()
        {
            var row = new Row(++RowsCount, ColumnsCount);
            Rows.Add(row);
            return row;
        }

        public void AddColumn()
        {
            foreach (var row in Rows)
                row.Add(new Cell());
            ColumnsCount++;
        }

        public void DeleteLastRow()
        {
            if (RowsCount == 0) return;
            Rows.RemoveAt(Rows.Count - 1);
            RowsCount--;
        }

        public void DeleteLastColumn()
        {
            if (ColumnsCount <= 1) return;
            foreach (var row in Rows)
                if (row.Count > 0)
                    row.RemoveAt(row.Count - 1);
            ColumnsCount--;
        }

        public string GetCsv()
        {
            var sb = new StringBuilder();
            foreach (var row in Rows)
                sb.Append(row);
            return sb.ToString();
        }
    }
}
