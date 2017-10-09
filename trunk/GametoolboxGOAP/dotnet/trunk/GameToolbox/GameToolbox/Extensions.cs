using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameToolbox
{
	/// <summary>
	/// Sets implementations of extension methods used by the planner.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class Extensions<T>
	{
		//TODO: make comparison delegates per-symbol rather than per-type

		private static string _tName = typeof(T).FullName;

		public static string TypeName { get { return _tName; } }

		public static Func<T, T, bool> IsLeftGreaterThanRight { get; set; }
		public static Func<T, T, bool> IsLeftEqualToRight { get; set; }
		public static Func<T, T, bool> IsLeftLessThanRight { get; set; }

		/// <summary>
		/// Should return the estimated distance between the first and second parameters, as a double value between 0.0
		/// (first and second parameter have the same value) and 1.0 (first and second parameter are completely different).
		/// </summary>
		public static Func<T, T, double> DistanceBetween { get; set; }

		public static Func<T, T> Increment { get; set; }
		public static Func<T, T> Decrement { get; set; }
	}

	internal static class Extensions
	{
		internal static bool IsGreaterThan<T>(this T t, T other)
		{
			if (Extensions<T>.IsLeftGreaterThanRight == null)
				throw new InvalidOperationException("IsLeftGreaterThanRight is not set for type " + Extensions<T>.TypeName + ".");
			return Extensions<T>.IsLeftGreaterThanRight(t, other);
		}

		internal static bool IsEqualTo<T>(this T t, T other)
		{
			if (Extensions<T>.IsLeftEqualToRight == null)
				throw new InvalidOperationException("IsLeftEqualToRight is not set for type " + Extensions<T>.TypeName + ".");
			return Extensions<T>.IsLeftEqualToRight(t, other);
		}

		public static bool IsLessThan<T>(this T t, T other)
		{
			if (Extensions<T>.IsLeftLessThanRight == null)
				throw new InvalidOperationException("IsLeftLessThanRight is not set for type " + Extensions<T>.TypeName + ".");
			return Extensions<T>.IsLeftLessThanRight(t, other);
		}

		internal static double DistanceFrom<T>(this T t, T other)
		{
			if (Extensions<T>.DistanceBetween == null)
				throw new InvalidOperationException("DistanceBetween is not set for type " + Extensions<T>.TypeName + ".");
			return Extensions<T>.DistanceBetween(t, other);
		}

		internal static T Increment<T>(this T t)
		{
			if (Extensions<T>.Increment == null)
				throw new InvalidOperationException("IncrementThis is not set for the type " + Extensions<T>.TypeName + ".");
			return Extensions<T>.Increment(t);
		}

		internal static T Decrement<T>(this T t)
		{
			if (Extensions<T>.Decrement == null)
				throw new InvalidOperationException("DecrementThis is not set for the type " + Extensions<T>.TypeName + ".");
			return Extensions<T>.Decrement(t);
		}
	}
}
