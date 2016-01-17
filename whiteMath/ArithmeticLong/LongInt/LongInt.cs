﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.General;
using whiteMath.Randoms;
using System.Diagnostics.Contracts;

namespace whiteMath.ArithmeticLong
{
    /// <summary>
    /// Represents a long integer number with theoretically unlimited precision.
    /// </summary>
    [ContractVerification(true)]
    [Serializable]
    public partial class LongInt<B>: ICloneable where B: IBase, new()
    {
        public static readonly int BASE = new B().getBase();

        public static bool BASE_is_power_of_ten { get; private set; }
        public static bool BASE_is_power_of_two { get; private set; }

        private static readonly string digitFormatter = getDigitFormatter();

        // значение степени десятки или двойки для получения основания. -1 значит, что основание - ни десятка, ни двойка. 
        // также отражает длину одной цифры в десятичных или бинарных знаках.
        private static readonly int fieldLength = -1;

        // --------------------------------------------------------
        #region Стандартные исключения
        // --------------------------------------------------------

        private static NotSupportedException NOT_SUPPORTED_DECIMAL_EXCEPTION =
            new NotSupportedException("The digits base for this number is not an integer power of ten. The operation is not supported.");

        private static NotSupportedException NOT_SUPPORTED_BINARY_EXCEPTION =
            new NotSupportedException("The digits base for this number is not an integer power of two. The operation is not supported.");

        // --------------------------------------------------------
        #endregion
        // --------------------------------------------------------

        // --------------------------------------------------------
        #region Узнать длину цифры в десятичных или двоичных знаках
        // --------------------------------------------------------

        /// <summary>
        /// Returns the length of the current number in decimal digits.
        /// Works ONLY for numbers whose digits base is an integer power of ten.
        /// </summary>
        public long LengthInDecimalPlaces
        {
            get
            {
                if (!BASE_is_power_of_ten)
                    throw NOT_SUPPORTED_DECIMAL_EXCEPTION;

                return LongInt<B>.fieldLength * (this.Length - 1) + (int)Math.Ceiling(Math.Log10(this[this.Length - 1]));
            }
        }

        /// <summary>
        /// Returns the length of the current number in binary digits.
        /// Works ONLY for numbers whose digits base is an integer power of two.
        /// </summary>
        public long LengthInBinaryPlaces
        {
            get
            {
                if (!BASE_is_power_of_two)
                    throw NOT_SUPPORTED_BINARY_EXCEPTION;

                return LongInt<B>.fieldLength * (this.Length - 1) + (int)Math.Ceiling(Math.Log(this[this.Length - 1], 2));
            }
        }

        /// <summary>
        /// Returns the amount of decimal places contained in a single numeric digit of this number.
        /// Works only if the base of the number is an integer power of ten, otherwise,
        /// a NotSupportedException shall be thrown.
        /// </summary>
        public static int PlacesInADigit_Decimal
        {
            get
            {
                if (!BASE_is_power_of_ten)
                    throw NOT_SUPPORTED_DECIMAL_EXCEPTION;

                return fieldLength;
            }
        }

        /// <summary>
        /// Returns the amount of binary places contained in a single numeric digit of this number.
        /// Works only if the base of the number is an integer power of two, otherwise,
        /// a NotSupportedException shall be thrown.
        /// </summary>
        public static int PlacesInADigit_Binary
        {
            get
            {
                if (!BASE_is_power_of_two)
                    throw NOT_SUPPORTED_BINARY_EXCEPTION;

                return fieldLength;
            }
        }

        // --------------------------------------------------------
        #endregion
        // --------------------------------------------------------

        //-----------------------------------
        //---------INVARIANTS ---------------
        //-----------------------------------

        [ContractInvariantMethod]
        private void ___invariant()
        {
            Contract.Invariant(this.Digits != null);
            Contract.Invariant(Contract.ForAll(this.Digits, x => (x >= 0)));    // never should a digit be negative
        }

        
        //-----------------------------------
        //---------PUBLIC PROPERTIES---------
        //-----------------------------------

        /// <summary>
        /// Returns the length of the current number in BASE-based digits.
        /// <see cref="BASE"/>
        /// </summary>
        public int Length { get { return Digits.Count; } }

        /// <summary>
        /// Returns the digits list for the current number.
        /// </summary>
        public List<int> Digits { get; private set; }

        /// <summary>
        /// Returns true if the current number is negative
        /// </summary>
        public bool Negative { get; private set; }

        /// <summary>
        /// Gets a flag signalizing whether the current number
        /// is even, i.e. has a zero remainder after division by 2.
        /// </summary>
        public bool IsEven { get { return LongIntegerMethods.IsEven(BASE, this.Digits); } }

        /// <summary>
        /// Returns the LongInt digit at specified position.
        /// Lower indexes correspond to less significant digits (i.e. the order is big endian), so
        /// with <c>i</c>'th digit corresponding to <c>BASE^i</c>.
        /// </summary>
        /// <param name="digitIndex">The index of the desired number.</param>
        /// <returns>The digit on the zero-based position <paramref name="digitIndex"/>.</returns>
        public int this[int digitIndex]
        {
            get { return Digits[digitIndex]; }
            set { Digits[digitIndex] = value; }
        }

        //-----------------------------------
        //-----------CONSTRUCTORS------------
        //-----------------------------------

        // static initialization
        
        /// <summary>
        /// Статическая инициализация. Проверяет, не является ли основание числа целой степенью
        /// десятки или двойки.
        /// </summary>
        static LongInt()
        {
            int? isPower;

            if (WhiteMath<int, CalcInt>.IsNaturalIntegerPowerOf(BASE, 10, out isPower))
            {
                LongInt<B>.BASE_is_power_of_ten = true;
                LongInt<B>.fieldLength = isPower.Value;
            }
            else if (WhiteMath<int, CalcInt>.IsNaturalIntegerPowerOf(BASE, 2, out isPower))
            {
                LongInt<B>.BASE_is_power_of_two = true;
                LongInt<B>.fieldLength = isPower.Value;
            }
        }

        /// <summary>
        /// Parameterless constructor, sets the zero value.
        /// </summary>
        public LongInt()
        {
            this.Negative = false;                  // is positive.
            this.Digits = new List<int>() { 0 };    // is zero.
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
            Contract.Requires<ArgumentOutOfRangeException>(digitCount > 0, "The number of digits should be positive");

            if(allowNegative)
                this.Negative = (generator.Next(0, 2) == 0 ? false : true);

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
            Contract.Requires<ArgumentOutOfRangeException>(digitCount > 0, "The number of digits should be positive");
            Contract.Requires<ArgumentOutOfRangeException>(digit < BASE, "The digit to fill the number with should be less than BASE.");

            this.Digits = new List<int>(digitCount);
            this.Negative = negative;

            this.Digits.FillByAppending(digitCount, digit);
        }

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
        /// <param name="generator">The IRandom implementation to generate uniformly distributed integer values.</param>
        /// <param name="decimalDigitCount">The amount of decimal digits for the number to be generated.</param>
        /// <param name="allowNegative">The flag which signalizes if negative values are allowed.</param>
        /// <returns>A pseudo-random LongInt number with the desired amount of decimal digits.</returns>
        public static LongInt<B> CreateRandom_Decimal(int decimalDigitCount, IRandomBounded<int> generator, bool allowNegative)
        {
            if (!BASE_is_power_of_ten)
                throw NOT_SUPPORTED_DECIMAL_EXCEPTION;

            int best = decimalDigitCount / fieldLength;         // how many BASE-based digits are there
            int rest = decimalDigitCount % fieldLength;         // additional digit may be presented

            LongInt<B> tmp = new LongInt<B>();

            if (allowNegative)
                tmp.Negative = (generator.Next(0, 2) == 0 ? false : true);
            else
                tmp.Negative = false;

            tmp.Digits = new List<int>(best + (rest > 0 ? 1 : 0));

            for (int i = 0; i < best; i++)
                tmp.Digits.Add(generator.Next(0, BASE));

            if (rest > 0)
                tmp.Digits.Add(generator.Next(1, WhiteMath<int, CalcInt>.PowerInteger(10, rest)));
            else if (tmp.Digits[tmp.Digits.Count - 1] == 0)
                tmp.Digits[tmp.Digits.Count - 1] = generator.Next(1, BASE);

            return tmp;
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
        public static LongInt<B> CreateRandom_Binary(int binaryDigitCount, IRandomBounded<int> generator, bool allowNegative)
        {
            if (!BASE_is_power_of_two)
                throw NOT_SUPPORTED_BINARY_EXCEPTION;

            int best = binaryDigitCount / fieldLength;         // how many BASE-based digits are there
            int rest = binaryDigitCount % fieldLength;         // additional digit may be presented

            LongInt<B> tmp = new LongInt<B>();

            if (allowNegative)
                tmp.Negative = (generator.Next(0, 2) == 0 ? false : true);
            else
                tmp.Negative = false;

            tmp.Digits = new List<int>(best + (rest > 0 ? 1 : 0));

            for (int i = 0; i < best; i++)
                tmp.Digits.Add(generator.Next(0, BASE));

            if (rest > 0)
                tmp.Digits.Add(generator.Next(1, WhiteMath<int, CalcInt>.PowerInteger(2, rest)));
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
            Contract.Requires<ArgumentOutOfRangeException>(basePower >= 0, "The power of BASE should not be negative.");

            LongInt<B> result = new LongInt<B>(basePower + 1);
            result.Digits.AddRange(new int[basePower + 1]);

            result.Digits[basePower] = 1;

            return result;
        }

        /// <summary>
        /// Initializes the LongInt number with a user-specified digit list.
        /// If the BASE of digits in <paramref name="digitsArray"/> is other than LongInt.BASE,
        /// the conversion may take longer time.
        /// </summary>
        /// <param name="BASE">The base of digits in the digit list.</param>
        /// <param name="digitsArray">A list of digits with increasing significance (the leftmost digit corresponds to BASE^0).</param>
        /// <param name="negative">A parameter which signalizes if the number should be treated as negative.</param>
        public LongInt(int BASE, IList<int> digitsArray, bool negative = false)
        {
            if (BASE == LongInt<B>.BASE)
                this.Digits = new List<int>(digitsArray);
            else
                this.Digits = new List<int>(BaseConversion.baseConvert(digitsArray, BASE, LongInt<B>.BASE));

            this.Negative = negative;   // устанавливаем отрицательность
            this.DealWithZeroes();      // разбираемся с нулями
        }

        /// <summary>
        /// Private constructor made for quick-trick.
        /// </summary>
        /// <param name="initialSize"></param>
        private LongInt(int initialSize)
        {
            Negative = false;                   // is positive
            Digits = new List<int>();           // no digits yet
            Digits.Capacity = initialSize;      // sets the initial size.
        }

        /// <summary>
        /// Makes a LongInt from the standard long.
        /// </summary>
        /// <param name="num">The long number to convert into LongInt.</param>
        public LongInt(long num)
        {
            Negative = (num < 0);               // set the negative flag
            num = (num > 0 ? num : -num);       // make num positive

            Digits = new List<int>();

            do                                  // fill in the digits list
            {
                Digits.Add((int)(num % BASE));
                num /= BASE;
            } 
            while (num > 0);
        }

        //-----------------------------------
        //------CONVERSION OPERATORS---------
        //-----------------------------------

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

            return (num.Negative ? -res : res);
        }

        /// <summary>
        /// Performs an implicit conversion of a long number into LongInt.
        /// </summary>
        /// <param name="num">The long number to convert.</param>
        /// <returns>The LongInt number.</returns>
        public static implicit operator LongInt<B>(long num)
        {
            return new LongInt<B>(num);
        }

        //-----------------------------------
        //------ARITHMETIC OPERATORS---------
        //-----------------------------------

        public static LongInt<B> operator +(LongInt<B> one, LongInt<B> two)
        {
            // Check for negativeness.

            if (!one.Negative && two.Negative)
            {
                two.Negative ^= true;
                LongInt<B> tmp = one - two;
                two.Negative ^= true;

                return tmp;
            }
            else if (one.Negative && !two.Negative)
            {
                one.Negative ^= true;
                LongInt<B> tmp = (two - one);
                one.Negative ^= true;

                return tmp;
            }

            int maxLength = Math.Max(one.Length, two.Length);
            int minLength = Math.Min(one.Length, two.Length);

            LongInt<B> res = new LongInt<B>(maxLength + 1);
            res.Digits.AddRange(new int[maxLength + 1]);
            res.Negative = one.Negative && two.Negative;        // negative only if both are negative,
                                                                // other cases at (*)

            LongIntegerMethods.Sum(LongInt<B>.BASE, res.Digits, one.Digits, two.Digits);
            res.DealWithZeroes();

            return res;
        }

        public static LongInt<B> operator -(LongInt<B> one, LongInt<B> two)
        {
            if (!one.Negative && two.Negative)
            {
                two.Negative ^= true;
                LongInt<B> tmp = one + two;
                two.Negative ^= true;

                return tmp;
            }
            else if (one.Negative && !two.Negative)
            {
                one.Negative ^= true;
                LongInt<B> tmp = one + two;
                one.Negative ^= true;

                tmp.Negative = true;
                return tmp;
            }

            LongInt<B> bigger, smaller;
            bool aMoreB = AbsMore(one, two, out bigger, out smaller);

            // -------- here comes the substracting

            LongInt<B> res = new LongInt<B>(bigger.Length);
            res.Digits.AddRange(new int[bigger.Length]);

            if (LongIntegerMethods.Dif(LongInt<B>.BASE, res.Digits, bigger.Digits, smaller.Digits))
                throw new Exception("Something really terrible happened. Consider it Apocalypse.");

            // ---- deal with negative flag

            if (aMoreB && !one.Negative && !two.Negative || !aMoreB && one.Negative && two.Negative)
                res.Negative = false;
            else
                res.Negative = true;

            res.DealWithZeroes();
            return res;
        }

        /// <summary>
        /// Multiplies one LongInt number by another.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static LongInt<B> operator *(LongInt<B> one, LongInt<B> two)
        {
            return Helper.MultiplySimple(one, two);
        }

        public static LongInt<B> operator /(LongInt<B> one, int two)
        {
            LongInt<B> res = new LongInt<B>(one.Length);
            res.Digits.AddRange(new int[one.Length]);
            res.Negative = one.Negative ^ (two < 0);

            if (two < 0) two = -two;

            LongIntegerMethods.DivideByInteger(LongInt<B>.BASE, res.Digits, one.Digits, two);

            // the remainder equals the overall remainder
            // cut the leading zeroes

            res.DealWithZeroes();
            return res;
        }

        public static LongInt<B> operator /(LongInt<B> one, LongInt<B> two)
        {
            return Helper.Div(one, two);
        }

        public static int operator %(LongInt<B> one, int two)
        {
            int remainder = (one.Negative ? -1 : 1);
            if (two < 0) two = -two;

            remainder *= LongIntegerMethods.DivideByInteger(LongInt<B>.BASE, new int[one.Length], one.Digits, two);

            return remainder;
        }

        public static LongInt<B> operator % (LongInt<B> one, LongInt<B> two)
        {
            LongInt<B> remainder;
            LongInt<B>.Helper.Div(one, two, out remainder);

            return remainder;
        }

        /// <summary>
        /// Unary minus operator. Negates the number.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static LongInt<B> operator -(LongInt<B> num)
        {
            LongInt<B> tmp = num.Clone() as LongInt<B>;
            tmp.Negative ^= true;

            return tmp;
        }

        /// <summary>
        /// Increments the current number by one.
        /// </summary>
        /// <param name="num"></param>
        public static LongInt<B> operator ++(LongInt<B> num)
        {
            int currentDigitIndex = 0;

            // Если отрицательное, на самом деле надо уменьшать.
            if (num.Negative)
            {
                num.Negative = false;
                num--;
                if(num<0) num.Negative = true;

                return num;
            }

            // Увеличиваем цифру числа на единицу.
            // Если происходит переполнение цифры, переходим к следующей и увеличиваем ее на единицу...
            // Так до тех пор, пока все-таки не прибавим - или у нас не кончатся цифры
            // -
            while (currentDigitIndex < num.Length)
            {
                num[currentDigitIndex]++;

                if (num[currentDigitIndex] == BASE)
                {
                    num[currentDigitIndex] = 0;
                    currentDigitIndex++;
                }
                else return num;
            }

            // Крайний случай - кончились цифры.
            // Заводим новую, остальные обнуляем
            // -
            num.Digits.Add(1);

            for (int i = 0; i < num.Length - 1; i++)
                num[i] = 0;

            return num;
        }

        /// <summary>
        /// Decrements the current number by one.
        /// </summary>
        /// <param name="num">The number to be decremented.</param>
        /// <returns>The original number in the decremented state.</returns>
        public static LongInt<B> operator --(LongInt<B> num)
        {
            int currentDigitIndex = 0;

            // Если число отрицательное, по модулю его на самом деле надо увеличивать
            //
            if(num.Negative)
            {
                num.Negative = false;
                ++num;
                num.Negative = true;
                
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

                    // Старшие цифры могли стать нулями в результате заимствований.
                    // Надо проверить.
                    // -
                    if (num[num.Length - 1] == 0)
                    {
                        num.DealWithZeroes();
                    }

                    return num;
                }
            }

            // Крайний случай - кончились цифры.
            // Значит теперь это минус единица!
            // -
            num.Digits.Clear();
            num.Digits.Add(1);
            num.Negative = true;

            return num;
        }

        //---------------------------------------
        //------FAST--BASE ROOT -MULTIPLICATION--
        //---------------------------------------

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
            // TODO: if power < 0.
            // -
            LongInt<B> tmp = this.Clone() as LongInt<B>;

            // Если степень числа не кратна длине цифры, которая и равна logValue, придется
            // работать дополнительно.
            // -
            if (rootPower % rootDegree > 0)
            {
                int remPower = WhiteMath<int, CalcInt>.PowerInteger(rootValue, rootPower % rootDegree);

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

                int powered = WhiteMath<int, CalcInt>.PowerInteger(rootValue, rootPower);
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

        // --------- BASE MULTIPLY -------------------

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
                return BaseDivide(-basePower);

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

        // -------------------------------------------

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

            else if (!LongInt<B>.BASE_is_power_of_ten)
            {
                LongInt<B> poweredInteger = WhiteMath<LongInt<B>, CalcLongInt<B>>.PowerInteger(10, tenPower);
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

            else if (!LongInt<B>.BASE_is_power_of_ten)
            {
                LongInt<B> powered = WhiteMath<LongInt<B>, CalcLongInt<B>>.PowerInteger(10, tenPower);
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

            else if (LongInt<B>.BASE_is_power_of_two)
                return one.BaseRootMultiply(2, fieldLength, howMuch);

            // Unquick part.

            LongInt<B> twoPower = WhiteMath<LongInt<B>, whiteMath.ArithmeticLong.CalcLongInt<B>>.PowerInteger(2, howMuch);
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

            else if (LongInt<B>.BASE_is_power_of_two)
                return one.BaseRootDivide(2, fieldLength, howMuch);

            // Unquick part.

            LongInt<B> twoPower = WhiteMath<LongInt<B>, whiteMath.ArithmeticLong.CalcLongInt<B>>.PowerInteger(2, howMuch);
            one /= twoPower;

            return one;
        }


        //-----------------------------------
        //------SERVICE METHODS--------------
        //-----------------------------------

        /// <summary>
        /// Gets the digit formatter for ToString() output.
        /// </summary>
        /// <returns></returns>
        private static string getDigitFormatter()
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
                this.Negative = false;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Chooses the bigger and the smaller absolute number of two.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="bigger"></param>
        /// <param name="smaller"></param>
        private static bool AbsMore(LongInt<B> one, LongInt<B> two, out LongInt<B> bigger, out LongInt<B> smaller)
        {
            bool neg1 = one.Negative;
            bool neg2 = two.Negative;

            one.Negative = false;
            two.Negative = false;

            bool res = one > two;

            one.Negative = neg1;
            two.Negative = neg2;

            bigger = res ? one : two;
            smaller = res ? two : one;

            return res;
        }

        //-----------------------------------
        //------COMPARISON OPERATORS---------
        //-----------------------------------

        public static bool operator >(LongInt<B> one, LongInt<B> two)
        {
            //-----------------------------------------------------
            //---If one number is negative and another is positive.
            //------------------------------------------------------

            if (!one.Negative && two.Negative) return true;
            else if (one.Negative && !two.Negative) return false;
            else if (one.Negative && two.Negative)
            {
                one.Negative ^= true;
                two.Negative ^= true;

                bool temp = two > one;

                one.Negative ^= true;
                two.Negative ^= true;

                return temp;
            }

            // ---------------------------
            // --Numbers are both positive
            // ---------------------------

            if (one.Length > two.Length) 
                return true;

            else if (one.Length < two.Length) 
                return false;

            // ------------------
            // --Lengths are same
            // ------------------

            bool junk;

            return LongIntegerMethods.More(one.Digits, two.Digits, out junk);
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
            if (one.Negative ^ two.Negative == true) return false;

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

        //-----------------------------------
        //------OBJECT METHODS OVERRIDE------
        //-----------------------------------

        /// <summary>
        /// Creates a copy of current LongInt number.
        /// </summary>
        /// <returns>A deep copy of current LongInt number.</returns>
        public object Clone()
        {
            LongInt<B> newInt = new LongInt<B>(this.Length);
            newInt.Digits.AddRange(this.Digits.GetRange(0, this.Length));
            newInt.Negative = this.Negative;

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
            int res = (Negative ? 1 : 0);

            for (int i = 0; i < this.Length; i++)
                res += (int)(this.Digits[i] % 10);

            return res;
        }

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
        public LongInt<B2> baseConvert<B2>() where B2: IBase, new()
        {
            int[] convertedArray = BaseConversion.baseConvert(this.Digits, BASE, LongInt<B2>.BASE);

            LongInt<B2> result = new LongInt<B2>(LongInt<B2>.BASE, convertedArray, this.Negative);

            return result;
        }

        //-----------------------------------
        //------STRING -------- METHODS------
        //-----------------------------------

        /// <summary>
        /// Returns the string representation of the current LongInt number.
        /// If the base of this number is not a power of ten, the process may become slow
        /// as the convertation shall be performed.
        /// </summary>
        /// <returns>The string representation of the current LongInt number.</returns>
        public override string ToString()
        {
            // Если основание не десятичное, то конвертанем.

            if (!LongInt<B>.BASE_is_power_of_ten)
                return this.baseConvert<Bases.B_10k>().ToString();

            // Если десятичное - радуемся жизни и выводим на экран.

            string result = (Negative ? "-" : "");

            // cut the leading zeroes from the elder digit
            result += this.Digits[this.Length - 1];

            for (int i = this.Length - 2; i >= 0; i--)
                result += String.Format(digitFormatter, this.Digits[i]);

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
            try { result = Parse(value); return true; }
            catch { result = null; return false; }
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
            Contract.Requires<ArgumentNullException>(value != null, "value");

            // Работает только для десятичных чисел.
            // Если нет, то придется сначала преобразовывать в десятичное, а потом
            // из десятичного в число по требуемому основанию.

            if (!LongInt<B>.BASE_is_power_of_ten)
                return LongInt<Bases.B_10k>.Parse(value).baseConvert<B>();

            // Declare a resulting number

            LongInt<B> res = new LongInt<B>(value.Length / fieldLength + 1);

            // Set the negative flag

            try
            {
                if (value[0] == '-')
                {
                    value = value.Substring(1);
                    res.Negative = true;
                }

                int digCount = (int)Math.Ceiling((double)value.Length / fieldLength);

                // Parse the digits

                int i = value.Length - fieldLength;

                for (; i > 0; i -= fieldLength)
                    res.Digits.Add(int.Parse(value.Substring(i, fieldLength)));

                // Add the "tail"

                res.Digits.Add(int.Parse(value.Substring(0, fieldLength + i)));

                // Cut the leading zeroes

                i = res.Digits.Count - 1;

                while (res.Digits[i] == 0 && i > 0)
                    res.Digits.RemoveAt(i--);

                return res;
            }
            catch(FormatException ex)
            {
                throw new FormatException("Couldn't parse the specified string value into LongInt.", ex);
            }
        }
    }
}
