namespace WhiteMath.ArithmeticLong.Infrastructure
{
	/// <summary>
	/// Static class containing the precisions for the LongExp numbers.
	/// </summary>
	public static class Precisions
	{
		/// <summary>
		/// The precision for 20 000 decimal signs with digit base 10 000.
		/// </summary>
		public class P_20k_10k : Bases.B10k
		{
			public int Precision => 20000;
		}

		/// <summary>
		/// The precision for 50 000 decimal signs with digit base 10 000.
		/// </summary>
		public class P_50k_10k : Bases.B10k
		{
			public int Precision => 50000;
		}

		/// <summary>
		/// The precision for 100 000 decimal signs with digit base 10 000.
		/// </summary>
		public class P_100k_10k : Bases.B10k
		{
			public int Precision => 100000;
		}

		/// <summary>
		/// The precision for 1 000 000 decimal signs with digit base 10 000.
		/// </summary>
		public class P_1000k_10k : Bases.B10k
		{
			public int Precision => 1000000;
		}
	}
}
