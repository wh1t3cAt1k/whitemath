using System;

namespace WhiteMath.Matrices.Winders
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
        protected override void MakeTrace()
        {
			RowByRowWinder rowByRowWinder = new RowByRowWinder(this._rowCount, this._columnCount);
            
			IndexPair temporaryIndexPair;

			this.trace = rowByRowWinder.trace;

            // Element exchange loop.
			// -
			for (int i = 0; i < trace.Length; i++)
            {
				int switchIndex1 = _generator.Next(trace.Length);
				int switchIndex2 = _generator.Next(trace.Length);

                temporaryIndexPair = trace[i];
                trace[i] = trace[switchIndex1];
                trace[switchIndex1] = temporaryIndexPair;

                temporaryIndexPair = trace[trace.Length - i - 1];
                trace[trace.Length - i - 1] = trace[switchIndex2];
                trace[switchIndex2] = temporaryIndexPair;
            }
        }
    }
}
