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
        public Text TxtTraining;

        public static float MinFitness = Mathf.Infinity;
        public static float MaxFitness = Mathf.NegativeInfinity;

        private const float MinX = -6;
        private const float MaxX = 7;
        private const float MinY = -3;
        private const float MaxY = 2.5f;
        private static readonly Random random = new Random(Guid.NewGuid().GetHashCode());

        private static float timeScale = 8f;
        private static LayerDefinition[] layerDefinitions;
        private static int population = 300;
        private static double mutationRate = 0.3;
        private static float eliteRatio = 0.1f; //the number of best agents who go to next generation unchanged.
        private static float newAgentsRatio = 0.1f; //the number of randomly generated agents each generation.

        private static int trainingCount = 5; // Each generation will train x many times with random position before generating new generation.

        private static int generationNo = 0;
        private static int trainingNo = 0;
        private static List<Agent> currentGeneration = null;

        private static int maxSimulationDuration = 30; //seconds
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
            // SampleAgent = new Agent(new NeuralNetwork(layers), Instantiate(PlayerPrefab), RightGoalTransform);
            // SampleAgent.EnableRenderer(true);
            // // SampleAgent.isSample = true;
            // SampleAgent.Activate();

            layerDefinitions = new LayerDefinition[] {
                new LayerDefinition(6, EnmActivations.None),
                new LayerDefinition(10, EnmActivations.LRelu),
                new LayerDefinition(3, EnmActivations.LRelu)
            };
  
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
                    if (trainingNo >= trainingCount)
                        CreateNewGeneration();
                    StartSimulation();
                }
            }
        }
        private void LateUpdate()
        {
            // currentGeneration = currentGeneration.OrderByDescending(a => a.fitness).ToList();
            // currentGeneration.Take(Mathf.CeilToInt(currentGeneration.Count * 0.1f)).ToList().ForEach(a => a.EnableRenderer(true));
            // currentGeneration.GetRange(Mathf.CeilToInt(currentGeneration.Count * 0.1f), Mathf.CeilToInt(currentGeneration.Count * 0.9f)).ToList().ForEach(a => a.EnableRenderer(false));
        }

        private void StartSimulation()
        {
            trainingNo++;
            TxtTraining.text = trainingNo.ToString();
            foreach (var agent in currentGeneration)
            {
                agent.Reset(GetRandomPosition(), trainingNo == 1);
                agent.Activate();
            }
            MinFitness = Mathf.Infinity;
            MaxFitness = Mathf.NegativeInfinity;
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
            //SampleAgent = new Agent(new NeuralNetwork(layers), Instantiate(PlayerPrefab), RightGoalTransform);
            //SampleAgent.EnableRenderer(true);
            //SampleAgent.isSample = true;
            //SampleAgent.Activate();

            currentGeneration = new List<Agent>();
            for (int a = 0; a < population; a++)
            {
                PlayerPrefab.transform.position = GetRandomPosition();
                currentGeneration.Add(new Agent(new FeedForwardNN(layerDefinitions), Instantiate(PlayerPrefab), RightGoalTransform));
            }

            for (int i = 0; i < currentGeneration.Count; i++)
            {
                var colliders1 = currentGeneration[i].Colliders;
                for (int c1 = 0; c1 < colliders1.Length; c1++)
                {
                    //foreach (var sampleCollider in SampleAgent.Colliders)
                    //{
                    //    Physics2D.IgnoreCollision(colliders1[c1], sampleCollider);
                    //}

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
            trainingNo = 0;
            MinFitness = Mathf.Infinity;
            MaxFitness = Mathf.NegativeInfinity;

            // Turn negative infinities to min fitness in the generation so we can calculate.
            var minGenFitness = currentGeneration.Where(a => a.fitness != Mathf.NegativeInfinity).Min(a => a.fitness);
            // currentGeneration.ForEach(a => a.fitness = a.fitness == Mathf.NegativeInfinity ? minGenFitness - 1000 : a.fitness);

            var orderedByFitness = currentGeneration.OrderByDescending(a => a.fitness);
            // var listt = orderedByFitness.ToList(); // just to debug
            var selectedAgents = orderedByFitness.Take((int)Math.Floor(currentGeneration.Count * 0.4f)).ToList();
            //SampleAgent.Brain = selectedAgents[0].Brain;

            // Get the elite to the list first
            int eliteCount = (int)Math.Floor(selectedAgents.Count * eliteRatio);
            // And new comers
            int newAgentsCount = (int)Math.Floor(selectedAgents.Count * newAgentsRatio);


            var newGenerationBrains = new List<FeedForwardNN>();

            for (int g = 0; g < eliteCount; g++)
            {
                newGenerationBrains.Add(selectedAgents[g].Brain);
            }

            for (int n = 0; n < newAgentsCount; n++)
            {
                newGenerationBrains.Add(new FeedForwardNN(layerDefinitions));
            }

            var remainingCount = (float)population - eliteCount - newAgentsCount;
            var countOfBestAgent = (remainingCount / selectedAgents.Count * 2 - 1);
            float selectiveRatio = countOfBestAgent / remainingCount; // for linear distribution
            for (int s = 0; s < selectedAgents.Count; s++)
            {
                int agentCount = Mathf.CeilToInt(selectiveRatio * (selectedAgents.Count - s));
                for (int a = 0; a < agentCount; a++)
                {
                    var parent1 = selectedAgents[s].Brain;
                    //var parent2 = SelectRandomlyByFitness(fitnessSum, minFitness);
                    FeedForwardNN childBrain = mutate(parent1);
                    newGenerationBrains.Add(childBrain);
                }
            }

            for (int v = 0; v < newGenerationBrains.Count - currentGeneration.Count; v++)
            {
                currentGeneration.Add(new Agent(new FeedForwardNN(layerDefinitions), Instantiate(PlayerPrefab), RightGoalTransform));
            }

            for (int b = 0; b < newGenerationBrains.Count; b++)
            {
                currentGeneration[b].Brain = newGenerationBrains[b];
            }

            generationNo++;
            TxtGeneration.text = generationNo.ToString();
            TxtPopulation.text = currentGeneration.Count.ToString();
        }

        public static Vector2 GetRandomPosition()
        {
            return new Vector2((float)random.NextDouble() * (MaxX - MinX) + MinX, (float)random.NextDouble() * (MaxY - MinY) + MinY);
        }

        private FeedForwardNN SelectRandomlyByFitness(List<Agent> agents, float fitnessSum, float minFitness)
        {
            double randomFitness = random.NextDouble() * fitnessSum;
            for (int f = 0; f < agents.Count; f++)
            {
                randomFitness -= agents[f].fitness;
                if (randomFitness <= 0)
                {
                    if (agents[f].fitness == 0)
                    {
                        Debug.LogError(agents[f].fitness);
                        Debug.LogError(agents[f].IsActive);
                    }
                    //Debug.LogError(f.ToString());
                    return agents[f].Brain;
                }
                if (f >= agents.Count - 1)
                    f = 0;
            }
            Debug.LogError("Selected Default!");
            return agents[random.Next(agents.Count - 1)].Brain;
        }


        // private static FeedForwardNN crossover(FeedForwardNN[] parents)
        // {
        //     var newBrain = new FeedForwardNN(parents[0].Layers);

        //     for (int l = 0; l < parents[0].Weights.Length; l++)
        //     {
        //         for (int n = 0; n < parents[0].Weights[l].GetLength(0); n++)
        //         {
        //             int parentIndex = random.Next(parents.Length);
        //             for (int w = 0; w < parents[0].Weights[l].GetLength(1); w++)
        //             {
        //                 Debug.Log(parentIndex);
        //                 newBrain.Weights[l][n, w] = random.NextDouble() > mutationRate ? FeedForwardNN.GetRandomWeight : parents[parentIndex].Weights[l][n, w];
        //             }
        //             newBrain.Biases[l][n] = random.NextDouble() > mutationRate ? FeedForwardNN.GetRandomBias : parents[parentIndex].Biases[l][n];
        //         }
        //     }
        //     return newBrain;
        // }


        private static FeedForwardNN mutate(FeedForwardNN brain)
        {
            var random = new Random();
            //return brain;
            var newBrain = new FeedForwardNN(brain.Layers);

            for (int l = 1; l < brain.Layers.Length; l++)
            {
                for (int n = 0; n < brain.Layers[l].NodeCount; n++)
                {
                    for (int w = 0; w < brain.Layers[l].Weights.ColumnCount; w++)
                    {
                        var mutBy = mutationRate * (random.NextDouble() - 0.5) * 2;
                        newBrain.Layers[l].Weights[n, w] = brain.Layers[l].Weights[n, w] * mutBy;
                    }
                    var mutByBias = mutationRate * (random.NextDouble() - 0.5) * 0.5;
                    newBrain.Layers[l].Biases[n] = brain.Layers[l].Biases[n] * mutByBias;
                }
            }
            return newBrain;
        }
    }
}