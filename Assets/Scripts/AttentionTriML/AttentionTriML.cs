using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Scripts
{

    public class AttentiveBrain
    {
        public FeedForwardNN[,] linears; // 3 FFNN(no hidden layer) for Q,K,V x N times
        public FeedForwardNN linear; // 1 FFNN(no hidden layer) after concat
        public FeedForwardNN reLu1; // 1 FFNN(arbitrary number of hidden layers)
        public FeedForwardNN reLu2; // 1 FFNN(arbitrary number of hidden layers) at the end

        public AttentiveBrain(FeedForwardNN[,] linears, FeedForwardNN linear, FeedForwardNN reLu1, FeedForwardNN reLu2)
        {
            Debug.Assert(linears.GetLength(1) == 3);
            Debug.Assert(linear.Layers.GetLength(0) == 2);
            this.linears = linears;
            this.linear = linear;
            this.reLu1 = reLu1;
            this.reLu2 = reLu2;
        }


    }

    // TODO: use Tensor instead of Array

    public class AttentionTriML
    {

        public AttentionTriML()
        {

        }

        const int sizeOfDouble = 8;

        // Repeat this N times iterating over FFNN array
        //      Q = FFNN(X), K = FFNN(X), V = FFNN(X)
        //      Attention(Q,K,V) = MatMul(SoftMax(MatMul(Q,K)/dimensionQ), V)
        // normedY = Normalize(FFNN(Concat(Attention[])) + X)
        // Normalize(FFNN(normedY) + normedY)
        // After this I might just concat the outputs and use another FFNN to get desired input shape.
        public void Process(AttentiveBrain attentiveBrain, double[,] inputs)
        {
            int inputCount = inputs.GetLength(0);
            int inputSize = inputs.GetLength(1);
            double[,] attentions = new double[inputCount, inputSize];
            for (int l = 0; l < attentiveBrain.linears.GetLength(0); l++)
            {
                var linearQ = attentiveBrain.linears[l, 0];
                var linearK = attentiveBrain.linears[l, 1];
                var linearV = attentiveBrain.linears[l, 2];
                double[,] Q = new double[inputCount, inputSize];
                double[,] K = new double[inputCount, inputSize];
                double[,] V = new double[inputCount, inputSize];
                for (int i = 0; i < inputCount; i++)
                {
                    double[] input = new double[inputSize];
                    Buffer.BlockCopy(inputs, i * inputSize * sizeOfDouble, input, 0, inputSize * sizeOfDouble);
                    var q = FeedForwardNN.FeedForward(linearQ, input);
                    for (int iq = 0; iq < q.Length; iq++)
                    {
                        Q[i, iq] = q[iq];
                    }
                    var k = FeedForwardNN.FeedForward(linearK, input);
                    for (int ik = 0; ik < k.Length; ik++)
                    {
                        K[i, ik] = k[ik];
                    }
                    var v = FeedForwardNN.FeedForward(linearV, input);
                    for (int iv = 0; iv < v.Length; iv++)
                    {
                        V[i, iv] = v[iv];
                    }
                }
                var matmulQK = Matrix.multiply(Q, K); // dim: inputCount x inputCount
                var softmaxQK = matmulQK;
                for (int m = 0; m < inputSize; m++)
                {
                    // var softmaxResult = Matrix.softmax();
                }
            }

        }


    }
}