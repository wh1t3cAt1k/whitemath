using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using WhiteStructs.Conditions;

namespace WhiteStructs.Testing
{
    /// <summary>
    /// Represents an event handler which is called once 
	/// the <see cref="PerformanceTester{T, HT}"/> performs 
	/// a single test during two-level testing.
    /// </summary>
    /// <typeparam name="HT">The type of high-level tester argument.</typeparam>
    /// <param name="highLevelValue">The value of high-level tester argument.</param>
    /// <param name="meanTime">The mean running time of tested procedure.</param>
    public delegate void TestPerformedEventHandler<HT>(HT highLevelValue, decimal meanTime);

    /// <summary>
    /// This class provides means for conducting perfomance tests
    /// of one-argument procedures.
    /// </summary>
    public class PerformanceTester<T, HT>
    {
        /// <summary>
        /// This event fires once the <see cref="PerformanceTester&lt;T, HT&gt;"/>
        /// during performs a single test during two-level testing.
        /// </summary>
        public event TestPerformedEventHandler<HT> TestPerformed;

        /// <summary>
        /// Gets the procedure which mean running time is tested
		/// by the current <see cref="PerformanceTester{T, HT}"/>.
        /// </summary>
        public Action<T> TestedProcedure { get; private set; }

        /// <summary>
        /// Initializes the <see cref="PerformanceTester&lt;T, HT&gt;"/> with 
        /// a one-argument procedure which mean running time is
        /// to be estimated.
        /// </summary>
        /// <param name="procedure">A delegate pointing to a one-argument procedure.</param>
        public PerformanceTester(Action<T> procedure)
        {
			Condition.ValidateNotNull(procedure, nameof(procedure));

            this.TestedProcedure = procedure;
        }

        public List<KeyValuePair<HT, decimal>> TestProcedureTwoLevel(
            HT highLevelInitialValue,
            Func<int, HT, HT> highLevelChangeFunction,
            Func<int, HT, bool> highLevelStopCriteria,
            Func<int, HT, T> lowLevelValueProvider,
            Func<int, HT, Func<int, bool>> lowLevelStopCriteriaProvider,
			bool idleRun = true)
        {
			Condition.ValidateNotNull(highLevelChangeFunction, nameof(highLevelChangeFunction));
			Condition.ValidateNotNull(highLevelStopCriteria, nameof(highLevelStopCriteria));
			Condition.ValidateNotNull(lowLevelValueProvider, nameof(lowLevelValueProvider));
			Condition.ValidateNotNull(lowLevelStopCriteriaProvider, nameof(lowLevelStopCriteriaProvider));

            List<KeyValuePair<HT, decimal>> result = new List<KeyValuePair<HT, decimal>>();

            int highLevelChanges = 0;
            HT highLevelValue = highLevelInitialValue;

            while (!highLevelStopCriteria(highLevelChanges, highLevelValue))
            {
                T lowLevelValue = lowLevelValueProvider(highLevelChanges, highLevelValue);
                Func<int, bool> lowLevelStopCriteria = lowLevelStopCriteriaProvider(highLevelChanges, highLevelInitialValue);

                decimal currentTime = TestProcedure(lowLevelValue, lowLevelStopCriteria, idleRun);

                result.Add(new KeyValuePair<HT, decimal>(highLevelValue, currentTime));

                this.TestPerformed?.Invoke(highLevelValue, currentTime);

                highLevelValue = highLevelChangeFunction(highLevelChanges, highLevelValue);
                ++highLevelChanges;
            }

            return result;
        }

        public List<KeyValuePair<HT, decimal>> TestProcedureTwoLevel(
            HT highLevelInitialValue,
            Func<int, HT, HT> highLevelChangeFunction,
            Func<int, HT, bool> highLevelStopCriteria,
            Func<int, HT, Func<int, T>> lowLevelGeneratorProvider,
            Func<int, HT, Func<int, bool>> lowLevelStopCriteriaProvider,
            bool lowLevelPrecalculateArguments = true,
            bool idleRun = true)
        {
			Condition.ValidateNotNull(highLevelChangeFunction, nameof(highLevelChangeFunction));
			Condition.ValidateNotNull(highLevelStopCriteria, nameof(highLevelStopCriteria));
			Condition.ValidateNotNull(lowLevelGeneratorProvider, nameof(lowLevelGeneratorProvider));
			Condition.ValidateNotNull(lowLevelStopCriteriaProvider, nameof(lowLevelStopCriteriaProvider));

            Func<int, HT, T> initialValueProvider                   = (index, highValue) => (lowLevelGeneratorProvider(index, highValue))(0);
            Func<int, HT, Func<int, T, T>> changeFunctionProvider   = (highIndex, highValue) => ((lowIndex, junk) => lowLevelGeneratorProvider(highIndex, highValue)(lowIndex));
            Func<int, HT, Func<int, T, bool>> stopCriteriaProvider  = (highIndex, highValue) => ((lowIndex, junk) => lowLevelStopCriteriaProvider(highIndex, highValue)(lowIndex));

            return TestProcedureTwoLevel(
                highLevelInitialValue,
                highLevelChangeFunction,
                highLevelStopCriteria,
                initialValueProvider,
                changeFunctionProvider,
                stopCriteriaProvider,
                lowLevelPrecalculateArguments,
                idleRun);
        }

        public List<KeyValuePair<HT, decimal>> TestProcedureTwoLevel(
            HT highLevelInitialValue,
            Func<int, HT, HT> highLevelChangeFunction,
            Func<int, HT, bool> highLevelStopCriteria,
            Func<int, HT, T> lowLevelInitialValueProvider,
            Func<int, HT, Func<int, T, T>> lowLevelChangeFunctionProvider,
            Func<int, HT, Func<int, T, bool>> lowLevelStopCriteriaProvider,
            bool lowLevelPrecalculateArguments = true,
			bool performIdleRun = true)
        {
			Condition.ValidateNotNull(lowLevelChangeFunctionProvider, nameof(lowLevelChangeFunctionProvider));
			Condition.ValidateNotNull(lowLevelInitialValueProvider != null, nameof(lowLevelInitialValueProvider));
			Condition.ValidateNotNull(lowLevelStopCriteriaProvider != null, nameof(lowLevelStopCriteriaProvider));
			Condition.ValidateNotNull(highLevelChangeFunction != null, nameof(highLevelChangeFunction));
			Condition.ValidateNotNull(highLevelStopCriteria != null, nameof(highLevelStopCriteria));

            List<KeyValuePair<HT, decimal>> result = new List<KeyValuePair<HT, decimal>>();

            int highLevelChanges = 0;
            HT highLevelValue = highLevelInitialValue;

            while(!highLevelStopCriteria(highLevelChanges, highLevelValue))
            {
                Console.WriteLine(highLevelChanges);

                T lowLevelInitialValue = lowLevelInitialValueProvider(highLevelChanges, highLevelValue); 
                Func<int, T, T> lowLevelChangeFunction = lowLevelChangeFunctionProvider(highLevelChanges, highLevelValue);
                Func<int, T, bool> lowLevelStopCriteria = lowLevelStopCriteriaProvider(highLevelChanges, highLevelValue);

                decimal currentTime = TestProcedure(
					lowLevelInitialValue, 
					lowLevelChangeFunction, 
					lowLevelStopCriteria, 
					lowLevelPrecalculateArguments, 
					performIdleRun);

				// One idle run is enough.
				// -
				if (performIdleRun)
				{
					performIdleRun = false;
				}

                result.Add(new KeyValuePair<HT, decimal>(highLevelValue, currentTime));

				this.TestPerformed?.Invoke(highLevelValue, currentTime);

                highLevelValue = highLevelChangeFunction(highLevelChanges, highLevelValue);

                ++highLevelChanges;
            }

            return result;
        }

        public decimal TestProcedure(
            Func<int, T> generator, 
            Func<int, bool> stopCriteria, 
            bool precalculateArguments = true, 
			bool performIdleRun = true)
        {
			Condition.ValidateNotNull(generator, nameof(generator));
			Condition.ValidateNotNull(stopCriteria, nameof(stopCriteria));

            return TestProcedure(
                generator(0),
                (totalChanges, _) => generator(totalChanges),
                (totalChanges, _) => stopCriteria(totalChanges), 
                precalculateArguments, 
                performIdleRun);
        }

        /// <summary>
        /// Performs a performance test of specified procedure by substituting different 
        /// values of its argument and returning a mean value of function performance time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initialArgument"></param>
        /// <param name="changeFunction"></param>
        /// <param name="stopCriteria"></param>
        /// <param name="precalculateArguments"></param>
        /// <param name="performIdleRun"></param>
        /// <returns></returns>
        public decimal TestProcedure(
            T initialArgument, 
            Func<int, T, T> changeFunction, 
            Func<int, T, bool> stopCriteria, 
            bool precalculateArguments = true, 
			bool performIdleRun = true)
        {
			Condition.ValidateNotNull(changeFunction, nameof(changeFunction));
			Condition.ValidateNotNull(stopCriteria, nameof(stopCriteria));

            int totalChanges = 0;

			Stopwatch stopwatch = new Stopwatch();

            T currentArgument = initialArgument;

            if (precalculateArguments)
            {
                List<T> preparedArguments = new List<T>();
                preparedArguments.Add(initialArgument);

                while (!stopCriteria(totalChanges, currentArgument))
                {
                    currentArgument = changeFunction(totalChanges, currentArgument);
                    preparedArguments.Add(currentArgument);

                    ++totalChanges;
                }

				// We have in fact pre-calculated all argument values,
				// now we call the overloaded variant of the function.
				// -
                return TestProcedure(preparedArguments, performIdleRun);
            }

            if (performIdleRun)
            {
                this.TestedProcedure(currentArgument);
                
                stopCriteria(0, currentArgument);
                changeFunction(0, currentArgument);
            }

            stopwatch.Reset();
			stopwatch.Start();

            while (!stopCriteria(totalChanges, currentArgument))
			{
                this.TestedProcedure(currentArgument);

                currentArgument = changeFunction(totalChanges, currentArgument);
                ++totalChanges;
            }

			stopwatch.Stop();

			return (decimal)stopwatch.ElapsedMilliseconds / totalChanges;
        }

        /// <summary>
        /// Performance test procedure with a sequence of pre-defined arguments.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="performIdleRun">
		/// If set to <c>true</c>, the tested procedure will be called once prior
		/// to starting the stopwatch, with the first element of <paramref name="arguments"/> 
		/// passed as the procedure argument.
		/// </param>
		public decimal TestProcedure(IEnumerable<T> arguments, bool performIdleRun = true)
        {
			Condition.ValidateNotNull(arguments, nameof(arguments));
			Condition.ValidateNotEmpty(arguments, "The argument list should not be empty.");

            int testCount = 0;

			if (performIdleRun)
			{
				this.TestedProcedure(arguments.First());
			}

			Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            foreach (T nextArgument in arguments)
            {
                this.TestedProcedure(nextArgument);
                ++testCount;
            }

            stopwatch.Stop();

			return (decimal)stopwatch.ElapsedMilliseconds / testCount;
        }

        /// <summary>
        /// Several tests with the same argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument"></param>
        /// <param name="stopCriteria"></param>
        /// <param name="performIdleRun"></param>
        /// <returns></returns>
        public decimal TestProcedure(
			T argument, 
			Func<int, bool> stopCriteria, 
			bool performIdleRun = true)
        {
            int testCount = 0;

            if (performIdleRun)
            {
                this.TestedProcedure(argument);
                stopCriteria(0);
            }

			Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            while (!stopCriteria(testCount))
            {
                this.TestedProcedure(argument);
                ++testCount;
            }

            stopwatch.Stop();

			return (decimal)stopwatch.ElapsedMilliseconds / testCount;
        }
    }
}
