using Assets.Scripts;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public event EventHandler OnBallCollisionEnter;

    public Agent Agent;
    public Rigidbody2D ballRigidbody;

    private PlayerInputActions playerActions;
    new private Rigidbody2D rigidbody;

    private const float BallHitDistance = 0.65f;
    private const float BallHitForce = 100f;

    private Vector2 moveVector = new Vector2();

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        playerActions = new PlayerInputActions();
        playerActions.Player.Move.performed += Move_performed;
        playerActions.Player.Move.Enable();
        playerActions.Player.Shoot.performed += Shoot_performed;
        playerActions.Player.Shoot.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.Move.performed -= Move_performed;
        playerActions.Player.Move.Disable();
        playerActions.Player.Shoot.performed -= Shoot_performed;
        playerActions.Player.Shoot.Disable();
    }


    // User input
    private void Shoot_performed(InputAction.CallbackContext ctx)
    {
        Debug.Log("Shoot");
        if (Vector2.Distance(transform.position, ballRigidbody.transform.position) < BallHitDistance)
        {
            ballRigidbody.AddForce((ballRigidbody.transform.position - transform.position).normalized * BallHitForce);
        }
    }


    // User input
    private void Move_performed(InputAction.CallbackContext ctx)
    {
        moveVector = ctx.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyOutput(Agent.ProcessesInputs(PrepareInputs()));
        rigidbody.AddForce(moveVector * 100 * Time.deltaTime);
    }


    public double[] PrepareInputs()
    {
        // 0-7: direction of the ball
        // 8: distance to the ball
        double[] inputs = new double[3];

        var diffVec = transform.position - ballRigidbody.transform.position;
        float angle = Mathf.Atan2(diffVec.y, diffVec.x);
        inputs[0] = Mathf.Sin(angle);
        inputs[1] = Mathf.Cos(angle);

        // Distance
        inputs[2] = Vector2.Distance(transform.position, ballRigidbody.transform.position) / 16f;

        return inputs;
    }

    public void ApplyOutput(double[] output)
    {
        // 0: acc x
        // 1: acc y
        moveVector[0] = Mathf.Asin((float)output[0] - 0.5f);
        moveVector[1] = Mathf.Acos((float)output[1] - 0.5f);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            OnBallCollisionEnter?.Invoke(this, null);
        }
    }
}
