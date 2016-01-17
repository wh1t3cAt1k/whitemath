using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

/* ДАННЫЙ ФАЙЛ СОДЕРЖИТ определение класса FUNCTION, а также набор вспомогательных математических функций.
 * Объявлен также интерфейс IFunction, обеспечивающий функциональность выдачи значения по аргументу
 * 
 * To be implemented: 
 * 1. Long integer and long exponential arithmetics.
 * 2. Functions: factorial, gamma-function etc.

/*
 * A list of short names for elementary mathematic functions
 * in the 'actions array'.
 * 
 * x        -- ret:x
 * x+y      -- +:x,y
 * x-y      -- -:x,y
 * x*y      -- *:x,y
 * x/y      -- /:x,y
 * x^y      -- ^:x,y
 * |x|      -- abs:x
 * [x]      -- floor:x
 * ceil(x)  -- ceil:x
 * sqrt(x)  -- sqrt:x
 * sin(x)   -- sin:x
 * cos(x)   -- cos:x
 * tg(x)    -- tg:x
 * ctg(x)   -- ctg:x
 * arcsin(x)-- arcsin:x
 * arccos(x)-- arccos:x
 * arctg(x) -- arctg:x
 * sinh(x)  -- sinh:x
 * cosh(x)  -- cosh:x
 * ln(x)    -- ln:x
 * lg(x)    -- lg:x
 * log(x,a) -- log:x,a
 * exp(x)   -- exp:x 
 * sgn(x)   -- sign:x
 * if (x > 0) return y else return z -- >:x,y,z 
 * if (x = 0) return y else return z -- =:x,y,z
 * if (x >= 0) return y else return z -- >=:x,y,z
 * if (x < 0) return y else return z -- <:x,y,z
 * if (x != 0) return y else return z -- !=:x,y,z
 * if (x <= 0) return y else return z -- <=:x,y,z

 * x,y,z - meta-signs of operands.
 * In the actual action syntax these meta-signs can be replaced by:
 * 
 * 1) The $ symbol - takes the result of the previous action.
 * 2) $n$, where n is an integer - takes the result of action number n.
 *      warning! Action indices are zero-based, n must be less than the current action index.
 * 3) %n%, where n is an integer - takes the result of inner function number n.
 *      warning! Inner function indices are zero-based.
 * 4) The ! symbol - takes the value of the argument
 * 5) #errormessage# - raises an exception with the specified message.
 * 5) a number. (e.g.: 5.2)
 *      warning! The integer and fractional parts must be separated by a DOT, not a comma: '.'
 */

namespace whiteMath.Functions
{
    public class Function: IFunction<double, double>
    {
        public double this[double x] { get { return Value(x); } }

        delegate double UnaryAction(double x);
        delegate double BinaryAction(double x, double y);
        delegate bool CheckerAction(double x);

        protected internal char argument;
        protected internal List<IFunction<double, double>> composedFunc;
        protected internal List<string> actions;
        
        private double[] actionResults;

        /// <summary>
        /// A special constructor for AnalyticFunction class
        /// </summary>
        protected Function()
        { }

        /// <summary>
        /// Creates a new function using the specified action list
        /// </summary>
        /// <param name="actions"></param>
        public Function(List<string> actions)
        {
            this.actions = actions;
            composedFunc = new List<IFunction<double,double>>();
        }

        public Function(List<string> actions, List<IFunction<double,double>> composedFunctions)
        {
            this.actions = actions;
            this.composedFunc = composedFunctions;
        }

        /// <summary>
        /// Calculates the function value depending on the argument passed.
        /// </summary>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public double Value(double argumentValue)
        {
            UnaryAction unaryAction;
            BinaryAction binaryAction;
            CheckerAction checkerAction;

            actionResults = new double[actions.Count];

            for (int i = 0; i < actions.Count; ++i)
            {
                string currentActionString = actions[i];

                if (string.IsNullOrWhiteSpace(currentActionString))
                {
                    throw new FunctionActionSyntaxException("Action string for action number " + i + " should not be empty.");
                }

                unaryAction = new UnaryAction(x => x);
                binaryAction = new BinaryAction((x, y) => (x + y));
                checkerAction = new CheckerAction(x => (x == 0));

                // Binary or unary action flag.
                // -
                bool binary = true; 

                // If no user-defined error messages in the action,
                // remove all whitespace.
                // -
                if (currentActionString.IndexOf('#') == -1)
                {
                    currentActionString = currentActionString.Replace(" ", "");
                }
                // Otherwise, do some magic.
                // -
                else
                {
                    StringBuilder stringWithoutWhitespace = new StringBuilder();
                    bool isInsideErrorMessage = false;

                    for (int j = 0; j < currentActionString.Length; j++)
                    {
                        if (currentActionString[j] == '#')
                        {
                            isInsideErrorMessage = !isInsideErrorMessage;
                        }
                        else if (currentActionString[j] != ' ' || isInsideErrorMessage)
                        {
                            stringWithoutWhitespace.Append(currentActionString[j]);
                        }
                    }

                    currentActionString = stringWithoutWhitespace.ToString();
                }

                double a, b;
                string operand1, operand2, operand3;

                switch(currentActionString.getActionSubString())
                {
                    case "ret": binary = false; unaryAction = new UnaryAction(x => x); break;
                    
                    case "+": binaryAction = new BinaryAction((x, y) => (x + y)); break;
                    case "-": binaryAction = new BinaryAction((x, y) => (x - y)); break;
                    case "*": binaryAction = new BinaryAction((x, y) => (x * y)); break;
                    case "/": binaryAction = new BinaryAction((x, y) => (x / y)); break;
                    case "^": binaryAction = new BinaryAction(Math.Pow); break;
                    
                    case "abs": binary = false; unaryAction = new UnaryAction(Math.Abs); break;
                    case "floor": binary = false; unaryAction = new UnaryAction(Math.Floor); break;
                    case "ceil": binary = false; unaryAction = new UnaryAction(Math.Ceiling); break;
                    case "round": binary = false; unaryAction = new UnaryAction(number => Math.Round(number, MidpointRounding.AwayFromZero)); break;

                    case "sin": binary = false; unaryAction = new UnaryAction(Math.Sin); break;
                    case "cos": binary = false; unaryAction = new UnaryAction(Math.Cos); break;
                    case "tg": binary = false; unaryAction = new UnaryAction(Math.Tan); break;
                    case "ctg": binary = false; unaryAction = new UnaryAction(x => (1 / Math.Tan(x))); break;

                    case "arcsin": binary = false; unaryAction = new UnaryAction(Math.Asin); break;
                    case "arccos": binary = false; unaryAction = new UnaryAction(Math.Acos); break;
                    case "arctg": binary = false; unaryAction = new UnaryAction(Math.Atan); break;
                    
                    case "sinh": binary = false; unaryAction = new UnaryAction(Math.Sinh); break;
                    case "cosh": binary = false; unaryAction = new UnaryAction(Math.Cosh); break;
                    
                    case "log": binary = true; binaryAction = new BinaryAction(Math.Log); break;
                    case "lg": binary = false; unaryAction = new UnaryAction(Math.Log10); break;
                    
                    case "sqrt": binary = false; unaryAction = new UnaryAction(Math.Sqrt); break;
                    case "ln": binary = false; unaryAction = new UnaryAction(Math.Log); break;
                    
                    case "exp": binary = false; unaryAction = new UnaryAction(Math.Exp); break;
                    case "sign": binary = false; unaryAction = new UnaryAction(x => Math.Sign(x)); break;
                    
                    case ">": case "=": case ">=": case "<=": case "<": case "!=": goto CONDITION;
                    default: throw new FunctionActionSyntaxException("Unknown function name in the list of actions."); 
                }

                if (binary)
                {
                    operand1 = currentActionString.getFirstOperand();
                    operand2 = currentActionString.getSecondOperand();

                    try
                    {
                        a = operandMeaning(operand1, composedFunc, actionResults, i, argumentValue);
                        b = operandMeaning(operand2, composedFunc, actionResults, i, argumentValue);
                    }
                    catch (Exception actionExecutionException)
                    {
                        throw new FunctionActionExecutionException(actionExecutionException.Message, i); 
                    }

                    double operationResult; // результат операции

                    try 
                    { 
                        operationResult = binaryAction(a, b); 
                    }
                    catch 
                    { 
                        operationResult = double.NaN; 
                    }

                    actionResults[i] = operationResult;
                    goto RETURNER;
                }
                else
                {
                    operand1 = currentActionString.getFirstOperand();

                    try 
                    { 
                        a = operandMeaning(operand1, composedFunc, actionResults, i, argumentValue); 
                    }
                    catch (Exception xxx)
                    {
                        if (xxx is FunctionActionUserThrownException)
                        {
                            throw;
                        }
                        else
                        {
                            throw new FunctionActionExecutionException(xxx.Message, i);
                        }
                    }

                    double operationResult;

                    try { operationResult = unaryAction(a); }
                    catch { operationResult = double.NaN; }

                    actionResults[i] = operationResult;
                    goto RETURNER;
                }

            // This block executes in case of the conditional action operator.
            // -
            CONDITION: 

                if (i == 0) throw new FunctionActionSyntaxException("No conditional operators are allowed in the action number 0.");
                
                // Should declare additional variable since the action is ternary.
                // -
                double c;
                
                operand1 = currentActionString.getFirstOperand();
                operand2 = currentActionString.getSecondOperand();
                operand3 = currentActionString.getThirdOperand();

                try
                {
                    a = operandMeaning(operand1, composedFunc, actionResults, i, argumentValue);
                }
                catch (Exception xxx)
                {
                    if (xxx is FunctionActionUserThrownException) throw;
                    throw new Exception("Ошибка при выполнении действия " + i + ". " + xxx.Message); 
                }

                switch(currentActionString.getActionSubString())
                {
                    case ">": checkerAction = new CheckerAction(x => (x > 0)); break;
                    case "<": checkerAction = new CheckerAction(x => (x < 0)); break;
                    case "=": checkerAction = new CheckerAction(x => (x == 0)); break;
                    case ">=": checkerAction = new CheckerAction(x => (x >= 0)); break;
                    case "<=": checkerAction = new CheckerAction(x => (x <= 0)); break;
                    case "!=": checkerAction = new CheckerAction(x => (x != 0)); break;
                }

                if (checkerAction(a)) 
                    try
                        {   
                            b = operandMeaning(operand2, composedFunc, actionResults, i, argumentValue);
                            actionResults[i] = b;
                        }
                    catch (Exception xxx)
                        {
                            if (xxx is FunctionActionUserThrownException) throw;
                            throw new Exception("Error while performing action number " + i + ". " + xxx.Message); 
                        }                        
                else
                    try
                        {
                            c = operandMeaning(operand3, composedFunc, actionResults, i, argumentValue);
                            actionResults[i] = c;
                        }
                    catch (Exception xxx)
                        {
                            if (xxx is FunctionActionUserThrownException) throw;
                            throw new Exception("Error while performing action number " + i + ". " + xxx.Message);
                        }
            
            RETURNER: ; 
            }

            return actionResults[actionResults.Length - 1];
        }

        private double operandMeaning(string operand, List<IFunction<double,double>> composedFunc, double[] actionResults, int num, double x)
        {
            if (operand == null || operand == "") throw new FunctionActionSyntaxException("One of the operands is missing.");

            int negativeFlag = 1;

            // 'negflag' is multiplied by -1 if the operand
            // is preceded by a minus sign '-'.
            // 
            if (operand[0] == '-') 
            { 
                negativeFlag = -1; 
                operand = operand.Substring(1); 
            }

            // Reference the digit value.
            // -
            if (char.IsDigit(operand[0])) 
            { 
                try 
                { 
                    double tmp = double.Parse(
                        operand.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)); return negativeFlag * tmp; 
                } 
                catch 
                { 
                    throw
                        new FunctionActionSyntaxException("Unknown operand in the action."); 
                } 
            }
            // Referencing the argument value.
            // -
            else if (operand.Length == 1 && operand[0] == '!')
            {
                return negativeFlag * x;
            }
            // Referencing the previous action result.
            // -
            else if (operand.Length == 1 && operand[0] == '$')
            {
                if (num <= 0) throw new FunctionActionSyntaxException("No previous action for action 0.");
                return negativeFlag * actionResults[num - 1];
            }
            // Referencing one of the action results by its number (index).
            // -
            else if (operand[0] == '$')
            {
                if (operand.Length <= 2 || operand.LastIndexOf('$') != operand.Length - 1) throw new Exception("Ошибка синтаксиса действия: неверно указано обращение к результату действия по номеру.");

                operand = operand.Replace("$", "");
                int callOperationNum = -1;
                try
                {
                    callOperationNum = int.Parse(operand);
                }
                catch { throw new FunctionActionSyntaxException("неизвестный номер действия, указанный в обращении."); }
                if (callOperationNum >= num) throw new FunctionActionSyntaxException("попытка рефлексивного обращения в действии или попытка обращения к еще не выполненному (следующему) действию.");
                return negativeFlag * actionResults[callOperationNum]; // возвращаем значение действия по номеру
            }
            else if (operand[0] == '%') // вызов результата вложенной функции по номеру
            {
                if (operand.Length <= 2 || operand.LastIndexOf('%') != operand.Length - 1) throw new FunctionActionSyntaxException("неверно указано обращение к результату вложенной функции.");

                operand = operand.Replace("%", "");

                int callFuncNum = -1;

                try
                {
                    callFuncNum = int.Parse(operand);
                }
                catch { throw new FunctionActionSyntaxException("wrong inner function number specified."); }

                if (callFuncNum < 0 || callFuncNum >= composedFunc.Count)
                    throw new FunctionActionSyntaxException("there is no inner function with such a number (numeration starts from 0!).");

                return negativeFlag * composedFunc[callFuncNum].Value(x);
            }
            else if (operand[0] == '#') // пользовательское сообщение об ошибке
            {
                if (operand.LastIndexOf('#') != operand.Length - 1) throw new FunctionActionSyntaxException("пользовательское сообщение об ошибке в синтаксисе действия должно находиться между двумя символами #.");
                throw new FunctionActionUserThrownException(operand.Remove(0, 1).Remove(operand.Length - 2));
            }

            throw new FunctionActionSyntaxException("unknown action string syntax.");
        }

        //TODO: переписать интеграл, чтобы считал по-человечески
        public double Integral(double xMin, double xMax, double xStep)
        {
            double integralSum=0;

            if (xStep <= 0) throw new ArgumentException("Для подсчета интегральных сумм шаг должен быть больше нуля.");

            for (double i = xMin; i<xMax; i += xStep)
            {
                if (double.IsNaN(this[i])) return double.NaN;
                if (this[i] == double.PositiveInfinity) return double.PositiveInfinity;
                if (this[i] == double.NegativeInfinity) return double.NegativeInfinity;
                //предыдущее спорно. есть предложение выкидывать здесь эксепшн

                if(!(this[i]*this[i-xStep]<=0)) integralSum += (this[i] + this[i+xStep])/2 * xStep;
            }

            return integralSum;
        }

        // ------------------------------------------------
        // ---------- PRECOMPILED REFERENCE ---------------
        // ------------------------------------------------

        /// <summary>
        /// Returns the precompiled version of the function.
        /// Consumes more memory, but calculates its value faster.
        /// </summary>
        public PrecompiledFunction PrecompiledVersion
        {
            get { return new PrecompiledFunction(this); }
        }
    }

    public static class MathFunctions
    {
        public static readonly IFunction<double,double> gamma = new GammaFunction();
        public static readonly IFunction<double,double> factorialLogarithmic = new FactorialLogarithmic();
        public static readonly IFunction<long,long> factorial = new Factorial();
    }

    #region FunctionWorkForDelegates

    internal static class Functions
    {
        internal static double IfMore(double a, double b, double c)
        {
            return (a > 0 ? b : c);
        }

        internal static double IfMoreEquals(double a, double b, double c)
        {
            return (a >= 0 ? b : c);
        }

        internal static double IfEquals(double a, double b, double c)
        {
            return (a == 0 ? b : c);
        }

        internal static double IfLess(double a, double b, double c)
        {
            return (a < 0 ? b : c);
        }

        internal static double IfLessEquals(double a, double b, double c)
        {
            return (a <= 0 ? b : c);
        }

        internal static double IfNotEquals(double a, double b, double c)
        {
            return (a != 0 ? b : c);
        }
    }

    #endregion

    #region OtherFunctionClasses

    internal class GammaFunction: IFunction<double,double> // гамма-функция вещественного аргумента не дописано
    {
        public double Value(double x) { return 0; }
    }

    internal class FactorialLogarithmic : IFunction<double,double> // логарифмический факториал - высокая скорость, низкая точность
    {
        public double Value(double x)
        {
            if (x < 1) return 1;

            double tmp=0;

            for (int i = 2; i <= x; i++)
                tmp += Math.Log(i);

            return Math.Exp(tmp);
        }
    }

    internal class Factorial: IFunction<long, long>
    {
        public long Value(long x)
        {
            if (x < 1) return 1;

            long tmp = 2;
            for (int i = 2; i <= x; i++) tmp *= i;

            return tmp;
        }
    }

    #endregion
}
