using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Assets.Scripts
{
    public class FeedForwardNN
    {
        static Random random = new Random(Guid.NewGuid().GetHashCode());

        public FeedForwardNN(int[] layers)
        {
            Layers = layers;
            Weights = new Matrix<double>[layers.Length - 1];
            Biases = new Vector<double>[layers.Length - 1];

            for (int i = 0; i < layers.Length - 1; i++)
            {
                Weights[i] = Matrix<double>.Build.Random(layers[i+1], layers[i]);
                Biases[i] = Vector<double>.Build.Random(layers[i+1]);
            }
        }

        public int[] Layers { get; private set; }

        public Matrix<double>[] Weights { get; set; }
        public Vector<double>[] Biases { get; set; }

        public static double GetRandomWeight => random.NextDouble() * 2 - 1f;
        public static double GetRandomBias => random.NextDouble() * 2 - 1f;

        public static Vector<double> FeedForward(FeedForwardNN nn, Vector<double> inputs)
        {
            return FeedForward(inputs, nn.Weights, nn.Biases, 0);
        }
        public static Vector<double> FeedForward(Vector<double> inputs, Matrix<double>[] weights, Vector<double>[] biases, int layer)
        {
            var output = Tanh(weights[layer].Multiply(inputs).Add(biases[layer]));
            if (layer == biases.Length - 1)
                return output;
            return FeedForward(output, weights, biases, layer + 1);
        }

        public static Vector<double> Tanh(Vector<double> X)
        {
            return X.Map<double>(x => Math.Tanh(x));
        }

        public static Vector<double> Sigmoid(Vector<double> X)
        {
            return X.Map<double>(x =>  Sigmoid(x));
        }
        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        public static double LogSigmoid(double x)
        {
            if (x < -45.0) return 0.0;
            else if (x > 45.0) return 1.0;
            else return Sigmoid(x);
        }

        public static double HyperbolicTangtent(double x)
        {
            if (x < -45.0) return -1.0;
            else if (x > 45.0) return 1.0;
            else return Math.Tanh(x);
        }
    }

}