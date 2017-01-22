using System;
using System.Collections.Generic;

using NUnit.Framework;

using WhiteMath.Functions.ExpressionNodes;

namespace WhiteMathTests
{
	[TestFixture]
	public class UnitTests
	{
		[Test]
		[TestCase("5 + 3 = 0", "5+3=0")]
		[TestCase(" 25x + (((x))) - 6 = 0   ", "25x+(((x)))-6=0")]
		[TestCase("    37 + 2. 01237", "37+2.01237")]
		public void Test_RemoveAllWhitespace_RemovesAllWhitespace(
			string expression, string expectedResult)
		{
			Assert.That(ExpressionHelper.RemoveAllWhitespace(expression) == expectedResult);
		}

		[Test]
		[TestCase(")(")]
		[TestCase("((())")]
		[TestCase("(()))")]
		public void Test_BracketsAreBalanced_ThrowsException_When_BracketsNotBalanced(
			string expression)
		{
			Assert.Throws<ArgumentException>(
				() => ExpressionHelper.CheckBracketsAreBalanced(expression));
		}

		[Test]
		[TestCase("()")]
		[TestCase("")]
		[TestCase("(3x)+2.014-6((5)+ 4)")]
		public void Test_BracketsAreBalanced_DoesNotThrowException_When_BracketsAreBalanced(
			string expression)
		{
			Assert.DoesNotThrow(
				() => ExpressionHelper.CheckBracketsAreBalanced(expression));
		}

		[Test]
		[TestCase("5x + 6 = 0")]
		[TestCase("0 = 12 - 28x + 4")]
		[TestCase("5 = 5")]
		public void Test_IsEquationMode_ReturnsTrue_When_EquationSignIsPresent(
			string expression)
		{
			Assert.True(ExpressionHelper.IsEquationMode(expression));
		}

		[Test]
		[TestCase("28+4728")]
		[TestCase("")]
		[TestCase("999 - 48(123x + 4)")]
		public void Test_IsEquationMode_ReturnsFalse_When_EquationSignNotPresent(
			string expression)
		{
			Assert.False(ExpressionHelper.IsEquationMode(expression));
		}

		[Test]
		[TestCase("3=2x=0")]
		[TestCase("=1238=x")]
		public void Test_IsEquationMode_ThrowsException_When_TooManyEquationSigns(
			string expression)
		{
			Assert.Throws<ArgumentException>(() => ExpressionHelper.IsEquationMode(expression));
		}

		[Test]
		[TestCase("3=-2x+5", "3-(-2x+5)")]
		[TestCase("82*5721=0", "82*5721-(0)")]
		[TestCase("17x-x*x=8", "17x-x*x-(8)")]
		public void Test_NormalizeEquation_MovesEverythingToTheLeftAndRemovesEquationSign(
			string expression, string expectedResult)
		{
			Assert.That(ExpressionHelper.NormalizeEquation(expression) == expectedResult);
		}

		[Test]
		[TestCase("=123")]
		[TestCase("5x+3=")]
		public void Test_NormalizeEquation_ThrowsException_When_NothingToLeftOrRightOfEquationSign(
			string expression)
		{
			Assert.Throws<ArgumentException>(() => ExpressionHelper.NormalizeEquation(expression));
		}

		[Test]
		public void Test_InsertMultiplicationSigns_DoesNotInsertMultiplicationSignInsideEscapedStrings()
		{
			Assert.That(
				ExpressionHelper.InsertMultiplicationSigns("@abs@(x)"),
				Is.EqualTo("@abs@(x)"));
		}

		[Test]
		public void Test_InsertMultiplicationSigns_InsertsMultiplicationSignBetweenBracketedGroups()
		{
			Assert.That(
				ExpressionHelper.InsertMultiplicationSigns("(1+22)((-3)+4)"),
				Is.EqualTo("(1+22)*((-3)+4)"));
		}

		[Test]
		public void Test_InsertMultiplicationSigns_InsertsMultiplicationSignBetweenLetters()
		{
			Assert.That(
				ExpressionHelper.InsertMultiplicationSigns("abc"),
				Is.EqualTo("a*b*c"));
		}

		[Test]
		public void Test_InsertMultiplicationSigns_InsertsMultiplicationSignBetweenNumberAndLetter()
		{
			Assert.That(
				ExpressionHelper.InsertMultiplicationSigns("18.52x+3y"),
				Is.EqualTo("18.52*x+3*y"));
		}

		[Test]
		public void Test_InsertMultiplicationSigns_InsertsMultiplicationSignBetweenNumberAndFunction()
		{
			Assert.That(
				ExpressionHelper.InsertMultiplicationSigns("12@sin@(3x)"),
				Is.EqualTo("12*@sin@(3*x)"));
		}

		[Test]
		[TestCase("(((3x+6)))")]
		[TestCase("((3x+2)+(2x+6))")]
		[TestCase("()")]
		public void Test_HasOuterBrackets_ReturnsTrue_When_ExpressionHasOuterBrackets(
			string expression)
		{
			Assert.That(ExpressionHelper.HasOuterBrackets(expression) == true);
		}

		[Test]
		[TestCase("")]
		[TestCase("(3x)+(2x)")]
		[TestCase("abc")]
		public void Test_HasOuterBrackets_ReturnsFalse_When_ExpressionHasNoOuterBrackets(
			string expression)
		{
			Assert.That(ExpressionHelper.HasOuterBrackets(expression) == false);
		}

		[Test]
		[TestCase("(((((3x+6)))))", "3x+6")]
		[TestCase("(82-@sin@(x))", "82-@sin@(x)")]
		public void Test_RemoveOuterBrackets_RemovesOuterBrackets(
			string expression, string expectedResult)
		{
			Assert.That(
				ExpressionHelper.RemoveOuterBrackets(expression),
				Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase("(3x)+(6x)", "(3x)+(6x)")]
		[TestCase("(((sin(x))+(-8)))", "(sin(x))+(-8)")]
		public void Test_RemoveOuterBrackets_DoesNotRemoveBracketsThatAreNotOuter(
			string expression, string expectedResult)
		{
			Assert.That(
				ExpressionHelper.RemoveOuterBrackets(expression),
				Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase("1+2", 1)]
		[TestCase("(1+2)", -1)]
		[TestCase("(1+2)+3)", 5)]
		public void Test_IndexOfCharacterNotInsideBrackets_ReturnsIndexOfCharacterNotInsideBrackets(
			string expression, int expectedResult)
		{
			Assert.That(
				ExpressionHelper.IndexOfCharacterNotInsideBrackets(expression, '+'),
				Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase("abc", 1, "a", "c")]
		[TestCase("ab", 1, "a", "")]
		[TestCase("ab", 0, "", "b")]
		[TestCase("a", 0, "", "")]
		public void Test_SplitOnIndex_SplitsStringOnSpecifiedIndex(
			string expression, int index, string expectedFirstPart, string expectedSecondPart)
		{
			Tuple<string, string> splitResult = ExpressionHelper.SplitOnIndex(expression, index);

			Assert.That(splitResult.Item1, Is.EqualTo(expectedFirstPart));
			Assert.That(splitResult.Item2, Is.EqualTo(expectedSecondPart));
		}

		[Test]
		[TestCase("-1", "0-1")]
		[TestCase("-(-2+x)", "0-(-2+x)")]
		[TestCase("-x+3", "0-x+3")]
		public void Test_InsertZeroIfNeeded_InsertsZero_When_ExpressionStartsWithMinus(
			string expression, string expectedResult)
		{
			Assert.That(
				ExpressionHelper.InsertZeroIfNeeded(expression),
				Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase("1", "1")]
		[TestCase("x-3", "x-3")]
		public void Test_InsertZeroIfNeeded_DoesNotInsertZero_When_ExpressionDoesNotStartWithMinus(
			string expression, string expectedResult)
		{
			Assert.That(
				ExpressionHelper.InsertZeroIfNeeded(expression),
				Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase("@sin@(3x+2)")]
		[TestCase("@pow@(2,36)")]
		public void Test_IsFunctionExpression_ReturnsTrue_When_IsFunctionExpression(
			string expression)
		{
			Assert.That(
				ExpressionHelper.IsFunctionExpression(expression), 
				Is.EqualTo(true));
		}

		[Test]
		[TestCase("@sin@x")]
		[TestCase("sin(x)")]
		[TestCase("")]
		[TestCase("-1")]
		[TestCase("x+y")]
		public void Test_IsFunctionExpression_ReturnsFalse_When_IsNotFunctionExpression(
			string expression)
		{
			Assert.That(
				ExpressionHelper.IsFunctionExpression(expression),
				Is.EqualTo(false));
		}

		[Test]
		[TestCase("@sin@(x),((x))", new string[] { "@sin@(x)", "x" })]
		[TestCase("((@sin@(3x+2)))", new string[] { "@sin@(3x+2)" })]
		[TestCase("a,((b)),(c),(@cos@((d)))", new string[] { "a", "b", "c", "@cos@((d))" })]
		[TestCase("(a,b,c)", new string[] { "a", "b", "c" })]
		public void Test_GetFunctionArguments_ReturnsExpectedArguments(
			string expression, IEnumerable<string> expectedArguments)
		{
			CollectionAssert.AreEqual(
				ExpressionHelper.GetFunctionArguments(expression),
				expectedArguments);
		}

		[Test]
		public void Test_ExpressionNode_ThrowsException_When_NotAllVariablesAreBound()
		{
			ExpressionNode expressionNode = new ExpressionNode("2x - 3y + 3");

			Assert.Throws<Exception>(() => expressionNode.GetValue(
				new Dictionary<char, double> { { 'x', 3 } }));
		}

		[Test]
		public void Test_ExpressionNode_EvaluatesExpressions_When_VariablesAreBound()
		{
			ExpressionNode expressionNode = new ExpressionNode("2x - 3y + 3");

			Assert.That(
				expressionNode.GetValue(new Dictionary<char, double>
				{
					{ 'x', 3 },
					{ 'y', 4 },
				}), 
				Is.EqualTo(-3));
		}

		[Test]
		[TestCase("3(1-2*4)+abs(-2)", -19)]
		[TestCase("(sin(0.5))^2 + (cos(0.5))^2", 1)]
		[TestCase("3*6-2+4", 20)]
		[TestCase("(((3-4)+25)/6)*4", 16)]
		[TestCase("(((3-4)+25)/6)*4+(10-2^2)", 22)] 
		public void Test_ExpressionNode_EvaluatesExpressionsCorrectly(
			string expression, double result)
		{
			Assert.That(
				new ExpressionNode(expression).GetValue(),
				Is.EqualTo(result));
		}
	}
}
