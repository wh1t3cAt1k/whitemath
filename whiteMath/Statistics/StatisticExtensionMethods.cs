using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

using whiteMath.General;

namespace whiteMath.Statistics
{
    [ContractVerification(true)]
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
            int count = values.Count();

            if (count == 0)
                throw new ArgumentException("Cannot calculate the sample average value for an empty sequence.");

            Numeric<T, C> sum = Numeric<T, C>.Zero;

            foreach (T value in values)
                sum += value;

            return sum / (Numeric<T,C>)values.Count();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="values"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static T SampleMedian<T, C>(this IEnumerable<T> values, IComparer<T> comparer = null) where C : ICalc<T>, new()
        {
            if(values.Count() == 0)
                throw new ArgumentException("Cannot evaluate the sample median value for an empty sequence.");

            T[] arr = values.ToArray();

            arr.SortShell(comparer);

            if (arr.Length % 2 == 0)
                return ((Numeric<T, C>)arr[(arr.Length - 1) / 2] + arr[arr.Length / 2]) / (Numeric<T, C>._2);
            else
                return (arr[arr.Length / 2]);
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

            if (count == 0)
                throw new ArgumentException("Cannot return the series count for an empty sequence.");
            else if (count == 1)
                return 0;

            int signChanges = 0;

            IEnumerator<T> enumerator = values.GetEnumerator();
            ICalc<T> calc = Numeric<T,C>.Calculator;

            enumerator.MoveNext();

            // доходим до первого значения, не равного медиане.

            while (calc.eqv(enumerator.Current, sampleMedian))
                if (enumerator.MoveNext() == false)
                    return 0;

            bool isPlus = calc.mor(enumerator.Current, sampleMedian);

            while (enumerator.MoveNext())
            {
                if (!isPlus && calc.mor(enumerator.Current, sampleMedian))
                {
                    signChanges++;
                    isPlus = true;
                }
                else if (isPlus && calc.mor(sampleMedian, enumerator.Current))
                {
                    signChanges++;
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
            int count = values.Count();

            if (count == 0)
                throw new ArgumentException("Cannot calculate the sample variance for an empty sequence.");
            else if (count == 1)
                return Numeric<T, C>.Zero;

            Numeric<T, C> sum = Numeric<T, C>.Zero;
            ICalc<T> calc = Numeric<T, C>.Calculator;

            foreach (T value in values)
                sum += WhiteMath<T, C>.PowerInteger(calc.dif(value, sampleAverage), 2);

            return sum / (Numeric<T, C>)(values.Count()-1);
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
            return SampleUnbiasedVariance<T, C>(values, sampleAverage) * (Numeric<T,C>)(values.Count() - 1) / (Numeric<T,C>)(values.Count());
        }

        // - 
        // MOVING AVERAGES
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
        /// <param name="tailValuesHandling">A tail values handling flag, see <see cref="TailvaluesHandling"/></param>
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
            Contract.Requires<ArgumentNullException>(values != null, "values");
            Contract.Requires<ArgumentOutOfRangeException>(windowWidth >= 0, "The window width should be non-negative.");
            Contract.Requires<IndexOutOfRangeException>(currentIndex >= 0 && currentIndex < values.Count, "The index is out of range");

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
            Contract.Requires<ArgumentNullException>(values != null, "values");
            Contract.Requires<ArgumentOutOfRangeException>(windowWidth >= 0, "The window width should be non-negative");
            Contract.Requires<ArgumentException>(
                tailValuesHandling != TailValuesHandling.UseSymmetricAvailableWindow ||
                windowType == WindowType.Symmetric,
                "Symmetric tail values handling is only available for symmetric windows.");

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
                    T average = SampleAverage<T, C>(windowSequence);
                    result.Add(average);
                }
            }

            return result;
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
            Contract.Requires<ArgumentNullException>(values != null, "values");
            Contract.Requires<ArgumentOutOfRangeException>(windowWidth >= 0, "The window width should be non-negative");
            Contract.Requires<ArgumentException>(
                tailValuesHandling != TailValuesHandling.UseSymmetricAvailableWindow ||
                windowType == WindowType.Symmetric,
                "Symmetric tail values handling is only available for symmetric windows.");

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
                    T median = SampleMedian<T, C>(windowSequence);
                    result.Add(median);
                }
            }

            return result;
        }
    }
}
