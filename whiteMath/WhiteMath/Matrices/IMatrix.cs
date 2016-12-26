using System;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// The interface supporting the minimal functionality
    /// to get an item from specified point, and to know
    /// both row and column count of the table.
    /// </summary>
    public interface IMatrix
    {
        /// <summary>
        /// Returns the total amount of rows in the matrix.
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Returns the total amount of columns in the matrix.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Returns the element at specified matrix position.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If no such row or column exist in the 
        /// matrix, an ArgumentOutOfRange exception may be thrown.
        /// </exception>
        /// <param name="row">A zero-based index of a matrix row.</param>
        /// <param name="column">A zero-based index of a matrix column.</param>
        /// <returns>The element at specified matrix position.</returns>
        object GetElementValue(int row, int column);
    }

    /// <summary>
    /// The interface of mutable matrices allowing
    /// to set elements at specified indices.
    /// </summary>
    public interface IMutableMatrix: IMatrix
    {
        /// <summary>
        /// Sets the element at specified matrix position.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If no such row or column exist in the 
        /// matrix, an ArgumentOutOfRange exception may be thrown.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// If the matrix is strictly typed, the cast operation on the value
        /// passed may fail, thus resulting in an InvalidCastException.
        /// </exception>
        /// <param name="row">A zero-based index of a matrix row.</param>
        /// <param name="column">A zero-based index of a matrix column.</param>
        /// <param name="value"></param>
        void SetElementValue(int row, int column, object value);
    }

    /// <summary>
    /// The generic interface supporting the minimal functionality
    /// to get an item from specified point, and to know
    /// both row and column count of the table.
    /// </summary>
    public interface IMatrix<T>: IMatrix
    {
        /// <summary>
        /// Returns the element at specified matrix position.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If no such row or column exist in the 
        /// matrix, an ArgumentOutOfRange exception may be thrown.
        /// </exception>
        /// <param name="row">A zero-based index of a matrix row.</param>
        /// <param name="column">A zero-based index of a matrix column.</param>
        /// <returns>The element at specified matrix position.</returns>
        new T GetElementValue(int row, int column);
    }

    /// <summary>
    /// The generic interface of mutable matrices allowing
    /// to set elements at specified indices.
    /// </summary>
    public interface IMutableMatrix<T> : IMutableMatrix, IMatrix<T>
    {       
        /// <summary>
        /// Sets the element at specified matrix position.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If no such row or column exist in the 
        /// matrix, an ArgumentOutOfRange exception may be thrown.
        /// </exception>
        /// <param name="row">A zero-based index of a matrix row.</param>
        /// <param name="column">A zero-based index of a matrix column.</param>
        /// <param name="value"></param>
        void SetElementValue(int row, int column, T value);
    }
}
