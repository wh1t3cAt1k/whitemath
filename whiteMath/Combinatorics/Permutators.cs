using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.General;

namespace whiteMath.Combinatorics
{
    /// <summary>
    /// Represents a class that provides the capability
    /// to generate all permutations of elements of
    /// the specified list.
    /// </summary>
    /// <typeparam name="T">The types of elements in the list.</typeparam>
    public abstract class Permutator<T>: IList<T>
    {
        protected int[] permutation;
        protected IList<T> list;

        public Permutator(IList<T> list)
        {
            this.list = list;

            this.permutation = new int[list.Count];
            this.permutation.FillByAssign(delegate(int i) { return i; });
        }

        /// <summary>
        /// Returns the value at the specified index according
        /// to the current permutation.
        /// </summary>
        /// <exception cref="NotSupportedException">The setter will always throw as <c>Permutators</c> are read-only collections.</exception>
        /// <param name="index">The index of desired element.</param>
        /// <returns>The value at the specified index according to the current permutation.</returns>
        public T this[int index] 
        { 
            get { return list[permutation[index]]; }
            set { throw ex; }
        }

        /// <summary>
        /// Creates the next permutation.
        /// </summary>
        /// <returns>True if the next permutation was available to create, false in case when all permutations have been generated.</returns>
        public abstract bool CreateNextPermutation();

        /// <summary>
        /// Resets the <c>Permutator</c>
        /// so the cycle of generations may be done once again.
        /// </summary>
        public virtual void Reset()
        {
            this.permutation.FillByAssign(currentIndex => currentIndex);
        }

        // ----------------------------------------

        /// <summary>
        /// Returns the sign of the permutation.
        /// </summary>
        public int Sign
        {
            get {

                int i;
                
                bool[] visits = new bool[permutation.Length];
                
                List<List<int>> cycles = new List<List<int>>();

                while(true)
                {
                    // Find an unvisited permutation element.
                    //
                    i = Array.IndexOf(visits, false);

                    // If there are none, exit.
                    //
                    if (i < 0) break;

                    // If there is, there is a new cycle.
                    // 
                    cycles.Add(new List<int>());
                    cycles.Last().Add(i);

                    visits[i] = true;

                    while(true)
                    {
                        int x = permutation[cycles.Last().Last()];

                        if (!visits[x])
                        {
                            visits[x] = true;
                            cycles.Last().Add(x);
                        }
                        else
                            break;
                    }
                }

                // All cycles discovered.
                // -
                int k = 0;

                foreach (List<int> cycle in cycles)
                    k += cycle.Count - 1;

                return k % 2;
           }
        }

        // ----------- ENUMERATORS ----------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ClassicEnumerator<T>(this);
        }

        // ----------- FAIL -----------------------

        private static readonly NotSupportedException ex = new NotSupportedException(Combinatorics.ErrorMessages.PermutatorsDoNotSupportAdditionRemoval);

        void IList<T>.Insert(int index, T key) { throw ex; }
        void IList<T>.RemoveAt(int index) { throw ex; }

        void ICollection<T>.Add(T key) { throw ex; }
        void ICollection<T>.Clear() { throw ex; }
        bool ICollection<T>.Remove(T key) { throw ex; }

        public bool IsReadOnly { get { return true; } }

        public int Count { get { return list.Count; } }

        public bool Contains(T key) 
        {
            return Enumerable.Contains(this, key);
        }

        public void CopyTo(T[] arr, int startIndex)
        {
            for (int i = 0; i < this.Count; i++)
                arr[startIndex + i] = this[i];
        }

        public int IndexOf(T key)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].Equals(key))
                    return i;

            return -1;
        }
    }

    // -----------------------------------

    public class LexicographicPermutator<T> : Permutator<T>
    {
        public LexicographicPermutator(IList<T> list)
            : base(list)
        { }

        public override bool CreateNextPermutation()
        {
            if (permutation.Length < 2)
                return false;

            // ----------------

            int k = permutation.Length - 2;
            int l = permutation.Length - 1;

            for (; k >= 0; k--)
                if (permutation[k] < permutation[k + 1])
                    break;

            if (k < 0)
                return false;

            for (; l > k; l--)
                if (permutation[k] < permutation[l])
                    break;

            permutation.Swap(k, l);

            // ----------

            int maxInd = ((k + permutation.Length) % 2 == 0) ? ((k + permutation.Length) / 2) : ((k + permutation.Length) / 2 + 1);

            for (int i = k + 1; i < maxInd; i++)
                permutation.Swap(i, permutation.Length - i + k);

            return true;
        }
    }

    public class InductivePermutator<T> : Permutator<T>
    {
        public int curPos;
        public int[] trace;

        public InductivePermutator(IList<T> list)
            : base(list)
        {
            trace = new int[list.Count];
            this.Reset();
        }

        public override void Reset()
        {
            base.Reset();

            curPos = list.Count - 1;

            trace.FillByAssign(delegate(int i) { return i; });
            
            if(trace.Length>0)
                trace[0] = -1;
        }

        public override bool CreateNextPermutation()
        {
            if (curPos > trace.Length)
                return false;

            NextPermutation(curPos);

            if (curPos > trace.Length)
                return false;

            return true;
        }

        private void NextPermutation(int pos)
        {
            if (trace[pos] == pos)
            {
                bool flag = true;

                for(int i=pos-1; i>0; i--)
                    if (trace[i] != -1)
                    {
                        flag = false;
                        break;
                    }

                if (!flag)
                {
                    for (int i = 0; i < pos; i++)
                        if (trace[i] != -1)
                            NextPermutation(i);
                }
                
                trace.Swap(pos, pos - 1);
                permutation.Swap(pos, pos - 1);
            }
            else // был обмен
            {
                int i;

                for (i = pos - 1; i >= 0; i--)
                    if (trace[i] == pos)
                        break;

                if (i == 0)
                {
                    trace.Swap(i, pos);
                    permutation.Swap(i, pos);

                    for (int j = 0; j < pos; j++)
                        trace[j] = j;

                    if (pos == curPos)
                        curPos++;
                }
                else
                {
                    trace.Swap(i, i - 1);
                    permutation.Swap(i, i - 1);
                }
            }
            
        }
    }
}
