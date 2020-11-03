using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public GameObject BallPrefab;

        public Transform rightGoalTransform;

        public event EventHandler OnBallCollisionEnter;
        public event EventHandler OnBallCollisionStay2D;
        public event EventHandler OnCornerCollisionStay2D;
        public event EventHandler<double[]> OnInput;

        public Collider2D ballCollider { get; private set; }
        private Rigidbody2D ballRigidbody;

        private PlayerInputActions playerActions;
        new private Rigidbody2D rigidbody;

        private const float BallHitDistance = 0.65f;
        private const float BallHitForce = 100f;

        private Vector2 moveVector = new Vector2();

        // Start is called before the first frame update
        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            BallPrefab.transform.position = Evolution.GetRandomPosition();
            var ball = Instantiate(BallPrefab);
            ballRigidbody = ball.GetComponent<Rigidbody2D>();
            ballCollider = ball.GetComponent<Collider2D>();

            playerActions = new PlayerInputActions();
            playerActions.Player.Move.performed += Move_performed;
            playerActions.Player.Move.Enable();
            playerActions.Player.Shoot.performed += Shoot_performed;
            playerActions.Player.Shoot.Enable();
        }

        // We want to reuse this object instead of creating new ones each generation.
        public void Reset(Vector2 playerPos)
        {
            transform.position = playerPos;
            ballRigidbody.transform.position = Evolution.GetRandomPosition();
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
            if (OnInput != null)
                OnInput.Invoke(this, PrepareInputs());
            rigidbody.AddForce(moveVector * 100 * Time.deltaTime);
        }


        public double[] PrepareInputs()
        {
            // 0,1: direction of the ball
            // 8: distance to the ball
            double[] inputs = new double[6];

            // Player to Ball
            var diffVec = transform.position - ballRigidbody.transform.position;
            float angle = Mathf.Atan2(diffVec.y, diffVec.x);
            inputs[0] = Mathf.Sin(angle) * 10;
            inputs[1] = Mathf.Cos(angle) * 10;

            // Distance
            inputs[2] = Vector2.Distance(transform.position, ballRigidbody.transform.position);

            // Ball to Right Goal
            diffVec = ballRigidbody.transform.position - rightGoalTransform.position;
            angle = Mathf.Atan2(diffVec.y, diffVec.x);
            inputs[3] = Mathf.Sin(angle) * 10;
            inputs[4] = Mathf.Cos(angle) * 10;

            // Distance
            inputs[5] = Vector2.Distance(ballRigidbody.transform.position, rightGoalTransform.position);

            return inputs;
        }

        public void ApplyOutput(double[] output)
        {
            // 0: acc x
            // 1: acc y
            moveVector[0] = (float)output[0] - 0.5f;
            moveVector[1] = (float)output[1] - 0.5f;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Ball"))
            {
                OnBallCollisionEnter?.Invoke(this, null);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Ball"))
            {
                OnBallCollisionStay2D?.Invoke(this, null);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Corner"))
            {
                OnCornerCollisionStay2D?.Invoke(this, null);
            }
        }

        public void ShowYourself()
        {
            GetComponent<Renderer>().material.SetColor("BodyColor", Color.white);
        }

        public void HideYourself()
        {
            GetComponent<Renderer>().material.SetColor("BodyColor", new Color32(58,58,58, 255));
        }
    }
}
