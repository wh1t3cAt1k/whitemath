using System;

using System.Collections.Generic;

using whiteMath.Algorithms;
using whiteMath.General;
using whiteMath.Randoms;

namespace whiteMath.Functions
{
    /// <summary>
    /// The generic function interface.
    /// </summary>
    /// <typeparam name="TypeArg">The type of the function argument.</typeparam>
    /// <typeparam name="TypeValue">The type of the function value.</typeparam>
    public interface IFunction<TypeArg, TypeValue>
    {
        TypeValue Value(TypeArg x);
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

    // ------------------------------------

    /// <summary>
    /// Static class containing extension methods for any kind of IFunction.
    /// </summary>
    public static class IFunctionExtensions
    {
        // --------------------------------------------
        // --------- КОНВЕРТАЦИЯ МЕЖДУ-----------------
        // ----------Func и IFunction -----------------
        // --------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IFunction<TArg, TVal> As_IFunction<TArg, TVal>(Func<TArg, TVal> func)
        {
            return new _FUNC_FUNCTION<TArg, TVal>(func);
        }

        public static Func<TArg, TVal> As_Func<TArg, TVal>(IFunction<TArg, TVal> function)
        {
            return delegate(TArg argument) { return function.Value(argument); };
        }

        /// <summary>
        /// Функция создаваемая на основе делегата Func(T,T).
        /// </summary>
        private class _FUNC_FUNCTION<TArg, TVal> : IFunction<TArg, TVal>
        {
            private Func<TArg, TVal> function;

            public _FUNC_FUNCTION(Func<TArg, TVal> function)
            {
                function.Assert_NotNull("The function delegate should not be null.");

                this.function = function;
            }

            public TVal Value(TArg x)
            {
                return function.Invoke(x);
            }
        }

        // --------------------------------------------
        // --------- ТАБЛИЦЫ ФУНКЦИЙ ------------------
        // --------------------------------------------

        /// <summary>
        /// Creates the function table basing on the arguments list.
        /// The function table is represented by the point array.
        /// </summary>
        /// <example>
        /// If the argument list is { 1, 2 }, then the object returned is
        /// { (1, f(1)), (2, f(2)) }.
        /// </example>
        /// <typeparam name="T">The type of function argument and value.</typeparam>
        /// <param name="obj">The calling function object.</param>
        /// <param name="arguments">The argument parameter list.</param>
        /// <returns>The function table in the format of a point array.</returns>
        public static Point<T>[] GetFunctionTable<T>(this IFunction<T, T> obj, IList<T> arguments)
        {
            Point<T>[] arr = new Point<T>[arguments.Count];

            for (int i = 0; i < arguments.Count; i++)
                arr[i] = new Point<T>(arguments[i], obj.Value(arguments[i]));

            return arr;
        }

        /// <summary>
        /// Creates the function table beginning from a certain point x0, 
        /// with a certain step and overall point count.
        /// The function table is represented by the point array.
        /// </summary>
        /// <typeparam name="T">The type of function argument and value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument/value type.</typeparam>
        /// <param name="obj">The calling function object.</param>
        /// <param name="x0">The first point of the table.</param>
        /// <param name="step">The step between the table points.</param>
        /// <param name="pointCount">The overall point count.</param>
        /// <returns>The function table in the format of a point array.</returns>
        public static Point<T>[] GetFunctionTable<T, C>(this IFunction<T, T> obj, T x0, T step, int pointCount) where C: ICalc<T>, new()
        {
            Point<T>[] arr = new Point<T>[pointCount];

            Numeric<T,C> current = x0;

            for (int i = 0; i < pointCount; i++)
            {
                arr[i] = new Point<T>(current, obj.Value(current));
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
        public static Point<T>[] GetFunctionTable<T, C>(this IFunction<T, T> obj, BoundedInterval<T,C> interval, int pointCount) where C: ICalc<T>, new()
        {
            Point<T>[] arr = new Point<T>[pointCount];

            Numeric<T, C> current;

            for (int i = 0; i < pointCount; i++)
            {
                current = interval.LeftBound + (interval.RightBound - interval.LeftBound) * (Numeric<T,C>)i / (Numeric<T,C>)(pointCount - 1);
                arr[i] = new Point<T>(current, obj.Value(current));
            }

            return arr;
        }

        // --------------------------------------------
        // --------- РАВЕНСТВО ФУНКЦИЙ ----------------
        // --------------------------------------------

        /// <summary>
        /// Checks whether the current function is equal to another at least within the
        /// finite set of points. 
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument.</typeparam>
        /// <param name="obj">The calling function object.</param>
        /// <param name="another">The function object to test equality with.</param>
        /// <param name="epsilon">The upper epsilon bound of 'equality' criteria. If |f(x) - g(x)| &lt; eps, two functions are considered equal.</param>
        /// <param name="points">The points list to test equality on.</param>
        /// <returns>True if the functions are epsilon-equal within the points list passed, false otherwise.</returns>
        public static bool PointwiseEquals<T,C>(this IFunction<T, T> obj, IFunction<T, T> another, T epsilon, IList<T> points) where C: ICalc<T>, new()
        {
            Func<T, T> abs = WhiteMath<T, C>.Abs;
            ICalc<T> calc = Numeric<T, C>.Calculator;

            for (int i = 0; i < points.Count; i++)
                if (!calc.mor(epsilon, abs(calc.dif(obj.Value(points[i]), another.Value(points[i])))))
                    return false;

            return true;
        }

        /// <summary>
        /// Checks whether the current function is equal to another at least within the
        /// finite set of points. 
        /// </summary>
        /// <typeparam name="T">The type of function argument/value.</typeparam>
        /// <typeparam name="C">The calculator for the function argument.</typeparam>
        /// <param name="obj">The calling function object.</param>
        /// <param name="another">The function object to test equality with.</param>
        /// <param name="epsilon">The upper epsilon bound of 'equality' criteria. If |f(x) - g(x)| &lt eps, two functions are considered equal.</param>
        /// <param name="points">The points list to test equality on.</param>
        /// <returns>True if the functions are epsilon-equal within the points list passed, false otherwise.</returns>
        public static bool MaybeEquals<T, C>(this IFunction<T, T> obj, IFunction<T, T> another, T epsilon, params T[] points) where C : ICalc<T>, new()
        {
            return PointwiseEquals<T,C>(obj, another, epsilon, points as IList<T>);
        }


        // --------------------------------------------
        // --------- ЗНАК ФУНКЦИИ ---------------------
        // --------------------------------------------

        /// <summary>
        /// Returns the number of sign variations in the function list for some certain
        /// point.
        /// </summary>
        /// <typeparam name="T">The type of polynom coefficients.</typeparam>
        /// <typeparam name="C">The type of polynom coefficients' calculator.</typeparam>
        /// <param name="list">The list of the polynoms to be analyzed.</param>
        /// <returns>The number of sign variations within the list. Zero values do not count.</returns>
        public static int SignVariations<T, C>(this IList<IFunction<T, T>> list, Numeric<T, C> point) where C : ICalc<T>, new()
        {
            int signPrev = 0;
            int signNew = 0;
            int k = 0;

            for (int i = 0; i < list.Count; i++)
            {
                signNew = WhiteMath<T, C>.Sign(list[i].Value(point));

                if (signPrev == 0)
                {
                    if (signNew != 0)
                        k++;
                    else continue;
                }
                else if (signPrev < 0 || signPrev > 0)
                {
                    if (signNew != 0)
                        k++;
                    else continue;
                }

                signPrev = signNew;
            }

            return k;
        }

        // --------------------------------------------
        // --------- ПОИСК МАКСИМУМА НА ИНТЕРВАЛЕ -----
        // --------------------------------------------

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
            Func<T, T, bool> test = (what == SearchFor.Maximum ? (Func<T, T, bool>)delegate(T a, T b) { return calc.mor(b, a); } : calc.mor);

            while (true)
            {
                if (right - left < absolutePrecision)
                    return (left + right) / two;

                leftThird = (left * two + right) / three;
                rightThird = (left + two * right) / three;

                funcValLT = obj.Value(leftThird);
                funcValRT = obj.Value(rightThird);

                if (test(funcValLT, funcValRT))
                    left = leftThird;
                else
                    right = rightThird;
            }
        }


        // --------------------------------------------
        // --------- ПОИСК НУЛЕЙ НА ИНТЕРВАЛЕ ---------
        // --------------------------------------------

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
        /// <param name="obj">The calling function object.</param>
        /// <param name="interval">The interval on which the zero is searched. The value </param>
        /// <param name="epsilonArg">The epsilon value of the argument interval. The 'root' argument would be actually any value in the interval [-epsilon; +epsilon]</param>
        /// <param name="epsilonFunc">The epsilon value of the function zero. That means, that any function value in the interval [-epsilonFunc; epsilonFunc] is considered zero value. This parameter is usually set to absolute zero (so that only true 0 counts), but may become useful when the function calculation method contains precision errors resulting in zero becoming 'non-zero'.</param>
        /// <returns>The argument value resulting in f(x) ~= 0.</returns>
        public static T ZeroSearchBisectionMethod<T, C>(this IFunction<T, T> obj, BoundedInterval<T, C> interval, Numeric<T,C> epsilonArg, Numeric<T,C> epsilonFunc) where C : ICalc<T>, new()
        {
            ICalc<T> calc = Numeric<T,C>.Calculator;
            
            Numeric<T,C> zero = Numeric<T,C>.Zero;
            Numeric<T,C> two = calc.fromInt(2);

            Numeric<T,C> left= interval.LeftBound;
            Numeric<T,C> right = interval.RightBound;

            Numeric<T,C> xMid;

            Func<T, T> abs = WhiteMath<T, C>.Abs;
            Func<T, int> sgn = WhiteMath<T, C>.Sign;

            Numeric<T,C> leftVal = obj.Value(left);
            Numeric<T,C> midVal;
            Numeric<T,C> rightVal = obj.Value(right);

            // -- флаг - чтобы не проделывать лишних вычислений.

            bool leftChanged = false;

            // ----- если на концах интервала ноль, то возвращаем сразу.

            if ((abs(leftVal) - zero) < epsilonFunc)
                return left;

            else if ((abs(rightVal) - zero) < epsilonFunc)
                return right;

            // --------- проверочка

            if (sgn(leftVal) == sgn(rightVal))
                throw new ArgumentException("Error: the function values are of the same sign on the interval bounds.");

            // ---------------------------------------------------------

            while(true)
            {                
                xMid = (right + left) / two;

                // ------- достигли требуемой точности - ура!

                if (xMid - left < epsilonArg)
                    return xMid;

                // ----------------------------------------------

                if(leftChanged)
                    leftVal = obj.Value(left);
                else 
                    rightVal = obj.Value(right);

                midVal = obj.Value(xMid);

                // ------------ ура! Нашли риальни ноль! --------

                if (abs(midVal - zero) < epsilonFunc)
                    return xMid;

                else if (sgn(rightVal) * sgn(midVal) < 0)
                {
                    leftChanged = true;
                    left = xMid;
                }

                else if (sgn(leftVal) * sgn(midVal) < 0)
                {
                    leftChanged = false;
                    right = xMid;
                }
                else
                    throw new FunctionException(string.Format("Some particular method iteration failed, possibly due to the precision loss. The function is intended to be of different signs on the interval bounds, but the values are {0} and {1}.", leftVal, rightVal));
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
        /// <param name="obj">The calling function object.</param>
        /// <param name="generator">The uniform distribution (pseudo)random generator.</param>
        /// <param name="interval">The interval on which the integral will be approximated.</param>
        /// <param name="rectangleHeight">The height of the testing rectangle. Should be more than the function's absolute maximum on the interval tested, otherwise the method would return wrong results. On the other side, the difference between the height and the function's absolute maximum should not be very large as it will reduce the accuracy of the method. The ideal case is equality of the max f(x) on [a; b] and the rectangle height.</param>
        /// <param name="throwsCount">A positive integer value of overall tests.</param>
        /// <returns>The value approximating the function integral on the interval specified.</returns>
        public static T IntegralMonteCarloApproximation<T, C>(this IFunction<T, T> obj, IRandomBounded<T> generator, BoundedInterval<T, C> interval, T rectangleHeight, T throwsCount) where C: ICalc<T>, new()
        {
            ICalc<T> calc = Numeric<T, C>.Calculator;

            T hits = calc.zero;     // overall hits.
            Point<T> randomPoint;   // random point.

            if(!calc.mor(throwsCount, calc.zero))
                throw new ArgumentException("The amount of point throws count should be a positive integer value.");

            for (T i = calc.zero; calc.mor(throwsCount, i); i = calc.increment(i))
            {
                randomPoint = new Point<T>(generator.Next(interval.LeftBound, interval.RightBound), generator.Next(calc.zero, rectangleHeight));

                // Если попали под функцию - увеличиваем количество хитов.

                if (!calc.mor(randomPoint.Y, obj.Value(randomPoint.X)))
                    hits = calc.increment(hits);
            }

            T result = calc.div(calc.mul(calc.mul(interval.Length, rectangleHeight), hits), throwsCount);

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

            // Если интервал нулевой, то ваще забей.

            if (interval.IsZeroLength)
                return calc.zero;

            // Если нет, то ваще не забей.
            
            Numeric<T,C> step = calc.div(interval.Length, calc.fromInt(pointCount));

            Numeric<T, C> two = (Numeric<T, C>)2;

            Numeric<T, C> left = interval.LeftBound + step / two;
            Numeric<T, C> right = interval.RightBound - step / two;

            // Будем хранить элементы по порядку, чтобы при суммировании не терять ерунды.
            
            SortedSet<Numeric<T, C>> sorted = new SortedSet<Numeric<T, C>>(Numeric<T, C>.NumericComparer);

            for (int i = 0; i < pointCount; i++)
            {
                T current = left + (right - left) * (Numeric<T, C>)i / (Numeric<T, C>)(pointCount - 1);
                sorted.Add(obj.Value(current));
            }

            // Теперь будем суммировать по порядку, начиная с самых маленьких.

            Numeric<T, C> sum = calc.zero;

            foreach (Numeric<T, C> element in sorted)
                sum += element;

            return sum*step;
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

            if (interval.IsZeroLength)
                return new BoundedInterval<T, C>(calc.zero, calc.zero, true, true);

            // Если нет, то ваще не забей.

            Point<T>[] table = obj.GetFunctionTable<T, C>(interval, pointCount+1);

            Numeric<T, C> step = calc.dif(table[2].X, table[0].X);
            
            Numeric<T, C> sumOne = calc.zero;
            Numeric<T, C> sumTwo = calc.zero;

            for (int i = 0; i < pointCount; i++)
            {
                if(i<pointCount-1)
                    sumOne += table[i].Y;
                if(i>0)
                    sumTwo += table[i].Y;
            }

            Numeric<T,C> res1 = sumOne * step;
            Numeric<T,C> res2 = sumTwo * step;

            return new BoundedInterval<T, C>(WhiteMath<T, C>.Min(res1, res2), WhiteMath<T, C>.Max(res1, res2), true, true);
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

            Point<T>[] table = obj.GetFunctionTable(interval, pointCount);

            Numeric<T, C> sum1 = table[1].Y;
            Numeric<T, C> sum2 = table[2].Y;

            for (int i = 2; i <= pointCount - 2; i+=2)
            {
                sum1 += table[i].Y;
                sum2 += table[i + 1].Y;
            }

            Numeric<T, C> result = (Numeric<T,C>.Calculator.sum(table[0].Y, table[table.Length - 1].Y) + (Numeric<T,C>)2 * sum2 + (Numeric<T,C>)4 * sum1) * (interval.RightBound - interval.LeftBound) / (Numeric<T,C>)(3 * pointCount);

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
            Numeric<T, C> hFourth = WhiteMath<T,C>.PowerInteger((interval.RightBound - interval.LeftBound) / (Numeric<T, C>)(pointCount / 2), 4);

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
            Numeric<T, C> nFourth = (Numeric<T,C>)maxFourthDerivative * WhiteMath<T, C>.PowerInteger(interval.RightBound - interval.LeftBound, 5) / (maxError * (Numeric<T, C>)2880 ); 
            
            // TODO: почему 100 членов в ряде тейлора?
            // надо вычислять точность.

            Numeric<T, C> power = (Numeric<T,C>)0.25;
            Numeric<T, C> n = WhiteMath<T, C>.SquareRootHeron(WhiteMath<T, C>.SquareRootHeron(nFourth, maxError), maxError);

            return Numeric<T,C>._2 * WhiteMath<T, C>.Ceiling(n);
        }
    }
}
