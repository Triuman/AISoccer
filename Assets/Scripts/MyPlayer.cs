using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class MyPlayer : MonoBehaviour
    {
        public Rigidbody2D ballRigidbody;

        private PlayerInputActions playerActions;
        new private Rigidbody2D rigidbody;

        private const float BallHitDistance = 0.65f;
        private const float BallHitForce = 100f;

        private NeuralNetwork NN;


        // Start is called before the first frame update
        void Awake()
        {
            int[] layers = { 3, 1, 2 };

            NN = new NeuralNetwork(layers);

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



        private void Shoot_performed(InputAction.CallbackContext ctx)
        {
            Debug.Log("Shoot");
            if (Vector2.Distance(transform.position, ballRigidbody.transform.position) < BallHitDistance)
            {
                ballRigidbody.AddForce((ballRigidbody.transform.position - transform.position).normalized * BallHitForce);
            }
        }

        Vector2 moveVector = new Vector2();

        private void Move_performed(InputAction.CallbackContext ctx)
        {
            moveVector = ctx.ReadValue<Vector2>();
            // Debug.Log(moveVector * 100);
        }

        // Update is called once per frame
        void Update()
        {
            rigidbody.AddForce(moveVector * 100 * Time.deltaTime);

            ApplyOutput(NeuralNetwork.FeedForward(NN, PrepareInputs()));
        }

        const float eighthPI = Mathf.PI / 8;
        const float quarterPI = Mathf.PI / 4;

        double[] PrepareInputs()
        {
            // 0-7: direction of the ball
            // 8: distance to the ball
            double[] inputs = new double[3];

            var diffVec = transform.position - ballRigidbody.transform.position;
            float angle = Mathf.Atan2(diffVec.y, diffVec.x);
            inputs[0] = Mathf.Sin(angle);
            inputs[1] = Mathf.Cos(angle);

            Debug.Log("");
            Debug.Log(inputs[0]);
            Debug.Log(inputs[1]);
            // Distance
            inputs[2] = Vector2.Distance(transform.position, ballRigidbody.transform.position) / 16f;

            return inputs;
        }

        void ApplyOutput(double[] output)
        {
            // 0: acc x
            // 1: acc y
            moveVector[0] = Mathf.Asin((float)output[0] - 0.5f);
            moveVector[1] = Mathf.Acos((float)output[1] - 0.5f);
            Debug.Log(output[0]);
            Debug.Log(output[1]);
            Debug.Log(moveVector[0]);
            Debug.Log(moveVector[1]);
        }
    }
}