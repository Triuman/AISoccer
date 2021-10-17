using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public event EventHandler OnRightGoalCollisionEnter;
    public event EventHandler OnRightGoalCollisionStay;
    public event EventHandler OnLeftGoalCollisionEnter;
    public event EventHandler OnLeftGoalCollisionStay;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("RightGoal"))
        {
            OnRightGoalCollisionEnter?.Invoke(this, null);
        }
        else
        if (collider.CompareTag("LeftGoal"))
        {
            OnLeftGoalCollisionEnter?.Invoke(this, null);
        }
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("RightGoal"))
        {
            OnRightGoalCollisionStay?.Invoke(this, null);
        }
        else
        if (collider.CompareTag("LeftGoal"))
        {
            OnRightGoalCollisionStay?.Invoke(this, null);
        }
    }
}
