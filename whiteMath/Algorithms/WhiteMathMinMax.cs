using whiteMath.Calculators;

// This code file contains a part of whiteMath class
// devoted to:
//     1. Finding the minimum of two numbers
//     2. Finding the maximum of two numbers
//     3. Finding the absolute of two numbers

namespace whiteMath.Algorithms
{
    public static partial class WhiteMath<T, C> where C : ICalc<T>, new()
    {
        /// <summary>
        /// Finds the minimum of two numbers.
        /// 
        /// If T is a reference type, please notice that no new objects are created
        /// during this procedure, consider calling MinCopy() if necessary.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static T Min(T one, T two)
        {
            return (calc.mor(one, two) ? two : one);
        }

        /// <summary>
        /// Finds the minimum of two numbers and returns its copy.
        /// Safe for use with reference types - no changes made
        /// to the value returned will be reflected on any of the arguments.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static T MinCopy(T one, T two)
        {
            return calc.getCopy(Min(one, two));
        }

        /// <summary>
        /// Finds the maximum of two numbers.
        /// 
        /// If T is a reference type, please notice that no numbers are created
        /// during this procedure, consider calling MaxCopy() if necessary.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static T Max(T one, T two)
        {
            return (calc.mor(one, two) ? one : two);
        }

        /// <summary>
        /// Finds the maximum of two numbers and returns its copy.
        /// Safe for use with reference types - no changes made
        /// to the value returned will be reflected on any of the arguments.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static T MaxCopy(T one, T two)
        {
            return calc.getCopy(Max(one, two));
        }

        /// ABSOLUTE VALUE
        /// 
        /// <summary>
        /// Returns the absolute value of a generic number.
        /// The method calls to the calculator's zero field representing
        /// a zero value for the number.
        /// 
        /// If the result of the call to zero() is mor() than the number, 
        /// it is considered "negative", positive otherwise.
        /// 
        /// Safe to use with reference-types, changes on the value returned
        /// won't be reflected on the argument passed.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static T Abs(T number)
        {
            if (calc.mor(calc.zero, number)) return calc.negate(number);
            return calc.getCopy(number);
        }
    }
}
