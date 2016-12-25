using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.ArithmeticLong
{
    public static class Bases
    {
        public class B_100k : IBase
        {
            public int Base => 100000;
        }

        public class B_10k : IBase
        {
			public int Base => 10000;
        }

        public class B_1000 : IBase
        {
			public int Base => 1000;
        }

        public class B_100 : IBase
        {
			public int Base => 100;
        }

        public class B_10 : IBase
        {
			public int Base => 10;
        }

        public class B_256 : IBase
        {
			public int Base => 256;
        }

        public class B_32768 : IBase
        {
			public int Base => 32768;
        }

        public class B_65536 : IBase
        {
			public int Base => 65536;
        }
    }

    /// <summary>
    /// Static class containing the precisions for the LongExp numbers.
    /// </summary>
    public static class Precisions
    {
        /// <summary>
        /// The precision for 20 000 decimal signs with digit base 10 000.
        /// </summary>
        public class P_20k_10k: Bases.B_10k
        {
			public int Precision => 20000;
        }

        /// <summary>
        /// The precision for 50 000 decimal signs with digit base 10 000.
        /// </summary>
        public class P_50k_10k: Bases.B_10k
        {
			public int Precision => 50000;
        }

        /// <summary>
        /// The precision for 100 000 decimal signs with digit base 10 000.
        /// </summary>
        public class P_100k_10k: Bases.B_10k
        {
			public int Precision => 100000;
        }

        /// <summary>
        /// The precision for 1 000 000 decimal signs with digit base 10 000.
        /// </summary>
        public class P_1000k_10k: Bases.B_10k
        {
			public int Precision => 1000000;
        }

    }
}
