using System;

namespace table_editor.classes
{
    public interface IColumnSorter : IDisposable
    {
        TimeSpan Sort(double[] data);
    }
}
