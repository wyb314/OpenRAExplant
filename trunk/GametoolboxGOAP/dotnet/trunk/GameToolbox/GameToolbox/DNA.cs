using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameToolbox
{
	/// <summary>
	/// Class representing DNA for a genetic algorithm; a string of bytes, with helpers to make it random and to handle
	/// reproduction. The onus is on the user to interpret the bytes in the desired manner.
	/// </summary>
	[Serializable]
	public class DNA
	{
		private static Random _random = new Random();
		private byte[] _genes;

		/// <summary>
		/// The mutation probability used for DNA created using the + operator.
		/// </summary>
		public static double DefaultMutationProbability { get; set; }

		public DNA(int length)
		{
			_genes = new byte[length];
			_random.NextBytes(_genes);
		}

		public DNA(DNA parent)
			:this(parent, DefaultMutationProbability)
		{
		}

		public DNA(DNA parent, double mutationProbability)
		{
			_genes = (byte[])parent._genes.Clone();
			for (int i = 0; i < _genes.Length; i++)
			{
				if (_random.NextDouble() <= mutationProbability)
				{
					_genes[i] = (byte)_random.Next(0, 256);
				}
			}
		}

		/// <summary>
		/// Note that the newly constructed DNA will have the same length as parent1.
		/// </summary>
		/// <param name="parent1"></param>
		/// <param name="parent2"></param>
		public DNA(DNA parent1, DNA parent2)
			:this(parent1, parent2, DefaultMutationProbability)
		{
		}

		/// <summary>
		/// Note that the newly constructed DNA will have the same length as parent1.
		/// </summary>
		/// <param name="parent1"></param>
		/// <param name="parent2"></param>
		/// <param name="mutationProbability"></param>
		public DNA(DNA parent1, DNA parent2, double mutationProbability)
		{
			_genes = (byte[])parent1._genes.Clone();
			for (int i = 0; i < _genes.Length; i++)
			{
				if (_random.Next(0, 2) == 1)
					_genes[i] = parent2[i];
				if (_random.NextDouble() <= mutationProbability)
				{
					_genes[i] = (byte)_random.Next(0, 256);
				}
			}
		}

		public byte this[int gene]
		{
			get
			{
				return _genes[gene];
			}
		}

		public int Length
		{
			get
			{
				return _genes.Length;
			}
		}

		/// <summary>
		/// Note that the resulting DNA will have the same length as the left operand.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static DNA operator +(DNA left, DNA right)
		{
			return new DNA(left, right);
		}
	}
}
