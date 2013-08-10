using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics.Contracts;

namespace whiteStructs.Testing
{
    /// <summary>
    /// Represents a one-argument function.
    /// </summary>
    /// <typeparam name="T">The type of the function's argument.</typeparam>
    /// <typeparam name="R">The type of the function's return value.</typeparam>
    /// <param name="arg">The argument received by the procedure.</param>
    /// <returns>A value as specified by function's logic.</returns>
    public delegate R OneArgumentFunction<T, R>(T arg);

    /// <summary>
    /// This class contains the event data for the
    /// <c>TestConductedEventHandler&lt;<typeparamref name="T"/>,<typeparamref name="R"/>&gt;</c>
    /// event.
    /// </summary>
    /// <see cref="<c>TestConductedEventHandler&lt;T,R&gt;</c>"/>
    /// <typeparam name="T">The type of tested function's argument.</typeparam>
    /// <typeparam name="R">The type of tested function's return value.</typeparam>
    [ContractVerification(true)]
    public class TestConductedEventArgs<T, R> : EventArgs
    {
        /// <summary>
        /// Gets the zero-based number of the test in the series.
        /// </summary>
        public int TestNumber { get; private set; }

        /// <summary>
        /// Gets the value of the argument which was used
        /// to invoke the tested function.
        /// </summary>
        public T Argument { get; private set; }

        /// <summary>
        /// Gets the result produced by the tested function.
        /// </summary>
        public R Result { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the
        /// <c>ResultTester&lt;<typeparamref name="T"/>,<typeparamref name="R"/>&gt;</c>
        /// should stop further testing.
        /// </summary>
        public bool Stop { get; set; }

        public TestConductedEventArgs(int testNumber, T argument, R result)
        {
            Contract.Requires<ArgumentOutOfRangeException>(testNumber > 0, "The number of the test in the series should be a positive number.");
            
            this.TestNumber = testNumber;
            this.Argument = argument;
            this.Result = result;
        }
    }

    /// <summary>
    /// Represents a method which handles the event of successfully conducting
    /// a test on a <c>OneArgumentFunction&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;</c>
    /// </summary>
    /// <typeparam name="T">The type of tested function's argument.</typeparam>
    /// <typeparam name="R">The type of tested function's return value.</typeparam>
    /// <param name="source">
    /// A <c>ResultTester&lt;<typeparamref name="T"/>,<typeparamref name="R"/>&gt;</c>
    /// object that raised the event.
    /// </param>
    /// <param name="e">
    /// The event data, including the zero-based number of the test, the argument and 
    /// the return value of the function.
    /// </param>
    public delegate void TestConductedEventHandler<T, R>(object source, TestConductedEventArgs<T, R> e);

    /// <summary>
    /// This class provides means for testing return
    /// values of one-argument functions.
    /// </summary>
    public class ResultTester
    {
         
    }
}
