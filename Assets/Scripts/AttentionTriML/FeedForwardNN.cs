using System;
using System.Collections;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts
{

    public enum EnmActivations
    {
        LRelu,
        Sigmoid
    }


    public class LayerDefinition
    {
        public int NodeCount { get; }
        public EnmActivations Activation { get; }

        public LayerDefinition(int nodeCount, EnmActivations activation)
        {
            NodeCount = nodeCount;
            Activation = activation;
        }
    }

    public class Layer
    {
        public int NodeCount;
        public EnmActivations Activation;
        public Matrix<double> Weights;
        public Vector<double> Biases;

        public Layer(LayerDefinition layerDefinition, Matrix<double> weights = null, Vector<double> biases = null)
        {
            NodeCount = layerDefinition.NodeCount;
            Activation = layerDefinition.Activation;
            Weights = weights;
            Biases = biases;
        }
    }

    public class FeedForwardNN
    {
        static Random random = new Random(Guid.NewGuid().GetHashCode());

        public FeedForwardNN(LayerDefinition[] layerDefinitions)
        {
            Init(layerDefinitions);
        }
        public FeedForwardNN(Layer[] layers)
        {
            LayerDefinition[] layerDefinitions = new LayerDefinition[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                layerDefinitions[i] = new LayerDefinition(layers[i].NodeCount, layers[i].Activation);
            }
            Init(layerDefinitions);
        }

        private void Init(LayerDefinition[] layerDefinitions)
        {
            Layers = new Layer[layerDefinitions.Length];
            Layers[0] = new Layer(layerDefinitions[0]);
            for (int i = 1; i < layerDefinitions.Length; i++)
            {
                var weights = Matrix<double>.Build.Random(layerDefinitions[i].NodeCount, layerDefinitions[i - 1].NodeCount);
                var biases = Vector<double>.Build.Random(layerDefinitions[i].NodeCount);
                Layers[i] = new Layer(layerDefinitions[i], weights, biases);
            }
        }

        public Layer[] Layers { get; private set; }

        public static double GetRandomWeight => random.NextDouble() * 2 - 1f;
        public static double GetRandomBias => random.NextDouble() * 2 - 1f;

        public static Vector<double> FeedForward(FeedForwardNN nn, Vector<double> inputs)
        {
            return FeedForward(inputs, nn.Layers, 1);
        }
        public static Vector<double> FeedForward(Vector<double> inputs, Layer[] layers, int layerIndex)
        {
            var output = Activate(layers[layerIndex].Weights.Multiply(inputs).Add(layers[layerIndex].Biases), layers[layerIndex].Activation);
            if (layerIndex == layers.Length - 1)
                return output;
            return FeedForward(output, layers, layerIndex + 1);
        }

        public static Vector<double> Activate(Vector<double> X, EnmActivations activation)
        {
            switch (activation)
            {
                default:
                case EnmActivations.LRelu:
                    return Activations.LeakyReLU(X, 0.2);
            }
        }

        public static Vector<double> Tanh(Vector<double> X)
        {
            return X.Map<double>(x => Math.Tanh(x));
        }

        public static Vector<double> Sigmoid(Vector<double> X)
        {
            return X.Map<double>(x => Sigmoid(x));
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