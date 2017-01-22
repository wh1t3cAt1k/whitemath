using WhiteMath.Matrices;

/// <summary>
/// TODO: check if it works all right
/// </summary>
internal sealed class SpiralWinder : Winder
{
    int count;
    int i, j;

    internal SpiralWinder(IMatrix matrix) : base(matrix.RowCount, matrix.ColumnCount) { }
    internal SpiralWinder(int rowCount, int columnCount) : base(rowCount, columnCount) { }

    protected override void formTrace()
    {
        count = elements;
        i = j = 0;

        bool wereSteppingDown;
        bool wereSteppingRight;
        int level = 0;

        do
        {
            wereSteppingDown = false;
            wereSteppingRight = false;

			// Stepping right at upper border.
			// -
			if (j < columns - level - 1)
			{
				wereSteppingRight = true;
			}

            while (j < columns - level - 1)
            {
                trace[elements - (count--)] = new IndexPair(i, j);
                j++;
            }

			// Stepping down at rightmost border.
			// -
			if (i < rows - level - 1)
			{
				wereSteppingDown = true;
			}

            while (i < rows - level - 1)
            {
                trace[elements - (count--)] = new IndexPair(i, j);
                i++;
            }

            // Stepping left at downmost border.
			// -
            if (wereSteppingDown && wereSteppingRight)
            {
                while (j > level)
                {
                    trace[elements - (count--)] = new IndexPair(i, j);
                    j--;
                }
            }

            // Stepping up at leftmost border.
			// -
            if (wereSteppingDown && wereSteppingRight)
            {
                while (i > level + 1)
                {
                    trace[elements - (count--)] = new IndexPair(i, j);
                    i--;
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
			trace[elements - 1] = new IndexPair(i, j);
		}

        return;
    }
}
