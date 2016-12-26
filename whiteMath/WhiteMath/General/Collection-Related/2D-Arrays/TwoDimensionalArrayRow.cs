using System;
using System.Collections.Generic;
using System.Collections;

using whiteStructs.Conditions;

namespace WhiteMath.General
{
    /// <summary>
    /// Represents a row of a two-dimensional array.
    /// </summary>
    public class TwoDimensionalArrayRow<T>: IList<T>
    {
        #region Not Implemented Interface Members

        public void Add(T element)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T element)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T element)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Interface Members

        /// <summary>
        /// Gets the flag indicating whether the row is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the flag indicating whether the current row
        /// contains a particular element.
        /// </summary>
        /// <param name="element">The element to be found in the row.</param>
        /// <returns>
        /// True if there is an element in the row equal 
        /// to the element passed, false otherwise.
        /// </returns>
        public bool Contains(T element)
        {            
            foreach (T item in this)
            {
                if (item.Equals(element))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the entire row to a compatible one-dimensional array, starting 
        /// at the specified index of the target array.
        /// </summary>
        /// <param name="array">A compatible one-dimensional array.</param>
        /// <param name="startIndex">The starting index in the target array.</param>
        public void CopyTo(T[] array, int startIndex)
        {
            int sourceIndex = 0;

            foreach (T element in this)
            {
                array[startIndex + sourceIndex] = element;
                ++sourceIndex;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new ClassicEnumerator<T>(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of a particular element in the 
        /// collection, or a negative value if the element is absent.
        /// </summary>
        /// <param name="element">The element to search for.</param>
        /// <returns>
        /// A zero-based index of the element in the collection, 
        /// or a negative value if the element is not found.
        /// </returns>
        public int IndexOf(T element)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                if (this[i].Equals(element))
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        /// <summary>
        /// Gets the original two-dimensional array
        /// upon which the current object was created. 
        /// </summary>
        public T[,] Parent { get; private set; }

        /// <summary>
        /// Gets the index of the row in the parent
        /// two-dimensional which the current object
        /// wraps around. 
        /// </summary>
        public int ParentRow { get; private set; }

        /// <summary>
        /// Returns the number of elements in the row.
        /// </summary>
        public int Count
        {
            get
            {
                return Parent.GetLength(1);
            }
        }

        /// <summary>
        /// Gets or sets the value at the specified index of the collection.
        /// </summary>
        /// <param name="index">The index of the element to be got or set.</param>
        /// <returns>The value at the specified index of the collection.</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return this.Parent[ParentRow, index];
            }
            set
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                this.Parent[ParentRow, index] = value;
            }
        }

        /// <summary>
        /// Constructs the wrapper collection using a parent
        /// two-dimensional array object and the row index 
        /// specified.
        /// </summary>
        /// <param name="matrix">
        /// The source two-dimensional array object
        /// to be wrapped.
        /// </param>
        /// <param name="rowIndex">
        /// The row index of the source object to be fixed.
        /// </param>
        public TwoDimensionalArrayRow(T[,] matrix, int rowIndex)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));
			Condition
				.Validate(rowIndex >= 0 && rowIndex < matrix.GetLength(0))
				.OrArgumentOutOfRangeException("The row index is out of range.");

            this.Parent = matrix;
            this.ParentRow = rowIndex;
        }
    }
}
