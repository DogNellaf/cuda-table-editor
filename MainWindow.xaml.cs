using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using table_editor.classes;
using table_editor.windows;

namespace table_editor
{
    public partial class MainWindow : Window
    {
        private CudaDataTable _dataTable;
        private readonly IColumnSorter _sorter;

        public MainWindow()
        {
            InitializeComponent();
            _sorter = new CudaSorter();
            _dataTable = new CudaDataTable(0, 0);
        }

        // ── File commands ─────────────────────────────────────────────────────────

        private void NewFile_Execute(object sender, RoutedEventArgs e)
        {
            var dialog = new NewTableWindow();
            if (dialog.ShowDialog() != true) return;

            _dataTable.Dispose();
            _dataTable = new CudaDataTable(dialog.RowsCount, dialog.ColumnsCount);
            DrawTable();
        }

        private void OpenFile_Execute(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                var csv = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                _dataTable.Dispose();
                _dataTable = new CudaDataTable(csv);
                DrawTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open file:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveFile_Execute(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                FileName = "table",
                DefaultExt = ".csv",
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                File.WriteAllText(dialog.FileName, _dataTable.GetCsv(), Encoding.UTF8);
                SetStatus($"Saved to {Path.GetFileName(dialog.FileName)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save file:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Table editing ─────────────────────────────────────────────────────────

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            _dataTable.AddRow();
            DrawTable();
        }

        private void AddCol_Click(object sender, RoutedEventArgs e)
        {
            _dataTable.AddColumn();
            DrawTable();
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            _dataTable.DeleteLastRow();
            DrawTable();
        }

        private void DeleteCol_Click(object sender, RoutedEventArgs e)
        {
            _dataTable.DeleteLastColumn();
            DrawTable();
        }

        private void RecalcFormulas_Click(object sender, RoutedEventArgs e)
        {
            _dataTable.RecalculateFormulas();
            DrawTable();
            SetStatus("All formulas recalculated.");
        }

        // ── Grid events ───────────────────────────────────────────────────────────

        private void mainDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;

            int rowIndex = mainDataGrid.Items.IndexOf(e.Row.Item);
            if (rowIndex < 0 || rowIndex >= _dataTable.Rows.Count) return;

            var row = _dataTable.Rows[rowIndex];
            for (int j = 1; j < row.Count; j++)
            {
                if (row[j].Type == CellType.Formula)
                    row[j].DisplayValue = _dataTable.EvaluateCell(rowIndex, j);
            }
        }

        private void mainDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            int colIndex = mainDataGrid.Columns.IndexOf(e.Column);
            if (colIndex <= 0) return; // skip row-number column

            var values = _dataTable.Rows
                .Select(r => double.TryParse(r[colIndex].Value, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var d) ? d : 0.0)
                .ToArray();

            TimeSpan elapsed = _sorter.Sort(values);

            bool ascending = e.Column.SortDirection != ListSortDirection.Ascending;
            e.Column.SortDirection = ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;

            if (!ascending)
                values = values.Reverse().ToArray();

            for (int i = 0; i < _dataTable.Rows.Count; i++)
                _dataTable.Rows[i][colIndex].Value =
                    values[i].ToString(CultureInfo.InvariantCulture);

            DrawTable();
            SetStatus($"Column sorted in {elapsed.TotalMilliseconds:F2} ms  |  " +
                      $"Rows: {_dataTable.RowsCount}  Columns: {_dataTable.ColumnsCount - 1}");
        }

        // ── Window lifecycle ──────────────────────────────────────────────────────

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _sorter.Dispose();
            _dataTable.Dispose();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void DrawTable()
        {
            mainDataGrid.Columns.Clear();
            mainDataGrid.ItemsSource = null;

            foreach (var column in _dataTable.Columns)
                mainDataGrid.Columns.Add(column);

            mainDataGrid.ItemsSource = _dataTable.Rows;

            SetStatus($"Rows: {_dataTable.RowsCount}  Columns: {_dataTable.ColumnsCount - 1}");
        }

        private void SetStatus(string message) =>
            statusText.Text = message;
    }
}
