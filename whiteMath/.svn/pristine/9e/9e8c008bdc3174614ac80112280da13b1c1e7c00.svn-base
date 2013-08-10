using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
{
    public static partial class WhiteMath
    {
        /// <summary>
        /// Returns the full series of complex roots of unity, starting with <c>(1, 0i)</c>.
        /// Used in Discrete Fourier Transform and has some other appliances.
        /// </summary>
        /// <param name="rootDegree">A positive integer degree of the root. May be 1.</param>
        /// <returns>
        /// The full series of complex roots of unity of 
        /// degree <paramref name="rootDegree"/>, starting with <c>(1, 0i)</c>.
        /// </returns>
        public static Complex[] rootsOfUnity(int rootDegree)
        {
            if (rootDegree < 1)
                throw new ArgumentException("The root degree should be at least 1 (meaningless, but will work). No zeroes and negative numbers allowed.");

            Complex[] tmp = new Complex[rootDegree];

            tmp[0] = 1;

            for (int i = 1; i < rootDegree; i++)
                tmp[i] = new Complex(Math.Cos(2 * Math.PI * i / rootDegree), Math.Sin(2 * Math.PI * i / rootDegree));

            return tmp;
        }
    }
}
