using System;
using System.Collections.Generic;
using System.Linq;

namespace WhiteStructs.Conditions
{
	/// <summary>
	/// Provides basic functionality for method pre-condition checking.
	/// <example>
	/// Condition.Validate(x >= 0).OrThrowArgumentException("Argument is negative");
	/// </example>
	/// <example>
	/// Condition.ValidateNotNull(x);
	/// </example>
	/// </summary>
	public static class Condition
	{
		public sealed class ValidationException: Exception { }

		public static ConditionTestResult Validate(bool condition)
		{
			return new ConditionTestResult(condition);
		}

		/// <summary>
		/// Validates that a given value is not <c>null</c>,
		/// or throws a <see cref="ArgumentNullException"/>.
		/// </summary>
		/// <param name="argument">A value to test for <c>null</c>.</param>
		/// <param name="exceptionMessage">An optional exception message.</param>
		public static void ValidateNotNull<T>(T argument, string exceptionMessage = null)
		{
			Condition.Validate(argument != null).OrArgumentNullException(exceptionMessage);
		}

		/// <summary>
		/// Validates that a given value is positive, or throws an 
		/// <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
		/// <param name="argument">A value to test for being positive.</param>
		/// <param name="exceptionMessage">An optional exception message.</param>
		public static void ValidatePositive(long argument, string exceptionMessage = null)
		{
			Condition
				.Validate(argument > 0)
				.OrArgumentOutOfRangeException(exceptionMessage);
		}

		/// <summary>
		/// Validates that a given value is non-negative, or throws an
		/// <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
		/// <param name="argument">A value to test for being non-negative.</param>
		/// <param name="exceptionMessage">An optional exception message.</param>
		public static void ValidateNonNegative(long argument, string exceptionMessage = null)
		{
			Condition
				.Validate(argument >= 0)
				.OrArgumentOutOfRangeException(exceptionMessage);
		}

		/// <summary>
		/// Validates that a given sequence is not empty, or throws
		/// an <see cref="ArgumentException"/>. Does not test that 
		/// the sequence is not <c>null</c>.
		/// </summary>
		/// <param name="sequence">The sequence to test for.</param>
		/// <param name="exceptionMessage">An optional exception message.</param>
		public static void ValidateNotEmpty<T>(IEnumerable<T> sequence, string exceptionMessage = null)
		{
			Condition.Validate(sequence.Any()).OrArgumentException(exceptionMessage);
		}

		public class ConditionTestResult
		{
			/// <summary>
			/// Gets a value indicating whether this 
			/// <see cref="ConditionTestResult"/> holds.
			/// </summary>
			/// <value><c>true</c> if the condition holds; otherwise, <c>false</c>.</value>
			public bool Holds { get; }

			internal ConditionTestResult(bool condition)
			{
				this.Holds = condition;
			}

			/// <summary>
			/// Throws an <see cref="ArgumentException"/> if the condition doesn't hold.
			/// </summary>
			/// <param name="exceptionMessage">An optional exception message.</param>
			public void OrArgumentException(string exceptionMessage = null)
			{
				if (this.Holds)
				{
					return;
				}

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
			/// Throws an <see cref="ArgumentNullException"/> if the condition doesn't hold.
			/// </summary>
			/// <param name="exceptionMessage">An optional exception message.</param>
			public void OrArgumentNullException(string exceptionMessage = null)
			{
				if (this.Holds)
				{
					return;
				}

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
			/// Throws an <see cref="ArgumentOutOfRangeException"/> if the 
			/// condition doesn't hold.
			/// </summary>
			/// <param name="exceptionMessage">An optional exception message.</param>
			public void OrArgumentOutOfRangeException(string exceptionMessage = null)
			{
				if (this.Holds)
				{
					return;
				}

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
			public void OrIndexOutOfRangeException(string exceptionMessage = null)
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

			/// <summary>
			/// Throws a given <see cref="Exception"/> if the 
			/// condition doesn't hold.
			/// </summary>
			/// <param name="exception">The exception to throw.</param>
			public void OrException(Exception exception = null)
			{
				exception = exception ?? new ValidationException();

				if (this.Holds)
				{
					return;
				}
				else
				{
					throw exception;
				}
			}
		}
	}
}

