using WhiteMath;
using WhiteMath.Matrices;

/// <summary>
/// This method provides extension methods for constructing different matrix winders.
/// </summary>
public static class Winders
{
    public static IWinder GetZigZagWinder(this IMatrix matrix)
    {
        return new ZigZagWinder(matrix);
    }

    public static IWinder GetZigZagWinder<T>(this T[,] matrix)
    {
        return new ZigZagWinder(matrix.GetLength(0), matrix.GetLength(1));
    }

    public static IWinder GetRowByRowWinder(this IMatrix matrix)
    {
        return new RowByRowWinder(matrix);
    }

    public static IWinder GetRowByRowWinder<T>(this T[,] matrix)
    {
        return new RowByRowWinder(matrix.GetLength(0), matrix.GetLength(1));
    }

    public static IWinder GetSpiralWinder(this IMatrix matrix)
    {
        return new SpiralWinder(matrix);
    }

    public static IWinder GetSpiralWinder<T>(this T[,] matrix)
    {
        return new SpiralWinder(matrix.GetLength(0), matrix.GetLength(1));
    }

    public static IWinder GetChaoticWinder(this IMatrix matrix)
    {
        return new ChaoticWinder(matrix);
    }

    public static IWinder GetChaoticWinder<T>(this T[,] matrix)
    {
        return new ChaoticWinder(matrix.GetLength(0), matrix.GetLength(1));
    }
}
