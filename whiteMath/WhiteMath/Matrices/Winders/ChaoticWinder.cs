using System;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// Performs a (pseudo) chaotic matrix winding/unwinding.
    /// Please notice: though randomized, the order of winding/unwinding 
    /// will not change once created.
    /// </summary>
    internal sealed class ChaoticWinder: Winder
    {
        internal ChaoticWinder(int rowCount, int columnCount)
            : base(rowCount, columnCount) { }

        internal ChaoticWinder(IMatrix matrix) 
            : base(matrix.RowCount, matrix.ColumnCount) { }

		Random _generator = new Random();
        
        /// <summary>
        /// Обеспечивает двусторонний проход по массиву стандартной построчной развертки
        /// путем пошаговой перестановки текущего элемента со случайным и 
        /// центрально-симметрично текущему со случайным.
        /// </summary>
        protected override void formTrace()
        {
            // ------ первоначальная развертка

            RowByRowWinder temp = new RowByRowWinder(this.rows, this.columns);
            IndexPair t;

			this.trace = temp.trace;

            // ------ проход по циклу с обменами

            for (int i = 0; i < trace.Length; i++)
            {
                int switchInd = _generator.Next(trace.Length);
                int switchInd2 = _generator.Next(trace.Length);

                t = trace[i];
                trace[i] = trace[switchInd];
                trace[switchInd] = t;

                t = trace[trace.Length - i - 1];
                trace[trace.Length - i - 1] = trace[switchInd2];
                trace[switchInd2] = t;
            }
        }
    }
}
