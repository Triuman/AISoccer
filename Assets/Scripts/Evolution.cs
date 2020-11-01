using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Assets.Scripts
{
    public class Evolution : MonoBehaviour
    {
        public Player PlayerPrefab;

        public Text TxtGeneration;
        public Text TxtPopulation;

        private static readonly Random random = new Random(Guid.NewGuid().GetHashCode());


        private static readonly int[] layers = new[] { 3, 8, 2 };
        private static int population = 40;
        private static float mutationRate = 0.1f;
        private static float eliteRatio = 0;     //the number of best agents who go to next generation unchanged.

        private static int generationNo = 0;
        private static List<Agent> currentGeneration = null;

        private static int maxSimulationDuration = 10; //seconds
        private static bool isSimulating = false;
        private static float simulationStartTime = 0;


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
            Time.timeScale = 4;

            CreateNewGeneration();
            StartSimulation();
        }

        private void Update()
        {
            if (isSimulating)
            {
                if (Time.time - simulationStartTime >= maxSimulationDuration)
                {
                    StopSimulation();
                    CreateNewGeneration();
                    StartSimulation();
                }
            }
        }

        private void StartSimulation()
        {
            foreach (var agent in currentGeneration)
            {
                agent.Activate();
            }
            simulationStartTime = Time.time;
            isSimulating = true;
        }

        private void StopSimulation()
        {
            isSimulating = false;
            foreach (var agent in currentGeneration)
            {
                agent.Deactivate();
            }
        }

        private void CreateNewGeneration()
        {
            var newGeneration = new List<Agent>();
            if (generationNo == 0)
            {
                for (int a = 0; a < population; a++)
                {
                    newGeneration.Add(new Agent(new NeuralNetwork(layers), Instantiate(PlayerPrefab)));
                }
            }
            else
            {
                currentGeneration = currentGeneration.OrderByDescending(a => a.fitness).ToList();

                // Get the elite to the list first
                int eliteCount = (int)Math.Floor(currentGeneration.Count * eliteRatio);
                newGeneration.AddRange(currentGeneration); // We take all as we want to reuse all the existing agents
                for (int e = 0; e < eliteCount; e++)
                {
                    // We put all of the old generation to new one but only the first x are elite and will stay same.
                    newGeneration[e].Reset(PlayerPrefab.transform.position);
                }

                var minFitness = currentGeneration.Min(a => a.fitness);
                var fitnessSum = currentGeneration.Sum(a => a.fitness) - minFitness * currentGeneration.Count;

                int remainingCount = currentGeneration.Count - eliteCount;
                for (int g = 0; g < remainingCount; g++)
                {
                    var parent1 = SelectRandomlyByFitness(fitnessSum, minFitness);
                    var parent2 = SelectRandomlyByFitness(fitnessSum, minFitness);
                    var childBrain = mutate(crossover(new[] { parent1, parent2 }));
                    if (newGeneration.Count > eliteCount)
                    {
                        newGeneration[eliteCount++].Reset(PlayerPrefab.transform.position, childBrain);
                        continue;
                    }
                    Debug.Log("Added new agent.");
                    newGeneration.Add(new Agent(childBrain, Instantiate(PlayerPrefab)));
                }
            }

            for (int i = 0; i < newGeneration.Count; i++)
            {
                for (int j = i + 1; j < newGeneration.Count; j++)
                {
                    var colliders1 = newGeneration[i].Colliders;
                    var colliders2 = newGeneration[j].Colliders;
                    for (int c1 = 0; c1 < colliders1.Length; c1++)
                    {
                        for (int c2 = 0; c2 < colliders2.Length; c2++)
                        {
                            Physics2D.IgnoreCollision(colliders1[c1], colliders2[c2]);
                        }
                    }
                }
            }

            currentGeneration = newGeneration;
            generationNo++;
            TxtGeneration.text = generationNo.ToString();
            TxtPopulation.text = currentGeneration.Count.ToString();
        }


        private NeuralNetwork SelectRandomlyByFitness(float fitnessSum, float minFitness)
        {
            double randomFitness = random.NextDouble() * fitnessSum;
            for (int f = 0; f < currentGeneration.Count; f++)
            {
                randomFitness -= (currentGeneration[f].fitness - minFitness); // to make all positive numbers
                if (randomFitness <= 0)
                {
                    return currentGeneration[f].Brain;
                }
                if (f >= currentGeneration.Count - 1)
                    f = 0;
            }
            Debug.LogError("Selected Default!");
            return currentGeneration[random.Next(currentGeneration.Count - 1)].Brain;
        }


        private static NeuralNetwork crossover(NeuralNetwork[] parents)
        {
            var newBrain = new NeuralNetwork(parents[0].Layers);

            for (int l = 0; l < parents[0].Weights.Length; l++)
            {
                for (int n = 0; n < parents[0].Weights[l].GetLength(0); n++)
                {
                    for (int w = 0; w < parents[0].Weights[l].GetLength(1); w++)
                    {
                        int parentIndexWeight = random.Next(parents.Length - 1);
                        newBrain.Weights[l][n, w] = random.NextDouble() > mutationRate ? NeuralNetwork.GetRandomWeight : parents[parentIndexWeight].Weights[l][n, w];
                    }
                    int parentIndexBias = random.Next(parents.Length - 1);
                    newBrain.Biases[l][n] = random.NextDouble() > mutationRate ? NeuralNetwork.GetRandomBias : parents[parentIndexBias].Biases[l][n];
                }
            }
            return newBrain;
        }


        private static NeuralNetwork mutate(NeuralNetwork brain)
        {
            //return brain;
            var newBrain = new NeuralNetwork(brain.Layers);

            for (int l = 0; l < brain.Weights.Length; l++)
            {
                for (int n = 0; n < brain.Weights[l].GetLength(0); n++)
                {
                    bool mutate = random.NextDouble() < mutationRate;
                    for (int w = 0; w < brain.Weights[l].GetLength(1); w++)
                    {
                        newBrain.Weights[l][n, w] = mutate ? NeuralNetwork.GetRandomWeight : brain.Weights[l][n, w];
                    }
                    newBrain.Biases[l][n] = mutate ? NeuralNetwork.GetRandomBias : brain.Biases[l][n];
                }
            }
            return newBrain;
        }
    }
}
