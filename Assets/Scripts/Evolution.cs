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


        // population = 1000;
        // mutationRate = 0.1;


        // generationNo
        // currentGeneration

        //elitPercentage: the number of best agents who go to next generation unchanged.


        // fitness function
        // mutate
        // crossover

        Agent mutate(Agent agent)
        {
            var newAgent = new Agent();

            for (int l = 0; l < agent.Brain.Weights.Length; l++)
            {
                for (int n = 0; n < agent.Brain.Weights[0].GetLength(0); n++)
                {
                    for (int w = 0; w < agent.Brain.Weights[0].GetLength(1); w++)
                    {
                        newAgent.Brain.Weights[l][n, w] = agent.Brain.Weights[l][n, w];
                    }

                }
            }

            return newAgent;
        }
    }
}
