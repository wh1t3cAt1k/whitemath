using System;
using System.Collections.Generic;
using System.Reflection;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Vectors;

namespace WhiteMath.Functions
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

        // The coefficients are stored as an array
		// indexed by the power of X.
		// -
		private Numeric<T, C>[] _coefficients;

        /// <summary>
        /// Returns the degree of the polynom, that is, the
        /// maximum power of X in the analytic notation.
        /// </summary>
        public int Degree => _coefficients.Length - 1;

        /// <summary>
        /// Tests whether the current polynom equals zero.
        /// </summary>
        public bool IsZero => 
			this.Degree == 0 
	        && this[0] == Numeric<T, C>.Zero;

        /// <summary>
        /// Returns the polynom normalized by its high-order coefficient.
        /// The highest-order coefficient of the resulting polynom would be equal to 1.
        /// The polynom returned would contain all the same roots as the current.
        /// </summary>
		public Polynom<T, C> NormalizedPolynom
        {
            get
            {
				Numeric<T, C>[] newCoefficients = new Numeric<T, C>[this._coefficients.Length];

				for (int i = 0; i < newCoefficients.Length; ++i)
				{
					newCoefficients[i] = this._coefficients[i] / this._coefficients[this._coefficients.Length - 1];
				}

                return new Polynom<T, C>(newCoefficients);
            }
        }

		/// <summary>
		/// Returns the polynom coefficient at the specified power of X.
		/// </summary>
		/// <param name="index">The X power which coefficient is to be returned.</param>
		/// <returns>The respective polynom coefficient at the specified X power.</returns>
		public Numeric<T, C> this[int index] => _coefficients[index];

		IFunctionDifferentiable<T, T> IFunctionDifferentiable<T, T>.Derivative 
			=> GetDerivative(1);

        /// <summary>
        /// Returns the derivative of a certain degree for the polynom.
        /// If the current polynom is of degree N, its derivative of degree D 
        /// is also a polynomial function of degree Max(0; N-D).
        /// </summary>
        /// <param name="derivativeDegree"></param>
        /// <returns></returns>
		public Polynom<T, C> GetDerivative(int derivativeDegree)
        {
			if (derivativeDegree == 0)
			{
				return this.Clone() as Polynom<T, C>;
			}
			else if (derivativeDegree < 0)
			{
				throw new ArgumentException("Derivative degree should be a non-negative integer value.");
			}
			else if (derivativeDegree > this.Degree)
			{
				return new Polynom<T, C>(new Numeric<T, C>[] { Numeric<T, C>.Zero });
			}

            int newCoefCount = this._coefficients.Length - derivativeDegree;

            Numeric<T, C>[] coefficients = new Numeric<T, C>[newCoefCount];

            for (int i = 0; i < newCoefCount; i++)
            {
                coefficients[i] = this._coefficients[i + derivativeDegree];

				for (int j = 0; j < derivativeDegree; j++)
				{
					coefficients[i] *= calc.FromInteger(i + derivativeDegree - j);
				}
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
			{
				throw new ArgumentException("The integral degree should be a non-negative value.");
			}
			else if (integralDegree == 0)
			{
				return this.Clone() as Polynom<T, C>;
			}

            Numeric<T, C>[] newCoefs = new Numeric<T, C>[_coefficients.Length + integralDegree];

            for (int i = 0; i < _coefficients.Length; i++)
            {
                Numeric<T, C> product = (Numeric<T, C>)(i + 1);

                for (int j = 2; j <= integralDegree; j++)
                    product *= (Numeric<T, C>)(i + j);

                newCoefs[i + integralDegree] = this._coefficients[i] / product;
            }

            return new Polynom<T, C>(newCoefs);
        }

        /// <summary>
        /// Returns the polynom that is the integral of the current polynom
        /// with specified free constant value.
        /// </summary>
        /// <param name="freeConstant">The value of the integral's free constant.</param>
        /// <returns>The integral polynom object.</returns>
		public Polynom<T, C> GetIntegral(Numeric<T, C> freeConstant)
        {
            Numeric<T, C>[] newCoefficients = new Numeric<T, C>[_coefficients.Length + 1];

            newCoefficients[0] = freeConstant;

			for (int i = 0; i < _coefficients.Length; ++i)
			{
				newCoefficients[i + 1] = _coefficients[i] / (Numeric<T, C>)(i + 1);
			}

            return new Polynom<T, C>(newCoefficients);
        }

        /// <summary>
        /// Returns the polynom that is the integral of the current polynom,
        /// which satisfies the condition of passing the specified point.
        /// </summary>
		public Polynom<T, C> GetIntegral(Point<T> point)
        {
            Polynom<T, C> newPolynom = this.GetIntegral(Numeric<T, C>.Zero);
            newPolynom._coefficients[0] = calc.Subtract(point.Y, newPolynom.GetValue(point.X));

            return newPolynom;
        }

        public T GetValue(T x)
        {
			Numeric<T, C> result = calc.Zero;

            for (int i = _coefficients.Length - 1; i >= 0; --i)
            {
                result = result * x + _coefficients[i];
            }

            return result;
        }

        // ----------------------------
        // --------- ctors ------------
        // ----------------------------

        /// <summary>
        /// Creates a new instance of Polynom object basing on the coefficients.
        /// </summary>
        /// <param name="coefficients"></param>
        public Polynom(params Numeric<T, C>[] coefficients)
            : this(coefficients as IReadOnlyList<Numeric<T, C>>)
        { }

        /// <summary>
        /// Creates a new instance of Polynom object basing on the coefficients
        /// array.
        /// </summary>
        /// <param name="coefficients"></param>
        public Polynom(IReadOnlyList<Numeric<T, C>> coefficients)
        {
            int countSignificant = coefficients.CountSignificant();

            this._coefficients = new Numeric<T, C>[countSignificant];
			ServiceMethods.Copy(coefficients, 0, this._coefficients, 0, countSignificant);
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

			for (int i = 0; i < n; i++)
			{
				frees[i] = points[i].Y;
			}

            Matrices.MatrixSDA<T, C> matrix = new Matrices.MatrixSDA<T, C>(n, n);

            Numeric<T, C>[] argument = new Numeric<T, C>[n];
			argument.FillByAssign((Numeric<T, C>)calc.FromInteger(1));

			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					matrix[i, j] = argument[i];
					argument[i] *= points[i].X;
				}
			}

            Vector<T, C> result;

            Matrices.SlaeSolving.SolveLuFactorization(matrix, frees, out result);

            this._coefficients = result.AsNumericArray().Cut();

            return;
        }

        /// <summary>
        /// Creates an analytic function from the polynomial.
        /// </summary>
		/// <remarks>
		/// The calculation of the function's value would be performed 
		/// without using Gorner's scheme, thus slower and less accurate. 
		/// </remarks>
        public AnalyticFunction ToAnalyticFunction()
        {
            return new AnalyticFunction(this.ToString(PolynomStringRepresentationType.Functional));
        }

        // ------------------------------
        // -------- operators -----------
        // ------------------------------

		public static Polynom<T, C> operator +(Polynom<T, C> first, Polynom<T, C> second)
        {
            Numeric<T, C>[] coefficients = new Numeric<T, C>[Math.Max(first._coefficients.Length, second._coefficients.Length)];

			for (int i = 0; i < coefficients.Length; ++i)
			{
				if (i < first._coefficients.Length && i < second._coefficients.Length)
				{
					coefficients[i] = first._coefficients[i] + second._coefficients[i];
				}
				else if (i < first._coefficients.Length)
				{
					coefficients[i] = first._coefficients[i];
				}
				else
				{
					coefficients[i] = second._coefficients[i];
				}
			}

            return new Polynom<T, C>(coefficients);
        }

		public static Polynom<T, C> operator -(Polynom<T, C> polynom)
        {
			Numeric<T, C>[] resultCoefficients = new Numeric<T, C>[polynom._coefficients.Length];

			for (int i = 0; i < polynom._coefficients.Length; ++i)
			{
				resultCoefficients[i] = -polynom._coefficients[i];
			}

            return new Polynom<T, C>(resultCoefficients);
        }

        public static Polynom<T, C> operator -(Polynom<T, C> one, Polynom<T, C> two)
        {
            Numeric<T, C>[] coefficients = new Numeric<T, C>[Math.Max(one._coefficients.Length, two._coefficients.Length)];

			for (int i = 0; i < coefficients.Length; i++)
			{
				if (i < one._coefficients.Length && i < two._coefficients.Length)
				{
					coefficients[i] = one._coefficients[i] - two._coefficients[i];
				}
				else if (i < one._coefficients.Length)
				{
					coefficients[i] = one._coefficients[i];
				}
				else
				{
					coefficients[i] = -two._coefficients[i];
				}
			}

            return new Polynom<T, C>(coefficients);
        }

        public static Polynom<T, C> operator *(Polynom<T, C> one, Polynom<T, C> two)
        {
            Numeric<T, C>[] coefficients = new Numeric<T, C>[one.Degree + two.Degree + 1];

			for (int i = 0; i < one._coefficients.Length; i++)
			{
				for (int j = 0; j < two._coefficients.Length; j++)
				{
					coefficients[i + j] += one[i] * two[j];
				}
			}

            return new Polynom<T, C>(coefficients);
        }

        public static Polynom<T, C> operator *(Polynom<T, C> one, Numeric<T, C> number)
        {
            Numeric<T, C>[] coefficients = new Numeric<T, C>[one._coefficients.Length];

			for (int i = 0; i < coefficients.Length; ++i)
			{
				coefficients[i] = one._coefficients[i] * number;
			}

            return new Polynom<T, C>(coefficients);
        }

        public static Polynom<T, C> operator /(Polynom<T, C> one, Polynom<T, C> two)
        {
			Polynom<T, C> _;

            return Divide(one, two, out _);
        }

        public static Polynom<T, C> operator %(Polynom<T, C> one, Polynom<T, C> two)
        {
			Polynom<T, C> remainder;

            Divide(one, two, out remainder);

            return remainder;
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
        /// <param name="first">The dividend polynom.</param>
        /// <param name="second">The divisor polynom.</param>
        /// <param name="remainder">The reference to the polynom to which the remainder result will be assigned.</param>
        /// <returns>The division result.</returns>
		public static Polynom<T, C> Divide(Polynom<T, C> first, Polynom<T, C> second, out Polynom<T, C> remainder)
        {
            if (first.Degree < second.Degree)
            {
                remainder = first.Clone() as Polynom<T, C>;
                return new Polynom<T, C>(Numeric<T, C>.Zero);
            }

			Numeric<T, C>[] firstCoefficients = new Numeric<T, C>[first._coefficients.Length];
            Numeric<T, C>[] result = new Numeric<T, C>[first.Degree - second.Degree + 1];

            ServiceMethods.Copy(first._coefficients, firstCoefficients);

            for (int i = firstCoefficients.Length - 1; i >= second.Degree; i--)
            {
                // The next coefficient of the result.
				// -
                result[result.Length - firstCoefficients.Length + i] 
					= firstCoefficients[i] / second._coefficients[second._coefficients.Length - 1];

                // When multiplying back, we get zero in the
				// respective coefficient.
				// -
                firstCoefficients[i] = Numeric<T, C>.Zero;

				// From the dividend, subtract the divisor multiplied by
				// the resulting coefficient.
				// -
				for (int j = second._coefficients.Length - 2; j >= 0; j--)
				{
					firstCoefficients[i + j - second._coefficients.Length + 1] 
						-= second._coefficients[j] * result[result.Length - firstCoefficients.Length + i];
				}
            }

            remainder = new Polynom<T, C>(firstCoefficients);

            return new Polynom<T, C>(result);
        }

        // ------------------------------
        // -------- equality test -------
        // ------------------------------

        public override bool Equals(object obj)
        {
			if (obj is Polynom<T, C>)
			{
				return this.Equals(obj as Polynom<T, C>);
			}
			else if (obj is LagrangePolynom<T, C>)
			{
				return this.Equals(obj as LagrangePolynom<T, C>);
			}

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
			for (int i = 0; i < _coefficients.Length; i++)
			{
				if (this._coefficients[i] != polynom._coefficients[i])
				{
					return false;
				}
			}

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

				for (int i = 0; i < _coefficients.Length; i++)
				{
					coefString += "|" + this._coefficients[i];
				}

                coefString += "|";

                return coefString;
            }
            else
            {
                string functionString = "f(x) = ";

                Numeric<T, C> one = calc.FromInteger(1);

				for (int i = _coefficients.Length - 1; i > 0; i--)
				{
					if (_coefficients[i] != Numeric<T, C>.Zero)
					{
						functionString += string.Format(
							"{0}{1}x{2}", 
							(_coefficients[i] >= calc.Zero 
							 	? (i < _coefficients.Length - 1 ? " + " : "") 
							 	: (i < _coefficients.Length - 1 ? " - " : "-")), 
							(_coefficients[i] != one 
							 	? Mathematics<T, C>.Abs(_coefficients[i]).ToString() 
							 	: ""), 
							(i == 1 ? "" : "^" + i));
					}
				}

				if (_coefficients[0] != Numeric<T, C>.Zero)
				{
					functionString += string.Format(
						" {0} {1} ",
						_coefficients[0] > Numeric<T, C>.Zero ? "+" : "-",
						Mathematics<T, C>.Abs(_coefficients[0]));
				}

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
            return new Polynom<T, C>(_coefficients);
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
                Divide(one, two, out rem1);

                if (rem1.IsZero)
                    return two;

                Divide(two, rem1, out rem2);

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
			ArgumentException exception = new ArgumentException("Only polynoms with integer coefficients can be cancelled.");

			if (_coefficients[0].FractionalPart != Numeric<T, C>.Zero)
			{
				throw exception;
			}

            Numeric<T, C> gcd = _coefficients[0];

            for (int i = 1; i < _coefficients.Length; i++)
            {
				if (_coefficients[i].FractionalPart != Numeric<T, C>.Zero)
				{
					throw exception;
				}
				else
				{
					gcd = Mathematics<T, C>.GreatestCommonDivisor(gcd, _coefficients[i]);
				}
            }

			Numeric<T, C>[] resultCoefficients = new Numeric<T, C>[_coefficients.Length];

			for (int i = 0; i < _coefficients.Length; i++)
			{
				resultCoefficients[i] = this._coefficients[i] / gcd;
			}

			return new Polynom<T, C>(resultCoefficients);
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
                T nCoef = Mathematics<T, C>.Abs(_coefficients[_coefficients.Length - 1]);
                T max = calc.Zero;

                for (int i = 0; i < _coefficients.Length; i++)
                {
                    T cur = Mathematics<T, C>.Abs(_coefficients[i]);

                    if (calc.GreaterThan(cur, max))
                        max = Mathematics<T, C>.Abs(cur);
                }

                T upper = calc.Add(calc.FromInteger(1), calc.Divide(max, nCoef));

                return new BoundedInterval<T, C>(calc.Negate(upper), upper);
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

                T one = _PositiveLagrangeBound(new ReverseList<Numeric<T, C>>(_coefficients), func);
                T two = _PositiveLagrangeBound(_coefficients, func);

                if (one == Numeric<T, C>.Zero)
                    return new BoundedInterval<T, C>(calc.Zero, calc.Zero);

                return new BoundedInterval<T, C>(calc.Divide(calc.FromInteger(1), one), two);
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

                int len = this._coefficients.Length;

                Func<int, Numeric<T, C>, Numeric<T, C>> func2 = delegate(int i, Numeric<T, C> obj)
                {
                    if ((len - i - 1) % 2 == 0)
                        return obj;
                    else
                        return -obj;
                };

                T one = _PositiveLagrangeBound(_coefficients, func);
                T two = _PositiveLagrangeBound(new ReverseList<Numeric<T, C>>(_coefficients), func2);

                if (two == Numeric<T, C>.Zero)
                    return new BoundedInterval<T, C>(calc.Zero, calc.Zero);

                return new BoundedInterval<T, C>(calc.Negate(one), calc.Negate(calc.Divide(calc.FromInteger(1), two)));
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

            T one = calc.FromInteger(1);
            T max = calc.Zero;

            int index = -1;         // индекс первого отрицательного коэффициента
            bool found = false;     // найдены ли отрицательные коэффициенты!

            for (int i = coefficients.Count - minus; i >= 0; i--)
            {
                cur = selector(i, coefficients[i]);

                if (!found && calc.GreaterThan(calc.Zero, cur))
                {
                    found = true;
                    index = coefficients.Count - i - minus;
                }

                // -- здесь cur больше не нужна, а нужно только ее
                // абсолютное значение. его и используем.

                cur = Mathematics<T, C>.Abs(cur);

                if (calc.GreaterThan(cur, max))
                    max = cur;
            }

            if (!found)
                return calc.Zero;

            return calc.Add(one, Mathematics<T, C>.Power(calc.Divide(max, Mathematics<T, C>.Abs(nCoef)), calc.Divide(one, calc.FromInteger(index))));
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

                Polynom<T, C> first = this / GreatestCommonDivisor(this, this.GetDerivative(1));
                Polynom<T, C> second = first.GetDerivative(1);

                // дальше по накатанной.

                List<Polynom<T, C>> sequence = new List<Polynom<T, C>>(first._coefficients.Length);

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
                            Numeric<T, C> discriminant = sequence[i - 1][1] * sequence[i - 1][1] - calc.FromInteger(4) * sequence[i - 1][0] * sequence[i - 1][2];

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
                    signNew = Mathematics<T, C>.Sign(list[i][0]);
                else
                    signNew = Mathematics<T, C>.Sign(list[i][list[i].Degree]) *
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
