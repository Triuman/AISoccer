using System;
using System.Diagnostics;
// using System.Numerics.Tensors;
// using MathNet.Numerics.LinearAlgebra;
// using MathNet.Numerics.LinearAlgebra.Double;

namespace Assets.Scripts
{

    public class AttentiveBrain
    {
        public FeedForwardNNOld[,] linears; // 3 FFNN(no hidden layer) for Q,K,V x N times
        public FeedForwardNNOld linear; // 1 FFNN(no hidden layer) after concat
        public FeedForwardNNOld reLu1; // 1 FFNN(arbitrary number of hidden layers)
        public FeedForwardNNOld reLu2; // 1 FFNN(arbitrary number of hidden layers) at the end

        public AttentiveBrain(FeedForwardNNOld[,] linears, FeedForwardNNOld linear, FeedForwardNNOld reLu1, FeedForwardNNOld reLu2)
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
            // var denseTensor = new DenseTensor<int>(new[] { 3, 5 });
            // var asd = denseTensor[1, 4];

            // var m = new DenseMatrix(3);
            
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
            double[][,] attentions = new double[attentiveBrain.linears.GetLength(0)][,];
            for (int l = 0; l < attentiveBrain.linears.GetLength(0); l++)
            {
                var linearQ = attentiveBrain.linears[l, 0];
                var linearK = attentiveBrain.linears[l, 1];
                var linearV = attentiveBrain.linears[l, 2];
                double[,] Q = new double[inputCount, linearQ.Layers[linearQ.Layers.Length - 1]];
                double[,] K = new double[inputCount, linearK.Layers[linearK.Layers.Length - 1]];
                double[,] V = new double[inputCount, linearV.Layers[linearV.Layers.Length - 1]];
                for (int i = 0; i < inputCount; i++)
                {
                    double[] input = new double[inputSize];
                    Buffer.BlockCopy(inputs, i * inputSize * sizeOfDouble, input, 0, inputSize * sizeOfDouble);
                    var q = FeedForwardNNOld.FeedForward(linearQ, input);
                    for (int iq = 0; iq < q.Length; iq++)
                    {
                        Q[i, iq] = q[iq];
                    }
                    var k = FeedForwardNNOld.FeedForward(linearK, input);
                    for (int ik = 0; ik < k.Length; ik++)
                    {
                        K[i, ik] = k[ik];
                    }
                    var v = FeedForwardNNOld.FeedForward(linearV, input);
                    for (int iv = 0; iv < v.Length; iv++)
                    {
                        V[i, iv] = v[iv];
                    }
                }
                var matmulQK = MatrixOld.multiply(Q, K); // dim: inputCount x inputCount
                var sqrtDim = Math.Sqrt(Q.GetLength(1));
                // var scaled = Matrix.divide(matmulQK, sqrtDim);
                // var softmaxQK = Matrix.softmax(scaled);
                // var Z = Matrix.matmul(softmaxQK, V);
                // attentions[l] = Z;
            }

            // double[,] attentionsConcat = Matrix.concat(attentions);
            // var output = FeedForwardNN.FeedForward(attentiveBrain.reLu1, attentionsConcat);

        }


    }
}