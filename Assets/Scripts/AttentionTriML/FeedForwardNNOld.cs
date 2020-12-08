using System;

namespace Assets.Scripts
{
    public class FeedForwardNNOld
    {
        static Random random = new Random(Guid.NewGuid().GetHashCode());

        public FeedForwardNNOld(int[] layers)
        {
            Layers = layers;
            Weights = new double[layers.Length - 1][,];
            Biases = new double[layers.Length - 1][];

            for (int i = 0; i < layers.Length - 1; i++)
            {
                Weights[i] = new double[layers[i+1], layers[i]];
                Biases[i] = new double[layers[i+1]];

                for (int a = 0; a < layers[i+1]; a++)
                {
                    for (int b = 0; b < layers[i]; b++)
                    {
                        Weights[i][a, b] = GetRandomWeight;
                    }
                    Biases[i][a] = GetRandomBias;
                }
            }
        }

        public int[] Layers { get; private set; }

        public double[][,] Weights { get; set; }
        public double[][] Biases { get; set; }

        public static double GetRandomWeight => random.NextDouble() * 2 - 1f;
        public static double GetRandomBias => random.NextDouble() * 2 - 1f;

        public static double[] FeedForward(FeedForwardNNOld nn, double[] inputs)
        {
            return FeedForward(inputs, nn.Weights, nn.Biases, 0);
        }
        public static double[] FeedForward(double[] inputs, double [][,] weights, double [][] biases, int layer)
        {
            if (layer == biases.Length - 1)
                return Tanh(MatrixOld.add(MatrixOld.multiply(weights[layer], inputs), biases[layer]));
            return FeedForward(Tanh(MatrixOld.add(MatrixOld.multiply(weights[layer], inputs), biases[layer])), weights, biases, layer + 1);
        }

        public static double[] Tanh(double[] X)
        {
            var result = new double[X.Length];
            for (int i = 0; i < X.Length; i++)
            {
                result[i] = Math.Tanh(X[i]);
            }
            return result;
        }


        public static double[] Sigmoid(double[] X)
        {
            var result = new double[X.Length];
            for (int i = 0; i < X.Length; i++)
            {
                result[i] = 1.0 / (1.0 + Math.Exp(-X[i]));
            }
            return result;
        }
        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        public static double LogSigmoid(double x)
        {
            if (x < -45.0) return 0.0;
            else if (x > 45.0) return 1.0;
            else return 1.0 / (1.0 + Math.Exp(-x));
        }

        public static double HyperbolicTangtent(double x)
        {
            if (x < -45.0) return -1.0;
            else if (x > 45.0) return 1.0;
            else return Math.Tanh(x);
        }
    }

}