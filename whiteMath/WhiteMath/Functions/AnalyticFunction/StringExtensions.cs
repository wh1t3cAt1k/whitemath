﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using WhiteMath.General;

using WhiteStructs.Conditions;

namespace WhiteMath.Functions
{
    public static class StringExtensions
    {
        // Regex для нахождения выражений, заключенных в скобки

        public static string parentheses = @"((?'Open'\()[^()]*)+((?'Close-Open'\))(?(Open)([^()]*)))+(?(Open)(@ERROR@))";
        public static Regex simpleParentRegex = new Regex(parentheses, RegexOptions.Compiled);

        // Regex для нахождения выражений, заключенных в скобки + проверки кое-каких параметров.

        public static string prePostParentheses = "(?<preChar>.)?" + "(?<inner>" + parentheses + ")" + "(?<postChar>.)?";
        public static Regex prePostParentRegex = new Regex(prePostParentheses, RegexOptions.Compiled);

        // Regex для проверки того, что ВСЯ строка заключена в скобки.

        public static string fullParentheses = @"^[^()]*(((?'Open'\()[^()]*)+((?'Close-Open'\))(?(Open)([^()]*)))+)*(?(Open)(@ERROR@))$";
        public static Regex fullParentRegex = new Regex(fullParentheses, RegexOptions.Compiled);

        /// <summary>
        /// Returs a reference to the string equal to the calling object.
        /// But... without any outer parentheses. ((sin(x))) = sin(x);
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string WithoutOuterParentheses(this string str)
        {
            string copy = str.Clone() as string;

			while (RemoveOuterParentheses(ref copy))
			{ }

            return copy;
        }

        /// <summary>
        /// Finds all occurrences of the specified characters in a string
        /// and returns the list of their indices; the characters
        /// located inside the parentheses do not count.
        /// </summary>
        /// <param name="sourceString">The string where the search is to be done.</param>
        /// <param name="characters">A parameter array containing the characters to be found.</param>
        /// <returns>
        /// A list of indices in the <paramref name="sourceString"/>
        /// where any one of the characters in <paramref name="characters"/> 
        /// has been found, excluding those located inside any parentheses.
        /// </returns>
        internal static List<int> FindCharactersNotInParentheses(this string sourceString, params char[] characters)
        {
			Condition.ValidateNotNull(sourceString, nameof(sourceString));
			Condition.ValidateNotNull(characters, nameof(characters));

            List<int> indexes = new List<int>();

            for (int i = 0; i < sourceString.Length; i++)
            {
                // We should leave alone (do not touch) whatever is inside the parentheses.
                // -
                if (sourceString[i] == '(')
                {
                    int openBracketCount = 1;
                    
                    // Enter the loop which will not match
                    // any character while inside the parentheses.
                    // -
                    do
                    {
                        ++i;

                        if (sourceString[i] == '(')
                        {
                            openBracketCount++;
                        }
                        else if (sourceString[i] == ')')
                        {
                            openBracketCount--;
                        }
                    } while (openBracketCount > 0);
                }
                else if (characters.Contains<char>(sourceString[i]))
                {
                    indexes.Add(i);
                }
            }

            return indexes;
        }

        /// <summary>
        /// Проверяет, есть ли внешние скобки в строке функции.
        /// Если есть, то убирает их и возвращает true.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool RemoveOuterParentheses(ref string str)
        {
            if (str.Length == 0 || str[0] != '(' || str[str.Length - 1] != ')') return false;

            // однако!
            // все равно может быть уловка :) типа ((x)) + log(x,3)

            int parCount = 1;
            int i = 0;

            // there is no need to perform parentheses correctiveness check
            // because it has been done by CheckNPrepare.

            while (parCount > 0 && i<str.Length-1)
            {
                ++i;
                if (str[i] == '(') parCount++;
                else if (str[i] == ')') parCount--;
            }

            if (i == str.Length - 1)
            {
                str = str.Substring(1, str.Length - 2);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns the flag whether the string has outer parentheses.
        /// without modifying it.
        /// </summary>
        /// <param name="str">The calling string object.</param>
        /// <returns>THe flag indicating whether the string has outer parentheses.</returns>
        public static bool HasOuterParentheses(this string str)
        {
            return fullParentRegex.Match(str).Success;
        }

        /// <summary>
        /// Encloses the calling object in parentheses if it symbolizes a negative value.
        /// </summary>
        internal static string ParenthesizeIfNegative(this string text)
        {
            if (text.Length > 0 && text[0] == '-')
                return "(" + text + ")";
            else
                return text;
        }

        // ------------ PARSE OPERATION --------

        static string decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        static MatchEvaluator evaluator = new MatchEvaluator(delegate(Match obj) { return obj.Groups["num"].Value; });


        /// <summary>
        /// Parses the two strings into their double equivalents and performs an operation on them.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        internal static string parseOperation(string one, string two, Func<double, double, double> operation)
        {
            one = Regex.Replace(one, @"\((?<num>.*)\)", evaluator).Replace(".", decimalSeparator);
            two = Regex.Replace(two, @"\((?<num>.*)\)", evaluator).Replace(".", decimalSeparator);
            
            double x = double.Parse(one);
            double y = double.Parse(two);

            // парсим обратно в строку и закрываем в скобки, если результат отрицателен

            return operation(x, y).ToString().Replace(decimalSeparator, ".").ParenthesizeIfNegative();
        }

        /// ------------------------------------------
        /// ------- PARENTHESES LEVEL PREPARE --------
        /// ------------------------------------------

        //todo not public
        public static string enumerateParentheses(this string parent)
        {
            int counter = 0;

            string obj = parent.Clone() as string;

            for(int i=0; i<obj.Length; )
                if (obj[i] == '(')
                {
                    string replacement = string.Format("@{0}(@", counter.ToString());
                    obj = obj.SubstringToIndex(0, i) + replacement + obj.Substring(i + 1);

                    counter++;
                    i += replacement.Length;
                }
                else if (obj[i] == ')')
                {
                    // все верно - СНАЧАЛА уменьшить, потом заменять.
                    counter--;

                    string replacement = string.Format("@{0})@", counter.ToString());
                    obj = obj.SubstringToIndex(0, i) + replacement + obj.Substring(i + 1);

                    i += replacement.Length;
                }
                else
                    i++;

            return obj;
        }

        static Regex enumeratedParentheses = new Regex(@"@\d+(?<parent>\(|\))@", RegexOptions.Compiled);

        /// <summary>
        /// Removes the parentheses enumeration from the string.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static string removeParenthesesEnumeration(this string parent)
        {
            string obj = parent.Clone() as string;

        REPEAT:

            Match match = enumeratedParentheses.Match(obj);

            if (match.Success)
            {
                obj = match.Result("$`${parent}$'");
                goto REPEAT;
            }

            return obj;
        }

        /// ------------------------------------------
        /// ------- REMOVE DOUBLING PARENTHESES-------
        /// ------------------------------------------

        // все рассчитано под то, что уже заменили.

        static string numberPattern = @"(?<par>\()?" + @"-?\d+(\.\d+)?" + @"(?(par)\))" + @"(?![\d.)])";
        static string expressionPattern = string.Format(@"({0}|[a-zA-Z]+?{0}|[^@+\-*/\^()0-9]|(.{{0}}(?![\^\(*/@])))(?!\d)", parentheses);
        
        // Либо это просто выражение со скобками
        // Либо это функция
        // Либо это просто аргумент
        // Либо это ничего, но уж справа оттуда ничего стоять не должно кроме знаков "плюс-минус"
        
        static Regex parent = new Regex(@"(?<beforeChar>.)?(@(?<number>\d+)\(@" + @"(?<parentInner>.*?)" + @"@(\k<number>)\)@)(?<afterChar>.)", RegexOptions.Compiled);
        
        public static Regex number = new Regex(numberPattern);
        public static Regex expression = new Regex(expressionPattern);

        // в самом начале plusMinusLeft не должно быть левой скобочки. а то он будет ловить выражения (-21.35) хотя это просто число.

        public static Regex plusMinusLeft = new Regex(string.Format(@"((?<!\()(?<sign2>[+\-])(?<num>{0})(\*)?(?<expr>{1})(?!\d))", numberPattern, expressionPattern), RegexOptions.Compiled);
        public static Regex plusMinusRight = new Regex(string.Format(@"((?<!\d)(?<num>{0})(\*)?(?<expr>{1})(?=(?<sign1>[+\-])))", numberPattern, expressionPattern), RegexOptions.Compiled); 

        // В конце plusMinusRight надо лишь убеждаться в наличии знака, но не ловить его.
        // этот знак нам нужен при дальнейшем исследовании, потому что если это минус,
        // то он может относиться к составляющей num другого слагаемого plusMinusRight.

        /// <summary>
        /// Сокращает сумму в одно слагаемое, если они отличаются только коэффициентом.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool hasPlusMinus(ref string obj)
        {
            bool somethingChangedFlag = false;        // изменилось ли что-нибудь после вызова этой функции

            REPEAT:

            foreach(Match leftPlus in plusMinusLeft.Matches(obj))
                foreach (Match rightPlus in plusMinusRight.Matches(obj))
                {
                    // забраковываем сразу то, что не выйдет

                    if (leftPlus.Index <= rightPlus.Index)
                        continue;

                    Group num1 = rightPlus.Groups["num"];
                    Group num2 = leftPlus.Groups["num"];
                    
                    Group expr1 = rightPlus.Groups["expr"];
                    Group expr2 = leftPlus.Groups["expr"];

                    Group sign1 = rightPlus.Groups["sign1"];
                    Group sign2 = leftPlus.Groups["sign2"];

                    // нет подсчета скобок

                    if (expr1.Value == expr2.Value)
                    {
                        Func<double, double, double> operation;

                        if (sign2.Value == "+")
                            operation = delegate(double a, double b) { return a + b; };
                        else
                            operation = delegate(double a, double b) { return a - b; };

                        string substitution = parseOperation(num1.Value, num2.Value, operation);

                        // разберемся со знаком.

                        if (rightPlus.Index != 0)
                        {
                            substitution = substitution.Replace("(", "").Replace(")", "");
                            
                            if (substitution[0] != '-')
                                substitution = "+" + substitution;
                        }

                        obj = obj.Substring(0, rightPlus.Index) +
                            substitution +
                            (expr1.Value == "" ? "" : "*") +
                            expr1.Value +
                            obj.SubstringToIndex(rightPlus.Index + rightPlus.Length, leftPlus.Index) +
                            obj.Substring(leftPlus.Index + leftPlus.Length);

                        somethingChangedFlag = true;

                        goto REPEAT;
                    }
                }

            return somethingChangedFlag;
        }

        /// <summary>
        /// Modifies the string object so that it does not contain any unneeded parentheses anymore.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool hasUnneededParentheses(ref string obj)
        {
			throw new NotImplementedException();
        }

        // ------------------------------
        // --------- INSERT * -----------
        // ------------------------------

		private static Regex MultiplicationSignInsertionRegex = new Regex(@"(\d)([a-zA-Z@\(])", RegexOptions.Compiled);
        
        /// <summary>
        /// Inserts the multiplication sign where safe, e.g. 25sin(x) => 25*sin(x)
        /// and returns the modified string.
        /// </summary>
        /// <param name="sourceString">The source string object (remains as is).</param>
        /// <returns>The modified string with multiplication signs inserted where safe.</returns>
        internal static string InsertMultiplicationSigns(this string sourceString)
        {
            Match match = MultiplicationSignInsertionRegex.Match(sourceString);

            while (match.Success)
            {
                sourceString = match.Result("$`$1*$2$'");
                match = MultiplicationSignInsertionRegex.Match(sourceString);
            }

            return sourceString;
        }
    }
    
}
