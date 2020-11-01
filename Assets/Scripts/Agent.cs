
using UnityEngine;

namespace Assets.Scripts
{
    public class Agent
    {
        public NeuralNetwork Brain;
        private Player Player;
        public float fitness = 0;

        public bool isActive = false;

        public Agent(NeuralNetwork brain, Player player)
        {
            Brain = brain;
            Player = player;

            Player.OnInput += Player_OnInput;
            Player.OnBallCollisionEnter += Player_OnBallCollisionEnter;
        }

        public Collider2D[] Colliders => new[] { Player.gameObject.GetComponent<Collider2D>(), Player.ballCollider };

        private void Player_OnInput(object sender, double[] inputs)
        {
            if (isActive)
            {
                Player.ApplyOutput(NeuralNetwork.FeedForward(Brain, inputs));
            }
        }

        private void Player_OnBallCollisionEnter(object sender, System.EventArgs e)
        {
            // TODO: calculate fitness
        }

    }
}