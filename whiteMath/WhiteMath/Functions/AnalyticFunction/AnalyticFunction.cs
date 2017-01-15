using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace WhiteMath.Functions
{
    /// <summary>
    /// A function that automatically forms its action list
    /// depending on the string representation of the function.
    /// </summary>
    public partial class AnalyticFunction : ActionListFunction
    {
		/// <summary>
		/// Gets the string representation of the function.
		/// </summary>
		public string FunctionString 
		{
			get;
			private set;
		}

        /// <summary>
        /// Creates a new analytic function object on the basis
        /// of function string like "f(x) = 5x"
        /// </summary>
        public AnalyticFunction(string functionString)
        {
			string functionStringCopy = functionString;

			_argumentSymbol = Normalize(ref functionStringCopy);
			_actions = Analyze(functionStringCopy, _argumentSymbol, 0); 

            this.FunctionString = functionString;
        }

        /// <summary>
        /// Returns the letter of the argument (e.g. 'x') and normalizes the string
		/// for further syntax analysis.
        /// </summary>
		private static char Normalize(ref string functionString)
        {
            // Kill all whitespace characters.
            // -
            functionString = functionString.Replace(" ", "");

			if (char.IsLetter(functionString, 0) && char.IsLetter(functionString, 1))
			{
				throw new FunctionStringSyntaxException("Only single letters are allowed for the function name (i.e. 'f').");
			}

			if (functionString[1] != '(' || functionString[3] != ')')
			{
				throw new FunctionStringSyntaxException("The function can only depend on one argument. The argument should be a single latin letter (i.e. 'x').");
			}

            // Catch the variable.
            // -
            char argument = char.ToLower(functionString[2]);

            if (!char.IsLetter(argument) || !(argument >= 'a' && argument <= 'z'))
            {
                throw new FunctionStringSyntaxException("Only small latin letters can be used for the argument name.");
            }

            // Ready to work!
            // -
            functionString = functionString.Substring(5);
            functionString = functionString.Replace("@", "");

            // Insert multiplication signs where assumed: 15log(x) == 15*log(x)
            // -
            functionString = functionString.InsertMultiplicationSigns();

            // Find elementary functions
            // -
            functionString = functionString.Replace("abs", "@abs@");
            functionString = functionString.Replace("arcsin", "@asi@");
            functionString = functionString.Replace("arccos", "@aco@");
            functionString = functionString.Replace("arctg", "@ata@");
            functionString = functionString.Replace("sinh", "@sih@");
            functionString = functionString.Replace("cosh", "@coh@");
            functionString = functionString.Replace("sin", "@sin@");
            functionString = functionString.Replace("cos", "@cos@");
            functionString = functionString.Replace("ctg", "@cot@");
            functionString = functionString.Replace("tg", "@tan@");
            functionString = functionString.Replace("ln", "@lna@");
            functionString = functionString.Replace("lg", "@lg1@");
            functionString = functionString.Replace("log", "@log@");
            functionString = functionString.Replace("exp", "@exp@");
            functionString = functionString.Replace("sqrt", "@sqr@");
            functionString = functionString.Replace("floor", "@flr@");
            functionString = functionString.Replace("ceil", "@cei@");
            functionString = functionString.Replace("round", "@rou@");

            // Check for brackets correctness and incorrect operation signs / uknown function names.
            // -
			int leftBracketCount = 0, rightBracketCount = 0;

			for (int characterIndex = 0; characterIndex < functionString.Length; characterIndex++)
            {
                if (functionString[characterIndex] == '@') { characterIndex += 4; continue; }

				if (char.IsSymbol(functionString[characterIndex]) 
				    && $"+-*/()^.{argument}".IndexOf(functionString[characterIndex]) == -1)
				{
					throw new FunctionStringSyntaxException("Syntax arror: unknown operation / argument name / function name.");
				}

				// Insert multiplication signs where needed (old version applied for safety).
				// For example, 15x == 15*x и 15(x+5) == 15*(x+5).
				// -
				if (characterIndex < functionString.Length - 1)
				{
					if (char.IsNumber(functionString[characterIndex]) && (argument + "(@").IndexOf(functionString[characterIndex + 1]) != -1)
					{
						functionString = functionString.Insert(characterIndex + 1, "*");
					}
				}
                
                if (functionString[characterIndex] == '(') 
				{ 
					leftBracketCount++; 
				}
                else if (functionString[characterIndex] == ')') 
				{ 
					rightBracketCount++; 
				}
            }

			if (leftBracketCount > rightBracketCount)
			{
				throw new FunctionStringSyntaxException("Syntax error: not enough closing brackets ')'");
			}
			if (leftBracketCount < rightBracketCount)
			{
				throw new FunctionStringSyntaxException("Syntax error: not enough opening brackets '('");
			}

            return argument;
        }
    }
}
