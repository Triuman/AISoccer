using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    public class Field : MonoBehaviour
    {
        // Field keeps the state of every simulation for one agent.

        public List<Collider2D> colliders;

        public Player playerPrefab;
        public GameObject ballPrefab;

        public Player Player;
        public GameObject ball;

        public void Init()
        {
            Player = Instantiate(playerPrefab, transform);
            AddCollider(Player.GetComponent<Collider2D>());
            ball = Instantiate(ballPrefab, transform);
            AddCollider(ball.GetComponent<Collider2D>());
            Player.ballRigidbody = ball.GetComponent<Rigidbody2D>();
        }

        public void SetAgent(Agent agent)
        {
            Player.Agent = agent;
        }

        private void AddCollider(Collider2D collider)
        {
            colliders.Add(collider);
        }

    }
}