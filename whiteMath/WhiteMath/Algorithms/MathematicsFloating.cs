using System;

using WhiteMath.Calculators;

namespace WhiteMath.Mathematics
{
    public partial class Mathematics<T, C> where C: ICalc<T>, new() 
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
                return Calculator.Zero;
            
            else if (divisor == zero)
                throw new DivideByZeroException("Cannot divide by zero.");

            Numeric<T,C> value = absdividend - Calculator.IntegerPart((absdividend/absdivisor)) * absdivisor;

            return (dividend < zero ? -value : value);
        }

        /// <summary>
        /// Rounds a number to the nearest integer number.
        /// </summary>
        /// <param name="number">The number to be rounded.</param>
        /// <returns>The nearest integer number.</returns>
        public static T Round(T number)
        {
            return Calculator.IntegerPart(Calculator.Add(number, Calculator.FromDouble(0.5)));
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

			if (Calculator.GreaterThan(Calculator.Zero, number)
				&& Calculator.GreaterThan(Calculator.FractionalPart(number), Calculator.Zero))
			{
				return Calculator.Subtract(Calculator.IntegerPart(number), Calculator.FromInteger(1));
			}

			// Во всех остальных случаях можно забить и возвращать просто целую часть.

			else
				return Calculator.IntegerPart(number);
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

			if (Calculator.Equal(Calculator.FractionalPart(number), Calculator.Zero))
			{
				return Calculator.GetCopy(number);
			}

			// Если число отрицательно и содержит дробную часть, например -4.5,
			// то можно просто отбросить эту дробную часть и будет нормально :)

			else if (Calculator.GreaterThan(Calculator.Zero, number))
                return Calculator.IntegerPart(number);
            
            // Для положительных чисел и нуля с чем-то - надо вернуть целую часть +1.

            return Calculator.Add(Calculator.IntegerPart(number), Calculator.FromInteger(1));
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

            T intPart = Calculator.IntegerPart(power);
            T fracPart = Calculator.Subtract(power, intPart);

            if (intPart != Numeric<T, C>.Zero)
            {
                // Дробная часть показателя не равна нулю

                if (fracPart != Numeric<T, C>.Zero)
                    return Calculator.Multiply(
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
                    return Calculator.FromInteger(1);

                return Calculator.FromInteger(0);
            }
            
            // Если все ОК.

            else if (pow < Numeric<T,C>.Zero)
                return Calculator.Divide(Calculator.FromInteger(1), Power(number, Calculator.Negate(power), taylorMemberCount));
            
            else if (pow == Numeric<T,C>.Zero)
                return Calculator.FromInteger(1);

            return Exponent(Calculator.Multiply(power, LogarithmNatural(number, taylorMemberCount)), taylorMemberCount);
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
            T sum = Calculator.FromInteger(1);

            for (int i = taylorMemberCount - 1; i > 0; i--)
            {
                T memberNumber = Calculator.FromInteger(i);
                sum = Calculator.Add(sum, Calculator.Divide(PowerInteger(number, i), Factorial(memberNumber)));
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
            if (Calculator.Equal(Calculator.FromInteger(1), number))
                return Calculator.Zero;
            else if
                (!Calculator.GreaterThan(number, Calculator.Zero))
                throw new ArgumentException("The argument passed must be positive.");

            // Если операнд больше двойки, раскладываем в 2^n + остаток
            // Затем складываем полученные логарифмы

            else if (Calculator.GreaterThan(number, Calculator.FromInteger(2)))
            {
                T tmp = Calculator.FromInteger(2);
                int z = 0;

                while (Calculator.GreaterThan(number, tmp))
                {
                    tmp = Calculator.Multiply(tmp, Calculator.FromInteger(2));
                    z++;
                }

                tmp = Calculator.FromInteger(2);
                return Calculator.Add(Calculator.Multiply(Calculator.FromInteger(z), LogarithmNatural(tmp, taylorMemberCount)), LogarithmNatural(Calculator.Divide(number, PowerInteger(tmp, z)), taylorMemberCount));
            }

            // если операнд - двойка, факторизуем и складываем логарифмы.
            // ln2 = ln(1.25 * 1.6) = ln(1.25) + ln(1.6).

            else if (Calculator.Equal(Calculator.FromInteger(2), number))
                return Calculator.Add(LogarithmNatural(Calculator.FromDouble(1.25), taylorMemberCount), LogarithmNatural(Calculator.FromDouble(1.6), taylorMemberCount));

            // если операнд от 1 до 2 не включая, можно применять разложение в ряд Тейлора

            T sum = Calculator.Zero;
            T newNum = Calculator.Subtract(number, Calculator.FromInteger(1));

            for (int i = taylorMemberCount; i >= 1; i--)
            { 
                T tmp = Calculator.Divide(PowerInteger(newNum, i), Calculator.FromInteger(i));

                if (i % 2 != 0)
                    sum = Calculator.Add(sum, tmp);
                else
                    sum = Calculator.Subtract(sum, tmp);
            }

            return sum;
        }
    }
}
