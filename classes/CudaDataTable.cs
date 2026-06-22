using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace table_editor.classes
{
    public class CudaDataTable : IDisposable
    {
        private readonly TableModel _model;
        private readonly IFormulaEvaluator _evaluator;
        private bool _disposed;

        public int RowsCount => _model.RowsCount;
        public int ColumnsCount => _model.ColumnsCount;
        public List<Row> Rows => _model.Rows;
        public List<DataGridColumn> Columns { get; } = new();

        public CudaDataTable(int rowsCount, int columnsCount, IFormulaEvaluator? evaluator = null)
        {
            _evaluator = evaluator ?? new CudaEvaluator();
            _model = new TableModel(rowsCount, columnsCount);
            RebuildColumns();
        }

        public CudaDataTable(string csv, IFormulaEvaluator? evaluator = null)
        {
            _evaluator = evaluator ?? new CudaEvaluator();
            _model = new TableModel(csv);
            RebuildColumns();
            RecalculateFormulas();
        }

        public Row AddRow() => _model.AddRow();

        public DataGridColumn AddColumn()
        {
            _model.AddColumn();
            var col = MakeDataColumn($"A{ColumnsCount - 1}", ColumnsCount - 1);
            Columns.Add(col);
            return col;
        }

        public void DeleteLastRow() => _model.DeleteLastRow();

        public void DeleteLastColumn()
        {
            if (ColumnsCount <= 1) return;
            _model.DeleteLastColumn();
            if (Columns.Count > 0)
                Columns.RemoveAt(Columns.Count - 1);
        }

        public string EvaluateCell(int row, int col)
        {
            try
            {
                return _evaluator.Evaluate(row, col, Rows);
            }
            catch (Exception ex)
            {
                return $"#ERR: {ex.Message}";
            }
        }

        public void RecalculateFormulas()
        {
            for (int i = 0; i < Rows.Count; i++)
                for (int j = 0; j < Rows[i].Count; j++)
                    if (Rows[i][j].Type == CellType.Formula)
                        Rows[i][j].DisplayValue = EvaluateCell(i, j);
        }

        public string GetCsv() => _model.GetCsv();

        private void RebuildColumns()
        {
            Columns.Clear();
            Columns.Add(MakeRowNumberColumn());
            for (int i = 1; i < ColumnsCount; i++)
                Columns.Add(MakeDataColumn($"A{i}", i));
        }

        private static DataGridTextColumn MakeRowNumberColumn() =>
            new DataGridTextColumn
            {
                Header = "",
                Binding = new Binding("[0].Value"),
                IsReadOnly = true,
                Width = 40
            };

        private static DataGridColumn MakeDataColumn(string header, int index)
        {
            var col = new DataGridTemplateColumn { Header = header };

            var displayFactory = new FrameworkElementFactory(typeof(TextBlock));
            displayFactory.SetBinding(TextBlock.TextProperty, new Binding($"[{index}].DisplayValue"));
            displayFactory.SetValue(TextBlock.MarginProperty, new Thickness(2, 0, 2, 0));
            col.CellTemplate = new DataTemplate { VisualTree = displayFactory };

            var editFactory = new FrameworkElementFactory(typeof(TextBox));
            editFactory.SetBinding(TextBox.TextProperty,
                new Binding($"[{index}].Value") { UpdateSourceTrigger = UpdateSourceTrigger.LostFocus });
            col.CellEditingTemplate = new DataTemplate { VisualTree = editFactory };

            return col;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _evaluator.Dispose();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
