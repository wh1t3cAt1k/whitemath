using System;

using WhiteMath.Calculators;

namespace WhiteMath.Randoms
{
    /// <summary>
    /// This class is a wrapper around a uniformly distributed
    /// fractional random numbers generator and is dedicated to
    /// generating normal random numbers with specified expected 
	/// value and standard deviation.
    /// </summary>
    /// <typeparam name="T">The type of random numbers generated. Is expected to be a fractional number type.</typeparam>
    /// <typeparam name="C">A calculator for the <typeparamref name="T"/> numeric type.</typeparam>
    public class RandomNormalBoxMuller<T, C>: IRandomUnbounded<T> where C: ICalc<T>, new()
    {
		private static readonly ICalc<T> Calculator = Numeric<T, C>.Calculator;

		private IRandomFloatingPoint<T> _generator;
        
		private Numeric<T,C> _mean;
		private Numeric<T,C> _standardDeviation;
		private Func<T, T> _naturalLogarithm;
		private Func<T, T> _squareRoot;
		private Func<T, T, T> _transformFunction;

		private T _next;
		private bool _isNextAvailable;

		public bool IsIntegerGenerator => false;

        public RandomNormalBoxMuller(
			IRandomFloatingPoint<T> uniformGenerator, 
			T mean, 
			T standardDeviation, 
			Func<T, T> naturalLogarithm, 
			Func<T, T> squareRoot)
        {
            _generator = uniformGenerator;

            _mean = mean;
            _standardDeviation = standardDeviation;

			_naturalLogarithm = naturalLogarithm;
			_squareRoot = squareRoot;

            _isNextAvailable = false;
           	_next = default(T);

            this._transformFunction = (value, squareSum) => 
            	Calculator.Multiply(
					value,
					squareRoot(-Numeric<T,C>._2 * _naturalLogarithm(squareSum) / squareSum));
        }

        public RandomNormalBoxMuller(
			IRandomFloatingPoint<T> uniformGenerator, 
			Func<T, T> naturalLogarithmFunction, 
			Func<T, T> squareRootFunction)
            : this(
				uniformGenerator, 
				Numeric<T,C>.Zero, 
				Numeric<T,C>._1, 
				naturalLogarithmFunction, 
				squareRootFunction)
        { }

        // ---------------------------------
        // ----- concrete implementations --
        // ---------------------------------

		public static RandomNormalBoxMuller<double, CalcDouble> CreateGeneratorDouble(
			double mean = 0, 
			double standardDeviation = 1, 
			uint seed = 0)
        {
            return new RandomNormalBoxMuller<double, CalcDouble>(
				new RandomMersenneTwister(seed), 
				mean, 
				standardDeviation, 
				Math.Log, 
				Math.Sqrt); 
        }

        // -------------- RNG methods ------

        private Numeric<T, C> NextFromMinusOneToPlusOne()
        {
			// We take number in [0; 1),
			// Double it, get a number in [0; 2),
			// Subtract a number in [0; 1),
			// Resulting value is in [-1; +1).
			// -
            return Calculator.Subtract(
				Calculator.Multiply(
					Numeric<T, C>._2, 
					_generator.NextInUnitInterval()), 
				_generator.NextInUnitInterval());
        }

        public T Next()
        {
            if (_isNextAvailable)
            {
                _isNextAvailable = false;
                return this._mean + _next * this._standardDeviation;
            }

            T current;
            
            while (true)
            {
				Numeric<T,C> firstRandom = NextFromMinusOneToPlusOne();
				Numeric<T,C> secondRandom = NextFromMinusOneToPlusOne();

				Numeric<T, C> sumOfSquares = firstRandom * firstRandom + secondRandom * secondRandom;

				if (sumOfSquares > Numeric<T, C>._1 || sumOfSquares == Numeric<T, C>.Zero)
				{
					continue;
				}
                else
                {
                    current = this._transformFunction(firstRandom, sumOfSquares);
                    _next = this._transformFunction(secondRandom, sumOfSquares);
                    break;
                }
            }

            return this._mean + current * this._standardDeviation;
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
