﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace whiteStructs.Conditions
{
	/// <summary>
	/// Provides basic functionality for method pre-condition checking.
	/// <example>
	/// Condition.Requires(x >= 0).OrThrowArgumentException("Argument is negative");
	/// </example>
	/// <example>
	/// Condition.RequiresNotNull(x);
	/// </example>
	/// </summary>
	public static class Condition
	{
		public static ConditionTestResult Validate(bool condition)
		{
			return new ConditionTestResult(condition);
		}

		/// <summary>
		/// Validates that a given value is not <c>null</c>,
		/// or throws a <see cref="System.ArgumentNullException"/>.
		/// </summary>
		/// <param name="argument">A value to test for <c>null</c>.</param>
		/// <param name="exceptionMessage">An optional exception message.</param>
		public static void ValidateNotNull<T>(T argument, string exceptionMessage = null)
		{
			Condition.Validate(argument != null).OrThrowArgumentNullException(exceptionMessage);
		}

		/// <summary>
		/// Validates that a given sequence is not empty, or throws
		/// a <see cref="System.ArgumentException"/>. Does not test that 
		/// the sequence is not <c>null</c>.
		/// </summary>
		/// <param name="sequence">The sequence to test for.</param>
		/// <param name="exceptionMessage">An optional exception message.</param>
		public static void ValidateNotEmpty<T>(IEnumerable<T> sequence, string exceptionMessage = null)
		{
			Condition.Validate(sequence.Any()).OrThrowArgumentException(exceptionMessage);
		}

		public class ConditionTestResult
		{
			/// <summary>
			/// Gets a value indicating whether this <see cref="whiteStructs.Conditions.Condition+ConditionTestResult"/> holds.
			/// </summary>
			/// <value><c>true</c> if the condition holds; otherwise, <c>false</c>.</value>
			public bool Holds { get; }

			internal ConditionTestResult(bool condition)
			{
				this.Holds = condition;
			}

			/// <summary>
			/// Throws a <see cref="System.ArgumentException"/> if the condition doesn't hold.
			/// </summary>
			/// <param name="exceptionMessage">An optional exception message.</param>
			public void OrThrowArgumentException(string exceptionMessage = null)
			{
				if (Holds)
					return;

				if (exceptionMessage != null)
				{
					throw new ArgumentException(exceptionMessage);
				}
				else
				{
					throw new ArgumentException();
				}
			}

			/// <summary>
			/// Throws a <see cref="System.ArgumentNullException"/> if the condition doesn't hold.
			/// </summary>
			/// <param name="exceptionMessage">An optional exception message.</param>
			public void OrThrowArgumentNullException(string exceptionMessage = null)
			{
				if (this.Holds)
					return;

				if (exceptionMessage != null)
				{
					throw new ArgumentNullException(exceptionMessage);
				}
				else
				{
					throw new ArgumentNullException();
				}
			}

			/// <summary>
			/// Throws a <see cref="System.ArgumentOutOfRangeException"/> if the 
			/// condition doesn't hold.
			/// </summary>
			/// <param name="exceptionMessage">An optional exception message.</param>
			public void OrThrowArgumentOutOfRangeException(string exceptionMessage = null)
			{
				if (Holds)
					return;

				if (exceptionMessage != null)
				{
					throw new ArgumentOutOfRangeException(exceptionMessage);
				}
				else
				{
					throw new ArgumentOutOfRangeException();
				}
			}

			/// <summary>
			/// Throws a <see cref="System.IndexOutOfRangeException"/> if the 
			/// condition doesn't hold.
			/// </summary>
			/// <param name="exceptionMessage">An optional exception message.</param>
			public void OrThrowIndexOutOfRangeException(string exceptionMessage = null)
			{
				if (Holds)
					return;

				if (exceptionMessage != null)
				{
					throw new IndexOutOfRangeException(exceptionMessage);
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
		}
	}
}
