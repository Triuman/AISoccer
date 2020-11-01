
using UnityEngine;

namespace Assets.Scripts
{
    public class Agent
    {
        public NeuralNetwork Brain;
        private Player Player;
        public float fitness;

        public bool IsActive { get; private set; }

        public Agent(NeuralNetwork brain, Player player)
        {
            Brain = brain;
            Player = player;
            fitness = 0;
        }

        // We want to reuse this object instead of creating new ones each generation.
        public void Reset(Vector2 playerPos, NeuralNetwork brain = null)
        {
            Brain = brain ?? Brain;
            fitness = 0;
            Player.Reset(playerPos);
        }

        public Collider2D[] Colliders => new[] { Player.gameObject.GetComponent<Collider2D>(), Player.ballCollider };

        public void Activate()
        {
            Player.OnInput += Player_OnInput;
            Player.OnBallCollisionEnter += Player_OnBallCollisionEnter;
            Player.OnBallCollisionStay2D += Player_OnBallCollisionStay2D;
            IsActive = true;
        }
        public void Deactivate()
        {
            Player.OnInput -= Player_OnInput;
            Player.OnBallCollisionEnter -= Player_OnBallCollisionEnter;
            Player.OnBallCollisionStay2D -= Player_OnBallCollisionStay2D;
            IsActive = false;
        }

        private void Player_OnInput(object sender, double[] inputs)
        {
            Player.ApplyOutput(NeuralNetwork.FeedForward(Brain, inputs));
            CalculateApproximityFitness();
            //Debug.Log(fitness);
        }

        private void CalculateApproximityFitness()
        {
            fitness += 1 / Vector2.Distance(Player.transform.position, Player.ballCollider.transform.position) / 10000 - 0.0005f;
        }

        private void Player_OnBallCollisionEnter(object sender, System.EventArgs e)
        {
            fitness += 5;
        }

        private void Player_OnBallCollisionStay2D(object sender, System.EventArgs e)
        {
            fitness += 0.2f;
        }

    }
}