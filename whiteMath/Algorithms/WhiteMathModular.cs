using System.Collections.Generic;
using System.Linq;

using whiteMath.Calculators;

using whiteStructs.Conditions;

namespace whiteMath.Algorithms
{
    public static partial class WhiteMath<T,C> where C: ICalc<T>, new()
    {
        /// <summary>
        /// For residue class rings modulo <c>N</c> where <c>N >= 1</c>,
        /// this method finds all modular roots of unity in the specified search interval.
        /// </summary>
        /// <param name="searchInterval">
        /// The interval on which the search will be performed.
        /// The inclusive bounds of the interval specified should be both located within the interval <c>[0; N-1]</c>.
        /// The default value of this parameter is <c>null</c>, and in this case, the interval <c>[0; N-1]</c> will be
        /// substituted automatically.
        /// </param>
        /// <param name="modulus">The modulus of the residue class ring, expected to be >= 2.</param>
        /// <param name="rootDegrees">
        /// An enumeration of root degrees for which the primitive roots should be found.
        /// </param>
        /// <returns></returns>
        public static Dictionary<T, List<T>> RootsOfUnity(T modulus, IEnumerable<T> rootDegrees, BoundedInterval<T, C>? searchInterval = null)
        {
			Condition.Validate(calc.IsIntegerCalculator).OrException(new NonIntegerTypeException(typeof(T).Name));
			Condition.ValidateNotNull(modulus);
			Condition.Validate(modulus > Numeric<T, C>._1).OrArgumentOutOfRangeException("The module should be more than 1.");
			Condition
				.Validate(rootDegrees.All(x => x >= Numeric<T, C>._1 && x < (Numeric<T, C>)modulus))
				.OrArgumentOutOfRangeException("All of the root degrees specified should be located inside the [1; N-1] interval.");
//			
            if (!searchInterval.HasValue)
                searchInterval = new BoundedInterval<T, C>(Numeric<T, C>.Zero, modulus - Numeric<T, C>._1, true, true);

            Numeric<T, C> lowerBound = searchInterval.Value.LeftBound;
            Numeric<T, C> upperBound = searchInterval.Value.RightBound;

			if (!searchInterval.Value.IsLeftInclusive)
			{
				lowerBound++;
			}

			if (!searchInterval.Value.IsRightInclusive)
			{
				upperBound--;
			}

            // Contract.Requires<ArgumentOutOfRangeException>(lowerBound > Numeric<T, C>.Zero && upperBound < module - Numeric<T, C>.CONST_1, "The search interval should be located inside the [0; N-1] interval.");
            
			// We need just the unique values!
			// -
            ISet<Numeric<T, C>> rootDegreeSet = new HashSet<Numeric<T, C>>();

			foreach (T degree in rootDegrees)
			{
				rootDegreeSet.Add(degree);
			}

            // -----------------------------

            bool evenModule = calc.IsEven(modulus);

			// If the lower bound is even, and the modulus is even – definitely not coprime.
			// Which means that the number is a zero divisor and cannot be a root of unity.
			// Thus, increment.
			// -
			if (calc.IsEven(lowerBound) && evenModule)
			{
				lowerBound++;
			}

            Dictionary<T, List<T>> result = new Dictionary<T, List<T>>();

            for (Numeric<T, C> current = lowerBound; current <= upperBound; current++)
            {
				// Of not coprime with modulus – cannot be a primitive root.
				// -
                if (WhiteMath<T, C>.GreatestCommonDivisor(current, modulus) != Numeric<T, C>._1)
                    goto ENDING;

                // Now we test.
				// -
                Numeric<T, C> currentPower = Numeric<T,C>._1;

                Numeric<T, C> tmp = current;

                while (true)
                {
                    if (tmp == Numeric<T, C>._1)
                    {
                        // Check the root degree
						// -
                        if (rootDegreeSet.Contains(currentPower))
                        {
                            if (!result.ContainsKey(currentPower))
                                result.Add(currentPower, new List<T>());

                            List<T> currentDegreeRootList = result[currentPower];
                            currentDegreeRootList.Add(current);
                        }

                        goto ENDING;
                    }
                    else if (tmp == Numeric<T, C>.Zero) 
                        goto ENDING;

                    tmp = (tmp * tmp) % modulus;
                    ++currentPower;
                }
                
                ENDING:

                // We need to increment in twos for an even modulus.
				// -
				if (evenModule)
				{
					current++;
				}
            }

            return result;
        }
    }
}
