using System.Text.RegularExpressions;

namespace WhiteMath.Functions
{
    /// <summary>
    /// Class used for syntax analysis of function action lists.
    /// </summary>
    internal static class SyntaxAnalyzer
    {
		private static Regex actionSubstringRegex = new Regex(
			@"(?<command>[^,]+):", 
			RegexOptions.Compiled);
        
		internal static string GetActionSubstring(this string str)
        {
            return actionSubstringRegex.Match(str).Groups["command"].Value;
        }

        // ---------------------------

		private static Regex firstOperandRegex = new Regex(
			@":(?<operand>[0-9Ee\+\-\.!%$]+),?", 
			RegexOptions.Compiled);

        internal static string GetFirstOperand(this string str)
        {
            return firstOperandRegex.Match(str).Groups["operand"].Value;
        }

        // ---------------------------

		private static Regex secondOperandRegex = new Regex(
			@"[^,]*:[^,]*,(?<operand>([0-9Ee\+\-\.!%$]+|#.*#)),?", 
			RegexOptions.Compiled);

        internal static string GetSecondOperand(this string str)
        {
            return secondOperandRegex.Match(str).Groups["operand"].Value;
        }

        // ---------------------------

		private static Regex thirdOperandRegex = new Regex(
			@"[^,]*:[^,]*,([^,]*|#.*#),(?<operand>([0-9Ee\+\-\.!%$]+|#.*#))", 
			RegexOptions.Compiled);

        internal static string GetThirdOperand(this string str)
        {
            return thirdOperandRegex.Match(str).Groups["operand"].Value;
        }
    }
}
