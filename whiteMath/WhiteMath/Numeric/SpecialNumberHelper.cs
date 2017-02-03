using System;

namespace WhiteMath.Numeric
{
	public static class SpecialNumberHelper
	{
		public static SpecialNumberType? Add(SpecialNumberType firstNumberType, SpecialNumberType secondNumberType)
		{
			switch (firstNumberType)
			{
				case SpecialNumberType.NegativeInfinity:
					switch (secondNumberType)
					{
						case SpecialNumberType.NegativeInfinity:
						case SpecialNumberType.None:
							return SpecialNumberType.NegativeInfinity;
						case SpecialNumberType.PositiveInfinity:
						case SpecialNumberType.NaN:
							return SpecialNumberType.NaN;
						default:
							throw new ArgumentOutOfRangeException(nameof(secondNumberType));
					}
				case SpecialNumberType.None:
					switch (secondNumberType)
					{
						case SpecialNumberType.NegativeInfinity:
						case SpecialNumberType.PositiveInfinity:
						case SpecialNumberType.NaN:
							return secondNumberType;
						case SpecialNumberType.None:
							return null;
						default:
							throw new ArgumentOutOfRangeException(nameof(secondNumberType));
					}
				case SpecialNumberType.PositiveInfinity:
					switch (secondNumberType)
					{
						case SpecialNumberType.NegativeInfinity:
							return SpecialNumberType.NaN;
						case SpecialNumberType.None:
						case SpecialNumberType.PositiveInfinity:
							return SpecialNumberType.PositiveInfinity;
						case SpecialNumberType.NaN:
							return SpecialNumberType.NaN;
						default:
							throw new ArgumentOutOfRangeException(nameof(secondNumberType));
					}
				case SpecialNumberType.NaN:
					switch (secondNumberType)
					{
						case SpecialNumberType.NegativeInfinity:
						case SpecialNumberType.None:
						case SpecialNumberType.PositiveInfinity:
						case SpecialNumberType.NaN:
							return SpecialNumberType.NaN;
						default:
							throw new ArgumentOutOfRangeException(nameof(secondNumberType));
					}
				default:
					throw new ArgumentOutOfRangeException(nameof(firstNumberType));
			}
		}

		public static bool? IsGreaterThan(SpecialNumberType firstNumberType, SpecialNumberType secondNumberType)
		{
			switch (firstNumberType)
			{
				case SpecialNumberType.NegativeInfinity: 
					return false;
				case SpecialNumberType.None:
					switch (secondNumberType)
					{
						case SpecialNumberType.NegativeInfinity:
							return true;
						case SpecialNumberType.None:
							return null;
						case SpecialNumberType.PositiveInfinity:
						case SpecialNumberType.NaN:
							return false;
						default:
							throw new ArgumentException(nameof(secondNumberType));
					}
				case SpecialNumberType.PositiveInfinity:
					switch (secondNumberType)
					{
						case SpecialNumberType.NegativeInfinity:
						case SpecialNumberType.None:
							return true;
						case SpecialNumberType.PositiveInfinity:
						case SpecialNumberType.NaN:
							return false;
						default:
							throw new ArgumentException(nameof(secondNumberType));
					}
				case SpecialNumberType.NaN:
					return false;
				default:
					throw new ArgumentException(nameof(firstNumberType));
			}
		}

		public static bool? AreEqual(SpecialNumberType firstNumberType, SpecialNumberType secondNumberType)
		{
			switch (firstNumberType)
			{
				case SpecialNumberType.NegativeInfinity:
					switch (secondNumberType)
					{
						case SpecialNumberType.NegativeInfinity:
							return true;
						case SpecialNumberType.None:
						case SpecialNumberType.PositiveInfinity:
						case SpecialNumberType.NaN:
							return false;
						default:
							throw new ArgumentException(nameof(secondNumberType));
					}
				case SpecialNumberType.None:
					switch (secondNumberType)
					{
						case SpecialNumberType.NegativeInfinity:
							return false;
						case SpecialNumberType.None:
							return null;
						case SpecialNumberType.PositiveInfinity:
						case SpecialNumberType.NaN:
							return false;
						default:
							throw new ArgumentException(nameof(secondNumberType));
					}
				case SpecialNumberType.PositiveInfinity:
					switch (secondNumberType)
					{
						case SpecialNumberType.NegativeInfinity:
						case SpecialNumberType.None:
							return false;
						case SpecialNumberType.PositiveInfinity:
							return true;
						case SpecialNumberType.NaN:
							return false;
						default:
							throw new ArgumentException(nameof(secondNumberType));
					}
				case SpecialNumberType.NaN:
					return false;
				default:
					throw new ArgumentException(nameof(firstNumberType));
			}
		}
	}
}
