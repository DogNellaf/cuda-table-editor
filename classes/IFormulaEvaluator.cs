using System;
using System.Collections.Generic;

namespace table_editor.classes
{
    public interface IFormulaEvaluator : IDisposable
    {
        string Evaluate(int row, int col, IList<Row> rows);
    }
}
