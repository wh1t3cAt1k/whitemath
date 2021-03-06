﻿using System;
using System.Collections.Generic;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;
using WhiteMath.General;

using WhiteStructs.Conditions;

namespace WhiteMath.Numeric
{
    /// <summary>
    /// This struct represents a numeric interval with either inclusive or exclusive bounds.
    /// Is supposed to be bounded.
    /// </summary>
    /// <typeparam name="T">The type of numbers in the interval.</typeparam>
    /// <typeparam name="C">The calculator for the number type.</typeparam>
    public struct BoundedInterval<T, C> where C: ICalc<T>, new()
    {
		private static ICalc<T> Calculator = Numeric<T, C>.Calculator;
        
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
		public bool IsZeroLength => Calculator.Equal(this.Length, Calculator.Zero);

        /// <summary>
        /// Tests whether the interval contains exactly 0 real-value points.
        /// </summary>
		public bool HasNoRealPoints => IsZeroLength && !IsLeftInclusive && !IsRightInclusive;

        /// <summary>
        /// Tests whether the interval contains exactly 0 integer points.
        /// </summary>
		public bool HasNoIntegerPoints => !(
			Mathematics<T, C>.Floor(this.LeftBound) + Numeric<T, C>._1 < this.RightBound
			|| this.IsRightInclusive && this.RightBound.FractionalPart == Numeric<T, C>.Zero
			|| this.IsLeftInclusive && this.LeftBound.FractionalPart == Numeric<T, C>.Zero);

        /// <summary>
        /// Returns the length of the interval.
        /// </summary>
        public Numeric<T, C> Length => RightBound - LeftBound;

        /// <summary>
        /// Returns the arithmetic middle of the interval.
        /// </summary>
        public Numeric<T, C> Middle => (RightBound + LeftBound) / Calculator.FromInteger(2);

        /// <summary>
        /// Returns the number of points in the interval.
        /// </summary>
        /// <remarks>Works only for integer <typeparamref name="T"/> types!</remarks>
        public T PointCount
        {
            get
            {
				Condition
					.Validate(Numeric<T, C>.Calculator.IsIntegerCalculator)
					.OrException(new NonIntegerTypeException(typeof(T).Name));

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

        /// <summary>
        /// Creates a new instance of an interval.
        /// </summary>
        /// <param name="left">The leftmost interval bound.</param>
        /// <param name="leftInclusive">The flag determining whether the leftmost bound is included into the interval.</param>
        /// <param name="right">The rightmost interval bound.</param>
        /// <param name="rightInclusive">The flag determining whether the rightmost bound is included into the interval.</param>
        public BoundedInterval(T left, T right, bool leftInclusive = false, bool rightInclusive = false): this()
        {
            if (!Calculator.GreaterThan(right, left))
                if (Calculator.GreaterThan(left, right))
                    throw new ArgumentException("The lower interval bound exceeds the upper interval bound.");
                else if (leftInclusive != rightInclusive)
                    throw new ArgumentException("The interval contains at most one point, but the bounds' inclusiveness is inconsistent. Please check.");

            LeftBound = left;
            IsLeftInclusive = leftInclusive;

            RightBound = right;
            IsRightInclusive = rightInclusive;
        }

        /// <summary>
        /// Tests whether the point is located inside the interval bounds.
        /// </summary>
        /// <param name="x">The point to test.</param>
        /// <returns>True if the interval contains the specified point, false otherwise.</returns>
        public bool Contains(T x)
        {
            return
                (Calculator.GreaterThan(x, LeftBound) && Calculator.GreaterThan(RightBound, x)) ||
                (IsLeftInclusive && Calculator.Equal(LeftBound, x)) ||
                (IsRightInclusive && Calculator.Equal(RightBound, x));
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
			Condition
				.Validate(Numeric<T, C>.Calculator.IsIntegerCalculator)
				.OrException(new NonIntegerTypeException(typeof(T).Name));
			
            Numeric<T, C> leftInclusive = this.IsLeftInclusive ? this.LeftBound : (this.LeftBound + Numeric<T, C>._1);
            Numeric<T, C> rightInclusive = this.IsRightInclusive ? this.RightBound : (this.RightBound - Numeric<T, C>._1);

			if (leftInclusive > rightInclusive)
			{
				throw new InvalidOperationException("The interval does not contain a single integer point, so it cannot be converted to a bound-inclusive interval.");
			}

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
			Condition
				.Validate(Numeric<T, C>.Calculator.IsIntegerCalculator)
				.OrException(new NonIntegerTypeException(typeof(T).Name));
            
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
			Condition
				.Validate(parts > 0)
				.OrArgumentOutOfRangeException("The number of parts should be positive.");
			Condition
				.Validate(this.Length / (Numeric<T, C>)parts != Numeric<T,C>.Zero)
				.OrArgumentException("The specified amount of parts will result in zero-length intervals due to numeric precision.");

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
			Condition
				.Validate(length > Numeric<T,C>.Zero)
				.OrArgumentOutOfRangeException("The desired length should be positive.");
			Condition
				.Validate(length <= (Numeric<T,C>)this.Length)
				.OrArgumentOutOfRangeException("The desired length should not exceed the current interval's length.");
            
			Numeric<T, C> partsCount = ((Numeric<T, C>)this.Length / length).IntegerPart;

            List<BoundedInterval<T, C>> result = new List<BoundedInterval<T, C>>();

            if (partsCount == Numeric<T,C>._1)
            {
				if (this.Length == length || options == BoundedIntervalSplitOptions.BiggerLastInterval)
				{
					result.Add(this);
				}
				else
				{
					result.Add(new BoundedInterval<T,C>(this.LeftBound, this.LeftBound + length, this.IsLeftInclusive, false));
                    result.Add(new BoundedInterval<T,C>(this.LeftBound + length, this.RightBound, true, this.IsRightInclusive));
                }

                return result;
            }

            // We have parts >= 2 here.
			// The first interval is treated separately, because its
			// left boundary should have the same inclusiveness as the
			// master interval.
			// -
			Numeric<T, C> leftBoundary = this.LeftBound;
			Numeric<T, C> rightBoundary = leftBoundary + length;

            result.Add(new BoundedInterval<T, C>(leftBoundary, rightBoundary, this.IsLeftInclusive, false));

            Numeric<T, C> i = Numeric<T, C>._1;

            for (; i < partsCount - Numeric<T,C>._1; i++)
            {
                leftBoundary = rightBoundary;
                rightBoundary = this.LeftBound + (i + Numeric<T,C>._1) * length;

                result.Add(new BoundedInterval<T, C>(leftBoundary, rightBoundary, true, false));
            }

			// Handle the last interval.
			// -
            leftBoundary = rightBoundary;
            rightBoundary = this.LeftBound + (i + Numeric<T, C>._1) * length;

            if (rightBoundary == this.RightBound || options == BoundedIntervalSplitOptions.BiggerLastInterval)
            {
                result.Add(new BoundedInterval<T, C>(leftBoundary, this.RightBound, true, this.IsRightInclusive));
            }
            else
            {
				// There is a tail remaining.
				// It should be added to an additional interval.
				// -
                result.Add(new BoundedInterval<T, C>(leftBoundary, rightBoundary, true, false));
                result.Add(new BoundedInterval<T, C>(rightBoundary, this.RightBound, true, this.IsRightInclusive));
            }

            return result;
        }

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
            public static Comparison<BoundedInterval<T, C>> LeftBoundWeakComparison
            {
                get
                {
                    return delegate(BoundedInterval<T, C> one, BoundedInterval<T, C> two)
                    {
                        if (Calculator.GreaterThan(one.LeftBound, two.LeftBound))
                            return 1;
                        else if (Calculator.Equal(one.LeftBound, two.LeftBound))
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
                        if (Calculator.GreaterThan(one.LeftBound, two.LeftBound))
                            return 2;
                        else if (Calculator.Equal(one.LeftBound, two.LeftBound))
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
			public static Comparison<BoundedInterval<T, C>> RightBoundWeakComparison
            {
                get
                {
                    return delegate(BoundedInterval<T, C> one, BoundedInterval<T, C> two)
                    {
                        if (Calculator.GreaterThan(one.RightBound, two.RightBound))
                            return 1;
                        else if (Calculator.Equal(one.RightBound, two.RightBound))
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
                        if (Calculator.GreaterThan(one.RightBound, two.RightBound))
                            return 2;
                        else if (Calculator.Equal(one.RightBound, two.RightBound))
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
                        if (Calculator.GreaterThan(two.Length, one.Length))
                            return 1;
                        else if (Calculator.Equal(two.Length, one.Length))
                            return 0;
                        else
                            return -1;
                    };
                }
            }
        }

        public override string ToString()
			=> (this.IsLeftInclusive ? "[" : "(")
				+ this.LeftBound 
				+ "; "
				+ this.RightBound
				+ (this.IsRightInclusive ? "]" : ")");

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
                Calculator.Equal(this.RightBound, interval.RightBound) &&
                Calculator.Equal(this.LeftBound, interval.LeftBound);
        }
    }

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
        /// A <c>PotentialResult&lt;Numeric&lt;T, C&gt;&gt;</c> object which will store
        /// the maximum number within a given interval for which the <paramref name="predicate"/> holds, if the algorithm
        /// finds one.
        /// </returns>
        public static PotentialResult<Numeric<T, C>> Max_LinearSearch<T, C>(this BoundedInterval<T, C> interval, Predicate<T> predicate)
            where C: ICalc<T>, new()
        {
			Condition
				.Validate(Numeric<T, C>.Calculator.IsIntegerCalculator)
				.OrException(new NonIntegerTypeException(typeof(T).Name));
			Condition
				.Validate(!interval.HasNoIntegerPoints)
				.OrArgumentException("The interval should contain at least one integer point.");
			Condition.ValidateNotNull(predicate, nameof(predicate));

			BoundedInterval<T, C> inclusiveInterval = interval.ToInclusiveIntegerInterval();

            bool found = false;

            Numeric<T, C> max = Numeric<T, C>.Zero;

            for (Numeric<T, C> current = inclusiveInterval.LeftBound; current <= inclusiveInterval.RightBound; ++current)
            {
                if (predicate(current))
                {
                    found = true;
                    max = current;
                }
            }

			if (!found)
			{
				return PotentialResult<Numeric<T, C>>.CreateFailure();
			}
			else
			{
				return PotentialResult<Numeric<T, C>>.CreateSuccess(max);
			}
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
			Condition
				.Validate(Numeric<T, C>.Calculator.IsIntegerCalculator)
				.OrException(new NonIntegerTypeException(typeof(T).Name));
			Condition
				.Validate(!interval.HasNoIntegerPoints)
			    .OrArgumentException("The interval should contain at least one integer point.");
			Condition.ValidateNotNull(predicate, nameof(predicate));

            Numeric<T, C> leftBoundary = (interval.IsLeftInclusive ? interval.LeftBound : interval.LeftBound + Numeric<T, C>._1);
            Numeric<T, C> rightBoundary = (interval.IsRightInclusive ? interval.RightBound : interval.RightBound - Numeric<T, C>._1); 

            Numeric<T, C> one = Numeric<T, C>._1;
            Numeric<T, C> two = Numeric<T, C>._2;

            while (!(leftBoundary > rightBoundary))
            {
                Numeric<T, C> middle = (leftBoundary + rightBoundary) / two;

				if (predicate(middle))
				{
					leftBoundary = middle + one;
				}
				else
				{
					rightBoundary = middle - one;
				}
            }

            return leftBoundary - one;
        }

        /// <summary>
		/// Searches a special-way sorted list of non-intersecting <see cref="BoundedInterval{T, C}"/> 
        /// intervals and returns the index of interval which contains the specified point.
        /// </summary>
        /// <typeparam name="T">A type of numbers contained inside the interval.</typeparam>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="list">A list of non-intersecting intervals, sorted on their left bounds.</param>
        /// <param name="point">The point to search for.</param>
        /// <remarks>Warning! If the list is either not sorted on interval's left bounds or contains several intersecting intervals, the result of the function is undefined.</remarks>
        /// <returns>The index of interval containing the specified point, or a negative value if such interval hasn't been found.</returns>
        public static int HitTest<T, C>(this IList<BoundedInterval<T, C>> list, T point) where C : ICalc<T>, new()
        {
			Condition.ValidateNotNull(list, nameof(list));
			
			int index = list.WhiteBinarySearch<T, BoundedInterval<T, C>>(
				(x => x.LeftBound), 
				point, 
				Numeric<T, C>.UnderlyingTypeComparer);

            // Why 1 is subtracted?
            // Because the left boundary of the "current" interval
            // after applying ~ will always be greater than the tested point.
			// -
			if (index < 0)
			{
				index = ~index - 1;
			}

            if (index >= 0 && index < list.Count)
            {
                if (list[index].Contains(point))
                    return index;
            }
            else if (index - 1 >= 0 && index - 1 < list.Count)
            {
				if (list[index - 1].Contains(point))
				{
					return index - 1;
				}
            }

            return -1;
        }
    }
}
