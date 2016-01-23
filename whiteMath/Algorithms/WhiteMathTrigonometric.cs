using whiteMath.Calculators;

namespace whiteMath.Algorithms
{
    public static partial class WhiteMath<T,C> where C: ICalc<T>, new()
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

            T sum = calc.zero;

            for (int i = taylorMemberCount-1; i >=0; --i)
            {
                T tmp = calc.div( 
                            PowerInteger(argument, 2*i + 1), 
                            Factorial(calc.fromInt(2*i + 1)) 
                            );

                if ((i % 2) == 0)
                    sum = calc.sum(sum, tmp);
                else
                    sum = calc.dif(sum, tmp);
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
            T sum = calc.zero;

            for (int i = taylorMemberCount - 1; i >= 0; --i)
            {
                T tmp = calc.div(
                            PowerInteger(argument, 2 * i),
                            Factorial(calc.fromInt(2 * i))
                            );

                if ((i & 2) == 0)
                    sum = calc.sum(sum, tmp);
                else
                    sum = calc.dif(sum, tmp);
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
            return calc.div(sine(argument, taylorMemberCount), cosine(argument, taylorMemberCount));
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
            return calc.div(cosine(argument, taylorMemberCount), sine(argument, taylorMemberCount));
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
            T tmp = calc.getCopy(argument);
            T piByTwo = calc.mul(calc.fromInt(2), pi);

            if (calc.mor(tmp, piByTwo) || calc.mor(calc.negate(piByTwo), tmp))
                tmp = calc.dif(tmp, calc.mul(piByTwo, calc.intPart(calc.div(tmp, piByTwo))));
            
            if (calc.mor(tmp, pi))
                tmp = calc.dif(tmp, piByTwo);
            else if(calc.mor(calc.negate(pi), tmp))
                tmp = calc.sum(tmp, piByTwo);

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
            T tmp = calc.getCopy(argument);
            T piByTwo = calc.mul(calc.fromInt(2), pi);

            while(calc.mor(tmp, pi))
                tmp = calc.dif(tmp, piByTwo);
            
            while(calc.mor(calc.negate(pi), tmp))
                tmp = calc.sum(tmp, piByTwo);

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
            T tmp = calc.getCopy(argument);
            
            if (calc.mor(tmp, pi) || calc.mor(calc.negate(pi), tmp))
            {
                tmp = calc.div(tmp, pi);
                tmp = calc.dif(tmp, calc.intPart(tmp));
                tmp = calc.mul(tmp, pi);
            }

            if (calc.mor(calc.zero, tmp))
                tmp = calc.sum(tmp, pi);

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
            T tmp = calc.getCopy(argument);

            while (calc.mor(tmp, pi))
                tmp = calc.dif(tmp, pi);

            while (calc.mor(calc.zero, tmp))
                tmp = calc.sum(tmp, pi);

            return tmp;
        }
    }
}
