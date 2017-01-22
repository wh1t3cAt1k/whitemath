using System;
using System.Collections.Generic;
using System.Linq;

using WhiteMath.Calculators;
using WhiteMath.General;

namespace WhiteMath.Functions
{
    /// <summary>
    /// This enum determines the type of the piece function.
    /// </summary>
    public enum PieceFunctionType
    {
        PieceLinearFunction, NaturalCubicSpline, Other 
    }

    /// <summary>
    /// Represents a piece function where argument intervals are mapped to their respective functions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="C"></typeparam>
    public class PieceFunction<T,C>: IFunction<T, T> where C: ICalc<T>, new()
    {
        protected KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>>[] pieces;
        protected T[] intervalLefts;

        protected T defaultValue;
        
        /// <summary>
        /// Gets the type of the piece function.
        /// </summary>
        public PieceFunctionType Type { get; internal set; }

		private static IComparer<KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>>> _kvpComparerInstance 
			= new _KVPComparer();

        /// <summary>
		/// Compares key-value pairs based on their interval left boundaries.
        /// </summary>
        private class _KVPComparer : IComparer<KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>>>
        {
            private Comparison<BoundedInterval<T, C>> _comparison = BoundedInterval<T, C>.IntervalComparisons.LeftBoundComparison;

            public int Compare(
				KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>> one, 
				KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>> two)
            {
                return _comparison(one.Key, two.Key);
            }
        }

        /// <summary>
        /// Creates a new instance of PieceFunction using a parameter list of KeyValuePair objects and a default value.
        /// </summary>
        /// <param name="pieces">The list of the KeyValuePair objects mapping intervals to functions.</param>
        /// <param name="defaultValue">The default value to return when the argument passed maps to no interval.</param>
        public PieceFunction(T defaultValue, params KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>>[] pieces)
            : this(pieces, defaultValue)
        { }

        /// <summary>
        /// Creates a new instance of PieceFunction using a list (or array) of KeyValuePair objects and a default value.
        /// </summary>
        /// <param name="pieces">The list of the KeyValuePair objects mapping intervals to functions.</param>
        /// <param name="defaultValue">The default value to return when the argument passed maps to no interval.</param>
        public PieceFunction(IList<KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>>> pieces, T defaultValue)
        {
            if (pieces is KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>>[])
                this.pieces = pieces as KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>>[];
            else
                this.pieces = pieces.ToArray();

            // Сортируем массив по левым границам.

            Array.Sort(this.pieces, _kvpComparerInstance);

            // Создаем отдельно массив левых границ, чтобы быстро искать
            // При вычислении значения функции.

            this.intervalLefts = new T[pieces.Count];

            for (int i = 0; i < this.pieces.Length; i++)
                this.intervalLefts[i] = this.pieces[i].Key.LeftBound;

            // Назначаем значение по умолчанию.

            this.defaultValue = defaultValue;

            // Проверяем.

            this.selfCheck();
        }

        /// <summary>
        /// Creates a new instance of PieceFunction using two separate lists
        /// of intervals and functions. The lengths of the lists should be the same,
        /// with intervals[i] logically mapped to functions[i].
        /// </summary>
        /// <param name="intervals">The list of intervals for the piece function.</param>
        /// <param name="pieces">The list of functions for the piece function.</param>
        /// <param name="defaultValue">The default value to return when the argument is out of every interval's bounds.</param>
        public PieceFunction(IList<BoundedInterval<T,C>> intervals, IList<IFunction<T,T>> functions, T defaultValue)
        {
            if (intervals.Count != functions.Count)
                throw new ArgumentException("The lengths of the intervals' and functions' lists should be the same.");

            // Создаем всяческую ерунду и сортируем ее.

            this.pieces = new KeyValuePair<BoundedInterval<T, C>, IFunction<T, T>>[intervals.Count];
            
            for (int i = 0; i < intervals.Count; i++)
                this.pieces[i] = new KeyValuePair<BoundedInterval<T,C>, IFunction<T, T>>(intervals[i], functions[i]);

            Array.Sort(this.pieces, _kvpComparerInstance);

            // Отдельно - массив левых границ
            
            this.intervalLefts = new T[intervals.Count];

            for (int i = 0; i < this.pieces.Length; i++)
                this.intervalLefts[i] = this.pieces[i].Key.LeftBound;

            // Значение по умолчанию

            this.defaultValue = defaultValue;

            this.selfCheck();
        }

        // -------------------------
        // ------ checker-----------
        // -------------------------

        /// <summary>
        /// Checks that intervals do not intersect.
        /// </summary>
        private void selfCheck()
        {
            HashSet<T> exclusiveLefts = new HashSet<T>();
            HashSet<T> inclusiveLefts = new HashSet<T>();
            HashSet<T> exclusiveRights = new HashSet<T>();
            HashSet<T> inclusiveRights = new HashSet<T>();

            ArgumentException ex = new ArgumentException("The intervals passed to the constructor intersect, but they shouldn't. Please check."); 

            for (int i=0; i<this.pieces.Length; i++)
            {
                BoundedInterval<T, C> interval = this.pieces[i].Key;

                if (!interval.IsLeftInclusive && exclusiveLefts.Contains(interval.LeftBound))
                    throw ex;
                else
                    exclusiveLefts.Add(interval.LeftBound);

                if (interval.IsLeftInclusive && inclusiveLefts.Contains(interval.LeftBound))
                    throw ex;
                else
                    inclusiveLefts.Add(interval.LeftBound);

                if (!interval.IsRightInclusive && exclusiveRights.Contains(interval.RightBound))
                    throw ex;
                else
                    inclusiveLefts.Add(interval.LeftBound);

                if (interval.IsRightInclusive && inclusiveRights.Contains(interval.RightBound))
                    throw ex;
                else
                    inclusiveLefts.Add(interval.LeftBound);

                // проверяем на пересечение, чтобы включающие границы не пересекались

                if (inclusiveLefts.Intersect(inclusiveRights).Count() > 0)
                    throw ex;
            }
        }

        // -------------------------
        // ------ value ------------
        // -------------------------

        IComparer<BoundedInterval<T,C>> comp = BoundedInterval<T,C>.IntervalComparisons.LeftBoundComparison.CreateComparer();

        /// <summary>
        /// Returns the value of the piece function.
        /// Searches through the intervals mapping to the functions,
        /// when the argument is within the interval, the value of the 
        /// respective function is returned.
        /// </summary>
        /// <param name="x">The argument of the function.</param>
        /// <returns>The value of the piece function in the specified point.</returns>
        public T GetValue(T x)
        {
            int index = Array.BinarySearch(intervalLefts, x);

            if (index < 0)
            {
                index = ~index;

                if (index > intervalLefts.Length || index == 0)
                    return this.defaultValue;
                else
                    index--;
            }

            BoundedInterval<T, C> needed = pieces[index].Key;

            if (needed.Contains(x))
                return pieces[index].Value.GetValue(x);

                // граничный случай - попали в исключенную границу интервала. Ищем [3; 3], а попали в needed на (3; 6].
                // надо поискать в прошлом интервале.

            else if (index > 0 && pieces[index - 1].Key.Contains(x))
                return pieces[index - 1].Value.GetValue(x);
            
                // если нет - ничего не поделаешь.

            else
                return this.defaultValue;
        }
    }
}
