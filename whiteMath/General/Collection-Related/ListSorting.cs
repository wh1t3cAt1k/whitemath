using System.Collections.Generic;
using System.Linq;

using whiteStructs.Collections;

namespace whiteMath.General
{
    public static class ListSorting
    {
        /// <summary>
        /// Checks whether the sequence of objects is ascending sorted, uses the comparer passed
        /// for sequence elements, or, if a null value is specified, the standard system
        /// comparer (if exists).
        /// 
        /// Takes O(n) operations to check if a sequence implements IList(T).
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The calling IEnumerable(T) object.</param>
        /// <param name="comparer">Optional comparer for sequence elements. If null value is specified, the standard system comparer would be used (if exists).</param>
        /// <returns>True if the sequence is ascending sorted according to the comparer specified, false otherwise.</returns>
        public static bool IsSorted<T>(this IEnumerable<T> sequence, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;

			if (sequence.IsEmpty() || sequence.IsSingleton())
			{
				return true;
			}

            IEnumerator<T> previous = sequence.GetEnumerator();
            IEnumerator<T> next = sequence.GetEnumerator();

            next.MoveNext();

            while (next.MoveNext() && previous.MoveNext())
                if (comparer.Compare(next.Current, previous.Current) < 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Returns the Pratt Sequence for Shell Sort algorithm.
        /// </summary>
        private static IList<int> getPrattSequence(int n)
        {
            int half = n / 2;

            SortedSet<int> set = new SortedSet<int>();
            set.Add(1);

            int i = 1;

            while (true)
            {
                i *= 3;

                if (i > half)
                    break;

                for (int j = 1; ; j *= 2)
                {
                    int mul = i * j;

                    if (mul > half)
                        break;
                    else
                        set.Add(mul);
                }
            }

            return set.ToArray();
        }

        /// <summary>
        /// Invokes a quick shellsort algorithm on a list.
        /// Has O(n^2) asympthotics, but usually works much faster than insertion sort.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The invoking list object.</param>
        /// <param name="comparer">The comparer for list elements.</param>
        /// <param name="shellSequence">The Shell sequence used to determine the sorting step. Its first element should always be '1', and the sequence should grow monotonously. If 'null' parameter is passed, a certain quick sequence would be used automatically.</param>
        public static void SortShell<T>(this IList<T> list, IComparer<T> comparer = null, IList<int> shellSequence = null)
        {
            T key;
            int i, j;

            int n = list.Count;

            if (comparer == null)
                comparer = Comparer<T>.Default;

            if (shellSequence == null)
                shellSequence = getPrattSequence(list.Count);

            // доматываем указатель последовательности до применимых границ

            int currentSequenceIndex = shellSequence.Count - 1;
            
            // нет смысла использовать при шаге shellSequence[currentSequenceIndex] >= n. Последовательность в 0 или 1 элемент уже отсортирована :)

            while (n < (shellSequence[currentSequenceIndex] + 1) && currentSequenceIndex > 0)
                currentSequenceIndex--;

            // конец проматывания

            for (int k = shellSequence[currentSequenceIndex]; currentSequenceIndex >= 0; currentSequenceIndex--)
            {
                k = shellSequence[currentSequenceIndex];

                for (i = k; i < n; i += k)
                {
                    key = list[i];
                    j = i - k;

                    while (j >= 0 && comparer.Compare(list[j], key) > 0)
                    {
                        list[j + k] = list[j];
                        j -= k;
                    }

                    list[j + k] = key;
                }
            }
        }

        // -----------------------------------

        /// <summary>
        /// Invokes a quick mergesort algorithm on a list.
        /// Works with O(n*log(n)) speed.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">A list to be sorted.</param>
        /// <param name="comparer">The comparer for list elements.</param>
        public static void SortMerge<T>(this IList<T> list, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            sortMergeTrueMethod(list, comparer, 0, list.Count - 1, list.Count);
        }

        private static void sortMergeTrueMethod<T>(IList<T> arr, IComparer<T> comp, int lb, int rb, int n)
        {
            if (n <= 1) return;

            int a = n / 2; // количество элементов левой части массива
            int b = n / 2 + n % 2; // количество элементов правой части массива

            // сортируем обе части по отдельности

            sortMergeTrueMethod(arr, comp, lb, lb + a - 1, a);
            sortMergeTrueMethod(arr, comp, lb + a, rb, b);

            T[] cArr = new T[n];     // новый массив, в который 
            // идут отсортированные половины

            int c = 0;               // указатель на номер элемента нового массива

            int i = lb, j = lb + a; // указатели номера первых элементов
            // отсортированных частей.

            while (i < lb + a && j <= rb)
            {
                if (comp.Compare(arr[i], arr[j]) < 0)
                    cArr[c++] = arr[i++];
                else
                    cArr[c++] = arr[j++];
            }
            while (i < lb + a)
                cArr[c++] = arr[i++];
            while (j <= rb)
                cArr[c++] = arr[j++];

            // копируем элементы сортированного массива по нужному адресу
            for (c = 0; c < n; c++)
                arr[lb + c] = cArr[c];
        }
    }
}
