namespace WhiteMath
{
    /// <summary>
    /// Public interface providing the capability to
    /// convert variables of one type to another.
    /// 
	/// For example, IConvertible{int, double} is expected to convert integer variables 
    /// to double-presicion floating-point variables.
    /// 
    /// It is recommended for numeric conversions that the type T is able to store both the precision and the range of numeric type F.
    /// If precision may be lost, at least the safe range storing is strongly recommended.
    /// </summary>
    /// <typeparam name="F">The type to convert from.</typeparam>
    /// <typeparam name="T">The type to convert to.</typeparam>
    public interface IConvertible<F, T>
    {
        /// <summary>
        /// Converts an F type variable to the T type variable.
        /// </summary>
        /// <param name="variable">The variable to convert to the type T.</param>
        /// <returns>The conversion result of type F.</returns>
        T Convert(F variable);
    }
}
