using System;
using System.Globalization;

namespace WhiteMath.Functions
{
    public class PrecompiledFunction: IFunction<double, double>
    {
        private IFunctionAction<double, double>[] actions;
        private IFunction<double, double>[] composedFunctions;

        public PrecompiledFunction(Function function, bool compileComposedFunctions = true)
        {
            this.actions = new IFunctionAction<double,double>[function._actions.Count];

            if (function._composedFunctions != null)
            {
                this.composedFunctions = new IFunction<double, double>[function._composedFunctions.Count];

                for (int i = 0; i < function._composedFunctions.Count; i++)
                {
                    if (function._composedFunctions[i] is Function && compileComposedFunctions)
                        this.composedFunctions[i] = new PrecompiledFunction(function._composedFunctions[i] as Function, true);
                    else
                        this.composedFunctions[i] = function._composedFunctions[i];
                }
            }

            for (int i = 0; i < function._actions.Count; i++)
            {
                string action = function._actions[i].GetActionSubstring();

                switch (action)
                {
                    case "ret": case "return": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), x => x); break;
                    case "sin": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Sin); break;
                    case "cos": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Cos); break;
                    case "sinh": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Sinh); break;
                    case "cosh": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Cosh); break;
                    case "tg": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Tan); break;
                    case "ctg": case "cotan": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), x => (1 / Math.Tan(x))); break;
                    case "arcsin": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Asin); break;
                    case "arccos": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Acos); break;
                    case "arctg": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Atan); break;
                    case "ln": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Log); break;
                    case "lg": case "log10": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Log10); break;
                    case "abs": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Abs); break;
                    case "exp": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Exp); break;
                    case "sign": case "sig": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), x => Math.Sign(x)); break;                    
                    case "sqrt": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Sqrt); break;
                    case "floor": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Floor); break;
                    case "ceil": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), Math.Ceiling); break;
                    case "round": this.actions[i] = new UnaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), x => Math.Round(x, MidpointRounding.AwayFromZero)); break;

                    case "+": this.actions[i] = new BinaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), (x, y) => (x + y)); break;
                    case "-": this.actions[i] = new BinaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), (x, y) => (x - y)); break;
                    case "*": this.actions[i] = new BinaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), (x, y) => (x * y)); break;
                    case "/": this.actions[i] = new BinaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), (x, y) => (x / y)); break;
                    case "^": this.actions[i] = new BinaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), Math.Pow); break;

                    case "log": this.actions[i] = new BinaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), Math.Log); break;

                    case ">": this.actions[i] = new TernaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), analyzeOperand(i, function._actions[i].GetThirdOperand()), Functions.IfMore); break;
                    case ">=": this.actions[i] = new TernaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), analyzeOperand(i, function._actions[i].GetThirdOperand()), Functions.IfMoreEquals); break;
                    case "==": this.actions[i] = new TernaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), analyzeOperand(i, function._actions[i].GetThirdOperand()), Functions.IfEquals); break;
                    case "<": this.actions[i] = new TernaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), analyzeOperand(i, function._actions[i].GetThirdOperand()), Functions.IfLess); break;
                    case "<=": this.actions[i] = new TernaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), analyzeOperand(i, function._actions[i].GetThirdOperand()), Functions.IfLessEquals); break;
                    case "!=": this.actions[i] = new TernaryAction<double, double>(analyzeOperand(i, function._actions[i].GetFirstOperand()), analyzeOperand(i, function._actions[i].GetSecondOperand()), analyzeOperand(i, function._actions[i].GetThirdOperand()), Functions.IfNotEquals); break;

                    default: throw new FunctionActionSyntaxException("Unknown action string."); 
                }
            }
        }

        public double GetValue(double x)
        {
            return actions[actions.Length-1].GetValue(x);     // возвращаем значение функции
        }

        // -----------------------------------------------
        // ------------------- analyzer ------------------
        // -----------------------------------------------

        private IFunction<double, double> analyzeOperand(int actionNum, string operand)
        {
            if(operand.Length==1 && !char.IsDigit(operand[0]))
            {
                if(operand.Equals("!")) return argument;
                else if (operand.Equals("$")) return this.actions[actionNum-1];
                else
                    throw new FunctionActionSyntaxException("Bad action syntax in the function action #" + actionNum);
            }
            
            // здесь уже точно номер внутри скобочек стоит

            switch (operand[0])
            {
                case '#': return new FunctionExceptionThrower<double, double> (operand.Substring(1, operand.Length - 2));
                case '%': return this.composedFunctions[int.Parse(operand.Substring(1, operand.Length - 2))];
                case '$': return this.actions[int.Parse(operand.Substring(1, operand.Length - 2))];
                default: return new ConstantReturner<double, double>(double.Parse(operand.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)));
            }
        }

        // -----------------------------------------------
        // ------------------- private argument ----------
        // -----------------------------------------------

        private static Argument argument = new Argument();

        /// <summary>
        /// The function node symbolizing the argument.
        /// </summary>
        private class Argument: IFunction<double, double>
        {
            public double GetValue(double x)
            {
                return x;
            }
        }
    }
}
