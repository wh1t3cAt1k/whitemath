using System;
using System.Collections.Generic;

using WhiteMath.Mathematics;
using WhiteMath.ArithmeticLong.Infrastructure;
using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Randoms;

using WhiteStructs.Conditions;
using WhiteStructs.Collections;

namespace WhiteMath.ArithmeticLong
{
    /// <summary>
    /// Represents a long integer number with theoretically unlimited precision.
    /// </summary>
    [Serializable]
    public partial class LongInt<B>: ICloneable 
		where B: IBase, new()
    {
        public static readonly int BASE = new B().Base;

        public static bool IsBasePowerOfTen { get; private set; }
        public static bool IsBasePowerOfTwo { get; private set; }

        private static readonly string digitFormatter = GetDigitFormatter();

		// The length of one digit in decimal or binary places.
		// At the same time, denotes the power of two or ten
		// which is equal to BASE.
		// -
        private static readonly int fieldLength = -1;

        #region Standard exceptions

        private static NotSupportedException NOT_SUPPORTED_DECIMAL_EXCEPTION =
            new NotSupportedException("The digits base for this number is not an integer power of ten. The operation is not supported.");

        private static NotSupportedException NOT_SUPPORTED_BINARY_EXCEPTION =
            new NotSupportedException("The digits base for this number is not an integer power of two. The operation is not supported.");

        #endregion

        /// <summary>
        /// Returns the length of the current number in decimal digits.
        /// Works only for numbers whose digits base is an integer power of ten.
        /// </summary>
        public long LengthInDecimalPlaces
        {
            get
            {
				if (!IsBasePowerOfTen)
				{
					throw NOT_SUPPORTED_DECIMAL_EXCEPTION;
				}

                return LongInt<B>.fieldLength * (this.Length - 1) + (int)Math.Ceiling(Math.Log10(this[this.Length - 1]));
            }
        }

        /// <summary>
        /// Returns the length of the current number in binary digits.
        /// Works ONLY for numbers whose digits base is an integer power of two.
        /// </summary>
        public long LengthInBits
        {
            get
            {
                if (!IsBasePowerOfTwo)
                    throw NOT_SUPPORTED_BINARY_EXCEPTION;

                return LongInt<B>.fieldLength * (this.Length - 1) + (int)Math.Ceiling(Math.Log(this[this.Length - 1], 2));
            }
        }

        /// <summary>
        /// Returns the amount of decimal places contained in a single numeric digit of this number.
        /// Works only if the base of the number is an integer power of ten, otherwise,
        /// a NotSupportedException shall be thrown.
        /// </summary>
		public static int DecimalPlacesPerDigit
        {
            get
            {
                if (!IsBasePowerOfTen)
                    throw NOT_SUPPORTED_DECIMAL_EXCEPTION;

                return fieldLength;
            }
        }

        /// <summary>
        /// Returns the amount of binary places contained in a single numeric digit of this number.
        /// Works only if the base of the number is an integer power of two, otherwise,
        /// a NotSupportedException shall be thrown.
        /// </summary>
		public static int BinaryPlacesPerDigit
        {
            get
            {
                if (!IsBasePowerOfTwo)
                    throw NOT_SUPPORTED_BINARY_EXCEPTION;

                return fieldLength;
            }
        }
       
		#region Object State

        /// <summary>
		/// Returns the length of the number in <see cref="BASE"/>-based digits.
        /// </summary>
        public int Length { get { return Digits.Count; } }

        /// <summary>
        /// Returns the digits list for the current number.
        /// </summary>
        internal List<int> Digits { get; private set; }

		/// <summary>
		/// Gets the list of <see cref="BASE"/>-based digits 
		/// of the current number.
		/// </summary>
		public IReadOnlyList<int> DigitList => Digits;

        /// <summary>
        /// Returns <c>true</c> if the current number is negative.
        /// </summary>
        public bool IsNegative { get; private set; }

        /// <summary>
        /// Gets a flag signalizing whether the current number
        /// is even, i.e. has a zero remainder after division by two.
        /// </summary>
        public bool IsEven => LongIntegerMethods.IsEven(BASE, this.Digits);

        /// <summary>
        /// Gets or sets the value of the digit at the specified position.
        /// Smaller indices correspond to less significant digits, so the 
		/// <c>i</c>'th digit corresponds to the <c>i</c>th power of 
		/// <see cref="BASE"/>.
        /// </summary>
        /// <param name="digitIndex">The index of the desired number.</param>
        /// <returns>The digit on the zero-based position <paramref name="digitIndex"/>.</returns>
        public int this[int digitIndex]
        {
            get { return Digits[digitIndex]; }
            set { Digits[digitIndex] = value; }
        }

		#endregion

		#region Constructors

        /// <summary>
		/// Static initializer. Checks if the base is a whole integer power
		/// of two or ten.
        /// </summary>
        static LongInt()
        {
            int? isPower;

            if (Mathematics<int, CalcInt>.IsNaturalIntegerPowerOf(BASE, 10, out isPower))
            {
                LongInt<B>.IsBasePowerOfTen = true;
                LongInt<B>.fieldLength = isPower.Value;
            }
            else if (Mathematics<int, CalcInt>.IsNaturalIntegerPowerOf(BASE, 2, out isPower))
            {
                LongInt<B>.IsBasePowerOfTwo = true;
                LongInt<B>.fieldLength = isPower.Value;
            }
        }

        /// <summary>
        /// Parameterless constructor, sets the zero value.
        /// </summary>
        public LongInt()
        {
            this.IsNegative = false;
            this.Digits = new List<int>() { 0 };
        }

        /// <summary>
        /// The constructor designed to create a pseudo-random LongInt
        /// number, using a positive integer amount of BASE-based digits specified by the user.
        /// </summary>
        /// <param name="generator">The IRandom generator for the digits.</param>
        /// <param name="digitCount">The amount of BASE-based digits for the number.</param>
        /// <param name="allowNegative">An optional parameter which signalizes whether negative numbers should be allowed.</param>
        public LongInt(int digitCount, IRandomBounded<int> generator, bool allowNegative = false)
        {
			Condition.ValidatePositive(digitCount, "The number of digits should be positive.");

            if(allowNegative)
                this.IsNegative = (generator.Next(0, 2) == 0 ? false : true);

            this.Digits = new List<int>(digitCount);

            for (int i = 0; i < digitCount - 1; i++)
                this.Digits.Add(generator.Next(0, BASE));

            this.Digits.Add(generator.Next(1, BASE));   // последняя цифра не должна быть нулевой
        }

        /// <summary>
        /// This constructor is designed to create a LongInt number
        /// using a positive amount of BASE-based digits, filling the number with a fixed specified digit.
        /// </summary>
        /// <param name="digitCount">The amount of BASE-based digits for the number.</param>
        /// <param name="digit">The digit to fill the number with.</param>
        /// <param name="negative">If this flag is true, the number will be considered negative.</param>
        public LongInt(int digitCount, int digit, bool negative = false)
        {
			Condition.ValidatePositive(digitCount, "The number of digits should be positive.");
			Condition
				.Validate(digit < BASE)
				.OrArgumentOutOfRangeException("The digit to fill the number should be less than BASE.");

            this.Digits = new List<int>(digitCount);
            this.IsNegative = negative;

            this.Digits.FillByAppending(digitCount, digit);
        }

		#endregion

        /// <summary>
        /// The constructor designed to create a pseudo-random LongInt
        /// number, using an integer amount of DECIMAL digits specified by the user.
        /// 
        /// For example, if the BASE of the current number is 10 000, and
        /// <paramref name="decimalDigitCount"/> is 5, the number would contain
        /// two 10000-based digits.
        /// </summary>
        /// <remarks>The method works ONLY when the <c>BASE</c> is an exact integer power of ten.</remarks>
        /// <exception cref="NotSupportedException">This method will throw if the BASE is not an exact integer power of ten.</exception>
        /// <param name="digitGenerator">The IRandom implementation to generate uniformly distributed integer values.</param>
        /// <param name="decimalDigitCount">The amount of decimal digits for the number to be generated.</param>
        /// <param name="allowNegative">The flag which signalizes if negative values are allowed.</param>
        /// <returns>A pseudo-random LongInt number with the desired amount of decimal digits.</returns>
		public static LongInt<B> CreateRandomByNumberOfDecimalDigits(
			int decimalDigitCount, 
			IRandomBounded<int> digitGenerator, 
			bool allowNegative)
        {
            if (!IsBasePowerOfTen)
                throw NOT_SUPPORTED_DECIMAL_EXCEPTION;

            int best = decimalDigitCount / fieldLength;         // how many BASE-based digits are there
            int rest = decimalDigitCount % fieldLength;         // additional digit may be presented

			LongInt<B> result = new LongInt<B>();

			if (allowNegative)
			{
				result.IsNegative = (digitGenerator.Next(0, 2) == 0 ? false : true);
			}
			else
			{
				result.IsNegative = false;
			}

            result.Digits = new List<int>(best + (rest > 0 ? 1 : 0));

			for (int digitIndex = 0; digitIndex < best; ++digitIndex)
			{
				result.Digits.Add(digitGenerator.Next(0, BASE));
			}

			if (rest > 0)
			{
				result.Digits.Add(digitGenerator.Next(1, Mathematics<int, CalcInt>.PowerInteger(10, rest)));
			}
			else if (result.Digits[result.Digits.Count - 1] == 0)
			{
				result.Digits[result.Digits.Count - 1] = digitGenerator.Next(1, BASE);
			}

            return result;
        }

        /// <summary>
        /// The constructor designed to create a pseudo-random LongInt
        /// number, using an integer amount of BINARY digits (bits) specified by the user.
        /// 
        /// For example, if the BASE of the current number is 256, and
        /// <paramref name="binaryDigitCount"/> is 9, the number would contain
        /// two 256-based digits.
        /// </summary>
        /// <remarks>The method works ONLY for bases which are an exact integer power of two.</remarks>
        /// <exception cref="NotSupportedException">This method will throw if the <c>BASE</c> is not an exact integer power of two.</exception>
        /// <param name="generator">The IRandom implementation to generate uniformly distributed integer values.</param>
        /// <param name="binaryDigitCount">The amount of decimal digits for the number to be generated.</param>
        /// <param name="allowNegative">The flag which signalizes if negative values are allowed.</param>
        /// <returns>A pseudo-random LongInt number with the desired amount of binary digits (bits).</returns>
		public static LongInt<B> CreateRandomByNumberOfBits(int binaryDigitCount, IRandomBounded<int> generator, bool allowNegative)
        {
            if (!IsBasePowerOfTwo)
                throw NOT_SUPPORTED_BINARY_EXCEPTION;

            int best = binaryDigitCount / fieldLength;         // how many BASE-based digits are there
            int rest = binaryDigitCount % fieldLength;         // additional digit may be presented

            LongInt<B> tmp = new LongInt<B>();

            if (allowNegative)
                tmp.IsNegative = (generator.Next(0, 2) == 0 ? false : true);
            else
                tmp.IsNegative = false;

            tmp.Digits = new List<int>(best + (rest > 0 ? 1 : 0));

            for (int i = 0; i < best; i++)
                tmp.Digits.Add(generator.Next(0, BASE));

            if (rest > 0)
                tmp.Digits.Add(generator.Next(1, Mathematics<int, CalcInt>.PowerInteger(2, rest)));
            else if (tmp.Digits[tmp.Digits.Count - 1] == 0)
                tmp.Digits[tmp.Digits.Count - 1] = generator.Next(1, BASE);

            return tmp;
        }

        /// <summary>
        /// Creates a <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> number which
        /// is a desired power of <c>BASE</c>.
        /// </summary>
        /// <param name="basePower">The desired non-negative power of <c>BASE</c>.</param>
        /// <returns>
        /// A <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> number which
        /// is a desired power of <c>BASE</c>.
        /// </returns>
        public static LongInt<B> CreatePowerOfBase(int basePower)
        {
			Condition.ValidateNonNegative(basePower, "The power of BASE should not be negative.");

            LongInt<B> result = new LongInt<B>(basePower + 1);
            result.Digits.AddRange(new int[basePower + 1]);

            result.Digits[basePower] = 1;

            return result;
        }

        /// <summary>
        /// Initializes the LongInt number with a user-specified digit list.
        /// If the BASE of digits in <paramref name="digits"/> is other than LongInt.BASE,
        /// the conversion may take longer time.
        /// </summary>
        /// <param name="digitsBase">The base of digits in the digit list.</param>
        /// <param name="digits">A list of digits with increasing significance (the leftmost digit corresponds to BASE^0).</param>
        /// <param name="negative">A parameter which signalizes if the number should be treated as negative.</param>
		public LongInt(int digitsBase, IList<int> digits, bool negative = false)
        {
			Condition.Validate(digitsBase > 1);

            if (digitsBase == LongInt<B>.BASE)
                this.Digits = new List<int>(digits);
            else
				this.Digits = new List<int>(BaseConversion.BaseConvert(digits.AsReadOnly(), digitsBase, LongInt<B>.BASE));

            this.IsNegative = negative;
            this.DealWithZeroes();
        }

        /// <summary>
        /// Private constructor made for quick-trick.
        /// </summary>
        private LongInt(int initialSize)
        {
            this.IsNegative = false;
			this.Digits = new List<int>(initialSize);
        }

        /// <summary>
        /// Makes a LongInt from the standard long.
        /// </summary>
        /// <param name="number">
		/// The long number to convert into LongInt.
		/// </param>
		public LongInt(long number)
        {
            this.IsNegative = (number < 0);
            
			number = (number > 0 ? number : -number);

			// TODO: might serve us well to guess the
			// initial capacity here.
			// -
            this.Digits = new List<int>();

            do
            {
                Digits.Add((int)(number % BASE));
                number /= BASE;
            }
            while (number > 0);
        }

		#region Conversion Operators

        /// <summary>
        /// Performs an unchecked conversion of a LongInt number into a primitive long.
        /// </summary>
        /// <exception cref="OverflowException">May result in an overflow exception.</exception>
        /// <returns>A long number which is equal to the current number.</returns>
        public static explicit operator long(LongInt<B> num)
        {
            long res = num.Digits[num.Length - 1];

            for (int i = num.Length - 2; i >= 0; --i)
            {
                res *= LongInt<B>.BASE;
                res += num.Digits[i];
            }

            return (num.IsNegative ? -res : res);
        }

        /// <summary>
		/// Performs an implicit conversion of a long number 
		/// into <see cref="LongInt{B}"/>.
        /// </summary>
        /// <param name="num">The long number to convert.</param>
        /// <returns>The LongInt number.</returns>
        public static implicit operator LongInt<B>(long num)
        {
            return new LongInt<B>(num);
        }

		#endregion

		#region Arithmetic Operators

        public static LongInt<B> operator +(LongInt<B> one, LongInt<B> two)
        {
			// TODO: this is not quite thread-safe?
			// -
            if (!one.IsNegative && two.IsNegative)
            {
                two.IsNegative ^= true;
                LongInt<B> tmp = one - two;
                two.IsNegative ^= true;

                return tmp;
            }
            else if (one.IsNegative && !two.IsNegative)
            {
                one.IsNegative ^= true;
                LongInt<B> tmp = (two - one);
                one.IsNegative ^= true;

                return tmp;
            }

            int maxLength = Math.Max(one.Length, two.Length);
            int minLength = Math.Min(one.Length, two.Length);

            LongInt<B> res = new LongInt<B>(maxLength + 1);
            res.Digits.AddRange(new int[maxLength + 1]);
            res.IsNegative = one.IsNegative && two.IsNegative;        // negative only if both are negative,
                                                                // other cases at (*)
            LongIntegerMethods.Sum(LongInt<B>.BASE, res.Digits, one.Digits, two.Digits);
            res.DealWithZeroes();

            return res;
        }

        public static LongInt<B> operator -(LongInt<B> one, LongInt<B> two)
        {
            if (!one.IsNegative && two.IsNegative)
            {
                two.IsNegative ^= true;
                LongInt<B> tmp = one + two;
                two.IsNegative ^= true;

                return tmp;
            }
            else if (one.IsNegative && !two.IsNegative)
            {
                one.IsNegative ^= true;
                LongInt<B> tmp = one + two;
                one.IsNegative ^= true;

                tmp.IsNegative = true;
                return tmp;
            }

			LongInt<B> larger, smaller;

			bool firstBiggerThanSecondAbsolute 
				= CompareAbsolute(one, two, out larger, out smaller);

            // Here comes the subtracting.
			// -
			LongInt<B> result = new LongInt<B>(larger.Length);
            result.Digits.AddRange(new int[larger.Length]);

			if (LongIntegerMethods.Subtract(
				BASE, 
				result.Digits, 
				larger.Digits, 
				smaller.Digits)) 
			{
				throw new Exception("Something really terrible happened. Consider it Apocalypse.");
			}

			// Deal with the negative flag.
			// -
			result.IsNegative = !(
				firstBiggerThanSecondAbsolute && !one.IsNegative && !two.IsNegative
				|| !firstBiggerThanSecondAbsolute && one.IsNegative && two.IsNegative);

            result.DealWithZeroes();
            return result;
        }

        /// <summary>
		/// Multiplies one <see cref="LongInt{B}"/> number by another
		/// and returns a number representing their product.
        /// </summary>
        public static LongInt<B> operator *(LongInt<B> one, LongInt<B> two)
        {
            return Helper.MultiplySimple(one, two);
        }

        public static LongInt<B> operator /(LongInt<B> one, int two)
        {
            LongInt<B> res = new LongInt<B>(one.Length);
            res.Digits.AddRange(new int[one.Length]);
            res.IsNegative = one.IsNegative ^ (two < 0);

            if (two < 0) two = -two;

            LongIntegerMethods.DivideByInteger(LongInt<B>.BASE, res.Digits, one.Digits, two);

            // the remainder equals the overall remainder
            // cut the leading zeroes

            res.DealWithZeroes();
            return res;
        }

        public static LongInt<B> operator /(LongInt<B> one, LongInt<B> two)
        {
            return Helper.Divide(one, two);
        }

        public static int operator %(LongInt<B> one, int two)
        {
            int remainder = (one.IsNegative ? -1 : 1);
            if (two < 0) two = -two;

            remainder *= LongIntegerMethods.DivideByInteger(LongInt<B>.BASE, new int[one.Length], one.Digits, two);

            return remainder;
        }

        public static LongInt<B> operator %(LongInt<B> one, LongInt<B> two)
        {
            LongInt<B> remainder;
            LongInt<B>.Helper.Divide(one, two, out remainder);

            return remainder;
        }

        /// <summary>
        /// Unary minus operator. Negates the number.
        /// </summary>
        public static LongInt<B> operator -(LongInt<B> num)
        {
			LongInt<B> result = num.Clone() as LongInt<B>;
            result.IsNegative ^= true;

            return result;
        }

        /// <summary>
        /// Increments the number by one.
        /// </summary>
		public static LongInt<B> operator ++(LongInt<B> number)
        {
            int currentDigitIndex = 0;

			// Special treating of negative numbers. 
			// -
            if (number.IsNegative)
            {
                number.IsNegative = false;
                number--;
                
				if (number < 0) number.IsNegative = true;

                return number;
            }

			// Increment the current digit. If it overflows, we switch to 
			// the next one and increment it. We repeat this process until
			// some digit yields without overflowing, or until we run out 
			// of digits.
            // -
            while (currentDigitIndex < number.Length)
            {
                number[currentDigitIndex]++;

                if (number[currentDigitIndex] == BASE)
                {
                    number[currentDigitIndex] = 0;
                    currentDigitIndex++;
                }
                else return number;
            }

            // Edge case - we have run out of digits.
			// Adding new, zeroing out all others.
            // -
            number.Digits.Add(1);

			for (int digitIndex = 0; digitIndex < number.Length - 1; ++digitIndex)
			{
				number[digitIndex] = 0;
			}

            return number;
        }

        /// <summary>
        /// Decrements the current number by one.
        /// </summary>
        /// <param name="num">The number to be decremented.</param>
        /// <returns>The original number in the decremented state.</returns>
        public static LongInt<B> operator --(LongInt<B> num)
        {
            int currentDigitIndex = 0;

			// If the number is negative, its modulus should be
			// incremented insted.
            // -
            if(num.IsNegative)
            {
                num.IsNegative = false;
                ++num;
                num.IsNegative = true;
                
                return num;
            }

            // Уменьшаем текущую цифру числа на единицу.
            // Если получилось, оставляем в покое.
            // Если получилось меньше нуля, переходим в следующий разряд, пока не кончатся цифры

            while (currentDigitIndex < num.Length)
            {
                int newDigit = num[currentDigitIndex] - 1;

                if (newDigit < 0)
                {
                    num[currentDigitIndex] = BASE - 1;
                    currentDigitIndex++;
                }
                else
                {
                    num[currentDigitIndex] = newDigit;

					// The elder digits could have become zero
					// as a result of borrowing. Need to clean up.
                    // -
                    if (num[num.Length - 1] == 0)
                    {
                        num.DealWithZeroes();
                    }

                    return num;
                }
            }

			// Edge case - we have ran out of digits.
			// This means that the number is (-1).
            // -
            num.Digits.Clear();
            num.Digits.Add(1);
            num.IsNegative = true;

            return num;
        }

		#endregion

		#region Fast Multiplication by a BASE root

        /// <summary>
        /// If the base of the current number digits is an integer power of some value <paramref name="rootValue"/>,
        /// then this method, provided with information on which power it is exactly (<paramref name="rootDegree"/>),
        /// will return the current number multiplied by <paramref name="rootValue"/>^<paramref name="rootPower"/>.
        /// 
        /// Is QUICK for any digit bases. Takes as much as O(n) time to perform.
        /// 
        /// For example, if the base of number digits is 125, you can easily perform
        /// a multiplication by 25, because the base is a power of five, and 25 is 5^2.
        /// 
        /// Thus the result would be [current num] * 5^2 - and calculated quickly.
        /// 
        /// WARNING: the method will return totally wrong values if the base of the digits for current number
        /// is not an exact integer power of <paramref name="rootValue"/>.
        /// </summary>
        /// <param name="rootValue">The number which is, being powered to <paramref name="rootDegree"/>, equals to the current number's digits base.</param>
        /// <param name="rootDegree">The exponent of the <paramref name="rootValue"/> which makes the expression <paramref name="rootValue"/>^<paramref name="rootDegree"/> be equal to the current base.</param>
        /// <param name="rootPower">The value of <paramref name="rootValue"/>'s integer power to multiply by. For example, if <paramref name="rootPower"/> is 2 and <paramref name="rootValue"/> is 5, the multiplication by 25 will be performed.</param>
        /// <returns>The result of current number's multiplication by <paramref name="rootValue"/>^<paramref name="rootPower"/>.</returns>
        public LongInt<B> BaseRootMultiply(int rootValue, int rootDegree, int rootPower)
        {
            // TODO: if power < 0?
            // -
            LongInt<B> tmp = this.Clone() as LongInt<B>;

            // Если степень числа не кратна длине цифры, которая и равна logValue, придется
            // работать дополнительно.
            // -
            if (rootPower % rootDegree > 0)
            {
                int remPower = Mathematics<int, CalcInt>.PowerInteger(rootValue, rootPower % rootDegree);

                long product = tmp.Digits[tmp.Length - 1] * remPower;

                // добавлена ли еще одна цифра по основанию BASE.
                // -
                bool digitAdded = false;    

                if (product > BASE)
                {
                    tmp.Digits[tmp.Length - 1] = (int)(product % BASE);
                    tmp.Digits.Add((int)(product / BASE));

                    digitAdded = true;
                }
                else
                    tmp.Digits[tmp.Length - 1] = (int)product;

                for (int i = tmp.Length - 2 - (digitAdded ? 1 : 0); i >= 0; i--)
                {
                    product = tmp.Digits[i] * remPower;

                    if (product > BASE)
                    {
                        tmp.Digits[i] = (int)(product % BASE);
                        tmp.Digits[i+1] += ((int)(product / BASE));
                    }
                }
            }

            // То, что кратно - просто добавляем в начало числа
            // как нули.
            // -
            tmp.Digits.InsertRange(0, new int[rootPower / rootDegree]);

            return tmp;
        }

        /// <summary>
        /// If the base of the current number digits is an integer power of some value <paramref name="rootValue"/>,
        /// then this method, provided with information on which power it is exactly (<paramref name="rootDegree"/>),
        /// will return the current number integrally divided by <paramref name="rootValue"/>^<paramref name="rootPower"/>.
        /// 
        /// Is QUICK for any digit bases. Takes as much as O(n) time to perform.
        /// 
        /// For example, if the base of number digits is 125, you can easily perform
        /// a division by 25, because the base is a power of five, and 25 is 5^2.
        /// 
        /// Thus the result would be [current num] / 5^2 - and calculated quickly.
        /// 
        /// WARNING: the method will return totally wrong values if the base of current number's digits
        /// is not an exact integer power of <paramref name="rootValue"/>.
        /// </summary>
        /// <param name="rootValue">The number which is, being powered to <paramref name="rootDegree"/>, equals to the current number's digits base.</param>
        /// <param name="rootDegree">The exponent of the <paramref name="rootValue"/> which makes the expression <paramref name="rootValue"/>^<paramref name="rootDegree"/> be equal to the current base.</param>
        /// <param name="rootPower">The value of <paramref name="rootValue"/>'s integer power to divide by. For example, if <paramref name="multiplyByRootPower"/> is 2 and <paramref name="rootValue"/> is 5, the division by 25 will be performed.</param>
        /// <returns>The result of current number's division by <paramref name="rootValue"/>^<paramref name="rootPower"/>.</returns>
        public LongInt<B> BaseRootDivide(int rootValue, int rootDegree, int rootPower)
        {
            if (rootPower < 0) 
            { 
                return BaseRootMultiply(rootValue, rootDegree, -rootPower); 
            }

            // Create a copy of the current number.
            // -
            LongInt<B> result = this.Clone() as LongInt<B>;

            // Delete the digits. It is possible that everything would
            // be deleted, so we take a min. 
            //
            result.Digits.RemoveRange(0, Math.Min(rootPower / rootDegree, this.Length));

            // It is reasonable to do something only if any digits are left.
            // If not, just add a zero and finish. 
            // -
            if (result.Length > 0)
            {
                // Now we are dealing with the remainder.
                //
                rootPower %= rootDegree;

                // То, на что осталось поделить все цифры числа со сдвигом.
                // Из следующей цифры числа берем остаток от деления на rootValue в остаточной степени.

                int powered = Mathematics<int, CalcInt>.PowerInteger(rootValue, rootPower);
                int backpowered = BASE / powered;

                for (int i = 0; i < result.Digits.Count - 1; i++)
                    result.Digits[i] = result.Digits[i] / powered + result.Digits[i + 1] % powered * backpowered;

                // Divide the last digit. Do not add anything to it.
                // -
                result.Digits[result.Length - 1] /= powered;
                
                // It is possible that leading zeroes will appear.
                // -
                result.DealWithZeroes();
            }
            else result.Digits.Add(0);

            return result;
        }

        /// <summary>
        /// If the digits base for the current number X is BASE,
        /// the method would return return 
        /// X * BASE ^ <paramref name="basePower"/>.
        /// 
        /// Equivalent to adding <paramref name="basePower"/> empty digits to the beginning
        /// of the number, i.e. shifting left by <paramref name="basePower"/> BASE-based digits.
        /// 
        /// Is quick for any digit base.
        /// </summary>
        /// <param name="basePower">The base power to multiply by.</param>
        /// <returns>The current number multiplied by X * BASE ^ <paramref name="basePower"/></returns>
        public LongInt<B> BaseMultiply(int basePower)
        {
			if (basePower < 0)
			{
				return BaseDivide(-basePower);
			}

            LongInt<B> tmp = this.Clone() as LongInt<B>;

            tmp.Digits.InsertRange(0, new int[basePower]);

            return tmp;
        }

        /// <summary>
        /// If the digits base for the current number X is BASE,
        /// the method would return return 
        /// X / BASE ^ <paramref name="basePower"/>.
        /// 
        /// Equivalent to removing <paramref name="basePower"/> from the beginning
        /// of the number, i.e. shifting right by <paramref name="basePower"/> BASE-based digits.
        /// 
        /// Is quick for any digit base.
        /// </summary>
        /// <param name="basePower">The base power to divide by.</param>
        /// <returns>The current number divided by BASE ^ <paramref name="basePower"/></returns>
        public LongInt<B> BaseDivide(int basePower)
        {
            if (basePower < 0)
                return BaseMultiply(-basePower);

            LongInt<B> tmp = this.Clone() as LongInt<B>;

            tmp.Digits.RemoveRange(0, basePower);

            return tmp;
        }

        /// <summary>
        /// Shifts the current number to the left by the specified amount of decimal digits.
        /// Equivalent to multiplication by 10^tenPower.
        /// 
        /// WARNING! IS quick only if the base of this number is a power of ten.
        /// 
        /// Otherwise, a LongInt(B) of current base will be first created as a result
        /// of integer powering 10^tenPower, and further multiplication shall be performed.
        /// 
        /// Implicitly calls the method <see cref="BaseRootMultiply"/> with parameters
        /// rootValue = 10, rootDegree = log10(BASE), multiplyByRootPower = <paramref name="tenPower"/>.
        /// </summary>
        /// <param name="tenPower">The amount of decimal digits to shift by.</param>
        public LongInt<B> DecimalMultiply(int tenPower)
        {
            if (tenPower < 0) 
                return this.DecimalDivide(-tenPower);

            // Если основание - не степень десятки, придется просто умножать.

            else if (!LongInt<B>.IsBasePowerOfTen)
            {
                LongInt<B> poweredInteger = Mathematics<LongInt<B>, CalcLongInt<B>>.PowerInteger(10, tenPower);
                return this * poweredInteger;
            }
            
            else
                return BaseRootMultiply(10, fieldLength, tenPower);
        }

        /// <summary>
        /// Shifts the current number to the right by the specified amount of decimal digits.
        /// Equivalent to division by 10^tenPower.
        /// 
        /// IS quick only if the base of current number's digits is a power of ten.
        /// Otherwise, simple powering and multiplication operations shall be performed.
        /// 
        /// Implicitly calls the method <see cref="BaseRootDivide"/> with parameters
        /// rootValue = 10, rootDegree = log10(BASE), divideByRootPower = <paramref name="tenPower"/>.
        /// </summary>
        /// <param name="tenPower">The amount of decimal digits to shift by.</param>
        public LongInt<B> DecimalDivide(int tenPower)
        {
            if (tenPower < 0)
                return this.DecimalMultiply(-tenPower);

            else if (!LongInt<B>.IsBasePowerOfTen)
            {
                LongInt<B> powered = Mathematics<LongInt<B>, CalcLongInt<B>>.PowerInteger(10, tenPower);
                return this / powered;
            }

            else
                return BaseRootDivide(10, fieldLength, tenPower);
        }

        /// <summary>
        /// Shifts the specified LongInt to the left by <paramref name="howMuch"/> binary digits.
        /// Equivalent to multiplication by 2^<paramref name="howMuch"/>.
        /// 
        /// IS quick only if the digits base for the current number is a power of two, e.g. 256.
        /// 
        /// Otherwise, simple multiplication will be performed.
        /// </summary>
        /// <param name="one">The number to be multiplied.</param>
        /// <param name="howMuch">The amount of binary digits to shift by.</param>
        public static LongInt<B> operator <<(LongInt<B> one, int howMuch)
        {
            if (howMuch < 0) return one >> (-howMuch);

            else if (LongInt<B>.IsBasePowerOfTwo)
                return one.BaseRootMultiply(2, fieldLength, howMuch);

            // Unquick part.

			LongInt<B> twoPower = Mathematics<LongInt<B>, CalcLongInt<B>>.PowerInteger(2, howMuch);
            one *= twoPower;

            return one;
        }

        /// <summary>
        /// Shifts the specified LongInt to the right by <paramref name="howMuch"/> binary digits.
        /// Equivalent to integral division by 2^<paramref name="howMuch"/>.
        /// 
        /// IS quick only if the digits base for the current number is an exact power of two, e.g. 256.
        /// 
        /// Otherwise, simple division will be performed.
        /// </summary>
        /// <param name="one">The number to be divided.</param>
        /// <param name="howMuch">The amount of binary digits to shift by.</param>
        public static LongInt<B> operator >>(LongInt<B> one, int howMuch)
        {
            if (howMuch < 0) return one << (-howMuch);

            else if (LongInt<B>.IsBasePowerOfTwo)
                return one.BaseRootDivide(2, fieldLength, howMuch);

            // Unquick part.

            LongInt<B> twoPower = Mathematics<LongInt<B>, CalcLongInt<B>>.PowerInteger(2, howMuch);
            one /= twoPower;

            return one;
        }

		#endregion

        /// <summary>
        /// Gets the digit formatter for ToString() output.
        /// </summary>
        /// <returns></returns>
		private static string GetDigitFormatter()
        {
            long fieldLength = (long)Math.Log10(BASE);
            return "{0:d" + fieldLength.ToString() + "}";
        }

        /// <summary>
        /// Cuts the non-significant leading zeroes in the number.
        /// Also checks whether the result is zero value and makes it positive.
        /// </summary>
        /// <returns>
        /// True if the number is actually zero.
        /// </returns>
        public bool DealWithZeroes()
        {
            this.Digits.CutInPlace();

            if (this.Length == 1 && this.Digits[0] == 0)
            {
                this.IsNegative = false;
                return true;
            }
            else
			{
                return false;
			}
        }

        /// <summary>
        /// Chooses the bigger and the smaller absolute number of two.
        /// </summary>
        private static bool CompareAbsolute(LongInt<B> one, LongInt<B> two, out LongInt<B> bigger, out LongInt<B> smaller)
        {
            bool neg1 = one.IsNegative;
            bool neg2 = two.IsNegative;

            one.IsNegative = false;
            two.IsNegative = false;

            bool res = one > two;

            one.IsNegative = neg1;
            two.IsNegative = neg2;

            bigger = res ? one : two;
            smaller = res ? two : one;

            return res;
        }

		#region Comparison Operators

        public static bool operator >(LongInt<B> one, LongInt<B> two)
        {
            if (!one.IsNegative && two.IsNegative) return true;
            else if (one.IsNegative && !two.IsNegative) return false;
            else if (one.IsNegative && two.IsNegative)
            {
                one.IsNegative ^= true;
                two.IsNegative ^= true;

                bool temp = two > one;

                one.IsNegative ^= true;
                two.IsNegative ^= true;

                return temp;
            }

			// Now we know that numbers are both positive.
			// -
			if (one.Length > two.Length)
			{
				return true;
			}
			else if (one.Length < two.Length)
			{
				return false;
			}

            bool _;
			return LongIntegerMethods.GreaterThan(one.Digits, two.Digits, out _);
        }

        public static bool operator <(LongInt<B> one, LongInt<B> two)
        {
            return two > one;
        }

        public static bool operator ==(LongInt<B> one, LongInt<B> two)
        {
            if (object.ReferenceEquals(two, null))
                return (object.ReferenceEquals(one, null));

            else if (object.ReferenceEquals(one, null))
                return (object.ReferenceEquals(two, null));

            // if numbers are of different signs - false.
            if (one.IsNegative ^ two.IsNegative == true) return false;

            // if numbers are of different sizes - false.
            if (one.Length != two.Length) return false;

            // check the digits.
            for (int i = 0; i < one.Length; i++)
                if (one.Digits[i] != two.Digits[i]) return false;

            return true;
        }

        public static bool operator !=(LongInt<B> one, LongInt<B> two)
        {
            return !(one == two);
        }

        public static bool operator >=(LongInt<B> one, LongInt<B> two)
        {
            return !(two > one);
        }

        public static bool operator <=(LongInt<B> one, LongInt<B> two)
        {
            return !(one > two);
        }

		#endregion

		#region Object Methods Override

        /// <summary>
        /// Creates a copy of current LongInt number.
        /// </summary>
        /// <returns>A deep copy of current LongInt number.</returns>
        public object Clone()
        {
            LongInt<B> newInt = new LongInt<B>(this.Length);
            newInt.Digits.AddRange(this.Digits.GetRange(0, this.Length));
            newInt.IsNegative = this.IsNegative;

            return newInt;
        }

        /// <summary>
        /// Checks if two LongInt objects store the same numeric value.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if true, false if false...</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is LongInt<B>)) return false;

            LongInt<B> compValue = obj as LongInt<B>;
            return this == compValue;
        }

        /// <summary>
        /// Gets the hashing code for the current number.
        /// Works very stupidly at the moment, wait for better life.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int res = (IsNegative ? 1 : 0);

            for (int i = 0; i < this.Length; i++)
                res += (int)(this.Digits[i] % 10);

            return res;
        }

		#endregion

        //-----------------------------------
        //------CONVERSION ---- METHODS------
        //-----------------------------------

        /// <summary>
        /// Converts the current LongInt number from one base
        /// to another using the IBase interface.
        /// 
        /// The resulting LongInt will be numerically equal to the current, but
        /// have different digits numeric base.
        /// </summary>
        /// <typeparam name="B2">The IBase interface specifying the type of outcoming LongInt(<typeparamref name="B2"/>)</typeparam>
        /// <returns>A LongInt(<typeparamref name="B2"/>) number with digits of base specified by <typeparamref name="B2"/>'s getBase() value.</returns>
        public LongInt<B2> BaseConvert<B2>() where B2: IBase, new()
        {
            int[] convertedArray = BaseConversion.BaseConvert(this.Digits, BASE, LongInt<B2>.BASE);

            LongInt<B2> result = new LongInt<B2>(LongInt<B2>.BASE, convertedArray, this.IsNegative);

            return result;
        }

		#region String Methods

        /// <summary>
        /// Returns the string representation of the current LongInt number.
        /// If the base of this number is not a power of ten, the process may become slow
        /// as the convertation shall be performed.
        /// </summary>
        /// <returns>The string representation of the current LongInt number.</returns>
        public override string ToString()
        {
			if (!LongInt<B>.IsBasePowerOfTen)
			{
				return this.BaseConvert<Bases.B10k>().ToString();
			}

            string result = (IsNegative ? "-" : "");

            // Cut the leading zeroes from the elder digit
            // -
			result += this.Digits[this.Length - 1];

			for (int i = this.Length - 2; i >= 0; i--)
			{
				result += String.Format(digitFormatter, this.Digits[i]);
			}

            return result;
        }

        /// <summary>
        /// Tries to parse the specified string value into a LongInt number.
        /// If successful, returns true.
        /// False otherwise. In the latter case, the result is set to NULL.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string value, out LongInt<B> result)
        {
            try 
			{ 
				result = Parse(value); 
				return true; 
			}
            catch 
			{ 
				result = null; 
				return false; 
			}
        }

        /// <summary>
        /// Parses the specified string value into a LongInt number.
        /// If unsuccessful, a FormatException is thrown.
        /// 
        /// If the numeric base of the IBase class for this long integer
        /// is not a power of ten, the value will be first parsed to
        /// LongInt(Bases.B_10k) and then converted to the current base,
        /// thus the process may become slower.
        /// </summary>
        /// <param name="value">A string containing a number to convert</param>
        /// <returns>The long integer value containing all of the digits specified.</returns>
        public static LongInt<B> Parse(string value)
        {
			Condition.ValidateNotNull(value, nameof(value));

			if (!LongInt<B>.IsBasePowerOfTen)
			{
				return LongInt<Bases.B10k>.Parse(value).BaseConvert<B>();
			}

			LongInt<B> result = new LongInt<B>(value.Length / fieldLength + 1);

            try
            {
                if (value[0] == '-')
                {
                    value = value.Substring(1);
                    result.IsNegative = true;
                }

                // Parse the digits
				// -
				int digitIndex = value.Length - fieldLength;

				for (; digitIndex > 0; digitIndex -= fieldLength)
				{
					result.Digits.Add(int.Parse(value.Substring(digitIndex, fieldLength)));
				}

                // Add the "tail"
				// -
                result.Digits.Add(int.Parse(value.Substring(0, fieldLength + digitIndex)));

				result.DealWithZeroes();

                return result;
            }
			catch (FormatException exception)
            {
                throw new FormatException(
					"Couldn't parse the specified string value into LongInt.", 
					exception);
            }
        }

		#endregion
    }
}
