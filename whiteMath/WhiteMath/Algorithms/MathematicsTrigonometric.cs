using WhiteMath.Calculators;

namespace WhiteMath.Mathematics
{
    public static partial class Mathematics<T,C> where C: ICalc<T>, new()
    {
        /// <summary>
        /// Return the sine of the argument using Taylor series.
        /// 
        /// It is strongly recommended that the argument is in range [-pi; pi],
        /// precision may be lost otherwise.
        /// <see cref="sineCosineDivideNormalize"/>
        /// <see cref="sineCosineSubstractNormalize"/>
        /// </summary>
        /// <param name="argument">The number whose sine is to be found.</param>
        /// <param name="taylorMemberCount">The amount of numbers in the taylor series.</param>
        /// <returns>The result of the sine computation.</returns>
        public static T sine(T argument, int taylorMemberCount = 100)
        {
            // Поделить на 2pi
            // отбросить целую часть
            // Умножить на 2pi

            T sum = Calculator.Zero;

            for (int i = taylorMemberCount-1; i >=0; --i)
            {
                T tmp = Calculator.Divide( 
                            PowerInteger(argument, 2*i + 1), 
                            Factorial(Calculator.FromInteger(2*i + 1)) 
                            );

                if ((i % 2) == 0)
                    sum = Calculator.Add(sum, tmp);
                else
                    sum = Calculator.Subtract(sum, tmp);
            }

            return sum;
        }

        /// <summary>
        /// Return the cosine of the argument using Taylor series.
        /// It is strongly recommended that the argument is in range [-pi; pi]
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="taylorMemberCount"></param>
        /// <returns></returns>
        public static T cosine(T argument, int taylorMemberCount = 100)
        {
            T sum = Calculator.Zero;

            for (int i = taylorMemberCount - 1; i >= 0; --i)
            {
                T tmp = Calculator.Divide(
                            PowerInteger(argument, 2 * i),
                            Factorial(Calculator.FromInteger(2 * i))
                            );

                if ((i & 2) == 0)
                    sum = Calculator.Add(sum, tmp);
                else
                    sum = Calculator.Subtract(sum, tmp);
            }
            
            return sum;
        }

        /// <summary>
        /// Returns the tangent of the argument using Taylor series.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="taylorMemberCount"></param>
        /// <returns>The result of tangent computation.</returns>
        public static T Tangent(T argument, int taylorMemberCount = 100)
        {
            return Calculator.Divide(sine(argument, taylorMemberCount), cosine(argument, taylorMemberCount));
        }

        /// <summary>
        /// Returns the cotangent of the argument using Taylor series
        /// using calls to sine and cosine functions.
        /// </summary>
        /// <param name="argument">The number whose cotangent is to be found.</param>
        /// <param name="taylorMemberCount">The amount of Taylor series member for sine and cosine functions.</param>
        /// <returns>The result of cotangent computation.</returns>
        public static T cotangent(T argument, int taylorMemberCount = 100)
        {
            return Calculator.Divide(cosine(argument, taylorMemberCount), sine(argument, taylorMemberCount));
        }

        // ------------------------------------- Sine normalization --------------------------------------

        /// <summary>
        /// Normalizes the number to the period [-pi; pi] as recommended by sine() and cosine() 
        /// methods of the class.
        /// 
        /// Performes dividing the number by 2pi, substracting the integral part, multiplying by 2pi again and
        /// (possibly) shifting to reach the [-pi; pi] boundaries. May be inaccurate with low-precision types,
        /// but only way to use when the argument's absolute value is quite large.
        /// 
        /// <see cref="sineCosineSubstractNormalize"/>
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static T sineCosineDivideNormalize(T argument, T pi)
        {
            T tmp = Calculator.GetCopy(argument);
            T piByTwo = Calculator.Multiply(Calculator.FromInteger(2), pi);

            if (Calculator.GreaterThan(tmp, piByTwo) || Calculator.GreaterThan(Calculator.Negate(piByTwo), tmp))
                tmp = Calculator.Subtract(tmp, Calculator.Multiply(piByTwo, Calculator.IntegerPart(Calculator.Divide(tmp, piByTwo))));
            
            if (Calculator.GreaterThan(tmp, pi))
                tmp = Calculator.Subtract(tmp, piByTwo);
            else if(Calculator.GreaterThan(Calculator.Negate(pi), tmp))
                tmp = Calculator.Add(tmp, piByTwo);

            return tmp;
        }

        /// <summary>
        /// Normalizes the number to the period [-pi; pi] as recommended by sine() and cosine() 
        /// methods of the class.
        /// 
        /// Performed by iterative substraction/addition of the 2pi period from the numbers.
        /// Shows VERY poor performance when the argument's absolute value is large, but
        /// may be more precisive at "near-boundaries" ranges, depending on the type.
        /// 
        /// <see cref="sineCosineDivideNormalize"/>
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static T sineCosineSubstractNormalize(T argument, T pi)
        {
            T tmp = Calculator.GetCopy(argument);
            T piByTwo = Calculator.Multiply(Calculator.FromInteger(2), pi);

            while(Calculator.GreaterThan(tmp, pi))
                tmp = Calculator.Subtract(tmp, piByTwo);
            
            while(Calculator.GreaterThan(Calculator.Negate(pi), tmp))
                tmp = Calculator.Add(tmp, piByTwo);

            return tmp;
        }


        /// <summary>
        /// Normalizes the number to the period [0; pi] as recommended by the tangent() and cotangent()
        /// method of the class.
        /// 
        /// Performes dividing the number by pi, substracting the integral part, multiplying by 2pi again and
        /// (possibly) shifting to reach the [0; pi] boundaries. May be inaccurate with low-precision types,
        /// but only way to use when the argument's absolute value is quite large.
        /// <see cref="tangentCotangentSubstractNormalize"/>
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static T tangentCotangentDivideNormalize(T argument, T pi)
        {
            T tmp = Calculator.GetCopy(argument);
            
            if (Calculator.GreaterThan(tmp, pi) || Calculator.GreaterThan(Calculator.Negate(pi), tmp))
            {
                tmp = Calculator.Divide(tmp, pi);
                tmp = Calculator.Subtract(tmp, Calculator.IntegerPart(tmp));
                tmp = Calculator.Multiply(tmp, pi);
            }

            if (Calculator.GreaterThan(Calculator.Zero, tmp))
                tmp = Calculator.Add(tmp, pi);

            return tmp;
        }

        /// <summary>
        /// Normalizes the number to the period [-pi; pi] as recommended by tangent() and cotangent() 
        /// methods of the class.
        /// 
        /// Performed by iterative substraction/addition of the pi period from the numbers.
        /// Shows VERY poor performance when the argument's absolute value is large, but
        /// may be more precisive at "near-boundaries" ranges, depending on the type.
        /// 
        /// <see cref="sineCosineDivideNormalize"/>
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static T tangentCotangentSubstractNormalize(T argument, T pi)
        {
            T tmp = Calculator.GetCopy(argument);

            while (Calculator.GreaterThan(tmp, pi))
                tmp = Calculator.Subtract(tmp, pi);

            while (Calculator.GreaterThan(Calculator.Zero, tmp))
                tmp = Calculator.Add(tmp, pi);

            return tmp;
        }
    }
}
