using System.Collections.ObjectModel;
using System.Text;

namespace table_editor.classes
{
    public class Row : ObservableCollection<Cell>
    {
        public Row() { }

        public Row(int index, int columnsCount)
        {
            Add(new Cell($"{index}"));
            for (int i = 1; i < columnsCount; i++)
                Add(new Cell());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var cell in this)
                sb.Append(cell).Append(';');
            sb.Append('\n');
            return sb.ToString();
        }
    }
}
