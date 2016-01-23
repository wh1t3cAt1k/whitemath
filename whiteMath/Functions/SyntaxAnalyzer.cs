using System.Text.RegularExpressions;

namespace whiteMath.Functions
{
    /// <summary>
    /// Class used for syntax analysis of function action lists.
    /// </summary>
    public static class SyntaxAnalyzer
    {
        static Regex instruction = new Regex(@"(?<command>[^,]+):", RegexOptions.None);
        
        public static string getActionSubString(this string str)
        {
            return instruction.Match(str).Groups["command"].Value;
        }

        // ---------------------------

        static Regex firstOp = new Regex(@":(?<operand>[0-9Ee\+\-\.!%$]+),?", RegexOptions.None);

        public static string getFirstOperand(this string str)
        {
            return firstOp.Match(str).Groups["operand"].Value;
        }

        // ---------------------------

        static Regex secondOp = new Regex(@"[^,]*:[^,]*,(?<operand>([0-9Ee\+\-\.!%$]+|#.*#)),?", RegexOptions.None);

        public static string getSecondOperand(this string str)
        {
            return secondOp.Match(str).Groups["operand"].Value;
        }

        // ---------------------------

        static Regex thirdOp = new Regex(@"[^,]*:[^,]*,([^,]*|#.*#),(?<operand>([0-9Ee\+\-\.!%$]+|#.*#))", RegexOptions.None);

        public static string getThirdOperand(this string str)
        {
            return thirdOp.Match(str).Groups["operand"].Value;
        }
    }
}
