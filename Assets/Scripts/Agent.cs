
using System;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts
{
    public class Agent
    {
        public FeedForwardNN Brain;
        public float fitness;
        public bool isSample = false;

        private Player Player;
        private Vector2 BallInitialPosition;
        private Vector2 RightGoalPosition;

        bool fitnessCanIncrease;
        bool shotTheBall;
        bool shotTheBallRewarded;
        bool touchedRightGoal;
        bool touchingRightGoal;
        bool touchedBall;
        bool touchingBall;
        bool touchingCorner;
        bool hasMaxFitness;

        public bool IsActive { get; private set; }

        public Agent(FeedForwardNN brain, Player player, Transform rightGoalTransform)
        {
            Brain = brain;
            Player = player;
            Player.rightGoalTransform = rightGoalTransform;
            BallInitialPosition = Player.ballInitialPosition;
            RightGoalPosition = rightGoalTransform.position;
            fitness = Mathf.NegativeInfinity;
        }

        public void EnableRenderer(bool enable)
        {
            Player.EnableRenderer(enable);
        }

        // We want to reuse this object instead of creating new ones each generation.
        public void Reset(Vector2 playerPos, bool resetFitness = false, FeedForwardNN brain = null)
        {
            Brain = brain ?? Brain;
            fitness = resetFitness ? Mathf.NegativeInfinity : fitness;
            hasMaxFitness = false;
            shotTheBall = false;
            shotTheBallRewarded = false;
            fitnessCanIncrease = false;
            touchedRightGoal = false;
            touchingRightGoal = false;
            touchedBall = false;
            touchingCorner = false;
            Player.Reset(playerPos);
            BallInitialPosition = Player.ballInitialPosition;
        }

        public Collider2D[] Colliders => new[] { Player.gameObject.GetComponent<Collider2D>(), Player.ballCollider };

        public void Activate()
        {
            if (IsActive)
                return;
            Player.OnInput += Player_OnInput;
            Player.OnShootTheBall += Player_OnShootTheBall;
            Player.OnRightGoalCollisionEnter += Player_OnRightGoalCollisionEnter;
            Player.OnRightGoalCollisionStay += Player_OnRightGoalCollisionStay;
            Player.OnBallCollisionEnter += Player_OnBallCollisionEnter;
            Player.OnBallCollisionStay2D += Player_OnBallCollisionStay2D;
            Player.OnCornerCollisionStay2D += Player_OnCornerCollisionStay2D;
            Player.UpdateColor();
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;
            Player.OnInput -= Player_OnInput;
            Player.OnShootTheBall -= Player_OnShootTheBall;
            Player.OnRightGoalCollisionEnter -= Player_OnRightGoalCollisionEnter;
            Player.OnRightGoalCollisionStay -= Player_OnRightGoalCollisionStay;
            Player.OnBallCollisionEnter -= Player_OnBallCollisionEnter;
            Player.OnBallCollisionStay2D -= Player_OnBallCollisionStay2D;
            Player.OnCornerCollisionStay2D -= Player_OnCornerCollisionStay2D;
            Player.HideYourself();
            IsActive = false;
        }

        private void Player_OnInput(object sender, double[] inputs)
        {
            if (!IsActive)
                return;
                
            Vector<double>.Build.DenseOfArray(inputs);
            Player.ApplyOutput(FeedForwardNN.FeedForward(Brain, Vector<double>.Build.DenseOfArray(inputs)).AsArray());
            CalculateFitness();
            //Debug.Log(fitness);
        }

        private void CalculateFitness()
        {
            if (!IsActive || isSample)
                return;
            var currentFitness = 0f;

            // // Distance to ball
            // if (touchedBall)
            // {
            //     var fitnessByDis = 1 / Vector2.Distance(Player.transform.position, Player.ballCollider.transform.position) / 10000 - 0.0001f;
            //     currentFitness += fitnessByDis;
            //     Debug.Log(fitnessByDis);
            // }
            // Take ball to the right goal
            if (touchedBall)
            {
                var fitnessByDis = Mathf.Pow(Mathf.Lerp(1, 0, Vector2.Distance(RightGoalPosition, Player.ballCollider.transform.position) / Vector2.Distance(BallInitialPosition, RightGoalPosition)), 2);
                currentFitness += fitnessByDis * (shotTheBall ? 1 : 0.5f);
            }

            if (touchedRightGoal && shotTheBall)
            {
                currentFitness += 50;
                touchedRightGoal = false;
            }

            if (shotTheBall && !shotTheBallRewarded)
            {
                currentFitness += 10;
                shotTheBallRewarded = true;
            }

            if (touchingRightGoal && shotTheBall)
            {
                currentFitness += 0.1f;
                touchingRightGoal = false;
            }

            if (touchingBall && shotTheBall)
            {
                currentFitness += 0.2f;
                touchingBall = false;
            }

            // if (touchingCorner)
            //     currentFitness -= 0.6f;

            // // If the ball got further from the goal, we count it as no achievement.
            // if (Vector2.Distance(BallInitialPosition, RightGoalPosition) < Vector2.Distance(RightGoalPosition, Player.ballCollider.transform.position)){
            //     fitness = Mathf.NegativeInfinity;
            //     fitnessCanIncrease = false;
            // }


            if (!fitnessCanIncrease)
                return;
            fitness += currentFitness;

            if (shotTheBall)
                Player.UpdateColor((fitness - Evolution.MinFitness) / (Evolution.MaxFitness - Evolution.MinFitness));
        }


        private void Player_OnShootTheBall(object sender, System.EventArgs e)
        {
            if (!IsActive || isSample)
                return;
            Player.OnShootTheBall -= Player_OnShootTheBall;
            shotTheBall = true;
            fitness = fitness == Mathf.NegativeInfinity ? 0 : fitness;
            fitnessCanIncrease = true; // this means; if didnt shot the ball, player gets no point.
        }

        private void Player_OnRightGoalCollisionEnter(object sender, System.EventArgs e)
        {
            if (!IsActive || isSample)
                return;
            Player.OnRightGoalCollisionEnter -= Player_OnRightGoalCollisionEnter;
            touchedRightGoal = true;
        }

        private void Player_OnRightGoalCollisionStay(object sender, System.EventArgs e)
        {
            if (!IsActive || isSample)
                return;
            touchingRightGoal = true;
        }

        private void Player_OnBallCollisionEnter(object sender, System.EventArgs e)
        {
            if (!IsActive || isSample)
                return;
            Player.OnBallCollisionEnter -= Player_OnBallCollisionEnter;
            touchedBall = true;
            fitness = fitness == Mathf.NegativeInfinity ? 10 : fitness;
            fitnessCanIncrease = true;
        }

        private void Player_OnBallCollisionStay2D(object sender, System.EventArgs e)
        {
            if (!IsActive || isSample)
                return;
            touchingBall = true;
        }

        private void Player_OnCornerCollisionStay2D(object sender, System.EventArgs e)
        {
            // if (!IsActive || isSample)
            //     return;
            // touchingCorner = true;
        }

    }
}