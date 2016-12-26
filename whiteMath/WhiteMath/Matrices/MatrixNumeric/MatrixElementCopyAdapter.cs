using WhiteMath.Calculators;

namespace WhiteMath.Matrices
{
    public abstract partial class Matrix<T,C> where C: ICalc<T>, new()
    {
        public class ElementCopyAdapter
        {
            private Matrix<T, C> parent;

            internal ElementCopyAdapter(Matrix<T, C> obj)
            {
                this.parent = obj;
            }

            public void convertFromArray(T[,] matrix)
            {
                T[,] newMatrix = new T[matrix.GetLength(0), matrix.GetLength(1)];

                for (int i = 0; i < matrix.GetLength(0); i++)
                    for (int j = 0; j < matrix.GetLength(1); j++)
                        parent[i, j] = calc.GetCopy(matrix[i, j]);
            }

            public T[,] convertToArray()
            {
                T[,] newMatrix = new T[parent.RowCount, parent.ColumnCount];

                for (int i = 0; i < parent.RowCount; i++)
                    for (int j = 0; j < parent.ColumnCount; j++)
                        newMatrix[i, j] = calc.GetCopy(parent[i, j]);

                return newMatrix;
            }

            // TODO: depending on the matrix type, do fully independent cloning.
        }
    }
}
