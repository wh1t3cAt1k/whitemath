using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace whiteMath.General
{
    [ContractVerification(true)]
    public class HitTester<T, C> where C: ICalc<T>, new()
    {
        private List<BoundedInterval<T, C>> intervalList;
        private List<int> intervalHits;

        private int lastFoundIndex = -1;

        [ContractInvariantMethod]
        private void __invariant()
        {
            Contract.Invariant(this.intervalList != null);
            Contract.Invariant(this.intervalHits != null);
        }

        /// <summary>
        /// Initializes the <c>HitTester</c> with a sequence
        /// of non-intersecting intervals.
        /// </summary>
        /// <remarks>
        /// If the incoming <paramref name="intervalSequence"/> contains
        /// intersecting intervals, the behaviour of the <c>HitTester</c> is undefined.
        /// </remarks>
        /// <param name="intervalSequence">A sequence of non-intersecting intervals.</param>
        public HitTester(IEnumerable<BoundedInterval<T, C>> intervalSequence)
        {
            Contract.Requires<ArgumentNullException>(intervalSequence != null, "intervalSequence");
            Contract.Requires<ArgumentException>(intervalSequence.Count() > 0, "The interval sequence should contain at least 1 interval for reasonable hit testing.");

            this.intervalList = new List<BoundedInterval<T, C>>(intervalSequence);

            if (!this.intervalList.IsSorted(BoundedInterval<T, C>.IntervalComparisons.LeftBoundLazyComparison.CreateComparer()))
                this.intervalList.Sort(BoundedInterval<T, C>.IntervalComparisons.LeftBoundLazyComparison);

            this.intervalHits = new List<int>(this.intervalList.Count);
            this.intervalHits.AddRange(new int[this.intervalList.Count]);
        }

        /// <summary>
        /// Performs a hit test of a particular point,
        /// increasing the number of hits for the interval that contains it
        /// (if one has been found).
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>True if the interval containing the point has been found, false otherwise.</returns>
        public bool HitTest(T point)
        {
            lastFoundIndex = intervalList.HitTest(point);

            Contract.Assume(lastFoundIndex < this.intervalHits.Count);

            if (lastFoundIndex >= 0)
            {
                ++intervalHits[lastFoundIndex];
                ++TotalHits;

                return true;
            }
 
            return false;
        }

        /// <summary>
        /// Returns a <c>Nullable</c> object with
        /// either the interval containing the last point passed to
        /// the <c>HitTest()</c> method, or <c>null</c>
        /// if the last hit test was unsuccessful.
        /// </summary>
        public BoundedInterval<T, C>? LastFoundInterval
        {
            get
            {
                if (lastFoundIndex < 0)
                    return null;

                Contract.Assume(lastFoundIndex < this.intervalList.Count);

                return intervalList[lastFoundIndex];
            }
        }

        /// <summary>
        /// Returns the amount of hits registered for
        /// the specified interval.
        /// </summary>
        /// <param name="interval">The interval to test for amount of hits.</param>
        /// <returns>
        /// A non-negative amount of hits if the <c>Equals()</c>-equivalent interval 
        /// is registered in the tester; a negative value otherwise.</returns>
        public int HitCount(BoundedInterval<T, C> interval)
        {
            int index = this.intervalList.WhiteBinarySearch(interval, BoundedInterval<T, C>.IntervalComparisons.LeftBoundComparison.CreateComparer());

            Contract.Assume(index >= 0 && index < this.intervalHits.Count && index < this.intervalList.Count);

            if (this.intervalList[index].Equals(interval))
                return this.intervalHits[index];

            return -1;
        }

        /// <summary>
        /// Returns the amount of total hits registered.
        /// </summary>
        public int TotalHits { get; private set; }

        /// <summary>
        /// Returns the list of intervals associated with
        /// the current <c>HitTester&lt;<typeparamref name="T"/>,<typeparamref name="C"/>&gt;</c>,
        /// ordered by left bounds of the intervals.
        /// </summary>
        public List<BoundedInterval<T, C>> Intervals
        {
            get
            {
                return new List<BoundedInterval<T, C>>(this.intervalList);
            }
        }

        /// <summary>
        /// Returns the list of interval-hit pairs, ordered by 
        /// left bounds of the intervals.
        /// </summary>
        public List<KeyValuePair<BoundedInterval<T, C>, int>> IntervalHitPairs
        {
            get
            {
                List<KeyValuePair<BoundedInterval<T, C>, int>> result = 
                    new List<KeyValuePair<BoundedInterval<T, C>, int>>(this.intervalList.Count);

                result.FillByAppending(
                    this.intervalList.Count,
                    i => new KeyValuePair<BoundedInterval<T, C>, int>(this.intervalList[i], this.intervalHits[i]));

                return result;
            }
        }
    }
}
