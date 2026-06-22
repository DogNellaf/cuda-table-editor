using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace table_editor.classes
{
    /// <summary>
    /// Cell address format: {row}A{col} where row is 1-based and col is 1-based.
    /// Example: "2A3" means row 2, column 3.
    /// </summary>
    public static class FormulaParser
    {
        private static readonly Regex CellRefPattern = new(@"(\d+)A(\d+)", RegexOptions.Compiled);

        public static bool IsFormula(string value) =>
            !string.IsNullOrEmpty(value) && value[0] == '=';

        public static string GetBody(string formula) =>
            formula[1..];

        public static IReadOnlyList<CellReference> ExtractReferences(string expression)
        {
            var result = new List<CellReference>();
            foreach (Match m in CellRefPattern.Matches(expression))
            {
                int row = int.Parse(m.Groups[1].Value) - 1;  // 0-based row index
                int col = int.Parse(m.Groups[2].Value);       // 1-based column index (matches table layout)
                result.Add(new CellReference(row, col, m.Value));
            }
            return result;
        }
    }

    public readonly struct CellReference
    {
        public int Row { get; }
        public int Col { get; }
        public string Token { get; }

        public CellReference(int row, int col, string token)
        {
            Row = row;
            Col = col;
            Token = token;
        }
    }
}
