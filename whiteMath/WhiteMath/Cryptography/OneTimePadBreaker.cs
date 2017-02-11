using System;
using System.Collections.Generic;
using System.Linq;

using WhiteMath.General;

using WhiteStructs.Conditions;

namespace WhiteMath.Cryptography
{
    /// <summary>
    /// This class tries to break the one-time pad cipher using
    /// several distinct ASCII-encoded messages which were 
    /// encrypted with the same one-time pad key.
    /// The messages are expected to contain ONLY whitespace 0x20 
	/// and letter characters.
    /// </summary>
    [Serializable]
    public class OneTimePadBreaker
    {
		private List<byte[]> _cipherTexts; 
		private List<byte?[]> _messages;
        private byte?[] key;

        /// <summary>
        /// Adds a ciphertext to the breaker and processes it,
        /// recovering as much information as possible about the current 
		/// and the previously passed messages, as well as about the key used
        /// to encrypt them.
        /// </summary>
		public void AddCipherText(string hexString, bool bigEndian = false)
        {
			Condition.ValidateNotNull(hexString, nameof(hexString));

            this.addCipherText(ByteSequenceToString.FromHexString(hexString, bigEndian));
        }

        public void addCipherText(byte[] cipherText)
        {
			Condition.ValidateNotNull(cipherText, nameof(cipherText));
			Condition.ValidateNonNegative(cipherText.Length, "The cipher text should not be empty");

            _cipherTexts.Add(cipherText.Clone() as byte[]);
            _messages.Add(new byte?[cipherText.Length]);

            ProcessLastCipherText();
        }

        /// <summary>
        /// Bitwise XORs the messages, perhaps of different length.
        /// </summary>
        /// <returns></returns>
		private static byte[] BitwiseXor(byte[] messageOne, byte[] messageTwo)
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
		private void ProcessLastCipherText()
        {
            byte[] lastCipherText = _cipherTexts.Last();

            // Process all but the last cyphertexts,
            // XORing them with the last one.
			// -
            for (int i = 0; i < _cipherTexts.Count - 1; ++i)
            {
                byte[] currentCipherText = _cipherTexts[i];
                byte[] cipherXor = BitwiseXor(currentCipherText, lastCipherText);

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
