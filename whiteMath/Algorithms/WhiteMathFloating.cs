using System;

using whiteMath.Calculators;

namespace whiteMath.Algorithms
{
    public partial class WhiteMath<T, C> where C: ICalc<T>, new() 
    {
        // --------------------------------
        // --------- DIFFERENT ------------
        // --------------------------------

        /// <summary>
        /// Returns the first number modulo second.
        /// Equivalent to the remainder operation; the formula for the modulus is:
        /// 
        /// MODULUS = sgn(dividend) * (|dividend| - (|divisor| * floor(|dividend| / |divisor|))).
        /// 
        /// May produce non-integer results for floating point numbers, use 
        /// Ceiling() / Floor() or Round() if needed!
        /// </summary>
        /// <see cref="Ceiling"/>
        /// <see cref="Floor"/>
        /// <see cref="Round"/>
        /// <param name="dividend">The number to be divided.</param>
        /// <param name="divisor">The number to be divided by.</param>
        /// <returns>The first operand modulo second operand.</returns>
        public static T Modulus(T dividend, T divisor)
        {
            Numeric<T,C> absdividend = Abs(dividend);
            Numeric<T,C> absdivisor = Abs(divisor);

            Numeric<T,C> zero = Numeric<T,C>.Zero;

            if (dividend == zero)
                return calc.Zero;
            
            else if (divisor == zero)
                throw new DivideByZeroException("Cannot divide by zero.");

            Numeric<T,C> value = absdividend - calc.IntegerPart((absdividend/absdivisor)) * absdivisor;

            return (dividend < zero ? -value : value);
        }

        /// <summary>
        /// Rounds a number to the nearest integer number.
        /// </summary>
        /// <param name="number">The number to be rounded.</param>
        /// <returns>The nearest integer number.</returns>
        public static T Round(T number)
        {
            return calc.IntegerPart(calc.Add(number, calc.FromDouble(0.5)));
        }

        /// <summary>
        /// Returns the nearest lower integer number.
        /// </summary>
        /// <param name="number">The number to be rounded.</param>
        /// <returns>The nearest lower integer number.</returns>
        public static T Floor(T number)
        {
			/*
            Contract.Ensures(number - (Numeric<T, C>)Contract.Result<T>() >= Numeric<T, C>.Zero);
            Contract.Ensures(number - (Numeric<T, C>)Contract.Result<T>() < Numeric<T, C>._1);
			*/

            // Если число отрицательно и содержит дробную часть, напр. -4.5,
            // то необходимо вернуть МЕНЬШЕЕ. Т.е. (-4-1) = -5.

            if (calc.GreaterThan(calc.Zero, number) && calc.GreaterThan(calc.FractionalPart(number), calc.Zero))
                return calc.Subtract(calc.IntegerPart(number), calc.FromInteger(1));

            // Во всех остальных случаях можно забить и возвращать просто целую часть.

            else
                return calc.IntegerPart(number);
        }

        /// <summary>
        /// Returns the nearest bigger integer number.
        /// </summary>
        /// <param name="number">The number to be rounded.</param>
        /// <returns>The nearest bigger integer number.</returns>
        public static T Ceiling(T number)
        {
			/*
            Contract.Ensures((Numeric<T, C>)Contract.Result<T>() - number >= Numeric<T, C>.Zero);
            Contract.Ensures((Numeric<T, C>)Contract.Result<T>() - number < Numeric<T, C>._1);
			*/

            // Если дробной части нет, то надо вернуть само число.

            if(calc.Equal(calc.FractionalPart(number), calc.Zero))
                return calc.GetCopy(number);

            // Если число отрицательно и содержит дробную часть, например -4.5,
            // то можно просто отбросить эту дробную часть и будет нормально :)

            else if (calc.GreaterThan(calc.Zero, number))
                return calc.IntegerPart(number);
            
            // Для положительных чисел и нуля с чем-то - надо вернуть целую часть +1.

            return calc.Add(calc.IntegerPart(number), calc.FromInteger(1));
        }


        /// <summary>
        /// Returns the number raised to the power specified by the user.
        /// Uses the Taylor series.
        /// </summary>
        /// <param name="number">The number to be raised to the power.</param>
        /// <param name="power">The power to be raised to.</param>
        /// <param name="taylorMemberCount">The minimal amount of Taylor series members used in calculations.</param>
        /// <returns></returns>
        public static T Power(T number, T power, int taylorMemberCount = 100)
        {
            // Если степень имеет целую часть, то считаем по формуле:
            // a^(b+c) = a^b * a^c

            T intPart = calc.IntegerPart(power);
            T fracPart = calc.Subtract(power, intPart);

            if (intPart != Numeric<T, C>.Zero)
            {
                // Дробная часть показателя не равна нулю

                if (fracPart != Numeric<T, C>.Zero)
                    return calc.Multiply(
                        PowerInteger_Generic(number, intPart),  // целая часть
                        Power(number, fracPart)                 // дробная часть
                    );

                // Если равна - нам тут нечего делать.
                // Просто возвращаем результат возведения в целочисленную степень.

                else
                    return PowerInteger_Generic(number, intPart);
            }

            // Re-type the objects.

            Numeric<T, C> num = number;
            Numeric<T, C> pow = power;

            // На этом этапе мы в любом случае имеем ненулевой дробный показатель.
            // Поэтому, если число меньше нуля, то мы обязаны вернуть ошибку.

            if (num < Numeric<T,C>.Zero)
                throw new ArgumentException("Only non-negative numbers can be raised to a fractional power.");

            // Если число равно нулю, то есть несколько исходов.
            // Если степень отрицательна, возвращаем ошибку.
            // Если степень нулевая - возвращаем единицу.
            // В противном случае ноль в любой степени дает ноль.

            else if (num == Numeric<T,C>.Zero)
            {
                if (pow < Numeric<T,C>.Zero)
                    throw new ArgumentException("A zero number cannot be raised to a negative power.");
                else if (pow == Numeric<T,C>.Zero)
                    return calc.FromInteger(1);

                return calc.FromInteger(0);
            }
            
            // Если все ОК.

            else if (pow < Numeric<T,C>.Zero)
                return calc.Divide(calc.FromInteger(1), Power(number, calc.Negate(power), taylorMemberCount));
            
            else if (pow == Numeric<T,C>.Zero)
                return calc.FromInteger(1);

            return Exponent(calc.Multiply(power, LogarithmNatural(number, taylorMemberCount)), taylorMemberCount);
        }

        /// <summary>
        /// Returns the exponent of a real (or complex) number.
        /// Uses the Taylor series, user can explicitly specify the amount of members used in calculations.
        /// </summary>
        /// <param name="number">The number whose exponent is to be found.</param>
        /// <param name="taylorMemberCount">The amount of Taylor member series used.</param>
        /// <returns></returns>
        public static T Exponent(T number, int taylorMemberCount = 100)
        {
            T sum = calc.FromInteger(1);

            for (int i = taylorMemberCount - 1; i > 0; i--)
            {
                T memberNumber = calc.FromInteger(i);
                sum = calc.Add(sum, calc.Divide(PowerInteger(number, i), Factorial(memberNumber)));
            }

            return sum;
        }

        /// <summary>
        /// Returns the natural logarithm of a real number.
        /// Uses the Taylor series, user can explicitly specify the amount of members used in calculations.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="taylorMemberCount"></param>
        /// <returns></returns>
        public static T LogarithmNatural(T number, int taylorMemberCount = 60)
        {
            if (calc.Equal(calc.FromInteger(1), number))
                return calc.Zero;
            else if
                (!calc.GreaterThan(number, calc.Zero))
                throw new ArgumentException("The argument passed must be positive.");

            // Если операнд больше двойки, раскладываем в 2^n + остаток
            // Затем складываем полученные логарифмы

            else if (calc.GreaterThan(number, calc.FromInteger(2)))
            {
                T tmp = calc.FromInteger(2);
                int z = 0;

                while (calc.GreaterThan(number, tmp))
                {
                    tmp = calc.Multiply(tmp, calc.FromInteger(2));
                    z++;
                }

                tmp = calc.FromInteger(2);
                return calc.Add(calc.Multiply(calc.FromInteger(z), LogarithmNatural(tmp, taylorMemberCount)), LogarithmNatural(calc.Divide(number, PowerInteger(tmp, z)), taylorMemberCount));
            }

            // если операнд - двойка, факторизуем и складываем логарифмы.
            // ln2 = ln(1.25 * 1.6) = ln(1.25) + ln(1.6).

            else if (calc.Equal(calc.FromInteger(2), number))
                return calc.Add(LogarithmNatural(calc.FromDouble(1.25), taylorMemberCount), LogarithmNatural(calc.FromDouble(1.6), taylorMemberCount));

            // если операнд от 1 до 2 не включая, можно применять разложение в ряд Тейлора

            T sum = calc.Zero;
            T newNum = calc.Subtract(number, calc.FromInteger(1));

            for (int i = taylorMemberCount; i >= 1; i--)
            { 
                T tmp = calc.Divide(PowerInteger(newNum, i), calc.FromInteger(i));

                if (i % 2 != 0)
                    sum = calc.Add(sum, tmp);
                else
                    sum = calc.Subtract(sum, tmp);
            }

            return sum;
        }
    }
}
