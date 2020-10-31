using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event EventHandler OnBallCollisionEnter;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            OnBallCollisionEnter?.Invoke(this, null);
        }
    }
}
