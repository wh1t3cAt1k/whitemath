namespace WhiteMath.Matrices.Winders
{
	/// <summary>
	/// TODO: check if it works all right
	/// </summary>
	internal sealed class SpiralWinder : Winder
	{

		internal SpiralWinder(IMatrix matrix) : base(matrix.RowCount, matrix.ColumnCount) { }
		internal SpiralWinder(int rowCount, int columnCount) : base(rowCount, columnCount) { }

		protected override void MakeTrace()
		{
			int count = _elementCount;
			int rowIndex = 0;
			int columnIndex = 0;

			bool wereSteppingDown;
			bool wereSteppingRight;
			int level = 0;

			do
			{
				wereSteppingDown = false;
				wereSteppingRight = false;

				// Stepping right at upper border.
				// -
				if (columnIndex < _columnCount - level - 1)
				{
					wereSteppingRight = true;
				}

				while (columnIndex < _columnCount - level - 1)
				{
					trace[_elementCount - (count--)] = new IndexPair(rowIndex, columnIndex);
					columnIndex++;
				}

				// Stepping down at rightmost border.
				// -
				if (rowIndex < _rowCount - level - 1)
				{
					wereSteppingDown = true;
				}

				while (rowIndex < _rowCount - level - 1)
				{
					trace[_elementCount - (count--)] = new IndexPair(rowIndex, columnIndex);
					rowIndex++;
				}

				// Stepping left at downmost border.
				// -
				if (wereSteppingDown && wereSteppingRight)
				{
					while (columnIndex > level)
					{
						trace[_elementCount - (count--)] = new IndexPair(rowIndex, columnIndex);
						columnIndex--;
					}
				}

				// Stepping up at leftmost border.
				// -
				if (wereSteppingDown && wereSteppingRight)
				{
					while (rowIndex > level + 1)
					{
						trace[_elementCount - (count--)] = new IndexPair(rowIndex, columnIndex);
						rowIndex--;
					}
				}

				// Increase indent level
				// -
				++level;
			}
			while (count > 1);

			if (count > 0)
			{
				// For null matrices.
				// -
				trace[_elementCount - 1] = new IndexPair(rowIndex, columnIndex);
			}
		}
	}
}