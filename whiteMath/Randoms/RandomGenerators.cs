using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Randoms
{
    /// <summary>
    /// Contains ready-to-use random generators.
    /// </summary>
    public static class RandomGenerators
    {
        /// <summary>
        /// Represents a <see cref="RandomLaggedFibonacci"/> floating-point generator.
        /// </summary>
        public static readonly RandomLaggedFibonacci LaggedFibonacci = new RandomLaggedFibonacci();

        /// <summary>
        /// Represents a <see cref="RandomStandard"/> universal wrapper around .NET standard
        /// random generator.
        /// </summary>
        public static readonly RandomStandard Standard = new RandomStandard();

        /// <summary>
        /// Represents a <see cref="RandomMersenneTwister"/> universal generator.
        /// </summary>
        public static readonly RandomMersenneTwister MersenneTwister = new RandomMersenneTwister();
    }
}
