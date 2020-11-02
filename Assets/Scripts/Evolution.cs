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
        public Transform RightGoalTransform;

        public Text TxtGeneration;
        public Text TxtPopulation;

        private const float MinX = -6;
        private const float MaxX = 7;
        private const float MinY = -3;
        private const float MaxY = 2.5f;
        private static readonly Random random = new Random(Guid.NewGuid().GetHashCode());

        private static float timeScale = 10f;
        private static readonly int[] layers = new[] { 6, 16, 2 };
        private static int population = 100;
        private static double mutationRate = 0.1;
        private static float eliteRatio = 0.1f;     //the number of best agents who go to next generation unchanged.

        private static int generationNo = 0;
        private static List<Agent> currentGeneration = null;

        private static int maxSimulationDuration = 20; //seconds
        private static bool isSimulating = false;
        private static float simulationStartTime = 0;


        private Agent SampleAgent;


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
            Time.timeScale = timeScale;

            InitGeneration();
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

        private void InitGeneration()
        {
            SampleAgent = new Agent(new NeuralNetwork(layers), Instantiate(PlayerPrefab), RightGoalTransform);
            SampleAgent.EnableRenderer(true);
            SampleAgent.Activate();

            currentGeneration = new List<Agent>();
            for (int a = 0; a < population; a++)
            {
                PlayerPrefab.transform.position = GetRandomPosition();
                currentGeneration.Add(new Agent(new NeuralNetwork(layers), Instantiate(PlayerPrefab), RightGoalTransform));
            }

            for (int i = 0; i < currentGeneration.Count; i++)
            {
                var colliders1 = currentGeneration[i].Colliders;
                for (int c1 = 0; c1 < colliders1.Length; c1++)
                {
                    foreach (var sampleCollider in SampleAgent.Colliders)
                    {
                        Physics2D.IgnoreCollision(colliders1[c1], sampleCollider);
                    }

                    for (int j = i + 1; j < currentGeneration.Count; j++)
                    {
                        var colliders2 = currentGeneration[j].Colliders;
                        for (int c2 = 0; c2 < colliders2.Length; c2++)
                        {
                            Physics2D.IgnoreCollision(colliders1[c1], colliders2[c2]);
                        }
                    }
                }
            }
        }

        private void CreateNewGeneration()
        {
            var newGenerationBrains = new List<NeuralNetwork>();
            currentGeneration = currentGeneration.OrderByDescending(a => a.fitness).ToList();
            SampleAgent.Brain = currentGeneration[0].Brain;

            // Get the elite to the list first
            int eliteCount = (int)Math.Floor(currentGeneration.Count * eliteRatio);
            // newGeneration.AddRange(currentGeneration); // We take all as we want to reuse all the existing agents
            //for (int e = 0; e < eliteCount; e++)
            //{
            //    // We put all of the old generation to new one but only the first x are elite and will stay same.
            //    newGeneration[e].Reset(GetRandomPosition());
            //}

            var minFitness = currentGeneration.Min(a => a.fitness * a.fitness);
            var fitnessSum = currentGeneration.Sum(a => a.fitness * a.fitness) - minFitness * currentGeneration.Count;

            for (int g = eliteCount; g < currentGeneration.Count; g++)
            {
                var parent1 = SelectRandomlyByFitness(fitnessSum, minFitness);
                //var parent2 = SelectRandomlyByFitness(fitnessSum, minFitness);
                var childBrain = mutate(parent1);
                newGenerationBrains.Add(childBrain);
            }

            for (int b = 0; b < newGenerationBrains.Count; b++)
            {
                currentGeneration[b].Reset(GetRandomPosition(), newGenerationBrains[b]);
            }

            generationNo++;
            TxtGeneration.text = generationNo.ToString();
            TxtPopulation.text = currentGeneration.Count.ToString();
        }

        public static Vector2 GetRandomPosition()
        {
            return new Vector2((float)random.NextDouble() * (MaxX - MinX) + MinX, (float)random.NextDouble() * (MaxY - MinY) + MinY);
        }

        private NeuralNetwork SelectRandomlyByFitness(float fitnessSum, float minFitness)
        {
            double randomFitness = random.NextDouble() * fitnessSum;
            for (int f = 0; f < currentGeneration.Count; f++)
            {
                randomFitness -= (currentGeneration[f].fitness * currentGeneration[f].fitness - minFitness); // to make all positive numbers
                if (randomFitness <= 0)
                {
                    if (currentGeneration[f].fitness == 0)
                    {
                        Debug.LogError(currentGeneration[f].fitness);
                        Debug.LogError(currentGeneration[f].IsActive);
                    }
                    Debug.LogError(f);
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
                    int parentIndex = random.Next(parents.Length);
                    for (int w = 0; w < parents[0].Weights[l].GetLength(1); w++)
                    {
                        Debug.Log(parentIndex);
                        newBrain.Weights[l][n, w] = random.NextDouble() > mutationRate ? NeuralNetwork.GetRandomWeight : parents[parentIndex].Weights[l][n, w];
                    }
                    newBrain.Biases[l][n] = random.NextDouble() > mutationRate ? NeuralNetwork.GetRandomBias : parents[parentIndex].Biases[l][n];
                }
            }
            return newBrain;
        }


        private static NeuralNetwork mutate(NeuralNetwork brain)
        {
            var random = new Random();
            //return brain;
            var newBrain = new NeuralNetwork(brain.Layers);

            for (int l = 0; l < brain.Weights.Length; l++)
            {
                for (int n = 0; n < brain.Weights[l].GetLength(0); n++)
                {
                    for (int w = 0; w < brain.Weights[l].GetLength(1); w++)
                    {
                        var mutBy = 1 - mutationRate * (random.NextDouble() - 0.5) * 2;
                        newBrain.Weights[l][n, w] = brain.Weights[l][n, w] * mutBy;
                    }
                    var mutByBias = 1 - mutationRate * (random.NextDouble() - 0.5) * 2;
                    newBrain.Biases[l][n] = brain.Biases[l][n] * mutByBias;
                }
            }
            return newBrain;
        }
    }
}