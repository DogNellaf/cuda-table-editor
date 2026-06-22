namespace table_editor.classes
{
    public class Column
    {
        public string Name { get; private set; }

        public Column(string name)
        {
            Name = name;
        }
    }
}
