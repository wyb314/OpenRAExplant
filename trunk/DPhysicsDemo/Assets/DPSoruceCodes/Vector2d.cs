using System;
using UnityEngine;

public struct Vector2d
{
	public FInt x;

	public FInt y;

	public static Vector2d zero = new Vector2d(0, 0);

	public static Vector2d one = new Vector2d(1, 1);

	public static Vector2d up = new Vector2d(0, 1);

	public static Vector2d right = new Vector2d(1, 0);

	public FInt this[int index]
	{
		get
		{
			switch (index)
			{
			case 0:
				return this.x;
			case 1:
				return this.y;
			default:
				throw new System.IndexOutOfRangeException("Invalid Vector2d index!");
			}
		}
		set
		{
			switch (index)
			{
			case 0:
				this.x = value;
				return;
			case 1:
				this.y = value;
				return;
			default:
				throw new System.IndexOutOfRangeException("Invalid Vector2d index!");
			}
		}
	}

	public Vector2d localright
	{
		get
		{
			return new Vector2d(this.y, -this.x);
		}
	}

	public FInt Magnitude(out FInt ret)
	{
		FInt fInt;
		this.x.AbsoluteValue(out fInt);
		if (fInt.RawValue == 0L)
		{
			if (this.y.RawValue == -1L || this.y.RawValue == 1L)
			{
				ret.RawValue = 1048576L;
			}
		}
		else if (fInt.RawValue == 1L && this.y.RawValue == 0L)
		{
			ret.RawValue = 1048576L;
		}
		this.x.Multiply(this.x.RawValue, out ret);
		FInt fInt2;
		this.y.Multiply(this.y.RawValue, out fInt2);
		ret.Add(fInt2.RawValue, out ret);
		Mathd.Sqrt(ret.RawValue, out ret);
		return ret;
	}

	public FInt SqrMagnitude(out FInt ret)
	{
		this.x.Multiply(this.x.RawValue, out ret);
		FInt fInt;
		this.y.Multiply(this.y.RawValue, out fInt);
		ret.Add(fInt.RawValue, out ret);
		return ret;
	}

	public Vector2d(FInt x, FInt y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2d(int xInt, int yInt)
	{
		this.x.RawValue = (long)xInt << 20;
		this.y.RawValue = (long)yInt << 20;
	}

	public static explicit operator Vector2d(Vector3 v)
	{
		return new Vector2d(FInt.Create(v.x), FInt.Create(v.z));
	}

	public static explicit operator Vector3(Vector2d v)
	{
		return new Vector3(v.x.ToFloat(), 0f, v.y.ToFloat());
	}

	public void Add(ref Vector2d Other, out Vector2d ret)
	{
		this.x.Add(Other.x.RawValue, out ret.x);
		this.y.Add(Other.y.RawValue, out ret.y);
	}

	public void Subtract(ref Vector2d Other, out Vector2d ret)
	{
		this.x.Subtract(Other.x.RawValue, out ret.x);
		this.y.Subtract(Other.y.RawValue, out ret.y);
	}

	public void Multiply(long OtherRawValue, out Vector2d ret)
	{
		this.x.Multiply(OtherRawValue, out ret.x);
		this.y.Multiply(OtherRawValue, out ret.y);
	}

	public void Multiply(int OtherValue, out Vector2d ret)
	{
		this.x.Multiply(OtherValue, out ret.x);
		this.y.Multiply(OtherValue, out ret.y);
	}

	public void Divide(long OtherRawValue, out Vector2d ret)
	{
		this.x.Divide(OtherRawValue, out ret.x);
		this.y.Divide(OtherRawValue, out ret.y);
	}

	public void Divide(int OtherValue, out Vector2d ret)
	{
		this.x.Divide(OtherValue, out ret.x);
		this.y.Divide(OtherValue, out ret.y);
	}

	public void Invert()
	{
		this.x.RawValue = -this.x.RawValue;
		this.y.RawValue = -this.y.RawValue;
	}

	public static Vector2d operator +(Vector2d a, Vector2d b)
	{
		a.Add(ref b, out a);
		return a;
	}

	public static Vector2d operator -(Vector2d a, Vector2d b)
	{
		a.Subtract(ref b, out a);
		return a;
	}

	public static Vector2d operator -(Vector2d a)
	{
		return new Vector2d(-a.x, -a.y);
	}

	public static Vector2d operator *(Vector2d a, FInt d)
	{
		a.Multiply(d.RawValue, out a);
		return a;
	}

	public static Vector2d operator *(Vector2d a, int d)
	{
		a.Multiply(d, out a);
		return a;
	}

	public static Vector2d operator /(Vector2d a, FInt d)
	{
		a.Divide(d.RawValue, out a);
		return a;
	}

	public static Vector2d operator /(Vector2d a, int d)
	{
		a.Divide(d, out a);
		return a;
	}

	public void Normalize()
	{
		if (this.x.RawValue == 0L && this.y.RawValue == 0L)
		{
			return;
		}
		FInt fInt;
		this.x.AbsoluteValue(out fInt);
		FInt fInt2;
		this.y.AbsoluteValue(out fInt2);
		if (fInt.RawValue == 0L)
		{
			if (fInt2.RawValue == 1048576L)
			{
				return;
			}
		}
		else if (fInt2.RawValue == 0L && fInt.RawValue == 1048576L)
		{
			return;
		}
		if (fInt.RawValue > fInt2.RawValue)
		{
			this.Divide(fInt.RawValue + fInt2.RawValue / 2L, out this);
		}
		else
		{
			this.Divide(fInt2.RawValue + fInt.RawValue / 2L, out this);
		}
		FInt fInt3;
		this.Magnitude(out fInt3);
		if (fInt3.RawValue > 0L && fInt3.RawValue != 1048576L)
		{
			this.Divide(fInt3.RawValue, out this);
		}
	}

	public Vector2 ToSinglePrecision()
	{
		return new Vector2(this.x.ToFloat(), this.y.ToFloat());
	}

	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"(",
			this.x.ToDouble().ToString(),
			", ",
			this.y.ToDouble().ToString(),
			")"
		});
	}

	public override int GetHashCode()
	{
		return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
	}

	public bool Equals(ref Vector2d other)
	{
		return this.x.RawValue == other.x.RawValue && this.y.RawValue == other.y.RawValue;
	}

	public static void Dot(ref Vector2d lhs, ref Vector2d rhs, out FInt ret)
	{
		lhs.x.Multiply(rhs.x.RawValue, out ret);
		FInt fInt;
		lhs.y.Multiply(rhs.y.RawValue, out fInt);
		ret.Add(fInt.RawValue, out ret);
	}

	public static void Cross(ref Vector2d U, ref Vector2d B, out FInt ret)
	{
		U.x.Multiply(B.y.RawValue, out ret);
		FInt fInt;
		U.y.Multiply(B.x.RawValue, out fInt);
		ret.Subtract(fInt.RawValue, out ret);
	}

	public static void Reflect(ref Vector2d vector, ref Vector2d normal, out Vector2d ret)
	{
		FInt fInt;
		Vector2d.Dot(ref vector, ref normal, out fInt);
		normal.Multiply(fInt.RawValue, out ret);
		ret.Multiply(2, out ret);
		ret.Subtract(ref vector, out ret);
	}

	public void Rotate(long CosRaw, long SinRaw, out Vector2d ret)
	{
		FInt fInt;
		this.x.Multiply(CosRaw, out fInt);
		FInt fInt2;
		this.y.Multiply(-SinRaw, out fInt2);
		fInt.Add(fInt2.RawValue, out fInt);
		FInt src;
		this.x.Multiply(SinRaw, out src);
		this.y.Multiply(CosRaw, out fInt2);
		src.Add(fInt2.RawValue, out src);
		ret.x = -src;
		ret.y = fInt;
	}

	public void RotateTowards(ref Vector2d target, FInt amount, out Vector2d ret)
	{
		ret = this;
		Vector2d localright = this.localright;
		FInt fInt;
		Vector2d.Dot(ref localright, ref target, out fInt);
		int num;
		if (fInt.RawValue == 0L)
		{
			num = 0;
		}
		else if (fInt.RawValue > 0L)
		{
			num = 1;
		}
		else
		{
			num = -1;
		}
		if (!num.Equals(0))
		{
			Vector2d vector2d;
			localright.Multiply(num, out vector2d);
			vector2d.Multiply(amount.RawValue, out vector2d);
			Vector2d vector2d2;
			this.Add(ref vector2d, out vector2d2);
			Vector2d localright2 = vector2d2.localright;
			FInt fInt2;
			Vector2d.Dot(ref localright2, ref target, out fInt2);
			fInt2.Sign();
			if (fInt2.RawValue == (long)num || fInt2.RawValue > 0L == num > 0)
			{
				vector2d2.Normalize();
				ret = vector2d2;
				return;
			}
			ret = target;
		}
	}
}
