using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using System.Diagnostics.Contracts;

namespace whiteMath.Cryptography
{
    /// <summary>
    /// This class provides methods for generating prime numbers.
    /// </summary>
    [ContractVerification(true)]
    public static class PrimeGeneration
    {
        /// <summary>
        /// Returns a list of all prime numbers which are less than or equal to the specified integer number.
        /// </summary>
        /// <param name="num">An upper bound of prime numbers found. Should be positive.</param>
        /// <returns>A list of all prime numbers which are less than or equal to <paramref name="num"/>.</returns>
        public static List<int> EratospheneSieve(int num)
        {
            Contract.Requires<ArgumentOutOfRangeException>(num > 0, "The upper bound of generated numbers should be positive.");

            BitArray ba = new BitArray(num, true);
            List<int> result = new List<int>();

            uint i = 2;

            for( ; i * i <= num; i++)
            {
                if (ba[(int)i - 1])
                {
                    result.Add((int)i);

                    for (uint j = i * i; j <= num; j += i)
                        ba[(int)j - 1] = false;
                }
            }

            for (int k = (int)i - 1; k < ba.Length; ++k)
                if (ba[k])
                    result.Add(k + 1);

            return result;
        }         
    }

}
