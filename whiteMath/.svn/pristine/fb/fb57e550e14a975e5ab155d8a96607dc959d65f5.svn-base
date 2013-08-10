using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace whiteMath.General
{
    public sealed class LTDefaultList<T>: IList<T>
    {
        T defaultValue;
        IList<T> parent;

        public int LeadingAmount { get; internal set; }
        public int TrailingAmount { get; internal set; }

        // ---------- count ---------------

        public int Count { get { return parent.Count + LeadingAmount + TrailingAmount; } }

        // --------------------------------
        // ---------- indexer -------------
        // --------------------------------

        public T this[int i]
        {
            get
            {
                if (i >= LeadingAmount && i < LeadingAmount + parent.Count)
                    return parent[i - LeadingAmount];
                else if ((i >= 0 && i < LeadingAmount + parent.Count + TrailingAmount))
                    return defaultValue;
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                if (this.IsReadOnly)
                    throw new NotSupportedException("The list is read-only.");

                if (i >= LeadingAmount && i < LeadingAmount + parent.Count)
                    parent[i - LeadingAmount] = value;
                else if (i >= 0 && i < LeadingAmount)
                {
                    if (value.Equals(defaultValue))
                        return;

                    T[] array = new T[LeadingAmount - i];

                    array.FillByAssign(defaultValue);
                    array[0] = value;

                    if (parent is List<T>)
                        (parent as List<T>).InsertRange(0, array);
                    else
                        for (int j = array.Length - 1; j >= 0; j--)
                            parent.Insert(0, array[j]);

                    LeadingAmount -= array.Length;
                }
                else if (i >= LeadingAmount + parent.Count && i < LeadingAmount + parent.Count + TrailingAmount)
                {
                    if (value.Equals(defaultValue))
                        return;

                    T[] array = new T[i - LeadingAmount - parent.Count + 1];

                    array.FillByAssign(defaultValue);
                    array[array.Length - 1] = value;

                    if (parent is List<T>)
                        (parent as List<T>).AddRange(array);
                    else
                        foreach (T element in array)
                            parent.Add(element);

                    TrailingAmount -= array.Length;
                }
                else
                    throw new IndexOutOfRangeException();
            }
        }

        // --------------------------------
        // ---------- enumerator ----------
        // --------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ClassicEnumerator<T>(this);
        }

        // --------------------------------
        // -----IList implementation ------
        // --------------------------------

        public bool IsReadOnly { get { return parent.IsReadOnly; } }

        public void Add(T item)
        {
            if (this.IsReadOnly)
                throw new NotSupportedException("The list is read-only.");

            if (!item.Equals(defaultValue))
            {
                T[] array = new T[TrailingAmount + 1];

                array.FillByAssign(defaultValue);
                array[array.Length - 1] = item;

                if (parent is List<T>)
                    (parent as List<T>).AddRange(array);
                else
                    foreach (T element in array)
                        parent.Add(element);

                TrailingAmount = 0;
            }
            else
                TrailingAmount++;
        }

        public void Insert(int i, T item)
        {
            if(this.IsReadOnly)
                throw new NotSupportedException("The list is read-only.");

            if (i >= LeadingAmount && i < LeadingAmount + parent.Count)
                parent.Insert(i - LeadingAmount, item);

            else if (i >= 0 && i < LeadingAmount)
            {
                if (item.Equals(defaultValue))
                    LeadingAmount++;
                else
                {
                    T[] array = new T[LeadingAmount - i + 1];

                    array.FillByAssign(defaultValue);
                    array[0] = item;

                    if (parent is List<T>)
                        (parent as List<T>).InsertRange(0, array);
                    else
                        for (int j = array.Length - 1; j >= 0; j--)
                            parent.Insert(0, array[j]);

                    LeadingAmount -= array.Length - 1;
                }
            }
            else if (i >= LeadingAmount + parent.Count && i < LeadingAmount + parent.Count + TrailingAmount)
            {
                if (item.Equals(defaultValue))
                    TrailingAmount++;
                else
                {
                    T[] array = new T[i - LeadingAmount - parent.Count + 1];

                    array.FillByAssign(defaultValue);
                    array[array.Length - 1] = item;

                    if (parent is List<T>)
                        (parent as List<T>).AddRange(array);
                    else
                        foreach (T element in array)
                            parent.Add(element);

                    TrailingAmount -= array.Length - 1;
                }
            }
        }

        public bool Contains(T key)
        {
            foreach (T element in this)
                if (key.Equals(element))
                    return true;

            return false;
        }

        public void Clear()
        {
            if (this.IsReadOnly)
                throw new NotSupportedException("The list is read-only.");

            parent.Clear();

            LeadingAmount = 0;
            TrailingAmount = 0;
        }

        public void CopyTo(T[] array, int startIndex)
        {
            for (int i = 0; i < this.Count; i++)
                array[i + startIndex] = this[i];
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].Equals(item))
                    return i;

            return -1;
        }

        public bool Remove(T item)
        {
            if (this.IsReadOnly)
                throw new NotSupportedException("The list is read-only.");

            for (int i = 0; i<this.Count; i++)
                if (this[i].Equals(item))
                {
                    this.RemoveAt(i);
                    return true;
                }

            return false;
        }

        public void RemoveAt(int i)
        {
            if (this.IsReadOnly)
                throw new NotSupportedException("The list is read-only.");

            if (i >= LeadingAmount && i < LeadingAmount + parent.Count)
                parent.RemoveAt(i - LeadingAmount);
            else if (i >= 0 && i < LeadingAmount)
                LeadingAmount--;
            else if (i >= LeadingAmount + parent.Count && i < LeadingAmount + parent.Count + TrailingAmount)
                TrailingAmount--;
            else
                throw new IndexOutOfRangeException();
        }

        // --------------------------------
        // ---------- ctors ---------------
        // --------------------------------

        public LTDefaultList(T defaultValue)
            : this(defaultValue, new List<T>(), 0, 0)
        { }

        public LTDefaultList(T defaultValue, int capacity)
            : this(defaultValue, new List<T>(capacity), 0, 0)
        { }

        public LTDefaultList(T defaultValue, int leadingDefaultsAmount, int trailingDefaultsAmount)
            : this(defaultValue, new List<T>(), leadingDefaultsAmount, trailingDefaultsAmount)
        { }

        public LTDefaultList(T defaultValue, int capacity, int leadingDefaultsAmount, int trailingDefaultsAmount)
            : this(defaultValue, new List<T>(capacity), leadingDefaultsAmount, trailingDefaultsAmount)
        { }

        public LTDefaultList(T defaultValue, IList<T> parent)
            : this(defaultValue, parent, 0, 0)
        { }

        public LTDefaultList(T defaultValue, IList<T> parent, int leadingDefaultsAmount, int trailingDefaultsAmount)
        {
            this.defaultValue = defaultValue;
            this.parent = parent;

            this.LeadingAmount = leadingDefaultsAmount;
            this.TrailingAmount = trailingDefaultsAmount;
        }
    }
}
