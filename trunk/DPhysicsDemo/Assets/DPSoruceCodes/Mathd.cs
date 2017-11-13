using System;

public static class Mathd
{
	private const int HalfShift = 10;

	public static FInt PI = FInt.Create(3.1415);

	public static void Sqrt(long RawValue, out FInt ret)
	{
		if (RawValue > 0L)
		{
			long num = Mathd.IntSqrt(RawValue);
			num <<= 10;
			ret.RawValue = num;
			return;
		}
		ret.RawValue = RawValue;
	}

	public static long IntSqrt(long d)
	{
		if (d > 0L)
		{
			long num = (d >> 1) + 1L;
			long num2;
			for (num2 = num + d / num >> 1; num2 < num; num2 = num + d / num >> 1)
			{
				num = num2;
			}
			return num2;
		}
		return d;
	}
}
