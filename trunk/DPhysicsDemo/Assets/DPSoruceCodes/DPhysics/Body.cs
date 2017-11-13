using System;
using System.Collections.Generic;
using UnityEngine;

namespace DPhysics
{
	[RequireComponent(typeof(DCollider))]
	public class Body : MonoBehaviour
	{
		public delegate void CollisionEvent(Body other);

		public delegate void CollisionStartEvent(Body other);

		public delegate void CollisionEndEvent(Body other);

		private bool active = true;

		public bool IsTrigger;

		public int Mass = 1;

		[HideInInspector]
		public DCollider dCollider;

		[HideInInspector]
		public Vector2d Velocity;

		[HideInInspector]
		public Vector2d LastVelocity;

		public FInt Speed;

		[HideInInspector]
		public FInt RotationalVelocity;

		[HideInInspector]
		public bool Changed = true;

		[HideInInspector]
		public int ActedCount = DPhysicsManager.MaxActionsUpon;

		private Vector2d position;

		[HideInInspector]
		private Vector3 lastposition;

		[HideInInspector]
		private Vector3 curPosition;

		private bool HasInterpolated;

		[HideInInspector]
		public bool PositionChanged;

		[HideInInspector]
		public bool PositionChangedBuffer;

		public System.Collections.Generic.Dictionary<ushort, CollisionPair> MyPairs;

		private bool InterpolatePosition = true;

		protected Vector2d rotation;

		private Vector2d CacheRotation;

		private Quaternion currotation;

		private Quaternion lastrotation;

		[HideInInspector]
		public bool RotationChanged = true;

		private bool InterpolateRotation = true;

		public HashSet<Body> Children;

		[HideInInspector]
		public Body Parent;

		protected Vector2d LocalStartRotation;

		protected Vector2d LocalStartPosition;

		public Body.CollisionEndEvent OnCollideEnd;

		[HideInInspector]
		public ushort SimID;

		private Transform leTransform;

		public event Body.CollisionEvent OnCollide;

		public event Body.CollisionStartEvent OnCollideStart;

		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.Changed = true;
				this.active = value;
			}
		}

		[HideInInspector]
		public Vector2d Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.PositionChanged = true;
				this.position = value;
			}
		}

		public Vector2d Rotation
		{
			get
			{
				return this.CacheRotation;
			}
			set
			{
				this.CacheRotation = value;
				this.RotationChanged = true;
			}
		}

		public void Offset(ref Vector2d change)
		{
			this.position.Add(ref change, out this.position);
			this.PositionChanged = true;
			this.dCollider.Offset(ref change);
			if (this.Children != null)
			{
				foreach (Body current in this.Children)
				{
					current.PositionChanged = true;
					current.Offset(ref change);
				}
			}
		}

		[HideInInspector]
		public bool SetPosition()
		{
			this.lastposition = this.curPosition;
			if (!this.PositionChanged)
			{
				this.InterpolatePosition = false;
				this.PositionChangedBuffer = this.PositionChanged;
				return false;
			}
			this.HasInterpolated = false;
			this.PositionChangedBuffer = this.PositionChanged;
			this.InterpolatePosition = true;
			this.dCollider.Center = this.position;
			this.curPosition.x = this.position.x.ToFloat();
			this.curPosition.z = this.position.y.ToFloat();
			this.PositionChanged = false;
			return true;
		}

		public bool SetRotation()
		{
			this.lastrotation = this.currotation;
			this.InterpolateRotation = false;
			if (!this.RotationChanged)
			{
				return false;
			}
			this.RotationChanged = false;
			this.HasInterpolated = false;
			this.InterpolateRotation = true;
			this.rotation = this.CacheRotation;
			this.dCollider.Rotation = this.rotation;
			FInt x = this.Rotation.x;
			float arg_7B_0 = x.ToFloat();
			float arg_7B_1 = 0f;
			FInt y = this.Rotation.y;
			this.currotation = Quaternion.LookRotation(new Vector3(arg_7B_0, arg_7B_1, y.ToFloat()));
			if (this.Children != null)
			{
				foreach (Body current in this.Children)
				{
					current.LocalStartRotation.Rotate(this.rotation.x.RawValue, this.rotation.y.RawValue, out current.CacheRotation);
					current.RotationChanged = true;
					current.LocalStartPosition.Rotate(this.rotation.x.RawValue, this.rotation.y.RawValue, out current.position);
					current.position.Add(ref this.position, out current.position);
					current.PositionChanged = true;
				}
			}
			return true;
		}

		public void Attach(Body child)
		{
			if (this.Children == null)
			{
				this.Children = new HashSet<Body>();
			}
			this.Children.Add(child);
			child.rotation.Rotate(-this.rotation.x.RawValue, this.rotation.y.RawValue, out child.LocalStartRotation);
			child.position.Subtract(ref this.position, out child.LocalStartPosition);
			child.LocalStartPosition.Rotate(-this.rotation.x.RawValue, this.rotation.y.RawValue, out child.LocalStartPosition);
		}

		public void Detach(Body child)
		{
			if (this.Children.Remove(child))
			{
				child.Parent = null;
			}
		}

		public void SetLocalPosition(ref Vector2d localposition)
		{
			if (this.Parent == null)
			{
				Vector2d vector2d;
				localposition.Subtract(ref this.position, out vector2d);
				this.Offset(ref vector2d);
				return;
			}
			Vector2d vector2d2;
			localposition.Rotate(this.Parent.rotation.y.RawValue, this.Parent.rotation.x.RawValue, out vector2d2);
			vector2d2.Add(ref this.Parent.position, out vector2d2);
			Vector2d vector2d3;
			vector2d2.Subtract(ref this.position, out vector2d3);
			this.Offset(ref vector2d3);
		}

		public void Initialize(Vector2d position)
		{
			this.leTransform = transform;
			this.leTransform.position = (Vector3)position;
		    this.curPosition = this.leTransform.position;
		    this.lastposition = this.leTransform.position;
			this.dCollider = this.gameObject.GetComponent<DCollider>();
			this.dCollider.Initialize(this);
			this.Offset(ref position);
			this.Rotation = Vector2d.up;
			this.rotation = Vector2d.up;
		    this.lastrotation = Quaternion.identity;
		    this.currotation = Quaternion.identity;
			DPhysicsManager.Assimilate(this);
		}

		public void StartCollision(Body body)
		{
			if (this.OnCollideStart != null)
			{
				this.OnCollideStart(body);
			}
		}

		public void DoCollision(Body body)
		{
			if (this.OnCollide != null)
			{
				this.OnCollide(body);
			}
		}

		public void EndCollision(Body body)
		{
			if (this.OnCollideEnd != null)
			{
				this.OnCollideEnd(body);
			}
		}

		public void ApplyForce(ref Vector2d force)
		{
			if (this.Mass <= 0 || !this.active)
			{
				return;
			}
			Vector2d vector2d;
			force.Divide(this.Mass, out vector2d);
			this.Velocity.Add(ref vector2d, out this.Velocity);
		}

		public void ApplyVelocity(ref Vector2d vel)
		{
			if (this.Mass <= 0 || !this.active)
			{
				return;
			}
			this.Velocity.Add(ref vel, out this.Velocity);
		}

		public void ApplyRotationalVelocity(FInt vel)
		{
			if (this.Mass <= 0 || !this.active)
			{
				return;
			}
			this.RotationalVelocity += vel;
		}

		public void Visualize(float LerpTime)
		{
			if (this.leTransform != null)
			{
				if (this.InterpolatePosition || this.HasInterpolated)
				{
					this.leTransform.position = (Vector3.Lerp(this.lastposition, this.curPosition, LerpTime));
				}
				else
				{
					this.lastposition = this.curPosition;
					this.leTransform.position = (this.curPosition);
				}
				if (this.InterpolateRotation || !this.HasInterpolated)
				{
					this.leTransform.rotation = (Quaternion.Lerp(this.lastrotation, this.currotation, LerpTime));
				}
				else
				{
					this.leTransform.rotation = (this.currotation);
				}
				this.HasInterpolated = true;
			}
		}
	}
}
