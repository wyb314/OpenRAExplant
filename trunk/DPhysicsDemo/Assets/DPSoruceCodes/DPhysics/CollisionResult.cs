using System;

namespace DPhysics
{
	public class CollisionResult
	{
		public bool Intersect;

		public Vector2d PenetrationVector;

		public Vector2d PenetrationDirection;

		public CollisionPair pair;

		public CollisionResult(CollisionPair _pair)
		{
			this.pair = _pair;
		}

		public void Calculate()
		{
			this.Intersect = false;
			this.PenetrationVector = Vector2d.zero;
			DCollider dCollider = this.pair.BodyA.dCollider;
			DCollider dCollider2 = this.pair.BodyB.dCollider;
			if (dCollider.MyBounds.IsCircle && dCollider2.MyBounds.IsCircle)
			{
				FInt fInt;
				if (Bounder.CanIntersect(dCollider, dCollider2, ref this.pair.CombinedSqrRadius, out fInt))
				{
					if (!dCollider.IsCircle || !dCollider2.IsCircle)
					{
						goto IL_155;
					}
					this.Intersect = true;
					FInt other;
					Mathd.Sqrt(fInt.RawValue, out other);
					Vector2d penetrationDirection;
					if (other.RawValue > 0L)
					{
						dCollider.center.Subtract(ref dCollider2.center, out penetrationDirection);
						penetrationDirection.Normalize();
					}
					else
					{
						Vector2d vector2d;
						dCollider.center.Subtract(ref this.pair.BodyA.Velocity, out vector2d);
						Vector2d vector2d2;
						dCollider2.center.Subtract(ref this.pair.BodyB.Velocity, out vector2d2);
						vector2d.Subtract(ref vector2d2, out penetrationDirection);
						penetrationDirection.Magnitude(out other);
						penetrationDirection.Normalize();
					}
					penetrationDirection.Multiply((dCollider.radius + dCollider2.radius - other).RawValue, out this.PenetrationVector);
					this.PenetrationDirection = penetrationDirection;
				}
				return;
			}
			if (!Bounder.CanIntersect(dCollider, dCollider2))
			{
				return;
			}
			IL_155:
			int num = (dCollider.Edges != null) ? dCollider.Edges.Length : 0;
			int num2 = (dCollider2.Edges != null) ? dCollider2.Edges.Length : 0;
			FInt fInt2 = FInt.MaxValue;
			Vector2d penetrationDirection2 = default(Vector2d);
			this.Intersect = true;
			for (int i = 0; i < num + num2; i++)
			{
				Vector2d vector2d3;
				if (i < num)
				{
					vector2d3 = dCollider.Edges[i];
				}
				else
				{
					vector2d3 = dCollider2.Edges[i - num];
				}
				Vector2d localright = vector2d3.localright;
				localright.Normalize();
				FInt zeroF = FInt.ZeroF;
				FInt zeroF2 = FInt.ZeroF;
				FInt zeroF3 = FInt.ZeroF;
				FInt zeroF4 = FInt.ZeroF;
				CollisionResult.ProjectPolygon(localright, dCollider, out zeroF, out zeroF3);
				CollisionResult.ProjectPolygon(localright, dCollider2, out zeroF2, out zeroF4);
				FInt fInt3 = CollisionResult.IntervalDistance(zeroF, zeroF3, zeroF2, zeroF4);
				if (fInt3.RawValue >= 0L)
				{
					this.Intersect = false;
					return;
				}
				fInt3.Inverse(out fInt3);
				if (fInt3.RawValue < fInt2.RawValue)
				{
					fInt2 = fInt3;
					penetrationDirection2 = localright;
					Vector2d vector2d4;
					dCollider.center.Subtract(ref dCollider2.center, out vector2d4);
					FInt fInt4;
					Vector2d.Dot(ref vector2d4, ref penetrationDirection2, out fInt4);
					if (fInt4.RawValue < 0L)
					{
						penetrationDirection2.Invert();
					}
				}
			}
			this.PenetrationDirection = penetrationDirection2;
			penetrationDirection2.Multiply(fInt2.RawValue, out this.PenetrationVector);
		}

		public static FInt IntervalDistance(FInt minA, FInt maxA, FInt minB, FInt maxB)
		{
			if (minA.RawValue < minB.RawValue)
			{
				return minB - maxA;
			}
			return minA - maxB;
		}

		public static void ProjectPolygon(Vector2d axis, DCollider dCollider, out FInt min, out FInt max)
		{
			min.RawValue = 0L;
			max.RawValue = 0L;
			if (dCollider.IsCircle)
			{
				FInt fInt;
				Vector2d.Dot(ref dCollider.center, ref axis, out fInt);
				fInt.Subtract(dCollider.radius.RawValue, out min);
				fInt.Add(dCollider.radius.RawValue, out max);
				return;
			}
			FInt fInt2;
			Vector2d.Dot(ref axis, ref dCollider.points[0], out fInt2);
			min.RawValue = fInt2.RawValue;
			max.RawValue = fInt2.RawValue;
			for (int i = 0; i < dCollider.points.Length; i++)
			{
				Vector2d.Dot(ref dCollider.points[i], ref axis, out fInt2);
				if (fInt2.RawValue < min.RawValue)
				{
					min = fInt2;
				}
				else if (fInt2.RawValue > max.RawValue)
				{
					max = fInt2;
				}
			}
		}
	}
}
