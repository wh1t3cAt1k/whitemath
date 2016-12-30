using WhiteMath.Calculators;

namespace WhiteMath.Matrices
{
    public abstract partial class Matrix<T,C> where C: ICalc<T>, new()
    {
        public class ElementCopyAdapter
        {
			private Matrix<T, C> _parent;

            internal ElementCopyAdapter(Matrix<T, C> obj)
            {
                _parent = obj;
            }

			public void ConvertFrom2DArray(T[,] matrix)
            {
				for (int rowIndex = 0; rowIndex < matrix.GetLength(0); ++rowIndex)
				{
					for (int columnIndex = 0; columnIndex < matrix.GetLength(1); ++columnIndex)
					{
						_parent[rowIndex, columnIndex] = calc.GetCopy(matrix[rowIndex, columnIndex]);
					}
				}
            }

			public T[,] ConvertTo2DArray()
            {
                T[,] newMatrix = new T[_parent.RowCount, _parent.ColumnCount];

				for (int rowIndex = 0; rowIndex < _parent.RowCount; rowIndex++)
				{
					for (int columnIndex = 0; columnIndex < _parent.ColumnCount; columnIndex++)
					{
						newMatrix[rowIndex, columnIndex] = calc.GetCopy(_parent[rowIndex, columnIndex]);
					}
				}

                return newMatrix;
            }

            // TODO: depending on the matrix type, do fully independent cloning.
        }
    }
}
