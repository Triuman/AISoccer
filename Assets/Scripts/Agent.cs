using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    public NeuralNetwork Brain;
    public GameObject gameObject;

    public Agent(NeuralNetwork brain)
    {
        this.Brain = brain;
    }
}
