using System;
using System.Collections.Generic;
using UnityEngine;

namespace DPhysics
{
	public static class DPhysicsManager
	{
		public const int MaxPhysicsObjects = 2000;

		public static FInt SimulationDelta = FInt.Create(0.1);

		public static FInt Drag = FInt.Create(0.8);

		public static FInt SleepVelocity = FInt.Create(0.1);

		public static FInt CollisionDamp = FInt.Create(0.8);

		public static FInt MinimumCollisionOffset = FInt.Create(0.1);

		public static FInt Restitution = FInt.Create(0.8);

		public static int MaxActionsUpon = 8;

		private static Stack<ushort> AvailableCounts = new Stack<ushort>();

		private static Vector2d randomDirection = new Vector2d(3, 5);

		private static Vector2d randomChange = new Vector2d(-13, 7);

		public static Body[] SimObjects = new Body[2000];

		private static ushort PeakCount = 0;

		private static float LastSimulateTime;

		public static Vector2d RandomDirection
		{
			get
			{
				DPhysicsManager.randomDirection.Rotate(DPhysicsManager.randomChange.x.RawValue, DPhysicsManager.randomChange.y.RawValue, out DPhysicsManager.randomDirection);
				FInt x = DPhysicsManager.randomChange.x;
				DPhysicsManager.randomChange.x = DPhysicsManager.randomChange.y;
				DPhysicsManager.randomChange.y = -x;
				DPhysicsManager.randomChange.Normalize();
				DPhysicsManager.randomDirection.Normalize();
				return DPhysicsManager.randomDirection;
			}
		}

		public static void Simulate()
		{
			Partition.StartPartitioning();
			for (ushort num = 0; num < DPhysicsManager.PeakCount; num += 1)
			{
				Body body = DPhysicsManager.SimObjects[(int)num];
				if (!(body == null) && body.Active)
				{
					System.Collections.Generic.Dictionary<ushort, CollisionPair> myPairs = body.MyPairs;
					for (ushort num2 = (ushort)(num + (ushort)1); num2 < DPhysicsManager.PeakCount; num2 += 1)
					{
						Body body2 = DPhysicsManager.SimObjects[(int)num2];
						if (!(body2 == null))
						{
							CollisionPair collisionPair = myPairs[num2];
							collisionPair.GenerateCollision();
							CollisionResult myCollisionResult = collisionPair.MyCollisionResult;
							if (myCollisionResult.Intersect && collisionPair.SimulatePhysics)
							{
								bool flag = body.ActedCount > 0;
								bool flag2 = body2.ActedCount > 0;
								if (flag)
								{
									body.ActedCount--;
								}
								if (flag2)
								{
									body2.ActedCount--;
								}
								bool flag3 = body.Mass != 0;
								bool flag4 = body2.Mass != 0;
								if (!body.IsTrigger && !body2.IsTrigger && (flag3 || flag4))
								{
									bool flag5 = true;
									Vector2d vector2d;
									body2.Velocity.Subtract(ref body.Velocity, out vector2d);
									if (vector2d.x.AbsoluteValueMoreThan(DPhysicsManager.SleepVelocity.RawValue) || vector2d.y.AbsoluteValueMoreThan(DPhysicsManager.SleepVelocity.RawValue))
									{
										FInt fInt;
										Vector2d.Dot(ref myCollisionResult.PenetrationVector, ref vector2d, out fInt);
										if (fInt.RawValue < 0L)
										{
											flag5 = false;
										}
									}
									if (flag5)
									{
										Vector2d vector2d2;
										myCollisionResult.PenetrationVector.Multiply(DPhysicsManager.CollisionDamp.RawValue, out vector2d2);
										if (!flag3 || !flag4)
										{
											if (!flag3)
											{
												myCollisionResult.PenetrationVector.Invert();
												body2.Offset(ref vector2d2);
											}
											else
											{
												body.Offset(ref vector2d2);
											}
										}
										else
										{
											FInt fInt2;
											body.Speed.Add(body2.Speed.RawValue, out fInt2);
											FInt one;
											if (fInt2.AbsoluteValueMoreThan(DPhysicsManager.SleepVelocity.RawValue))
											{
												body2.Speed.Divide(fInt2.RawValue, out one);
											}
											else
											{
												one = FInt.HalfF;
											}
											Vector2d zero = Vector2d.zero;
											if (one.AbsoluteValueMoreThan(DPhysicsManager.MinimumCollisionOffset.RawValue))
											{
												vector2d2.Multiply(one.RawValue, out zero);
											}
											else
											{
												vector2d2.Multiply(DPhysicsManager.MinimumCollisionOffset.RawValue, out zero);
											}
											body.Offset(ref zero);
											one -= FInt.OneF;
											if (one.AbsoluteValueMoreThan(DPhysicsManager.MinimumCollisionOffset.RawValue))
											{
												vector2d2.Multiply(one.RawValue, out zero);
											}
											else
											{
												vector2d2.Multiply(DPhysicsManager.MinimumCollisionOffset.RawValue, out zero);
											}
											body2.Offset(ref zero);
										}
									}
									if (flag && flag2)
									{
										Vector2d vector2d3;
										if (!flag3)
										{
											FInt fInt3;
											Vector2d.Dot(ref body2.Velocity, ref myCollisionResult.PenetrationDirection, out fInt3);
											myCollisionResult.PenetrationDirection.Multiply(fInt3.RawValue, out vector2d3);
											vector2d3.Multiply(DPhysicsManager.Restitution.RawValue * (long)body2.Mass, out vector2d3);
											vector2d3.Invert();
										}
										else
										{
											FInt fInt3;
											Vector2d.Dot(ref body.Velocity, ref myCollisionResult.PenetrationDirection, out fInt3);
											myCollisionResult.PenetrationDirection.Multiply(fInt3.RawValue, out vector2d3);
											vector2d3.Multiply((DPhysicsManager.Restitution * body.Mass).RawValue, out vector2d3);
										}
										Vector2d vector2d4;
										if (!flag4)
										{
											FInt fInt3;
											Vector2d.Dot(ref body.Velocity, ref myCollisionResult.PenetrationDirection, out fInt3);
											myCollisionResult.PenetrationDirection.Multiply(fInt3.RawValue, out vector2d4);
											vector2d4.Multiply(DPhysicsManager.Restitution.RawValue * (long)body.Mass, out vector2d4);
											vector2d4.Invert();
										}
										else
										{
											FInt fInt3;
											Vector2d.Dot(ref body2.Velocity, ref myCollisionResult.PenetrationDirection, out fInt3);
											myCollisionResult.PenetrationDirection.Multiply(fInt3.RawValue, out vector2d4);
											vector2d4.Multiply((DPhysicsManager.Restitution * body2.Mass).RawValue, out vector2d4);
										}
										body.ApplyForce(ref vector2d4);
										body2.ApplyForce(ref vector2d3);
										vector2d3.Invert();
										vector2d4.Invert();
										body.ApplyForce(ref vector2d3);
										body2.ApplyForce(ref vector2d4);
									}
								}
							}
						}
					}
				}
			}
			Body[] simObjects = DPhysicsManager.SimObjects;
			for (int i = 0; i < simObjects.Length; i++)
			{
				Body body3 = simObjects[i];
				if (!(body3 == null))
				{
					DPhysicsManager.SimulateBody(body3);
				}
			}
			DPhysicsManager.LastSimulateTime = Time.time;
		}

		private static void SimulateBody(Body b1)
		{
			b1.ActedCount = DPhysicsManager.MaxActionsUpon;
			bool flag = false;
			if (b1.Velocity.x.AbsoluteValueMoreThan(DPhysicsManager.SleepVelocity.RawValue) || b1.Velocity.y.AbsoluteValueMoreThan(DPhysicsManager.SleepVelocity.RawValue))
			{
				Vector2d vector2d;
				b1.Velocity.Multiply(DPhysicsManager.SimulationDelta.RawValue, out vector2d);
				b1.Offset(ref vector2d);
				b1.Velocity.Multiply(DPhysicsManager.Drag.RawValue, out b1.Velocity);
				flag = true;
			}
			else
			{
				b1.Velocity = Vector2d.zero;
			}
			if (!b1.Velocity.Equals(b1.LastVelocity))
			{
				if (flag)
				{
					b1.Velocity.Magnitude(out b1.Speed);
				}
				else
				{
					b1.Speed = FInt.ZeroF;
				}
				b1.LastVelocity = b1.Velocity;
			}
			if (b1.RotationalVelocity.AbsoluteValueMoreThan(DPhysicsManager.SleepVelocity.RawValue))
			{
				Vector2d rotation;
				b1.Rotation.localright.Multiply(b1.RotationalVelocity.RawValue, out rotation);
				b1.Rotation.Add(ref rotation, out rotation);
				rotation.Normalize();
				b1.Rotation = rotation;
				b1.RotationalVelocity.Multiply(DPhysicsManager.Drag.RawValue, out b1.RotationalVelocity);
			}
			else if (flag && b1.Speed.RawValue > 0L)
			{
				b1.RotationalVelocity = FInt.ZeroF;
				Vector2d vector2d2;
				b1.Velocity.Divide(b1.Speed.RawValue, out vector2d2);
				Vector2d rotation2;
				b1.Rotation.RotateTowards(ref vector2d2, DPhysicsManager.SimulationDelta * (b1.Speed / b1.Mass), out rotation2);
				b1.Rotation = rotation2;
			}
			DPhysicsManager.CheckChange(b1);
		}

		private static void CheckChange(Body b1)
		{
			bool flag = b1.SetPosition();
			bool flag2 = b1.SetRotation();
			b1.Changed = (flag || flag2);
		}

		public static void Assimilate(Body body)
		{
			if (body.dCollider == null)
			{
				return;
			}
			ushort num;
			if (DPhysicsManager.AvailableCounts.Count > 0)
			{
				num = DPhysicsManager.AvailableCounts.Pop();
			}
			else
			{
				if ((int)DPhysicsManager.PeakCount == DPhysicsManager.SimObjects.Length)
				{
					Debug.LogError("More objects assimilated than capacity. Consider changing the MaxPhysicsObjects value in DPhysicsManager.");
					return;
				}
				num = DPhysicsManager.PeakCount;
				DPhysicsManager.PeakCount += 1;
			}
			body.SimID = num;
			DPhysicsManager.SimObjects[(int)num] = body;
			ushort simID = body.SimID;
			System.Collections.Generic.Dictionary<ushort, CollisionPair> dictionary = new System.Collections.Generic.Dictionary<ushort, CollisionPair>();
			body.MyPairs = dictionary;
			for (ushort num2 = (ushort)(simID + 1); num2 < DPhysicsManager.PeakCount; num2 += 1)
			{
				Body body2 = DPhysicsManager.SimObjects[(int)num2];
				if (!(body2 == null))
				{
					dictionary.Add(num2, new CollisionPair(body, body2));
				}
			}
			for (ushort num3 = 0; num3 < simID; num3 += 1)
			{
				Body body3 = DPhysicsManager.SimObjects[(int)num3];
				if (!(body3 == null))
				{
					body3.MyPairs.Add(simID, new CollisionPair(body3, body));
				}
			}
			Partition.AllBodies.Add(body);
		}

		public static void Dessimilate(Body body)
		{
			DPhysicsManager.SimObjects[(int)body.SimID] = null;
			DPhysicsManager.AvailableCounts.Push(body.SimID);
			ushort simID = body.SimID;
			for (ushort num = 0; num < simID; num += 1)
			{
				Body body2 = DPhysicsManager.SimObjects[(int)num];
				if (!(body2 == null))
				{
					body2.MyPairs.Remove(simID);
				}
			}
			body.MyPairs = null;
			Partition.AllBodies.Remove(body);
		}

		public static void Visualize()
		{
			Body[] simObjects = DPhysicsManager.SimObjects;
			for (int i = 0; i < simObjects.Length; i++)
			{
				Body body = simObjects[i];
				if (body != null && body.Active)
				{
					body.Visualize((Time.time - DPhysicsManager.LastSimulateTime) / Time.fixedDeltaTime);
				}
			}
		}

		public static void ApplyGlobalForce(Vector2d force)
		{
			Body[] simObjects = DPhysicsManager.SimObjects;
			for (int i = 0; i < simObjects.Length; i++)
			{
				Body body = simObjects[i];
				if (body != null)
				{
					body.ApplyForce(ref force);
				}
			}
		}

		public static void ApplyGlobalVelocity(Vector2d velocity)
		{
			Body[] simObjects = DPhysicsManager.SimObjects;
			for (int i = 0; i < simObjects.Length; i++)
			{
				Body body = simObjects[i];
				if (body != null)
				{
					body.ApplyVelocity(ref velocity);
				}
			}
		}
	}
}
