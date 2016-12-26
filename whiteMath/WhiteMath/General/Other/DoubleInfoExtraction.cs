using System;

namespace WhiteMath.General
{
    /// <summary>
    /// This structure provides information about
    /// a double number, i.e. its sign, exponent and mantissa.
    /// </summary>
    public struct DoubleInfo
    {
        public bool     Negative { get; private set; }

        public int      Exponent { get; private set; }
        public long     Mantissa { get; private set; }

        /// <summary>
        /// Returns the sign of the number in integer form.
        /// </summary>
        public int Sign
        {
            get
            {
                if (this.Negative)
                    return -1;

                else if (this.Mantissa != 0)
                    return 1;

                else
                    return 0;
            }
        }

        public double MantissaDouble
        {
            get
            {
                return 0;
            }
        }

        public DoubleInfo(bool negative, long mantissa, int exponent)
            : this()
        {
            this.Negative = negative;
            this.Exponent = exponent;
            this.Mantissa = mantissa;
        }
    }

    public static class DoubleInfoExtraction
    {
        public static DoubleInfo ExtractInfo(this double number)
        {
            // Translate the double into sign, exponent and mantissa.
            long bits = BitConverter.DoubleToInt64Bits(number);

            // Note that the shift is sign-extended, hence the test against -1 not 1
            bool negative   = (bits < 0);
            int exponent    = (int)((bits >> 52) & 0x7ffL);
            long mantissa   = bits & 0xfffffffffffffL;

            // Subnormal numbers; exponent is effectively one higher,
            // but there's no extra normalisation bit in the mantissa
            if (exponent == 0)
                exponent++;
            
            // Normal numbers; leave exponent as it is but add extra
            // bit to the front of the mantissa
            else
                mantissa = mantissa | (1L << 52);
       
            // Bias the exponent. It's actually biased by 1023, but we're
            // treating the mantissa as m.0 rather than 0.m, so we need
            // to subtract another 52 from it.
            exponent -= 1075;

            if (mantissa == 0)
                return new DoubleInfo(false, 0, 0);

            /* Normalize */
            while ((mantissa & 1) == 0)
            {    /*  i.e., Mantissa is even */
                mantissa >>= 1;
                exponent++;
            }

            return new DoubleInfo(negative, mantissa, exponent);
        }
    }
}
