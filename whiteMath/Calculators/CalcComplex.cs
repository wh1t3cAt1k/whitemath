using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
{
    public class CalcComplex: ICalc<Complex>
    {
        public bool isIntegerCalculator { get { return false; } }

        public Complex parse(string value)
        { 
            return Complex.Parse(value); 
        }

        public Complex zero { get { return new Complex(0, 0); } }
        
        public Complex fromDouble(double num) { return new Complex(num, 0); }
        public Complex fromInt(long num) { return new Complex(num, 0); }

        public bool isEven(Complex num)     { throw new NonIntegerTypeException("Complex"); }
        public bool isNegInf(Complex num)   { return false; }
        public bool isPosInf(Complex num)   { return false; }
        public bool isNaN(Complex num)      { return false; }

        public Complex getCopy(Complex num) { return num; }

        public bool eqv(Complex one, Complex two) { return one == two; }
        public bool mor(Complex one, Complex two) { return one.Module > two.Module; }       // сравниваем модули чисел

        public Complex intPart(Complex num) { return new Complex((long)num.RealCounterPart, (long)num.ImaginaryCounterPart); }

        public Complex increment(Complex num) { num.RealCounterPart++; return num; }        // увеличиваем реальную часть
        public Complex decrement(Complex num) { num.ImaginaryCounterPart--; return num; }   // уменьшаем реальную часть

        public Complex negate(Complex num) { return -num; }
        public Complex rem(Complex one, Complex two) { throw new NonIntegerTypeException("Complex"); }

        public Complex mul(Complex one, Complex two) { return one * two; }
        public Complex div(Complex one, Complex two) { return one / two; }
        public Complex dif(Complex one, Complex two) { return one - two; }
        public Complex sum(Complex one, Complex two) { return one + two; }

    }
}
