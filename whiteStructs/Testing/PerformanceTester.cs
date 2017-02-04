using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using WhiteStructs.Conditions;

namespace WhiteStructs.Testing
{
    /// <summary>
    /// This class provides means for conducting perfomance 
	/// tests of generic one-argument procedures.
    /// </summary>
    public class PerformanceTester<T, HT>
    {
		/// <summary>
		/// Represents an event handler which is called once 
		/// the <see cref="PerformanceTester{T, HT}"/> performs 
		/// a single test during two-level testing.
		/// </summary>
		/// <typeparam name="HT">The type of high-level tester argument.</typeparam>
		/// <param name="highLevelValue">The value of high-level tester argument.</param>
		/// <param name="meanTime">The mean running time of tested procedure.</param>
		public delegate void TestPerformedEventHandler(HT highLevelValue, decimal meanTime);

        /// <summary>
        /// This event fires once the <see cref="PerformanceTester&lt;T, HT&gt;"/>
        /// during performs a single test during two-level testing.
        /// </summary>
        public event TestPerformedEventHandler TestPerformed;

        /// <summary>
        /// Gets the procedure which mean running time is tested
		/// by the current <see cref="PerformanceTester{T, HT}"/>.
        /// </summary>
        public Action<T> TestedProcedure { get; private set; }

		/// <summary>
		/// If set to <c>true</c>, the tested procedure will be called once prior
		/// to starting the performance stopwatch. The procedure will be called with 
		/// the first available argument.
		/// </summary>
		public bool PerformIdleRun { get; private set; }

        /// <summary>
		/// Initializes the <see cref="PerformanceTester{T, HT}"/> with a 
		/// one-argument procedure whose average running time is to be estimated.
        /// </summary>
        /// <param name="procedure">A delegate pointing to a one-argument procedure.</param>
        public PerformanceTester(Action<T> procedure, bool performIdleRun)
        {
			Condition.ValidateNotNull(procedure, nameof(procedure));

            this.TestedProcedure = procedure;
			this.PerformIdleRun = performIdleRun;
        }

        public List<KeyValuePair<HT, decimal>> TestProcedureTwoLevel(
            HT highLevelInitialValue,
            Func<int, HT, HT> highLevelChangeFunction,
            Func<int, HT, bool> highLevelStopCriteria,
            Func<int, HT, T> lowLevelValueProvider,
            Func<int, HT, Func<int, bool>> lowLevelStopCriteriaProvider)
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

				decimal currentTime = TestProcedure(lowLevelValue, lowLevelStopCriteria);

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
            bool lowLevelPrecalculateArguments = true)
        {
			Condition.ValidateNotNull(highLevelChangeFunction, nameof(highLevelChangeFunction));
			Condition.ValidateNotNull(highLevelStopCriteria, nameof(highLevelStopCriteria));
			Condition.ValidateNotNull(lowLevelGeneratorProvider, nameof(lowLevelGeneratorProvider));
			Condition.ValidateNotNull(lowLevelStopCriteriaProvider, nameof(lowLevelStopCriteriaProvider));

            Func<int, HT, T> initialValueProvider = (index, highValue) => (lowLevelGeneratorProvider(index, highValue))(0);
            Func<int, HT, Func<int, T, T>> changeFunctionProvider = (highIndex, highValue) => ((lowIndex, junk) => lowLevelGeneratorProvider(highIndex, highValue)(lowIndex));
            Func<int, HT, Func<int, T, bool>> stopCriteriaProvider = (highIndex, highValue) => ((lowIndex, junk) => lowLevelStopCriteriaProvider(highIndex, highValue)(lowIndex));

            return TestProcedureTwoLevel(
                highLevelInitialValue,
                highLevelChangeFunction,
                highLevelStopCriteria,
                initialValueProvider,
                changeFunctionProvider,
                stopCriteriaProvider,
                lowLevelPrecalculateArguments);
        }

        public List<KeyValuePair<HT, decimal>> TestProcedureTwoLevel(
            HT highLevelInitialValue,
            Func<int, HT, HT> highLevelChangeFunction,
            Func<int, HT, bool> highLevelStopCriteria,
            Func<int, HT, T> lowLevelInitialValueProvider,
            Func<int, HT, Func<int, T, T>> lowLevelChangeFunctionProvider,
            Func<int, HT, Func<int, T, bool>> lowLevelStopCriteriaProvider,
            bool lowLevelPrecalculateArguments = true)
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
                T lowLevelInitialValue = lowLevelInitialValueProvider(highLevelChanges, highLevelValue); 
                Func<int, T, T> lowLevelChangeFunction = lowLevelChangeFunctionProvider(highLevelChanges, highLevelValue);
                Func<int, T, bool> lowLevelStopCriteria = lowLevelStopCriteriaProvider(highLevelChanges, highLevelValue);

                decimal currentTime = TestProcedure(
					lowLevelInitialValue, 
					lowLevelChangeFunction, 
					lowLevelStopCriteria, 
					lowLevelPrecalculateArguments);

                result.Add(new KeyValuePair<HT, decimal>(highLevelValue, currentTime));

				this.TestPerformed?.Invoke(highLevelValue, currentTime);

                highLevelValue = highLevelChangeFunction(highLevelChanges, highLevelValue);

                ++highLevelChanges;
            }

            return result;
        }

		/// <summary>
		/// Measures the tested procedure's average execution time
		/// over the range of arguments defined by the argument provider
		/// function.
		/// </summary>
		/// <returns>The procedure.</returns>
		/// <param name="argumentProvider">
		/// A function accepting the zero-based number of the
		/// current procedure run and returning the value of
		/// the argument that the tested procedure should be
		/// executed with during the run.
		/// </param>
		/// <param name="stopCriterion">
		/// A function accepting the zero-based number of the
		/// current procedure run and returning <c>true</c>
		/// if testing should be stopped prior to the run.
		/// </param>
        public decimal TestProcedure(
			Func<int, T> argumentProvider, 
			Func<int, bool> stopCriterion, 
            bool precalculateArguments)
        {
			Condition.ValidateNotNull(argumentProvider, nameof(argumentProvider));
			Condition.ValidateNotNull(stopCriterion, nameof(stopCriterion));

            return TestProcedure(
                argumentProvider(0),
                (totalChanges, _) => argumentProvider(totalChanges),
                (totalChanges, _) => stopCriterion(totalChanges), 
                precalculateArguments);
        }

        /// <summary>
		/// Measures the tested procedure's average execution time
		/// over the range of arguments defined by the initial argument
		/// value and the argument change rule. The tested procedure is 
		/// repeatedly called using the next argument value until the 
		/// specified stop criterion function returns <c>true</c>.
        /// </summary>
        public decimal TestProcedure(
            T initialArgument, 
			Func<int, T, T> argumentChangeFunction, 
			Func<int, T, bool> stopCriterion, 
            bool precalculateArguments)
        {
			Condition.ValidateNotNull(argumentChangeFunction, nameof(argumentChangeFunction));
			Condition.ValidateNotNull(stopCriterion, nameof(stopCriterion));

			int runsCount = 0;

            T currentArgument = initialArgument;

            if (precalculateArguments)
            {
				ICollection<T> preparedArguments = new List<T>();
                preparedArguments.Add(initialArgument);

                while (!stopCriterion(runsCount, currentArgument))
                {
                    currentArgument = argumentChangeFunction(runsCount, currentArgument);
                    preparedArguments.Add(currentArgument);

                    ++runsCount;
                }

				// We have in fact pre-calculated all argument values,
				// now we call the overloaded variant of the function.
				// -
                return TestProcedure(preparedArguments);
            }

			if (this.PerformIdleRun)
            {
                this.TestedProcedure(currentArgument);
            }

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

            while (!stopCriterion(runsCount, currentArgument))
			{
                this.TestedProcedure(currentArgument);

                currentArgument = argumentChangeFunction(runsCount, currentArgument);
                ++runsCount;
            }

			stopwatch.Stop();

			return (decimal)stopwatch.ElapsedMilliseconds / runsCount;
        }

        /// <summary>
        /// Measures the tested procedure's average execution time
		/// over the sequence of specified arguments.
        /// </summary>
		public decimal TestProcedure(IEnumerable<T> arguments)
        {
			Condition.ValidateNotNull(arguments, nameof(arguments));
			Condition.ValidateNotEmpty(arguments, "The argument list should not be empty.");

            int testCount = 0;

			if (this.PerformIdleRun)
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
		/// Measures the tested procedure's average execution time.
		/// The tested procedure is repeatedly called with the 
		/// specified argument value until the specified stop
		/// criterion function returns <c>true</c>.
        /// </summary>
        /// <param name="argument">
		/// The argument value with which the <see cref="TestedProcedure"/>
		/// will be called during performance test.
		/// </param>
        /// <param name="stopCriterion">
		/// A function accepting the number of procedure executions as 
		/// a parameter and returning a boolean value that indicates 
		/// whether the performance testing should stop.
		/// </param>
        public decimal TestProcedure(
			T argument, 
			Func<int, bool> stopCriterion)
        {
			if (this.PerformIdleRun)
            {
                this.TestedProcedure(argument);
            }

			int testCount = 0;

			Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!stopCriterion(testCount))
            {
                this.TestedProcedure(argument);
                ++testCount;
            }

            stopwatch.Stop();

			return (decimal)stopwatch.ElapsedMilliseconds / testCount;
        }
    }
}
