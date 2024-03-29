﻿/*	
 *	Project Name:	Nai Project - Neural Network.
 *	Author:			Andrzej Torski
 *	Index:			s10415
 */

using System;
using System.IO;
using en.AndrewTorski.Nai.TaskOne;
using OfficeOpenXml;

namespace ActivationFunctionMetric
{
	public class ActivationFunctionEvaluation
	{
		public static void Main(string[] args)
		{

			//var correctSampleRun = GetFileFromFunctionEvaluationRun("Andrzej", 1.0);
			//var incorrectSampleRun = GetFileFromFunctionEvaluationRun("MaxKolo", 0.0);
			var runForAllSets = GetFileFromFunctionEvaluationRun();
			//var networkNeuralData = NetworkPrinter.RetrieveNeuralData(network);

			//File.WriteAllBytes(@"C:\Nai\func\Nai.Function.Correct.xlsx", correctSampleRun);
			//File.WriteAllBytes(@"C:\Nai\func\Nai.Function.InCorrect.xlsx", incorrectSampleRun);
			//File.WriteAllBytes(@"C:\Nai\func\Nai.Neural.Data.xlsx", networkNeuralData);
			File.WriteAllBytes(@"C:\Nai\Nai.Test.xlsx", runForAllSets);
			//	Done.
			Console.WriteLine("Done;");
			Console.ReadKey();
		}

		public static byte[] GetFileFromFunctionEvaluationRun()
		{
			//	Create a sigmoid function with default values = 1.
			IActivationFunction function;

			//	Get 20 training sets - half is true, half is false.
			var trainingSets = NetworkUtils.GetTrainingSet();

			//	value assigned to eta has proven to be the best choice for learning.
			const double eta = 0.35;

			const int numberOfLearningRuns = 10000;

			//	Create a container with a 2D array which will contain the sums of error for given alpha and beta.
			var statData = new FunctionEvaluationStatisticalContainer(20, 20);

			//	In total to each cell(of given alpha and beta) we will add a result from below variable number of runs.
			const int numberOfFunctionEvaluationRuns = 200;

			//	Starting point for each of the function's parameters.
			const double alphaStart = 0.5;
			const double betaStart = -5.0;

			//	These variables represent the current value of function parameters.
			var currentAlpha = 0.5;
			var currentBeta = -5.0;

			//	The value by which we will increase parameters.
			const double alphaIncrement = 0.5;
			const double betaIncrement = 0.5;

			Network network;
			for (var currentRun = 0; currentRun < numberOfFunctionEvaluationRuns; currentRun++)
			{
				Console.WriteLine(currentRun);
				function = new SigmoidalActivationFunction();
				//	First create a network of following configuration 7-3-1.
				network = new Network(7, 3, function);

				//	SET UP THE NETWORK!!!(meaning that we create neurons in place of old ones and randomize their weights)
				network.SetUp();

				//	Train the network with function's parameters Alpha = 1 and Beta = 0.
				for (var i = 0; i < numberOfLearningRuns; i++)
				{
					//	Shuffle the training sets for believable training.
					trainingSets.Shuffle();
					foreach (var trainingSet in trainingSets)
					{
						network.ConductClassification(trainingSet.AsciiVectors);
						network.TrainNeurons(trainingSet.ExpectedValue, eta);
					}
				}
				
				NetworkUtils.printNeuronInfo(network.OutputNeuron);
				Console.ReadKey();
				foreach (var hiddenNeuron in network.HiddenNeurons)
				{
					Console.Clear();
					NetworkUtils.printNeuronInfo(hiddenNeuron);
					Console.ReadKey();
				}

				foreach (var inputNeuron in network.InputNeurons)
				{
					Console.Clear();
					NetworkUtils.printNeuronInfo(inputNeuron);
					Console.ReadKey();
				}

				//	Reset beta step to the starting point.
				currentBeta = betaStart;
				for (var currentBetaStep = 1; currentBetaStep <= 20; currentBetaStep++)
				{
					//	Once we are finished evaluating beta, reset alpha to inital state.
					currentAlpha = alphaStart;

					//	Change the beta value of the activation function.
					function.Beta = currentBeta;

					for (var currentAlphaStep = 1; currentAlphaStep <= 20; currentAlphaStep++)
					{
						//	Change the alpha value of the activation function.
						function.Alpha = currentAlpha;

						foreach (var trainingSet in trainingSets)
						{
							var calculated = network.ConductClassification(trainingSet.AsciiVectors);
							//	Add the calculated error to the 2d array.
							statData[currentBetaStep - 1, currentAlphaStep - 1] += calculated - trainingSet.ExpectedValue;
							//Console.WriteLine("Function: Alpha: {0}\tBeta: {1}\tExpected: {2}\tOutput: {3}\t Abs:{4}", function.Alpha, function.Beta, trainingSet.ExpectedValue, calculated, trainingSet.ExpectedValue - calculated);
						}
						
						currentAlpha += alphaIncrement;
					}
					currentBeta += betaIncrement;
				}
			}

			//	Divide each value in cells in the statistical container by the total number of runs. 
			statData.CalculateAverageForEachCell(numberOfFunctionEvaluationRuns);

			//	Revert alphastep and betastep to initial values so we may reuse them in later section.
			currentAlpha = alphaStart;
			currentBeta = betaStart;

			using (var xlPackage = new ExcelPackage())
			{
				xlPackage.Workbook.Properties.Author = "Andrzej Torski";
				xlPackage.Workbook.Properties.Title = "Nai.TaskOne.ActivationFunctionEvaluation";
				xlPackage.Workbook.Properties.Company = "Andrew Torski Artifical Computing";

				xlPackage.Workbook.Worksheets.Add("Data");

				var workSheet = xlPackage.Workbook.Worksheets[1];

				workSheet.Name = "Data";
				ExcelRange cell;
				//	Parse in the first column which contains beta values starting from [2,1] and going vertically down from that point.
				for (var i = 1; i <= 20; i++)
				{
					cell = workSheet.Cells[i + 1, 1];
					cell.Value = currentBeta;
					currentBeta += betaIncrement;
				}

				// Parse the alpha values row starting from [1,2] and going horizontally right from that point.
				for (var i = 1; i <= 20; i++)
				{
					cell = workSheet.Cells[1, i + 1];
					cell.Value = currentAlpha;
					currentAlpha += alphaIncrement;
				}


				for (var currentBetaStep = 0; currentBetaStep < 20; currentBetaStep++)
				{
					for (var currentAlphaStep = 0; currentAlphaStep < 20; currentAlphaStep++)
					{
						cell = workSheet.Cells[currentBetaStep + 2, currentAlphaStep + 2];
						cell.Value = statData[currentBetaStep, currentAlphaStep];
					}
				}

				var binaryData = xlPackage.GetAsByteArray();
				return binaryData;
			}
		}

	}
}
