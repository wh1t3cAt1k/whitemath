using System;

namespace WhiteMath.Matrices.Winders
{
	/// <summary>
	/// Performs a zig-zag matrix winding/unwinding.
	/// </summary>
	internal sealed class ZigZagWinder : Winder
	{
		/// <summary>
		/// The direction to go.
		/// 
		/// 1 - Left
		/// 2 - Up-Left
		/// 3 - Up
		/// 4 - Up-Right
		/// 5 - Right
		/// 6 - Down-Right
		/// 7 - Down
		/// 8 - Down-Left
		/// 0 - Stay
		/// </summary>
		private int _direction;

		/// <summary>
		/// The cell type basing on the surrounding matrix borders type
		/// 
		/// Two-based summarizing:
		/// 
		/// 0 - No borders
		/// 1 - Upper border
		/// 2 - Down border
		/// 4 - Left border
		/// 8 - Right border
		/// ==>
		/// 
		/// 0 - No borders
		/// 1 - Upper border
		/// 2 - Down border
		/// 3 - Up-Down border
		/// 4 - Left border
		/// 5 - Up-Left border
		/// 6 - Down-left border
		/// 7 - Up-Down-Left border
		/// 8 - Right border
		/// 9 - Up-Right border
		/// 10 - Down-Right border
		/// 11 - Up-Down-Right border
		/// 12 - Left-Right border
		/// 13 - Up-Left-Right border
		/// 14 - Left-Right-Down border
		/// 15 - All-side border (single element matrix)
		/// </summary>
		private int _celltype;

		/// <summary>
		/// Analyzes the direction basing on where from we have came to the moment
		/// and on what the cell type is.
		/// </summary>
		private void AnalyzeDirection(int rowIndex, int columnIndex)
		{
			// The direction map
			// 2 3 4
			// 1   5
			// 8 7 6
			switch (_celltype)
			{
				case 0: if (_direction == 4) _direction = 4; else _direction = 8; break;
				case 1: if (_direction == 4) _direction = 5; else _direction = 8; break;
				case 2:
				case 6: if (_direction == 8) _direction = 5; else _direction = 4; break;
				case 3:
				case 7: _direction = 5; break;
				case 4: if (_direction == 8) _direction = 7; else _direction = 4; break;
				case 5: _direction = 5; break;
				case 8:
				case 9: if (_direction == 4) _direction = 7; else _direction = 8; break;
				case 10:
				case 11:
				case 14:
				case 15: _direction = 0; break;
				case 12:
				case 13: _direction = 7; break;
				default: break;
			}
		}

		/// <summary>
		/// Makes one baby step inside the matrix, depending on the current direction.
		/// There is no need to check boundaries – they must already be checked at
		/// the previous step.
		/// </summary>
		private IndexPair PerformStep(int rowIndex, int columnIndex)
		{
			switch (_direction)
			{
				case 4: 
					--rowIndex; 
					++columnIndex;
					break;
				case 5: 
					++columnIndex; 
					break;
				case 7: 
					++rowIndex;
					break;
				case 8: 
					++rowIndex; 
					--columnIndex; 
					break;
				default: 
					throw new Exception();
			}

			return new IndexPair(rowIndex, columnIndex);
		}

		/// <summary>
		/// Analyzes the cell type basing on what the surrounding matrix borders are.
		/// </summary>
		private void AnalyzeCellType(int rowIndex, int columnIndex)
		{
			_celltype = 0;                           // No borders by default
			if (rowIndex == 0) _celltype++;                 // Upper border
			if (rowIndex == _rowCount - 1) _celltype += 2;       // Down border
			if (columnIndex == 0) _celltype += 4;              // Left border
			if (columnIndex == _columnCount - 1) _celltype += 8;    // Right border
		}

		internal ZigZagWinder(IMatrix matrix) : base(matrix.RowCount, matrix.ColumnCount) { }
		internal ZigZagWinder(int rowCount, int columnCount) : base(rowCount, columnCount) { }

		protected override void MakeTrace()
		{
			int rowIndex = 0;
			int columnIndex = 0;

			for (int traceIndex = 0; traceIndex < this._elementCount; ++traceIndex)
			{
				trace[traceIndex] = new IndexPair(rowIndex, columnIndex);

				AnalyzeCellType(rowIndex, columnIndex);
				AnalyzeDirection(rowIndex, columnIndex);

				IndexPair stepResult = PerformStep(rowIndex, columnIndex);

				rowIndex = stepResult.Row;
				columnIndex = stepResult.Column;
			}
		}
	}
}