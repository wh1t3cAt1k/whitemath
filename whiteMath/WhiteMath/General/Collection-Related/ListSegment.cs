using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using whiteStructs.Conditions;

namespace WhiteMath.General
{
    /// <summary>
    /// Wrapper class which represents a segment of an IList&lt;<typeparamref name="T"/>&gt;.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class ListSegment<T>: IList<T>
    {
        int offset;
        int length;
        private IList<T> list;

        public ListSegment(IList<T> list, int offset, int length)
        {
			Condition.ValidateNotNull(list, nameof(list));
			Condition.ValidateNonNegative(length, "The length must be non-negative.");
			Condition
				.Validate(offset >= 0 && offset < list.Count)
				.OrIndexOutOfRangeException("The offset index is out of the list boundaries.");
			Condition
				.Validate(offset + length <= list.Count)
				.OrIndexOutOfRangeException("The length specified runs out of the list boundaries.");

            this.list = list;
            this.offset = offset;
            this.length = length;
        }

        public T this[int ind]
        {
            get { return list[transformIndex(ind)]; }
            set { list[transformIndex(ind)] = value; }
        }

        // ------------------------------------------

        public int Offset { get { return offset; } }

        //-------ENUMERATORS-------------------------

        public IEnumerator<T> GetEnumerator()
        {
            return new ClassicEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //-------------------------------------------
        //-------INTERFACE MEMBER IMPLEMENTATION-----
        //-------------------------------------------

        public bool Contains(T key)
        {
            for (int i = 0; i < length; i++)
                if (this[i].Equals(key)) return true;

            return false;
        }

        /// <summary>
        /// Returns the element count in the current list segment
        /// </summary>
        public int Count
        {
            get { return length; }
        }

        /// <summary>
        /// Returns true if current list segment is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return list.IsReadOnly; }
        }

        //-------------------------- ELEMENTS MANIPULATION

        /// <summary>
        /// Adds an element to the end of the segment.
        /// Is NOT quick. The length of the segment is automatically increased.
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            list.Insert(offset + length, value);
            length++;
        }

        /// <summary>
        /// Removes the first occurence of an object from the list segment.
        /// The length of the segment is automatically decreased if successful.
        /// </summary>
        /// <param name="key"></param>
        public bool Remove(T key)
        {
            int ind = IndexOf(key);
            if (ind >= 0)
            {
                RemoveAt(ind);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Copies the elements from the list segment to an array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        public void CopyTo(T[] array, int offset)
        {
            for (int i = 0; i < this.Count; i++)
                array[offset + i] = this[i];
        }

        /// <summary>
        /// Inserts the item into the segment.
        /// The length of the segment is automatically incremented.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, T value)
        {
            int listInd = transformIndex(index);

            list.Insert(listInd, value);
            length++;
        }

        /// <summary>
        /// Removes an item from the segment.
        /// The length of the segment is automatically decreased.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            int listInd = transformIndex(index);

            list.RemoveAt(listInd);
            length--;
        }

        /// <summary>
        /// Clears the current list segment.
        /// The parent list is also modified.
        /// </summary>
        public void Clear()
        {
            for(int i=0; i<length; i++, list.RemoveAt(offset)) 
                ;
        }

        //-----------------------------------------------------------

        /// <summary>
        /// Returns the list segment index of a key specified by user.
        /// If the key is not found within the segment, a negative value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IndexOf(T key)
        {
            for (int i = 0; i < length; i++)
                if (this[i].Equals(key)) return i;

            return -1;
        }

        // ------------------------------------------- SERVICE

        /// <summary>
        /// Transfors the inner index into the parent list index.
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        private int transformIndex(int ind)
        {
            if (ind < 0 || ind >= length)
                throw new IndexOutOfRangeException("The index specified is out of the segment bounds.");

            return offset + ind;
        }

        // -------------------------------
        // ---------- Override -----------

        public override string ToString()
        {
            return string.Format("{0}, element range [{1}..{2}]", this.GetType().Name, this.offset, this.offset + this.length - 1);
        }

        public override bool Equals(object obj)
        {
            ListSegment<T> ls = obj as ListSegment<T>;

            if(ls == null)
                return false;

 	        return this.list.Equals(ls.list) && this.offset == ls.offset && this.length == ls.length;
        }

        public override int GetHashCode()
        {
            return this.list.GetHashCode() + this.offset.GetHashCode() + this.length.GetHashCode();
        }
    }

    /// <summary>
    /// This class provides convenience extension methods for <c>IList&lt;T&gt;</c> objects 
    /// related to creating <c>ListSegment</c>'s from them.
    /// </summary>
	/// <see cref="ListSegment{T}"/>
    public static class ListSegmentationExtensions
    {
        /// <summary>
        /// This enum contains options to specify when covering an <c>IList&lt;T&gt;</c> into a sequence of 
        /// <c>ListSegment</c>'s and the length of the source list does not contain a whole number of desired lengths. 
        /// </summary>
	    public enum SegmentationOptions
	    {
		    /// <summary>
		    /// When using this option, in case of presence of the 'tail' length,
		    /// the last segment will have length bigger than the desired.
		    /// The overall number of segments produced will be floor(list.length / desiredLength).
		    /// </summary>
		    BiggerLastSegment,

		    /// <summary>
		    /// When using this option, in case of presence of the 'tail' length,
		    /// the last segment will have length smaller than the desired.
		    /// The overall number of intervals produced will be ceil(list.length / desiredLength).
		    /// </summary>
		    SmallerLastSegment
	    }

        /// <summary>
        /// Covers a <c>List&lt;T&gt;</c> with a sequence of non-intersecting <c>ListSegment</c>'s.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The calling <c>List&lt;T&gt;</c> object.</param>
        /// <param name="segmentLength">
        /// The desired length of segments. Depending on the <paramref name="options"/>,
        /// if the length of the <paramref name="list"/> does not contain a whole number
        /// of desired lengths, the last segment may be smaller or bigger.
        /// </param>
        /// <param name="options">
        /// Options which matter when the length of the <paramref name="list"/>
        /// does not contain a whole number of desired length. This parameter
        /// will affect the length of the last segment and the total number of segments in this case.
        /// </param>
        /// <returns>A list of non-intersecting segments which cover the <paramref name="list"/>.</returns>
        public static List<ListSegment<T>> CoverWithSegments<T>(this IList<T> list, int segmentLength, SegmentationOptions options)
        {
			Condition.ValidateNotNull(list, nameof(list));

			/*
            Contract.Ensures(cr != null);
            Contract.Ensures(cr.Sum(x => x.Count) == list.Count);
            Contract.Ensures(
                options == SegmentationOptions.BiggerLastSegment && cr.Last().Count >= segmentLength ||
                options == SegmentationOptions.SmallerLastSegment && cr.Last().Count <= segmentLength);
            */

            List<ListSegment<T>> result = new List<ListSegment<T>>(list.Count / segmentLength + 1);

            int remainingCount = list.Count;
            int offset = 0;

            while (remainingCount >= 2 * segmentLength)
            {
                result.Add(new ListSegment<T>(list, offset, segmentLength));

                offset += segmentLength;
                remainingCount -= segmentLength;
            }

            if (remainingCount > 0)
            {
                if (options == SegmentationOptions.BiggerLastSegment || remainingCount == segmentLength)
                    result.Add(new ListSegment<T>(list, offset, remainingCount));

                else if (options == SegmentationOptions.SmallerLastSegment)
                {
                    result.Add(new ListSegment<T>(list, offset, segmentLength));

                    offset += segmentLength;
                    remainingCount -= segmentLength;

                    result.Add(new ListSegment<T>(list, offset, remainingCount));
                }

                else
                    throw new EnumFattenedException("Enum fattened, and method stopped working correctly dendranul.");
            }

            return result;
        }

        /// <summary>
        /// Creates a wrapper list segment containing elements from the
        /// specified offset index in the parent list and to the end of the source list.
        /// 
        /// The resulting list segment is reindexed from zero. 
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">A source list.</param>
        /// <param name="offset">A zero-based offset inclusive index, starting with which the resulting list segment will be produced.</param>
        /// <returns>
        /// A wrapper list segment containing elements from the
        /// specified offset index (inclusive) in the parent list and to the end of the source list.
        /// </returns>
        public static ListSegment<T> ListSegmentFrom<T>(this IList<T> list, int offset)
        {
			Condition.ValidateNotNull(list, nameof(list));
			Condition
				.Validate(offset >= 0 && offset < list.Count)
				.OrIndexOutOfRangeException("The offset is outside of the list boundaries.");

			// TODO: fishy edge conditions

            return new ListSegment<T>(list, offset, list.Count - offset);
        }

        /// <summary>
        /// Creates a wrapper list segment containing elements from the
        /// beginning of a source list and to the specified index (inclusively).
        /// 
        /// The resulting list segment is reindexed from zero.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">A source list.</param>
        /// <param name="toInclusiveIndex">A zero-based inclusive index, which will produce the last element for the wrapper segment.</param>
        /// <returns>
        /// A wrapper list segment containing elements from the
        /// beginning of <paramref name="list"/> and to the specified <paramref name="toInclusiveIndex"/> (inclusively).
        /// </returns>
        public static ListSegment<T> ListSegmentTo<T>(this IList<T> list, int toInclusiveIndex)
        {
			Condition.ValidateNotNull(list, nameof(list));
			Condition
				.Validate(toInclusiveIndex >= 0 && toInclusiveIndex < list.Count - 1)
				.OrIndexOutOfRangeException("The index is outside of the list boundaries.");

			// TODO: fishy edge conditions

            return new ListSegment<T>(list, 0, toInclusiveIndex + 1);
        }

        /// <summary>
        /// Creates a wrapper list segment containing elements of a source list
        /// located between a pair of specified inclusive indices.
        /// 
        /// The resulting list segment is reindexed from zero.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">A source list.</param>
        /// <param name="fromInclusiveIndex">A zero-based inclusive index, which will produce the first element for the wrapper segment.</param>
        /// <param name="toInclusiveIndex">A zero-based inclusive index, which will produce the last element for the wrapper segment.</param>
        /// <returns>
        /// A wrapper list segment containing elements of a source list
        /// starting from <paramref name="fromInclusiveIndex"/> (inclusively) and to
        /// <paramref name="toInclusiveIndex"/> (inclusively).
        /// </returns>
        public static ListSegment<T> ListSegmentFromTo<T>(this IList<T> list, int fromInclusiveIndex, int toInclusiveIndex)
        {
			Condition.ValidateNotNull(list, nameof(list));
			Condition
				.Validate(fromInclusiveIndex >= 0 && fromInclusiveIndex < list.Count - 1)
				.OrIndexOutOfRangeException("The index is outside of the list boundaries.");
			Condition
				.Validate(fromInclusiveIndex < toInclusiveIndex)
				.OrArgumentException("The starting index should not exceed the ending index.");

			// TODO: fishy edge conditions

            return new ListSegment<T>(list, fromInclusiveIndex, toInclusiveIndex - fromInclusiveIndex + 1);
        }

        /// <summary>
        /// In a list containing two or more elements, excludes the boundaries
        /// i.e. the leftmost and the rightmost element and
        /// returns a wrapper list containing all but these elements,
        /// reindexed from zero. The parent list object remains intact. 
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">A source list object, containing two or more elements.</param>
        /// <returns>
        /// A zero-indexed wrapper list containing all but the leftmost
        /// and the rightmost elements of the parents.
        /// </returns>
        public static ListSegment<T> ExcludeBoundaries<T>(this IList<T> list)
        {
			Condition.ValidateNotNull(list, nameof(list));
			Condition
				.Validate(list.Count > 1)
			    .OrArgumentException("The list should contain at least two elements.");

            return new ListSegment<T>(list, 1, list.Count - 2);
        }
    }
}
