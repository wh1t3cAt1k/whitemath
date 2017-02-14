using System;
using System.Linq;

using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Matrices;

namespace WhiteMath.Vectors
{
    /// <summary>
    /// Represents a numeric vector.
    /// </summary>
    /// <typeparam name="T">
	/// The type of numbers stored by the vector.
	/// </typeparam>
    public class Vector<T, C> : ICloneable, IMutableMatrix<T>, IMutableMatrix<Numeric<T, C>> 
		where C : ICalc<T>, new()
    {
		private static C calculator = new C();

		private T[] _array;

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public int Length => _array.Length; 

        /// <summary>
        /// The indexer of the vector.
        /// </summary>
		public Numeric<T, C> this[int elementIndex]
        {
            get { return _array[elementIndex]; }
            set { _array[elementIndex] = value; }
        }

        /// <summary>
        /// A copy constructor.
        /// Creates an independent copy of the vector object passed.
        /// </summary>
		public Vector(Vector<T, C> source)
        {
			_array = source._array
				.Select(element => calculator.GetCopy(element))
				.ToArray();
        }

        /// <summary>
        /// Creates a new vector filled up with zero numeric values.
        /// </summary>
        /// <param name="dimension"></param>
        public Vector(int dimension)
            : this(dimension, calculator.Zero)
        { }

        /// <summary>
        /// Creates a new vector with elements all initialized equal to the value passed.
        /// </summary>
        public Vector(int dimension, T value)
        {
            _array = new T[dimension];
			_array.FillByAssign(_ => calculator.GetCopy(value));
        }

        /// <summary>
        /// Private constructor used by helper methods.
        /// </summary>
        private Vector(T[] array)
        {
            this._array = array;
            return;
        }

        /// <summary>
        /// Creates the vector as a wrapper for the array passed.
        /// </summary>
		/// <remarks>
		/// (!) All changes to the vector will be reflected on the array, and vice versa.
		/// </remarks>
		public static Vector<T, C> Wrap(T[] array)
        {
            return new Vector<T, C>(array);
        }

        /// <summary>
        /// Creates the vector using the shallow copy for the array passed.
        /// If the shallow array copy is logically deep, no changes to the vector
        /// will be reflected on the array, and vice versa.
        /// </summary>
        public static Vector<T, C> CreateIndependent(T[] array)
        {
            return new Vector<T, C>(Wrap(array));
        }

        /// <summary>
        /// Unwraps the current vector to present it as a simple T array.
        /// All changes to the array object returned will be reflected on the vector,
        /// and vice versa.
        /// </summary>
		public T[] Unwrap
        {
            get { return this._array; }
        }

        /// <summary>
        /// Unwraps the current vector to present it as a Numeric array.
        /// </summary>
        public Numeric<T, C>[] AsNumericArray()
        {
            return this._array.ConvertToNumericArray<T, C>();
        }

        public static implicit operator Vector<T, C>(T[] array)
        {
            return Vector<T, C>.Wrap(array);
        }

        public static implicit operator T[](Vector<T, C> vector)
        {
            return vector.Unwrap;
        }

        /// <summary>
        /// Returns the string representation of the current vector using 
        /// the specified format string for each of the vector elements.
        /// </summary>
        /// <param name="vectorElementsFormatter">The format string for vector elements</param>
        /// <param name="elementSeparator">The string that separates different vector elements, ex.: "; "</param>
        /// <returns></returns>
        public string ToString(string vectorElementsFormatter, string elementSeparator)
        {
            string res = "(";

            foreach (T obj in _array)
                res += String.Format(vectorElementsFormatter, obj) + elementSeparator;

            res = res.Remove(res.Length - 2) + ")";

            return res;
        }

        /// <summary>
        /// Returns the string representation of the current vector using
        /// the default string format for each of the vector elements.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString("{0}", "; ");
        }

        public object Clone()
        {
            return new Vector<T, C>(this);
        }

		int IMatrix.ColumnCount => 1;
        int IMatrix.RowCount => Length;

		private void CheckMatrixRange(int row, int column)
        {
			if (column > 0 || row >= Length)
			{
				throw new ArgumentOutOfRangeException("At least one of the indexes was out of range.");
			}
        }

        object IMatrix.GetElementAt(int row, int column)
        {
            CheckMatrixRange(row, column);
            return this[row];
        }

        T IMatrix<T>.GetElementAt(int row, int column)
        {
            CheckMatrixRange(row, column);
            return this[row];
        }

        Numeric<T, C> IMatrix<Numeric<T, C>>.GetElementAt(int row, int column)
        {
            CheckMatrixRange(row, column);
            return this[row];
        }

        void IMutableMatrix.SetElementAt(int row, int column, object value)
        {
            CheckMatrixRange(row, column);
            this[row] = (T)value;
        }

        void IMutableMatrix<T>.SetElementAt(int row, int column, T value)
        {
            CheckMatrixRange(row, column);
            this[row] = value;
        }

        void IMutableMatrix<Numeric<T, C>>.SetElementAt(int row, int column, Numeric<T, C> value)
        {
            CheckMatrixRange(row, column);
            this[row] = value;
        }
    }
}
