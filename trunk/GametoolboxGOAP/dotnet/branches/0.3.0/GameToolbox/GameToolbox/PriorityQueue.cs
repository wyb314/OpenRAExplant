using System;
using System.Collections.Generic;

namespace GameToolbox.DataStructures
{
	/// <summary>
	/// A generic priority queue, backed by a generic List.
	/// </summary>
	/// <typeparam name="T">The type of value to be stored in the priority queue.</typeparam>
	[Serializable]
	public class PriorityQueue<T>
	{
		private List<Item> _heap = new List<Item>();
		private Comparison<T> _comparison;

		/// <summary>
		/// The number of items in the priority queue.
		/// </summary>
		public int Count { get { return _heap.Count; } }

		/// <summary>
		/// Gets or sets the max number of items allowed in the priority queue. A value of 0 indicates
		/// no maximum.
		/// </summary>
		public int MaxItems { get; set; }

		/// <summary>
		/// Constructor. Requires a priority comparison, which must return values according to the description
		/// of the Comparison generic delegate.
		/// </summary>
		/// <param name="priorityComparison">The comparison used to prioritize the queue.</param>
		public PriorityQueue(Comparison<T> priorityComparison)
		{
			_comparison = priorityComparison;
			MaxItems = 0;
		}

		/// <summary>
		/// Adds a value to the queue.
		/// </summary>
		/// <param name="value">The value to add to the queue.</param>
		/// <returns>The item added. This must be preserved if the priority is to be recalculated.</returns>
		public Item Enqueue(T value)
		{
			Item newItem = new Item { Value = value, Index = Count };
			_heap.Add(newItem);
			BubbleUp(newItem.Index);

			if (MaxItems > 0)
			{
				while (Count > MaxItems)
				{
					_heap.RemoveAt(Count - 1);
				}
			}

			return newItem;
		}

		/// <summary>
		/// Gets the highest-priority value and removes it from the queue.
		/// </summary>
		/// <returns>The value with the highest priority in the queue.</returns>
		public T Dequeue()
		{
			if (Count == 0)
				return default(T);

			T result = _heap[0].Value;
			Swap(0, Count - 1);
			_heap.RemoveAt(Count - 1);
			TrickleDown(0);

			return result;
		}

		/// <summary>
		/// Gets the highest-priority value without removing it from the queue.
		/// </summary>
		/// <returns>The value with the highest priority in the queue.</returns>
		public T Peek()
		{
			if (Count == 0)
				return default(T);

			return _heap[0].Value;
		}

		/// <summary>
		/// Returns true if there is an item in the priority queue which matches the given value. This is a O(n) operation,
		/// where n is the number of items in the priority queue.
		/// </summary>
		/// <param name="value">The value to find.</param>
		/// <returns>True if the value is found in the priority queue, otherwise false.</returns>
		public bool Contains(T value)
		{
			return _heap.Exists((item) => { return item.Value.Equals(value); });
		}

		/// <summary>
		/// Recalculates the priority of the given item in the priority queue.
		/// </summary>
		/// <param name="item">The item to recalculate priority of.</param>
		/// <returns>True if the priority was recalculated, otherwise false.</returns>
		public bool RecalculatePriority(Item item)
		{
			if (item.Index >= Count || _heap[item.Index] != item)
				return false;

			//bubble up will swap if priority has increased
			if (!BubbleUp(item.Index))
				TrickleDown(item.Index); //if bubble up didn't swap, priority decreased, so trickle down

			return true;
		}

		/// <summary>
		/// Clears the priority queue.
		/// </summary>
		public void Clear()
		{
			_heap.Clear();
		}

		/// <summary>
		/// Bubbles up the value at the given index in the priority queue until it is at a position which
		/// preserves the heap rules.
		/// </summary>
		/// <param name="index">The index to bubble up.</param>
		/// <returns>True if items were swapped in the heap, otherwise false.</returns>
		private bool BubbleUp(int index)
		{
			int parent = Parent(index);
			bool swapped = false;

			while ((index > 0) && (_comparison(_heap[parent].Value, _heap[index].Value) < 0))
			{
				Swap(index, parent);
				swapped = true;

				index = parent;
				parent = Parent(index);
			}

			return swapped;
		}

		/// <summary>
		/// Trickles down the value at the given index in the priority queue until it is at a position which
		/// preserves the heap rules.
		/// </summary>
		/// <param name="index">The index to trickle down.</param>
		/// <returns>True if items were swapped in the heap, othersiwe false.</returns>
		private bool TrickleDown(int index)
		{
			int leftChild = LeftChild(index);
			int rightChild = RightChild(index);
			int greaterChild = leftChild;
			bool swapped = false;

			while (leftChild < Count)
			{
				if ((rightChild < Count) && (_comparison(_heap[leftChild].Value, _heap[rightChild].Value) < 0))
					greaterChild = rightChild;
				if (_comparison(_heap[index].Value, _heap[greaterChild].Value) > 0)
					break;

				Swap(index, greaterChild);
				swapped = true;

				index = greaterChild;
				leftChild = LeftChild(index);
				rightChild = RightChild(index);
				greaterChild = leftChild;
			}

			return swapped;
		}

		/// <summary>
		/// Gets the parent index of the given index.
		/// </summary>
		/// <param name="index">The index to get the parent index of.</param>
		/// <returns>The parent index of the given index.</returns>
		private int Parent(int index)
		{
			return (index - 1) / 2;
		}

		/// <summary>
		/// Gets the index of the left child for the given index.
		/// </summary>
		/// <param name="index">The index to get the left child index of.</param>
		/// <returns>The left child index.</returns>
		private int LeftChild(int index)
		{
			return index * 2 + 1;
		}

		/// <summary>
		/// Gets the index of the right child for the given index.
		/// </summary>
		/// <param name="index">The index to get the right child index of.</param>
		/// <returns>The right child index.</returns>
		private int RightChild(int index)
		{
			return LeftChild(index) + 1;
		}

		/// <summary>
		/// Swaps the values in the heap at the two indices.
		/// </summary>
		/// <param name="index1">The index of the first value to swap.</param>
		/// <param name="index2">The index of the second value to swap.</param>
		private void Swap(int index1, int index2)
		{
			Item temp = _heap[index1];
			_heap[index1] = _heap[index2];
			_heap[index2] = temp;

			_heap[index1].Index = index1;
			_heap[index2].Index = index2;
		}

		/// <summary>
		/// Represents an item in the priority queue.
		/// </summary>
		[Serializable]
		public class Item
		{
			internal int Index { get; set; }
			public T Value { get; set; }
		}
	}
}
