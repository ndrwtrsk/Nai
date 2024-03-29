﻿using System.Linq;
using Shared;
using Shared.Bases;
using Shared.Utils;

namespace GeneticOperators.Crossovers
{
	/// <summary>
	///		Allows to perform a One point crossover on the population.
	/// </summary>
	public class OnePointCrossover : Recombinator
	{
		/// <summary>
		///		Occurence at which the crossing over should occur.
		/// </summary>
		private readonly double _crossoverChance;

		/// <summary>
		///		Initializes the object with the mutation probability.
		/// </summary>
		/// <param name="crossoverChance">
		///		Crossover chance ranging from 0.0 to 1.0.
		/// </param>
		public OnePointCrossover(double crossoverChance)
		{
			this._crossoverChance = crossoverChance;
		}

		/// <summary>
		///		Peforms a One Point Crossover on two solutions.
		/// </summary>
		/// <param name="firstParent">
		///		First genome to perform Crossover on.
		/// </param>
		/// <param name="secondParent">
		///		Second genome to perform Crossover on.
		/// </param>
		public override void Crossover(CandidateSolution firstParent, CandidateSolution secondParent)
		{
			this.Crossover(new Pair<CandidateSolution>(firstParent, secondParent));
		}

		///  <summary>
		/// 		Peforms aOne Point Crossover on a Pair of solutions.
		///  </summary>
		/// <param name="solutionPair">
		///		Pair to perform a crossover on.
		/// </param>
		public override void Crossover(Pair<CandidateSolution> solutionPair)
		{
			//	check if crossing-over will even happen
			var randomDouble = RandomGenerator.GetRandomDouble();
			if (randomDouble > this._crossoverChance)
			{
				return;
			}

			var firstGenome = solutionPair.X.Solution;
			var secondGenome = solutionPair.Y.Solution;

			//	select random crossing point solution's index range.
			var crossingPoint = RandomGenerator.GetRandomInt(0, firstGenome.Count());

			//	[Former][Latter]
			//	splice parent genomes into 4 parts
			var firstGenomeFormerPart = firstGenome.Take(crossingPoint);
			var secondGenomeFormerPart = firstGenome.Take(crossingPoint);

			var firstGenomeLatterPart = firstGenome.Skip(crossingPoint);
			var secondGenomeLatterPart = secondGenome.Skip(crossingPoint);

			//	join splices into children
			var firstChildGenome = firstGenomeFormerPart.Concat(secondGenomeLatterPart);
			var secondChildGenome = secondGenomeFormerPart.Concat(firstGenomeLatterPart);

			firstGenome = firstChildGenome;
			secondGenome = secondChildGenome;
		}

		///  <summary>
		/// 		Peforms aOne Point Crossover on a Pair of solutions.
		///  </summary>
		/// <param name="solutionPair">
		///		Pair to perform a crossover on.
		/// </param>
		public void CrossoverForTestsOnly(Pair<CandidateSolution> solutionPair, int fixedCrossingIndex)
		{
			//	check if crossing-over will even happen
			var randomDouble = RandomGenerator.GetRandomDouble();
			if (randomDouble > this._crossoverChance)
			{
				return;
			}

			var firstGenome = solutionPair.X.Solution;
			var secondGenome = solutionPair.Y.Solution;

			//	select random crossing point solution's index range.
			var crossingPoint = fixedCrossingIndex;

			//	[Former][Latter]
			//	splice parent genomes into 4 parts
			var firstGenomeFormerPart = firstGenome.Take(crossingPoint);
			var secondGenomeFormerPart = secondGenome.Take(crossingPoint);

			var firstGenomeLatterPart = firstGenome.Skip(crossingPoint);
			var secondGenomeLatterPart = secondGenome.Skip(crossingPoint);

			//	join splices into children
			var firstChildGenome = firstGenomeFormerPart.Concat(secondGenomeLatterPart);
			var secondChildGenome = secondGenomeFormerPart.Concat(firstGenomeLatterPart);

			solutionPair.X.Solution = firstChildGenome;
			solutionPair.Y.Solution = secondChildGenome;
		}
	}
}
