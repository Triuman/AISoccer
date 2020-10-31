
namespace Assets.Scripts
{
    public class Agent
    {
        public NeuralNetwork Brain;
        public Player Player;
        public float fitness = 0;

        public Agent(NeuralNetwork brain)
        {
            this.Brain = brain;
        }
    }
}