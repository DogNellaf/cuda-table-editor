using ManagedCuda;
using ManagedCuda.VectorTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace table_editor.classes
{
    public class CudaEvaluator : IFormulaEvaluator
    {
        private readonly CudaContext _context;
        private readonly CudaKernel _kernel;
        private bool _disposed;

        public CudaEvaluator()
        {
            _context = new CudaContext();
            _kernel = _context.LoadKernel("evaluate.ptx", "EvaluateExpression");
        }

        /// <summary>
        /// Evaluates a plain arithmetic expression (no cell references) on the GPU.
        /// The CUDA kernel handles +, -, *, / with integer/float operands, left-to-right.
        /// </summary>
        public double EvaluateExpression(string expression)
        {
            double[] result = new double[1];
            using var d_result = new CudaDeviceVariable<double>(1);
            using var d_expr = new CudaDeviceVariable<byte>(expression.Length + 1);

            d_expr.CopyToDevice(Encoding.ASCII.GetBytes(expression + "\0"));

            _kernel.BlockDimensions = new dim3(1, 1, 1);
            _kernel.GridDimensions = new dim3(1, 1, 1);
            _kernel.Run(d_expr.DevicePointer, d_result.DevicePointer);

            d_result.CopyToHost(result);
            return result[0];
        }

        public string Evaluate(int row, int col, IList<Row> rows) =>
            EvaluateCell(row, col, rows, new HashSet<(int, int)>());

        private string EvaluateCell(int row, int col, IList<Row> rows, HashSet<(int, int)> visited)
        {
            if (row < 0 || row >= rows.Count || col < 0 || col >= rows[row].Count)
                return "#REF!";

            var cell = rows[row][col];

            if (cell.Type == CellType.String)
                return "#TYPE!";

            if (cell.Type != CellType.Formula)
                return cell.Value;

            if (!visited.Add((row, col)))
                return "#CIRC!";

            try
            {
                var body = FormulaParser.GetBody(cell.Value);

                foreach (var r in FormulaParser.ExtractReferences(body))
                {
                    var val = EvaluateCell(r.Row, r.Col, rows, visited);
                    body = body.Replace(r.Token, val);
                }

                return EvaluateExpression(body)
                    .ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return $"#ERR: {ex.Message}";
            }
            finally
            {
                visited.Remove((row, col));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _context.Dispose();
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
