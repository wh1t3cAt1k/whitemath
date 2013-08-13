using System;
using System.Collections.Generic;

using whiteMath.General;

using System.Reflection;

namespace whiteMath.Functions
{
    /// <summary>
    /// Represents a polynomial function of certain degree.
    /// The function is determined by the coefficients' array.
    /// It is supposed that coefficients of the polynom are all REAL.
    /// 
    /// Warning! This class DOES NOT WORK with complex number types for coefficients!
    /// </summary>
    /// <typeparam name="T">The type of the polynom coefficients and values.</typeparam>
    /// <typeparam name="C">The calculator for the coefficient values.</typeparam>
    public class Polynom<T, C> : IFunctionDifferentiable<T, T>, ICloneable where C : ICalc<T>, new()
    {
        private static ICalc<T> calc = Numeric<T, C>.Calculator;

        // Коэффициенты хранятся, начиная с нулевой степени
        // многочлена.
        private Numeric<T, C>[] coefficients;

        /// <summary>
        /// Returns the degree of the polynom, that is, the
        /// maximum power of the X in the analytic notation.
        /// </summary>
        public int Degree { get { return coefficients.Length - 1; } }

        /// <summary>
        /// Tests whether the current polynom equals zero.
        /// </summary>
        public bool IsZero { get { return this.Degree == 0 && this[0] == Numeric<T, C>.Zero; } }

        /// <summary>
        /// Returns the polynom normed by its high-order coefficient.
        /// The high-order coefficient of the resulting polynom would be equal to 1.
        /// 
        /// The polynom returned would contain all the same roots as the current.
        /// </summary>
        public Polynom<T, C> NormedPolynom
        {
            get
            {
                Numeric<T, C>[] newCoefs = new Numeric<T, C>[this.coefficients.Length];

                for (int i = 0; i < newCoefs.Length; i++)
                    newCoefs[i] = this.coefficients[i] / this.coefficients[this.coefficients.Length - 1];

                return new Polynom<T, C>(newCoefs);
            }
        }

        /// <summary>
        /// Returns the polynom coefficient at the specified X power.
        /// </summary>
        /// <param name="index">The X power which coefficient is to be returned.</param>
        /// <returns>The respective polynom coefficient at the specified X power.</returns>
        public Numeric<T, C> this[int index]
        {
            get { return coefficients[index]; }
        }

        // ----------------------------------
        // ------- DERIVATIVE GETTING -------
        // ----------------------------------

        // Неявная реализация интерфейса.
        IFunctionDifferentiable<T, T> IFunctionDifferentiable<T, T>.Derivative
        {
            get { return this.Derivative(1); }
        }

        /// <summary>
        /// Returns the derivative of a certain degree for the polynom.
        /// If the current polynom is of degree N, its derivative of degree D 
        /// is also a polynomial function of degree Max(0; N-D).
        /// </summary>
        /// <param name="derivativeDegree"></param>
        /// <returns></returns>
        public Polynom<T, C> Derivative(int derivativeDegree)
        {
            if (derivativeDegree == 0)
                return this;
            else if (derivativeDegree < 0)
                throw new ArgumentException("Derivative degree should be a non-negative integer value.");
            else if (derivativeDegree > this.Degree)
                return new Polynom<T, C>(new Numeric<T, C>[] { Numeric<T, C>.Zero });

            int newCoefCount = this.coefficients.Length - derivativeDegree;

            Numeric<T, C>[] coefficients = new Numeric<T, C>[newCoefCount];

            for (int i = 0; i < newCoefCount; i++)
            {
                coefficients[i] = this.coefficients[i + derivativeDegree];

                for (int j = 0; j < derivativeDegree; j++)
                    coefficients[i] *= calc.fromInt(i + derivativeDegree - j);
            }

            return new Polynom<T, C>(coefficients);
        }

        /// <summary>
        /// Returns the polynom that is the multiple integral of the current polynom
        /// with specified integral degree.
        /// 
        /// Equivalent to taking a single integral (each time - with zero free constant value)
        /// several times - but works significantly faster.
        /// 
        /// All coefficients of the resulting polynom up to index 'integralDegree'
        /// are set to zero.
        /// </summary>
        /// <param name="integralDegree">The degree of the integral.</param>
        /// <returns>The multiple integral of the current polynom.</returns>
        public Polynom<T, C> IntegralMultiple(int integralDegree)
        {
            if (integralDegree < 0)
                throw new ArgumentException("The integral degree should be a non-negative value.");
            else if (integralDegree == 0)
                return this.Clone() as Polynom<T, C>;

            Numeric<T, C>[] newCoefs = new Numeric<T, C>[coefficients.Length + integralDegree];

            for (int i = 0; i < coefficients.Length; i++)
            {
                Numeric<T, C> product = (Numeric<T, C>)(i + 1);

                for (int j = 2; j <= integralDegree; j++)
                    product *= (Numeric<T, C>)(i + j);

                newCoefs[i + integralDegree] = this.coefficients[i] / product;
            }

            return new Polynom<T, C>(newCoefs);
        }

        /// <summary>
        /// Returns the polynom that is the integral of the current polynom
        /// with specified free constant value.
        /// </summary>
        /// <param name="freeConstant">The value of the integral's free constant.</param>
        /// <returns>The integral polynom object.</returns>
        public Polynom<T, C> Integral(Numeric<T, C> freeConstant)
        {
            Numeric<T, C>[] newCoefficients = new Numeric<T, C>[coefficients.Length + 1];

            newCoefficients[0] = freeConstant;

            for (int i = 0; i < coefficients.Length; i++)
                newCoefficients[i + 1] = coefficients[i] / (Numeric<T, C>)(i + 1);

            return new Polynom<T, C>(newCoefficients);
        }

        /// <summary>
        /// Returns the polynom that is the integral of the current polynom,
        /// which satisfies the condition of passing the specified point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Polynom<T, C> Integral(Point<T> point)
        {
            Polynom<T, C> newPolynom = this.Integral(Numeric<T, C>.Zero);
            newPolynom.coefficients[0] = calc.dif(point.Y, newPolynom.Value(point.X));

            return newPolynom;
        }

        // ----------------------------
        // --------- value ------------
        // ----------------------------

        public T Value(T x)
        {
            Numeric<T, C> sum = calc.zero;

            for (int i = coefficients.Length - 1; i >= 0; --i)
            {
                sum = sum * x + coefficients[i];
            }

            return sum;
        }

        // ----------------------------
        // --------- ctors ------------
        // ----------------------------

        /// <summary>
        /// Creates a new instance of Polynom object basing on the coefficients.
        /// </summary>
        /// <param name="coefficients"></param>
        public Polynom(params Numeric<T, C>[] coefficients)
            : this(coefficients as IList<Numeric<T, C>>)
        { }

        /// <summary>
        /// Creates a new instance of Polynom object basing on the coefficients
        /// array.
        /// </summary>
        /// <param name="coefficients"></param>
        public Polynom(IList<Numeric<T, C>> coefficients)
        {
            // Нули при старших степенях нас не интересуют.
            int countSignificant = coefficients.CountSignificant();

            this.coefficients = new Numeric<T, C>[countSignificant];
            General.ServiceMethods.Copy(coefficients, 0, this.coefficients, 0, countSignificant);
        }

        /// <summary>
        /// Creates the polynom passing each of the points in the array.
        /// </summary>
        public Polynom(params Point<T>[] points)
            : this(points as IList<Point<T>>)
        { }

        /// <summary>
        /// Creates the polynom passing each of the points in the array.
        /// </summary>
        /// <param name="points">The array of the points.</param>
        public Polynom(IList<Point<T>> points)
        {
            int n = points.Count;

            Vector<T, C> frees = new Vector<T, C>(n);

            // --------- PointNames Y

            for (int i = 0; i < n; i++)
                frees[i] = points[i].Y;

            // --------- Filling the matrix

            Matrices.Matrix_SDA<T, C> matrix = new Matrices.Matrix_SDA<T, C>(n, n);

            Numeric<T, C>[] argument = new Numeric<T, C>[n];
            argument.FillByAssign(calc.fromInt(1));

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = argument[i];
                    argument[i] *= points[i].X;
                }

            Vector<T, C> result;

            Matrices.SLAESolving.LU_FactorizationSolving(matrix, frees, out result);

            // Конвертируем в массив и обрезаем ведущие нули.
            // Радуемся жизни.
            this.coefficients = result.AsNumericArray().Cut();

            return;
        }

        // ------------------------------
        // ---------- to functions ------
        // ------------------------------

        /// <summary>
        /// Creates an analytic function from the polynomial.
        /// The calculation of its value would be performed without
        /// any Gorner's scheme, thus slower and less accurate. 
        /// </summary>
        /// <returns></returns>
        public AnalyticFunction ToAnalyticFunction()
        {
            return new AnalyticFunction(this.ToString(PolynomStringRepresentationType.Functional));
        }

        // ------------------------------
        // -------- operators -----------
        // ------------------------------

        // Сложение
        public static Polynom<T, C> operator +(Polynom<T, C> one, Polynom<T, C> two)
        {
            Numeric<T, C>[] coefficients = new Numeric<T, C>[Math.Max(one.coefficients.Length, two.coefficients.Length)];

            for (int i = 0; i < coefficients.Length; i++)
                if (i < one.coefficients.Length && i < two.coefficients.Length)
                    coefficients[i] = one.coefficients[i] + two.coefficients[i];
                else if (i < one.coefficients.Length)
                    coefficients[i] = one.coefficients[i];
                else
                    coefficients[i] = two.coefficients[i];

            return new Polynom<T, C>(coefficients);
        }

        // Негатификация
        public static Polynom<T, C> operator -(Polynom<T, C> one)
        {
            Numeric<T, C>[] newCoefs = new Numeric<T, C>[one.coefficients.Length];

            for (int i = 0; i < one.coefficients.Length; i++)
                newCoefs[i] = -one.coefficients[i];

            return new Polynom<T, C>(newCoefs);
        }

        // Вычитание
        public static Polynom<T, C> operator -(Polynom<T, C> one, Polynom<T, C> two)
        {
            Numeric<T, C>[] coefficients = new Numeric<T, C>[Math.Max(one.coefficients.Length, two.coefficients.Length)];

            for (int i = 0; i < coefficients.Length; i++)
                if (i < one.coefficients.Length && i < two.coefficients.Length)
                    coefficients[i] = one.coefficients[i] - two.coefficients[i];
                else if (i < one.coefficients.Length)
                    coefficients[i] = one.coefficients[i];
                else
                    coefficients[i] = -two.coefficients[i];

            return new Polynom<T, C>(coefficients);
        }

        // Умножение. Квадратишное.
        public static Polynom<T, C> operator *(Polynom<T, C> one, Polynom<T, C> two)
        {
            Numeric<T, C>[] coefficients = new Numeric<T, C>[one.Degree + two.Degree + 1];

            for (int i = 0; i < one.coefficients.Length; i++)
                for (int j = 0; j < two.coefficients.Length; j++)
                    coefficients[i + j] += one[i] * two[j];

            return new Polynom<T, C>(coefficients);
        }

        public static Polynom<T, C> operator *(Polynom<T, C> one, Numeric<T, C> number)
        {
            Numeric<T, C>[] coefficients = new Numeric<T, C>[one.coefficients.Length];

            for (int i = 0; i < coefficients.Length; i++)
                coefficients[i] = one.coefficients[i] * number;

            return new Polynom<T, C>(coefficients);
        }

        public static Polynom<T, C> operator /(Polynom<T, C> one, Polynom<T, C> two)
        {
            Polynom<T, C> junk;

            return Div(one, two, out junk);
        }

        public static Polynom<T, C> operator %(Polynom<T, C> one, Polynom<T, C> two)
        {
            Polynom<T, C> rem;

            Div(one, two, out rem);

            return rem;
        }

        public static bool operator ==(Polynom<T, C> one, Polynom<T, C> two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Polynom<T, C> one, Polynom<T, C> two)
        {
            return !one.Equals(two);
        }

        /// <summary>
        /// Performs the polynom division with the remainder.
        /// </summary>
        /// <param name="one">The dividend polynom.</param>
        /// <param name="two">The divisor polynom.</param>
        /// <param name="rem">The reference to the polynom to which the remainder result will be assigned.</param>
        /// <returns>The division result.</returns>
        public static Polynom<T, C> Div(Polynom<T, C> one, Polynom<T, C> two, out Polynom<T, C> rem)
        {
            if (one.Degree < two.Degree)
            {
                rem = one.Clone() as Polynom<T, C>;
                return new Polynom<T, C>(Numeric<T, C>.Zero);
            }

            Numeric<T, C>[] oneCopy = new Numeric<T, C>[one.coefficients.Length];
            Numeric<T, C>[] result = new Numeric<T, C>[one.Degree - two.Degree + 1];

            ServiceMethods.Copy(one.coefficients, oneCopy);

            for (int i = oneCopy.Length - 1; i >= two.Degree; i--)
            {
                // Очередной коэффициент результата
                result[result.Length - oneCopy.Length + i] = oneCopy[i] / two.coefficients[two.coefficients.Length - 1];

                // При обратном умножении получаем ноль в соответствующем коэффициенте.
                oneCopy[i] = Numeric<T, C>.Zero;

                // Теперь из делимого вычтем делитель, умноженный на полученный коэффициент.
                for (int j = two.coefficients.Length - 2; j >= 0; j--)
                    oneCopy[i + j - two.coefficients.Length + 1] -= two.coefficients[j] * result[result.Length - oneCopy.Length + i];
            }

            rem = new Polynom<T, C>(oneCopy);

            return new Polynom<T, C>(result);
        }

        // ------------------------------
        // -------- equality test -------
        // ------------------------------

        public override bool Equals(object obj)
        {
            if (obj is Polynom<T, C>)
                return this.Equals(obj as Polynom<T, C>);

            else if (obj is LagrangePolynom<T, C>)
                return this.Equals(obj as LagrangePolynom<T, C>);

            return false;
        }

        /// <summary>
        /// Tests whether the current polynom is equal
        /// to another using objects' coefficients lists.
        /// </summary>
        /// <param name="polynom">The object to test equality with.</param>
        /// <returns>True if the current polynom is equal to the passed object, false otherwise.</returns>
        public bool Equals(Polynom<T, C> polynom)
        {
            for (int i = 0; i < this.coefficients.Length; i++)
                if (this.coefficients[i] != polynom.coefficients[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Tests whether the current polynom is equal
        /// to a certain LagrangePolynom by converting the latter
        /// to canonical polynomial form and comparing the coefficients.
        /// </summary>
        /// <param name="polynom">The object to test equality with.</param>
        /// <returns>True if the current polynom is equal to the passed object, false otherwise.</returns>
        public bool Equals(LagrangePolynom<T, C> polynom)
        {
            return this.Equals(polynom.AsStandardPolynom);
        }

        // ------------------------------
        // -------- string --------------
        // ------------------------------

        /// <summary>
        /// Determines the type of polynom's string representation
        /// in the ToString() method.
        /// </summary>
        public enum PolynomStringRepresentationType
        {
            Functional, Coefficients
        }

        public override string ToString()
        {
            return ToString(PolynomStringRepresentationType.Functional);
        }

        /// <summary>
        /// Returns the string representation of the polynomial function
        /// as an array of coefficients - or, in the functional form
        /// i.e. 'f(x) = ...'.
        /// </summary>
        /// <param name="type">The type of the string representation.</param>
        /// <returns>The string representation of the current object.</returns>
        public string ToString(PolynomStringRepresentationType type)
        {
            if (type == PolynomStringRepresentationType.Coefficients)
            {
                string coefString = "";

                for (int i = 0; i < coefficients.Length; i++)
                    coefString += "|" + this.coefficients[i].ToString();

                coefString += "|";

                return coefString;
            }
            else
            {
                string functionString = "f(x) = ";

                Numeric<T, C> one = calc.fromInt(1);

                for (int i = coefficients.Length - 1; i > 0; i--)
                    if (coefficients[i] != Numeric<T, C>.Zero)
                        functionString += string.Format("{0}{1}x{2}", (coefficients[i] >= calc.zero ? (i<coefficients.Length-1?" + ":"") : (i<coefficients.Length-1?" - ":"-")), (coefficients[i] != one ? WhiteMath<T, C>.Abs(coefficients[i]).ToString() : ""), (i == 1 ? "" : "^" + i.ToString()));

                if(coefficients[0] != Numeric<T,C>.Zero)
                    functionString += string.Format(" {0} {1} ", (coefficients[0] > Numeric<T,C>.Zero ? "+" : "-"), WhiteMath<T, C>.Abs(coefficients[0]));

                return functionString;
            }
        }

        // --------------------------------------
        // ----------- Cloneable ----------------
        // --------------------------------------

        /// <summary>
        /// Creates a deep copy of the current Polynom object.
        /// </summary>
        /// <returns>A deep copy of the current Polynom object.</returns>
        public object Clone()
        {
            return new Polynom<T, C>(this.coefficients);
        }

        // --------------------------------------
        // ----------- GCD ----------------------
        // --------------------------------------

        /// <summary>
        /// Finds the greatest common divisor of two polynoms.
        /// 
        /// You may wish to cancel the resulting polynom
        /// as its coefficients may be multiples of the same number.
        /// </summary>
        /// <param name="first">The first polynom.</param>
        /// <param name="second">The second polynom.</param>
        /// <returns>The greatest common divisor of </returns>
        public static Polynom<T, C> GreatestCommonDivisor(Polynom<T, C> first, Polynom<T, C> second)
        {
            Polynom<T, C> one = (first.Degree > second.Degree ? first.Clone() : second.Clone()) as Polynom<T, C>;
            Polynom<T, C> two = (first.Degree > second.Degree ? second.Clone() : first.Clone()) as Polynom<T, C>;

            Polynom<T, C> rem1, rem2;

            while (true)
            {
                Div(one, two, out rem1);

                if (rem1.IsZero)
                    return two;

                Div(two, rem1, out rem2);

                if (rem2.IsZero)
                    return rem1;

                one = rem1;
                two = rem2;
            }
        }

        /// <summary>
        /// Returns the greatest common divisor of the current
        /// polynom and the polynom passed as the argument.
        /// 
        /// You may wish to cancel the returning polynom
        /// as the coefficients may be multiples of the same number.
        /// </summary>
        /// <param name="another">The second polynom.</param>
        /// <returns>The greatest common divisor of the current polynom and the other.</returns>
        public Polynom<T, C> GreatestCommonDivisor(Polynom<T, C> another)
        {
            return GreatestCommonDivisor(this, another);
        }

        /// <summary>
        /// Cancels the polynom with integer coefficients by the greatest common divisor of
        /// its coefficients.
        /// 
        /// The resulting polynom contains all the same roots, but its value range is less than
        /// of the current.
        /// 
        /// If either some of the coefficients isn't integer or GCD = 1, the current polynom will be returned.
        /// <returns>The polynom with cancelled coefficients containing all the same roots.</returns>
        /// </summary>
        public Polynom<T, C> IntegerCoefficientsCancel()
        {
            ArgumentException ex = new ArgumentException("Only polynoms with integer coefficients can be cancelled.");

            if (coefficients[0].FractionalPart != Numeric<T, C>.Zero)
                throw ex;

            Numeric<T, C> gcd = coefficients[0];

            for (int i = 1; i < coefficients.Length; i++)
            {
                if (coefficients[i].FractionalPart != Numeric<T, C>.Zero)
                    throw ex;
                else
                    gcd = WhiteMath<T, C>.GreatestCommonDivisor(gcd, coefficients[i]);
            }

            Numeric<T, C>[] newCoefs = new Numeric<T, C>[coefficients.Length];

            for (int i = 0; i < coefficients.Length; i++)
                newCoefs[i] = this.coefficients[i] / gcd;

            return new Polynom<T, C>(newCoefs);
        }

        // --------------------------------------
        // ----------- ROOT FINDING -------------
        // --------------------------------------

        /// <summary>
        /// Returns the interval in which all of the polynom
        /// real roots are.
        /// </summary>
        public BoundedInterval<T, C> RealRootGuaranteedInterval
        {
            get
            {
                T nCoef = WhiteMath<T, C>.Abs(coefficients[coefficients.Length - 1]);
                T max = calc.zero;

                for (int i = 0; i < coefficients.Length; i++)
                {
                    T cur = WhiteMath<T, C>.Abs(coefficients[i]);

                    if (calc.mor(cur, max))
                        max = WhiteMath<T, C>.Abs(cur);
                }

                T upper = calc.sum(calc.fromInt(1), calc.div(max, nCoef));

                return new BoundedInterval<T, C>(calc.negate(upper), upper);
            }
        }

        /// <summary>
        /// Returns the interval in which all of the polynom's positive roots are.
        /// Uses the Lagrange theorem.
        /// </summary>
        public BoundedInterval<T, C> RealRootPositiveInterval
        {
            get
            {
                Func<int, Numeric<T, C>, Numeric<T, C>> func = delegate(int i, Numeric<T, C> obj)
                    {
                        return obj;
                    };

                T one = _PositiveLagrangeBound(new ReverseList<Numeric<T, C>>(coefficients), func);
                T two = _PositiveLagrangeBound(coefficients, func);

                if (one == Numeric<T, C>.Zero)
                    return new BoundedInterval<T, C>(calc.zero, calc.zero);

                return new BoundedInterval<T, C>(calc.div(calc.fromInt(1), one), two);
            }
        }

        /// <summary>
        /// Returns the interval in which all of the polynom's positive roots are.
        /// Uses the Lagrange theorem.
        /// </summary>
        public BoundedInterval<T, C> RealRootsNegativeInterval
        {
            get
            {
                // Для неперевернутого листа знаки должны зависеть от четности.

                Func<int, Numeric<T, C>, Numeric<T, C>> func = delegate(int i, Numeric<T, C> obj)
                {
                    if (i % 2 == 0)
                        return obj;
                    else
                        return -obj;
                };

                // Поскольку мы делаем ReverseList, при четном количестве коэффициентов
                // знаки, возвращаемые func для тех же элементов что и в неперевернутом - поменяются. 
                // Этого не должно быть.
                // Для перевернутого листа знаки должны сохраняться.
                // Вводим новую функцию.

                int len = this.coefficients.Length;

                Func<int, Numeric<T, C>, Numeric<T, C>> func2 = delegate(int i, Numeric<T, C> obj)
                {
                    if ((len - i - 1) % 2 == 0)
                        return obj;
                    else
                        return -obj;
                };

                T one = _PositiveLagrangeBound(coefficients, func);
                T two = _PositiveLagrangeBound(new ReverseList<Numeric<T, C>>(coefficients), func2);

                if (two == Numeric<T, C>.Zero)
                    return new BoundedInterval<T, C>(calc.zero, calc.zero);

                return new BoundedInterval<T, C>(calc.negate(one), calc.negate(calc.div(calc.fromInt(1), two)));
            }
        }

        // --------- this is the key to happiness

        private static T _PositiveLagrangeBound(IList<Numeric<T, C>> coefficients, Func<int, Numeric<T, C>, Numeric<T, C>> selector)
        {
            // нули в начале нам совсем не нужны.

            int minus = 1;

            while (coefficients[coefficients.Count - minus] == Numeric<T, C>.Zero)
            {
                minus++;

                if (minus > coefficients.Count)
                    throw new ArgumentException("This polynom contains only zero coefficients. Cannot evaluate the finite roots interval.");
                // todo - return INFINITY.
            }

            // ------- начальный коэффициент - меньше нуля?
            // заменяем функцию g(x) = -f(x) и сохраняем корни и нервы.

            if (selector(coefficients.Count - minus, coefficients[coefficients.Count - minus]) < Numeric<T, C>.Zero)
            {
                object target = selector.Target;
                MethodInfo method = selector.Method;

                selector = delegate(int i, Numeric<T, C> obj) { return -(Numeric<T, C>)method.Invoke(target, new object[] { i, obj }); };
            }

            // -----------------

            T nCoef = coefficients[coefficients.Count - minus];

            T cur;

            T one = calc.fromInt(1);
            T max = calc.zero;

            int index = -1;         // индекс первого отрицательного коэффициента
            bool found = false;     // найдены ли отрицательные коэффициенты!

            for (int i = coefficients.Count - minus; i >= 0; i--)
            {
                cur = selector(i, coefficients[i]);

                if (!found && calc.mor(calc.zero, cur))
                {
                    found = true;
                    index = coefficients.Count - i - minus;
                }

                // -- здесь cur больше не нужна, а нужно только ее
                // абсолютное значение. его и используем.

                cur = WhiteMath<T, C>.Abs(cur);

                if (calc.mor(cur, max))
                    max = cur;
            }

            if (!found)
                return calc.zero;

            return calc.sum(one, WhiteMath<T, C>.Power(calc.div(max, WhiteMath<T, C>.Abs(nCoef)), calc.div(one, calc.fromInt(index))));
        }

        // ----------- still roots here

        /// <summary>
        /// Returns the Sturm polynom sequence for the current polynom.
        /// Used in Sturm theorem which helps to find the number of real roots
        /// contained in some interval.
        /// 
        /// nrr(f(x), x in (a; b)) = V(S(a)) - V(S(b));
        /// 
        /// Where V(S(x)) is number of sign changes in the polynom's Sturm sequence.
        /// Zeroes of the sequence do not count.
        /// 
        /// For example, if the Sturm sequence at some point 'x' produces values -1, 1, 0, 1, then V(S(x)) = 1.
        /// </summary>
        /// <returns>The Sturm polynom sequence for the current polynom.</returns>
        public Polynom<T, C>[] SturmPolynomSequence
        {
            get
            {
                // вырожденный случай:

                if (this.Degree == 0)
                    return new Polynom<T, C>[] { this.Clone() as Polynom<T, C> };

                // ------------------
                // как максимум нам потребуется столько же членов, сколько коэффициентов -
                // если нет кратных корней. иначе меньше.

                // первый член. Избавляемся от всех кратных корней.

                Polynom<T, C> first = this / GreatestCommonDivisor(this, this.Derivative(1));
                Polynom<T, C> second = first.Derivative(1);

                // дальше по накатанной.

                List<Polynom<T, C>> sequence = new List<Polynom<T, C>>(first.coefficients.Length);

                sequence.Add(first);
                sequence.Add(second);

                int i = 2;

                while (true)
                {
                    if (sequence[i - 1].Degree == 0)
                        return sequence.ToArray();
                    else
                    {
                        // квадратный трехчлен. надо проверять дискриминант.
                        if (sequence[i - 1].Degree == 2)
                        {
                            Numeric<T, C> discriminant = sequence[i - 1][1] * sequence[i - 1][1] - calc.fromInt(4) * sequence[i - 1][0] * sequence[i - 1][2];

                            if (discriminant < Numeric<T, C>.Zero)
                                return sequence.ToArray();
                        }

                        sequence.Add(-(sequence[i - 2] % sequence[i - 1]));
                        ++i;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the overall amount of different real polynom roots.
        /// Some of the roots may be multiple.
        /// </summary>
        /// <returns>The overall amount of different real polynom roots.</returns>
        public int DifferentRealRootsAmount()
        {
            int rootsAmount;

            // Генерируем последовательность Штурма для приведенного полинома.
            Polynom<T, C>[] sturmSeq = this.SturmPolynomSequence;

            // Количество корней равно разности количества вариаций в +inf и в -inf.
            rootsAmount = sturmSeq.InfinitySignVariations(true) - sturmSeq.InfinitySignVariations(false);

            // Общее количество корней не включает кратные.
            return rootsAmount;
        }
    }

    public static class PolynomExtensions
    {
        /// <summary>
        /// Gets the amount of polynom sequence's sign variations either
        /// at positive or negative infinity using polynom's degree and the sign of its higher coefficient.
        /// 
        /// Works fast.
        /// </summary>
        /// <typeparam name="T">The type of polynom coefficients.</typeparam>
        /// <typeparam name="C">The calculator for the coefficients' type.</typeparam>
        /// <param name="list">The sequence of polynoms to be evaluated for sign variations.</param>
        /// <param name="negativeInfinity">If this flag is set to FALSE, the evalution is at positive infinity. Otherwise, at negative infinity.</param>
        /// <returns>The number of polynom's sign variations.</returns>
        public static int InfinitySignVariations<T,C>(this IList<Polynom<T,C>> list, bool negativeInfinity) where C: ICalc<T>, new()
        {
            int signPrev = 0;
            int signNew = 0;
            int k = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Degree == 0)
                    signNew = WhiteMath<T, C>.Sign(list[i][0]);
                else
                    signNew = WhiteMath<T, C>.Sign(list[i][list[i].Degree]) *
                            (negativeInfinity ? ((list[i].Degree % 2 == 0 ? 1 : -1)) : 1);

                if (signNew == 0)
                    continue;
                else if (signNew != signPrev && signPrev != 0)
                    k++;
                
                signPrev = signNew;
            }

            return k;
        }
    }
}
