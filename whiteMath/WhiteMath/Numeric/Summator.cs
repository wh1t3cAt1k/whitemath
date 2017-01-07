using System;
using System.Collections.Generic;

using WhiteMath.General;

namespace WhiteMath
{
    /// <summary>
    /// This class provides methods to sum or multiply different sequences member
    /// represented by the generic, [index and/or argument]-determined formula.
    /// 
    /// Different methods provide summation / multiplication in different order 
    /// determined by [or regardless of] different metrics of the object.
    /// 
    /// For example, you may want to use this class to sum up
    /// the taylor sequence for exp(x) with members indexed from 2 to 30, getting
    /// maximum precision by starting with smallest ones and ending up with biggest.
    /// </summary>
    public class Summator<T>
    {
        /// <summary>
        /// Gets the value served as zero.
        /// </summary>
        public T Zero { get; private set; }

        /// <summary>
        /// Gets the method which calculates the sum of two <typeparamref name="T"/> values.
        /// </summary>
        public Func<T, T, T> OperatorPlus { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of Summator(<typeparamref name="T"/>) class
        /// using a <typeparamref name="T"/> value serving as a zero initializer and a function for the summing up two <typeparamref name="T"/> values.
        /// </summary>
        /// <param name="zero">A <typeparamref name="T"/> value serving as a zero initializer for overall sum.</param>
        /// <param name="operatorPlus">A function delegate allowing to get a sum of two numbers.</param>
        public Summator(T zero, Func<T, T, T> operatorPlus)
        {
            this.Zero = zero;
            this.OperatorPlus = operatorPlus;
        }

        /// <summary>
        /// Performs a simple, sequential summation of members of an index-dependent sequence.
        /// Requires a general term formula dependent only on the integer member index (<paramref name="memberFormula"/>).
        /// </summary>
        /// <param name="startIndex">The inclusive beginning index of sequence summation.</param>
        /// <param name="endIndex">The inclusive ending index of sequence summation.</param>
        /// <param name="memberFormula">The general term formula depending as the function of integer index.</param>
        /// <returns>The result of the sequential summation of provided sequence starting with index <paramref name="startIndex"/> and ending with index <paramref name="endIndex"/>, both inclusive.</returns>
        public T SumSequentially(Func<int, T> memberFormula, int startIndex, int endIndex)
        {
            T sum = this.Zero;

            if (startIndex >= endIndex)
                for (int i = startIndex; i <= endIndex; i++)
                    sum = this.OperatorPlus(sum, memberFormula(i));
            else
                for (int i = endIndex; i >= startIndex; i--)
                    sum = this.OperatorPlus(sum, memberFormula(i));

            return sum;
        }

        /// <summary>
        /// Performs the simple, sequential summation of members of both argument- and index-dependent sequence. 
        /// Requires the current function argument as well as a general term formula.
        /// </summary>
        /// <param name="argument">The current argument for the sequence.</param>
        /// <param name="startIndex">The inclusive beginning index of sequence summation.</param>
        /// <param name="endIndex">The inclusive ending index of sequence summation.</param>
        /// <param name="memberFormula">The general term formula as the function of <typeparamref name="T"/> argument and an integer index.</param>
        /// <returns>The result of sequential summation of the sequence starting with index <paramref name="startIndex"/> and ending with index <paramref name="endIndex"/>, both inclusive.</returns>
		public T SumSequentially(T argument, Func<T, int, T> memberFormula, int startIndex, int endIndex)
        {
            T sum = this.Zero;

            if (startIndex >= endIndex)
                for (int i = startIndex; i <= endIndex; i++)
                    sum = this.OperatorPlus(sum, memberFormula(argument, i));
            else
                for (int i = endIndex; i >= startIndex; i--)
                    sum = this.OperatorPlus(sum, memberFormula(argument, i));

            return sum;
        }

        /// <summary>
        /// Performs a smaller-to-bigger summation of sequence members.
        /// Works slightly slower than a simple sequential summation, 
        /// but as smallest values are summed up first, this method
        /// significantly increases precision if the sequence doesn't non-decrease
        /// monotonously and consequent values are of different degrees.
        /// 
        /// Requires a general term formula dependent only on the member index (<paramref name="memberFormula"/>),
        /// as well as a comparer allowing of knowing which sequence element is bigger and which is smaller.
        /// </summary>
        /// <param name="startIndex">The inclusive beginning index of sequence summation.</param>
        /// <param name="endIndex">The inclusive ending index of sequence summation.</param>
        /// <param name="comparer">The comparer to compare values of type <typeparamref name="T"/>. Should return a positive value if the first value is bigger than the second, zero value if they are equal, and a negative value if the first object is less.</param>
        /// <param name="memberFormula">The general term formula dependent on an integer index.</param>
        /// <returns>The result of lower-to-bigger summation of the sequence starting with index <paramref name="startIndex"/> and ending with index <paramref name="endIndex"/>, both inclusive.</returns>
        public T SumSmallerToLarger(Func<int, T> memberFormula, int startIndex, int endIndex, IComparer<T> comparer)
        {
            IPriorityQueue<T> sequence = new BinaryHeap<T>(comparer.GetReverseComparer());

            if (startIndex >= endIndex)
                for (int i = startIndex; i <= endIndex; i++)
                    sequence.Insert(memberFormula(i));
            else
                for (int i = endIndex; i >= startIndex; i--)
                    sequence.Insert(memberFormula(i));

            T sum = this.Zero;

            while (!sequence.IsEmpty)
                sum = this.OperatorPlus(sum, sequence.Pop());

            return sum;
        }

        /// <summary>
        /// Performs a smaller-to-bigger summation of sequence members.
        /// Works slightly slower than a simple sequential summation, 
        /// but as smallest values are summed up first, this method
        /// significantly increases precision if the sequence doesn't non-decrease
        /// monotonously and consequent values are of different degrees.
        /// 
        /// Requires a general term formula dependent on both argument and integer index (<paramref name="memberFormula"/>) as well as the current function argument.
        /// Also requires a comparer allowing of knowing which sequence element is bigger and which is smaller.
        /// </summary>
        /// <param name="startIndex">The inclusive beginning index of sequence summation.</param>
        /// <param name="endIndex">The inclusive ending index of sequence summation.</param>
        /// <param name="comparer">The comparer to compare values of type <typeparamref name="T"/>. Should return a positive value if the first value is bigger than the second, zero value if they are equal, and a negative value if the first object is less.</param>
        /// <param name="memberFormula">The general term formula dependent on both argument and integer index.</param>
        /// <param name="argument">The current argument for the sequence.</param>
        /// <returns>The result of sequential summation of the sequence starting with index <paramref name="startIndex"/> and ending with index <paramref name="endIndex"/>, both inclusive.</returns>
		public T SumSmallerToLarger(T argument, Func<T, int, T> memberFormula, int startIndex, int endIndex, IComparer<T> comparer)
        {
            IPriorityQueue<T> sequence = new BinaryHeap<T>(comparer.GetReverseComparer());

            if (startIndex >= endIndex)
                for (int i = startIndex; i <= endIndex; i++)
                    sequence.Insert(memberFormula(argument, i));
            else
                for (int i = endIndex; i >= startIndex; i--)
                    sequence.Insert(memberFormula(argument, i));

            T sum = this.Zero;

            while (!sequence.IsEmpty)
                sum = this.OperatorPlus(sum, sequence.Pop());

            return sum;
        }

        /// <summary>
        /// Performs a summation of sequence members according to their integer metric provided by a IMetricProvider object.
        /// The summation is performed starting with lowest metric and ending up with highest.
        /// Works slower than a simple sequential summation.
        /// 
        /// Requires a general term formula dependent on an integer index (<paramref name="memberFormula"/>).
        /// </summary>
        /// <param name="startIndex">The inclusive beginning index of sequence summation.</param>
        /// <param name="endIndex">The inclusive ending index of sequence summation.</param>
        /// <param name="metricProvider">The metric mapping values of type <typeparamref name="T"/> to their integer metric.</param>
        /// <param name="memberFormula">The general term formula dependent on an integer index.</param>
        /// <returns>The result of sequential summation of the sequence starting with index <paramref name="startIndex"/> and ending with index <paramref name="endIndex"/>, both inclusive.</returns>
		public T SumByIncreasingMetric(Func<int, T> memberFormula, int startIndex, int endIndex, IMetricProvider<T> metricProvider)
        {
            IComparer<KeyValuePair<int, T>> comparer = Comparer<int>.Default.GetKVPComparerOnKey<int, T>();
            IPriorityQueue<KeyValuePair<int, T>> queue = new BinaryHeap<KeyValuePair<int, T>>(comparer);

			if (startIndex >= endIndex)
			{
				for (int i = startIndex; i <= endIndex; i++)
				{
					T res = memberFormula(i);
					queue.Insert(new KeyValuePair<int, T>(metricProvider.GetMetric(res), res));
				}
			}
			else
			{
				for (int i = endIndex; i >= startIndex; i--)
				{
					T res = memberFormula(i);
					queue.Insert(new KeyValuePair<int, T>(metricProvider.GetMetric(res), res));
				}
			}

            T sum = this.Zero;

            while (!queue.IsEmpty)
            {
                KeyValuePair<int, T> kvp = queue.Pop();
                sum = this.OperatorPlus(sum, kvp.Value);
            }

            return sum;
        }


        /// <summary>
        /// Performs the summation of sequence members according to their integer metric provided by a IMetricProvider object.
        /// The summation is performed starting with lowest metric and ending up with highest.
        /// Works slower than a simple sequential summation.
        /// 
        /// Requires a general term formula dependent on both argument and integer index (<paramref name="memberFormula"/>) as well as the current function argument.
        /// </summary>
        /// <param name="startIndex">The inclusive beginning index of sequence summation.</param>
        /// <param name="endIndex">The inclusive ending index of sequence summation.</param>
        /// <param name="metricProvider">The metric mapping values of type <typeparamref name="T"/> to their integer metric.</param>
        /// <param name="memberFormula">The general term formula dependent on both argument and integer index.</param>
        /// <param name="argument">The current argument for the sequence.</param>
        /// <returns>The result of sequential summation of the sequence starting with index <paramref name="startIndex"/> and ending with index <paramref name="endIndex"/>, both inclusive.</returns>
		public T SumByIncreasingMetric(T argument, Func<T, int, T> memberFormula, int startIndex, int endIndex, IMetricProvider<T> metricProvider)
        {
            IComparer<KeyValuePair<int, T>> comparer = Comparer<int>.Default.GetKVPComparerOnKey<int, T>();
            IPriorityQueue<KeyValuePair<int, T>> queue = new BinaryHeap<KeyValuePair<int, T>>(comparer);
            
            if (startIndex >= endIndex)
                for (int i = startIndex; i <= endIndex; i++)
                {
                    T res = memberFormula(argument, i);
                    queue.Insert(new KeyValuePair<int, T>(metricProvider.GetMetric(res), res));
                }
            else
                for (int i = endIndex; i >= startIndex; i--)
                {
                    T res = memberFormula(argument, i); 
                    queue.Insert(new KeyValuePair<int, T>(metricProvider.GetMetric(res), res));
                }

            T sum = this.Zero;

            while(!queue.IsEmpty)
            {
                KeyValuePair<int, T> kvp = queue.Pop();
                sum = this.OperatorPlus(sum, kvp.Value);
            }

            return sum;
        }

        // --------------------------------------
        // ------ By minimum current metric -----
        // --------------------------------------

		public T SumByMinimumMetricSum(Func<int, T> memberFormula, int startIndex, int endIndex, IMetricProvider<T> metricProvider)
        {
			throw new NotImplementedException();
        }

        // ---------- abs comparer ---------------

        private class AbsoluteIntegerComparer: IComparer<int>
        {
            public int Compare(int one, int two)
            {
                int absOne = Math.Abs(one);
                int absTwo = Math.Abs(two);

				if (absOne > absTwo)
				{
					return 1;
				}
				else if (absOne < absTwo)
				{
					return -1;
				}
				else
				{
					return 0;
				}
            }
        }

        public T SumByMinimumCurrentMetricSum(Func<int, T> memberFormula, int startIndex, int endIndex, IMetricProvider<T> metricProvider)
        {
            return SumByMinimumCurrentMetricSum(this.Zero, delegate(T arg, int index) { return memberFormula(index); }, startIndex, endIndex, metricProvider);
        }

		public T SumByMinimumCurrentMetricSum(T argument, Func<T, int, T> memberFormula, int startIndex, int endIndex, IMetricProvider<T> metricProvider)
        {
            LinkedList<KeyValuePair<int, T>> listPositive = new LinkedList<KeyValuePair<int, T>>();  
            LinkedList<KeyValuePair<int, T>> listNegative = new LinkedList<KeyValuePair<int, T>>();
            LinkedList<KeyValuePair<int, T>> listZero = new LinkedList<KeyValuePair<int, T>>();

            IComparer<KeyValuePair<int, T>> comparer = 
				new AbsoluteIntegerComparer().GetKVPComparerOnKey<int, T>();

			// ---- добавляем в список -----

			if (startIndex >= endIndex)
			{
				for (int i = startIndex; i <= endIndex; ++i)
				{
					T res = memberFormula(argument, i);
					int metric = metricProvider.GetMetric(res);

					if (metric > 0)
					{
						listPositive.AddLast(new KeyValuePair<int, T>(metric, res));
					}
					else if (metric < 0)
					{
						listNegative.AddLast(new KeyValuePair<int, T>(metric, res));
					}
					else
					{
						listZero.AddLast(new KeyValuePair<int, T>(0, res));
					}
				}
			}
			else
			{
				for (int i = endIndex; i >= startIndex; --i)
				{
					T res = memberFormula(argument, i);
					int metric = metricProvider.GetMetric(res);

					if (metric > 0)
						listPositive.AddLast(new KeyValuePair<int, T>(metric, res));
					else if (metric < 0)
						listNegative.AddLast(new KeyValuePair<int, T>(metric, res));
					else
						listZero.AddLast(new KeyValuePair<int, T>(0, res));
				}
			}

			// Sort the list by increasing absolute key value.
			// The zeroes need not be sorted.
			// -
            // TODO: the sorting could be performed faster?
			// -
            listPositive.InsertionSort(comparer);
            listNegative.InsertionSort(comparer);

			// First we sum the zero-metric numbers as they do not
			// increase the current metric.
			// -
            T sum = this.OperatorPlus(this.Zero, SumValues(listZero));
            long metricSum = 0;                 
            
            LinkedListNode<KeyValuePair<int, T>> analyzedNode;

            while (listPositive.Count > 0 || listNegative.Count > 0)
            {
				#if DEBUG
              	Console.WriteLine(Math.Abs(metricSum));
				#endif

				// Once we have exhausted either of the the lists,
				// the only thing we can do is to sum the remaining list
				// sequentially.
				// -
				if (listNegative.Count == 0)
				{
					return this.OperatorPlus(sum, SumValues(listPositive));
				}
				else if (listPositive.Count == 0)
				{
					return this.OperatorPlus(sum, SumValues(listNegative));
				}

                if (metricSum > 0)     
                {
					// Look for the key in the negative list so as to decrease the 
					// absolute sum value as possible.
					// -
                    analyzedNode = listNegative.First;

					// Первое условие здесь - абсолютное значение 
					// ключа анализируемого узла меньше суммы метрик + абсолютное значение текущего отрицательного узла
					// Остальные нет смысла проверять - там неоптимально.

					while (Math.Abs(analyzedNode.Value.Key) <= metricSum && analyzedNode != null)
					{
						analyzedNode = analyzedNode.Next;
					}

                    if (analyzedNode == null)
                    {
						// Edge case, we have reached the end.
						// -
						// We take either the last negative, or the first 
						// positive, depending on what's better.
						// -
                        long positiveKeySum = metricSum + listPositive.First.Value.Key;
                        long negativeKeySum = metricSum + listNegative.Last.Value.Key;

                        if (Math.Abs(positiveKeySum) > Math.Abs(negativeKeySum))
                        {
                            metricSum = negativeKeySum;
                            sum = this.OperatorPlus(sum, listNegative.Last.Value.Value);

                            listNegative.RemoveLast();
                        }
                        else
                        {
                            metricSum = positiveKeySum;
                            sum = this.OperatorPlus(sum, listPositive.First.Value.Value);

                            listPositive.RemoveFirst();
                        }
                    }
                    else if (analyzedNode == listNegative.First)
                    {
						// We take either the first negative, or the first 
						// positive, depending on what's better.
						// -
                        long positiveKeySum = metricSum + listPositive.First.Value.Key;
                        long negativeKeySum = metricSum + listNegative.First.Value.Key;

                        if (Math.Abs(negativeKeySum) < Math.Abs(positiveKeySum))
                        {
							// Adding a negative is better.
							// -
							metricSum = negativeKeySum;
                            sum = this.OperatorPlus(sum, listNegative.First.Value.Value);

                            listNegative.RemoveFirst();
                        }
                        else
                        {
                            metricSum = positiveKeySum;
                            sum = this.OperatorPlus(sum, listPositive.First.Value.Value);

                            listPositive.RemoveFirst();
                        }

                    }
                    else
                    {
                        long positiveKeySum = metricSum + listPositive.First.Value.Key;
                        long negativeKeySum1 = metricSum + analyzedNode.Previous.Value.Key;
                        long negativeKeySum2 = metricSum + analyzedNode.Value.Key;

                        if (Math.Abs(negativeKeySum1) <= Math.Abs(negativeKeySum2) 
							&& Math.Abs(negativeKeySum1) <= Math.Abs(positiveKeySum))
                        {
                            metricSum = negativeKeySum1;
                            sum = this.OperatorPlus(sum, analyzedNode.Previous.Value.Value);

                            listNegative.Remove(analyzedNode.Previous);
                        }
                        else if (
							Math.Abs(negativeKeySum2) <= Math.Abs(negativeKeySum1) 
							&& Math.Abs(negativeKeySum2) <= Math.Abs(positiveKeySum))
                        {
                            metricSum = negativeKeySum2;
                            sum = this.OperatorPlus(sum, analyzedNode.Value.Value);

                            listNegative.Remove(analyzedNode);
                        }
                        else
                        {
                            metricSum = positiveKeySum;
                            sum = this.OperatorPlus(sum, listPositive.First.Value.Value);

                            listPositive.RemoveFirst();
                        }
                    }
                }
                else if (metricSum < 0)
                {
					// Look for such a key in the list of positives
					// so as to maximally decrease the absolute value
					// of metric sum.
					// -
                    analyzedNode = listPositive.First;

					// Первое условие здесь - абсолютное значение 
					// ключа анализируемого узла меньше суммы метрик + абсолютное значение текущего отрицательного узла
					// Остальные нет смысла проверять - там неоптимально.

					while (analyzedNode.Value.Key <= Math.Abs(metricSum) && analyzedNode != null)
					{
						analyzedNode = analyzedNode.Next;
					}

                    if (analyzedNode == null)
                    {
                        // Одно из двух - или последний положительный, или первый отрицательный.
                        // В зависимости от того, что хуже.

                        long positiveKeySum = metricSum + listPositive.Last.Value.Key;
                        long negativeKeySum = metricSum + listNegative.First.Value.Key;
                        
                        // Если прибавить положительное выгоднее.
                        if (Math.Abs(positiveKeySum) < Math.Abs(negativeKeySum))
                        {
                            metricSum = positiveKeySum;
                            sum = this.OperatorPlus(sum, listPositive.Last.Value.Value);

                            listPositive.RemoveLast();
                        }
                        else
                        {
                            metricSum = negativeKeySum;
                            sum = this.OperatorPlus(sum, listNegative.First.Value.Value);

                            listNegative.RemoveFirst();
                        }
                    }
                    else if (analyzedNode == listPositive.First)
                    {
                        // Одно из двух - или первый отрицательный, или первый положительный.
                        // В зависимости от того, что хуже.

                        long positiveKeySum = metricSum + listPositive.First.Value.Key;
                        long negativeKeySum = metricSum + listNegative.First.Value.Key;

                        // Если прибавить отрицательное выгоднее.
                        if (Math.Abs(positiveKeySum) > Math.Abs(negativeKeySum))
                        {
                            metricSum = negativeKeySum;
                            sum = this.OperatorPlus(sum, listNegative.First.Value.Value);

                            listNegative.RemoveFirst();
                        }
                        else
                        {
                            metricSum = positiveKeySum;
                            sum = this.OperatorPlus(sum, listPositive.First.Value.Value);

                            listPositive.RemoveFirst();
                        }

                    }
                    else
                    {
                        long negativeKeySum = metricSum + listNegative.First.Value.Key;
                        long positiveKeySum1 = metricSum + analyzedNode.Previous.Value.Key;
                        long positiveKeySum2 = metricSum + analyzedNode.Value.Key;

                        if (Math.Abs(positiveKeySum1) <= Math.Abs(positiveKeySum2) && Math.Abs(positiveKeySum1) <= Math.Abs(negativeKeySum))
                        {
                            metricSum = positiveKeySum1;
                            sum = this.OperatorPlus(sum, analyzedNode.Previous.Value.Value);

                            listPositive.Remove(analyzedNode.Previous);
                        }
                        else if (Math.Abs(positiveKeySum2) <= Math.Abs(positiveKeySum1) && Math.Abs(positiveKeySum2) <= Math.Abs(negativeKeySum))
                        {
                            metricSum = positiveKeySum2;
                            sum = this.OperatorPlus(sum, analyzedNode.Value.Value);

                            listPositive.Remove(analyzedNode);
                        }
                        else
                        {
                            metricSum = negativeKeySum;
                            sum = this.OperatorPlus(sum, listNegative.First.Value.Value);

                            listNegative.RemoveFirst();
                        }
                    }
                }
                else     
                {
					// The metric sum is zero. No need to search, just
					// decide what's better: to add the first positive
					// or the last negative.
					// -
                    if (listPositive.First.Value.Key < listNegative.First.Value.Key)
                    {
                        metricSum += listPositive.First.Value.Key;
                        sum = this.OperatorPlus(sum, listPositive.First.Value.Value); 

                        listPositive.RemoveFirst();
                    }
                    else
                    {
                        metricSum += listNegative.First.Value.Key;
                        sum = this.OperatorPlus(sum, listNegative.First.Value.Value); 

                        listNegative.RemoveFirst();
                    }
                }
            }

            return sum;
        }

        /// <summary>
        /// Sums all values in the linked list of key vale pairs.
        /// </summary>
		private T SumValues(IEnumerable<KeyValuePair<int, T>> sequence)
        {
            T sum = this.Zero;

			foreach (KeyValuePair<int, T> pair in sequence)
			{
				sum = this.OperatorPlus(sum, pair.Value);
			}

            return sum;
        }

    }
}
