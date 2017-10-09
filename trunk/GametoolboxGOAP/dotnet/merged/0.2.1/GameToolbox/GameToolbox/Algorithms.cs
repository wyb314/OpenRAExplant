using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameToolbox
{
	/// <summary>
	/// Useful algorithms.
	/// </summary>
	public static class Algorithms
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

		/// <summary>
		/// Iterates through the given number of generations, with the given population size, yielding each generation as it goes. Uses
		/// the fitness function to determine which set of DNA survive each generation.
		/// </summary>
		/// <param name="dnaLength"></param>
		/// <param name="populationSize">Must be greater than 3.</param>
		/// <param name="generations"></param>
		/// <param name="survivors">If this is less than or equal to 0, 1 is assumed. This must be less than populationSize.</param>
		/// <param name="mutationProbability"></param>
		/// <param name="fitness">Fitness function; should return a higher value for DNA which are more fit to survive.</param>
		/// <param name="isGoalMet">Should return true if this DNA meets the goal of the genetic algorithm.</param>
		/// <returns></returns>
		public static IEnumerable<IEnumerable<DNA>> Genetic(int dnaLength, int populationSize, int generations, int survivors,
			double mutationProbability, Func<DNA, double> fitness, Func<DNA, bool> isGoalMet)
		{
			var population = new List<DNA>(populationSize);
			var populationByFitness = new PriorityQueue<DNA>((dna1, dna2) =>
				{
					double dna1Fitness = fitness(dna1);
					double dna2Fitness = fitness(dna2);
					if (dna1Fitness > dna2Fitness)
						return 1;
					if (dna1Fitness < dna2Fitness)
						return -1;
					return 0;
				});
			var mostFit = new List<DNA>(survivors);

			if (populationSize < 3)
				yield break;
			if (survivors >= populationSize)
				yield break;

			for (int i = 0; i < populationSize; i++)
				population.Add(new DNA(dnaLength));

			for (int i = 0; i < generations; i++)
			{
				populationByFitness.Clear();

				yield return population;

				foreach(var dna in population)
				{
					populationByFitness.Enqueue(dna);
					if (isGoalMet(dna))
					{
						populationByFitness.Clear();
						yield break;
					}
				}

				if (survivors > 1)
				{
					mostFit.Clear();
					for(int j = 0; j < survivors; j++)
					{
						mostFit.Add(populationByFitness.Dequeue());
					}
					population = NextGeneration(populationSize, mutationProbability, mostFit.ToArray());
				}
				else
					population = NextGeneration(populationSize, mutationProbability, populationByFitness.Dequeue());
			}

			populationByFitness.Clear();

			yield break;
		}

		private static List<DNA> NextGeneration(int populationSize, double mutationProbability, params DNA[] mostFit)
		{
			var population = new List<DNA>(populationSize);

			if (mostFit.Length == 0)
				return population;

			while (population.Count < populationSize)
			{
				foreach(var dna1 in mostFit)
				{
					if (mostFit.Length == 1)
					{
						population.Add(new DNA(dna1, mutationProbability));
						if (population.Count >= populationSize)
							break;
					}
					else
					{
						foreach (var dna2 in mostFit)
						{
							if (dna1 != dna2)
							{
								population.Add(new DNA(dna1, dna2, mutationProbability));
								if (population.Count >= populationSize)
									break;
							}
						}
					}
				}
			}

			return population;
		}

		public static T Max<T>(T left, T right)
		{
			return (left.IsGreaterThan(right)) ? left : right;
		}

		public static T Min<T>(T left, T right)
		{
			return (left.IsLessThan(right)) ? left : right;
		}
	}
}
