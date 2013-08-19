using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace whiteMath.Functions
{
    public partial class AnalyticFunction
    {
        /// <summary>
        /// Analyzes the function string and recursively forms the action list.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="argument"></param>
        /// <param name="curAc"></param>
        /// <returns></returns>
        internal static List<string> Analyze(string str, char argument, int curAc)
        {
            if (str == "")
                throw new FunctionStringSyntaxException("Unknown error.");

            List<string> actionList = new List<string>();

            // убиваем внешние скобки

            str = str.withoutOuterParentheses();

            // If the string is "-smth", we should make it a mathematically correct:
            // 0 - smth

            if (str[0] == '-') str = str.Insert(0, "0");

            int lastActionindex=curAc;  // the index of previous subfunction's last action
            string argFormat;           // string formatted
            List<int> temp;             // indices of mathematical operators
            List<string> tempList;      // helper

            // -----------------Look for indices of first-level "+" and "-".
            // -----------------if there is no "+" or "-", look for the first-level "*" and "/"

            temp = str.findSign('+', '-');

            if (temp.Count == 0)
                temp = str.findSign('*', '/');

            if (temp.Count > 0)
            {
                // Recursively form the action list for first analyzed sub-function

                tempList = Analyze(str.Substring(0, temp[0]), argument, curAc);
                actionList.AddRange(tempList);
                curAc += tempList.Count;

                // For all others except the last...
                for (int j = 1; j < temp.Count; j++)
                {
                    tempList = Analyze(str.Substring(temp[j - 1] + 1, temp[j] - temp[j - 1] - 1), argument, curAc);
                    actionList.AddRange(tempList);

                    argFormat = String.Format("{0}:${1}$,$", str[temp[j - 1]], curAc-1);

                    actionList.Add(argFormat);
                    curAc += tempList.Count+1;
                }

                // For the last
                actionList.AddRange(Analyze(str.Substring(temp[temp.Count - 1] + 1), argument, curAc));
                
                argFormat = String.Format("{0}:${1}$,$", str[temp[temp.Count - 1]], curAc-1);
                actionList.Add(argFormat);

                return actionList;
            }

            // ------------------------ If not, search for mathematical power operation

            temp = str.findSign('^');

            if (temp.Count > 1)
                throw new FunctionStringSyntaxException("it is forbidden to write more than two sequential mathematical power operators without explicitly mentioning the powering order with parentheses.");
            else if (temp.Count == 1)
            {
                tempList = Analyze(str.Substring(0, temp[0]), argument, curAc);
                actionList.AddRange(tempList);
                curAc += tempList.Count;

                actionList.AddRange(Analyze(str.Substring(temp[0] + 1), argument, curAc));
                argFormat = String.Format("^:${0}$,$", curAc-1);

                actionList.Add(argFormat);
                return actionList;
            }

            // ------------------------ If not successfull, finally, search for
            // ------------------------ mathematical function signs.
            if (str[0] == '@')
            {
                // Check if function called is logarithm.
                if (str.Substring(1, 3) == "log")
                {
                    if (str.IndexOf(',') == -1) throw new FunctionStringSyntaxException("log(x,y) is called but the y base is not mentioned.");
                    
                    // 6 was...
                    tempList = Analyze(str.Substring(6, str.IndexOf(',') - 6), argument, curAc);
                    
                    actionList.AddRange(tempList);
                    curAc += tempList.Count;

                    actionList.AddRange(Analyze(str.Substring(str.IndexOf(',') + 1, str.LastIndexOf(')') - str.IndexOf(',') - 1), argument, curAc));
                }
                // else only one action is done
                else actionList.AddRange(Analyze(str.Substring(5), argument, curAc));

                switch (str.Substring(1, 3))
                {
                    case "abs": actionList.Add("abs:$"); break;
                    case "sin": actionList.Add("sin:$"); break;
                    case "cos": actionList.Add("cos:$"); break;
                    case "tan": actionList.Add("tg:$"); break;
                    case "asi": actionList.Add("arcsin:$"); break;
                    case "aco": actionList.Add("arccos:$"); break;
                    case "ata": actionList.Add("arctg:$"); break;
                    case "ctg": actionList.Add("ctg:$"); break;
                    case "sih": actionList.Add("sinh:$"); break;
                    case "coh": actionList.Add("cosh:$"); break;
                    case "lna": actionList.Add("ln:$"); break;
                    case "lg1": actionList.Add("log10:$"); break;
                    case "log": actionList.Add("log:$"+(curAc-1)+"$,$"); break;
                    case "exp": actionList.Add("exp:$"); break;
                    case "sqr": actionList.Add("sqrt:$"); break;
                    case "flr": actionList.Add("floor:$"); break;
                    case "cei": actionList.Add("ceil:$"); break;

                    default: throw new FunctionStringSyntaxException("unknown function called: " + str.Substring(1, 3));
                }

                return actionList;
            }

            // ------------------------ If this is an argument, return it.

            if (str[0] == argument)
            { 
                actionList.Add("ret:!"); 
                return actionList; 
            }

            // ------------------------ Finally, if this is number, just return it.

            if (str.Contains(',')) throw new FunctionStringSyntaxException("floating point numbers should be written using the dot separator.");

            double dtm;
            try { dtm = double.Parse(str.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)); }
            catch { throw new FunctionStringSyntaxException("uknown error."); }

            actionList.Add("ret:" + dtm.ToString().Replace(",", "."));
            return actionList;
        }
    }
}