using System;
using System.Collections.Generic;

using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Numeric;

namespace WhiteMath.ArithmeticLong
{
    public class LongExp<B, P>
		where B: IBase, new()
		where P: IPrecision, new()
    {        
		private static readonly int MAXIMUM_DIGITS = new P().Precision;
		private static readonly int DIGIT_BASE = new B().Base;

        private static readonly int fieldLength = (int)Math.Log10(DIGIT_BASE);
		private static readonly string digitFormatter = $"{{0:d{fieldLength}}}";

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
		public bool IsNegative { get; private set; }

        /// <summary>
        /// Parameterless constructor, sets the zero number value.
        /// </summary>
        public LongExp()
        {
			this.Mantiss = new List<int>();
            this.Exponent = 0;
            this.Mantiss.Add(0);

            this.IsNegative = false;
        }

        /// <summary>
        /// The constructor designed to create a pseudo-random LongExp
        /// number, using an integer amount of decimal digits specified by the user.
        /// </summary>
        /// <param name="powerInterval">An interval in which the exponent of the number can be presented.</param>
        /// <param name="generator">A random generator for the digits.</param>
        /// <param name="digitCount">The digit length of the number. Cannot be more than specified by IPrecision precision class.</param>
        public LongExp(int digitCount, BoundedInterval<int, CalcInt> powerInterval, System.Random generator)
        {
            this.IsNegative = (generator.Next(0, 2) == 0 ? false : true);

            // Randomize the explonent
            // -
            this.Exponent = generator.Next((powerInterval.IsLeftInclusive ? powerInterval.LeftBound : powerInterval.LeftBound + 1), (powerInterval.IsRightInclusive ? powerInterval.RightBound + 1 : powerInterval.RightBound));

            // Now to the mantiss.
            // -
            this.Mantiss = new List<int>(digitCount);
            this.Mantiss.Add(generator.Next(1, DIGIT_BASE));

            int digitIndex = 1;

			for (; digitIndex < digitCount - 1; ++digitIndex)
			{
				this.Mantiss.Add(generator.Next(0, DIGIT_BASE));
			}

            if (digitIndex < digitCount)
            {
                this.Mantiss.Add(generator.Next(1, DIGIT_BASE));
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
		/// <typeparam name="PNew">The precision class.</typeparam>
		public LongExp<B, PNew> PrecisionConvert<PNew>() 
			where PNew : IPrecision, new()
        {
			LongExp<B, PNew> result = new LongExp<B, PNew>();

            result.Mantiss = new List<int>();
            result.IsNegative = this.IsNegative;
            result.Exponent = this.Exponent;

            return result;
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
		/// specified by the used <see cref="IPrecision"/>.
        /// </summary>
        private void Normalize()
        {
            int exponentAdd = this.Mantiss.Count;

            this.Mantiss.CutInPlace();

            exponentAdd -= Mantiss.Count;

			if (exponentAdd > 0)
			{
				this.Exponent += exponentAdd;
			}

			// Cutting the excessive precision
			// -
			if (Mantiss.Count > MAXIMUM_DIGITS)
			{
				Mantiss.RemoveRange(0, Mantiss.Count - MAXIMUM_DIGITS);
			}
        }
    }
}
