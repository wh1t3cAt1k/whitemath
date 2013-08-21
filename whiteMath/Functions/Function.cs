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

        delegate double singleAct(double x);
        delegate double doubleAct(double x, double y);
        delegate bool tripleCheck(double x);

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
        /// <param name="x"></param>
        /// <returns></returns>
        public double Value(double x) // вычисление значения функции по значению аргумента
        {
            singleAct act1;
            doubleAct act2;
            tripleCheck act3;

            actionResults = new double[actions.Count];

            for (int i=0; i<actions.Count; i++)
            {
                string str = actions[i];

                if (str == null || str.Trim() == "") throw new FunctionActionSyntaxException("cтрока действия за номером " + i + " является пустой.");

                act1 = new singleAct(Functions.Ret);
                act2 = new doubleAct(Functions.Sum);
                act3 = new tripleCheck(Functions.Equals);

                bool binary = true; // бинарное действие или унарное

                if (str.IndexOf('#') == -1) str = str.Replace(" ", ""); // удаляем все пробелы, если нет сообщений об ошибке
                else
                {
                    string tmp = "";
                    bool insideErrorMsg = false;

                    for (int j = 0; j < str.Length; j++)
                    {    
                        if (str[j] == '#') insideErrorMsg = !insideErrorMsg;
                        if (str[j] != ' ' || insideErrorMsg) tmp += str[j];
                    }

                    str = tmp;
                }

                double a, b;
                string operand1, operand2, operand3;

                switch(str.getActionSubString())
                {
                    case "ret": binary = false; act1 = new singleAct(Functions.Ret); break;
                    
                    case "+": act2 = new doubleAct(Functions.Sum); break;
                    case "-": act2 = new doubleAct(Functions.Dif); break;
                    case "*": act2 = new doubleAct(Functions.Mul); break;
                    case "/": act2 = new doubleAct(Functions.Div); break;
                    case "^": act2 = new doubleAct(Math.Pow); break;
                    
                    case "abs": binary = false; act1 = new singleAct(Math.Abs); break;
                    case "floor": binary = false; act1 = new singleAct(Math.Floor); break;
                    case "ceil": binary = false; act1 = new singleAct(Math.Ceiling); break;

                    case "sin": binary = false; act1 = new singleAct(Math.Sin); break;
                    case "cos": binary = false; act1 = new singleAct(Math.Cos); break;
                    case "tg": binary = false; act1 = new singleAct(Math.Tan); break;
                    case "ctg": binary = false; act1 = new singleAct(Functions.Ctg); break;

                    case "arcsin": binary = false; act1 = new singleAct(Math.Asin); break;
                    case "arccos": binary = false; act1 = new singleAct(Math.Acos); break;
                    case "arctg": binary = false; act1 = new singleAct(Math.Atan); break;
                    
                    case "sinh": binary = false; act1 = new singleAct(Math.Sinh); break;
                    case "cosh": binary = false; act1 = new singleAct(Math.Cosh); break;
                    
                    case "log": binary = true; act2 = new doubleAct(Math.Log); break;
                    case "lg": binary = false; act1 = new singleAct(Math.Log10); break;
                    
                    case "sqrt": binary = false; act1 = new singleAct(Math.Sqrt); break;
                    case "ln": binary = false; act1 = new singleAct(Math.Log); break;
                    
                    case "exp": binary = false; act1 = new singleAct(Math.Exp); break;
                    case "sign": binary = false; act1 = new singleAct(Functions.Sgn); break;
                    
                    case ">": case "=": case ">=": case "<=": case "<": case "!=": goto CONDITION;
                    default: throw new FunctionActionSyntaxException("неизвестное обозначение функции в списке действий."); 
                }

                if (binary)
                {
                    operand1 = str.getFirstOperand();
                    operand2 = str.getSecondOperand();

                    try
                    {
                        a = operandMeaning(operand1, composedFunc, actionResults, i, x);
                        b = operandMeaning(operand2, composedFunc, actionResults, i, x);
                    }
                    catch (Exception xxx)
                    { throw new Exception("Ошибка при выполнении действия " + i + ". " + xxx.Message); }

                    double res; // результат операции
                    
                    try { res = act2(a, b); }
                    catch { res = double.NaN; }

                    actionResults[i] = res;
                    goto RETURNER;
                }
                else
                {
                    operand1 = str.getFirstOperand();

                    try { a = operandMeaning(operand1, composedFunc, actionResults, i, x); }
                    catch (Exception xxx)
                    {
                        if (xxx is FunctionActionUserThrownException) throw;
                        throw new Exception("Ошибка при выполнении действия " + i + ". " + xxx.Message); 
                    }

                    double res;

                    try { res = act1(a); }
                    catch { res = double.NaN; }

                    actionResults[i] = res;
                    goto RETURNER;
                }

            CONDITION: // выполняется в случае условного оператора действия: >, = , } 

                if (i == 0) throw new FunctionActionSyntaxException("недопустимо использование условных операторов в нулевом действии.");
                
                // объявим дополнительные переменные, так как действие тернарное.
                double c;
                
                operand1 = str.getFirstOperand();
                operand2 = str.getSecondOperand();
                operand3 = str.getThirdOperand();

                try
                {
                    a = operandMeaning(operand1, composedFunc, actionResults, i, x);
                }
                catch (Exception xxx)
                {
                    if (xxx is FunctionActionUserThrownException) throw;
                    throw new Exception("Ошибка при выполнении действия " + i + ". " + xxx.Message); 
                }

                switch(str.getActionSubString())
                {
                    case ">": act3 = new tripleCheck(Functions.More); break;
                    case "<": act3 = new tripleCheck(Functions.Less); break;
                    case "=": act3 = new tripleCheck(Functions.Equals); break;
                    case ">=": act3 = new tripleCheck(Functions.MoreQ); break;
                    case "<=": act3 = new tripleCheck(Functions.LessQ); break;
                    case "!=": act3 = new tripleCheck(Functions.EqualsNot); break;
                }

                if (act3(a)) 
                    try
                        {   
                            b = operandMeaning(operand2, composedFunc, actionResults, i, x);
                            actionResults[i] = b;
                        }
                    catch (Exception xxx)
                        {
                            if (xxx is FunctionActionUserThrownException) throw;
                            throw new Exception("Ошибка при выполнении действия " + i + ". " + xxx.Message); 
                        }                        
                else
                    try
                        {
                            c = operandMeaning(operand3, composedFunc, actionResults, i, x);
                            actionResults[i] = c;
                        }
                    catch (Exception xxx)
                        {
                            if (xxx is FunctionActionUserThrownException) throw;
                            throw new Exception("Ошибка при выполнении действия " + i + ". " + xxx.Message);
                        }
            
            RETURNER: ; 
            }

            return actionResults[actionResults.Length - 1];
        }

        private double operandMeaning(string operand, List<IFunction<double,double>> composedFunc, double[] actionResults, int num, double x)
        {
            if (operand == null || operand == "") throw new FunctionActionSyntaxException("не указан один из операндов.");

            int negflag=1;
            // если перед операндом стоит знак "минус", то negflag умножается на -1.
            if (operand[0] == '-') { negflag = -1; operand = operand.Substring(1); }

            if (char.IsDigit(operand[0])) // обращение к числовому значению
            { 
                try 
                { 
                    double tmp = double.Parse(
                        operand.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)); return negflag * tmp; 
                } 
                catch 
                { 
                    throw 
                        new Exception("Ошибка синтаксиса действия: неизвестный операнд в команде-действии."); 
                } 
            }
            else if (operand.Length == 1 && operand[0] == '!') return negflag * x; // обращение к значению аргумента
            else if (operand.Length == 1 && operand[0] == '$') // обращение к предыдущему действия
            {
                if (num <= 0) throw new FunctionActionSyntaxException("обращение к предыдущему действию из нулевого действия.");
                return negflag * actionResults[num - 1];
            }
            else if (operand[0] == '$') // обращение к результату выполненного действия по номеру
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
                return negflag * actionResults[callOperationNum]; // возвращаем значение действия по номеру
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

                return negflag * composedFunc[callFuncNum].Value(x);
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
        internal static double Sum(double a, double b)
        { return a+b; }

        internal static double Dif(double a, double b)
        { return a-b; }

        internal static double Mul(double a, double b)
        { return a*b; }

        internal static double Div(double a, double b)
        { return a/b; }

        internal static double Ret(double a)
        { return a; }

        internal static double Ctg(double a)
        { return 1 / Math.Tan(a); }

        internal static double Sgn(double a)
        { return Math.Sign(a); }

        internal static bool More(double a)
        { return (a > 0); }

        internal static bool Less(double a)
        { return (a < 0); }

        internal static bool MoreQ(double a)
        { return (a >= 0); }

        internal static bool LessQ(double a)
        { return (a <= 0); }

        internal static bool Equals(double a)
        { return (a == 0); }

        internal static bool EqualsNot(double a)
        { return (a != 0); }

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
