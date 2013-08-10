using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Diagnostics.Contracts;

namespace whiteMath.General
{
    /// <summary>
    /// Wrapper class which represents a segment of an IList&lt;<typeparamref name="T"/>&gt;.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [ContractVerification(true)]
    public class ListSegment<T>: IList<T>
    {
        int offset;
        int length;
        private IList<T> list;

        public ListSegment(IList<T> list, int offset, int length)
        {
            Contract.Requires<ArgumentNullException>(list != null, "list");
            Contract.Requires<ArgumentOutOfRangeException>(length >= 0, "The length must be non-negative");
            Contract.Requires<IndexOutOfRangeException>(offset >= 0 && offset < list.Count, "The offset index is out of the list bounds.");
            Contract.Requires<IndexOutOfRangeException>(offset + length <= list.Count, "The length specified runs out of the list bound.");

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
    /// <see cref="ListSegment&lt;T&gt;"/>
    [ContractVerification(true)]
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
            Contract.Requires<ArgumentNullException>(list != null, "list");

            var cr = Contract.Result<List<ListSegment<T>>>();

            Contract.Ensures(cr != null);
            Contract.Ensures(cr.Sum(x => x.Count) == list.Count);
            Contract.Ensures(
                options == SegmentationOptions.BiggerLastSegment && cr.Last().Count >= segmentLength ||
                options == SegmentationOptions.SmallerLastSegment && cr.Last().Count <= segmentLength);
            
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
    }
}
