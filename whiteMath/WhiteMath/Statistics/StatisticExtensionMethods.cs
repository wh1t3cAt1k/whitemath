using System;
using System.Collections.Generic;
using System.Linq;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;
using WhiteMath.General;

using whiteStructs.Conditions;
using whiteStructs.Collections;

namespace WhiteMath.Statistics
{
    public static class StatisticExtensionMethods
    {
        /// <summary>
        /// Calculates the sample average for a sequence of observations.
        /// </summary>
        /// <typeparam name="T">The type of observations' values.</typeparam>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="values">The sequence of observations.</param>
        /// <returns>The sample average for the sequence of observations passed.</returns>
        public static T SampleAverage<T, C>(this IEnumerable<T> values) where C : ICalc<T>, new()
        {
			Condition.ValidateNotEmpty(values, "Cannot calculate sample average for an empty sequence.");

			Numeric<T, C> sum = Numeric<T, C>.Zero;
			Numeric<T, C> elementCount = Numeric<T, C>.Zero;

			foreach (T value in values)
			{
				sum += value;
				++elementCount;
			}

            return sum / elementCount;
        }

        /// <summary>
        /// Calculates the sample median value for a sequence of observations, that is,
        /// such a value that (approximately) 50% of all observations in the sequence are less than
        /// (or equal to) this value, and other 50% are bigger than (or equal to) it.
        /// 
        /// Please note that if the number of observations is even (2k), the median
        /// returned may not be an element of the source sequence at all, 
        /// being calculated as an arithmetic average of the middle two adjacent
        /// observations in the sorted sequence (see the definition of sample median).
        /// </summary>
        /// <typeparam name="T">The type of observations' values.</typeparam>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="values">The sequence of observations.</param>
        /// <param name="comparer">A comparer object used to sort the incoming sequence ascending.</param>
        /// <returns>The sample median for the sequence of observations passed.</returns>
        public static T SampleMedian<T, C>(this IEnumerable<T> values, IComparer<T> comparer = null) where C : ICalc<T>, new()
        {
			Condition.ValidateNotNull(values, nameof(values));
			Condition.ValidateNotEmpty(values, "Cannot calculate the sample median for an empty sequence.");

            T[] sortedSequence = values.ToArray();

            sortedSequence.SortShell(comparer);

            if (sortedSequence.Length % 2 == 0)
            {
                // By definition of the sample median for a sequence with an even number of elements,
                // we calculate it as an arithmetic average of two adjacent middle elements of the
                // sequence sorted ascending. 
                // -
                return 
					((Numeric<T, C>)sortedSequence[(sortedSequence.Length - 1) / 2] 
					+ sortedSequence[sortedSequence.Length / 2]) / (Numeric<T, C>._2);
            }
            else
            {
                return (sortedSequence[sortedSequence.Length / 2]);
            }
        }

        /// <summary>
        /// Returns the runs test series count for a sequence of observations.
        /// The sample median for that sequence must first be calculated.
        /// </summary>
        /// <typeparam name="T">The type of observations' values.</typeparam>
        /// <typeparam name="C">The calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="values">The list of observations.</param>
        /// <param name="sampleMedian">The sample median of the observations' sequence.</param>
        /// <returns>The amount of series for the runs test.</returns>
        public static int RunsTest_SeriesCount<T, C>(this IEnumerable<T> values, T sampleMedian) where C : ICalc<T>, new()
        {
            int count = values.Count();

			if (values.IsEmpty())
			{
				throw new ArgumentException("Cannot return the series count for an empty sequence.");
			}

			if (values.IsSingleElement())
			{
				return 0;
			}

            int signChanges = 0;

            IEnumerator<T> enumerator = values.GetEnumerator();
            ICalc<T> calculator = Numeric<T,C>.Calculator;

            enumerator.MoveNext();

			// Find the first value not equal to the median.
			// -
			while (calculator.Equal(enumerator.Current, sampleMedian))
			{
				if (enumerator.MoveNext() == false)
				{
					return 0;
				}
			}

            bool isPlus = calculator.GreaterThan(enumerator.Current, sampleMedian);

            while (enumerator.MoveNext())
            {
				if (!isPlus && calculator.GreaterThan(enumerator.Current, sampleMedian))
                {
					++signChanges;
                    isPlus = true;
                }
                else if (isPlus && calculator.GreaterThan(sampleMedian, enumerator.Current))
                {
                    ++signChanges;
                    isPlus = false;
                }
            }

            return signChanges + 1;
        }

        /// <summary>
        /// Calculates the sample unbiased variation for a sequence of observations. 
        /// </summary>
        /// <typeparam name="T">The type of observations' values.</typeparam>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="values">The sequence of observations.</param>
        /// <param name="sampleAverage">The sample average for the observations sequence.</param>
        /// <returns>The sample unbiased variation for the sequence of observations.</returns>
        public static T SampleUnbiasedVariance<T, C>(this IEnumerable<T> values, T sampleAverage) where C : ICalc<T>, new()
        {
			Condition.ValidateNotNull(values, nameof(values));

			if (values.IsEmpty())
			{
				throw new ArgumentException("Cannot calculate the sample variance for an empty sequence.");
			}

			if (values.IsSingleElement())
			{
				return Numeric<T, C>.Zero;
			}

			Numeric<T, C> result = Numeric<T, C>.Zero;
			ICalc<T> calculator = Numeric<T, C>.Calculator;

			int valuesCount = 0;

			foreach (T value in values)
			{
				result += Mathematics<T, C>.PowerInteger(calculator.Subtract(value, sampleAverage), 2);
				++valuesCount;
			}

			return result / (Numeric<T, C>)(valuesCount - 1);
        }

        /// <summary>
        /// Calculates the sample biased variation for a sequence of observations. 
        /// </summary>
        /// <typeparam name="T">The type of observations' values.</typeparam>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="values">The sequence of observations.</param>
        /// <param name="sampleAverage">The sample average for the observations sequence.</param>
        /// <returns>The sample biased variation for the sequence of observations.</returns>
        public static T SampleVariance<T, C>(this IEnumerable<T> values, T sampleAverage) where C : ICalc<T>, new()
        {
			Condition.ValidateNotNull(values, nameof(values));

            return SampleUnbiasedVariance<T, C>(values, sampleAverage) 
				* (Numeric<T,C>)(values.Count() - 1) 
				/ (Numeric<T,C>)(values.Count());
        }

        // - 
        // MOVING STATISTICS (INCLUDING AVERAGES)
        // -

        /// <summary>
        /// Specifies the type of the 
        /// window in calculation of the moving
        /// statistics. 
        /// </summary>
        public enum WindowType
        {
            /// <summary>
            /// A symmetric window includes
            /// values both to the left and to the right
            /// of the current value.
            /// 
            /// Window width = 2
            /// Array:
            /// 1 2 3 4 5
            /// 
            /// A symmetric window for 3 will include 1, 2, 3, 4 and 5.
            /// </summary>
            Symmetric,
            /// <summary>
            /// A forward window only includes
            /// values to the right of the current
            /// value.
            /// 
            /// Window width = 2
            /// Array:
            /// 1 2 3 4 5
            /// 
            /// A forward window for 3 will include 3, 4 and 5.
            /// </summary>
            Forward,
            /// <summary>
            /// A backward window only includes
            /// values to the left of the current value.
            /// 
            /// Window width = 2
            /// Array:
            /// 1 2 3 4 5
            /// 
            /// A backward window for 3 will include 1, 2 and 3.
            /// </summary>
            Backward
        }

        /// <summary>
        /// Specifies the method of handling the tail
        /// values during the calculation of moving averages
        /// and moving medians.
        /// 
        /// A tail value is such a value that does not
        /// have enough values in the specified window interval
        /// (e.g. the first and the second value of the array 
        /// when the window width is 2):
        /// 
        /// EXAMPLE:
        /// Symmetric (centered) moving average.
        /// 
        /// Window width = 2
        /// Array:
        /// 1 2 3 4 5 6
        /// Tail flags:
        /// T T - - T T
        /// </summary>
        public enum TailValuesHandling
        {
            /// <summary>
            /// This method will use any available 
            /// window for averaging / median finding,
            /// even if it means that the window
            /// becomes non-symmetric.
            /// 
            /// Symmetric moving average:
            /// 
            /// Window width = 2
            /// Array:
            /// 1 2 3 4 5 6
            /// T T - - T T
            /// 
            /// For 1, the window will include 1, 2 and 3 (since the two values to the left of 1 are not available).
            /// For 2, the window will include 1, 2, 3 and 4 (since only one of the two values to the left of 2 is available).
            /// For 3, the window will include 1, 2, 3, 4 and 5 (all the values in the symmetric window are available).
            /// ...
            /// 
            /// Result array:
            /// 2 2.5 3 4 4.5 5
            /// </summary>
            UseAllAvailableWindow,
            /// <summary>
            /// This flag only works for symmetric moving statistics.
            /// 
            /// This method will force the window to be centered (symmetric),
            /// even if it means that the window width will be decreased.
            /// 
            /// Symmetric moving average:
            /// 
            /// Window width = 2
            /// Array
            /// 1 2 3 4 5 6
            /// T T - - T T
            /// 
            /// For 1, the window will only include 1 
            /// (since the two values to the left of 1 are not available, 
            /// values to the right cannot be used either under the requirement
            /// of symmetric window - thus, the window width decreases to 0 for this value)
            /// 
            /// For 2, the window will include 1, 2 and 3
            /// (since only one of the two values to the left of 2 are available,
            /// only one value to the right can be used under the requirement
            /// of symmetric window - thus, the window width decreases to 1 for this value)
            /// 
            /// For 3, the window will include all the necessary values.
            /// 
            /// Result array:
            /// 1 2 3 4 5 6
            /// </summary>
            UseSymmetricAvailableWindow,
            /// <summary>
            /// This flag leaves the tail values 
            /// as is and does not apply the window averaging
            /// to them.
            /// 
            /// Backward window moving average:
            /// 
            /// Window width = 2
            /// Array
            /// 1 2 3 4 5
            /// T T - - -
            /// 
            /// For 1, the value isn't touched, since there are no two values to the left of 1.
            /// For 2, the value isn't touched, since there is only one of two values to the left of 2.
            /// For 3, the value gets averaged between 1, 2 and 3.
            /// ...
            /// 
            /// Result array:
            /// 1 2 2 3 4
            /// </summary>
            DoNotTouch,
            /// <summary>
            /// This flag forces the method to 
            /// exclude the tail values from the resulting array.
            /// 
            /// Backward window moving average:
            /// 
            /// Window width = 2
            /// Array
            /// 1 2 3 4 5
            /// T T - - -
            /// 
            /// For 1, the value gets excluded.
            /// For 2, the value gets excluded.
            /// For 3, the value gets averaged between 1, 2 and 3.
            /// ...
            /// 
            /// Result array:
            /// 2 3 4 
            /// </summary>
            DoNotInclude
        }

        /// <summary>
        /// Having passed the list of values,
        /// a non-negative window width, and
        /// a <see cref="TailValuesHandling"/> flag,
        /// returns the appropriate segment of the source
        /// list containing the values falling in the window.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="values">The source sequence.</param>
        /// <param name="windowWidth">A non-negative window width.</param>
        /// <param name="currentIndex">
        /// The current index in the source sequence 
        /// pointing to the current element (for which the window sequence is calculated)</param>
        /// <param name="tailValuesHandling">A tail values handling flag, see <see cref="TailValuesHandling"/></param>
        /// <param name="windowType">The window type, see <see cref="WindowType"/>.</param>
        /// <returns>
        /// A window sequence for the current element (referenced by <paramref name="currentIndex"/>).
        /// The sequence can then be used for calculating the average or the median value.
        /// 
        /// If the value is a tail value that should be excluded (see <see cref="TailValuesHandling.DoNotInclude"/>), 
        /// this method returns an empty list segment.
        /// </returns>
        private static ListSegment<T> __getWindowSequence<T>(
            IList<T> values,
            int windowWidth,
            WindowType windowType,
            TailValuesHandling tailValuesHandling,
            int currentIndex)
        {
			Condition.ValidateNotNull(values, nameof(values));
			Condition.ValidateNonNegative(windowWidth, "The window width should be non-negative.");
			Condition
				.Validate(currentIndex >= 0 && currentIndex < values.Count)
				.OrIndexOutOfRangeException("The index is outside the list boundaries.");

            // Check the available number of elements to the left 
            // and to the right of the current value.
            // -
            int amountToTheLeft = currentIndex;
            int amountToTheRight = values.Count - currentIndex - 1;

            bool isLeftTailValue = amountToTheLeft < windowWidth && windowType != WindowType.Forward;
            bool isRightTailValue = amountToTheRight < windowWidth && windowType != WindowType.Backward;
            
            // These are the boundary indices in the resulting
            // list segment. To be modified further.
            // -
            int leftIndex = currentIndex - (windowType == WindowType.Forward ? 0 : windowWidth);
            int rightIndex = currentIndex + (windowType == WindowType.Backward ? 0 : windowWidth);

            switch (tailValuesHandling)
            {
                case TailValuesHandling.DoNotInclude:

                    if (isLeftTailValue || isRightTailValue)
                    {
                        return new ListSegment<T>(values, currentIndex, 0);
                    }
                    break;

                case TailValuesHandling.DoNotTouch:

                    if (isLeftTailValue || isRightTailValue)
                    {
                        leftIndex = currentIndex;
                        rightIndex = currentIndex;
                    }
                    break;

                case TailValuesHandling.UseAllAvailableWindow:

                    if (isLeftTailValue)
                    {
                        leftIndex = currentIndex - amountToTheLeft;
                    }
                    if (isRightTailValue)
                    {
                        rightIndex = currentIndex + amountToTheRight;
                    }
                    break;

                case TailValuesHandling.UseSymmetricAvailableWindow:

                    if (isLeftTailValue || isRightTailValue)
                    {
                        int minimumAmountAvaliable = Math.Min(amountToTheLeft, amountToTheRight);

                        leftIndex = currentIndex - minimumAmountAvaliable;
                        rightIndex = currentIndex + minimumAmountAvaliable;
                    }
                    break;

                default:
                    throw new EnumFattenedException("Ouch!");
            }

            return new ListSegment<T>(values, leftIndex, rightIndex - leftIndex + 1);
        }

        /// <summary>
        /// Finds the moving statistic sequence from the sequence of values 
        /// using the specified non-negative window width and the statistic functor
        /// <paramref name="statistic"/> of type <c>Func&lt;IEnumerable&lt;T&gt;, T&gt;"</c>. 
        /// 
        /// The tail values (such that there is not enough data in the window around them
        /// get handled using <see cref="TailValuesHandling"/> flag passed).
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <typeparam name="C">The numeric calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="values">The sequence of values.</param>
        /// <param name="windowWidth">
        /// A non-negative window width applied to both sides of the current value.
        /// It means that, e.g. when the window width is 1, the window will consist
        /// of three values: the current value, one value to the left and one value to the right.
        /// </param>
        /// <param name="windowType">
        /// The type of the window for calculation of the average. 
        /// See <see cref="WindowType"/>
        /// </param>
        /// <param name="tailValuesHandling">
        /// A flag specifying how the tail values should be handled 
        /// (such that there is not enough data in the window around them).
        /// See <see cref="TailValuesHandling"/>.
        /// </param>
        /// <param name="statistic">
        /// A functor object taking an <c>IEnumerable&lt;T&gt;</c> sequence of observations
        /// and returning a statistic such as sample average, sample median, sample variance / standard
        /// deviation etc.
        /// </param>
        /// <returns>
        /// A sequence of statictics calculated in the window
        /// around each value from the source sequence.
        /// i.e. the i-th index in the result sequence means
        /// that the <paramref name="statistic"/> was calculated in the
        /// respective window around the i-th element of the source sequence.
        /// </returns>
        public static List<T> MovingStatistic<T, C>(
            this IList<T> values,
            int windowWidth,
            WindowType windowType,
            TailValuesHandling tailValuesHandling,
            Func<IEnumerable<T>, T> statistic)
            where C : ICalc<T>, new()
        {
			Condition.ValidateNotNull(values, nameof(values));
			Condition.ValidateNonNegative(windowWidth, "The window width should be non-negative.");
			Condition
				.Validate(tailValuesHandling != TailValuesHandling.UseSymmetricAvailableWindow || windowType == WindowType.Symmetric)
				.OrArgumentException("Symmetric tail values handling is only available for symmetric windows.");

            List<T> result = new List<T>(values.Count);

            for (int index = 0; index < values.Count; ++index)
            {
                ListSegment<T> windowSequence = __getWindowSequence<T>(
                    values,
                    windowWidth,
                    windowType,
                    tailValuesHandling,
                    index);

                if (windowSequence.IsEmpty())
                {
                    continue;
                }
                else
                {
                    T statisticValue = statistic(windowSequence);
                    result.Add(statisticValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Finds the moving average sequence from the sequence of values
        /// and the specified non-negative window width. The tail values
        /// (such that there is not enough data in the window around them
        /// get handled using <see cref="TailValuesHandling"/> flag passed).
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <typeparam name="C">The numeric calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="values">The sequence of values.</param>
        /// <param name="windowWidth">
        /// A non-negative window width applied to both sides of the current value.
        /// It means that, e.g. when the window width is 1, the window will consist
        /// of three values: the current value, one value to the left and one value to the right.
        /// </param>
        /// <param name="windowType">
        /// The type of the window for calculation of the average. 
        /// See <see cref="WindowType"/>
        /// </param>
        /// <param name="tailValuesHandling">
        /// A flag specifying how the tail values should be handled 
        /// (such that there is not enough data in the window around them).
        /// See <see cref="TailValuesHandling"/>.
        /// </param>
        /// <returns>A sequence of moving averages calculated from the source sequence.</returns>
        public static List<T> MovingAverage<T, C>(
            this IList<T> values, 
            int windowWidth, 
            WindowType windowType = WindowType.Symmetric,
            TailValuesHandling tailValuesHandling = TailValuesHandling.UseSymmetricAvailableWindow) 
            where C : ICalc<T>, new()
        {
			Condition.ValidateNotNull(values, nameof(values));
			Condition.ValidateNonNegative(windowWidth, "The window width should be non-negative.");
			Condition
				.Validate(tailValuesHandling != TailValuesHandling.UseSymmetricAvailableWindow || windowType == WindowType.Symmetric)
				.OrArgumentException("Symmetric tail values handling is only available for symmetric windows.");

            return MovingStatistic<T, C>(
                values,
                windowWidth,
                windowType,
                tailValuesHandling,
                StatisticExtensionMethods.SampleAverage<T, C>);
        }

        /// <summary>
        /// Finds the moving median sequence from the sequence of values
        /// and the specified non-negative window width. The tail values
        /// (such that there is not enough data in the window around them
        /// get handled using <see cref="TailValuesHandling"/> flag passed).
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <typeparam name="C">The numeric calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="values">The sequence of values.</param>
        /// <param name="windowWidth">
        /// A non-negative window width applied to both sides of the current value.
        /// It means that, e.g. when the window width is 1, the window will consist
        /// of three values: the current value, one value to the left and one value to the right.
        /// </param>
        /// <param name="windowType">
        /// The type of the window for calculation of the median. 
        /// See <see cref="WindowType"/>
        /// </param>
        /// <param name="tailValuesHandling">
        /// A flag specifying how the tail values should be handled 
        /// (such that there is not enough data in the window around them).
        /// See <see cref="TailValuesHandling"/>.
        /// </param>
        /// <returns>A sequence of moving medians calculated from the source sequence.</returns>
        public static List<T> MovingMedian<T, C>(
            this IList<T> values,
            int windowWidth,
            WindowType windowType = WindowType.Symmetric,
            TailValuesHandling tailValuesHandling = TailValuesHandling.UseSymmetricAvailableWindow)
            where C : ICalc<T>, new()
        {
			Condition.ValidateNotNull(values, nameof(values));
			Condition.ValidateNonNegative(windowWidth, "The window width should be non-negative.");
			Condition
				.Validate(tailValuesHandling != TailValuesHandling.UseSymmetricAvailableWindow || windowType == WindowType.Symmetric)
				.OrArgumentException("Symmetric tail values handling is only available for symmetric windows.");

            return MovingStatistic<T, C>(
                values,
                windowWidth,
                windowType,
                tailValuesHandling,
                SampleAverage<T, C>);
        }

        /// <summary>
        /// From a sequence of objects, creates a dictionary of their absolute frequencies.
        /// That is, for each unique value in the source sequence,
        /// stores it as a key, then counts the number of its occurrences and stores the number as 
        /// a value in the resulting key-value pair.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">A source sequence object.</param>
        /// <param name="equalityComparer">
        /// An equality comparer for the <typeparamref name="T"/> type. 
        /// If <c>null</c>, a default equality comparer will be used.
        /// </param>
        /// <returns>
        /// A dictionary of unique values from the <paramref name="sequence"/> with 
        /// their corresponding absolute frequencies (occurrence counts).
        /// </returns>
        public static Dictionary<T, int> FrequenciesAbsolute<T>(this IEnumerable<T> sequence, IEqualityComparer<T> equalityComparer = null)
        {
			Condition.ValidateNotNull(sequence, nameof(sequence));

			equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

            Dictionary<T, int> result = new Dictionary<T, int>(equalityComparer);

            foreach (T element in sequence)
            {
                if (result.ContainsKey(element))
                {
                    result[element]++;
                }
                else
                {
                    result.Add(element, 1);
                }
            }

            return result;
        }

        /// <summary>
        /// From a sequence of objects, creates a dictionary of their relative frequencies.
        /// That is, for each unique value in the source sequence,
        /// stores it as a key, then counts the number of its occurrences and stores its
        /// proportion in the total source sequence element count as a value in the resulting key-value pair.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">A source sequence object.</param>
        /// <param name="equalityComparer">
        /// An equality comparer for the <typeparamref name="T"/> type. 
        /// If <c>null</c>, a default equality comparer will be used.
        /// </param>
        /// <returns>
        /// A dictionary of unique values from the <paramref name="sequence"/> with 
        /// their corresponding relative frequencies (proportions).
        /// </returns>
        public static Dictionary<T, double> FrequenciesRelative<T>(this IEnumerable<T> sequence, IEqualityComparer<T> equalityComparer = null)
        {
			Condition.ValidateNotNull(sequence, nameof(sequence));

            equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            
            int elementCount = sequence.Count();
            Dictionary<T, int> absoluteFrequencies = sequence.FrequenciesAbsolute(equalityComparer);

            Dictionary<T, double> result = new Dictionary<T, double>(absoluteFrequencies.Count);

            foreach (KeyValuePair<T, int> keyValuePair in absoluteFrequencies)
            {
                result.Add(keyValuePair.Key, (double)keyValuePair.Value / elementCount);
            }

            return result;
        }
    }
}
