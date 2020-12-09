using System;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;

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
        public void Process(AttentiveBrain attentiveBrain, Vector<double>[] inputs)
        {
            // int inputCount = inputs.Length;
            // Matrix<double> attentions = null;
            // for (int l = 0; l < attentiveBrain.linears.GetLength(0); l++)
            // {
            //     var linearQ = attentiveBrain.linears[l, 0];
            //     var linearK = attentiveBrain.linears[l, 1];
            //     var linearV = attentiveBrain.linears[l, 2];
            //     Matrix<double> Q = Matrix<double>.Build.Dense(inputCount, linearQ.Layers[linearQ.Layers.Length - 1]);
            //     Matrix<double> K = Matrix<double>.Build.Dense(inputCount, linearK.Layers[linearK.Layers.Length - 1]);
            //     Matrix<double> V = Matrix<double>.Build.Dense(inputCount, linearV.Layers[linearV.Layers.Length - 1]);
            //     for (int i = 0; i < inputCount; i++)
            //     {
            //         Vector<double> input = inputs[i];
            //         var q = FeedForwardNN.FeedForward(linearQ, input);
            //         for (int iq = 0; iq < q.Count; iq++)
            //         {
            //             Q[i, iq] = q[iq];
            //         }
            //         var k = FeedForwardNN.FeedForward(linearK, input);
            //         for (int ik = 0; ik < k.Count; ik++)
            //         {
            //             K[i, ik] = k[ik];
            //         }
            //         var v = FeedForwardNN.FeedForward(linearV, input);
            //         for (int iv = 0; iv < v.Count; iv++)
            //         {
            //             V[i, iv] = v[iv];
            //         }
            //     }
            //     // matmul QxK
            //     var matmulQK = Q.TransposeAndMultiply(K); // dim: inputCount x inputCount
                
            //     // scale
            //     var sqrtDim = Math.Sqrt(Q.ColumnCount);
            //     var scaledQK = matmulQK.Divide(sqrtDim);

            //     // // softmax - only for classification
            //     // var scaledExp = scaled.PointwiseExp();
            //     // var scaledExpSumVec = scaledExp.ColumnSums();
            //     // var softmaxQK = scaledExp.PointwiseDivide(scaledExpSumVec);

            //     var Z = scaledQK.Multiply(V);
            //     attentions = attentions == null ? Z : attentions.Append(Z);
            // }

            // var output = FeedForwardNN.FeedForward(attentiveBrain.reLu1, attentions);

        }


    }
}