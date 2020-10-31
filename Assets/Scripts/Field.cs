using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    // Field keeps the state of every simulation for one agent.

    public Collider2D[] colliders;

    public GameObject playerPrefab;
    public GameObject ballPrefab;

    public Agent agent;
    public GameObject ball;

    private void Start()
    {
        agent.gameObject = Instantiate(playerPrefab, transform);
        ball = Instantiate(ballPrefab, transform);
    }

}
