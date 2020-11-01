
namespace Assets.Scripts
{
    public class Agent
    {
        public NeuralNetwork Brain;
        public float fitness = 0;

        public Agent(NeuralNetwork brain)
        {
            this.Brain = brain;
        }

        public double[] ProcessesInputs(double[] inputs)
        {
           return NeuralNetwork.FeedForward(Brain, inputs);
        }

    }
}