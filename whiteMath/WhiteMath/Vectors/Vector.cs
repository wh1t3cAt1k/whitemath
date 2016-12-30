using System;

using WhiteMath.Calculators;
using WhiteMath.Matrices;

namespace WhiteMath.Vectors
{
    /// <summary>
    /// The vector of numeric objects.
    /// TODO: write the documentation.
    /// </summary>
    /// <typeparam name="T">The type of numbers stored by the vector.</typeparam>
    public class Vector<T, C> : ICloneable, IMutableMatrix<T>, IMutableMatrix<Numeric<T, C>> where C : ICalc<T>, new()
    {
        private static C calc = new C();
        private T[] array;

        // --------------------------------
        // ------ Vector properties--------
        // --------------------------------

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public int Length { get { return array.Length; } }

        // ------------------------------

        /// <summary>
        /// The indexer of the vector.
        /// </summary>
        public Numeric<T, C> this[int ind]
        {
            get { return array[ind]; }
            set { array[ind] = value; }
        }


        // --------------------------------
        // ------ Constructors ------------
        // --------------------------------

        /// <summary>
        /// The copy constructor.
        /// Creates an independent clone of the vector object passed.
        /// </summary>
        /// <param name="copy"></param>
        public Vector(Vector<T, C> copy)
        {
            array = new T[copy.Length];

            for (int i = 0; i < copy.Length; i++)
                this[i] = calc.GetCopy(copy[i]);

            return;
        }

        /// <summary>
        /// Creates a new vector filled up with zero numeric values.
        /// </summary>
        /// <param name="dimension"></param>
        public Vector(int dimension)
            : this(dimension, calc.Zero)
        { }

        /// <summary>
        /// Creates a new vector with elements all initialized equal to the value passed.
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="value"></param>
        public Vector(int dimension, T value)
        {
            this.array = new T[dimension];

            for (int i = 0; i < dimension; i++)
                array[i] = calc.GetCopy(value);
        }

        /// <summary>
        /// Private constructor used by helper methods.
        /// </summary>
        /// <param name="array"></param>
        private Vector(T[] array)
        {
            this.array = array;
            return;
        }

        // -------------------------------- Depending on the logic!

        /// <summary>
        /// Creates the vector as a wrapper for the array passed.
        /// (!) All changes to the vector will be reflected on the array, and vice versa.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Vector<T, C> CreateWrapper(T[] array)
        {
            return new Vector<T, C>(array);
        }

        /// <summary>
        /// Creates the vector using the shallow copy for the array passed.
        /// If the shallow array copy is logically deep, no changes to the vector
        /// will be reflected on the array, and vice versa.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Vector<T, C> CreateIndependent(T[] array)
        {
            return new Vector<T, C>(CreateWrapper(array));
        }

        // -------------------------------
        // --------- PROPERTIES ----------
        // -------------------------------

        /// <summary>
        /// Unwraps the current vector to present it as a simple T array.
        /// All changes to the array object returned will be reflected on the vector,
        /// and vice versa.
        /// </summary>
        /// <returns></returns>
        public T[] UnwrappedToSimpleArray
        {
            get { return this.array; }
        }

        /// <summary>
        /// Unwraps the current vector to present it as a Numeric array.
        /// </summary>
        public Numeric<T, C>[] AsNumericArray()
        {
            return this.array.ConvertToNumericArray<T, C>();
        }

        // --------------------------------
        // ------ Conversion methods ------
        // --------------------------------

        public static implicit operator Vector<T, C>(T[] array)
        {
            return Vector<T, C>.CreateWrapper(array);
        }

        public static implicit operator T[](Vector<T, C> vector)
        {
            return vector.UnwrappedToSimpleArray;
        }

        // --------------------------------
        // ------ ToString methods --------
        // --------------------------------

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

            foreach (T obj in array)
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

        // --------------------------------
        // ------ Cloneable realization----
        // --------------------------------

        public object Clone()
        {
            return new Vector<T, C>(this);
        }

        // --------------------------------
        // ------ IMatrix realization------
        // --------------------------------

        int IMatrix.ColumnCount { get { return 1; } }
        int IMatrix.RowCount { get { return Length; } }

        void ___checkMatrixRange(int row, int column)
        {
            if (column > 0 || row >= Length)
                throw new ArgumentOutOfRangeException("At least one of the indexes was out of range.");
        }

        object IMatrix.GetElementAt(int row, int column)
        {
            ___checkMatrixRange(row, column);
            return this[row];
        }

        T IMatrix<T>.GetElementAt(int row, int column)
        {
            ___checkMatrixRange(row, column);
            return this[row];
        }

        Numeric<T, C> IMatrix<Numeric<T, C>>.GetElementAt(int row, int column)
        {
            ___checkMatrixRange(row, column);
            return this[row];
        }

        void IMutableMatrix.SetElementAt(int row, int column, object value)
        {
            ___checkMatrixRange(row, column);
            this[row] = (T)value;
        }

        void IMutableMatrix<T>.SetElementAt(int row, int column, T value)
        {
            ___checkMatrixRange(row, column);
            this[row] = value;
        }

        void IMutableMatrix<Numeric<T, C>>.SetElementAt(int row, int column, Numeric<T, C> value)
        {
            ___checkMatrixRange(row, column);
            this[row] = value;
        }
    }
}
