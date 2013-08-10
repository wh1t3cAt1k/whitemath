using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.General;

namespace whiteMath.Randoms
{
    /// <summary>
    /// This class is a wrapper around a uniformly distributed
    /// fractional random numbers generator and is dedicated to
    /// generating normal random numbers with specified expectation
    /// and standard deviation.
    /// </summary>
    /// <typeparam name="T">The type of random numbers generated. Is expected to be a fractional number type.</typeparam>
    /// <typeparam name="C">A calculator for the <typeparamref name="T"/> numeric type.</typeparam>
    public class RandomNormalBoxMuller<T, C>: IRandomUnbounded<T> where C: ICalc<T>, new()
    {
        private IRandomFloatingPoint<T> generator;
        
        private Numeric<T,C>    mean;
        private Numeric<T,C>    standardDeviation;
        private Func<T, T>      naturalLogarithmFunction;
        private Func<T, T>      squareRootFunction;
        private Func<T, T, T>   transformFunction;

        private T           next;
        private bool        nextAvailable;

        public bool IsIntegerGenerator { get { return false; } }

        public RandomNormalBoxMuller(IRandomFloatingPoint<T> uniformGenerator, T mean, T standardDeviation, Func<T, T> naturalLogarithmFunction, Func<T, T> squareRootFunction)
        {
            this.generator          = uniformGenerator;

            this.mean               = mean;
            this.standardDeviation  = standardDeviation;

            this.naturalLogarithmFunction   = naturalLogarithmFunction;
            this.squareRootFunction         = squareRootFunction;

            this.nextAvailable      = false;
            this.next               = default(T);

            this.transformFunction = delegate(T value, T squareSum) 
            {
                return (Numeric<T,C>)value * squareRootFunction(-Numeric<T,C>._2 * naturalLogarithmFunction(squareSum) / squareSum);
            };
        }

        public RandomNormalBoxMuller(IRandomFloatingPoint<T> uniformGenerator, Func<T, T> naturalLogarithmFunction, Func<T, T> squareRootFunction)
            : this(uniformGenerator, Numeric<T,C>.Zero, Numeric<T,C>._1, naturalLogarithmFunction, squareRootFunction)
        { }

        // ---------------------------------
        // ----- concrete implementations --
        // ---------------------------------

        public static RandomNormalBoxMuller<double, CalcDouble> DoubleGenerator(double mean = 0, double standardDeviation = 1, uint seed = 0)
        {
            return new RandomNormalBoxMuller<double, CalcDouble>(new RandomMersenneTwister(seed), mean, standardDeviation, Math.Log, Math.Sqrt); 
        }

        // -------------- RNG methods ------

        private Numeric<T, C> ___nextFromMinusOneToPlusOne()
        {
            // Берем число в [0; 1)
            // и вычитаем число из [0; 1)
            // Получаем число от -1 до +1 :)

            return Numeric<T,C>.Calculator.dif(generator.Next_SingleInterval(), generator.Next_SingleInterval());
        }

        public T Next()
        {
            if (nextAvailable)
            {
                nextAvailable = false;
                return this.mean + next * this.standardDeviation;
            }

            ICalc<T> calc = Numeric<T,C>.Calculator;

            T minusOne      = -(Numeric<T,C>._1);
            T plusOne       = Numeric<T,C>._1;

            T current;
            
            while(true)
            {
                Numeric<T,C> randomFirst    = ___nextFromMinusOneToPlusOne();
                Numeric<T,C> randomSecond   = ___nextFromMinusOneToPlusOne();

                Numeric<T, C> squareSum     = randomFirst * randomFirst + randomSecond * randomSecond;

                if (squareSum > Numeric<T, C>._1 || squareSum == Numeric<T, C>.Zero)
                    continue;
                else
                {
                    current = this.transformFunction(randomFirst, squareSum);
                    next    = this.transformFunction(randomSecond, squareSum);

                    break;
                }
            }

            return this.mean + current * this.standardDeviation;
        }

        public T Next(T minValue, T maxValue)
        {
            Numeric<T,C> generatedValue;

            do generatedValue = this.Next(); 
                while(generatedValue < minValue || generatedValue >= maxValue);

            return generatedValue;
        }

        public T Next_SingleInterval()
        {
            return this.Next(Numeric<T, C>.Zero, Numeric<T, C>._1);
        }
    }
}
