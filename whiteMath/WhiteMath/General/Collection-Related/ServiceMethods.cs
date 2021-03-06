﻿using System;
using System.Collections.Generic;

using WhiteMath.Calculators;
using WhiteMath.Random;

using WhiteStructs.Conditions;
using WhiteStructs.Collections;

namespace WhiteMath.General
{
    public static class ListShufflingExtensions
    {
        /// <summary>
        /// Performs a quick, linear-complexity random shuffling of the list.
		/// The quality of shuffling is medium. Uses the standard <see cref="System.Random"/> 
		/// number generator.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The calling list object.</param>
		/// <remarks>The algorithm passes the list only once.</remarks>
        public static void ShuffleQuick<T>(this IList<T> list)
        {
			Condition.ValidateNotNull(list, nameof(list));

            list.ShuffleQuick<T>(new RandomStandard());
        }

        /// <summary>
        /// Performs a quick, linear-complexity random shuffling of the list.
        /// The quality of shuffling is medium.
        /// Makes use of a user-provided integer random number generator.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The calling list object.</param>
        /// <param name="generator">A user-provided integer random number generator.</param>
		/// <remarks>The algorithm passes the list only once.</remarks>
		public static void ShuffleQuick<T>(this IList<T> list, IRandomBounded<int> generator)
        {
			Condition.ValidateNotNull(list, nameof(list));
			Condition.ValidateNotNull(generator, nameof(generator));

            for (int i = 0; i < list.Count - 1; i++)
            {
                int swapIndex = generator.Next(i + 1, list.Count);
                list.SwapElements(i, swapIndex);
            }
        }

        /// <summary>
        /// Performs a high-quality random shuffling of the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The calling list object.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
			Condition.ValidateNotNull(list, nameof(list));

            list.Shuffle(new RandomLaggedFibonacci(), Comparer<double>.Default);
        }

        /// <summary>
        /// Performs a high-quality random shuffling of the list.
        /// Uses a user-provided random number generator, which
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <typeparam name="N">The type of numbers generated by the random numbers generator.</typeparam>
        /// <param name="list">The calling list object.</param>
        /// <param name="generator">A floating-point random number generator for the <typeparamref name="N"/> type.</param>
        /// <param name="numericComparer">An optional numeric comparer for <typeparamref name="N"/> type. If <c>null</c>, a standard comparer will be used (if exists, otherwise, an exception will be thrown).</param>
        public static void Shuffle<T, N>(
			this IList<T> list, 
			IRandomUnitInterval<N> generator, 
			IComparer<N> numericComparer = null)
        {
			Condition.ValidateNotNull(list, nameof(list));
			Condition.ValidateNotNull(generator, nameof(generator));

			numericComparer = numericComparer ?? Comparer<N>.Default;

            // Generate a pairs list with random keys, sorted by keys.
			// The values will therefore turn out shuffled.
			// -
            List<KeyValuePair<N, T>> pairList = new List<KeyValuePair<N, T>>(list.Count);

			foreach (T element in list)
			{
				pairList.Add(new KeyValuePair<N, T>(generator.NextInUnitInterval(), element));
			}

            IComparer<KeyValuePair<N, T>> pairComparer = numericComparer.GetKVPComparerOnKey<N, T>();

			pairList.Sort(pairComparer);

			for (int i = 0; i < pairList.Count; ++i)
			{
				list[i] = pairList[i].Value;
			}
        }
    }

    /// <summary>
    /// Provides different service methods, e.g. copying between the lists.
    /// </summary>
    public static class ServiceMethods
    {
        // --------------------------------------
        // ------ LINKED LISTS ------------------
        // --------------------------------------

        /// <summary>
        /// Returns the array of linked list nodes, starting with the first and ending with the last.
        /// Takes O(N) time.
        /// 
        /// Warning! The array may go to obsolete/inconsistent state, stopping to represent
        /// the full collection of linked list nodes in their supposed order, if
        /// the list object is changed after or during getting the nodes list. (e.g. nodes are reordered, added, removed etc.)
        /// 
        /// Consider using IsObsolete() with checkOrder parameter equal to true for such an array, if you are in doubt.
        /// </summary>
        /// <typeparam name="T">The type of elements stored in the list.</typeparam>
        /// <param name="list">The calling linked list object.</param>
        /// <returns>The array of linked list nodes.</returns>
        public static LinkedListNode<T>[] GetNodes<T>(this LinkedList<T> list)
        {
			Condition.ValidateNotNull(list, nameof(list));

            LinkedListNode<T>[] arr = new LinkedListNode<T>[list.Count];

            LinkedListNode<T> current = list.First;

            for (int i = 0; i < list.Count; i++)
            {
                arr[i] = current;
                current = current.Next;
            }

            return arr;
        }

        /// <summary>
        /// Checks whether the linked list's nodes list
        /// that is supposed to represent all of the linked list's nodes
        /// is in obsolete state:
        /// 
        /// 1) Either does not represent all of the list's nodes.
        /// 2) Either contains nodes than aren't in the list.
        /// 3) Or not every next node in the array is the result of Next() method of the previous node - if flag <paramref name="checkOrder"/> is true.
        /// </summary>
        /// <typeparam name="T">The type of values stored in the nodes.</typeparam>
        /// <param name="nodesList">The list of linked list nodes supposed to represent all the list nodes (if <paramref name="checkOrder"/> is true, the method also checks if the order of nodes in the array is the same as they their order within the list).</param>
        /// <returns>The boolean flag equal to true if the nodes array is obsolete, false otherwise.</returns>
        public static bool IsObsolete<T>(this IList<LinkedListNode<T>> nodesList, LinkedList<T> list, bool checkOrder = true)
        {
			Condition.ValidateNotNull(nodesList, nameof(nodesList));
			Condition.ValidateNotNull(list, nameof(list));

            if (list.Count != nodesList.Count)
                return true;

            if (nodesList.Count == 0)
                return false;

            if(nodesList[0].List != list)
                return true;

            for (int i = 1; i < nodesList.Count; i++)
                if (nodesList[i].List != list || (checkOrder && nodesList[i].Previous != nodesList[i - 1]))
                    return true;

            // Проверять на неповторяемость элементов массива нужно только в том случае,
            // если не проверяли порядок.

            if (checkOrder == false)
            {
                HashSet<LinkedListNode<T>> set = new HashSet<LinkedListNode<T>>(nodesList);
                
                if (set.Count != nodesList.Count)
                    return true;
            }

            return false;
        }

        // ----------------------------
        // -------- change order ------
        // ----------------------------

        private static ArgumentException _NODE_LIST_OBSOLETE_EXCEPTION = new ArgumentException("The nodes list passed is obsolete - it either doesn't contain all the linked list nodes, or contains nodes that aren't in the list anymore, or contains repeated nodes.");

        /// <summary>
        /// Reorders the nodes within the list so that their order is
        /// the same as nodes order within the nodes list object passed.
        /// 
        /// Requirement: the nodes list passed should contain unique nodes which are all the nodes of the linked list, not less and not more;
        /// This is automatically true if the nodes list passes the <see cref="IsObsolete&lt;T&gt;"/> test with <c>checkOrder</c> set to false.
        /// </summary>
        /// <typeparam name="T">The type of values stored in the linked list.</typeparam>
        /// <param name="list">The calling linked list object to be reordered.</param>
        /// <param name="nodesList">A nodes list containing unrepeatedly all the nodes of the linked list, and nothing more.</param>
        public static void ReorderAsInList<T>(this LinkedList<T> list, IList<LinkedListNode<T>> nodesList)
        {
			Condition.ValidateNotNull(list, nameof(list));
			Condition.ValidateNotNull(nodesList, nameof(nodesList));

            HashSet<LinkedListNode<T>> set = new HashSet<LinkedListNode<T>>();  // будем смотреть, нет ли в списке узлов повторений.

            if (list.Count != nodesList.Count)
                throw _NODE_LIST_OBSOLETE_EXCEPTION;

            // Пустой список нечего переиначивать.

            if (list.Count == 0)
                return;

            // Если первый элемент списка узлов не принадлежит связному списку, это кердык.

            if (nodesList[0].List != list)
                throw _NODE_LIST_OBSOLETE_EXCEPTION;

            // Делаем первый узел списка узлов также и первым узлом связного списка.

            else if (nodesList[0] != list.First)
            {
                list.Remove(nodesList[0]);
                list.AddFirst(nodesList[0]);
            }

            set.Add(nodesList[0]);

            // Дальше - по накатанной.

            for (int i = 0; i < nodesList.Count - 1; i++)
            {
                if (nodesList[i + 1].List != list)
                    throw _NODE_LIST_OBSOLETE_EXCEPTION;

                // Проверка на уникальность

                set.Add(nodesList[i + 1]);

                if (set.Count != i + 2)
                    throw _NODE_LIST_OBSOLETE_EXCEPTION;

                // Если порядок внутри связного списка не такой, какой внутри массива узлов,
                // Восстанавливаем его.

                if (nodesList[i].Next != nodesList[i + 1])
                {
                    list.Remove(nodesList[i + 1]);
                    list.AddAfter(nodesList[i], nodesList[i + 1]);
                }
            }

            return;
        }

        /// <summary>
        /// Exchanges the positions the two linked list nodes.
        /// </summary>
        /// <typeparam name="T">The type of values stored in the linked list.</typeparam>
        /// <param name="list">The calling linked list object.</param>
        /// <param name="nodeA">The first node to be exchanged.</param>
        /// <param name="nodeB">The second node to be exchanged.</param>
		public static void SwapNodes<T>(
			this LinkedList<T> list, 
			LinkedListNode<T> nodeA, 
			LinkedListNode<T> nodeB)
        {
			Condition.ValidateNotNull(list, nameof(list));
			Condition.ValidateNotNull(nodeA, nameof(nodeA));
			Condition.ValidateNotNull(nodeB, nameof(nodeB));

            if (nodeA == nodeB) return;
			            
            if (nodeA.Next == nodeB)
            {
				LinkedListNode<T> nodeBeforeA = nodeA.Previous;
                
                list.Remove(nodeA);
                list.Remove(nodeB);

                if (nodeBeforeA == null)
                {
                    list.AddFirst(nodeB);
                    list.AddAfter(list.First, nodeA);

                    return;
                }
                else
                {
                    list.AddAfter(nodeBeforeA, nodeB);
                    list.AddAfter(nodeB, nodeA);

                    return;
                }
            }
            else if (nodeB.Next == nodeA)      
            {
				LinkedListNode<T> nodeBeforeB = nodeB.Previous;

                list.Remove(nodeB);
                list.Remove(nodeA);

                if (nodeBeforeB == null)
                {
                    list.AddFirst(nodeA);
                    list.AddAfter(list.First, nodeB);

                    return;
                }                       // второй узел не начинает список
                else
                {
                    list.AddAfter(nodeBeforeB, nodeA);
                    list.AddAfter(nodeA, nodeB);

                    return;
                }
            }
            else if (nodeA != list.Last)
            {
                LinkedListNode<T> afterOne = nodeA.Next;
                list.Remove(nodeA);

                if (nodeB != list.Last)
                {
                    LinkedListNode<T> afterTwo = nodeB.Next;
                    list.Remove(nodeB);

                    list.AddBefore(afterTwo, nodeA);
                    list.AddBefore(afterOne, nodeB);

                    return;
                }
                else
                {
                    list.RemoveLast();

                    list.AddBefore(afterOne, nodeB);
                    list.AddLast(nodeA);

                    return;
                }
            }
            else
            {
				// The first node is last in the list.
				// -
                list.RemoveLast();

                // Since nodes were not neighbouring, there is a
				// node after the second node.
				// -
                LinkedListNode<T> afterTwo = nodeB.Next;
                list.Remove(nodeB);

                list.AddBefore(afterTwo, nodeA);
                list.AddLast(nodeB);

                return;
            }
        }

        /// <summary>
        /// Copies the entire source list to the destination list.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="destination">The destination list.</param>
        public static void Copy<T>(IReadOnlyList<T> source, IList<T> destination)
        {
			Condition.ValidateNotNull(source, nameof(source));
			Condition.ValidateNotNull(destination, nameof(destination));

            Copy(source, 0, destination, 0, source.Count);
        }

        /// <summary>
        /// Copies the elements from the source list (starting from sourceIndex)
        /// to the destination list. The filling of the destination list is started
        /// from destinationIndex.
        /// </summary>
        public static void Copy<T>(IReadOnlyList<T> source, int sourceIndex, IList<T> destination, int destinationIndex)
        {
			Condition.ValidateNotNull(source, nameof(source));

            Copy(source, sourceIndex, destination, destinationIndex, source.Count);
        }

        /// <summary>
        /// Copies the elements from the source list (starting from sourceIndex, overall 'length' elements are copied)
        /// to the destination list. The filling of the destination list is started
        /// from destinationIndex.
        /// </summary>
        public static void Copy<T>(IReadOnlyList<T> source, int sourceIndex, IList<T> destination, int destinationIndex, int length)
        {
			Condition.ValidateNotNull(source, nameof(source));

			for (int i = 0; i < length; ++i)
			{
				destination[destinationIndex + i] = source[sourceIndex + i];
			}

            return;
        }
        
        // ------------------------------------------
        // ------------- SWAP -----------------------
        // ------------------------------------------

        /// <summary>
        /// Performs the swapping of two list elements with specified indices.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list object reference.</param>
        /// <param name="firstIndex">The index of the first element to be swapped.</param>
        /// <param name="secondIndex">The index of the second element to be swapped.</param>
		public static void SwapElements<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
			Condition.ValidateNotNull(list, nameof(list));

            T temp = list[firstIndex];

            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        // ------------------------------------------
        // ------------- ARRAY EXTENSIONS -----------
        // ------------------------------------------

        private static System.Random gen = new System.Random();

        /// <summary>
        /// Randomizes the long integer digits array.
        /// </summary>
        /// <param name="arr">The calling array object.</param>
        /// <param name="BASE">The base of the digits in the long integer number.</param>
        public static void Randomize(this IList<int> arr, int BASE)
        {
			Condition.ValidateNotNull(arr, nameof(arr));

            for (int i = 0; i < arr.Count; i++)
                arr[i] = gen.Next(0, BASE);
        }

        /// <summary>
        /// Randomizes the integer list.
        /// The numbers being filled vary from minimum inclusive bound to the maximum exclusive bound.
        /// </summary>
        /// <param name="arr">The calling array object.</param>
        /// <param name="minInclusive">The lower inclusive bound of elements' value.</param>
        /// <param name="maxExclusive">The upper exclusive bound of elements' value.</param>
        public static void Randomize(this IList<int> arr, int minInclusive, int maxExclusive)
        {
			Condition.ValidateNotNull(arr, nameof(arr));

            for (int i = 0; i < arr.Count; i++)
                arr[i] = gen.Next(minInclusive, maxExclusive);
        }

        // ------------------------------------------
        // ------------- BINARY SEARCH --------------
        // ------------------------------------------

        /// <summary>
        /// Searches an entire sorted sequence of objects
        /// for a specific an <c>IComparer&lt;<typeparamref name="T"/>&gt;</c> instance. 
        /// </summary>
        /// <typeparam name="T">The type of the element to search for.</typeparam>
        /// <param name="list">The list that is sorted ascending according to the <paramref name="comparer"/></param>
        /// <param name="key">The element to search for.</param>
        /// <param name="comparer">An optional <c>IComparer</c> for <c><typeparamref name="T"/></c> objects. If <c>null</c>, a default comparer will be used (or an exception will be thrown if one doesn't exist).</param>
        /// <returns>
        /// The index of the specified <paramref name="key"/> 
        /// in the specified <paramref name="list"/>, if <paramref name="key"/> is found. 
        /// 
        /// If value is not found and value is less than one or more elements in <paramref name="list"/>, 
        /// a negative number which is the bitwise complement of the 
        /// index of the first element that is larger than value. 
        /// 
        /// If value is not found and value is greater than any of the elements in <paramref name="list"/>, 
        /// a negative number which is the bitwise complement of (the index of the last element plus 1). 
        /// </returns>
        public static int WhiteBinarySearch<T>(this IList<T> list, T key, IComparer<T> comparer = null)
        {
			Condition.ValidateNotNull(list, nameof(list));
            return WhiteBinarySearch(list, x => x, key, comparer);
        }

        /// <summary>
        /// Searches an entire sorted sequence of <c><typeparamref name="M"/></c>-typed objects
        /// for a specific <c><typeparamref name="T"/></c>-typed element, using
        /// a <c><typeparamref name="M"/>=><typeparamref name="T"/></c> projector function
        /// and an <c>IComparer&lt;<typeparamref name="T"/>&gt;</c> instance. 
        /// </summary>
        /// <typeparam name="T">The type of the element to search for.</typeparam>
        /// <typeparam name="M">The type of elements in the list.</typeparam>
        /// <param name="list">The list that is sorted ascending according to the <paramref name="projector"/> function and the <paramref name="comparer"/></param>
        /// <param name="projector">A function that maps <c><typeparamref name="M"/></c> to <c><typeparamref name="T"/></c></param>
        /// <param name="key">The element to search for.</param>
        /// <param name="comparer">An optional <c>IComparer</c> for <c><typeparamref name="T"/></c> objects. If <c>null</c>, a default comparer will be used (or an exception will be thrown if one doesn't exist).</param>
        /// <returns>
        /// The index of the specified <paramref name="key"/> (as the result of <paramref name="projector"/> function applied) 
        /// in the specified <paramref name="list"/>, if <paramref name="key"/> is found. 
        /// 
        /// If value is not found and value is less than one or more elements in <paramref name="list"/>, 
        /// a negative number which is the bitwise complement of the 
        /// index of the first element that is larger than value. 
        /// 
        /// If value is not found and value is greater than any of the elements in <paramref name="list"/>, 
        /// a negative number which is the bitwise complement of (the index of the last element plus 1). 
        /// </returns>
        public static int WhiteBinarySearch<T, M>(
			this IList<M> list, 
			Func<M, T> projector, T key, 
			IComparer<T> comparer = null)
        {
			Condition.ValidateNotNull(list, nameof(list));

            comparer = comparer ?? Comparer<T>.Default;

			int leftBoundaryIndex = 0;
			int rightBoundaryIndex = list.Count - 1;

			int middleIndex;
			int comparisonResult;

            while (true)
            {
                middleIndex = (leftBoundaryIndex + rightBoundaryIndex) / 2;

                comparisonResult = comparer.Compare(key, projector(list[middleIndex]));

				if (comparisonResult < 0)
				{
					rightBoundaryIndex = middleIndex - 1;
				}
				else if (comparisonResult > 0)
				{
					leftBoundaryIndex = middleIndex + 1;
				}
				else
				{
					return middleIndex;
				}

				if (leftBoundaryIndex > rightBoundaryIndex)
				{
					return ~leftBoundaryIndex;
				}
            }

        }

        // ------------------------------------------
        // ------------- USED WITH LONG INTS --------
        // ------------------------------------------

        /// <summary>
        /// For long integers. Returns the amount of significant digits in the 
        /// digits array (excluding the leading zeroes).
        /// </summary>
        /// <param name="list">The long integer digits list.</param>
        /// <returns>The number of significant digits in the number.</returns>
        public static int CountSignificant(this IReadOnlyList<int> list)
        {
			Condition.ValidateNotNull(list, nameof(list));

			int result = list.Count - 1;

			while (list[result] == 0 && result > 0)
			{
				result--;
			}

            return result + 1;
        }

        /// <summary>
        /// For polynoms. Returns the amount of significant coefficients in the
        /// coefficients array (excluding the leading zeroes).
        /// </summary>
        /// <typeparam name="T">The type of polynom coefficients.</typeparam>
        /// <typeparam name="C">The calculator for the coefficient type.</typeparam>
        /// <param name="list">The list of coefficients.</param>
        /// <returns>The number of significant coefficients of the polynom.</returns>
        public static int CountSignificant<T, C>(this IReadOnlyList<Numeric<T, C>> list) where C: ICalc<T>, new()
        {
			Condition.ValidateNotNull(list, nameof(list));

            C calc = Numeric<T, C>.Calculator;

			int result = list.Count - 1;

			while (list[result] == Numeric<T, C>.Zero && result > 0)
			{
				--result;
			}

            return result + 1;
        }

        /// <summary>
        /// Used with long integers digit arrays. Cuts the incoming list
        /// so that it contains only significant digits.
        /// </summary>
        /// <param name="list">The incoming digits list.</param>
        /// <returns>The list without leading zeroes.</returns>
        public static int[] Cut(this IReadOnlyList<int> list)
        {
			Condition.ValidateNotNull(list, nameof(list));

			int[] newList = new int[list.CountSignificant()];

            Copy(list, 0, newList, 0, newList.Length);

            return newList;
        }

        /// <summary>
        /// Used with long integers digit arrays. Cuts the incoming list
        /// so that it contains only significant digits.
        /// </summary>
        /// <param name="list">The list of the digits.</param>
        /// <returns>The list without any leading zeroes.</returns>
        public static List<int> Cut(this List<int> list)
        {
			Condition.ValidateNotNull(list, nameof(list));

            List<int> newList = new List<int>(list.CountSignificant());
            newList.AddRange(new int[newList.Capacity]);

            Copy(list, 0, newList, 0, newList.Count);

            return newList;
        }

        /// <summary>
        /// Used with long integers digit arrays. Cut the incoming list
        /// so that it contains only significant digits. The cutting is 
		/// performed in-place, without creation of new objects.
        /// </summary>
        /// <param name="list">The digits list to be cut.</param>
        public static void CutInPlace(this List<int> list)
        {
			Condition.ValidateNotNull(list, nameof(list));

            int difference = list.Count - list.CountSignificant();

			if (difference > 0)
			{
				list.RemoveRange(list.Count - difference, difference);
			}
        }

        /// <summary>
        /// Used with long integers digit arrays. Cuts the incoming list
        /// so that it contains only significant digits.
        /// </summary>
        /// <param name="list">The incoming digits list.</param>
        /// <returns>The list without leading zeroes.</returns>
        public static Numeric<T, C>[] Cut<T, C>(this IReadOnlyList<Numeric<T,C>> list) where C: ICalc<T>, new()
        {
			Condition.ValidateNotNull(list, nameof(list));

            Numeric<T,C>[] newList = new Numeric<T,C>[list.CountSignificant()];

            Copy(list, 0, newList, 0, newList.Length);

            return newList;
        }

        public static int InversionCount<T, C>(this IList<T> list) 
			where C: ICalc<T>, new()
        {
			int inversionsCount = 0;
            
            C calc = Numeric<T, C>.Calculator;

			for (int step = 1; step < list.Count; step++)
			{
				for (int i = step; i < list.Count; i++)
				{
					if (calc.GreaterThan(list[i - step], list[i]))
					{
						++inversionsCount;
					}
				}
			}

            return inversionsCount;
        }

        public static int InversionCount<T, C>(this IList<Numeric<T, C>> list) 
			where C : ICalc<T>, new()
        {
            int k = 0;

            for (int step = 1; step < list.Count; step++)
                for (int i = step; i < list.Count; i++)
                    if (list[i - step] > list[i])
                        k++;

            return k;
        }
    }
}
