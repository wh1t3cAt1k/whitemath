using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Diagnostics.Contracts;

using whiteStructs.Conditions;

namespace whiteMath.Functions
{
    [ContractVerification(true)]
    public partial class AnalyticFunction
    {
        /// <summary>
        /// Analyzes the function string and recursively forms the action list.
        /// </summary>
        /// <param name="currentFunctionString">The current part of the function string to be analyzed.</param>
        /// <param name="argumentCharacter">The expected argument character (e.g. 'x').</param>
        /// <param name="currentAction">The current action number.</param>
        /// <returns></returns>
        internal static List<string> Analyze(string currentFunctionString, char argumentCharacter, int currentAction)
        {
			Condition.ValidateNotNull(currentFunctionString, nameof(currentFunctionString));
			Condition
				.Validate(!string.IsNullOrWhiteSpace(currentFunctionString))
				.OrThrowException(new FunctionStringSyntaxException("Unknown error: the string is either empty or whitespace-only."));

            List<string> actionList = new List<string>();

            // Destroy the outer parentheses.
            // -
            currentFunctionString = currentFunctionString.withoutOuterParentheses();

            // If the string is "-smth", we should replace it with
            // "0 - smth"
            // -
            if (currentFunctionString[0] == '-') currentFunctionString = currentFunctionString.Insert(0, "0");

            int lastActionindex = currentAction;  // the index of previous subfunction's last action
            string actionFormatted;               // the formatted string of the action
            List<int> operatorsIndexList;         // indices of mathematical operators
            List<string> tempList;                // helper

            // Look for indices of first-level "+" and "-".
            // if there is no "+" or "-", look for the first-level "*" and "/"
            // -
            operatorsIndexList = currentFunctionString.FindCharactersNotInParentheses('+', '-');

            if (operatorsIndexList.Count == 0)
            {
                operatorsIndexList = currentFunctionString.FindCharactersNotInParentheses('*', '/');
            }

            if (operatorsIndexList.Count > 0)
            {
                // Recursively form the action list for first analyzed sub-function
                // -
                tempList = Analyze(currentFunctionString.Substring(0, operatorsIndexList[0]), argumentCharacter, currentAction);
                actionList.AddRange(tempList);
                currentAction += tempList.Count;

                // For all others except the last...
                // -
                for (int j = 1; j < operatorsIndexList.Count; j++)
                {
                    tempList = Analyze(
                        currentFunctionString.Substring(operatorsIndexList[j - 1] + 1, operatorsIndexList[j] - operatorsIndexList[j - 1] - 1), 
                        argumentCharacter, 
                        currentAction);

                    actionList.AddRange(tempList);
                    actionFormatted = String.Format("{0}:${1}$,$", currentFunctionString[operatorsIndexList[j - 1]], currentAction - 1);

                    actionList.Add(actionFormatted);
                    currentAction += tempList.Count + 1;
                }

                // For the last
                // -
                actionList.AddRange(
                    Analyze(currentFunctionString.Substring(operatorsIndexList[operatorsIndexList.Count - 1] + 1), 
                    argumentCharacter, 
                    currentAction));

                actionFormatted = String.Format(
                    "{0}:${1}$,$", 
                    currentFunctionString[operatorsIndexList[operatorsIndexList.Count - 1]], 
                    currentAction - 1);

                actionList.Add(actionFormatted);

                return actionList;
            }

            // If not, search for the mathematical power operator.
            // -
            operatorsIndexList = currentFunctionString.FindCharactersNotInParentheses('^');

            if (operatorsIndexList.Count > 1)
            {
                throw new FunctionStringSyntaxException("It is forbidden to write more than two sequential mathematical power operators without explicitly mentioning the powering order with parentheses.");
            }
            else if (operatorsIndexList.Count == 1)
            {
                tempList = Analyze(currentFunctionString.Substring(0, operatorsIndexList[0]), argumentCharacter, currentAction);
                actionList.AddRange(tempList);
                currentAction += tempList.Count;

                actionList.AddRange(Analyze(currentFunctionString.Substring(operatorsIndexList[0] + 1), argumentCharacter, currentAction));
                actionFormatted = String.Format("^:${0}$,$", currentAction - 1);

                actionList.Add(actionFormatted);
                return actionList;
            }

            // If not successfull, finally, search for
            // mathematical function signs.
            // -
            if (currentFunctionString[0] == '@')
            {
                // Check if function called is logarithm.
                // -
                if (currentFunctionString.Substring(1, 3) == "log")
                {
                    if (currentFunctionString.IndexOf(',') < 0)
                    {
                        throw new FunctionStringSyntaxException("log(x,y) is called but the y base is not mentioned.");
                    }

                    // 6 was...
                    tempList = Analyze(currentFunctionString.Substring(6, currentFunctionString.IndexOf(',') - 6), argumentCharacter, currentAction);
                    
                    actionList.AddRange(tempList);
                    currentAction += tempList.Count;

                    actionList.AddRange(Analyze(currentFunctionString.Substring(currentFunctionString.IndexOf(',') + 1, currentFunctionString.LastIndexOf(')') - currentFunctionString.IndexOf(',') - 1), argumentCharacter, currentAction));
                }
                // else only one action is done
                // -
                else actionList.AddRange(Analyze(currentFunctionString.Substring(5), argumentCharacter, currentAction));

                switch (currentFunctionString.Substring(1, 3))
                {
                    case "abs": actionList.Add("abs:$"); break;
                    case "sin": actionList.Add("sin:$"); break;
                    case "cos": actionList.Add("cos:$"); break;
                    case "tan": actionList.Add("tg:$"); break;
                    case "asi": actionList.Add("arcsin:$"); break;
                    case "aco": actionList.Add("arccos:$"); break;
                    case "ata": actionList.Add("arctg:$"); break;
                    case "cot": actionList.Add("ctg:$"); break;
                    case "sih": actionList.Add("sinh:$"); break;
                    case "coh": actionList.Add("cosh:$"); break;
                    case "lna": actionList.Add("ln:$"); break;
                    case "lg1": actionList.Add("lg:$"); break;
                    case "log": actionList.Add("log:$"+(currentAction-1)+"$,$"); break;
                    case "exp": actionList.Add("exp:$"); break;
                    case "sqr": actionList.Add("sqrt:$"); break;
                    case "flr": actionList.Add("floor:$"); break;
                    case "cei": actionList.Add("ceil:$"); break;
                    case "rou": actionList.Add("round:$"); break;

                    default: throw new FunctionStringSyntaxException("Unknown function called: " + currentFunctionString.Substring(1, 3));
                }

                return actionList;
            }

            // If this is the argument.
            // -
            if (currentFunctionString[0] == argumentCharacter)
            { 
                actionList.Add("ret:!"); 
                return actionList; 
            }

            // Finally, if this is number, just return it.
            // -
            if (currentFunctionString.Contains(','))
            {
                throw new FunctionStringSyntaxException("Floating point numbers should be written using the dot separator: ','");
            }

            double doubleValue;

            try 
            { 
                doubleValue = double.Parse(
                    currentFunctionString.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)); 
            }
            catch 
            { 
                throw new FunctionStringSyntaxException(
                    string.Format("Unknown error parsing string {0}.", currentFunctionString)); 
            }

            actionList.Add(string.Format("ret:{0}", doubleValue.ToString().Replace(',', '.')));

            return actionList;
        }
    }
}