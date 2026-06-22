using System.Windows;

namespace table_editor.windows
{
    public partial class NewTableWindow : Window
    {
        private int _rowsCount;
        private int _columnsCount;

        public int RowsCount => _rowsCount;
        public int ColumnsCount => _columnsCount;

        public NewTableWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(rowsCountBox.Text, out int rows) || rows < 1)
            {
                MessageBox.Show("Rows count must be a positive integer.",
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                rowsCountBox.Focus();
                rowsCountBox.SelectAll();
                return;
            }

            if (!int.TryParse(columnsCountBox.Text, out int cols) || cols < 1)
            {
                MessageBox.Show("Columns count must be a positive integer.",
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                columnsCountBox.Focus();
                columnsCountBox.SelectAll();
                return;
            }

            _rowsCount = rows;
            _columnsCount = cols;
            DialogResult = true;
        }
    }
}
