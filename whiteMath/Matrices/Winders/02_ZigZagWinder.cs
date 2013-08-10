using whiteMath.Matrices;

/// <summary>
/// Performs a zig-zag matrix winding/unwinding
/// </summary>
internal sealed class ZigZagWinder : Winder
{
    private int i;
    private int j;

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
    private int direction;

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
    private int celltype;

    /// <summary>
    /// Analyzes the direction basing on where from we have came to the moment
    /// and on what the cell type is.
    /// </summary>
    private void analyzeDirection()
    {
        /// The direction map
        /// 2 3 4
        /// 1   5
        /// 8 7 6
        switch (celltype)
        {
            case 0: if (direction == 4) direction = 4; else direction = 8; break;
            case 1: if (direction == 4) direction = 5; else direction = 8; break;
            case 2:
            case 6: if (direction == 8) direction = 5; else direction = 4; break;
            case 3:
            case 7: direction = 5; break;
            case 4: if (direction == 8) direction = 7; else direction = 4; break;
            case 5: direction = 5; break;
            case 8:
            case 9: if (direction == 4) direction = 7; else direction = 8; break;
            case 10:
            case 11:
            case 14:
            case 15: direction = 0; break;
            case 12:
            case 13: direction = 7; break;
            default: break;
        }
    }

    /// <summary>
    /// Делает один шаг по матрице, в зависимости от определенного направления
    /// Не нужно предусматривать выход за границы - он уже предусмотрен на предыдущем этапе.
    /// </summary>
    private void performStep()
    {
        switch (direction)
        {
            case 4: i--; j++; break;
            case 5: j++; break;
            case 7: i++; break;
            case 8: i++; j--; break;
            default: break;
        }
    }

    /// <summary>
    /// Analyzes the cell type basing on what the surrounding matrix borders are.
    /// </summary>
    private void analyzeCellType()
    {
        celltype = 0;                           // No borders by default
        if (i == 0) celltype++;                 // Upper border
        if (i == rows - 1) celltype += 2;       // Down border
        if (j == 0) celltype += 4;              // Left border
        if (j == columns - 1) celltype += 8;    // Right border
    }

    internal ZigZagWinder(IMatrix matrix) : base(matrix.RowCount, matrix.ColumnCount) { }
    internal ZigZagWinder(int rowCount, int columnCount) : base(rowCount, columnCount) { }

    protected override void formTrace()
    {
        for (int k = 0; k < this.elements; k++)
        {
            trace[k] = new IndexPair(i, j);
            analyzeCellType();
            analyzeDirection();
            performStep();
        }

        i = j = 0;
    }
}

