using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics.Contracts;

using whiteMath.General;
using whiteMath.Time;

namespace whiteStructs.Testing
{
    /// <summary>
    /// Represents a one-argument procedure without a return value.
    /// </summary>
    /// <typeparam name="T">The type of the argument that procedure receives.</typeparam>
    /// <param name="arg">The argument received by the procedure.</param>
    public delegate void OneArgumentProcedure<T>(T arg);

    /// <summary>
    /// Represents an event handler which is called once 
    /// the <see cref="PerformanceTester"/> performs a single
    /// test during two-level testing.
    /// </summary>
    /// <typeparam name="HT">The type of high-level tester argument.</typeparam>
    /// <param name="highLevelValue">The value of high-level tester argument.</param>
    /// <param name="meanTime">The mean running time of tested procedure.</param>
    public delegate void TestPerformedEventHandler<HT>(HT highLevelValue, double meanTime);

    /// <summary>
    /// This class provides means for conducting perfomance tests
    /// of one-argument procedures.
    /// </summary>
    [ContractVerification(true)]
    public class PerformanceTester<T, HT>
    {
        /// <summary>
        /// This event fires once the <see cref="PerformanceTester"/>
        /// during performs a single test during two-level testing.
        /// </summary>
        public event TestPerformedEventHandler<HT> TestPerformed;

        /// <summary>
        /// Gets the procedure which mean running time is tested
        /// by the current <see cref="PerformanceTester"/>.
        /// </summary>
        public OneArgumentProcedure<T> TestedProcedure { get; private set; }

        /// <summary>
        /// Initializes the <see cref="PerformanceTester"/> with 
        /// a one-argument procedure which mean running time is
        /// to be estimated.
        /// </summary>
        /// <param name="procedure">A delegate pointing to a one-argument procedure.</param>
        public PerformanceTester(OneArgumentProcedure<T> procedure)
        {
            Contract.Requires<ArgumentNullException>(procedure != null, "procedure");

            this.TestedProcedure = procedure;
        }

        public List<KeyValuePair<HT, double>> TestProcedureTwoLevel(
            HT                                  highLevelInitialValue,
            Func<int, HT, HT>                   highLevelChangeFunction,
            Func<int, HT, bool>                 highLevelStopCriteria,
            Func<int, HT, T>                    lowLevelValueProvider,
            Func<int, HT, Func<int, bool>>      lowLevelStopCriteriaProvider,
            bool                                idleRun = true
            )
        {
            Contract.Requires<ArgumentNullException>(highLevelStopCriteria != null, "highLevelStopCriteria");
            Contract.Requires<ArgumentNullException>(lowLevelValueProvider != null, "lowLevelValueProvider");
            Contract.Requires<ArgumentNullException>(lowLevelStopCriteriaProvider != null, "lowLevelStopCriteriaProvider");

            List<KeyValuePair<HT, double>> result = new List<KeyValuePair<HT, double>>();

            int highLevelChanges = 0;
            HT highLevelValue = highLevelInitialValue;

            while (!highLevelStopCriteria(highLevelChanges, highLevelValue))
            {
                T lowLevelValue = lowLevelValueProvider(highLevelChanges, highLevelValue);
                Func<int, bool> lowLevelStopCriteria = lowLevelStopCriteriaProvider(highLevelChanges, highLevelInitialValue);

                double currentTime = TestProcedure(lowLevelValue, lowLevelStopCriteria, idleRun);

                result.Add(new KeyValuePair<HT, double>(highLevelValue, currentTime));

                // -----------------------------------
                // ----- выполнил тест - скажи соседу!
                // -----------------------------------

                if (this.TestPerformed != null)
                    this.TestPerformed.Invoke(highLevelValue, currentTime);

                // -----------------------------------

                highLevelValue = highLevelChangeFunction(highLevelChanges, highLevelValue);
                ++highLevelChanges;
            }

            return result;
        }

        public List<KeyValuePair<HT, double>> TestProcedureTwoLevel(
            HT                                  highLevelInitialValue,
            Func<int, HT, HT>                   highLevelChangeFunction,
            Func<int, HT, bool>                 highLevelStopCriteria,
            Func<int, HT, Func<int, T>>         lowLevelGeneratorProvider,
            Func<int, HT, Func<int, bool>>      lowLevelStopCriteriaProvider,
            bool                                lowLevelPrecalculateArguments = true,
            bool                                idleRun = true)
        {
            Contract.Requires<ArgumentNullException>(highLevelChangeFunction != null, "highLevelChangeFunction");
            Contract.Requires<ArgumentNullException>(highLevelStopCriteria != null, "highLevelStopCriteria");
            Contract.Requires<ArgumentNullException>(lowLevelGeneratorProvider != null, "lowLevelGeneratorProvider");
            Contract.Requires<ArgumentNullException>(lowLevelStopCriteriaProvider != null, "lowLevelStopCriteriaProvider");

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

        public List<KeyValuePair<HT, double>> TestProcedureTwoLevel(
            HT                                  highLevelInitialValue,
            Func<int, HT, HT>                   highLevelChangeFunction,
            Func<int, HT, bool>                 highLevelStopCriteria,
            Func<int, HT, T>                    lowLevelInitialValueProvider,
            Func<int, HT, Func<int, T, T>>      lowLevelChangeFunctionProvider,
            Func<int, HT, Func<int, T, bool>>   lowLevelStopCriteriaProvider,
            bool                                lowLevelPrecalculateArguments = true,
            bool                                idleRun = true)
        {
            Contract.Requires<ArgumentNullException>(lowLevelChangeFunctionProvider != null, "lowLevelChangeFunctionProvider");
            Contract.Requires<ArgumentNullException>(lowLevelInitialValueProvider != null, "lowLevelInitialValueProvider");
            Contract.Requires<ArgumentNullException>(lowLevelStopCriteriaProvider != null, "lowLevelStopCriteriaProvider");
            Contract.Requires<ArgumentNullException>(highLevelChangeFunction != null, "highLevelChangeFunction");
            Contract.Requires<ArgumentNullException>(highLevelStopCriteria != null, "highLevelStopCriteria");

            List<KeyValuePair<HT, double>> result = new List<KeyValuePair<HT, double>>();

            int highLevelChanges    = 0;
            HT highLevelValue       = highLevelInitialValue;

            while(!highLevelStopCriteria(highLevelChanges, highLevelValue))
            {
                Console.WriteLine(highLevelChanges);

                T lowLevelInitialValue                  = lowLevelInitialValueProvider(highLevelChanges, highLevelValue); 
                Func<int, T, T> lowLevelChangeFunction  = lowLevelChangeFunctionProvider(highLevelChanges, highLevelValue);
                Func<int, T, bool> lowLevelStopCriteria = lowLevelStopCriteriaProvider(highLevelChanges, highLevelValue);

                double currentTime = TestProcedure(lowLevelInitialValue, lowLevelChangeFunction, lowLevelStopCriteria, lowLevelPrecalculateArguments, idleRun);

                // Одного холостого запуска вполне достаточно.

                if (idleRun)
                    idleRun = false;

                result.Add(new KeyValuePair<HT,double>(highLevelValue, currentTime));

                // -----------------------------------
                // ----- выполнил тест - скажи соседу!
                // -----------------------------------

                if (this.TestPerformed != null)
                    this.TestPerformed.Invoke(highLevelValue, currentTime);

                // -----------------------------------

                highLevelValue = highLevelChangeFunction(highLevelChanges, highLevelValue);
                ++highLevelChanges;
            }

            return result;
        }

        public double TestProcedure(
            Func<int, T>        generator, 
            Func<int, bool>     stopCriteria, 
            bool                precalculateArguments = true, 
            bool                idleRun = true)
        {
            Contract.Requires<ArgumentNullException>(generator != null, "generator");
            Contract.Requires<ArgumentNullException>(stopCriteria != null, "stopCriteria");

            return TestProcedure(
                generator(0),
                (totalChanges, junk) => generator(totalChanges),
                (totalChanges, junk) => stopCriteria(totalChanges), 
                precalculateArguments, 
                idleRun); 
        }

        /// <summary>
        /// Performs a performance test of specified procedure by substituting different 
        /// values of its argument and returning a mean value of function performance time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedure"></param>
        /// <param name="initialArgument"></param>
        /// <param name="changeFunction"></param>
        /// <param name="stopCriteria"></param>
        /// <param name="precalculateArguments"></param>
        /// <param name="idleRun"></param>
        /// <returns></returns>
        public double TestProcedure(
            T                       initialArgument, 
            Func<int, T, T>         changeFunction, 
            Func<int, T, bool>      stopCriteria, 
            bool                    precalculateArguments = true, 
            bool                    idleRun = true)
        {
            Contract.Requires<ArgumentNullException>(changeFunction != null, "changeFunction");
            Contract.Requires<ArgumentNullException>(stopCriteria != null, "stopCriteria");

            int totalChanges = 0;

            NanoStopWatch sw = new NanoStopWatch();
            T currentArgument = initialArgument;

            List<KeyValuePair<T, double>> result = new List<KeyValuePair<T, double>>();

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

                // Фактически мы выполнили предвычисление всех значений аргумента,
                // так что теперь выполняется вызов перегруженного варианта функции.

                return TestProcedure(preparedArguments, idleRun);
            }

            if (idleRun)
            {
                this.TestedProcedure(currentArgument);
                
                stopCriteria(0, currentArgument);
                changeFunction(0, currentArgument);
            }

            sw.reset();
            sw.start();

            while (!stopCriteria(totalChanges, currentArgument))
            {
                // make time measurements
                // -----------------------

                this.TestedProcedure(currentArgument);

                currentArgument = changeFunction(totalChanges, currentArgument);
                ++totalChanges;
            }

            sw.stop();

            return (double)sw.getIntervalInMillis() / totalChanges;
        }

        /// <summary>
        /// Для проведения тестов с множеством предопределенных аргументов.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedure"></param>
        /// <param name="arguments"></param>
        /// <param name="idleRun"></param>
        /// <returns></returns>
        public double TestProcedure(IEnumerable<T> arguments, bool idleRun = true)
        {
            Contract.Requires<ArgumentNullException>(arguments != null, "arguments");
            Contract.Requires<ArgumentException>(arguments.Count() > 0, "The argument list should not be empty.");

            int testCount = 0;

            if (idleRun)
                this.TestedProcedure(arguments.First());

            NanoStopWatch sw = new NanoStopWatch();

            sw.start();

            foreach (T nextArgument in arguments)
            {
                this.TestedProcedure(nextArgument);
                ++testCount;
            }

            sw.stop();

            return (double)(sw.getIntervalInMillis() / testCount);
        }

        /// <summary>
        /// Для проведения нескольких тестов с одним аргументом.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedure"></param>
        /// <param name="argument"></param>
        /// <param name="stopCriteria"></param>
        /// <param name="idleRun"></param>
        /// <returns></returns>
        public double TestProcedure(T argument, Func<int, bool> stopCriteria, bool idleRun = true)
        {
            int testCount = 0;

            if (idleRun)
            {
                this.TestedProcedure(argument);
                stopCriteria(0);
            }

            NanoStopWatch sw = new NanoStopWatch();

            sw.start();

            while (!stopCriteria(testCount))
            {
                this.TestedProcedure(argument);
                ++testCount;
            }

            sw.stop();

            return (double)(sw.getIntervalInMillis() / testCount);
        }
    }
}
