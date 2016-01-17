using System;

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

		public static void ValidateNotNull<T>(T argument, string exceptionMessage = null)
		{
			Condition.Validate(argument != null).OrThrowArgumentNullException(exceptionMessage);
		}

		public class ConditionTestResult
		{
			bool Holds { get; }

			public ConditionTestResult(bool condition)
			{
				this.Holds = condition;
			}

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

			public void OrThrowArgumentNullException(string exceptionMessage = null)
			{
				if (Holds)
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

