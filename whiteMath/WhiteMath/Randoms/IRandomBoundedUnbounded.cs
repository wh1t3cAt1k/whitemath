namespace WhiteMath.Randoms
{
    /// <summary>
    /// A generic interface providing means
    /// for generating random numbers either in the whole
    /// numeric range of type <typeparamref name="T"/> or
    /// in a user-specified concrete range.
    /// </summary>
    /// <typeparam name="T">The type of generated numbers.</typeparam>
    public interface IRandomBoundedUnbounded<T> : IRandomBounded<T>, IRandomUnbounded<T> 
	{ }
}
