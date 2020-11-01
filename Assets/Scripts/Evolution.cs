using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public class Evolution : MonoBehaviour
    {
        public Player PlayerPrefab;

        private static readonly Random random = new Random(Guid.NewGuid().GetHashCode());

        //Steps:
        //Get an environment for each agent
        //Generate a new generation with the population size
        //run simulation until max time or goal reached by every agent
        //order agents by their fitness value
        //select agents randomly proportional to their fitness
        //do crossover
        //do mutation

        void Start()
        {
            CreateNewGeneration();
            StartSimulation();
        }

        private void CreateNewGeneration(NeuralNetwork[] brains = null)
        {
            generationNo++;
            currentGeneration = new List<Agent>();
            if (brains != null)
            {
                for (int b = 0; b < brains.Length; b++)
                {
                    currentGeneration.Add(new Agent(brains[b], Instantiate(PlayerPrefab)));
                }
            }
            else
            {
                for (int p = 0; p < population; p++)
                {
                    currentGeneration.Add(new Agent(new NeuralNetwork(layers), Instantiate(PlayerPrefab)));
                }
            }
            for (int i = 0; i < currentGeneration.Count; i++)
            {
                for (int j = i + 1; j < currentGeneration.Count; j++)
                {
                    var colliders1 = currentGeneration[i].Colliders;
                    var colliders2 = currentGeneration[j].Colliders;
                    for (int c1 = 0; c1 < colliders1.Length; c1++)
                    {
                        for (int c2 = 0; c2 < colliders2.Length; c2++)
                        {
                            Physics2D.IgnoreCollision(colliders1[c1], colliders2[c2]);
                        }
                    }
                }
            }
        }

        private void StartSimulation()
        {
            foreach (var agent in currentGeneration)
            {
                agent.isActive = true;
            }
            isSimulating = true;
        }
        private void StopSimulation()
        {
            isSimulating = false;
            foreach (var agent in currentGeneration)
            {
                agent.isActive = false;
            }
        }


        //Crossover probability
        //Crossover distribution
        //Mutation probability: Mutation rate (probability): this rate determines how many chromosomes should be mutated in one generation; mutation rate is in the range of [0, 1].
        //Mutation distribution


        private static readonly int[] layers = new[] { 3, 8, 2 };
        private static int population = 10;
        private static float mutationRate = 0.1f;


        private static int generationNo = 0;
        private static List<Agent> currentGeneration = null;

        //the number of best agents who go to next generation unchanged.
        private static float elitRatio = 0.1f;

        private static int maxSimulationDuration = 1; //Minutes
        private static bool isSimulating = false;
        private static float simulationStartTime = 0;



        private static NeuralNetwork crossover(Agent[] parents)
        {
            var newBrain = new NeuralNetwork(parents[0].Brain.Layers);

            for (int l = 0; l < parents[0].Brain.Weights.Length; l++)
            {
                for (int n = 0; n < parents[0].Brain.Weights[0].GetLength(0); n++)
                {
                    for (int w = 0; w < parents[0].Brain.Weights[0].GetLength(1); w++)
                    {
                        int parentIndexWeight = random.Next(parents.Length - 1);
                        newBrain.Weights[l][n, w] = random.NextDouble() > mutationRate ? NeuralNetwork.GetRandomWeight : parents[parentIndexWeight].Brain.Weights[l][n, w];
                    }
                    int parentIndexBias = random.Next(parents.Length - 1);
                    newBrain.Biases[l][n] = random.NextDouble() > mutationRate ? NeuralNetwork.GetRandomBias : parents[parentIndexBias].Brain.Biases[l][n];
                }
            }
            return newBrain;
        }


        private static NeuralNetwork mutate(Agent agent)
        {
            var newBrain = new NeuralNetwork(agent.Brain.Layers);

            for (int l = 0; l < agent.Brain.Weights.Length; l++)
            {
                for (int n = 0; n < agent.Brain.Weights[0].GetLength(0); n++)
                {
                    for (int w = 0; w < agent.Brain.Weights[0].GetLength(1); w++)
                    {
                        newBrain.Weights[l][n, w] = random.NextDouble() > mutationRate ? NeuralNetwork.GetRandomWeight : agent.Brain.Weights[l][n, w];
                    }
                    newBrain.Biases[l][n] = random.NextDouble() > mutationRate ? NeuralNetwork.GetRandomBias : agent.Brain.Biases[l][n];
                }
            }
            return newBrain;
        }
    }
}
