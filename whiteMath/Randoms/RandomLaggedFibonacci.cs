using System;
using System.Collections.Generic;

namespace whiteMath.Randoms
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
    public class RandomLaggedFibonacci: IRandomFloatingPoint<double>
    {
        int a;
        int b;

        int max;

        LinkedList<double> list = new LinkedList<double>();

        LinkedListNode<double> xkmaNode;
        LinkedListNode<double> xkmbNode;    

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
        public RandomLaggedFibonacci(int seed = -1, int a = 97, int b = 33)
        {
            this.a = a;
            this.b = b;

            this.max = Math.Max(a, b);
            
            if(seed < 0)
            {
                // long val = 0;
                // whiteMath.General.NativeMethods.QueryPerformanceCounter(ref val);

                // seed = (int)val;
				seed = (int)(DateTime.Now.Ticks % int.MaxValue);
            }

            Random gen = new Random(seed);

            for (int i = 0; i < max; i++)
                list.AddLast(gen.NextDouble());

            xkmaNode = list.First;
            xkmbNode = list.First;

            for (int i = 0; i < max - a; i++)
                xkmaNode = xkmaNode.Next;

            for (int i = 0; i < max - b; i++)
                xkmbNode = xkmbNode.Next;

            return;
        }

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
        /// <param name="firstGenerator">The IRandom(T) implementer object to receive the first values of the pseudo-random sequence.</param>
        /// <param name="a">The first lag of the fibonacci generator. Optional. By default, equals 97.</param>
        /// <param name="b">The second lag of the fibonacci generator. Optional. By default, equals 33.</param>
        public RandomLaggedFibonacci(IRandomFloatingPoint<double> firstGenerator, int a = 97, int b = 33)
        {
            this.a = a;
            this.b = b;

            this.max = Math.Max(a, b);

            for (int i = 0; i < max; i++)
                list.AddLast(firstGenerator.Next_SingleInterval());

            xkmaNode = list.First;
            xkmbNode = list.First;

            for (int i = 0; i < max - a; i++)
                xkmaNode = xkmaNode.Next;

            for (int i = 0; i < max - b; i++)
                xkmbNode = xkmbNode.Next;

            return;
        }

        // --------------------------------

        /// <summary>
        /// Returns the next pseudo-random double number laying in the interval [0; 1).
        /// </summary>
        /// <returns>The next pseudo-random double number in the interval [0; 1).</returns>
        public double Next_SingleInterval()
        {
            double xkma = xkmaNode.Value;
            double xkmb = xkmbNode.Value;

            double xk = (xkma < xkmb ? xkma - xkmb + 1 : xkma - xkmb);

            list.AddLast(xk);

            xkmaNode = xkmaNode.Next;
            xkmbNode = xkmbNode.Next;
            
            list.RemoveFirst();

            return xk;
        }
    }
}
