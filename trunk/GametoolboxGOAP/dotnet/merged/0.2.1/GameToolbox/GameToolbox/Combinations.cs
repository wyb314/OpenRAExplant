using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameToolbox
{
	public static class Combinations
	{
		#region 1-collection combinations

		/// <summary>
		/// Returns the number of valid combinations of this size in the collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The collection to get the number of combinations for.</param>
		/// <param name="size">The number of items in each combination.</param>
		/// <returns>The total number of possible combinations of the given number of items in the collection.</returns>
		public static int Count<T>(IEnumerable<T> collection, int size)
		{
			if (size < 0)
				throw new Exception("Invalid negative parameter in Count");
			int collectionSize = collection.Count();
			if (collectionSize < size) return 0;
			if (collectionSize == size) return 1;
			if (size == 1) return collectionSize;

			int delta, max;

			if (size < collectionSize - size)
			{
				delta = collectionSize - size;
				max = size;
			}
			else
			{
				delta = size;
				max = collectionSize - size;
			}

			int result = delta + 1;

			for (int i = 2; i <= max; ++i)
			{
				checked { result = (result * (delta + i)) / i; }
			}

			return result;
		}

		/// <summary>
		/// Iterates through all combinations of the given number of items from the collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The collection of items.</param>
		/// <param name="size">The number of items in each combination.</param>
		/// <returns></returns>
		public static IEnumerable<Combination<T>> All<T>(IEnumerable<T> collection, int size)
		{
			return All(collection.ToList(), size);
		}

		/// <summary>
		/// Iterates through all combinations of the given number of items from the collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The collection of items.</param>
		/// <param name="size">The number of items in each combination.</param>
		/// <returns></returns>
		public static IEnumerable<Combination<T>> All<T>(IList<T> collection, int size)
		{
			Combination<T> current = new Combination<T>(collection, size);
			while (current != null)
			{
				yield return current;
			}
			yield break;
		}

		#endregion

		#region 2-collection combinations

		/// <summary>
		/// Returns the number of valid combinations of the given number of elements from each collection.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="collection1">The first collection.</param>
		/// <param name="size1">The number of items from the first collection in each combination.</param>
		/// <param name="collection2">The second collection.</param>
		/// <param name="size2">The number of items from the second collection in each combination.</param>
		/// <returns></returns>
		public static int Count<T1, T2>(IEnumerable<T1> collection1, int size1, IEnumerable<T2> collection2, int size2)
		{
			return Count(collection1, size1) * Count(collection2, size2);
		}

		/// <summary>
		/// Iterates through all combinations of the given number of items from the collections.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="collection1">The first collection of items.</param>
		/// <param name="size1">The number of items from the first collection in each combination.</param>
		/// <param name="collection2">The second collection of items.</param>
		/// <param name="size2">The number of items from the second collection in each combination.</param>
		/// <returns></returns>
		public static IEnumerable<Combination<T1, T2>> All<T1, T2>(IEnumerable<T1> collection1, int size1,
			IEnumerable<T2> collection2, int size2)
		{
			return All(collection1.ToList(), size1, collection2.ToList(), size2);
		}

		/// <summary>
		/// Iterates through all combinations of the given number of items from the collection.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="collection1">The first collection of items.</param>
		/// <param name="size1">The number of items from the first collection in each combination.</param>
		/// <param name="collection2">The second collection of items.</param>
		/// <param name="size2">The number of items from the second collection in each combination.</param>
		/// <returns></returns>
		public static IEnumerable<Combination<T1, T2>> All<T1, T2>(IList<T1> collection1, int size1,
			IList<T2> collection2, int size2)
		{
			Combination<T1, T2> current = new Combination<T1, T2>(collection1, size1, collection2, size2);
			while (current != null)
			{
				yield return current;
			}
			yield break;
		}

		#endregion

		#region 3-collection combinations

		/// <summary>
		/// Returns the number of valid combinations of the given number of elements from each collection.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <param name="collection1">The first collection.</param>
		/// <param name="size1">The number of items from the first collection in each combination.</param>
		/// <param name="collection2">The second collection.</param>
		/// <param name="size2">The number of items from the second collection in each combination.</param>
		/// <param name="collection3">The third collection.</param>
		/// <param name="size3">The number of items from the third collection in each combination.</param>
		/// <returns></returns>
		public static int Count<T1, T2, T3>(IEnumerable<T1> collection1, int size1, IEnumerable<T2> collection2, int size2,
			IEnumerable<T3> collection3, int size3)
		{
			return Count(collection1, size1) * Count(collection2, size2) * Count(collection3, size3);
		}

		/// <summary>
		/// Iterates through all combinations of the given number of items from each collection.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <param name="collection1">The first collection of items.</param>
		/// <param name="size1">The number of items from the first collection in each combination.</param>
		/// <param name="collection2">The second collection of items.</param>
		/// <param name="size2">The number of items from the second collection in each combination.</param>
		/// <param name="collection3">The third collection of items.</param>
		/// <param name="size3">The number of items from the third collection in each combination.</param>
		/// <returns></returns>
		public static IEnumerable<Combination<T1, T2, T3>> All<T1, T2, T3>(IEnumerable<T1> collection1, int size1,
			IEnumerable<T2> collection2, int size2,
			IEnumerable<T3> collection3, int size3)
		{
			return All(collection1.ToList(), size1, collection2.ToList(), size2, collection3.ToList(), size3);
		}

		/// <summary>
		/// Iterates through all combinations of the given number of items from each collection.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <param name="collection1">The first collection of items.</param>
		/// <param name="size1">The number of items from the first collection in each combination.</param>
		/// <param name="collection2">The second collection of items.</param>
		/// <param name="size2">The number of items from the second collection in each combination.</param>
		/// <param name="collection3">The third collection of items.</param>
		/// <param name="size3">The number of items from the third collection in each combination.</param>
		/// <returns></returns>
		public static IEnumerable<Combination<T1, T2, T3>> All<T1, T2, T3>(IList<T1> collection1, int size1,
			IList<T2> collection2, int size2,
			IList<T3> collection3, int size3)
		{
			Combination<T1, T2, T3> current = new Combination<T1, T2, T3>(collection1, size1, collection2, size2,
				collection3, size3);
			while (current != null)
			{
				yield return current;
			}
			yield break;
		}

		#endregion

		#region 4-collection combinations

		/// <summary>
		/// Returns the number of valid combinations of the given number of elements from each collection.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <param name="collection1">The first collection of items.</param>
		/// <param name="size1">The number of items from the first collection in each combination.</param>
		/// <param name="collection2">The second collection of items.</param>
		/// <param name="size2">The number of items from the second collection in each combination.</param>
		/// <param name="collection3">The third collection of items.</param>
		/// <param name="size3">The number of items from the third collection in each combination.</param>
		/// <param name="collection4">The fourth collection of items.</param>
		/// <param name="size4">The number of items from the fourth collection in each combination.</param>
		/// <returns></returns>
		public static int Count<T1, T2, T3, T4>(IEnumerable<T1> collection1, int size1, IEnumerable<T2> collection2, int size2,
			IEnumerable<T3> collection3, int size3, IEnumerable<T4> collection4, int size4)
		{
			return Count(collection1, size1) * Count(collection2, size2) * Count(collection3, size3) * Count(collection4, size4);
		}

		/// <summary>
		/// Iterates through all combinations of the given number of items from each collection.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <param name="collection1">The first collection of items.</param>
		/// <param name="size1">The number of items from the first collection in each combination.</param>
		/// <param name="collection2">The second collection of items.</param>
		/// <param name="size2">The number of items from the second collection in each combination.</param>
		/// <param name="collection3">The third collection of items.</param>
		/// <param name="size3">The number of items from the third collection in each combination.</param>
		/// <param name="collection4">The fourth collection of items.</param>
		/// <param name="size4">The number of items from the fourth collection in each combination.</param>
		/// <returns></returns>
		public static IEnumerable<Combination<T1, T2, T3, T4>> All<T1, T2, T3, T4>(IEnumerable<T1> collection1, int size1,
			IEnumerable<T2> collection2, int size2,
			IEnumerable<T3> collection3, int size3,
			IEnumerable<T4> collection4, int size4)
		{
			return All(collection1.ToList(), size1, collection2.ToList(), size2, collection3.ToList(), size3, collection4.ToList(), size4);
		}

		/// <summary>
		/// Iterates through all combinations of the given number of items from each collection.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <param name="collection1">The first collection of items.</param>
		/// <param name="size1">The number of items from the first collection in each combination.</param>
		/// <param name="collection2">The second collection of items.</param>
		/// <param name="size2">The number of items from the second collection in each combination.</param>
		/// <param name="collection3">The third collection of items.</param>
		/// <param name="size3">The number of items from the third collection in each combination.</param>
		/// <param name="collection4">The fourth collection of items.</param>
		/// <param name="size4">The number of items from the fourth collection in each combination.</param>
		/// <returns></returns>
		public static IEnumerable<Combination<T1, T2, T3, T4>> All<T1, T2, T3, T4>(IList<T1> collection1, int size1,
			IList<T2> collection2, int size2,
			IList<T3> collection3, int size3,
			IList<T4> collection4, int size4)
		{
			Combination<T1, T2, T3, T4> current = new Combination<T1, T2, T3, T4>(collection1, size1, collection2, size2,
				collection3, size3, collection4, size4);
			while (current != null)
			{
				yield return current;
			}
			yield break;
		}

		#endregion
	}
}
