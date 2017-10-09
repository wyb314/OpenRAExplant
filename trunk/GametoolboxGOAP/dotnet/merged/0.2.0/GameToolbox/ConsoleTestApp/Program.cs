using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameToolbox;

namespace ConsoleTestApp
{
	class Program
	{
		//TODO: figure out why planner is eating the ice cream (suboptimal plan)

		static void Main(string[] args)
		{
			var tarotDeck = GenTarotDeck();
			var playingDeck = GenPlayingDeck();
			var ints = GenTestInts();
			Graph<char, int> testGraph;
			Dictionary<char, int> graphIds;
			AStar<char, int> aStar;
			GenTestGraph(out testGraph, out graphIds, out aStar);
			ConsoleKeyInfo pressedKey;
			var actions = InitializeGOAP();

			do
			{
				DisplayChoices();
				pressedKey = Console.ReadKey(true);
				switch (pressedKey.Key)
				{
					case ConsoleKey.Escape: break;
					case ConsoleKey.NumPad1:
					case ConsoleKey.D1: Console.Clear();
						TestDeck(playingDeck);
						break;
					case ConsoleKey.NumPad2:
					case ConsoleKey.D2: Console.Clear();
						TestDeck(tarotDeck);
						break;
					case ConsoleKey.NumPad3:
					case ConsoleKey.D3: Console.Clear();
						TestHeapsort(ints);
						break;
					case ConsoleKey.NumPad4:
					case ConsoleKey.D4: Console.Clear();
						TestAStar(testGraph, graphIds, aStar);
						break;
					case ConsoleKey.NumPad5:
					case ConsoleKey.D5: Console.Clear();
						TestGOAP(actions);
						break;
					case ConsoleKey.NumPad6:
					case ConsoleKey.D6: Console.Clear();
						TestGenetic();
						break;
					default: continue;
				}
				if ((pressedKey.Key != ConsoleKey.Escape)
					&& (pressedKey.KeyChar != '1' && pressedKey.KeyChar != '2'))
					GetAnyKey();
			} while (pressedKey.Key != ConsoleKey.Escape);
		}

		private static void DisplayChoices()
		{
			Console.Clear();
			Console.WriteLine("Please choose by pressing the key for the desired selection.");
			Console.WriteLine("1    Test Playing Card Deck");
			Console.WriteLine("2    Test Tarot Card Deck");
			Console.WriteLine("3    Test Heapsort Algorithm");
			Console.WriteLine("4    Test A* Algorithm");
			Console.WriteLine("5    Test Goal-Oriented Action Planner");
			Console.WriteLine("6    Test Genetic Algorithm");
			Console.WriteLine("Esc  Quit");
			Console.Write("Selection: ");
		}

		private static void TestGenetic()
		{
			DNA.DefaultMutationProbability = 0.1;
			DNA test1 = new DNA(5);
			DNA test2 = new DNA(5);

			Console.Clear();
			Console.WriteLine("Test DNA 1:");
			PrintTestDNA(test1);
			Console.WriteLine();
			Console.WriteLine("Test DNA 2:");
			PrintTestDNA(test2);

			Console.WriteLine(Environment.NewLine);
			Console.WriteLine("Child 1 of DNA 1 & DNA 2:");
			PrintTestDNA(new DNA(test1, test2));
			Console.WriteLine();
			Console.WriteLine("Child 2 of DNA 1 & DNA 2:");
			PrintTestDNA(new DNA(test1, test2));

			Console.WriteLine("Press a key to continue.");
			Console.ReadKey(true);
			Console.Clear();
			Console.WriteLine("Genetic algorithm to find x, y, z, and w such that x^2 + 2y/z + w = 1:");

			Func<DNA, double> fitness = (dna) =>
				{
					double x = ((double)dna[0] + (double)dna[1] - 256) / 128;
					double y = ((double)dna[2] + (double)dna[3] - 256) / 128;
					double z = ((double)dna[4] + (double)dna[5] - 256) / 128;
					double w = ((double)dna[6] + (double)dna[7] - 256) / 128;
					double res = x * x + (2 * y) / z + w;
					return -Math.Abs(res - 1);
				};
			Func<DNA, bool> isGoalMet = (dna) =>
				{
					return Math.Round(fitness(dna), 3) == 0;
				};
			int maxGenerations = 1000;
			int populationSize = 100;
			var generations = Algorithms.Genetic(8, populationSize, maxGenerations, 2, 0.1, fitness, isGoalMet);
			int actualGenerations = generations.Count();
			Console.WriteLine("Last of {0} generations, population size {1}:", actualGenerations, populationSize);
			DNA bestDNA = null;
			double bestFitness = int.MinValue;
			double currentFitness;
			foreach (var dna in generations.Last())
			{
				PrintFormulaDNA(dna);

				currentFitness = fitness(dna);
				if (currentFitness > bestFitness)
				{
					bestFitness = currentFitness;
					bestDNA = dna;
				}
			}
			Console.WriteLine("Press a key to continue.");
			Console.ReadKey(true);
			Console.Clear();
			Console.WriteLine("Genetic algorithm to find x, y, z, and w such that x^2 + 2y/z + w = 1:");
			Console.WriteLine("Best match after {0} generations, population size {1}:", actualGenerations, populationSize);
			PrintFormulaDNA(bestDNA);
		}

		private static void PrintFormulaDNA(DNA dna)
		{
			double x = ((double)dna[0] + (double)dna[1] - 256) / 128;
			double y = ((double)dna[2] + (double)dna[3] - 256) / 128;
			double z = ((double)dna[4] + (double)dna[5] - 256) / 128;
			double w = ((double)dna[6] + (double)dna[7] - 256) / 128;
			Console.WriteLine("x = {0:0.000}, y = {1:0.000}, z = {2:0.000}, w = {3:0.000}", x, y, z, w);
			Console.WriteLine("{0:0.000}^2 + (2 * {1:0.000}) / {2:0.000} + {3:0.000} = {4:0.000}" + Environment.NewLine,
				x, y, z, w, x * x + (2 * y) / z + w);
		}

		private static void PrintTestDNA(DNA dna)
		{
			Console.Write("Hair: ");
			if (dna[1] < 100)
				Console.WriteLine("Brown");
			else if(dna[1] < 200)
				Console.WriteLine("Blonde");
			else
				Console.WriteLine("Red");

			Console.Write("Eyes: ");
			if(dna[2] < 100)
				Console.WriteLine("Brown");
			else if (dna[2] < 200)
				Console.WriteLine("Green");
			else
				Console.WriteLine("Blue");

			Console.Write("Gender: ");
			if(dna[3] < 128)
				Console.WriteLine("Male");
			else
				Console.WriteLine("Female");

			Console.Write("Height: ");
			double feet = ((double)dna[4] * 2) / 255;
			feet += 4.5;
			Console.WriteLine("{0:0.0} feet", feet);

			Console.Write("Weight: ");
			double lbs = ((double)dna[4] * 200) / 255;
			lbs += 100;
			Console.WriteLine("{0:0.0} lbs", lbs);
		}

		private static void TestGOAP(PlannerActionSet actions)
		{
			var initialState = new PlannerState();
			var goalState = new PlannerStateCondition();

			initialState.Add(new PlannerStateSymbol<Location>
				{
					Name = "Location",
					Value = Location.Couch
				});
			initialState.Add(new PlannerStateSymbol<Hunger>
				{
					Name = "Hunger",
					Value = Hunger.Hungry
				});
			initialState.Add(new PlannerStateSymbol<double>
				{
					Name = "Money",
					Value = 16
				});
			initialState.Add(new PlannerStateSymbol<Food>
				{
					Name = "Food",
					Value = Food.None
				});

			goalState.Add(new PlannerStateSymbolCondition<Hunger>
				{
					Name = "Hunger",
					Value = Hunger.NotHungry,
					Comparison = ComparisonOperator.EqualTo | ComparisonOperator.GreaterThan
				});
			goalState.Add(new PlannerStateSymbolCondition<double>
				{
					Name = "Money",
					Value = 1,
					Comparison = ComparisonOperator.EqualTo | ComparisonOperator.GreaterThan
				});

			Console.WriteLine("Goal state:");
			PrintPlannerStateCondition(goalState);
			Console.WriteLine();

			Console.WriteLine("Current state:");
			PrintPlannerState(initialState);
			Console.WriteLine();

			Console.WriteLine("Available actions:");
			PrintActionSet(actions);
			Console.WriteLine();

			Console.WriteLine("Press a key to continue.");
			Console.ReadKey();
			Console.Clear();

			var plan = GoalOrientedActionPlanner.Plan(initialState, goalState, actions);

			Console.WriteLine("Plan:");
			if(plan.Count() == 0)
				Console.WriteLine("No valid plan found.");
			int i = 1;
			foreach (var actionInstance in plan)
			{
				Console.WriteLine("{0}. \"{1}\"", i, actionInstance.Action.DisplayName);
				i++;
			}

			Console.WriteLine("Press a key to continue.");
			Console.ReadKey();

			var currentState = initialState;

			foreach (var actionInstance in plan)
			{
				Console.Clear();

				Console.WriteLine("Goal state:");
				PrintPlannerStateCondition(goalState);
				Console.WriteLine();

				Console.WriteLine("Current state:");
				PrintPlannerState(currentState);
				Console.WriteLine();

				if (!currentState.Equals(goalState))
				{
					Console.Write("Next action: ");
					PrintAction(actionInstance.Action);
					Console.WriteLine("Press a key to continue.");
					currentState = actionInstance.Action.Effects
						+ actionInstance.Action.SymbolicExecute(currentState, actionInstance.Parameters.ToArray())
						+ currentState;
					Console.ReadKey();
				}
			}

			Console.Clear();
			Console.WriteLine("Goal state:");
			PrintPlannerStateCondition(goalState);
			Console.WriteLine();
			Console.WriteLine("Current state:");
			PrintPlannerState(currentState);
			Console.WriteLine();
		}

		private static void PrintPlannerState(PlannerState state)
		{
			foreach (var symbol in state)
			{
				if (symbol.Value is double)
					Console.WriteLine("[{0}] = ${1:0.00}", symbol.Name, symbol.Value);
				else
					Console.WriteLine("[{0}] = {1}", symbol.Name, symbol.Value);
			}
		}

		private static void PrintPlannerStateCondition(PlannerStateCondition state)
		{
			string op1, op2, op3;
			if(state.Count == 0)
				Console.WriteLine("-none-");
			foreach (var symbol in state)
			{
				op1 = "";
				op2 = "";
				op3 = "";

				if ((symbol.Comparison & ComparisonOperator.LessThan) == ComparisonOperator.LessThan)
					op1 = "<";
				if ((symbol.Comparison & ComparisonOperator.GreaterThan) == ComparisonOperator.GreaterThan)
					op2 = ">";
				if ((symbol.Comparison & ComparisonOperator.EqualTo) == ComparisonOperator.EqualTo)
					op3 = "=";
				if (symbol.Value is double)
					Console.WriteLine("[{0}] {1}{2}{3} ${4:0.00}", symbol.Name, op1, op2, op3, symbol.Value);
				else
					Console.WriteLine("[{0}] {1}{2}{3} {4}", symbol.Name, op1, op2, op3, symbol.Value);
			}
		}

		private static void PrintPlannerStateSymbolList(IEnumerable<string> list)
		{
			foreach (string name in list)
				Console.WriteLine("[{0}]", name);
		}

		private static void PrintActionSet(PlannerActionSet actionSet)
		{
			foreach (var action in actionSet)
			{
				PrintAction(action);
				Console.WriteLine();
			}
		}

		private static void PrintActionInstance(PlannerActionInstance instance)
		{
			Console.Write("\"" + instance.Action.DisplayName + "\"");
			if (instance.Parameters.Count > 0)
			{
				if(instance.Parameters.Count > 1)
					Console.Write(" Parameters:");
				foreach(IPlannerStateSymbol parameter in instance.Parameters)
					Console.Write(" \"" + parameter.Name + "\" = " + parameter.Value);
			}

			Console.WriteLine();
		}

		private static void PrintAction(PlannerAction action)
		{
			Console.WriteLine("\"" + action.DisplayName + "\" (" + action.Name + ")");
			Console.WriteLine("Prerequisites:");
			PrintPlannerStateCondition(action.Prerequisites);
			Console.WriteLine("Effects:");
			PrintPlannerStateCondition(action.Effects);
			if (action.OtherAffectedSymbols.Count() > 0)
			{
				Console.WriteLine("Also affects:");
				PrintPlannerStateSymbolList(action.OtherAffectedSymbols);
			}
		}

		private static PlannerActionSet InitializeGOAP()
		{
			GoalOrientedActionPlanner.Initialize();

			PlannerActionPool actions = new PlannerActionPool();

			Extensions<Location>.IsLeftEqualToRight = (loc1, loc2) => { return loc1 == loc2; };
			Extensions<Location>.DistanceBetween = (loc1, loc2) => { return (loc1 == loc2) ? 0 : 1; };

			Extensions<Hunger>.IsLeftEqualToRight = (h1, h2) => { return h1 == h2; };
			Extensions<Hunger>.IsLeftGreaterThanRight = (h1, h2) => { return ((int)h1 > (int)h2); };
			Extensions<Hunger>.IsLeftLessThanRight = (h1, h2) => { return ((int)h1 < (int)h2); };
			Extensions<Hunger>.DistanceBetween = (h1, h2) => { return (h1 == h2) ? 0 : 1; };
			Extensions<Hunger>.Increment = (h) => { return (h == Hunger.Stuffed) ? h : (Hunger)((int)h++); };
			Extensions<Hunger>.Decrement = (h) => { return (h == Hunger.Starving) ? h : (Hunger)((int)h--); };

			Extensions<Food>.IsLeftEqualToRight = (f1, f2) => { return f1 == f2; };
			Extensions<Food>.IsLeftGreaterThanRight = (f1, f2) => { return f1 > f2; };
			Extensions<Food>.IsLeftLessThanRight = (f1, f2) => { return f1 < f2; };
			Extensions<Food>.DistanceBetween = (f1, f2) => { return (f1 == f2) ? 0 : 1; };

			Extensions<double>.IsLeftEqualToRight = (val1, val2) => { return val1 == val2; };
			Extensions<double>.IsLeftGreaterThanRight = (val1, val2) => { return val1 > val2; };
			Extensions<double>.IsLeftLessThanRight = (val1, val2) => { return val1 < val2; };
			Extensions<double>.DistanceBetween = (val1, val2) => { return Math.Abs(val1 - val2) / double.MaxValue; };
			Extensions<double>.Increment = (val) => { return val + 0.01; };
			Extensions<double>.Decrement = (val) => { return val - 0.01; };

			return actions.ToSet();
		}

		private static void GetAnyKey()
		{
			Console.WriteLine("Press a key to return to test selection.");
			Console.ReadKey(true);
		}

		private static void TestAStar(Graph<char, int> testGraph, Dictionary<char, int> graphIds, AStar<char, int> aStar)
		{
			Console.Write("Shortest path is: ");
			double totalCost = 0;
			PathEdge<char, int> lastEdge = null;
			foreach (var edge in aStar.Search(testGraph[graphIds['A']], testGraph[graphIds['G']]))
			{
				lastEdge = edge;
				Console.Write("({0}) -[{1}]-> ", edge.FromNode, edge.Value);
				totalCost += aStar.Cost(edge.FromNode, edge.Value, edge.ToNode);
			}
			if (lastEdge != null)
				Console.WriteLine("({0})", lastEdge.ToNode);
			else
				Console.WriteLine();
			Console.WriteLine("Total Cost: {0}", totalCost);
		}

		private static void TestHeapsort(List<int> ints)
		{
			Console.WriteLine("Ints:");
			foreach (int num in ints)
			{
				Console.WriteLine(num);
			}

			Console.WriteLine("Press any key to continue.");
			Console.ReadKey(true);
			Console.Clear();

			Console.WriteLine("Sorted ascending ints:");
			foreach (int num in Algorithms.Heapsort(ints, (x, y) =>
			{
				if (x > y)
					return -1;
				if (x == y)
					return 0;
				return 1;
			}))
			{
				Console.WriteLine(num);
			}
		}

		private static List<int> GenTestInts()
		{
			return new List<int> { 1, 5, 7, 3124, 232, 352, 56, 7, 140, 3, 52, 6, 125, 3, 492, 131234, 8, 55, 314159 };
		}

		private static void GenTestGraph(out Graph<char, int> testGraph, out Dictionary<char, int> ids, out AStar<char, int> aStar)
		{
			testGraph = new Graph<char, int>();
			ids = new Dictionary<char, int>();
			aStar = new AStar<char, int>();

			ids.Add('A', testGraph.AddNode('A'));
			ids.Add('B', testGraph.AddNode('B'));
			ids.Add('C', testGraph.AddNode('C'));
			ids.Add('D', testGraph.AddNode('D'));
			ids.Add('E', testGraph.AddNode('E'));
			ids.Add('F', testGraph.AddNode('F'));
			ids.Add('G', testGraph.AddNode('G'));

			testGraph.AddEdge(ids['A'], ids['B'], 12);
			testGraph.AddEdge(ids['A'], ids['C'], 22);
			testGraph.AddEdge(ids['B'], ids['A'], 10);
			testGraph.AddEdge(ids['B'], ids['C'], 7);
			testGraph.AddEdge(ids['C'], ids['D'], 9);
			testGraph.AddEdge(ids['C'], ids['F'], 16);
			testGraph.AddEdge(ids['D'], ids['F'], 4);
			testGraph.AddEdge(ids['D'], ids['G'], 5);
			testGraph.AddEdge(ids['G'], ids['D'], 6);
			testGraph.AddEdge(ids['A'], ids['D'], 30);
			testGraph.AddEdge(ids['F'], ids['E'], 1);
			testGraph.AddEdge(ids['E'], ids['G'], 3);
			testGraph.AddEdge(ids['E'], ids['B'], 8);

			aStar.Cost = (node1, cost, node2) =>
				{
					return (double)cost;
				};
			aStar.ExpectedCost = (node, target) =>
				{
					return 1;
				};
		}

		private static void TestDeck<T>(Deck<T> deck)
		{
			foreach (T card in deck.Contents)
				PrintCard(card);

			deck.Shuffle();

			Console.WriteLine("Press any key to continue.");
			Console.ReadKey();
			Console.Clear();
			Console.WriteLine("Shuffled deck.");

			while ((Console.ReadKey().Key != ConsoleKey.Escape) && (deck.Count > 0))
			{
				Console.Clear();
				Console.Write("Drew a card: ");
				PrintCard(deck.DrawTop());
				Console.WriteLine();
				Console.WriteLine("Next five cards:");
				foreach (T card in deck.TopFew(5))
					PrintCard(card);
				Console.WriteLine("Press any key to continue, or escape to return to test selection.");
			}
		}

		private static Deck<PlayingCard> GenPlayingDeck()
		{
			Deck<PlayingCard> playingDeck = new Deck<PlayingCard>();
			for (int i = 1; i < 14; i++)
			{
				playingDeck.AddBottom(new PlayingCard { Rank = (PlayingCardRank)i, Suit = PlayingCardSuit.Clubs });
				playingDeck.AddBottom(new PlayingCard { Rank = (PlayingCardRank)i, Suit = PlayingCardSuit.Diamonds });
				playingDeck.AddBottom(new PlayingCard { Rank = (PlayingCardRank)i, Suit = PlayingCardSuit.Hearts });
				playingDeck.AddBottom(new PlayingCard { Rank = (PlayingCardRank)i, Suit = PlayingCardSuit.Spades });
			}
			playingDeck.AddBottom(new PlayingCard { Rank = PlayingCardRank.Joker, Suit = PlayingCardSuit.None });
			playingDeck.AddBottom(new PlayingCard { Rank = PlayingCardRank.Joker, Suit = PlayingCardSuit.None });
			return playingDeck;
		}

		private static Deck<TarotCard> GenTarotDeck()
		{
			Deck<TarotCard> tarotDeck = new Deck<TarotCard>();
			TarotCardValue currentCardValue;
			for (int i = 1; i < 15; i++)
			{
				currentCardValue = new MinorArcana { Rank = (MinorArcanaRank)i, Suit = MinorArcanaSuit.Cups };
				tarotDeck.AddBottom(new TarotCard { Value = currentCardValue });

				currentCardValue = new MinorArcana { Rank = (MinorArcanaRank)i, Suit = MinorArcanaSuit.Pentacles };
				tarotDeck.AddBottom(new TarotCard { Value = currentCardValue });

				currentCardValue = new MinorArcana { Rank = (MinorArcanaRank)i, Suit = MinorArcanaSuit.Swords };
				tarotDeck.AddBottom(new TarotCard { Value = currentCardValue });

				currentCardValue = new MinorArcana { Rank = (MinorArcanaRank)i, Suit = MinorArcanaSuit.Wands };
				tarotDeck.AddBottom(new TarotCard { Value = currentCardValue });
			}

			for (int i = 0; i < 22; i++)
			{
				currentCardValue = new MajorArcana { Value = (MajorArcanaValue)i };
				tarotDeck.AddBottom(new TarotCard { Value = currentCardValue });
			}
			return tarotDeck;
		}

		private static void PrintCard<T>(T card)
		{
			if (card is TarotCard)
				PrintTarotCard(card as TarotCard);
			else if (card is PlayingCard)
				PrintPlayingCard(card as PlayingCard);
		}

		private static void PrintTarotCard(TarotCard card)
		{
			if (card == null)
				return;
			MinorArcana minor = card.Value as MinorArcana;
			if (minor != null)
			{
				Console.WriteLine(minor.Rank + " of " + minor.Suit);
			}
			else
			{
				MajorArcana major = card.Value as MajorArcana;
				if (major == null)
					Console.WriteLine("error");
				else
				{
					Console.WriteLine((int)major.Value + ", " + major.Value);
				}
			}
		}

		private static void PrintPlayingCard(PlayingCard card)
		{
			if (card == null)
				return;
			if (card.Suit != PlayingCardSuit.None)
			{
				Console.Write(card.Rank + " of " + card.Suit);
				switch (card.Rank)
				{
					case PlayingCardRank.Ace: Console.Write(" (A");
						break;
					case PlayingCardRank.Jack: Console.Write(" (J");
						break;
					case PlayingCardRank.Queen: Console.Write(" (Q");
						break;
					case PlayingCardRank.King: Console.Write(" (K");
						break;
					default: Console.Write(" (" + (int)card.Rank);
						break;
				}
				switch (card.Suit)
				{
					case PlayingCardSuit.Clubs: Console.WriteLine("♣)");
						break;
					case PlayingCardSuit.Diamonds: Console.WriteLine("♦)");
						break;
					case PlayingCardSuit.Hearts: Console.WriteLine("♥)");
						break;
					case PlayingCardSuit.Spades: Console.WriteLine("♠)");
						break;
					default: break;
				}
			}
			else
				Console.WriteLine(card.Rank);
		}
	}
}
