using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public GameObject BallPrefab;

        public Transform rightGoalTransform;

        public event EventHandler OnShootTheBall;
        public event EventHandler OnRightGoalCollisionEnter;
        public event EventHandler OnRightGoalCollisionStay;
        public event EventHandler OnBallCollisionEnter;
        public event EventHandler OnBallCollisionStay2D;
        public event EventHandler OnCornerCollisionStay2D;
        public event EventHandler<double[]> OnInput;

        public Collider2D ballCollider { get; private set; }
        public Vector2 ballInitialPosition = new Vector2();
        private Rigidbody2D ballRigidbody;

        private PlayerInputActions playerActions;
        new private Rigidbody2D rigidbody;

        private const float BallHitDistance = 0.6f;
        private const float BallHitForce = 50f;

        private const float Speed = 20;
        private Vector2 moveVector = new Vector2();

        // Start is called before the first frame update
        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            BallPrefab.transform.position = Evolution.GetRandomPosition();
            ballInitialPosition = BallPrefab.transform.position;
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
            ballInitialPosition = ballRigidbody.transform.position;
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
            Shoot();
        }


        // User input
        private void Move_performed(InputAction.CallbackContext ctx)
        {
            moveVector = ctx.ReadValue<Vector2>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (OnInput != null)
                OnInput.Invoke(this, PrepareInputs());
            rigidbody.AddForce(moveVector * Speed * Time.deltaTime);
        }


        public double[] PrepareInputs()
        {
            // 0,1: direction of the ball
            // 8: distance to the ball
            double[] inputs = new double[4];

            // Player to Ball
            var diffVec = transform.position - ballRigidbody.transform.position;
            float angle = Mathf.Atan2(diffVec.y, diffVec.x);
            inputs[0] = Mathf.Sin(angle);
            inputs[1] = Mathf.Cos(angle);

            // // Distance
            // inputs[0] = diffVec.x;
            // inputs[1] = diffVec.y;

            // Ball to Right Goal
            diffVec = ballRigidbody.transform.position - rightGoalTransform.position;
            angle = Mathf.Atan2(diffVec.y, diffVec.x);
            inputs[2] = Mathf.Sin(angle);
            inputs[3] = Mathf.Cos(angle);

            // // Distance
            // inputs[2] = diffVec.x;
            // inputs[3] = diffVec.y;

            return inputs;
        }

        public void ApplyOutput(double[] output)
        {
            // return;
            // 0: acc x
            // 1: acc y
            moveVector[0] = (float)output[0] - 0.5f;
            moveVector[1] = (float)output[1] - 0.5f;

            moveVector = moveVector.normalized;

            // Debug.Log(output[2]);
            if ((float)output[2] > 0.7f)
                Shoot();
        }

        private void Shoot()
        {
            // Debug.Log("Shoot");
            if (Vector2.Distance(transform.position, ballRigidbody.transform.position) < BallHitDistance)
            {
                ballRigidbody.AddForce((ballRigidbody.transform.position - transform.position).normalized * BallHitForce);
                OnShootTheBall?.Invoke(this, null);
            }
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

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("RightGoal"))
            {
                OnRightGoalCollisionEnter?.Invoke(this, null);
            }
        }
        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.CompareTag("Corner"))
            {
                OnCornerCollisionStay2D?.Invoke(this, null);
            } else if (collider.CompareTag("RightGoal"))
            {
                OnRightGoalCollisionStay?.Invoke(this, null);
            }
        }

        public void ShowYourself()
        {
            GetComponent<Renderer>().material.SetColor("BodyColor", Color.white);
        }

        public void HideYourself()
        {
            GetComponent<Renderer>().material.SetColor("BodyColor", new Color32(58, 180, 58, 255));
        }
        public void UpdateColor(float fitnessRatio)
        {
            byte color = (byte)Math.Ceiling(Mathf.Min(Mathf.Max(fitnessRatio * 255, 0), 255));
            GetComponent<Renderer>().material.SetColor("BodyColor", new Color32(color, color, color, 255));
        }
    }
}
