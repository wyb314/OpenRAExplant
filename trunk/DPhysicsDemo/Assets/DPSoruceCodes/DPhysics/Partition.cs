using System;
using System.Collections.Generic;

namespace DPhysics
{
	public static class Partition
	{
		private const int MaxContained = 16;

		private const int DefiniteMaxDepth = 7;

		private static int MaxDepth;

		private static FInt MinimumPartitionHalfLength = FInt.Create(2);

		public static HashSet<Body> AllBodies = new HashSet<Body>();

		public static void StartPartitioning()
		{
			long xMin;
			long xMax;
			long yMin;
			long yMax;
			Partition.GenerateBounds(Partition.AllBodies, out xMin, out xMax, out yMin, out yMax);
			Partition.NewPartition(0, xMin, xMax, yMin, yMax, Partition.AllBodies);
		}

		public static void NewPartition(int depth, long xMin, long xMax, long yMin, long yMax, HashSet<Body> ContainedBodies)
		{
			if (ContainedBodies.Count <= 16)
			{
				Partition.Establish(ContainedBodies);
				return;
			}
			if (depth >= Partition.MaxDepth)
			{
				Partition.Establish(ContainedBodies);
				return;
			}
			long num;
			long num2;
			Partition.GetSplitPoint(xMin, xMax, yMin, yMax, out num, out num2);
			if (xMax - num > Partition.MinimumPartitionHalfLength.RawValue || yMax - num2 > Partition.MinimumPartitionHalfLength.RawValue)
			{
				int depth2 = depth + 1;
				for (int i = 0; i < 4; i++)
				{
					bool flag = i == 0 || i == 3;
					bool flag2 = i == 0 || i == 1;
					long yMax2;
					long yMin2;
					if (flag)
					{
						yMax2 = yMax;
						yMin2 = num2;
					}
					else
					{
						yMax2 = num2;
						yMin2 = yMin;
					}
					long xMax2;
					long xMin2;
					if (flag2)
					{
						xMax2 = xMax;
						xMin2 = num;
					}
					else
					{
						xMax2 = num;
						xMin2 = xMin;
					}
					HashSet<Body> hashSet = new HashSet<Body>();
					foreach (Body current in ContainedBodies)
					{
						if (current.Active)
						{
							if (flag)
							{
								if (current.dCollider.MyBounds.yMax < num2)
								{
									continue;
								}
							}
							else if (current.dCollider.MyBounds.yMin >= num2)
							{
								continue;
							}
							if (flag2)
							{
								if (current.dCollider.MyBounds.xMax < num)
								{
									continue;
								}
							}
							else if (current.dCollider.MyBounds.xMin >= num)
							{
								continue;
							}
							hashSet.Add(current);
						}
					}
					Partition.NewPartition(depth2, xMin2, xMax2, yMin2, yMax2, hashSet);
				}
				return;
			}
			Partition.Establish(ContainedBodies);
		}

		private static void Establish(HashSet<Body> ContainedBodies)
		{
			if (ContainedBodies.Count >= 2)
			{
				ushort num = 0;
				foreach (Body current in ContainedBodies)
				{
					ushort num2 = 0;
					foreach (Body current2 in ContainedBodies)
					{
						if (num2 > num)
						{
							if (current.SimID < current2.SimID)
							{
								current.MyPairs[current2.SimID].SamePartition = true;
							}
							else
							{
								current2.MyPairs[current.SimID].SamePartition = true;
							}
						}
						num2 += 1;
					}
					num += 1;
				}
			}
		}

		private static void GenerateBounds(HashSet<Body> ContainedBodies, out long xMin, out long xMax, out long yMin, out long yMax)
		{
			bool flag = true;
			xMin = 0L;
			xMax = 0L;
			yMin = 0L;
			yMax = 0L;
			foreach (Body current in ContainedBodies)
			{
				if (flag)
				{
					flag = false;
					xMax = current.dCollider.MyBounds.xMax;
					xMin = current.dCollider.MyBounds.xMin;
					yMax = current.dCollider.MyBounds.yMax;
					yMin = current.dCollider.MyBounds.yMin;
				}
				if (current.dCollider.MyBounds.xMin < xMin)
				{
					xMin = current.dCollider.MyBounds.xMin;
				}
				else if (current.dCollider.MyBounds.xMax > xMax)
				{
					xMax = current.dCollider.MyBounds.xMax;
				}
				if (current.dCollider.MyBounds.yMin < yMin)
				{
					yMin = current.dCollider.MyBounds.yMin;
				}
				else if (current.dCollider.MyBounds.yMax > yMax)
				{
					yMax = current.dCollider.MyBounds.yMax;
				}
			}
			long num = yMax - yMin + (xMax - xMin);
			Partition.MaxDepth = (int)(num >> 25);
			Partition.MaxDepth++;
			if (Partition.MaxDepth > 7)
			{
				Partition.MaxDepth = 7;
			}
		}

		private static void GetSplitPoint(long xMin, long xMax, long yMin, long yMax, out long xSplit, out long ySplit)
		{
			xSplit = (xMin + xMax) / 2L;
			ySplit = (yMin + yMax) / 2L;
		}
	}
}
