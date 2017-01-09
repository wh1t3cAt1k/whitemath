using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WhiteMath.Functions
{
    public partial class AnalyticFunction
    {
        /// <summary>
        /// Returns true if the derivative analytic function can be calculated,
        /// else otherwise. For example, the function "sign(x)" does not have
        /// any analytic function derivative formula.
        /// </summary>
        public bool HasDerivative
        {
            get
            {
                string str = this.functionString;
                return (!str.Contains("sign") && !str.Contains("abs") && !str.Contains("floor") && !str.Contains("ceil"));
            }
        }


        /// <summary>
        /// Returns the derivative of the current analytic function.
        /// </summary>
        /// <returns></returns>
        public AnalyticFunction Derivative()
        {
            if (!this.HasDerivative)
                throw new FunctionBadArgumentException("No AnalyticFunction derivative formula can be calculated for this function.");

            Regex remainderFinder = new Regex("(?<firstPart>.*=)(?<remainder>.*)");
            Match match = remainderFinder.Match(this.functionString.Replace(" ", ""));
            
            string analyzeRemainder = match.Groups["remainder"].Value;

            // handle the *'s.
            analyzeRemainder = analyzeRemainder.InsertMultiplicationSigns();

            return new AnalyticFunction(match.Groups["firstPart"].Value + FindDerivative(analyzeRemainder, this._argumentSymbol).ParenthesizeIfNegative());
        }

        // ------------------------------------------
        // --------------- REGEXES ------------------
        // ------------------------------------------

        static string argPattern = @"([a-zA-Z]+\(.*\))|(\(.*\))|([^+\(\)]+)";

        static Regex plusminus = new Regex(string.Format(@"(?<firstArgument>{0})(?<sign>(\+|-))(?<secondArgument>.+)", argPattern), RegexOptions.Compiled);
        static Regex multiply = new Regex(string.Format(@"(?<firstArgument>{0})\*(?<secondArgument>.+)", argPattern), RegexOptions.Compiled);
        static Regex divide = new Regex(string.Format(@"(?<firstArgument>{0})\/(?<secondArgument>.+)", argPattern), RegexOptions.Compiled);
        static Regex power = new Regex(string.Format(@"(?<firstArgument>{0})\^(?<secondArgument>.+)", argPattern), RegexOptions.Compiled);

        // ------------------------------------------------------
        // --------------- MAIN BOSSY FUNCTION ------------------
        // ------------------------------------------------------

        // рекурсивный метод для нахождения строки производной функции

        private string FindDerivative(string functionString, char argument) 
        {
            // убираем внешние скобки

            functionString = functionString.withoutOuterParentheses();
            
            // ----------------------

            if (!functionString.Contains(argument))
                return "0";
            else if (functionString == argument.ToString())
                return "1";

            // - sinus

            Match sin = Regex.Match(functionString, @"sin\((?<inner>.*)\)");

            if (isSuccessfulFunctionMatch(sin, functionString))
                return string.Format("{0}*cos({1})", FindDerivative(sin.Groups["inner"].Value, argument).ParenthesizeIfNegative(), sin.Groups["inner"].Value);

            Match cos = Regex.Match(functionString, @"cos\((?<inner>.*)\)");

            if (isSuccessfulFunctionMatch(cos, functionString))
                return string.Format("{0}*(-sin({1}))", FindDerivative(cos.Groups["inner"].Value, argument).ParenthesizeIfNegative(), cos.Groups["inner"].Value);

            Match tg = Regex.Match(functionString, @"tg\((?<inner>.*)\)");

            if (isSuccessfulFunctionMatch(tg, functionString))
                return string.Format("{0}/((cos({1})^2)", FindDerivative(tg.Groups["inner"].Value, argument).ParenthesizeIfNegative(), tg.Groups["inner"].Value);

            Match ctg = Regex.Match(functionString, @"ctg\((?<inner>.*)\)");

            if (isSuccessfulFunctionMatch(ctg, functionString))
                return string.Format("(-{0})/((sin({1})^2)", FindDerivative(ctg.Groups["inner"].Value, argument).ParenthesizeIfNegative(), ctg.Groups["inner"].Value);

            Match arcsin = Regex.Match(functionString, @"arcsin\((?<inner>.*)\)");

            if (isSuccessfulFunctionMatch(arcsin, functionString))
                return string.Format("{0}/sqrt(1-({1})^2)", FindDerivative(arcsin.Groups["inner"].Value, argument).ParenthesizeIfNegative(), arcsin.Groups["inner"].Value);

            Match arccos = Regex.Match(functionString, @"arccos\((?<inner>.*)\)");

            if (isSuccessfulFunctionMatch(arccos, functionString))
                return string.Format("-{0}/sqrt(1-({1})^2)", FindDerivative(arccos.Groups["inner"].Value, argument).ParenthesizeIfNegative(), arccos.Groups["inner"].Value);

            Match arctg = Regex.Match(functionString, @"arctg\((?<inner>.*)\)");

            if (isSuccessfulFunctionMatch(arctg, functionString))
                return string.Format("{0}/(1+({1})^2)", FindDerivative(arctg.Groups["inner"].Value, argument).ParenthesizeIfNegative(), arctg.Groups["inner"].Value);
            
            Match exp = Regex.Match(functionString, @"exp\((?<inner>.*)\)");

            if (isSuccessfulFunctionMatch(exp, functionString))
                return string.Format("{0}*exp({1})", FindDerivative(exp.Groups["inner"].Value, argument).ParenthesizeIfNegative(), exp.Groups["inner"].Value);

            Match ln = Regex.Match(functionString, @"ln\((?<inner>.*)\)");

            if (isSuccessfulFunctionMatch(ln, functionString))
                return string.Format("{0}/{1}", FindDerivative(ln.Groups["inner"].Value, argument).ParenthesizeIfNegative(), ln.Groups["inner"].Value.ParenthesizeIfNegative());

            Match log = Regex.Match(functionString, string.Format(@"log\((?<first>{0}),(?<second>{0})\)", argPattern));

            if (isSuccessfulFunctionMatch(log, functionString))
                return FindDerivative(string.Format("ln({0})/ln({1})", log.Groups["first"].Value, log.Groups["second"].Value), argument);

            // no functions here.

            Match current = plusminus.Match(functionString);

            if (isSuccessfulBinaryMatch(current))
            {
                string first = current.Groups["firstArgument"].Value;
                string second = current.Groups["secondArgument"].Value;

                if (first.Contains(argument) && second.Contains(argument))
                    return FindDerivative(first, argument) + current.Groups["sign"].Value + FindDerivative(second, argument);
                else if (!first.Contains(argument) && second.Contains(argument))
                    return FindDerivative(second, argument);
                else if (first.Contains(argument) && !second.Contains(argument))
                    return FindDerivative(first, argument);
                else
                    return "0";
            }

            // тестируем на умножение

            current = multiply.Match(functionString);

            if (isSuccessfulBinaryMatch(current))
            {
                string first = current.Groups["firstArgument"].Value;
                string second = current.Groups["secondArgument"].Value;

                if (first.Contains(argument) && second.Contains(argument))
                    return string.Format("{0}*{1}+{2}*{3}", FindDerivative(first, argument), second, first, FindDerivative(second, argument));
                else if (first.Contains(argument) && !second.Contains(argument))
                    return string.Format("{0}*{1}", second, FindDerivative(first, argument));
                else if (!first.Contains(argument) && second.Contains(argument))
                    return string.Format("{0}*{1}", first, FindDerivative(second, argument));
            }

            // тестируем на деление

            current = divide.Match(functionString);

            if (isSuccessfulBinaryMatch(current))
            {
                string first = current.Groups["firstArgument"].Value;
                string second = current.Groups["secondArgument"].Value;

                if (first.Contains(argument) && second.Contains(argument))
                    return string.Format("({0}*{1}-{2}*{3})/(({1})^2)", FindDerivative(first, argument), second, first, FindDerivative(second, argument));
                else if (first.Contains(argument) && !second.Contains(argument))
                    return string.Format("({0})/({1})", FindDerivative(first, argument), second);
                else if (!first.Contains(argument) && second.Contains(argument))
                    return string.Format("((-{0})*{1})/(({2})^2)", first, FindDerivative(second, argument), second);
            }

            // тестируем на степень

            current = power.Match(functionString);

            if (isSuccessfulBinaryMatch(current))
            {
                string first = current.Groups["firstArgument"].Value;
                string second = current.Groups["secondArgument"].Value;

                if (first.Contains(argument) && second.Contains(argument))
                    return string.Format("(({0})^({1}))*({2}*ln({0})+{3}*{4}/{5})", first, second, FindDerivative(second, argument).ParenthesizeIfNegative(), second.ParenthesizeIfNegative(), FindDerivative(first, argument).ParenthesizeIfNegative(), first.ParenthesizeIfNegative());
                else if (first.Contains(argument) && !second.Contains(argument))
                    return string.Format("{0}*{1}*(({2})^({1}-1))", FindDerivative(first, argument).ParenthesizeIfNegative(), second, first);
                else if (!first.Contains(argument) && second.Contains(argument))
                    return string.Format("{0}*ln({1})*(({1})^({2}))", FindDerivative(second, argument).ParenthesizeIfNegative(), first, second);
            }

            return "0";
        }

        // -------------------------------------
        // ------- match testing ---------------
        // -------------------------------------

        /// <summary>
        /// Должны выполняться три условия, чтобы считать матч функцией.
        /// Например, для ln(x)/ln(x) не выполнится только одно.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="functionString"></param>
        /// <returns></returns>
        private bool isSuccessfulFunctionMatch(Match match, string functionString)
        {
            if (!match.Success) return false;

            string temp = functionString.Substring(functionString.IndexOf('('));

            return match.Value.Length == functionString.Length &&
                   temp.withoutOuterParentheses() != temp;
        }

        /// <summary>
        /// Должны выполняться два условия.
        /// Во втором аргументе количество ( должно быть равно количеству )
        /// Иначе забоссится вот что:
        /// 
        /// ln(x)/ln(2*x)
        /// 
        /// Он схватит результат 'x)'. Потому что умножение проверяется прежде сложения.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        private bool isSuccessfulBinaryMatch(Match match)
        {
            return match.Success &&
                   match.Groups["secondArgument"].Value.Count(delegate(char c) { return c == '('; }) == match.Groups["secondArgument"].Value.Count(delegate(char c) { return c == ')'; });
        }
    }
}
