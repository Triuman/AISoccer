
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Agent
    {
        public NeuralNetwork Brain;
        private Player Player;
        private Vector2 RightGoalPosition;
        public float fitness;

        public bool IsActive { get; private set; }

        public Agent(NeuralNetwork brain, Player player, Transform rightGoalTransform)
        {
            Brain = brain;
            Player = player;
            Player.rightGoalTransform = rightGoalTransform;
            RightGoalPosition = rightGoalTransform.position;
            fitness = 0;
        }

        public void EnableRenderer(bool enable)
        {
            Player.GetComponent<SpriteRenderer>().enabled = enable;
            Player.ballCollider.gameObject.GetComponent<SpriteRenderer>().enabled = enable;
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
            if (IsActive)
                return;
            Player.OnInput += Player_OnInput;
            Player.OnBallCollisionEnter += Player_OnBallCollisionEnter;
            Player.OnBallCollisionStay2D += Player_OnBallCollisionStay2D;
            Player.OnCornerCollisionStay2D += Player_OnCornerCollisionStay2D;
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;
            Player.OnInput -= Player_OnInput;
            Player.OnBallCollisionEnter -= Player_OnBallCollisionEnter;
            Player.OnBallCollisionStay2D -= Player_OnBallCollisionStay2D;
            Player.OnCornerCollisionStay2D -= Player_OnCornerCollisionStay2D;
            IsActive = false;
        }

        private void Player_OnInput(object sender, double[] inputs)
        {
            if (!IsActive)
                return;
            Player.ApplyOutput(NeuralNetwork.FeedForward(Brain, inputs));
            CalculateApproximityFitness();
            //Debug.Log(fitness);
        }

        bool touchedTheBall = false;
        bool hasMaxFitness = false;
        private void CalculateApproximityFitness()
        {
            if (!IsActive)
                return;
            // Go to ball
            fitness += 1 / Vector2.Distance(Player.transform.position, Player.ballCollider.transform.position) / 10000 - 0.0005f;
            // Take ball to the right goal
            if (touchedTheBall)
            {
                var distanceBallToRightGoal = Vector2.Distance(RightGoalPosition, Player.ballCollider.transform.position);
                var distanceFitnessRatio = 0.5f - 1 / (float)(1 + Math.Pow(Math.E, -distanceBallToRightGoal + 5)); // This gives a number between +0.5 and -0.5 using sigmoid.
                fitness += distanceFitnessRatio * 0.5f;
            }
            else
            {
                fitness -= 0.02f;
            }
            if (hasMaxFitness && Evolution.MaxFitness > fitness)
            {
                hasMaxFitness = false;
                Player.HideYourself();
            }
            else if (!hasMaxFitness && Evolution.MaxFitness <= fitness)
            {
                Evolution.MaxFitness = fitness;
                hasMaxFitness = true;
                Player.ShowYourself();
            }
        }

        private void Player_OnBallCollisionEnter(object sender, System.EventArgs e)
        {
            if (!IsActive)
                return;
            touchedTheBall = true;
               fitness += 2;
        }

        private void Player_OnBallCollisionStay2D(object sender, System.EventArgs e)
        {
            if (!IsActive)
                return;
            fitness += 0.002f;
        }

        private void Player_OnCornerCollisionStay2D(object sender, System.EventArgs e)
        {
            if (!IsActive)
                return;
            fitness -= 1f;
        }

    }
}