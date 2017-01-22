using System;
using System.Collections.Generic;
using System.Linq;

namespace WhiteMath.Functions.ExpressionNodes
{
	public class ExpressionNode : IFunction<IDictionary<char, double>, double>
	{
		private readonly HashSet<char> _arguments;
		private readonly List<ExpressionNode> _childNodes;
		private Func<IDictionary<char, double>, double> _getValueFunction;

		public ExpressionNode ParentNode 
		{ 
			get;
			private set;
		}

		public IEnumerable<char> Arguments => _arguments;

		public string Expression
		{
			get;
			private set;
		}

		public IEnumerable<ExpressionNode> ChildNodes => _childNodes;

		public double GetValue(IDictionary<char, double> argumentValues = null)
		{
			argumentValues = argumentValues ?? new Dictionary<char, double>();

			return _getValueFunction(argumentValues);
		}

		private ExpressionNode(
			ExpressionNode parentNode, 
			string expression, 
			HashSet<char> arguments)
		{
			_childNodes = new List<ExpressionNode>();
			_arguments = new HashSet<char>();

			ParentNode = parentNode;
			Expression = expression;

			Parse(expression);
		}

		public ExpressionNode(string expression)
			: this(null, expression, new HashSet<char>())
		{ }

		private void Parse(string expression)
		{
			if (ParentNode == null)
			{
				ExpressionHelper.CheckBracketsAreBalanced(expression);

				expression = ExpressionHelper.RemoveAllWhitespace(expression);
				expression = ExpressionHelper.EscapeFunctionNames(expression);
				expression = ExpressionHelper.InsertMultiplicationSigns(expression);
			}

			expression = ExpressionHelper.RemoveOuterBrackets(expression);
			expression = ExpressionHelper.InsertZeroIfNeeded(expression);

			if (FindBinaryOperation(expression, '+'))
			{
				_getValueFunction = (argumentValues) 
					=> _childNodes[0].GetValue(argumentValues) 
						+ _childNodes[1].GetValue(argumentValues);
			}
			else if (FindBinaryOperation(expression, '-'))
			{
				_getValueFunction = (argumentValues)
					=> _childNodes[0].GetValue(argumentValues)
						- _childNodes[1].GetValue(argumentValues);
			}
			else if (FindBinaryOperation(expression, '*'))
			{
				_getValueFunction = (argumentValues)
					=> _childNodes[0].GetValue(argumentValues)
						* _childNodes[1].GetValue(argumentValues);
			}
			else if (FindBinaryOperation(expression, '/'))
			{
				_getValueFunction = (argumentValues)
					=> _childNodes[0].GetValue(argumentValues)
						/ _childNodes[1].GetValue(argumentValues);
			}
			else if (FindBinaryOperation(expression, '^'))
			{
				_getValueFunction = (argumentValues) => Math.Pow(
					_childNodes[0].GetValue(argumentValues), 
					_childNodes[1].GetValue(argumentValues));
			}
			else if ((_getValueFunction = FindFunction(expression)) != null)
			{
				return;
			}
			else if (expression.Length == 1 && char.IsLetter(expression.First()))
			{
				char argumentSymbol = expression.Single();

				_arguments.Add(argumentSymbol);

				_getValueFunction = (argumentValues) =>
				{
					double argumentValue;

					if (!argumentValues.TryGetValue(argumentSymbol, out argumentValue))
					{
						throw new Exception($"Cannot evaluate expression '{expression}' because no value for argument '{argumentSymbol}' has been provided.");
					}

					return argumentValue;
				};
			}
			else
			{
				double numericValue;

				if (!double.TryParse(expression, out numericValue))
				{
					throw new Exception($"Unable to parse expression '{expression}': the expression is not well-formed.");
				}

				_getValueFunction = (argumentValues) => numericValue;
			}
		}

		private bool FindBinaryOperation(string expression, char operationSign)
		{
			int indexOfOperationSign = 
				ExpressionHelper.IndexOfCharacterNotInsideBrackets(expression, operationSign);

			if (indexOfOperationSign >= 0)
			{
				Tuple<string, string> splitResult =
					ExpressionHelper.SplitOnIndex(expression, indexOfOperationSign);

				_childNodes.Add(new ExpressionNode(this, splitResult.Item1, _arguments));
				_childNodes.Add(new ExpressionNode(this, splitResult.Item2, _arguments));

				return true;
			}

			return false;
		}

		private Func<IDictionary<char, double>, double> FindFunction(string expression)
		{
			string functionName;
			string functionArgumentsExpression;

			bool isFunctionExpression = ExpressionHelper.IsFunctionExpression(
				expression, 
				out functionName, 
				out functionArgumentsExpression);

			if (!isFunctionExpression) return null;

			IList<string> functionArguments = 
				ExpressionHelper.GetFunctionArguments(functionArgumentsExpression);

			if (functionArguments.Count == 1)
			{
				Func<double, double> unaryFunction;

				if (!ExpressionHelper.SupportedUnaryFunctions.TryGetValue(
					functionName,
					out unaryFunction))
				{
					throw new ArgumentException($"There is no supported unary function '{functionName}'.");
				}

				_childNodes.Add(new ExpressionNode(this, functionArguments[0], _arguments));

				return (argumentValues) 
					=> unaryFunction(_childNodes[0].GetValue(argumentValues));
			}
			else if (functionArguments.Count == 2)
			{
				Func<double, double, double> binaryFunction;

				if (!ExpressionHelper.SupportedBinaryFunctions.TryGetValue(
					functionName,
					out binaryFunction))
				{
					throw new ArgumentException($"There is no supported binary function '{functionName}'.");
				}

				_childNodes.Add(new ExpressionNode(this, functionArguments[0], _arguments));
				_childNodes.Add(new ExpressionNode(this, functionArguments[1], _arguments));

				return (argumentValues) => binaryFunction(
					_childNodes[0].GetValue(argumentValues),
					_childNodes[1].GetValue(argumentValues));
			}
			else
			{
				throw new ArgumentException($"There is no supported function '{functionName}' accepting {functionArguments.Count} arguments.");
			}
		}
	}
}
