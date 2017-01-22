using System;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace WhiteMath.Functions.ExpressionNodes
{
	internal static class ExpressionHelper
	{
		/// <summary>
		/// Removes all whitespace charactes in the expression
		/// string to ease parsing.
		/// </summary>
		public static string RemoveAllWhitespace(string expression)
			=> Regex.Replace(expression, @"\s", "");

		/// <summary>
		/// Checks that opening and closing brackets in the expression are balanced.
		/// </summary>
		public static void CheckBracketsAreBalanced(
			string expression, 
			char leftBracket = '(',
			char rightBracket = ')')
		{
			int bracketsBalance = 0;

			const string ERROR_STRING = 
				"The brackets inside the expression are not balanced.";

			foreach (char character in expression)
			{
				if (character == leftBracket)
				{
					++bracketsBalance;
				}

				if (character == rightBracket)
				{
					--bracketsBalance;
				}

				if (bracketsBalance < 0)
				{
					throw new ArgumentException(ERROR_STRING + " You might have missed a '('.");
				}
			}

			if (bracketsBalance != 0)
			{
				throw new ArgumentException(ERROR_STRING + " You might have missed a ')'.");
			}
		}

		public static bool IsEquationMode(string expression)
		{
			int equationSignCount = expression.Count(x => x == '=');

			if (equationSignCount == 0)
			{
				return false;
			}

			if (equationSignCount == 1)
			{
				return true;
			}

			throw new ArgumentException("There can only be one equation sign '=' in the expression.");
		}

		/// <summary>
		/// Normalizes the equation expression - gets rid of the equation sign,
		/// transferring everything to the right of the equation sign to the left.
		/// </summary>
		public static string NormalizeEquation(string expression)
		{
			if (expression.StartsWith("=", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("There is nothing to the left of the equation sign.");
			}

			if (expression.EndsWith("=", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("There is nothing to the right of the equation sign.");
			}

			return Regex.Replace(expression, "^(.+)=(.+)$", "$1-($2)");
		}

		public static readonly Dictionary<string, Func<double, double>> SupportedUnaryFunctions =
			new Dictionary<string, Func<double, double>>()
		{
			{ "abs", Math.Abs },
			{ "sign", x =>
				x > 0
					? 1
					: x <= double.Epsilon
						? 0
						: -1 },
			{ "exp", Math.Exp },
			{ "log", Math.Log },
			{ "ln", Math.Log },
			{ "lg", x => Math.Log(x, 10) },
			{ "round", Math.Round },
			{ "sin", Math.Sin },
			{ "cos", Math.Cos },
			{ "sinh", Math.Sinh },
			{ "cosh", Math.Cosh },
			{ "tg", Math.Tan },
			{ "tan", Math.Tan },
			{ "tgh", Math.Tanh },
			{ "tanh", Math.Tanh },
			{ "arcsin", Math.Asin },
			{ "asin", Math.Asin },
			{ "arccos", Math.Acos },
			{ "acos", Math.Acos },
			{ "arctan", Math.Atan },
			{ "arctg", Math.Atan },
			{ "ceil", Math.Ceiling },
			{ "floor", Math.Floor },
			{ "sqrt", Math.Sqrt },
		};

		public static readonly Dictionary<string, Func<double, double, double>> SupportedBinaryFunctions =
			new Dictionary<string, Func<double, double, double>>()
		{
			{ "log", Math.Log },
			{ "pow", Math.Pow },
		};

		public static string EscapeFunctionNames(string expression)
		{
			StringBuilder result = new StringBuilder(expression);

			foreach (string function 
				in SupportedUnaryFunctions.Keys.Concat(SupportedBinaryFunctions.Keys))
			{
				result = result.Replace(function, $"@{function}@");
			}

			return result.ToString();
		}

		public static string InsertMultiplicationSigns(string expression)
		{
			if (expression.Length < 2)
			{
				return expression;
			}

			StringBuilder result = new StringBuilder(expression);

			bool isEscapedMode = false;

			for (int index = 0; index < result.Length - 1; ++index)
			{
				char currentCharacter = result[index];
				char nextCharacter = result[index + 1];

				// Nothing is changed inside the escaped
				// characted sequences - it means we're
				// inside a function name.
				// -
				if (currentCharacter == '@')
				{
					isEscapedMode = !isEscapedMode;
					continue;
				}

				if (isEscapedMode) continue;

				if ((
						char.IsDigit(currentCharacter)
						|| char.IsLetter(currentCharacter)
						|| currentCharacter == ')')
					&& (
						nextCharacter == '@'
						|| char.IsLetter(nextCharacter)
						|| nextCharacter == '('))
				{
					result.Insert(index + 1, '*');
				}
			}

			return result.ToString();
		}

		public static bool HasOuterBrackets(
			string expression,
			char leftBracket = '(',
			char rightBracket = ')')
		{
			if (expression.Length < 2
				|| expression[0] != leftBracket
				|| expression[expression.Length - 1] != rightBracket)
			{
				return false;
			}

			int bracketBalance = 0;

			for (int index = 0; index < expression.Length - 1; ++index)
			{
				if (expression[index] == leftBracket)
				{
					++bracketBalance;
				}
				else if (expression[index] == rightBracket)
				{
					--bracketBalance;
				}

				// If bracket balance reaches zero before the final parenthesis,
				// it means that the overall expression is something like
				// (3x+6)+(2), which has no outer parentheses.
				// -
				if (bracketBalance <= 0)
				{
					return false;
				}
			}

			return bracketBalance == 1;
		}

		public static string RemoveOuterBrackets(
			string expression,
			char leftBracket = '(',
			char rightBracket = ')')
		{
			while (HasOuterBrackets(expression, leftBracket, rightBracket))
			{
				expression = expression.Substring(1, expression.Length - 2);
			}

			return expression;
		}

		public static int IndexOfCharacterNotInsideBrackets(
			string expression,
			char character,
			char leftBracket = '(',
			char rightBracket = ')')
		{
			int bracketsBalance = 0;

			for (int index = 0; index < expression.Length; ++index)
			{
				char currentCharacter = expression[index];

				if (currentCharacter == leftBracket)
				{
					++bracketsBalance;
				}
				else if (currentCharacter == rightBracket)
				{
					--bracketsBalance;
				}

				if (bracketsBalance == 0 && currentCharacter == character)
				{
					return index;
				}
			}

			return -1;
		}

		public static string InsertZeroIfNeeded(string expression)
		{
			if (expression.StartsWith("-", StringComparison.OrdinalIgnoreCase))
			{
				return $"0{expression}";
			}

			return expression;
		}

		public static Tuple<string, string> SplitOnIndex(string expression, int index)
		{
			if (index < 0 || index >= expression.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			return Tuple.Create(
				expression.Substring(0, index),
				expression.Substring(index + 1));
		}

		public static bool IsFunctionExpression(string expression)
		{
			string _;
			return IsFunctionExpression(expression, out _, out _);
		}

		public static bool IsFunctionExpression(
			string expression, 
			out string functionName,
			out string functionArgument)
		{
			Regex functionExpressionRegex = new Regex(@"^@(\w+)@(\(.*\))$");

			Match match = functionExpressionRegex.Match(expression);

			if (expression.Length >= 2
				&& match.Success
				&& HasOuterBrackets(match.Groups[2].ToString()))
			{
				functionName = match.Groups[1].ToString();
				functionArgument = match.Groups[2].ToString();

				return true;
			}
			else
			{
				functionName = null;
				functionArgument = null;

				return false;
			}
		}

		public static IList<string> GetFunctionArguments(
			string functionArgumentExpression)
		{
			List<string> result = new List<string>();

			while (true)
			{
				functionArgumentExpression =
					RemoveOuterBrackets(functionArgumentExpression);

				int indexOfArgumentSeparator = IndexOfCharacterNotInsideBrackets(
					functionArgumentExpression,
					',');

				if (indexOfArgumentSeparator < 0)
				{
					result.Add(functionArgumentExpression);
					break;
				}

				Tuple<string, string> arguments =
					SplitOnIndex(functionArgumentExpression, indexOfArgumentSeparator);

				result.Add(RemoveOuterBrackets(arguments.Item1));

				functionArgumentExpression = arguments.Item2;
			}

			return result;
		}
	}
}
