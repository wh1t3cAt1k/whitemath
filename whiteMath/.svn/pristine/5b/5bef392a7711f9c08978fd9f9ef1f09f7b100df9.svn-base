using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections;

using whiteMath.General;

namespace whiteMath
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
        /// <param name="module">The module of the residue class ring, expected to be >= 2.</param>
        /// <param name="rootDegrees">
        /// An enumeration of root degrees for which the primitive roots should be found.
        /// </param>
        /// <returns></returns>
        [ContractVerification(true)]
        public static Dictionary<T, List<T>> RootsOfUnity(T module, IEnumerable<T> rootDegrees, BoundedInterval<T, C>? searchInterval = null)
        {
            Contract.Requires<NonIntegerTypeException>(Numeric<T, C>.Calculator.isIntegerCalculator, "This method supports only integral types.");
            Contract.Requires<ArgumentNullException>(module != null, "module");
            Contract.Requires<ArgumentOutOfRangeException>(module > Numeric<T, C>._1, "The module should be more than 1.");
            Contract.Requires<ArgumentOutOfRangeException>(Contract.ForAll<T>(rootDegrees, (x => x >= Numeric<T, C>._1 && x < (Numeric<T, C>)module)), "All of the root degrees specified should be located inside the [1; N-1] interval.");

            if (!searchInterval.HasValue)
                searchInterval = new BoundedInterval<T, C>(Numeric<T, C>.Zero, module - Numeric<T, C>._1, true, true);

            Numeric<T, C> lowerBound = searchInterval.Value.LeftBound;
            Numeric<T, C> upperBound = searchInterval.Value.RightBound;

            if (!searchInterval.Value.IsLeftInclusive)
                lowerBound++;

            if (!searchInterval.Value.IsRightInclusive)
                upperBound--;

            // Contract.Requires<ArgumentOutOfRangeException>(lowerBound > Numeric<T, C>.Zero && upperBound < module - Numeric<T, C>.CONST_1, "The search interval should be located inside the [0; N-1] interval.");
            
            // Нам нужны только уникальные значения
            // поэтому сгенерируем множество

            ISet<Numeric<T, C>> rootDegreeSet = new HashSet<Numeric<T, C>>();

            foreach (T degree in rootDegrees)
                rootDegreeSet.Add(degree);

            // -----------------------------

            bool evenModule = calc.isEven(module);

            // Если нижняя граница четная, и модуль четный - по любому взаимно не простые.
            // Значит число - делитель нуля, и не может быть корнем из единицы ни при каких условиях.
            // Прибавляем единицу.

            if (calc.isEven(lowerBound) && evenModule)
                lowerBound++;

            Dictionary<T, List<T>> result = new Dictionary<T, List<T>>();

            for (Numeric<T, C> current = lowerBound; current <= upperBound; current++)
            {
                // Если не взаимно просты с модулем - по-любому не может быть
                // примитивным корнем.

                if (WhiteMath<T, C>.GreatestCommonDivisor(current, module) != Numeric<T, C>._1)
                    goto ENDING;

                // Теперь занимаемся тестированием.

                Numeric<T, C> currentPower = Numeric<T,C>._1;

                Numeric<T, C> tmp = current;

                while (true)
                {
                    if (tmp == Numeric<T, C>._1)
                    {
                        // Проверить степень корня
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

                    tmp = (tmp * tmp) % module;
                    ++currentPower;
                }
                
                ENDING:

                // Если четный модуль - надо перескочить через два.

                if (evenModule)
                    current++;
            }

            return result;
        }
    }
}
