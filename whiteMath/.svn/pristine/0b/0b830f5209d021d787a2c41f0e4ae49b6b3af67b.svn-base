using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

using whiteMath.General;

namespace whiteMath
{
    /// <summary>
    /// This enum contains options to specify when splitting the interval into a sequence of non-intersecting intervals
    /// and the source interval does not contain a whole number of intervals of the desired length. 
    /// </summary>
    public enum BoundedIntervalSplitOptions
    {
        /// <summary>
        /// When using this option, in case of presence of the 'tail' length,
        /// the last interval will have length bigger than the desired.
        /// The overall number of intervals produced will be floor(currentInterval.length / desiredLength).
        /// </summary>
        BiggerLastInterval,

        /// <summary>
        /// When using this option, in case of presence of the 'tail' length,
        /// the last interval will have length smaller than the desired.
        /// The overall number of intervals produced will be ceil(currentInterval.length / desiredLength).
        /// </summary>
        SmallerLastInterval
    }

    // ------------------------------------------
    // ------------------------------------------
    // ------------------------------------------

    /// <summary>
    /// This struct represents a numeric interval with either inclusive or exclusive bounds.
    /// Is supposed to be bounded.
    /// </summary>
    /// <typeparam name="T">The type of numbers in the interval.</typeparam>
    /// <typeparam name="C">The calculator for the number type.</typeparam>
    public struct BoundedInterval<T, C> where C:ICalc<T>, new()
    {
        private static ICalc<T> calc = Numeric<T, C>.Calculator;
        
        /// <summary>
        /// Gets the leftmost bound of the interval.
        /// </summary>
        public Numeric<T,C> LeftBound { get; private set; }

        /// <summary>
        /// Gets the rightmost bound of the interval.
        /// </summary>
        public Numeric<T,C> RightBound { get; private set; }

        /// <summary>
        /// Tests whether the leftmost bound is inclusive,.
        /// </summary>
        public bool IsLeftInclusive { get; private set; }

        /// <summary>
        /// Tests whether the rightmost bound is inclusive.
        /// </summary>
        public bool IsRightInclusive { get; private set; }

        /// <summary>
        /// Tests whether the interval has zero length.
        /// </summary>
        public bool IsZeroLength { get { return calc.eqv(this.Length, calc.zero); } }

        /// <summary>
        /// Tests whether the interval contains exactly 0 real-value points.
        /// </summary>
        public bool IsEmptyReal { get { return IsZeroLength && !IsLeftInclusive && !IsRightInclusive; } }

        /// <summary>
        /// Tests whether the interval contains exactly 0 integer points.
        /// </summary>
        public bool IsEmptyInteger
        {
            get
            {
                return
                    !
                    (WhiteMath<T, C>.Floor(this.LeftBound) + Numeric<T, C>._1 < this.RightBound ||
                    this.IsRightInclusive && this.RightBound.FractionalPart == Numeric<T, C>.Zero ||
                    this.IsLeftInclusive && this.LeftBound.FractionalPart == Numeric<T, C>.Zero);
            }
        }

        /// <summary>
        /// Returns the length of the interval.
        /// </summary>
        public Numeric<T, C> Length { get { return RightBound - LeftBound; } }

        /// <summary>
        /// Returns the arithmetic middle of the interval.
        /// </summary>
        public Numeric<T, C> Middle { get { return (RightBound + LeftBound) / calc.fromInt(2); } }

        /// <summary>
        /// Returns the amount of points in the interval.
        /// </summary>
        /// <remarks>Works only for integer <typeparamref name="T"/> types!</remarks>
        public T PointCount
        {
            get
            {
                Contract.Requires<NonIntegerTypeException>(Numeric<T, C>.Calculator.isIntegerCalculator, "The bounds of the interval should be of integer type.");

                Numeric<T, C> leftInclusive = (this.IsLeftInclusive ? this.LeftBound : (this.LeftBound + Numeric<T,C>._1));
                Numeric<T, C> rightInclusive = (this.IsRightInclusive ? this.RightBound : (this.RightBound - Numeric<T,C>._1));

                if (leftInclusive > rightInclusive) 
                    return Numeric<T,C>.Zero;

                return rightInclusive - leftInclusive + Numeric<T,C>._1;

                // (3; 3) => [4; 2]. 2 - 4 + 1 = -1.
                // (2; 3) => [3; 2]. 2 - 3 + 1 = 0.
                // (0; 1] => [1; 1]. 1 - 1 + 1 = 1. 
            }
        }

        // -----------------------------------
        // ----- constructors ----------------
        // -----------------------------------

        /// <summary>
        /// Creates a new instance of an interval.
        /// </summary>
        /// <param name="left">The leftmost interval bound.</param>
        /// <param name="leftInclusive">The flag determining whether the leftmost bound is included into the interval.</param>
        /// <param name="right">The rightmost interval bound.</param>
        /// <param name="rightInclusive">The flag determining whether the rightmost bound is included into the interval.</param>
        public BoundedInterval(T left, T right, bool leftInclusive = false, bool rightInclusive = false): this()
        {
            if (!calc.mor(right, left))
                if (calc.mor(left, right))
                    throw new ArgumentException("The lower interval bound exceeds the upper interval bound.");
                else if (leftInclusive != rightInclusive)
                    throw new ArgumentException("The interval contains at most one point, but the bounds' inclusiveness is inconsistent. Please check.");

            LeftBound = left;
            IsLeftInclusive = leftInclusive;

            RightBound = right;
            IsRightInclusive = rightInclusive;
        }

        // -----------------------------------
        // ----- instance methods ------------
        // -----------------------------------

        /// <summary>
        /// Tests whether the point is located inside the interval bounds.
        /// </summary>
        /// <param name="x">The point to test.</param>
        /// <returns>True if the interval contains the specified point, false otherwise.</returns>
        public bool Contains(T x)
        {
            return
                (calc.mor(x, LeftBound) && calc.mor(RightBound, x)) ||
                (IsLeftInclusive && calc.eqv(LeftBound, x)) ||
                (IsRightInclusive && calc.eqv(RightBound, x));
        }

        /// <summary>
        /// For integer intervals, returns an interval 
        /// containing the same integer values as the current, 
        /// but whose left and right bounds are both inclusive.
        /// </summary>
        /// <exception cref="NonIntegerTypeException">
        /// A <c>NonIntegerTypeException</c> will be raised if
        /// <typeparamref name="T"/> is not an integer type.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// An <c>InvalidOperationException</c> will be raised in case when
        /// the interval is empty so that it cannot be made both-bound inclusive.
        /// </exception>
        /// <returns>
        /// An interval 
        /// containing the same integer values as the current, 
        /// but whose left and right bounds are both inclusive.
        /// </returns>
        public BoundedInterval<T, C> ToInclusiveIntegerInterval()
        {
            Contract.Requires<NonIntegerTypeException>(Numeric<T,C>.Calculator.isIntegerCalculator, "The bounds of the interval should be of integer type.");
            
            Numeric<T, C> leftInclusive = this.IsLeftInclusive ? this.LeftBound : (this.LeftBound + Numeric<T, C>._1);
            Numeric<T, C> rightInclusive = this.IsRightInclusive ? this.RightBound : (this.RightBound - Numeric<T, C>._1);

            if (leftInclusive > rightInclusive)
                throw new InvalidOperationException("The interval does not contain a single integer point, so it cannot be converted to a bound-inclusive interval.");

            return new BoundedInterval<T, C>(leftInclusive, rightInclusive, true, true);
        }

        /// <summary>
        /// For integer intervals, returns an interval 
        /// containing the same integer values as the current, 
        /// but whose left and right bounds are both exclusive.
        /// </summary>
        /// <exception cref="NonIntegerTypeException">
        /// A <c>NonIntegerTypeException</c> will be raised if
        /// <typeparamref name="T"/> is not an integer type.
        /// </exception>
        /// <returns>
        /// An interval 
        /// containing the same integer values as the current, 
        /// but whose left and right bounds are both exclusive.
        /// </returns>
        public BoundedInterval<T, C> ToExclusiveIntegerInterval()
        {
            Contract.Requires<NonIntegerTypeException>(Numeric<T,C>.Calculator.isIntegerCalculator, "The bounds of the interval should be of integer type.");
            
            Numeric<T, C> leftExclusive = this.IsLeftInclusive ? (this.LeftBound - Numeric<T, C>._1) : this.LeftBound;
            Numeric<T, C> rightExclusive = this.IsRightInclusive ? (this.RightBound + Numeric<T, C>._1) : this.RightBound;

            return new BoundedInterval<T, C>(leftExclusive, rightExclusive, false, false);
        }

        // -----------------------------------
        // ----- splitting -------------------
        // -----------------------------------

        /// <summary>
        /// Splits the interval into a sequence of non-intersecting
        /// intervals of possibly equal length.
        /// </summary>
        /// <param name="parts">The number of parts.</param>
        /// <remarks>
        /// Due to roundoff errors the last interval of the sequence might be bigger than the others.
        /// </remarks>
        /// <returns>
        /// A <c>List</c> of [<c><paramref name="parts"/></c>] non-intersecting intervals which
        /// in union will make up the current.
        /// </returns>
        public List<BoundedInterval<T, C>> Split(int parts)
        {
            Contract.Requires<ArgumentOutOfRangeException>(parts > 0, "The amount of parts should be a positive integer number.");
            Contract.Requires<ArgumentException>(this.Length / (Numeric<T,C>)parts != Numeric<T,C>.Zero, "The specified amount of parts will result in zero-length intervals due to the numeric precision.");

            Contract.Ensures(Contract.Result<List<BoundedInterval<T, C>>>() != null);
            Contract.Ensures(Contract.Result<List<BoundedInterval<T,C>>>().Count > 0);

            return this.Split(this.Length / (Numeric<T,C>)parts, BoundedIntervalSplitOptions.BiggerLastInterval);
        }

        /// <summary>
        /// Splits the current interval into a sequence of intervals of desired length.
        /// </summary>
        /// <param name="length">The desired length of the interval.</param>
        /// <param name="options">
        /// If the length of the current interval does not contain a whole number of desired lengths, 
        /// the 'tail' length will be packed into the last interval depending on the options.
        /// </param>
        /// <returns>
        /// A sequence of intervals which give the current interval in union and have all
        /// (maybe except for the last one) the desired length.
        /// </returns>
        public List<BoundedInterval<T, C>> Split(T length, BoundedIntervalSplitOptions options)
        {
            Contract.Requires<ArgumentOutOfRangeException>(length > Numeric<T,C>.Zero, "The desired length should be positive.");
            Contract.Requires<ArgumentOutOfRangeException>(length <= (Numeric<T,C>)this.Length, "The desired length should not exceed the current interval's length.");
            Contract.Ensures(Contract.Result<List<BoundedInterval<T, C>>>() != null);

            Numeric<T, C> parts = ((Numeric<T, C>)this.Length / length).IntegerPart;

            List<BoundedInterval<T, C>> result = new List<BoundedInterval<T, C>>();

            if (parts == Numeric<T,C>._1)
            {
                // Если с длиной все в порядке
                // или же опция состоит в расширении последнего интервала, то добавляем текущий и не паримся.

                if ((Numeric<T, C>)this.Length == length || options == BoundedIntervalSplitOptions.BiggerLastInterval)
                    result.Add(this);

                // Иначе надо добавить кусочек текущего нужной длины + хвостик

                else
                {
                    result.Add(new BoundedInterval<T,C>(this.LeftBound, this.LeftBound + length, this.IsLeftInclusive, false));
                    result.Add(new BoundedInterval<T,C>(this.LeftBound + length, this.RightBound, true, this.IsRightInclusive));
                }

                return result;
            }

            // Здесь уже parts >= 2.
            // Отдельно обрабатываем первый интервал, потому что там
            // левая граница должна иметь ту же включенность, что и у текущего интервала.

            Numeric<T, C> lb = this.LeftBound;
            Numeric<T, C> rb = lb + length;

            result.Add(new BoundedInterval<T, C>(lb, rb, this.IsLeftInclusive, false));

            // Остальные, кроме последнего - в цикле.

            Numeric<T, C> i = Numeric<T, C>._1;

            for (; i < parts - Numeric<T,C>._1; i++)
            {
                lb = rb;
                rb = this.LeftBound + (i + Numeric<T,C>._1) * length;

                result.Add(new BoundedInterval<T, C>(lb, rb, true, false));
            }

            // Последний интервал. Тут несколько вариантов.

            lb = rb;
            rb = this.LeftBound + (i + Numeric<T, C>._1) * length;

            if (rb == this.RightBound || options == BoundedIntervalSplitOptions.BiggerLastInterval)
            {
                // Покрыли все без всяких хвостов.
                // ИЛИ ЖЕ последний интервал можно сделать немножко больше.
 
                result.Add(new BoundedInterval<T, C>(lb, this.RightBound, true, this.IsRightInclusive));
            }
            else
            {
                // Остался хвостик. 
                // Запихнем его в дополнительный интервал.

                result.Add(new BoundedInterval<T, C>(lb, rb, true, false));
                result.Add(new BoundedInterval<T, C>(rb, this.RightBound, true, this.IsRightInclusive));
            }

            return result;
        }

        // -----------------------------------
        // ----- comparers -------------------
        // -----------------------------------

        /// <summary>
        /// This class provides various-logic comparisons
        /// between bounded intervals.
        /// </summary>
        public static class IntervalComparisons
        {

            /// <summary>
            /// Compares the two intervals basing on their left bounds.
            /// Inclusiveness or exclusiveness of the bound doesn't count.
            /// </summary>
            public static Comparison<BoundedInterval<T, C>> LeftBoundLazyComparison
            {
                get
                {
                    return delegate(BoundedInterval<T, C> one, BoundedInterval<T, C> two)
                    {
                        if (calc.mor(one.LeftBound, two.LeftBound))
                            return 1;
                        else if (calc.eqv(one.LeftBound, two.LeftBound))
                            return 0;
                        else
                            return -1;
                    };
                }
            }

            /// <summary>
            /// Compares the two intervals basing on their left bounds.
            /// Exclusive bound is 'less' than inclusive.
            /// </summary>
            public static Comparison<BoundedInterval<T, C>> LeftBoundComparison
            {
                get
                {
                    return delegate(BoundedInterval<T, C> one, BoundedInterval<T, C> two)
                    {
                        if (calc.mor(one.LeftBound, two.LeftBound))
                            return 2;
                        else if (calc.eqv(one.LeftBound, two.LeftBound))
                        {
                            if (one.IsLeftInclusive && !two.IsLeftInclusive)
                                return 1;
                            else if (one.IsLeftInclusive == two.IsLeftInclusive)
                                return 0;
                            else
                                return -1;
                        }
                        else
                            return -2;
                    };
                }
            }

            /// <summary>
            /// Compares the two intervals basing on their right bounds.
            /// Inclusiveness or exclusiveness of the bound doesn't count.
            /// </summary>
            public static Comparison<BoundedInterval<T, C>> RightBoundLazyComparison
            {
                get
                {
                    return delegate(BoundedInterval<T, C> one, BoundedInterval<T, C> two)
                    {
                        if (calc.mor(one.RightBound, two.RightBound))
                            return 1;
                        else if (calc.eqv(one.RightBound, two.RightBound))
                            return 0;
                        else
                            return -1;
                    };
                }
            }

            /// <summary>
            /// Compares the two intervals basing on their right bounds.
            /// Exclusive bound is 'less' than inclusive.
            /// </summary>
            public static Comparison<BoundedInterval<T, C>> RightBoundComparison
            {
                get
                {
                    return delegate(BoundedInterval<T, C> one, BoundedInterval<T, C> two)
                    {
                        if (calc.mor(one.RightBound, two.RightBound))
                            return 2;
                        else if (calc.eqv(one.RightBound, two.RightBound))
                        {
                            if (one.IsRightInclusive && !two.IsRightInclusive)
                                return 1;
                            else if (one.IsRightInclusive == two.IsRightInclusive)
                                return 0;
                            else
                                return -1;
                        }
                        else
                            return -2;
                    };
                }
            }

            /// <summary>
            /// Compares two intervals basing on the interval length.
            /// Inclusiveness or exclusiveness of the bounds doesn't count.
            /// </summary>
            public static Comparison<BoundedInterval<T, C>> LengthComparison
            {
                get
                {
                    return delegate(BoundedInterval<T, C> one, BoundedInterval<T, C> two)
                    {
                        if (calc.mor(two.Length, one.Length))
                            return 1;
                        else if (calc.eqv(two.Length, one.Length))
                            return 0;
                        else
                            return -1;
                    };
                }
            }
        }

        // -----------------------------------
        // ----- object methods overriding----
        // -----------------------------------

        public override string ToString()
        {
            return (IsLeftInclusive?"[":"(")+LeftBound.ToString()+"; "+RightBound.ToString()+(IsRightInclusive?"]":")");
        }

        public override int GetHashCode()
        {
            return 
                (RightBound.GetHashCode() >> 3 << 3) + 
                (LeftBound.GetHashCode() >> 2 << 2) + 
                (IsLeftInclusive ? 2 : 0) + 
                (IsRightInclusive ? 1 : 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BoundedInterval<T, C>))
                return false;

            BoundedInterval<T,C> interval = (BoundedInterval<T, C>)obj;

            return
                this.IsLeftInclusive == interval.IsLeftInclusive &&
                this.IsRightInclusive == interval.IsRightInclusive &&
                calc.eqv(this.RightBound, interval.RightBound) &&
                calc.eqv(this.LeftBound, interval.LeftBound);
        }
    }

    // ---------------------------------------------------------------
    // ----------------------- EXTENSION METHODS ---------------------
    // ---------------------------------------------------------------

    /// <summary>
    /// This class provides different extensions (such as hit tests)
    /// for <c>BoundedInterval&lt;T, C&gt;</c> sequences.
    /// </summary>
    public static class BoundedIntervalExtensions
    {
        /// <summary>
        /// Finds the maximum number '<c>k</c>' within a given integer interval such that a predicate
        /// holds for this number '<c>k</c>'.
        /// </summary>
        /// <typeparam name="T">The type of numbers contained inside the interval.</typeparam>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="interval">
        /// An integer interval, containing at least one number for which the <paramref name="predicate"/>
        /// holds.
        /// </param>
        /// <param name="predicate">
        /// A predicate that should hold for the desired number and not for every number that is bigger and located
        /// inside the interval.
        /// </param>
        /// <remarks>
        /// This method takes <c>O(n)</c> time for intervals of length '<c>n</c>'. Thus, it usually works significantly 
        /// slower than <see cref="Max_BinarySearch&lt;T,C&gt;"/>, but does not put any restrictions 
        /// on the <paramref name="interval"/> object passed.
        /// </remarks>
        /// <returns>
        /// A <see cref="PotentialResult&lt;Numeric&lt;T,C&gt;&gt;"/> object which will store
        /// the maximum number within a given interval for which the <paramref name="predicate"/> holds, if the algorithm
        /// finds one.
        /// </returns>
        public static PotentialResult<Numeric<T, C>> Max_LinearSearch<T, C>(this BoundedInterval<T, C> interval, Predicate<T> predicate)
            where C: ICalc<T>, new()
        {
            Contract.Requires<ArgumentException>(Numeric<T, C>.Calculator.isIntegerCalculator, "This method works only for integer numbers.");
            Contract.Requires<ArgumentNullException>(predicate != null, "predicate");
            Contract.Requires<ArgumentException>(!interval.IsEmptyInteger, "The interval should contain at least one integer point.");

            BoundedInterval<T, C> inclusive = interval.ToInclusiveIntegerInterval();

            bool found = false;

            Numeric<T, C> max = Numeric<T, C>.Zero;

            for (Numeric<T, C> current = inclusive.LeftBound; current <= inclusive.RightBound; ++current)
            {
                if (predicate(current))
                {
                    found = true;
                    max = current;
                }
            }

            if (!found)
                return PotentialResult<Numeric<T, C>>.CreateFailure();
            else
                return PotentialResult<Numeric<T, C>>.CreateSuccess(max);
        }

        /// <summary>
        /// Finds the maximum number '<c>k</c>' within a given integer interval of special structure 
        /// such that a predicate holds for this number '<c>k</c>'.
        /// </summary>
        /// <typeparam name="T">The type of numbers contained inside the interval.</typeparam>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="interval">
        /// <para>
        /// An integer interval which has a structure such that all numbers, starting with the leftmost inclusive bound and 
        /// ending with the desired (sought-for) number, satisfy the predicate (and only these).
        /// In other words, the leftmost 'tail' of the interval should satisfy the predicate, 
        /// and the rightmost 'tail' should not; the rightmost tail, though, may be empty.
        /// </para>
        /// <para>
        /// It should be KNOWN that the interval has such structure and that at least the leftmost inclusive bound satisfies the predicate.
        /// Otherwise, the behaviour of the function is undefined.
        /// </para>
        /// </param>
        /// <param name="predicate">
        /// A predicate that should hold for all numbers, starting with the leftmost bound and ending with the 
        /// desired (sought-for) number, and only for these.
        /// </param>
        /// <returns>The maximum number within a given interval for which the <paramref name="predicate"/> holds.</returns>
        public static T Max_BinarySearch<T, C>(this BoundedInterval<T, C> interval, Predicate<T> predicate)
            where C: ICalc<T>, new()
        {
            Contract.Requires<NonIntegerTypeException>(Numeric<T, C>.Calculator.isIntegerCalculator, "This method works only for integer numbers.");
            Contract.Requires<ArgumentException>(!interval.IsEmptyInteger, "The interval should contain at least one integer point.");

            Numeric<T, C> lb = (interval.IsLeftInclusive ? interval.LeftBound : interval.LeftBound + Numeric<T, C>._1);
            Numeric<T, C> rb = (interval.IsRightInclusive ? interval.RightBound : interval.RightBound - Numeric<T, C>._1); 

            Numeric<T, C> one = Numeric<T, C>._1;
            Numeric<T, C> two = Numeric<T, C>._2;

            while (!(lb > rb))
            {
                Numeric<T, C> mid = (lb + rb) / two;

                if (predicate(mid))
                    lb = mid + one;
                else
                    rb = mid - one;
            }

            return lb - one;
        }

        /// <summary>
        /// Searches a special-way sorted list of non-intersecting <c>BoundedInterval&lt;<typeparamref name="T"/>, <typeparamref name="C"/>&gt;</c> 
        /// elements and returns the index of interval which contains the specified point.
        /// </summary>
        /// <typeparam name="T">A type of numbers contained inside the interval.</typeparam>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="list">A list of non-intersecting intervals, sorted on their left bounds.</param>
        /// <param name="point">The point to search for.</param>
        /// <remarks>Warning! If the list is either not sorted on interval's left bounds or contains several intersecting intervals, the result of the function is undefined.</remarks>
        /// <returns>The index of interval containing the specified point, or a negative value if such interval hasn't been found.</returns>
        public static int HitTest<T, C>(this IList<BoundedInterval<T, C>> list, T point) where C : ICalc<T>, new()
        {
            Contract.Requires<ArgumentNullException>(list != null, "list");
            Contract.Ensures(Contract.Result<int>() < list.Count);

            int index = list.WhiteBinarySearch((x => x.LeftBound), point, Numeric<T, C>.TComparer);

            // Почему минус единица...
            // Потому что левая граница "текущего" интервала
            // после ~ по любому будет больше тестируемой точки.

            if (index < 0)
                index = ~index - 1;

            if (index >= 0 && index < list.Count)
            {
                if (list[index].Contains(point))
                    return index;
            }
            else if (index - 1 >= 0 && index - 1 < list.Count)
            {
                if (list[index - 1].Contains(point))
                    return index - 1;
            }

            return -1;
        }
    }
}
