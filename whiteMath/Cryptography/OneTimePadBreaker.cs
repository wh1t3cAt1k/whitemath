using System;
using System.Collections.Generic;
using System.Linq;

using whiteMath.General;

using whiteStructs.Conditions;

namespace whiteMath.Cryptography
{
    /// <summary>
    /// This class tries to break the one-time pad cipher using
    /// several distinct ASCII-encoded messages which were 
    /// encrypted with the same one-time pad key.
    /// 
    /// The messages are expected to contain ONLY whitespace 0x20 and letter characters.
    /// </summary>
    [Serializable]
    public class OneTimePadBreaker
    {
        private List<byte[]> m_cipherTexts; 
        private List<byte?[]> m_messages;
        private byte?[] key;

        /// <summary>
        /// Adds a ciphertext to the breaker and processes it,
        /// recovering as much information as possible about the current and the 
        /// previously passed messages, as well as about the key used
        /// to encrypt them.
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="bigEndian"></param>
        public void addCipherText(string hexString, bool bigEndian = false)
        {
			Condition.ValidateNotNull(hexString, nameof(hexString));

            this.addCipherText(ByteSequenceToString.RestoreFromHexString(hexString, bigEndian));
        }

        public void addCipherText(byte[] cipherText)
        {
			Condition.ValidateNotNull(cipherText, nameof(cipherText));
			Condition.ValidateNonNegative(cipherText.Length, "The cipher text should not be empty");

            m_cipherTexts.Add(cipherText.Clone() as byte[]);
            m_messages.Add(new byte?[cipherText.Length]);

            _processLastCipherText();
        }

        /// <summary>
        /// Bitwise XORs the messages, perhaps of different length.
        /// </summary>
        /// <returns></returns>
        private static byte[] _xor(byte[] messageOne, byte[] messageTwo)
        {
            byte[] result = new byte[Math.Max(messageOne.Length, messageTwo.Length)];

            int i = 0;

            for ( ; i < Math.Min(messageOne.Length, messageTwo.Length); ++i)
            {
                result[i] = (byte)(messageOne[i] ^ messageTwo[i]);
            }

            while (i < messageOne.Length)
            {
                result[i] = messageOne[i];
            }

            while (i < messageTwo.Length)
            {
                result[i] = messageTwo[i];
            }

            return result;
        }

        /// <summary>
        /// Processes the lately added ciphertext.
        /// </summary>
        private void _processLastCipherText()
        {
            byte[] lastCipherText = m_cipherTexts.Last();

            // Process all but the last cyphertexts,
            // XORing them with the last one.

            for (int i = 0; i < m_cipherTexts.Count - 1; ++i)
            {
                byte[] currentCipherText = m_cipherTexts[i];
                byte[] cipherXor = _xor(currentCipherText, lastCipherText);

                // Now go through XOR of two messages and look for whitespace XORs
                // We need to look only on the first half of the byte of the character.
                // -
                for (int j = 0; j < cipherXor.Length; ++j)
                {
                    // If any character XORs with any character, first something less than 0x40 is obtained.
                    // If any character XORs with a whitespace, something in 0x40-0x7F is obtained, and 
                    // this is the clue.
                    // -
                    if (cipherXor[j] >= 0x40)
                    {
                        // Means that we have just XORed a character with a whitespace
                        // Since the case has changed, need to XOR with 0x20 again.
                        // -
                        byte trueCharacter = (byte)(cipherXor[j] ^ 0x20);
 
						// TODO: IMPLEMENTATION NOT FINISHED.
                    }
                }
            }

			throw new NotImplementedException();
        }
    }
}
