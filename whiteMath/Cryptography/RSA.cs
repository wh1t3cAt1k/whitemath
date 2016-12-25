using System;
using System.Collections.Generic;
using System.Linq;

using whiteMath.Algorithms;
using whiteMath.ArithmeticLong;
using whiteMath.Calculators;
using whiteMath.General;
using whiteMath.Randoms;

using whiteStructs.Conditions;
using whiteStructs.Collections;

namespace whiteMath.Cryptography
{
    /// <summary>
    /// This class provides methods for encryption and decryption of arbitrary data using
    /// RSA algorithm.
    /// </summary>
    public static class RSA
    {
        /// <summary>
        /// This property provides preset Fermat prime public exponents
        /// which are guaranteed to be coprime with Euler's totient function
        /// of any RSA module and may be safely used for RSA encryption.
        /// </summary>
        public class PublicExponentCollection
        {
            private LongInt<Bases.B_65536> [] arr = { 3, 17, 257, 65537 };
            
            /// <summary>
            /// Gets a preset Fermat prime public exponent
            /// which is guaranteed to be coprime with Euler's totient function
            /// of any RSA module and may be safely used for RSA encryption
            /// </summary>
            /// <param name="i">An index of the preset public exponent. Should be non-negative and less than <see cref="PublicExponentCollection.Length"/>.</param>
            /// <returns>The i'th preset Fermat prime public exponent from the list.</returns>
            public LongInt<Bases.B_65536> this[int i] { get { return arr[i].Clone() as LongInt<Bases.B_65536>; } }
            
            /// <summary>
            /// Gets the length of the preset public exponent list.
            /// </summary>
            public int Length { get { return arr.Length; } }
        }

        /// <summary>
        /// This property provides preset Fermat prime public exponents
        /// which are guaranteed to be coprime with Euler's totient function
        /// of any RSA module and may be safely used for RSA encryption.
        /// </summary>
        public static PublicExponentCollection PublicExponents { get; private set; }

        static RSA()
        {
            PublicExponents = new PublicExponentCollection();
        }

        // -------------------------------------------------------
        // ------------------- EXTENSION METHODS -----------------
        // -------------------------------------------------------

        /// <summary>
        /// Converts a sequence of bytes into a <c>LongInt</c> number, e.g. to encrypt 
        /// it using RSA algorithm.
        /// </summary>
        /// <typeparam name="B">The type specifying the digit base of the desired <c>LongInt</c>. Recommended to be <c>B_256</c> for quicker conversion.</typeparam>
        /// <param name="sequence">A sequence of bytes to be represented as a <c>LongInt</c>.</param>
        /// <returns>A <c>LongInt&lt;B&gt;</c> number which, being converted to base 256, would contain the same numeric values and in the same order as <paramref name="sequence"/>.</returns>
        public static LongInt<B> ToLongInt<B>(this IEnumerable<byte> sequence) where B: IBase, new()
        {
			Condition.ValidateNotNull(sequence, nameof(sequence));
			Condition.ValidateNotEmpty(sequence, "The sequence should contain at least one element.");

            int[] intArray = new int[sequence.Count()];

            int i = 0;

            foreach (byte nextByte in sequence)
                intArray[i++] = nextByte;

            return new LongInt<B>(256, intArray, false);
        }

        /// <summary>
        /// Converts a <c>LongInt</c> number into a sequence of bytes.
        /// </summary>
        /// <typeparam name="B">The type specifying the digit base of the incoming <c>LongInt&lt;B&gt;</c>. Recommended to be <c>B_256</c> for quicker conversion.</typeparam>
        /// <param name="number">The incoming number.</param>
        /// <returns>The byte array containing the same numeric values and in the same order as <paramref name="number"/> converted to base 256.</returns>
        public static byte[] ToByteArray<B>(this LongInt<B> number) where B : IBase, new()
        {
			Condition.ValidateNotNull(number, nameof(number));
			Condition
				.Validate(!number.Negative)
				.OrArgumentOutOfRangeException("The number should not be negative.");

            byte[] result;

            if (typeof(B) != typeof(Bases.B_256))
            {
                LongInt<Bases.B_256> converted = number.BaseConvert<Bases.B_256>();

                result = new byte[converted.Length];

                for (int i = 0; i < result.Length; i++)
                    result[i] = (byte)converted[i];
            }
            else
            {
                result = new byte[number.Length];

                for (int i = 0; i < result.Length; i++)
                    result[i] = (byte)number[i];
            }

            return result;
        }

        /// <summary>
        /// The enum which members specify how numbers
        /// bigger than the module are handled when encrypted
        /// or decrypted using the RSA algorithm.
        /// </summary>
        public enum BigNumberEncryptionMethod
        {
            /// <summary>
            /// <para>
            /// During the encryption process, if the number to be encrypted is bigger than the public key,
            /// this option makes its copy to be continously divided by the key until it becomes zero. 
            /// Remainders to be encrypted.
            /// </para>
            /// <para>
            /// During the decryption process, this option signalizes that decrypted numbers are
            /// actually the remainders of a former division, so they will be sequentially multiplied 
            /// by the public key and summarized to form the original message.
            /// </para>
            /// <para>
            /// In general, this option will make the encrypted sequence more compact, but may slow down 
            /// the process of both encryption and decryption. When the number is less than the module, this option
            /// doesn't matter.
            /// </para>
            /// </summary>
            Division,
            /// <summary>
            /// <para>
            /// During the encryption process, if the number to be encrypted is bigger than the public key,
            /// the former is split down into parts which for sure contain less digits than the key contains (thus, they are
            /// definitely numerically smaller when treated as separate numbers), and these parts are encrypted
            /// to form the sequence.
            /// </para>
            /// <para>
            /// During the decryption process, this option signalizes that decrypted numbers are
            /// actually parts which should be merged to form the original message.
            /// </para>
            /// <para>
            /// In general, this option will make the encrypted sequence longer, but may speed up
            /// the process of both encryption and decryption. When the number is less than the module, this option 
            /// doesn't matter.
            /// </para>
            /// </summary>
            Splitting
        }

        private static readonly int[] firstPrimes = new int[]
        {
            5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
            73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151,
            157, 163, 167, 173, 179, 181, 191, 193, 197, 199
        };

        /// <summary>
        /// Hidden method - tries the first 100 primes to test divisibility.
        /// If not divisible - proceeds with Miller-Rabin.
        /// </summary>
        private static bool __isPrimeOptimized<B>(LongInt<B> number)
            where B: IBase, new()
        {
            RandomMersenneTwister gen = new RandomMersenneTwister();
            RandomLongIntModular<B> lgen = new RandomLongIntModular<B>(gen);

            foreach (int prime in firstPrimes)
                if (number % prime == 0)
                    return false;

            if (PrimalityTests.IsPrime_MillerRabin(number, lgen, number.LengthInBinaryPlaces) < 1)
                return true;

            return false;
        }

        /// <summary>
        /// Creates a pair of two primes which, being multiplied one by another,
        /// produce a public key of desired length for the RSA algorithm.
        /// </summary>
        /// <typeparam name="B">An implementation of <c>IBase</c> interface which specifies the digit base of <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> numbers.</typeparam>
        /// <param name="digits">The desired number of digits in the public key.</param>
        /// <param name="randomGenerator">A random generator for long integer numbers.</param>
        /// <param name="primalityTest"></param>
        /// <returns></returns>
        public static Point<LongInt<B>> GetKey<B>(int digits, IRandomBounded<LongInt<B>> randomGenerator = null, Func<LongInt<B>, bool> primalityTest = null) where B: IBase, new()
        {
			Condition
				.Validate(digits > 1)
				.OrArgumentOutOfRangeException("The number of digits in the key should be more than 1.");
            
            /*
            Contract.Ensures(
                (Contract.Result<Point<LongInt<B>>>().X * Contract.Result<Point<LongInt<B>>>().Y)
                .Length == digits);
            */

            long bits = (long)Math.Ceiling(Math.Log(LongInt<B>.BASE, 2));   // сколько бит занимает BASE

            if (randomGenerator == null)
                randomGenerator = new RandomLongIntModular<B>(new RandomMersenneTwister());

            if (primalityTest == null)
                primalityTest = (x => __isPrimeOptimized(x));

            LongInt<B>
                firstPrime,
                secondPrime;

            int half = digits / 2;

            // На текущий момент длина ключа может оказаться МЕНЬШЕ
            // запланированной. Сделать так, чтобы она всегда была одна.
            // Нижеуказанный генератор тоже снести.

            IRandomBoundedUnbounded<int> tmp = new RandomCryptographic();

            do
            {
                firstPrime = new LongInt<B>(half, tmp, false);
            }
            while (!primalityTest(firstPrime));

            do
            {
                secondPrime = new LongInt<B>(digits - half, tmp, false);
            }
            while (!primalityTest(secondPrime));

            return new Point<LongInt<B>>(firstPrime, secondPrime);
        }

        /// <summary>
        /// Calculates a public exponent on the basis of
        /// a pair of primes which form the RSA secret key and
        /// the number chosen as public exponent.
        /// </summary>
        /// <typeparam name="B">An implementation of <c>IBase</c> interface which specifies the digit base of <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> numbers.</typeparam>
        /// <param name="secretKey">A pair of primes which form the RSA secret key.</param>
        /// <returns>The secret exponent of the RSA algorithm.</returns>
        public static LongInt<B> GetSecretExponent<B>(Point<LongInt<B>> secretKey, LongInt<B> publicExponent)
            where B: IBase, new()
        {
			Condition.ValidateNotNull(publicExponent, nameof(publicExponent));
			Condition.ValidateNotNull(secretKey.X, nameof(secretKey.X));
			Condition.ValidateNotNull(secretKey.Y, nameof(secretKey.Y));

            LongInt<B> totient = LongInt<B>.Helper.MultiplyFFTComplex(secretKey.X - 1, secretKey.Y - 1);

            return 
                WhiteMath<LongInt<B>, CalcLongInt<B>>.MultiplicativeInverse(publicExponent, totient);
        }

        /// <summary>
        /// Decrypts a long integer number using the RSA algorithm.
        /// </summary>
        /// <typeparam name="B">An implementation of <c>IBase</c> interface which specifies the digit base of <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> numbers.</typeparam>
        /// <typeparam name="E">An implementation of <see cref="IBase"/> interface which specifies the digit base of the public exponent. Should be a power of 2 for faster encryption.</typeparam>
        /// <param name="number">The number to encrypt.</param>
        /// <param name="publicKey">The public key of the RSA algorithm. Should be a product of two prime numbers.</param>
        /// <param name="secretExponent">The secret exponent of the RSA algorithm.</param>
        /// <remarks>If <paramref name="secretExponent"/> has digit base which is a power of 2, the decryption process will go faster.</remarks>
        /// <returns>The result of RSA decryption.</returns>
        public static LongInt<B> Decrypt<B, E>(LongInt<B> number, LongInt<B> publicKey, LongInt<E> secretExponent) 
            where B: IBase, new()
            where E: IBase, new()
        {
			Condition.ValidateNotNull(number, nameof(number));
			Condition.ValidateNotNull(publicKey, nameof(publicKey));
			Condition
				.Validate(!number.Negative)
				.OrArgumentOutOfRangeException("The decrypted number should not be negative.");
			Condition
				.Validate(!publicKey.Negative)
				.OrArgumentOutOfRangeException("The public key should not be negative.");
			Condition
				.Validate(secretExponent > 0)
				.OrArgumentOutOfRangeException("The secret exponent should be positive.");
			
            return LongInt<B>.Helper.PowerIntegerModular(number, secretExponent, publicKey);
        }

        /// <summary>
        /// Treats a sequence of <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> numbers as
        /// a single big number which has been encrypted modulo <paramref name="publicKey"/> and
        /// returns the result of decryption.
        /// </summary>
        /// <typeparam name="B">An implementation of <c>IBase</c> interface which specifies the digit base of <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> numbers.</typeparam>
        /// <typeparam name="E">An implementation of <see cref="IBase"/> interface which specifies the digit base of the public exponent. Should be a power of 2 for faster encryption.</typeparam>
        /// <param name="numberSequence">A sequence of encrypted numbers which are treated as a single number which was bigger than the <paramref name="publicKey"/> during the encryption process.</param>
        /// <param name="publicKey">The public key which was used during the encryption process. Should be a product of two primes.</param>
        /// <param name="secretExponent">The secret exponent of the RSA algorithm.</param>
        /// <param name="bnem">Options which were used during the encryption process. Wrong options will result in a wrong decryption.</param>
        /// <remarks>If <paramref name="secretExponent"/> has digit base which is a power of 2, the decryption process will go faster.</remarks>
        /// <returns>With all conditions of the RSA met and correct options specified, the method returns the decrypted number.</returns>
        public static LongInt<B> DecryptAsSingle<B, E>(IEnumerable<LongInt<B>> numberSequence, LongInt<B> publicKey, LongInt<E> secretExponent, BigNumberEncryptionMethod bnem) 
            where B : IBase, new()
            where E : IBase, new()
        {
			Condition.ValidateNotNull(numberSequence, nameof(numberSequence));
			Condition.ValidateNotEmpty(numberSequence, "The sequence should containt at least one element.");
			Condition
				.Validate(secretExponent > 0)
				.OrArgumentOutOfRangeException("The secret exponent should be positive.");
			
			/*
            Contract.Requires(Contract.ForAll(numberSequence, x => x != null), "At least one number in the sequence is null.");
            Contract.Requires(Contract.ForAll(numberSequence, x => !x.Negative), "At least one number in the sequence is negative.");
            */

			if (numberSequence.IsSingleton())
			{
				return Decrypt(numberSequence.First(), publicKey, secretExponent);
			}

            List<LongInt<B>> list = new List<LongInt<B>>();

			foreach (LongInt<B> encryptedNumber in numberSequence)
			{
				list.Add(
					Decrypt(encryptedNumber, publicKey, secretExponent));
			}

			if (bnem == BigNumberEncryptionMethod.Splitting)
			{
				return new LongInt<B>(LongInt<B>.BASE, list.SelectMany(x => x.Digits).ToList(), false);
			}
			else if (bnem == BigNumberEncryptionMethod.Division)
			{
				LongInt<B> result = list[list.Count - 1];

				for (int i = list.Count - 2; i >= 0; --i)
					result = result * publicKey + list[i];

				return result;
			}
			else
			{
				throw new EnumFattenedException("Big number encryption method enum has fattened, decryption process stuck.");
			}
        }

        /// <summary>
        /// Encrypts a long integer number using the RSA algorithm.
        /// In case when the number to encrypt is bigger than the public key,
        /// its copy is continously divided by the key until it becomes zero, and remainders are encrypted.
        /// </summary>
        /// <typeparam name="B">An implementation of <see cref="IBase"/> interface which specifies the digit base of the encrypted number.</typeparam>
        /// <typeparam name="E">An implementation of <see cref="IBase"/> interface which specifies the digit base of the public exponent. Should be a power of 2 for faster encryption.</typeparam>
        /// <param name="number">The number to encrypt.</param>
        /// <param name="publicKey">The public key of the RSA algorithm. Should be a product of two big prime numbers.</param>
        /// <param name="publicExponent">The public exponent of the RSA algorithm. Should be relatively prime to Euler's totient function value for <paramref name="publicKey"/>.</param>
        /// <param name="bnem"> Option specifying how numbers bigger than the <paramref name="publicKey"/> are handled when encrypted using the RSA algorithm.</param> 
        /// <remarks>If <paramref name="publicExponent"/> has digit base which is a power of 2, the decryption process will go faster.</remarks>
        /// <returns>A sequence of encrypted values, which, along with <paramref name="bnem"/> parameter, allows decryption using <c>Decrypt()</c> method.</returns>
        public static List<LongInt<B>> Encrypt<B, E>(LongInt<B> number, LongInt<B> publicKey, LongInt<E> publicExponent, BigNumberEncryptionMethod bnem) 
            where B: IBase, new()
            where E: IBase, new()
        {
			Condition.ValidateNotNull(number, nameof(number));
			Condition.ValidateNotNull(publicKey, nameof(publicKey));
			Condition
				.Validate(number > 0)
				.OrArgumentOutOfRangeException("The encrypted number should be positive.");
			Condition
				.Validate(publicExponent > 0)
				.OrArgumentOutOfRangeException("The public exponent should be positive.");
			Condition
				.Validate(publicKey.Length > 1 || bnem != BigNumberEncryptionMethod.Splitting)
				.OrArgumentException("When using the 'splitting' big number encryption option, the number of digits in the public key should be more than 1.");

            List<LongInt<B>> result = new List<LongInt<B>>();

            // Если число меньше ключа, можно его со спокойной душой кодировать
            // и пихать в массив.

            if (number < publicKey)
            {
                result.Add(LongInt<B>.Helper.PowerIntegerModular(number, publicExponent, publicKey));
                return result;
            }
            else if (bnem == BigNumberEncryptionMethod.Division)
            {
                while (number > publicKey)
                {
                    // Пока число больше открытого ключа, надо брать остатки от деления и
                    // кодировать их.

                    LongInt<B> remainder;

                    number = LongInt<B>.Helper.Div(number, publicKey, out remainder);
                    remainder = LongInt<B>.Helper.PowerIntegerModular(remainder, publicExponent, publicKey);

                    result.Add(remainder);
                }

                result.Add(LongInt<B>.Helper.PowerIntegerModular(number, publicExponent, publicKey));    
            }
            else if (bnem == BigNumberEncryptionMethod.Splitting)
            {
                // Разбиваем цифры исходного числа на сегменты длиной N-1, где N - длина открытого ключа.
                // Кодируем эти числа.

                List<ListSegment<int>> numberSegments = number.Digits.CoverWithSegments(publicKey.Length - 1, ListSegmentationExtensions.SegmentationOptions.SmallerLastSegment);

                foreach (ListSegment<int> ls in numberSegments)
                {
                    LongInt<B> currentEncodedNumber = new LongInt<B>(LongInt<B>.BASE, ls, false);
                    currentEncodedNumber = LongInt<B>.Helper.PowerIntegerModular(currentEncodedNumber, publicExponent, publicKey);

                    result.Add(currentEncodedNumber);
                }
            }
            else
                throw new EnumFattenedException("Big number encryption method enum has been fattened, encryption process stuck.");
            
            return result;
        }
    }
}
