using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    public class Field : MonoBehaviour
    {
        // Field keeps the state of every simulation for one agent.

        public Collider2D[] colliders;

        public Player playerPrefab;
        public GameObject ballPrefab;

        public Agent agent;
        public GameObject ball;

        private void Start()
        {
            agent.Player = Instantiate(playerPrefab, transform);
            ball = Instantiate(ballPrefab, transform);
        }

    }
}