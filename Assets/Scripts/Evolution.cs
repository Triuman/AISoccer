using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{

    // TODO: use ignoreCollision

    class Evolution
    {

        readonly Random random = new Random(Guid.NewGuid().GetHashCode());

        //Steps:
        //Get an environment for each agent
        //Generate a new generation with the population size
        //run simulation until max time or goal reached by every agent
        //order agents by their fitness value
        //select agents randomly proportional to their fitness
        //do crossover
        //do mutation





        //Crossover probability
        //Crossover distribution
        //Mutation probability: Mutation rate (probability): this rate determines how many chromosomes should be mutated in one generation; mutation rate is in the range of [0, 1].
        //Mutation distribution


        int population = 1000;
        float mutationRate = 0.1f;


        int generationNo = 0;
        Agent[] currentGeneration = null;

        //the number of best agents who go to next generation unchanged.
        float elitRatio = 0.1f; 
        

        // fitness function
        void calculateFitness()
        {

        }

        Agent crossover(Agent[] parents)
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
            return new Agent(newBrain);
        }


        Agent mutate(Agent agent)
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
            return new Agent(newBrain);
        }
    }
}
