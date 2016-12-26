using System;
using System.Collections.Generic;

using WhiteMath.Calculators;
using WhiteMath.General;

namespace WhiteMath.ArithmeticLong
{
    public class LongExp<P> where P: IPrecision, new()
    {
        private static P precision = new P();
        
		private static readonly int MAXDIGITS = precision.Precision;
		private static readonly int BASE = precision.Base;

		private static readonly string digitFormatter = getDigitFormatter();
        private static readonly int fieldLength = (int)Math.Log10(BASE);

        /// <summary>
        /// Gets the exponent of the number.
        /// </summary>
        public long Exponent { get; private set; }

        /// <summary>
        /// Gets the array of mantiss digits for the number.
        /// </summary>
        public List<int> Mantiss { get; private set; }

        /// <summary>
        /// Gets the flag determining whether the number is negative.
        /// </summary>
        public bool Negative { get; private set; }

        // -------------------------
        // ----- CTORS -------------
        // -------------------------

        private void init()
        {
            this.Mantiss = new List<int>();
        }

        /// <summary>
        /// Parameterless constructor, sets the zero number value.
        /// </summary>
        public LongExp()
        {
            init();

            this.Exponent = 0;
            this.Mantiss.Add(0);

            this.Negative = false;
        }

        /// <summary>
        /// The constructor designed to create a pseudo-random LongExp
        /// number, using an integer amount of decimal digits specified by the user.
        /// </summary>
        /// <param name="powerInterval">An interval in which the exponent of the number can be presented.</param>
        /// <param name="generator">A random generator for the digits.</param>
        /// <param name="digitCount">The digit length of the number. Cannot be more than specified by IPrecision precision class.</param>
        public LongExp(int digitCount, BoundedInterval<int, CalcInt> powerInterval, Random generator)
        {
            this.Negative = (generator.Next(0, 2) == 0 ? false : true);

            // Randomize the explonent
            // -
            this.Exponent = generator.Next((powerInterval.IsLeftInclusive ? powerInterval.LeftBound : powerInterval.LeftBound + 1), (powerInterval.IsRightInclusive ? powerInterval.RightBound + 1 : powerInterval.RightBound));

            // Now to the mantiss.
            // -
            this.Mantiss = new List<int>(digitCount);
            this.Mantiss.Add(generator.Next(1, BASE));

            // ...Because the first digit should be significant.

            int i = 1;

            for ( ; i < digitCount - 1; i++)
                this.Mantiss.Add(generator.Next(0, BASE));

            if (i < digitCount)
            {
                this.Mantiss.Add(generator.Next(1, BASE));
            }
        }

        // -------------------------
        // ----- CONVERSIONS -------
        // -------------------------

        /// <summary>
        /// Creates an independent dependent copy of the number to provide another precision.
        /// The precision class should be specified as the type argument for the method.
        /// REQUIREMENTS: will NOT convert to another numeric base. Only to another precision.
        /// </summary>
		/// <typeparam name="TPrecision">The precision class.</typeparam>
		public LongExp<TPrecision> PrecisionConvert<TPrecision>() 
			where TPrecision : IPrecision, new()
        {
            LongExp<TPrecision> obj = new LongExp<TPrecision>();

            obj.Mantiss = new List<int>();

			int newBase = new TPrecision().Base;

			if (newBase == LongExp<TPrecision>.BASE)
			{
				obj.Mantiss.AddRange(this.Mantiss.ToArray());
			}
			else
			{
				throw new ArgumentException("Cannot convert a long exponential number from one digits base to another. Precisions may vary, but the base should stay the same.");
			}

            obj.Negative = this.Negative;
            obj.Exponent = this.Exponent;

            return obj;
        }

        // -----------------------------
        // ------- MULTIPLICATION ------
        // -----------------------------


        // --------------------------
        // ------- NORMALIZE --------
        // --------------------------

        /// <summary>
        /// Normalizes the number so that it does not contain any leading zeroes
        /// (exponent additions may occur) and its precision is in the range
        /// specified by used IPrecision class.
        /// </summary>
        private void Normalize()
        {
            int exponentAdd = Mantiss.Count;

            Mantiss.CutInPlace();

            exponentAdd -= Mantiss.Count;   // теперь здесь хранится значение сдвига.

            if (exponentAdd > 0)
                this.Exponent += exponentAdd;

            // Обрубаем лишнюю точность.

            if (Mantiss.Count > MAXDIGITS)
                Mantiss.RemoveRange(0, Mantiss.Count - MAXDIGITS);
        }

        // --------------------------
        // ------- SERVICE ----------
        // --------------------------

        /// <summary>
        /// Gets the digit formatter for ToString() output.
        /// </summary>
        /// <returns></returns>
        private static string getDigitFormatter()
        {
            return "{0:d" + fieldLength.ToString() + "}";
        }
    }
}
