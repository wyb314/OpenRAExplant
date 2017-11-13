using System;

namespace DPhysics
{
	public class Bounder
	{
		private DCollider polygon;

		public FInt Radius;

		private FInt SqrRadius;

		public long xMax;

		public long xMin;

		public long yMax;

		public long yMin;

		public bool IsCircle;

		public Bounder(DCollider pol)
		{
			this.polygon = pol;
			this.BuildBounds(true);
		}

		public void BuildBounds(bool Thorough)
		{
			if (this.polygon.IsCircle)
			{
				this.Radius = this.polygon.radius;
				this.yMax = this.Radius.RawValue;
				this.xMax = this.yMax;
				this.yMin = -this.Radius.RawValue;
				this.xMin = this.yMin;
				this.IsCircle = true;
				return;
			}
			if (this.polygon.backupPoints.Length > 0)
			{
				this.xMin = 0L;
				this.xMax = 0L;
				this.yMin = 0L;
				this.yMax = 0L;
				Vector2d[] points = this.polygon.Points;
				for (int i = 0; i < points.Length; i++)
				{
					Vector2d vector2d = points[i];
					if (Thorough)
					{
						FInt sqrRadius;
						vector2d.SqrMagnitude(out sqrRadius);
						if (sqrRadius.RawValue > this.SqrRadius.RawValue)
						{
							this.SqrRadius = sqrRadius;
							Mathd.Sqrt(sqrRadius.RawValue, out this.Radius);
						}
					}
					if (vector2d.x.RawValue < this.xMin)
					{
						this.xMin = vector2d.x.RawValue;
					}
					else if (vector2d.x.RawValue > this.xMax)
					{
						this.xMax = vector2d.x.RawValue;
					}
					if (vector2d.y.RawValue < this.yMin)
					{
						this.yMin = vector2d.y.RawValue;
					}
					else if (vector2d.y.RawValue > this.yMax)
					{
						this.yMax = vector2d.y.RawValue;
					}
				}
				if (Thorough && (this.Radius * this.Radius * Mathd.PI).RawValue <= (this.yMax - this.yMin) * (this.xMax - this.xMin) >> 20)
				{
					this.IsCircle = true;
					this.xMax = this.Radius.RawValue;
					this.yMax = this.xMax;
					this.yMin = -this.Radius.RawValue;
					this.xMin = this.yMin;
				}
			}
		}

		public void Offset(ref Vector2d change)
		{
			this.xMin += change.x.RawValue;
			this.xMax += change.x.RawValue;
			this.yMin += change.y.RawValue;
			this.yMax += change.y.RawValue;
		}

		public static bool CanIntersect(DCollider polyA, DCollider polyB)
		{
			return polyA.MyBounds.xMax >= polyB.MyBounds.xMin && polyA.MyBounds.xMin <= polyB.MyBounds.xMax && polyA.MyBounds.yMax >= polyB.MyBounds.yMin && polyA.MyBounds.yMin <= polyB.MyBounds.yMax;
		}

		public static bool CanIntersect(DCollider polyA, DCollider polyB, ref FInt CombinedSqrRadius, out FInt sqrdistance)
		{
			if (polyA.MyBounds.xMax >= polyB.MyBounds.xMin && polyA.MyBounds.xMin <= polyB.MyBounds.xMax && polyA.MyBounds.yMax >= polyB.MyBounds.yMin && polyA.MyBounds.yMin <= polyB.MyBounds.yMax)
			{
				Vector2d vector2d;
				polyA.center.Subtract(ref polyB.center, out vector2d);
				vector2d.SqrMagnitude(out sqrdistance);
				if (sqrdistance.RawValue <= CombinedSqrRadius.RawValue)
				{
					return true;
				}
			}
			sqrdistance.RawValue = 0L;
			return false;
		}
	}
}
