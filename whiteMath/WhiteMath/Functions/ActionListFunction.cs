using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

/* ДАННЫЙ ФАЙЛ СОДЕРЖИТ определение класса FUNCTION, а также набор вспомогательных математических функций.
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

namespace WhiteMath.Functions
{
    public class ActionListFunction: IFunction<double, double>
    {
        public double this[double x] { get { return GetValue(x); } }

        private delegate double UnaryAction(double x);
        private delegate double BinaryAction(double x, double y);
        private delegate bool CheckerAction(double x);

        protected internal char _argumentSymbol;
        protected internal List<IFunction<double, double>> _composedFunctions;
        protected internal List<string> _actions;
        
		private double[] _actionResults;

        /// <summary>
        /// A special constructor for AnalyticFunction class
        /// </summary>
        protected ActionListFunction()
        { }

        /// <summary>
        /// Creates a new function using the specified action list
        /// </summary>
        /// <param name="actions"></param>
        public ActionListFunction(List<string> actions)
        {
            this._actions = actions;
            _composedFunctions = new List<IFunction<double,double>>();
        }

        public ActionListFunction(List<string> actions, List<IFunction<double,double>> composedFunctions)
        {
            this._actions = actions;
            this._composedFunctions = composedFunctions;
        }

        /// <summary>
        /// Calculates the function value depending on the argument passed.
        /// </summary>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public double GetValue(double argumentValue)
        {
            UnaryAction unaryAction;
            BinaryAction binaryAction;
            CheckerAction checkerAction;

            _actionResults = new double[_actions.Count];

			for (int actionIndex = 0; actionIndex < _actions.Count; ++actionIndex)
            {
                string currentActionString = _actions[actionIndex];

                if (string.IsNullOrWhiteSpace(currentActionString))
                {
                    throw new FunctionActionSyntaxException("Action string for action number " + actionIndex + " should not be empty.");
                }

				currentActionString = RemoveWhitespaceExceptInErrorMessages(currentActionString);

                unaryAction = new UnaryAction(x => x);
                binaryAction = new BinaryAction((x, y) => (x + y));
                checkerAction = new CheckerAction(x => (x == 0));

                // Binary or unary action flag.
                // -
				bool isBinaryAction = true;

                double a, b;
                string operand1, operand2, operand3;

                switch(currentActionString.GetActionSubstring())
                {
                    case "ret": isBinaryAction = false; unaryAction = new UnaryAction(x => x); break;
                    
                    case "+": binaryAction = new BinaryAction((x, y) => (x + y)); break;
                    case "-": binaryAction = new BinaryAction((x, y) => (x - y)); break;
                    case "*": binaryAction = new BinaryAction((x, y) => (x * y)); break;
                    case "/": binaryAction = new BinaryAction((x, y) => (x / y)); break;
                    case "^": binaryAction = new BinaryAction(Math.Pow); break;
                    
                    case "abs": isBinaryAction = false; unaryAction = new UnaryAction(Math.Abs); break;
                    case "floor": isBinaryAction = false; unaryAction = new UnaryAction(Math.Floor); break;
                    case "ceil": isBinaryAction = false; unaryAction = new UnaryAction(Math.Ceiling); break;
                    case "round": isBinaryAction = false; unaryAction = new UnaryAction(number => Math.Round(number, MidpointRounding.AwayFromZero)); break;

                    case "sin": isBinaryAction = false; unaryAction = new UnaryAction(Math.Sin); break;
                    case "cos": isBinaryAction = false; unaryAction = new UnaryAction(Math.Cos); break;
                    case "tg": isBinaryAction = false; unaryAction = new UnaryAction(Math.Tan); break;
                    case "ctg": isBinaryAction = false; unaryAction = new UnaryAction(x => (1 / Math.Tan(x))); break;

                    case "arcsin": isBinaryAction = false; unaryAction = new UnaryAction(Math.Asin); break;
                    case "arccos": isBinaryAction = false; unaryAction = new UnaryAction(Math.Acos); break;
                    case "arctg": isBinaryAction = false; unaryAction = new UnaryAction(Math.Atan); break;
                    
                    case "sinh": isBinaryAction = false; unaryAction = new UnaryAction(Math.Sinh); break;
                    case "cosh": isBinaryAction = false; unaryAction = new UnaryAction(Math.Cosh); break;
                    
                    case "log": isBinaryAction = true; binaryAction = new BinaryAction(Math.Log); break;
                    case "lg": isBinaryAction = false; unaryAction = new UnaryAction(Math.Log10); break;
                    
                    case "sqrt": isBinaryAction = false; unaryAction = new UnaryAction(Math.Sqrt); break;
                    case "ln": isBinaryAction = false; unaryAction = new UnaryAction(Math.Log); break;
                    
                    case "exp": isBinaryAction = false; unaryAction = new UnaryAction(Math.Exp); break;
                    case "sign": isBinaryAction = false; unaryAction = new UnaryAction(x => Math.Sign(x)); break;
                    
                    case ">": case "=": case ">=": case "<=": case "<": case "!=": goto CONDITION;
                    default: throw new FunctionActionSyntaxException("Unknown function name in the list of actions."); 
                }

                if (isBinaryAction)
                {
                    operand1 = currentActionString.GetFirstOperand();
                    operand2 = currentActionString.GetSecondOperand();

                    try
                    {
                        a = GetOperandValue(operand1, _composedFunctions, _actionResults, actionIndex, argumentValue);
                        b = GetOperandValue(operand2, _composedFunctions, _actionResults, actionIndex, argumentValue);
                    }
                    catch (Exception actionExecutionException)
                    {
                        throw new FunctionActionExecutionException(actionExecutionException.Message, actionIndex); 
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

                    _actionResults[actionIndex] = operationResult;
                    goto RETURNER;
                }
                else
                {
                    operand1 = currentActionString.GetFirstOperand();

                    try 
                    { 
                        a = GetOperandValue(operand1, _composedFunctions, _actionResults, actionIndex, argumentValue); 
                    }
                    catch (Exception xxx)
                    {
                        if (xxx is FunctionActionUserThrownException)
                        {
                            throw;
                        }
                        else
                        {
                            throw new FunctionActionExecutionException(xxx.Message, actionIndex);
                        }
                    }

                    double operationResult;

                    try { operationResult = unaryAction(a); }
                    catch { operationResult = double.NaN; }

                    _actionResults[actionIndex] = operationResult;
                    goto RETURNER;
                }

            // This block executes in case of the conditional action operator.
            // -
            CONDITION: 

                if (actionIndex == 0) throw new FunctionActionSyntaxException("No conditional operators are allowed in the action number 0.");
                
                // Should declare additional variable since the action is ternary.
                // -
                double c;
                
                operand1 = currentActionString.GetFirstOperand();
                operand2 = currentActionString.GetSecondOperand();
                operand3 = currentActionString.GetThirdOperand();

                try
                {
                    a = GetOperandValue(operand1, _composedFunctions, _actionResults, actionIndex, argumentValue);
                }
				catch (Exception exception)
                {
					if (exception is FunctionActionUserThrownException)
					{
						throw;
					}

					throw new Exception($"Error while completing action {actionIndex}: {exception.Message}"); 
                }

                switch(currentActionString.GetActionSubstring())
                {
                    case ">": checkerAction = new CheckerAction(x => (x > 0)); break;
                    case "<": checkerAction = new CheckerAction(x => (x < 0)); break;
                    case "=": checkerAction = new CheckerAction(x => (x == 0)); break;
                    case ">=": checkerAction = new CheckerAction(x => (x >= 0)); break;
                    case "<=": checkerAction = new CheckerAction(x => (x <= 0)); break;
                    case "!=": checkerAction = new CheckerAction(x => (x != 0)); break;
                }

				try
				{
					if (checkerAction(a))
					{
						b = GetOperandValue(operand2, _composedFunctions, _actionResults, actionIndex, argumentValue);
						_actionResults[actionIndex] = b;
					}
					else
					{
						c = GetOperandValue(operand3, _composedFunctions, _actionResults, actionIndex, argumentValue);
						_actionResults[actionIndex] = c;
					}
				}
				catch (FunctionActionUserThrownException)
				{
					throw;
				}
				catch (Exception exception)
				{
					throw new FunctionActionExecutionException(exception.Message, actionIndex);
				}


				RETURNER: ; 
            }

            return _actionResults[_actionResults.Length - 1];
        }

		private string RemoveWhitespaceExceptInErrorMessages(string actionString)
		{
			StringBuilder stringWithoutWhitespace = new StringBuilder();

			bool isInsideErrorMessage = false;

			for (int characterIndex = 0; characterIndex < actionString.Length; ++characterIndex)
			{
				if (actionString[characterIndex] == '#')
				{
					isInsideErrorMessage = !isInsideErrorMessage;
				}
				else if (!char.IsWhiteSpace(actionString[characterIndex]) || isInsideErrorMessage)
				{
					stringWithoutWhitespace.Append(actionString[characterIndex]);
				}
			}

			return stringWithoutWhitespace.ToString();
		}

		private double GetOperandValue(
			string operandString, 
			IList<IFunction<double,double>> composedFunctions, 
			IList<double> actionResults, 
			int num, 
			double x)
        {
			if (string.IsNullOrEmpty(operandString))
			{
				throw new FunctionActionSyntaxException("One of the operands is missing.");
			}

            int negativeFlag = 1;

            // 'negflag' is multiplied by -1 if the operand
            // is preceded by a minus sign '-'.
            // 
            if (operandString[0] == '-') 
            { 
                negativeFlag = -1; 
                operandString = operandString.Substring(1); 
            }

            // Reference the digit value.
            // -
            if (char.IsDigit(operandString[0])) 
            { 
                try 
                { 
                    double tmp = double.Parse(
                        operandString.Replace(
							".", 
							CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)); 

					return negativeFlag * tmp; 
                } 
                catch 
                { 
                    throw new FunctionActionSyntaxException("Unknown operand in the action."); 
                } 
            }
            // Referencing the argument value.
            // -
            else if (operandString.Length == 1 && operandString[0] == '!')
            {
                return negativeFlag * x;
            }
            // Referencing the previous action result.
            // -
            else if (operandString.Length == 1 && operandString[0] == '$')
            {
                if (num <= 0) throw new FunctionActionSyntaxException("There is no previous action for action 0.");
                return negativeFlag * actionResults[num - 1];
            }
            // Referencing one of the action results by its number (index).
            // -
            else if (operandString[0] == '$')
            {
                if (operandString.Length <= 2 || operandString.LastIndexOf('$') != operandString.Length - 1) throw new Exception("Ошибка синтаксиса действия: неверно указано обращение к результату действия по номеру.");

                operandString = operandString.Replace("$", "");
                
				int callOperationNum = -1;

                try
                {
                    callOperationNum = int.Parse(operandString);
                }
                catch 
				{ 
					throw new FunctionActionSyntaxException("неизвестный номер действия, указанный в обращении."); 
				}

				if (callOperationNum >= num)
				{
					throw new FunctionActionSyntaxException("попытка рефлексивного обращения в действии или попытка обращения к еще не выполненному (следующему) действию.");
				}

				return negativeFlag * actionResults[callOperationNum];
            }
			// Call an inner function by its index.
			// -
            else if (operandString[0] == '%')
            {
                if (operandString.Length <= 2 || operandString.LastIndexOf('%') != operandString.Length - 1) throw new FunctionActionSyntaxException("неверно указано обращение к результату вложенной функции.");

                operandString = operandString.Replace("%", "");

				int composedFunctionIndex = -1;

                try
                {
                    composedFunctionIndex = int.Parse(operandString);
                }
                catch { throw new FunctionActionSyntaxException("Wrong inner function number specified."); }

				if (composedFunctionIndex < 0 || composedFunctionIndex >= composedFunctions.Count)
				{
					throw new FunctionActionSyntaxException("There is no inner function with such a number (numeration starts from 0!).");
				}

                return negativeFlag * composedFunctions[composedFunctionIndex].GetValue(x);
            }
			// User-defined error message.
			// -
			else if (operandString[0] == '#')
            {
				if (operandString.LastIndexOf('#') != operandString.Length - 1)
				{
					throw new FunctionActionSyntaxException("The user-defined error message in the action string should be located strictly in between two '#' symbols.");
				}

                throw new FunctionActionUserThrownException(operandString.Remove(0, 1).Remove(operandString.Length - 2));
            }

			throw new FunctionActionSyntaxException($"Error parsing operand '{operandString}'.");
        }

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
        public double GetValue(double x) { return 0; }
    }

    internal class FactorialLogarithmic : IFunction<double,double> // логарифмический факториал - высокая скорость, низкая точность
    {
        public double GetValue(double x)
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
        public long GetValue(long x)
        {
            if (x < 1) return 1;

            long tmp = 2;
            for (int i = 2; i <= x; i++) tmp *= i;

            return tmp;
        }
    }

    #endregion
}
