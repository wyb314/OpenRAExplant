using System;
using System.Collections.Generic;
using System.Linq;

namespace GameToolbox
{
	/// <summary>
	/// Provides the basic functionality of a deck of cards.
	/// </summary>
	/// <typeparam name="T">The type of object this is a deck of.</typeparam>
	[Serializable]
	public class Deck<T>
	{
		private Random _random = new Random();
		private LinkedList<T> _deck = new LinkedList<T>();
		private LinkedList<T> _drawn = new LinkedList<T>();

		/// <summary>
		/// The contents of the deck. When getting, this provides the contents of the deck which haven't yet beend drawn.
		/// When setting, this replaces the entire previous deck with the new deck, with none of the cards drawn and with
		/// all the undrawn cards in the same order as the collection given.
		/// </summary>
		public IEnumerable<T> Contents
		{
			get
			{
				foreach (T card in _deck)
					yield return card;
				yield break;
			}
			set
			{
				_deck.Clear();
				_drawn.Clear();
				foreach (T card in value)
				{
					_deck.AddLast(card);
				}
			}
		}

		/// <summary>
		/// Gets the top card of the deck, without drawing it.
		/// </summary>
		public T Top
		{
			get
			{
				if (_deck.Count > 0)
				{
					return _deck.First.Value;
				}
				return default(T);
			}
		}

		/// <summary>
		/// Gets the bottom card of the deck, without drawing it.
		/// </summary>
		public T Bottom
		{
			get
			{
				if (_deck.Count > 0)
				{
					return _deck.Last.Value;
				}
				return default(T);
			}
		}

		/// <summary>
		/// The number of undrawn cards still in the deck.
		/// </summary>
		public int Count { get { return _deck.Count; } }

		/// <summary>
		/// The total number of cards in the deck, including both drawn and undrawn cards.
		/// </summary>
		public int Total { get { return _deck.Count + _drawn.Count; } }

		/// <summary>
		/// Draws a card from the top of the deck. If the deck is shuffled, the card will go back into the deck.
		/// </summary>
		/// <returns>The card drawn.</returns>
		public T DrawTop()
		{
			LinkedListNode<T> draw = _deck.First;
			_deck.Remove(draw);
			_drawn.AddLast(draw);
			return draw.Value;
		}

		/// <summary>
		/// Draws a card from the bottom of the deck. If the deck is shuffled, the card will go back into the deck.
		/// </summary>
		/// <returns>The card drawn.</returns>
		public T DrawBottom()
		{
			LinkedListNode<T> draw = _deck.Last;
			_deck.Remove(draw);
			_drawn.AddLast(draw);
			return draw.Value;
		}

		/// <summary>
		/// Attempts to draw the given card from the deck. If the card is successfully drawn, and then the deck is shuffled,
		/// the card will return to the deck. This method is slower than DrawTop and DrawBottom, since it must search the
		/// deck for the correct card. Note that if there are multiple cards with the same value, the first matching card is
		/// drawn.
		/// </summary>
		/// <param name="card">The card to draw.</param>
		/// <returns>True if the card was successfully drawn, otherwise false.</returns>
		public bool Draw(T card)
		{
			LinkedListNode<T> draw = _deck.Find(card);
			if (draw == null)
				return false;
			_deck.Remove(draw);
			_drawn.AddLast(draw);
			return true;
		}

		/// <summary>
		/// Adds a card to the top of the deck.
		/// </summary>
		/// <param name="card">The card to add to the top of the deck.</param>
		public void AddTop(T card)
		{
			_deck.AddFirst(card);
		}

		/// <summary>
		/// Adds a card to the bottom of the deck.
		/// </summary>
		/// <param name="card">The card to add to the bottom of the deck.</param>
		public void AddBottom(T card)
		{
			_deck.AddLast(card);
		}

		/// <summary>
		/// Removes the top card from the deck. If the deck is shuffled, the card will not go back into the deck.
		/// </summary>
		/// <returns>The card removed from the top of the deck.</returns>
		public T RemoveTop()
		{
			LinkedListNode<T> removed = _deck.First;
			_deck.Remove(removed);
			return removed.Value;
		}

		/// <summary>
		/// Removes the given card from the deck, whether it has already been drawn or not. This method is slower than
		/// RemoveTop and RemoveBottom, because it must search for an instance of the card in the deck. If the deck
		/// is shuffled, the card will not go back into the deck.
		/// </summary>
		/// <param name="card">The card to remove from the deck.</param>
		public void Remove(T card)
		{
			if (!_deck.Remove(card))
				_drawn.Remove(card);
		}

		/// <summary>
		/// Removes the bottom card from the deck. If the deck is shuffled, the card will not go back into the deck.
		/// </summary>
		/// <returns>The card removed from the bottom of the deck.</returns>
		public T RemoveBottom()
		{
			LinkedListNode<T> removed = _deck.Last;
			_deck.Remove(removed);
			return removed.Value;
		}

		/// <summary>
		/// Gets the top few cards of the deck, without drawing any of them.
		/// </summary>
		/// <param name="number">The number of cards to get.</param>
		/// <returns>An enumerable of the top few cards.</returns>
		public IEnumerable<T> TopFew(int number)
		{
			LinkedListNode<T> current = _deck.First;
			for (int i = 0; (i < number) && (i < Count); i++)
			{
				yield return current.Value;
				current = current.Next;
			}
			yield break;
		}

		/// <summary>
		/// Gets the bottom few cards of the deck, without drawing any of them.
		/// </summary>
		/// <param name="number">The number of cards to get.</param>
		/// <returns>An enumerable of the bottom few cards.</returns>
		public IEnumerable<T> BottomFew(int number)
		{
			LinkedListNode<T> current = _deck.Last;
			for (int i = 0; (i < number) && (i < Count); i++)
			{
				yield return current.Value;
				current = current.Previous;
			}
			yield break;
		}

		/// <summary>
		/// Shuffles the deck by putting all drawn cards back into it and randomizing the order of the cards.
		/// </summary>
		public void Shuffle()
		{
			if (_deck.Count + _drawn.Count == 0)
				return;

			while (Count > 0)
				DrawTop();

			var shuffled = _drawn.OrderBy((card) => { return _random.Next(); });

			foreach (T card in shuffled)
			{
				_deck.AddLast(card);
			}

			_drawn.Clear();
		}

		/// <summary>
		/// Shuffles the remaining undrawn cards in the deck, without returning the drawn cards to it.
		/// </summary>
		public void ShuffleRemaining()
		{
			if (_deck.Count == 0)
				return;

			var shuffled = _deck.OrderBy((card) => { return _random.Next(); }).ToList();

			_deck.Clear();

			foreach (T card in shuffled)
			{
				_deck.AddLast(card);
			}
		}
	}
}
