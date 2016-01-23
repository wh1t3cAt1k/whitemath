using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.General
{
    /// <summary>
    /// This class allows to convert byte sequences to hexadecimal string
    /// representations and to restore sequences from such strings.
    /// </summary>
    public static class ByteSequenceToString
    {
        /// <summary>
        /// Цифирку - в гекс =0)
        /// </summary>
        private static string ___toHexSymbol(this int digit, bool upperCase)
        {
            if (digit < 10)
                return digit.ToString();
            else if (digit == 10)
                return (upperCase ? "A" : "a");
            else if (digit == 11)
                return (upperCase ? "B" : "b");
            else if (digit == 12)
                return (upperCase ? "C" : "c");
            else if (digit == 13)
                return (upperCase ? "D" : "d");
            else if (digit == 14)
                return (upperCase ? "E" : "e");
            else if (digit == 15)
                return (upperCase ? "F" : "f");
            else
                throw new ArgumentException("APOCALYPTIC EXCEPTION. Hexadecimal digit is not a hexadecimal digit.");
        }

        /// <summary>
        /// Converts a sequence of bytes into a hexadecimal string.
        /// </summary>
        /// <param name="bigEndian">
        /// If this parameter is set to true, than in two consecutive hex string symbols, 
        /// the first is treated as the least significant part of the byte, 
        /// and the second one as the most significant, e.g. <c>A1</c>
        /// would mean <c>26</c> and not <c>161</c>.
        /// </param>
        /// <param name="sequence">A sequence of bytes to be represented as a hex string.</param>
        /// <param name="upperCase">A flag specifying whether the 
        /// A, B, C, D, E, F digits, if any appear, should be uppercase.</param>
        /// <returns>A hexadecimal string </returns>
        public static string ToHexString(IEnumerable<byte> sequence, bool upperCase, bool bigEndian = false)
        {
            Contract.Requires<ArgumentNullException>(sequence != null, "sequence");

            StringBuilder builder = new StringBuilder(sequence.Count() * 2);

            foreach (byte nextByte in sequence)
            {
                if (bigEndian)
                {
                    builder.Append((nextByte & 0x0f).___toHexSymbol(false));
                    builder.Append(((nextByte & 0xf0) >> 4).___toHexSymbol(false));
                }
                else
                {
                    builder.Append(((nextByte & 0xf0) >> 4).___toHexSymbol(false));
                    builder.Append((nextByte & 0x0f).___toHexSymbol(false));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Restores the byte array from its string representation created by
        /// <see cref="ToHexString"/>.
        /// </summary>
        /// <param name="hexString">
        /// A string consiting of hexadecimal digits in upper or lower case.
        /// </param>
        /// <param name="bigEndian">
        /// If this parameter is set to true, than in two consecutive hex string symbols, 
        /// the first is treated as the least significant part of the byte, 
        /// and the second one as the most significant, e.g. <c>A1</c>
        /// would mean <c>26</c> and not <c>161</c>.
        /// </param>
        /// <returns>A byte array made from <paramref name="hexString"/>.</returns>
        public static byte[] RestoreFromHexString(string hexString, bool bigEndian = false)
        {
            Contract.Requires<ArgumentNullException>(hexString != null, "hexString");

            // Remove prefixes of 
            // "#" or "0x"

            if (hexString.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                hexString = hexString.Substring(2);
            }
            else if (hexString.StartsWith("#", StringComparison.CurrentCultureIgnoreCase))
            {
                hexString = hexString.Substring(1);
            }

            byte[] arr = new byte[(hexString.Length + 1) / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                if (bigEndian)
                {
                    arr[i / 2] = byte.Parse(
                        (i + 1 < hexString.Length ? hexString[i + 1].ToString() : "") + hexString[i],
                        System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                else
                {
                    arr[i / 2] = byte.Parse(
                        hexString[i] + (i + 1 < hexString.Length ? hexString[i + 1].ToString() : ""),
                        System.Globalization.NumberStyles.AllowHexSpecifier);
                }
            }

            return arr;
        }
    }
}
