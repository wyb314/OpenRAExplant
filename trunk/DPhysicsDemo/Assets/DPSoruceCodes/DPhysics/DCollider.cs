using System;
using UnityEngine;

namespace DPhysics
{
	public class DCollider : MonoBehaviour
	{
		[HideInInspector]
		public Vector2d center;

		private Vector2d rotation;

		public Vector2[] Vertices = new Vector2[0];

		[HideInInspector]
		public Vector2d[] Points;

		[HideInInspector]
		public Vector2d[] points;

		[HideInInspector]
		public Vector2d[] backupPoints;

		private Vector2d[] edges;

		public bool IsCircle;

		public double Radius;

		public FInt radius;

		public Bounder MyBounds;

		public Vector2d Center
		{
			get
			{
				return this.center;
			}
			set
			{
				if (this.center.x.RawValue != value.x.RawValue || this.center.y.RawValue != value.y.RawValue)
				{
					Vector2d vector2d;
					value.Subtract(ref this.center, out vector2d);
					this.Offset(ref vector2d);
				}
			}
		}

		public Vector2d Rotation
		{
			get
			{
				return this.rotation;
			}
			set
			{
				if (this.IsCircle)
				{
					return;
				}
				this.rotation = value;
				for (int i = 0; i < this.Points.Length; i++)
				{
					this.backupPoints[i].Rotate(this.rotation.x.RawValue, this.rotation.y.RawValue, out this.Points[i]);
				}
				this.BuildBounds();
				this.BuildPoints();
				this.BuildEdges();
			}
		}

		public Vector2d[] Edges
		{
			get
			{
				return this.edges;
			}
		}

		public void Initialize(Body body)
		{
			if (this.IsCircle)
			{
				this.radius = FInt.Create(this.Radius);
				this.radius.AbsoluteValue(out this.radius);
			}
			else
			{
				this.backupPoints = new Vector2d[this.Vertices.Length];
				this.Points = new Vector2d[this.backupPoints.Length];
				this.points = new Vector2d[this.backupPoints.Length];
				for (int i = 0; i < this.backupPoints.Length; i++)
				{
					Vector2 vector = this.Vertices[i];
					this.backupPoints[i] = new Vector2d(FInt.Create(vector.x), FInt.Create(vector.y));
					this.Points[i] = this.backupPoints[i];
					this.points[i] = this.backupPoints[i];
				}
				this.Vertices = null;
				this.edges = new Vector2d[this.Points.Length];
				this.BuildEdges();
			}
			this.BuildBounds();
		}

		public void BuildBounds()
		{
			if (this.MyBounds == null)
			{
				this.MyBounds = new Bounder(this);
				this.MyBounds.Offset(ref this.center);
				return;
			}
			this.MyBounds.BuildBounds(false);
			this.MyBounds.Offset(ref this.center);
		}

		public void BuildEdges()
		{
			for (int i = 0; i < this.edges.Length; i++)
			{
				Vector2d vector2d = this.Points[i];
				Vector2d vector2d2;
				if (i + 1 >= this.Points.Length && this.edges.Length >= 3)
				{
					vector2d2 = this.Points[0];
				}
				else
				{
					vector2d2 = this.Points[i + 1];
				}
				vector2d2.Subtract(ref vector2d, out vector2d);
				this.edges[i] = vector2d;
			}
		}

		public void BuildPoints()
		{
			if (this.IsCircle)
			{
				return;
			}
			for (int i = 0; i < this.backupPoints.Length; i++)
			{
				this.Points[i].Add(ref this.center, out this.points[i]);
			}
		}

		public void Offset(ref Vector2d change)
		{
			this.center.Add(ref change, out this.center);
			this.MyBounds.Offset(ref change);
			if (this.IsCircle)
			{
				return;
			}
			for (int i = 0; i < this.backupPoints.Length; i++)
			{
				this.points[i].Add(ref change, out this.points[i]);
			}
		}

		public override string ToString()
		{
			string text = "";
			for (int i = 0; i < this.points.Length; i++)
			{
				text += this.points[i].ToString();
				if (i != this.points.Length - 1)
				{
					text += ", ";
				}
			}
			return text;
		}

		private void OnDrawGizmos()
		{
			if (this.IsCircle)
			{
				Gizmos.DrawWireSphere(new Vector3(this.center.x.ToFloat(), 0f, this.center.y.ToFloat()), (float)this.Radius);
				return;
			}
			if (this.points != null)
			{
				for (int i = 0; i < this.points.Length; i++)
				{
					Vector3 vector = (Vector3)this.points[i];
					Vector3 vector2 = (i + 1 < this.points.Length) ? ((Vector3)this.points[i + 1]) : ((Vector3)this.points[0]);
					Gizmos.DrawSphere(vector, 0.5f);
					Gizmos.DrawLine(vector, vector2);
				}
			}
		}
	}
}
