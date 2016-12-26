namespace WhiteMath.Calculators
{
    public class CalcComplex: ICalc<Complex>
    {
        public bool IsIntegerCalculator { get { return false; } }

        public Complex Parse(string value)
        { 
            return Complex.Parse(value); 
        }

        public Complex Zero { get { return new Complex(0, 0); } }
        
        public Complex FromDouble(double num) { return new Complex(num, 0); }
        public Complex FromInteger(long num) { return new Complex(num, 0); }

        public bool IsEven(Complex num)     { throw new NonIntegerTypeException("Complex"); }
        public bool IsNegativeInfinity(Complex num)   { return false; }
        public bool IsPositiveInfinity(Complex num)   { return false; }
        public bool IsNaN(Complex num)      { return false; }

       	public Complex GetCopy(Complex num) { return num; }

        public bool Equal(Complex one, Complex two) { return one == two; }
        public bool GreaterThan(Complex one, Complex two) { return one.Module > two.Module; }

        public Complex IntegerPart(Complex num) { return new Complex((long)num.RealCounterPart, (long)num.ImaginaryCounterPart); }

        public Complex Increment(Complex num) { num.RealCounterPart++; return num; }        // увеличиваем реальную часть
        public Complex Decrement(Complex num) { num.ImaginaryCounterPart--; return num; }   // уменьшаем реальную часть

        public Complex Negate(Complex num) { return -num; }
        public Complex Modulo(Complex one, Complex two) { throw new NonIntegerTypeException("Complex"); }

        public Complex Multiply(Complex one, Complex two) { return one * two; }
        public Complex Divide(Complex one, Complex two) { return one / two; }
        public Complex Subtract(Complex one, Complex two) { return one - two; }
        public Complex Add(Complex one, Complex two) { return one + two; }

    }
}
