using System;
using System.Collections.Generic;
using System.Linq;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Randoms;

namespace WhiteMath.Functions
{
    /// <summary>
    /// The generic function interface.
    /// </summary>
    /// <typeparam name="TArgument">The type of the function argument.</typeparam>
    /// <typeparam name="TValue">The type of the function value.</typeparam>
	public interface IFunction<TArgument, TValue>
    {
        TValue GetValue(TArgument x);
    }

    /// <summary>
    /// The function interface providing the capability to get the derivative function for the current.
    /// </summary>
    /// <typeparam name="TypeArg">The type of the function argument.</typeparam>
    /// <typeparam name="TypeValue">The type of the function value.</typeparam>
    public interface IFunctionDifferentiable<TypeArg, TypeValue> : IFunction<TypeArg, TypeValue>
    {
        IFunctionDifferentiable<TypeArg, TypeValue> Derivative { get; }
    }

    /// <summary>
    /// Static class containing extension methods for any kind of IFunction.
    /// </summary>
    public static class IFunctionExtensions
    {
		public static IFunction<TArg, TVal> AsFunction<TArg, TVal>(Func<TArg, TVal> func)
        {
            return new DelegateFunction<TArg, TVal>(func);
        }

		public static Func<TArg, TVal> AsDelegate<TArg, TVal>(IFunction<TArg, TVal> function)
        {
			return argument => function.GetValue(argument);
        }

        private class DelegateFunction<TArg, TVal> : IFunction<TArg, TVal>
        {
            private Func<TArg, TVal> function;

            public DelegateFunction(Func<TArg, TVal> function)
            {
                function.Assert_NotNull("The function delegate should not be null.");

                this.function = function;
            }

            public TVal GetValue(TArg x)
            {
                return function.Invoke(x);
            }
        }

		/// <summary>
		/// Creates the function value table basing on the list of argument value.
		/// The function table is represented by the point array.
		/// </summary>
		/// <example>
		/// If the argument list is { 1, 2 }, then the object returned is
		/// { (1, f(1)), (2, f(2)) }.
		/// </example>
		/// <typeparam name="T">The type of function argument and value.</typeparam>
		/// <param name="function">The calling function object.</param>
		/// <param name="arguments">The argument parameter list.</param>
		/// <returns>The function table in the format of a point array.</returns>
		public static Point<T>[] GetValueTable<T>(this IFunction<T, T> function, IList<T> arguments)
			=> arguments
				.Select(argument => new Point<T>(argument, function.GetValue(argument)))
				.ToArray();

        /// <summary>
        /// Creates the function table beginning from a certain point x0, 
        /// with a certain step and overall point count.
        /// The function table is represented by the point array.
        /// </summary>
        /// <typeparam name="T">The type of function argument and value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument/value type.</typeparam>
        /// <param name="function">The calling function object.</param>
        /// <param name="x0">The first point of the table.</param>
        /// <param name="step">The step between the table points.</param>
        /// <param name="pointCount">The overall point count.</param>
        /// <returns>The function table in the format of a point array.</returns>
		public static Point<T>[] GetValueTable<T, C>(this IFunction<T, T> function, T x0, T step, int pointCount) 
			where C: ICalc<T>, new()
        {
            Point<T>[] arr = new Point<T>[pointCount];

            Numeric<T,C> current = x0;

            for (int i = 0; i < pointCount; i++)
            {
                arr[i] = new Point<T>(current, function.GetValue(current));
                current += step;
            }

            return arr;
        }

        /// <summary>
        /// Creates the function table on a certain interval with a specified amount of table points.
        /// </summary>
        /// <typeparam name="T">The type of function argument and value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument/value type.</typeparam>
        /// <param name="obj">The calling function object.</param>
        /// <param name="interval">The interval on which the table is created. Warning! Both of the interval bounds are counted INCLUSIVE!</param>
        /// <param name="pointCount">The overall point count of the function table</param>
        /// <returns>The function table in the format of point array.</returns>
		public static Point<T>[] GetValueTable<T, C>(
			this IFunction<T, T> obj, 
			BoundedInterval<T,C> interval, 
			int pointCount) where C: ICalc<T>, new()
        {
			Point<T>[] result = new Point<T>[pointCount];

            Numeric<T, C> current;

            for (int i = 0; i < pointCount; i++)
            {
                current = 
					interval.LeftBound 
					+ (interval.RightBound - interval.LeftBound) 
					        * (Numeric<T,C>)i / (Numeric<T,C>)(pointCount - 1);
				
                result[i] = new Point<T>(current, obj.GetValue(current));
            }

            return result;
        }

        /// <summary>
        /// Checks whether the current function is equal to another at least within the
        /// specified finite set of points. 
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument.</typeparam>
        /// <param name="currentFunction">The calling function object.</param>
        /// <param name="anotherFunction">The function object to test equality with.</param>
        /// <param name="epsilon">The upper epsilon bound of 'equality' criteria. If |f(x) - g(x)| &lt; eps, two functions are considered equal.</param>
        /// <param name="points">The points list to test equality on.</param>
        /// <returns>True if the functions are epsilon-equal within the points list passed, false otherwise.</returns>
        public static bool PointwiseEquals<T, C>(
			this IFunction<T, T> currentFunction, 
			IFunction<T, T> anotherFunction, 
			T epsilon, 
			IList<T> points) where C: ICalc<T>, new()
        {
            Func<T, T> abs = Mathematics<T, C>.Abs;
            ICalc<T> calc = Numeric<T, C>.Calculator;

			for (int i = 0; i < points.Count; i++)
			{
				if (!calc.GreaterThan(
					epsilon, 
					abs(
						calc.Subtract(
							currentFunction.GetValue(points[i]), 
							anotherFunction.GetValue(points[i])))))
				{
					return false;
				}
			}

            return true;
        }

        /// <summary>
        /// Checks whether the current function is equal to another at least within the
        /// specified finite set of points. 
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument.</typeparam>
        /// <param name="function">The calling function object.</param>
        /// <param name="anotherFunction">The function object to test equality with.</param>
        /// <param name="epsilon">The upper epsilon bound of 'equality' criteria. If |f(x) - g(x)| &lt eps, two functions are considered equal.</param>
        /// <param name="points">The points list to test equality on.</param>
        /// <returns>True if the functions are epsilon-equal within the points list passed, false otherwise.</returns>
		public static bool PointwiseEquals<T, C>(
			this IFunction<T, T> function, 
			IFunction<T, T> anotherFunction, 
			T epsilon, 
			params T[] points) where C : ICalc<T>, new()
        	=> PointwiseEquals<T, C>(function, anotherFunction, epsilon, points as IList<T>);

        /// <summary>
        /// Returns the number of sign variations in the function list for a given point.
        /// </summary>
        /// <typeparam name="T">The type of polynom coefficients.</typeparam>
        /// <typeparam name="C">The type of polynom coefficients' calculator.</typeparam>
        /// <param name="list">The list of the polynoms to be analyzed.</param>
        /// <returns>The number of sign variations within the list. Zero values do not count.</returns>
		public static int GetNumberOfSignChanges<T, C>(this IList<IFunction<T, T>> list, Numeric<T, C> point) where C : ICalc<T>, new()
        {
			int signPrevious = 0;
            int signNew = 0;
			int signChangesCount = 0;

            for (int i = 0; i < list.Count; i++)
            {
                signNew = Mathematics<T, C>.Sign(list[i].GetValue(point));

                if (signPrevious == 0)
                {
					if (signNew != 0)
					{
						++signChangesCount;
					}
					else
					{
						continue;
					}
                }
                else if (signPrevious < 0 || signPrevious > 0)
                {
					if (signNew != 0)
					{
						++signChangesCount;
					}
					else
					{
						continue;
					}
                }

                signPrevious = signNew;
            }

            return signChangesCount;
        }

        public enum SearchFor
        {
            Maximum,
            Minimum
        }

        /// <summary>
        /// Searches for the function maximum/minimum. using the ternary search method.
        /// 
        /// Conditions:
        /// 1a. If one searches for the maximum, f(x) should be STRICTLY increasing before the maximum and STRICTLY decreasing after the maximum.
        /// 1b. If one searches for the minimum, f(x) should be STRICTLY decreasing before the minimum and STRICTLY increasing after the minimum.
        /// 1*. Only one maximum/minimum should exist on the interval.
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument.</typeparam>
        /// <param name="obj">The calling function object.</param>
        /// <param name="interval">The interval on which the maximum lies.</param>
        /// <param name="absolutePrecision">The desired precision of the argument value.</param>
        /// <param name="what">What to search for, maximum or minimum. Should be of 'SearchFor' enum type.</param>
        /// <returns>The 'x0' value such that f(x0) ~= max f(x) on the interval.</returns>
        public static T MaximumSearchTernaryMethod<T, C>(this IFunction<T, T> obj, BoundedInterval<T, C> interval, T absolutePrecision, SearchFor what) where C: ICalc<T>, new()
        {
            Numeric<T, C> left = interval.LeftBound;
            Numeric<T, C> right = interval.RightBound;

            Numeric<T, C> leftThird;
            Numeric<T, C> rightThird;

            Numeric<T, C> two = (Numeric<T, C>)2;
            Numeric<T, C> three = (Numeric<T, C>)3;

            Numeric<T, C> funcValLT, funcValRT;

            ICalc<T> calc = Numeric<T,C>.Calculator;
            Func<T, T, bool> test = (what == SearchFor.Maximum ? (Func<T, T, bool>)delegate(T a, T b) { return calc.GreaterThan(b, a); } : calc.GreaterThan);

            while (true)
            {
                if (right - left < absolutePrecision)
                    return (left + right) / two;

                leftThird = (left * two + right) / three;
                rightThird = (left + two * right) / three;

                funcValLT = obj.GetValue(leftThird);
                funcValRT = obj.GetValue(rightThird);

				if (test(funcValLT, funcValRT))
				{
					left = leftThird;
				}
				else
				{
					right = rightThird;
				}
            }
        }

        /// <summary>
        /// Searches for the function root (zero value) on the interval [a; b].
        /// The interval is counted as with INCLUSIVE bounds!
        /// 
        /// The conditions of method success:
        /// 
        /// 1. The function f(x) is continuous on the interval [a; b].
        /// 2. The value f(a) and f(b) are of different signs.
        /// 
        /// These conditions guarantee the existence of the zero on the interval [a; b].
        /// The method results in finding ONE of these zeroes.
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the argument type.</typeparam>
        /// <param name="function">The calling function object.</param>
        /// <param name="interval">The interval on which the zero is searched. The value </param>
        /// <param name="epsilonArg">The epsilon value of the argument interval. The 'root' argument would be actually any value in the interval [-epsilon; +epsilon]</param>
        /// <param name="epsilonFunc">The epsilon value of the function zero. That means, that any function value in the interval [-epsilonFunc; epsilonFunc] is considered zero value. This parameter is usually set to absolute zero (so that only true 0 counts), but may become useful when the function calculation method contains precision errors resulting in zero becoming 'non-zero'.</param>
        /// <returns>The argument value resulting in f(x) ~= 0.</returns>
		public static T ZeroSearchBisectionMethod<T, C>(
			this IFunction<T, T> function, 
			BoundedInterval<T, C> interval, 
			Numeric<T,C> epsilonArg, 
			Numeric<T,C> epsilonFunc) where C : ICalc<T>, new()
        {
            ICalc<T> calc = Numeric<T,C>.Calculator;
            
            Numeric<T,C> zero = Numeric<T,C>.Zero;
            Numeric<T,C> two = calc.FromInteger(2);

            Numeric<T,C> left= interval.LeftBound;
            Numeric<T,C> right = interval.RightBound;

            Numeric<T,C> xMid;

            Func<T, T> abs = Mathematics<T, C>.Abs;
			Func<T, int> sign = Mathematics<T, C>.Sign;

            Numeric<T,C> leftVal = function.GetValue(left);
            Numeric<T,C> midVal;
            Numeric<T,C> rightVal = function.GetValue(right);

			bool isLeftBoundaryChanged = false;

			// If there is a zero at either boundary of the interval,
			// we return at once.
			// -
			if ((abs(leftVal) - zero) < epsilonFunc)
			{
				return left;
			}
			else if ((abs(rightVal) - zero) < epsilonFunc)
			{
				return right;
			}

			if (sign(leftVal) == sign(rightVal))
			{
				throw new ArgumentException("Error: the function values are of the same sign on the interval bounds.");
			}

            while(true)
            {                
                xMid = (right + left) / two;

				if (xMid - left < epsilonArg)
				{
					return xMid;
				}

				if (isLeftBoundaryChanged)
				{
					leftVal = function.GetValue(left);
				}
				else
				{
					rightVal = function.GetValue(right);
				}

                midVal = function.GetValue(xMid);

				if (abs(midVal - zero) < epsilonFunc)
				{
					return xMid;
				}
				else if (sign(rightVal) * sign(midVal) < 0)
				{
					isLeftBoundaryChanged = true;
					left = xMid;
				}
				else if (sign(leftVal) * sign(midVal) < 0)
				{
					isLeftBoundaryChanged = false;
					right = xMid;
				}
				else
				{
					throw new FunctionException(string.Format("Some particular method iteration failed, possibly due to the precision loss. The function is intended to be of different signs on the interval bounds, but the values are {0} and {1}.", leftVal, rightVal));
				}
			}
        }

        // --------------------------------------------
        // --------- ЧИСЛЕННОЕ ИНТЕГРИРОВАНИЕ ---------
        // --------------------------------------------

        /// <summary>
        /// Returns the Monte-Carlo stochastic approximation of the function integral.
        /// 
        /// Requirements: calculator for the function argument/value should provide
        /// reasonable implementation of the 'fromDouble()' method.
        /// </summary>
        /// <typeparam name="T">The type of the function's argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument.</typeparam>
        /// <param name="function">The calling function object.</param>
        /// <param name="generator">The uniform distribution (pseudo)random generator.</param>
        /// <param name="interval">The interval on which the integral will be approximated.</param>
        /// <param name="rectangleHeight">
		/// The height of the testing rectangle. Should be greater than 
		/// the function's absolute maximum on the interval tested, otherwise the method 
		/// will return incorrect results. On the other side, the difference between the height 
		/// and the function's absolute maximum should not be very large as it will reduce the 
		/// accuracy of the method. The ideal case is equality of the rectangle height and the function's
		/// maximum on the specified interval.
		/// </param>
        /// <param name="countThrows">A positive integer value of overall tests.</param>
        /// <returns>The value approximating the function integral on the interval specified.</returns>
        public static T IntegralMonteCarloApproximation<T, C>(
			this IFunction<T, T> function, 
			IRandomBounded<T> generator, 
			BoundedInterval<T, C> interval, 
			T rectangleHeight, 
			T countThrows) 
			where C: ICalc<T>, new()
        {
			ICalc<T> calculator = Numeric<T, C>.Calculator;

			T countHits = calculator.Zero;     // overall hits.
            Point<T> randomPoint;   // random point.

			if (!calculator.GreaterThan(countThrows, calculator.Zero))
			{
				throw new ArgumentException("The amount of point throws count should be a positive integer value.");
			}

            for (T i = calculator.Zero; calculator.GreaterThan(countThrows, i); i = calculator.Increment(i))
            {
                randomPoint = new Point<T>(generator.Next(interval.LeftBound, interval.RightBound), generator.Next(calculator.Zero, rectangleHeight));

				if (!calculator.GreaterThan(randomPoint.Y, function.GetValue(randomPoint.X)))
				{
					countHits = calculator.Increment(countHits);
				}
            }

            T result = calculator.Divide(
				calculator.Multiply(
					calculator.Multiply(
						interval.Length, 
						rectangleHeight), 
					countHits), 
				countThrows);

            return result;
        }

        /// <summary>
        /// Returns the rectangle approximation of the function integral.
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the argument type.</typeparam>
        /// <param name="obj">The calling function object.</param>
        /// <param name="interval">The interval to approximate the integral on.</param>
        /// <param name="pointCount">The overall point count used to calculate the integral.</param>
        /// <returns></returns>
        public static T IntegralRectangleApproximation<T, C>(this IFunction<T, T> obj, BoundedInterval<T, C> interval, int pointCount) where C : ICalc<T>, new()
        {
            ICalc<T> calc = Numeric<T, C>.Calculator;

			if (interval.HasZeroLength)
			{
				return calc.Zero;
			}
            
            Numeric<T, C> step = calc.Divide(interval.Length, calc.FromInteger(pointCount));
			Numeric<T, C> two = Numeric<T, C>._2;

            Numeric<T, C> left = interval.LeftBound + step / two;
            Numeric<T, C> right = interval.RightBound - step / two;

			Summator<T> summator = new Summator<T>(
				Numeric<T, C>._0,
				calc.Add);

			Numeric<T, C> sum = summator.SumSmallerToLarger(
				index => left + (right - left) * (Numeric<T, C>)index / (Numeric<T, C>)(pointCount - 1),
				0,
				pointCount - 1,
				Numeric<T, C>.UnderlyingTypeComparer);

            return sum * step;
        }

        /// <summary>
        /// Warning! Works correctly only for the MONOTONOUS (on the interval specified) function!
        /// Returns the bounded approximation of monotonous function's Riman integral.
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the argument type.</typeparam>
        /// <param name="obj">The calling function object.</param>
        /// <param name="interval">The interval to approximate the integral on.</param>
        /// <param name="pointCount">The overall point count used to approximate the integral value. The more this value is, the more precise is the calculation.</param>
        /// <returns>The interval in which the integral's value lies.</returns>
        public static BoundedInterval<T, C> IntegralRectangleBoundedApproximation<T, C>(this IFunction<T, T> obj, BoundedInterval<T, C> interval, int pointCount) where C : ICalc<T>, new()
        {
            ICalc<T> calc = Numeric<T, C>.Calculator;

            // Если интервал нулевой длины, то ваще забей.

            if (interval.HasZeroLength)
                return new BoundedInterval<T, C>(calc.Zero, calc.Zero, true, true);

            // Если нет, то ваще не забей.

            Point<T>[] table = obj.GetValueTable(interval, pointCount+1);

            Numeric<T, C> step = calc.Subtract(table[2].X, table[0].X);
            
            Numeric<T, C> sumOne = calc.Zero;
            Numeric<T, C> sumTwo = calc.Zero;

            for (int i = 0; i < pointCount; i++)
            {
                if(i<pointCount-1)
                    sumOne += table[i].Y;
                if(i>0)
                    sumTwo += table[i].Y;
            }

            Numeric<T,C> res1 = sumOne * step;
            Numeric<T,C> res2 = sumTwo * step;

            return new BoundedInterval<T, C>(Mathematics<T, C>.Min(res1, res2), Mathematics<T, C>.Max(res1, res2), true, true);
        }

        /// <summary>
        /// Uses the Simpson formula to calculate the approximate integral area
        /// on the interval using an even number of points.
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for function argument.</typeparam>
        /// <param name="obj">The calling function object.</param>
        /// <param name="interval">The interval on which the integral should be approximated. No matter if bounds are exclusive, they ARE COUNTED AS INCLUSIVE.</param>
        /// <param name="pointCount">The number of points that should be used to calculate the method.</param>
        /// <returns>The approximate value of the integral calculated by Simpson method.</returns>
        public static T IntegralSimpsonMethod<T, C>(this IFunction<T, T> obj, BoundedInterval<T, C> interval, int pointCount) where C: ICalc<T>, new()
        {
            if (pointCount % 2 > 0)
                throw new ArgumentException("The number of points for this function should be even.");
            else if (pointCount < 4)
                throw new ArgumentException("The number of points is too low for this method. It should be >= 4.");

            Point<T>[] table = obj.GetValueTable(interval, pointCount);

            Numeric<T, C> sum1 = table[1].Y;
            Numeric<T, C> sum2 = table[2].Y;

            for (int i = 2; i <= pointCount - 2; i+=2)
            {
                sum1 += table[i].Y;
                sum2 += table[i + 1].Y;
            }

            Numeric<T, C> result = (Numeric<T,C>.Calculator.Add(table[0].Y, table[table.Length - 1].Y) + (Numeric<T,C>)2 * sum2 + (Numeric<T,C>)4 * sum1) * (interval.RightBound - interval.LeftBound) / (Numeric<T,C>)(3 * pointCount);

            return result;
        }

        /// <summary>
        /// Returns the maximum error for the Simpson integral approximation method
        /// depending on the interval on which the calculation is performed,
        /// the maximum absolute value of the function's fourth derivative and the point count (should be even).
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument.</typeparam>
        /// <param name="obj">The calling function object. Is not actually used.</param>
        /// <param name="interval">The interval on which the calculation is performed. No matter if bounds are exclusive, they are considered INCLUSIVE.</param>
        /// <param name="pointCount">The point count on the interval.</param>
        /// <param name="maxFourthDerivative">The maximum absolute value of the function's fourth derivative.</param>
        /// <returns>The maximum error of the simpson method.</returns>
        public static T IntegralSimpsonMethodError<T, C>(this IFunction<T,T> obj, BoundedInterval<T, C> interval, int pointCount, T maxFourthDerivative) where C: ICalc<T>, new()
        {
            Numeric<T, C> big = (Numeric<T, C>)2880;            
            Numeric<T, C> hFourth = Mathematics<T,C>.PowerInteger((interval.RightBound - interval.LeftBound) / (Numeric<T, C>)(pointCount / 2), 4);

            return (interval.RightBound - interval.LeftBound) * hFourth * maxFourthDerivative / big;
        }

        /// <summary>
        /// TODO: write description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="obj"></param>
        /// <param name="interval"></param>
        /// <param name="maxError"></param>
        /// <param name="maxFourthDerivative"></param>
        /// <returns></returns>
        public static T IntegralSimpsonNeededPointCount<T, C>(this IFunction<T, T> obj, BoundedInterval<T, C> interval, T maxError, T maxFourthDerivative) where C : ICalc<T>, new()
        {
            // N^4 степени.
            Numeric<T, C> nFourth = (Numeric<T,C>)maxFourthDerivative * Mathematics<T, C>.PowerInteger(interval.RightBound - interval.LeftBound, 5) / (maxError * (Numeric<T, C>)2880 ); 
            
            // TODO: почему 100 членов в ряде тейлора?
            // надо вычислять точность.

            Numeric<T, C> power = (Numeric<T,C>)0.25;
            Numeric<T, C> n = Mathematics<T, C>.SquareRootHeron(Mathematics<T, C>.SquareRootHeron(nFourth, maxError), maxError);

            return Numeric<T,C>._2 * Mathematics<T, C>.Ceiling(n);
        }
    }
}
