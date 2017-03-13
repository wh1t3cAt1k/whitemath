using System;
using System.Collections.Generic;

namespace WhiteMath.Random
{
    /// <summary>
    /// This class represents a pseudo-random lagged Fibonacci generator for <c>double</c> values.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Identical randomization degree of each resulting number bit.</item>
    /// <item>An enormous period of repetition.</item>
    /// </list>
    /// </remarks>
    public class RandomLaggedFibonacci: IRandomUnitInterval<double>
    {
		private readonly LinkedList<double> _valueList = new LinkedList<double>();

		private LinkedListNode<double> _xkmaNode;
		private LinkedListNode<double> _xkmbNode;

		/// <summary>
		/// Constructs a Lagged Fibonacci pseudo-random number generator with an integer seed
		/// number (uses an instance of standard Random() class for generating first sequence members)
		/// and the lag values.
		/// 
		/// Examples of recommended lag values:
		/// 
		/// a) 17; 5
		/// b) 55; 24
		/// c) 97; 33.
		/// 
		/// It is not recommended to provide random lag values as it will affect
		/// the quality of the generator randomization.
		/// </summary>
		/// <param name="seed">The integer seed number to generate the first numbers of the random sequence. Optional. By default, the negative value means that the seed will be queried from the CPU ticks counter.</param>
		/// <param name="a">The first lag of the fibonacci generator. Optional. By default, equals 97.</param>
		/// <param name="b">The second lag of the fibonacci generator. Optional. By default, equals 33.</param>
		public RandomLaggedFibonacci(int? seed = null, int a = 97, int b = 33)
			: this(new RandomStandard(seed), a, b)
		{ }

        /// <summary>
        /// Constructs a Fibonacci lagged pseudo-random number generator using the lag values and 
        /// another generator for the first sequence members.
        /// 
        /// Examples of recommended lag values:
        /// 
        /// a) 17; 5
        /// b) 55; 24
        /// c) 97; 33.
        /// 
        /// It is not recommended to provide random lag values as it will affect
        /// the quality of the generator randomization.
        /// </summary>
		/// <param name="firstValuesGenerator">
		/// The <see cref="IRandomUnitInterval{double}"/> generator to 
		/// receive the first values of the pseudo-random sequence.
		/// </param>
        /// <param name="a">The first lag parameter of the fibonacci generator. By default, equals 97.</param>
        /// <param name="b">The second lag parameter of the fibonacci generator. By default, equals 33.</param>
		public RandomLaggedFibonacci(IRandomUnitInterval<double> firstValuesGenerator, int a = 97, int b = 33)
        {
			int max = Math.Max(a, b);

			for (int i = 0; i < max; ++i)
			{
				_valueList.AddLast(firstValuesGenerator.NextInUnitInterval());
			}

            _xkmaNode = _valueList.First;
            _xkmbNode = _valueList.First;

			for (int i = 0; i < max - a; ++i)
			{
				_xkmaNode = _xkmaNode.Next;
			}

			for (int i = 0; i < max - b; ++i)
			{
				_xkmbNode = _xkmbNode.Next;
			}
        }

        /// <summary>
        /// Returns the next pseudo-random double number laying in the interval [0; 1).
        /// </summary>
        /// <returns>The next pseudo-random double number in the interval [0; 1).</returns>
        public double NextInUnitInterval()
        {
            double xkma = _xkmaNode.Value;
            double xkmb = _xkmbNode.Value;

            double xk = (xkma < xkmb ? xkma - xkmb + 1 : xkma - xkmb);

            _valueList.AddLast(xk);

            _xkmaNode = _xkmaNode.Next;
            _xkmbNode = _xkmbNode.Next;
            
            _valueList.RemoveFirst();

            return xk;
        }
    }
}
