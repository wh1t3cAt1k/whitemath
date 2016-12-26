namespace WhiteMath.ArithmeticLong.Infrastructure
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
}
