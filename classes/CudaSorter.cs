using ManagedCuda;
using System;

namespace table_editor.classes
{
    public class CudaSorter : IColumnSorter
    {
        private readonly CudaContext _context;
        private readonly CudaKernel _kernel;
        private bool _disposed;

        public CudaSorter()
        {
            _context = new CudaContext();
            _kernel = _context.LoadKernel("selectionsort.ptx", "swapOnKernel");
        }

        /// <summary>
        /// Sorts <paramref name="data"/> in-place on the GPU (ascending).
        /// Returns the GPU kernel execution time.
        /// </summary>
        public TimeSpan Sort(double[] data)
        {
            uint length = (uint)data.Length;

            using var d_arr = new CudaDeviceVariable<double>(length);
            d_arr.CopyToDevice(data);

            _kernel.SetComputeSize(length, length);

            var start = DateTime.UtcNow;
            _kernel.Run(d_arr.DevicePointer, length);
            var elapsed = DateTime.UtcNow - start;

            d_arr.CopyToHost(data);
            return elapsed;
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
