using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WhiteStructs.Conditions;

namespace WhiteMath.General
{
    /// <summary>
    /// This class allows to convert byte sequences to hexadecimal string
    /// representations and to restore sequences from such strings.
    /// </summary>
    public static class ByteSequenceToString
    {
        /// <summary>
        /// Digit to hexadecimal.
        /// </summary>
		private static string ToHexSymbol(this int digit, bool upperCase)
        {
			string result;

			switch (digit)
			{
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
					result = digit.ToString();
					break;
				case 10:
					result = "a";
					break;
				case 11:
					result = "b";
					break;
				case 12:
					result = "c";
					break;
				case 13:
					result = "d";
					break;
				case 14:
					result = "e";
					break;
				case 15:
					result = "f";
					break;
				default:
					throw new ArgumentException(
						"The argument is not a representation of a hexadecimal digit.",
						nameof(digit));
			}

			return upperCase 
				? result 
				: result.ToUpperInvariant();
        }

        /// <summary>
        /// Converts a sequence of bytes into a hexadecimal string.
        /// </summary>
        /// <param name="isBigEndian">
        /// If this parameter is set to <c>true</c>, than in two consecutive hex 
		/// string symbols, the first is treated as the least significant part 
		/// of the byte, and the second one as the most significant, e.g. <c>A1</c>
        /// would mean <c>26</c> and not <c>161</c>.
        /// </param>
        /// <param name="sequence">A sequence of bytes to be represented as a hex string.</param>
        /// <param name="isUpperCase">A flag specifying whether the 
        /// A, B, C, D, E, F digits, if any appear, should be uppercase.</param>
        /// <returns>A hexadecimal string </returns>
		public static string ToHexString(IEnumerable<byte> sequence, bool isUpperCase, bool isBigEndian = false)
        {
			Condition.ValidateNotNull(sequence, nameof(sequence));

            StringBuilder builder = new StringBuilder(sequence.Count() * 2);

            foreach (byte nextByte in sequence)
            {
                if (isBigEndian)
                {
					builder.Append((nextByte & 0x0f).ToHexSymbol(isUpperCase));
					builder.Append(((nextByte & 0xf0) >> 4).ToHexSymbol(isUpperCase));
                }
                else
                {
					builder.Append(((nextByte & 0xf0) >> 4).ToHexSymbol(isUpperCase));
					builder.Append((nextByte & 0x0f).ToHexSymbol(isUpperCase));
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
        public static byte[] FromHexString(string hexString, bool bigEndian = false)
        {
			Condition.ValidateNotNull(hexString, nameof(hexString));

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

			byte[] result = new byte[(hexString.Length + 1) / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                if (bigEndian)
                {
                    result[i / 2] = byte.Parse(
                        (i + 1 < hexString.Length ? hexString[i + 1].ToString() : "") + hexString[i],
                        System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                else
                {
                    result[i / 2] = byte.Parse(
                        hexString[i] + (i + 1 < hexString.Length ? hexString[i + 1].ToString() : ""),
                        System.Globalization.NumberStyles.AllowHexSpecifier);
                }
            }

            return result;
        }
    }
}
