using WhiteMath.Matrices;

/// <summary>
/// Provides a standard row-by-row winding/unwinding of the matrix.
/// </summary>
internal sealed class RowByRowWinder : Winder
{
    internal RowByRowWinder(IMatrix matrix) : base(matrix.RowCount, matrix.ColumnCount) { }
    internal RowByRowWinder(int rowCount, int columnCount) : base(rowCount, columnCount) { }

    protected override void formTrace()
    {
        for (int i = 0; i < elements; i++)
            trace[i] = new IndexPair(i / rows, i % rows);

        return;
    }
}
