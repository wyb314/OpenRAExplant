using System;

public struct FInt
{
	public const int SHIFT_AMOUNT = 20;

	public const long MAX_VALUE = 8796093022207L;

	public const long OneRaw = 1048576L;

	public long RawValue;

	public static FInt OneF = FInt.Create(1);

	public static FInt TwoF = FInt.Create(2);

	public static FInt ZeroF = FInt.Create(0);

	public static FInt HalfF = FInt.Create(524288L);

	public static FInt MaxValue = FInt.Create(9223372036854775807L);

	public static FInt Create(int StartingValue)
	{
		FInt result;
		result.RawValue = (long)StartingValue;
		result.RawValue <<= 20;
		return result;
	}

	public static FInt Create(long StartingRawValue)
	{
		FInt result;
		result.RawValue = StartingRawValue;
		return result;
	}

	public static FInt Create(float FloatValue)
	{
		FInt result;
		result.RawValue = (long)((decimal)FloatValue * 1048576m);
		return result;
	}

	public static FInt Create(double DoubleValue)
	{
		FInt result;
		result.RawValue = (long)((decimal)DoubleValue * 1048576m);
		return result;
	}

	public static FInt FromParts(long PreDecimal, long PostDecimal)
	{
		FInt result = FInt.Create(PreDecimal);
		if (PostDecimal != 0L)
		{
			result.RawValue += (FInt.Create((double)PostDecimal) / 1000).RawValue;
		}
		return result;
	}

	public void Multiply(long OtherRawValue, out FInt ret)
	{
		ret.RawValue = this.RawValue * OtherRawValue >> 20;
	}

	public void Multiply(int OtherValue, out FInt ret)
	{
		ret.RawValue = this.RawValue * (long)OtherValue;
	}

	public static FInt operator *(FInt one, FInt other)
	{
		one.RawValue = one.RawValue * other.RawValue >> 20;
		return one;
	}

	public static FInt operator *(FInt one, int multi)
	{
		one.RawValue *= (long)multi;
		return one;
	}

	public void Divide(long OtherRawValue, out FInt ret)
	{
		ret.RawValue = (this.RawValue << 20) / OtherRawValue;
	}

	public void Divide(int OtherValue, out FInt ret)
	{
		ret.RawValue = this.RawValue / (long)OtherValue;
	}

	public static FInt operator /(FInt one, FInt other)
	{
		one.RawValue = (one.RawValue << 20) / other.RawValue;
		return one;
	}

	public static FInt operator /(FInt one, int divisor)
	{
		one.RawValue /= (long)divisor;
		return one;
	}

	public void Modulo(long OtherRawValue, out FInt ret)
	{
		ret.RawValue = this.RawValue % OtherRawValue;
	}

	public void Modulo(int OtherValue, out FInt ret)
	{
		ret.RawValue = this.RawValue % ((long)OtherValue << 20);
	}

	public static FInt operator %(FInt one, FInt other)
	{
		one.Modulo(other.RawValue, out one);
		return one;
	}

	public void Add(long OtherRawValue, out FInt ret)
	{
		ret.RawValue = this.RawValue + OtherRawValue;
	}

	public void Add(int OtherValue, out FInt ret)
	{
		ret.RawValue = this.RawValue + ((long)OtherValue << 20);
	}

	public static FInt operator +(FInt one, FInt other)
	{
		one.RawValue += other.RawValue;
		return one;
	}

	public static FInt operator +(FInt one, int other)
	{
		one.RawValue += (long)other << 20;
		return one;
	}

	public void Subtract(long OtherRawValue, out FInt ret)
	{
		ret.RawValue = this.RawValue - OtherRawValue;
	}

	public void Subtract(int OtherValue, out FInt ret)
	{
		ret.RawValue = this.RawValue - ((long)OtherValue << 20);
	}

	public static FInt operator -(FInt one, FInt other)
	{
		one.RawValue -= other.RawValue;
		return one;
	}

	public static FInt operator -(FInt one, int other)
	{
		one.RawValue -= (long)other << 20;
		return one;
	}

	public bool Equals(long OtherRawValue)
	{
		return this.RawValue == OtherRawValue;
	}

	public static bool operator ==(FInt one, FInt other)
	{
		return one.Equals(other.RawValue);
	}

	public static bool operator !=(FInt one, FInt other)
	{
		return !one.Equals(other.RawValue);
	}

	public bool MoreEquals(long OtherRawValue)
	{
		return this.RawValue >= OtherRawValue;
	}

	public bool LessEquals(long OtherRawValue)
	{
		return this.RawValue <= OtherRawValue;
	}

	public bool More(long OtherRawValue)
	{
		return this.RawValue > OtherRawValue;
	}

	public bool Less(long OtherRawValue)
	{
		return this.RawValue < OtherRawValue;
	}

	public bool AbsoluteValueMoreThan(long OtherRawValue)
	{
		return this.RawValue > OtherRawValue || this.RawValue < -OtherRawValue;
	}

	public void AbsoluteValue(out FInt ret)
	{
		if (this.RawValue < 0L)
		{
			ret.RawValue = -this.RawValue;
			return;
		}
		ret.RawValue = this.RawValue;
	}

	public void Sign()
	{
		if (this.RawValue != 0L)
		{
			if (this.RawValue > 0L)
			{
				this.RawValue = FInt.OneF.RawValue;
				return;
			}
			this.RawValue = -FInt.OneF.RawValue;
		}
	}

	public void Inverse(out FInt ret)
	{
		ret.RawValue = this.RawValue * -1L;
	}

	public static FInt operator -(FInt src)
	{
		src.Inverse(out src);
		return src;
	}

	public float ToFloat()
	{
		return (float)((double)this.RawValue / 1048576.0);
	}

	public int ToInt()
	{
		return (int)(this.RawValue >> 20);
	}

	public double ToDouble()
	{
		return (double)this.RawValue / 1048576.0;
	}

	public short ToRoundedShort()
	{
		return (short)(this.RawValue >> 20);
	}

	public override int GetHashCode()
	{
		return this.RawValue.GetHashCode();
	}

	public override string ToString()
	{
		return this.RawValue.ToString();
	}
}
