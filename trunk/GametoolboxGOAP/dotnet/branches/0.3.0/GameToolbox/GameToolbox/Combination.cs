using System;
using System.Collections.Generic;
using System.Linq;
using GameToolbox.DataStructures;

namespace GameToolbox.Combinatorics
{
	/// <summary>
	/// Represents a combination of items in a collection. Based on the combination class in this article:
	/// http://msdn.microsoft.com/en-us/magazine/cc163957.aspx
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	public class Combination<T> : IEnumerable<T>
	{
		private readonly IList<T> _collection;
		private readonly int _size = 0;
		private int[] _indices;

		/// <summary>
		/// If the collection is modified, this combination could be invalidated; a new one
		/// should be created.
		/// </summary>
		/// <param name="collection">The collection of items.</param>
		/// <param name="combinationSize">The number of items in the combination.</param>
		public Combination(IEnumerable<T> collection, int size)
			:this(collection.ToList(), size)
		{
		}

		public Combination(IList<T> collection, int size)
		{
			if (size < 0)
				throw new Exception("Negative parameter in constructor");
			_collection = collection;
			_size = size;
			_indices = new int[_size];
			for (int i = 0; i < _indices.Length; i++)
				_indices[i] = i;
		}

		/// <summary>
		/// Gets the next valid combination of items in the collecion.
		/// </summary>
		/// <returns>The next valid combination of items.</returns>
		public Combination<T> Next()
		{
			int collectionSize = _collection.Count;
			if (this._indices.Length == 0 ||
				this._indices[0] == collectionSize - _size)
				return null;

			Combination<T> next = new Combination<T>(_collection, _size);

			int i;
			for (i = 0; i < _size; i++)
				next._indices[i] = _indices[i];

			for (i = _size - 1; i > 0 && next._indices[i] == collectionSize - _size + i; i--) ;

			next._indices[i]++;

			for (int j = i; j < _size - 1; j++)
				next._indices[j + 1] = next._indices[j] + 1;

			return next;
		}

		/// <summary>
		/// Gets the item at the given index in the combination.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= _indices.Length)
					throw new ArgumentOutOfRangeException();
				return _collection[_indices[index]];
			}
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item)
		{
			foreach (T t in this)
			{
				if (item.Equals(t))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the size of the combination.
		/// </summary>
		public int Size
		{
			get { return _size; }
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			foreach (int index in _indices)
			{
				yield return _collection[index];
			}
			yield break;
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// Represents a combination of items from two collections. Based on the combination class in this article:
	/// http://msdn.microsoft.com/en-us/magazine/cc163957.aspx
	/// </summary>
	/// <typeparam name="T1">The type of items in the first collection.</typeparam>
	/// <typeparam name="T2">The type of items in the second collection.</typeparam>
	public class Combination<T1, T2> : Tuple<IEnumerable<T1>, IEnumerable<T2>>
	{
		private readonly IList<T1> _collection1;
		private readonly IList<T2> _collection2;
		private readonly int _size1;
		private readonly int _size2;
		private Combination<T1> _combination1;
		private Combination<T2> _combination2;

		/// <summary>
		/// If a collection is modified, a new combination should be created.
		/// </summary>
		/// <param name="collection1">The first collection.</param>
		/// <param name="size1">The number of items from the first collection which will be in the combination.</param>
		/// <param name="collection2">The second collection.</param>
		/// <param name="size2">The number of items from the second collection which will be in the combination.</param>
		public Combination(IEnumerable<T1> collection1, int size1, IEnumerable<T2> collection2, int size2)
			:this(collection1.ToList(), size1, collection2.ToList(), size2)
		{
		}

		/// <summary>
		/// If a collection is modified, a new combination should be created.
		/// </summary>
		/// <param name="collection1">The first collection.</param>
		/// <param name="size1">The number of items from the first collection which will be in the combination.</param>
		/// <param name="collection2">The second collection.</param>
		/// <param name="size2">The number of items from the second collection which will be in the combination.</param>
		public Combination(IList<T1> collection1, int size1, IList<T2> collection2, int size2)
		{
			_collection1 = collection1;
			_size1 = size1;
			_collection2 = collection2;
			_size2 = size2;
			_combination1 = new Combination<T1>(_collection1, _size1);
			_combination2 = new Combination<T2>(_collection2, _size2);
		}

		/// <summary>
		/// Gets the next valid combination of items in the collecion.
		/// </summary>
		/// <returns>The next valid combination of items.</returns>
		public Combination<T1, T2> Next()
		{
			Combination<T1, T2> next = new Combination<T1,T2>(_collection1, _size1, _collection2, _size2);

			next._combination2 = _combination2.Next();
			if (next._combination2 == null)
			{
				next._combination1 = _combination1.Next();
				if (next._combination1 == null)
					return null;
				next._combination2 = new Combination<T2>(_collection2, _size2);
			}

			return next;
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T1 item)
		{
			return _combination1.Contains(item);
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T2 item)
		{
			return _combination2.Contains(item);
		}

		/// <summary>
		/// Gets the items from the first collection in this combination. Setting this has no effect.
		/// </summary>
		public override IEnumerable<T1> First
		{
			get { return _combination1; }
			set { }
		}

		/// <summary>
		/// Gets the items from the second collection in this combination. Setting this has no effect.
		/// </summary>
		public override IEnumerable<T2> Second
		{
			get { return _combination2; }
			set { }
		}
	}

	/// <summary>
	/// Represents a combination of items from three collections. Based on the combination class in this article:
	/// http://msdn.microsoft.com/en-us/magazine/cc163957.aspx
	/// </summary>
	/// <typeparam name="T1">The type of items in the first collection.</typeparam>
	/// <typeparam name="T2">The type of items in the second collection.</typeparam>
	/// <typeparam name="T3">The type of items in the third collection.</typeparam>
	public class Combination<T1, T2, T3> : Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>
	{
		private readonly IList<T1> _collection1;
		private readonly IList<T2> _collection2;
		private readonly IList<T3> _collection3;
		private readonly int _size1;
		private readonly int _size2;
		private readonly int _size3;
		private Combination<T1> _combination1;
		private Combination<T2> _combination2;
		private Combination<T3> _combination3;

		/// <summary>
		/// If a collection is modified, a new combination should be created.
		/// </summary>
		/// <param name="collection1">The first collection.</param>
		/// <param name="size1">The number of items from the first collection which will be in the combination.</param>
		/// <param name="collection2">The second collection.</param>
		/// <param name="size2">The number of items from the second collection which will be in the combination.</param>
		/// <param name="collection3">The third collection.</param>
		/// <param name="size3">The number of items from the third collection which will be in the combination.</param>
		public Combination(IEnumerable<T1> collection1, int size1,
			IEnumerable<T2> collection2, int size2,
			IEnumerable<T3> collection3, int size3)
			:this(collection1.ToList(), size1, collection2.ToList(), size2, collection3.ToList(), size3)
		{
		}

		/// <summary>
		/// If a collection is modified, a new combination should be created.
		/// </summary>
		/// <param name="collection1">The first collection.</param>
		/// <param name="size1">The number of items from the first collection which will be in the combination.</param>
		/// <param name="collection2">The second collection.</param>
		/// <param name="size2">The number of items from the second collection which will be in the combination.</param>
		/// <param name="collection3">The third collection.</param>
		/// <param name="size3">The number of items from the third collection which will be in the combination.</param>
		public Combination(IList<T1> collection1, int size1,
			IList<T2> collection2, int size2,
			IList<T3> collection3, int size3)
		{
			_collection1 = collection1;
			_size1 = size1;
			_collection2 = collection2;
			_size2 = size2;
			_collection3 = collection3;
			_size3 = size3;
			_combination1 = new Combination<T1>(_collection1, _size1);
			_combination2 = new Combination<T2>(_collection2, _size2);
			_combination3 = new Combination<T3>(_collection3, _size3);
		}

		/// <summary>
		/// Gets the next valid combination of items in the collecion.
		/// </summary>
		/// <returns>The next valid combination of items.</returns>
		public Combination<T1, T2, T3> Next()
		{
			Combination<T1, T2, T3> next = new Combination<T1, T2, T3>(_collection1, _size1,
				_collection2, _size2, _collection3, _size3);

			next._combination3 = _combination3.Next();
			if (next._combination3 == null)
			{
				next._combination2 = _combination2.Next();
				if (next._combination2 == null)
				{
					next._combination1 = _combination1.Next();
					if (next._combination1 == null)
						return null;
					next._combination2 = new Combination<T2>(_collection2, _size2);
				}
				next._combination3 = new Combination<T3>(_collection3, _size3);
			}

			return next;
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T1 item)
		{
			return _combination1.Contains(item);
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T2 item)
		{
			return _combination2.Contains(item);
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T3 item)
		{
			return _combination3.Contains(item);
		}

		/// <summary>
		/// Gets the items from the first collection in this combination. Setting this has no effect.
		/// </summary>
		public override IEnumerable<T1> First
		{
			get { return _combination1; }
			set { }
		}

		/// <summary>
		/// Gets the items from the second collection in this combination. Setting this has no effect.
		/// </summary>
		public override IEnumerable<T2> Second
		{
			get { return _combination2; }
			set { }
		}

		/// <summary>
		/// Gets the items from the third collection in this combination. Setting this has no effect.
		/// </summary>
		public override IEnumerable<T3> Third
		{
			get { return _combination3; }
			set { }
		}
	}

	/// <summary>
	/// Represents a combination of items from four collections. Based on the combination class in this article:
	/// http://msdn.microsoft.com/en-us/magazine/cc163957.aspx
	/// </summary>
	/// <typeparam name="T1">The type of items in the first collection.</typeparam>
	/// <typeparam name="T2">The type of items in the second collection.</typeparam>
	/// <typeparam name="T3">The type of items in the third collection.</typeparam>
	/// <typeparam name="T4">The type of items in the fourth collection.</typeparam>
	public class Combination<T1, T2, T3, T4> : Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
	{
		private readonly IList<T1> _collection1;
		private readonly IList<T2> _collection2;
		private readonly IList<T3> _collection3;
		private readonly IList<T4> _collection4;
		private readonly int _size1;
		private readonly int _size2;
		private readonly int _size3;
		private readonly int _size4;
		private Combination<T1> _combination1;
		private Combination<T2> _combination2;
		private Combination<T3> _combination3;
		private Combination<T4> _combination4;

		/// <summary>
		/// If a collection is modified, a new combination should be created.
		/// </summary>
		/// <param name="collection1">The first collection.</param>
		/// <param name="size1">The number of items from the first collection which will be in the combination.</param>
		/// <param name="collection2">The second collection.</param>
		/// <param name="size2">The number of items from the second collection which will be in the combination.</param>
		/// <param name="collection3">The third collection.</param>
		/// <param name="size3">The number of items from the third collection which will be in the combination.</param>
		/// <param name="collection4">The fourth collection.</param>
		/// <param name="size4">The number of items from the fourth collection which will be in the combination.</param>
		public Combination(IEnumerable<T1> collection1, int size1,
			IEnumerable<T2> collection2, int size2,
			IEnumerable<T3> collection3, int size3,
			IEnumerable<T4> collection4, int size4)
			: this(collection1.ToList(), size1, collection2.ToList(), size2, collection3.ToList(), size3,
			collection4.ToList(), size4)
		{
		}

		/// <summary>
		/// If a collection is modified, a new combination should be created.
		/// </summary>
		/// <param name="collection1">The first collection.</param>
		/// <param name="size1">The number of items from the first collection which will be in the combination.</param>
		/// <param name="collection2">The second collection.</param>
		/// <param name="size2">The number of items from the second collection which will be in the combination.</param>
		/// <param name="collection3">The third collection.</param>
		/// <param name="size3">The number of items from the third collection which will be in the combination.</param>
		/// <param name="collection4">The fourth collection.</param>
		/// <param name="size4">The number of items from the fourth collection which will be in the combination.</param>
		public Combination(IList<T1> collection1, int size1,
			IList<T2> collection2, int size2,
			IList<T3> collection3, int size3,
			IList<T4> collection4, int size4)
		{
			_collection1 = collection1;
			_size1 = size1;
			_collection2 = collection2;
			_size2 = size2;
			_collection3 = collection3;
			_size3 = size3;
			_collection4 = collection4;
			_size4 = size4;
			_combination1 = new Combination<T1>(_collection1, _size1);
			_combination2 = new Combination<T2>(_collection2, _size2);
			_combination3 = new Combination<T3>(_collection3, _size3);
			_combination4 = new Combination<T4>(_collection4, _size4);
		}

		/// <summary>
		/// Gets the next valid combination of items in the collecion.
		/// </summary>
		/// <returns>The next valid combination of items.</returns>
		public Combination<T1, T2, T3, T4> Next()
		{
			Combination<T1, T2, T3, T4> next = new Combination<T1, T2, T3, T4>(_collection1, _size1,
				_collection2, _size2, _collection3, _size3, _collection4, _size4);

			next._combination4 = _combination4.Next();
			if (next._combination4 == null)
			{
				next._combination3 = _combination3.Next();
				if (next._combination3 == null)
				{
					next._combination2 = _combination2.Next();
					if (next._combination2 == null)
					{
						next._combination1 = _combination1.Next();
						if (next._combination1 == null)
							return null;
						next._combination2 = new Combination<T2>(_collection2, _size2);
					}
					next._combination3 = new Combination<T3>(_collection3, _size3);
				}
				next._combination4 = new Combination<T4>(_collection4, _size4);
			}

			return next;
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T1 item)
		{
			return _combination1.Contains(item);
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T2 item)
		{
			return _combination2.Contains(item);
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T3 item)
		{
			return _combination3.Contains(item);
		}

		/// <summary>
		/// Returns true if the combination contains the given item. Uses Equals() to check equality.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T4 item)
		{
			return _combination4.Contains(item);
		}

		/// <summary>
		/// Gets the items from the first collection in this combination. Setting this has no effect.
		/// </summary>
		public override IEnumerable<T1> First
		{
			get { return _combination1; }
			set { }
		}

		/// <summary>
		/// Gets the items from the second collection in this combination. Setting this has no effect.
		/// </summary>
		public override IEnumerable<T2> Second
		{
			get { return _combination2; }
			set { }
		}

		/// <summary>
		/// Gets the items from the third collection in this combination. Setting this has no effect.
		/// </summary>
		public override IEnumerable<T3> Third
		{
			get { return _combination3; }
			set { }
		}

		/// <summary>
		/// Gets the items from the fourth collection in this combination. Setting this has no effect.
		/// </summary>
		public override IEnumerable<T4>  Fourth
		{
			get { return _combination4; }
			set { }
		}
	}
}
