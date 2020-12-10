using System;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts
{

    public enum EnmActivations
    {
        None,
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

            
            // Not the best solution. Init this somewhere else.
            if (layerTempVectors1 == null)
            {
                layerTempVectors1 = new Vector<double>[Layers.Length - 1];
                for (int l = 1; l < Layers.Length; l++)
                {
                    layerTempVectors1[l - 1] = Vector<double>.Build.Dense(Layers[l].NodeCount);
                }
                layerTempVectors2 = new Vector<double>[Layers.Length - 1];
                for (int l = 1; l < Layers.Length; l++)
                {
                    layerTempVectors2[l - 1] = Vector<double>.Build.Dense(Layers[l].NodeCount);
                }
                layerTempVectors3 = new Vector<double>[Layers.Length - 1];
                for (int l = 1; l < Layers.Length; l++)
                {
                    layerTempVectors3[l - 1] = Vector<double>.Build.Dense(Layers[l].NodeCount);
                }
            }
        }

        public Layer[] Layers { get; private set; }

        public static double GetRandomWeight => random.NextDouble() * 2 - 1f;
        public static double GetRandomBias => random.NextDouble() * 2 - 1f;

        public static Vector<double> FeedForward(FeedForwardNN nn, Vector<double> inputs)
        {
            return FeedForward(inputs, nn.Layers, 1);
        }

        private static Vector<double>[] layerTempVectors1 = null;
        private static Vector<double>[] layerTempVectors2 = null;
        private static Vector<double>[] layerTempVectors3 = null;
        public static Vector<double> FeedForward(Vector<double> inputs, Layer[] layers, int layerIndex)
        {
            // Profiler.BeginSample("FeedForward Sum");
            layers[layerIndex].Weights.Multiply(inputs, layerTempVectors1[layerIndex - 1]);
            layerTempVectors1[layerIndex - 1].Add(layers[layerIndex].Biases, layerTempVectors2[layerIndex - 1]);
            // Profiler.EndSample();
            // Profiler.BeginSample("FeedForward Activate");
            Activate(layerTempVectors2[layerIndex - 1], layers[layerIndex].Activation, layerTempVectors3[layerIndex - 1]);
            // Profiler.EndSample();
            if (layerIndex == layers.Length - 1)
                return layerTempVectors3[layerIndex - 1];
            return FeedForward(layerTempVectors3[layerIndex - 1], layers, layerIndex + 1);
        }

        public static void Activate(Vector<double> X, EnmActivations activation, Vector<double> result)
        {
            switch (activation)
            {
                default:
                case EnmActivations.LRelu:
                    Activations.LeakyReLU(X, 0.2, result);
                    return;
                case EnmActivations.Sigmoid:
                    Activations.Sigmoid(X, result);
                    return;
            }
        }

    }

}