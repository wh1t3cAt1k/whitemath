namespace WhiteMath.Matrices.Winders
{
	/// <summary>
	/// Provides a standard row-by-row winding/unwinding of the matrix.
	/// </summary>
	internal sealed class RowByRowWinder : Winder
	{
		internal RowByRowWinder(IMatrix matrix) : base(matrix.RowCount, matrix.ColumnCount) { }
		internal RowByRowWinder(int rowCount, int columnCount) : base(rowCount, columnCount) { }

		protected override void MakeTrace()
		{
			for (int index = 0; index < _elementCount; ++index)
			{
				trace[index] = new IndexPair(index / _rowCount, index % _rowCount);
			}
		}
	}
}