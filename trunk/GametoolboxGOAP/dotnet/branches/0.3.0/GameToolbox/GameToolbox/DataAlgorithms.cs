using System;
using System.Collections.Generic;
using GameToolbox.DataStructures;

namespace GameToolbox.Algorithms
{
	/// <summary>
	/// Useful algorithms.
	/// </summary>
	public static class DataAlgorithms
	{
		/// <summary>
		/// Performs a heapsort on the given enumerable list of items, using the given priority comparison for sorting.
		/// </summary>
		/// <typeparam name="T">The type of item to sort.</typeparam>
		/// <param name="list">The list of items to sort.</param>
		/// <param name="priorityComparison">The priority comparison which will yield the desired sort order.</param>
		/// <returns>A sorted IEnumerable of the items.</returns>
		public static IEnumerable<T> Heapsort<T>(IEnumerable<T> list, Comparison<T> priorityComparison)
		{
			List<T> sorted = new List<T>();
			PriorityQueue<T> heap = new PriorityQueue<T>(priorityComparison);

			foreach (T item in list)
			{
				heap.Enqueue(item);
			}

			while (heap.Count > 0)
			{
				sorted.Add(heap.Dequeue());
			}

			foreach (T item in sorted)
			{
				yield return item;
			}

			yield break;
		}
	}
}
