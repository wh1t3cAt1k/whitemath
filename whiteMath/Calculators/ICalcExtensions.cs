using System;

namespace whiteMath.Calculators
{
    /// <summary>
    /// This static class provides different extension methods for 
    /// the ICalc(T) interface.
    /// </summary>
    public static class ICalcExtensionMethods
    {
        /// <summary>
        /// If the current calculator for the <typeparamref name="T"/> type does not support conversions
        /// from the double type AND the <paramref name="num"/> does not exceed the range of long, 
        /// this method will result in smaller number of exceptions, because
        /// if <paramref name="num"/> value contains only integral part, the 'fromInt(int)' calculator method
        /// will be preferred over the 'fromDouble(double)'.
        /// </summary>
        /// <typeparam name="T">The type of numbers for the calculator.</typeparam>
        /// <param name="calculator">The calling calculator object.</param>
        /// <param name="num">The number which is to be converted to the <typeparamref name="T"/> type.</param>
        /// <returns>The <typeparamref name="T"/> value returned by either fromInt (preferred) of fromDouble calculator method.</returns>
        public static T FromDoubleSafe<T>(this ICalc<T> calculator, double num)
        {
			// TODO: use epsilong here.
			// -
            if (Numeric<double, CalcDouble>.Calculator.FractionalPart(num) == 0 
			    && num > long.MinValue 
			    && num < long.MaxValue)
			{
                return calculator.FromInteger((long)num);
			}

            return calculator.FromDouble(num);
        }

        /// <summary>
        /// Returns the fractional part of the number.
        /// </summary>
        /// <typeparam name="T">The type of numbers for the calculator.</typeparam>
        /// <param name="calculator">The calling calculator object.</param>
        /// <param name="num">The number which fractional part is to be returned.</param>
        /// <returns>The fractional part of the number.</returns>
        public static T FractionalPart<T>(this ICalc<T> calculator, T num)
        	=> calculator.Subtract(num, calculator.IntegerPart(num));

        /// <summary>
        /// Checks whether the number is not a NaN or an infinity.
        /// </summary>
		public static bool IsNumber<T>(this ICalc<T> calculator, T num)
        	=> !calculator.IsNaN(num) 
			&& !calculator.IsNegativeInfinity(num) 
			&& !calculator.IsPositiveInfinity(num);

        /// <summary>
        /// Tests whether a number is non-negative according to the calling calculator object.
        /// </summary>
		public static bool IsNonNegative<T>(this ICalc<T> calculator, T num)
        	=> !calculator.GreaterThan(calculator.Zero, num);

        /// <summary>
        /// Tests whether a number is non-positive according to the calling calculator object.
        /// </summary>
		public static bool IsNonPositive<T>(this ICalc<T> calculator, T num)
        	=> !calculator.GreaterThan(num, calculator.Zero);

        /// <summary>
        /// Tests whether a number is positive according to the calling calculator object.
        /// </summary>
		public static bool IsPositive<T>(this ICalc<T> calculator, T num)
        	=> calculator.GreaterThan(num, calculator.Zero);
        

        /// <summary>
        /// Tests whether a number is negative according to the calling calculator object.
        /// </summary>
        public static bool IsNegative<T>(this ICalc<T> calculator, T num)
        	=> calculator.GreaterThan(calculator.Zero, num);

        /// <summary>
        /// Checks whether the number is an infinity.
        /// </summary>
        /// <typeparam name="T">The type of the number.</typeparam>
        /// <param name="num">The number to be tested.</param>
        /// <param name="calculator">A calculator for the <typeparamref name="T"/> type.</param>
        /// <returns>True if the number is an infinity according to the <paramref name="calculator"/>, false othewise.</returns>
		public static bool IsInfinity<T>(this ICalc<T> calculator, T num)
        	=> calculator.IsPositiveInfinity(num) 
			|| calculator.IsNegativeInfinity(num);

        /// <summary>
		/// Tries to parse the string value into a number of type <typeparamref name="T"/>.
        /// If successful, returns true and sets the 'res' parameter to the parsed value.
        /// False is returned otherwise; no exceptions are thrown.
        /// </summary>
        /// <typeparam name="T">The type of the number.</typeparam>
        /// <param name="calculator">A calculator for the <typeparamref name="T"/> type.</param>
        /// <param name="value">The value to be parsed into a <typeparamref name="T"/> variable.</param>
        /// <param name="result">A variable to store the result. Will be null or zero in case of a failure.</param>
        /// <returns>True if the conversion succeeded, false otherwise.</returns>
		public static bool TryParse<T>(this ICalc<T> calculator, string value, out T result)
        {
            try
            {
                result = calculator.Parse(value);
            }
            catch
            {
                result = default(T);
                return false;
            }

            return true;
        }
    }

}
