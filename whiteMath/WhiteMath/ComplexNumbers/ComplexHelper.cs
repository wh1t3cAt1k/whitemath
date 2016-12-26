namespace WhiteMath
{
    public partial struct Complex
    {

	    /// <summary>
	    /// Static class providing various mathematical algorithms
	    /// working with complex numbers.
	    /// </summary>
        public static class Helper
        {
            /// <summary>
            /// Represents a simple multiplying algorithm
            /// for two complex numbers.
            /// 
            /// first = a + bi; 
            /// second = c + di; 
            /// result = (ac - bd; bc + ad)
            /// </summary>
            /// <param name="one"></param>
            /// <param name="two"></param>
            /// <returns></returns>
            public static Complex MultiplySimple(Complex one, Complex two)
            {
                return one * two;
            }

            /// <summary>
            /// A Karatsuba-like multiplying algorithm
            /// using three multiplications instead of four.
            /// </summary>
            public static Complex MultiplyKaratsuba(Complex one, Complex two)
            {
                return new Complex(
					one.RealCounterPart * (two.RealCounterPart - two.ImaginaryCounterPart) 
						+ two.ImaginaryCounterPart * (one.RealCounterPart - one.ImaginaryCounterPart),
                    two.ImaginaryCounterPart * (one.RealCounterPart - one.ImaginaryCounterPart) 
						+ one.ImaginaryCounterPart * (two.RealCounterPart + two.ImaginaryCounterPart));
            }
        }
    }
}
