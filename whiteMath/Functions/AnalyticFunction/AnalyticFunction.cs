using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace whiteMath.Functions
{
    /// <summary>
    /// A function that automatically forms its action list
    /// depending on the string representation of the function.
    /// </summary>
    public partial class AnalyticFunction : Function
    {
        private string functionString;
        public string FunctionString { get { return functionString; } }

        private AnalyticFunction(List<string> actions)
            : base(actions)
        { }

        /// <summary>
        /// Creates a new analytic function object on the basis
        /// of function string like "f(x) = 5x"
        /// </summary>
        /// <param name="functionString"></param>
        public AnalyticFunction(string functionString)
        {
            string beta = functionString;           // временная
            char arg;                               // символ аргумента

            arg = CheckNPrepare(ref beta); 
            this.actions = AnalyticFunction.Analyze(beta, arg, 0); 

            this.argument = arg;
            this.functionString = functionString;
        }

        // ------------------------------------------------
        // ------------ Check and prepare the string ------
        // ------------------------------------------------

        /// <summary>
        /// Returns the letter of the argument (e.g. 'x') and prepares everything.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static char CheckNPrepare(ref string str)
        {
            // Kill all whitespace characters.
            // -
            str = str.Replace(" ", "");

            if (char.IsLetter(str, 0) && char.IsLetter(str, 1)) throw new FunctionStringSyntaxException("Only single letters are allowed for the function name (i.e. 'f').");
            if (str[1] != '(' || str[3] != ')') throw new FunctionStringSyntaxException("The function can only depend on one argument. The argument should be a single latin letter (i.e. 'x').");

            // Catch the variable.
            // -
            char argument = char.ToLower(str[2]);

            if (!char.IsLetter(argument) || !(argument >= 'a' && argument <= 'z'))
            {
                throw new FunctionStringSyntaxException("Only small latin letters can be used for the argument name.");
            }

            // Ready to work!
            // -
            str = str.Substring(5);
            str = str.Replace("@", "");

            // Insert multiplication signs where assumed: 15log(x) == 15*log(x)
            // -
            str = str.insertMultiplicationSign();

            // Find elementary functions
            // -
            str = str.Replace("abs", "@abs@");
            str = str.Replace("arcsin", "@asi@");
            str = str.Replace("arccos", "@aco@");
            str = str.Replace("arctg", "@ata@");
            str = str.Replace("sinh", "@sih@");
            str = str.Replace("cosh", "@coh@");
            str = str.Replace("sin", "@sin@");
            str = str.Replace("cos", "@cos@");
            str = str.Replace("ctg", "@ctg@");
            str = str.Replace("tg", "@tan@");
            str = str.Replace("ln", "@lna@");
            str = str.Replace("lg", "@lg1@");
            str = str.Replace("log", "@log@");
            str = str.Replace("exp", "@exp@");
            str = str.Replace("sqrt", "@sqr@");
            str = str.Replace("floor", "@flr@");
            str = str.Replace("ceil", "@cei@");
            str = str.Replace("round", "@rou@");

            // Check for brackets correctness and incorrect operation signs / uknown function names.
            // -
            int leftCount = 0, rightCount = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '@') { i += 4; continue; }

                if (char.IsSymbol(str[i]) && ("+-*/()^." + argument.ToString()).IndexOf(str[i]) == -1)
                    throw new FunctionStringSyntaxException("Syntax arror: unknown operation / argument name / function name.");

                // Insert multiplication signs where needed (old version applied for safety).
                // -
                if (i < str.Length - 1)
                    if (char.IsNumber(str[i]) && (argument.ToString() + "(@").IndexOf(str[i + 1]) != -1)
                        str = str.Insert(i + 1, "*"); // 15x == 15*x и 15(x+5) == 15*(x+5)
                
                if (str[i] == '(') { leftCount++; }
                else if (str[i] == ')') { rightCount++; }
            }

            if (leftCount > rightCount) throw new FunctionStringSyntaxException("Syntax error: not enough closing brackets ')'");
            if (leftCount < rightCount) throw new FunctionStringSyntaxException("Syntax error: not enough opening brackets '('");

            return argument;
        }
    }
}
