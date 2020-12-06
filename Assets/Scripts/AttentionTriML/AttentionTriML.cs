



// 3 FFNN(no hidden layer) for Q,K,V x N times
// 1 FFNN(no hidden layer) after concat
// 1 FFNN(arbitrary number of hidden layers) at the end


// Repeat this N times iterating over FFNN array
//      Q = FFNN(X), K = FFNN(X), V = FFNN(X)
//      Attention(Q,K,V) = MatMul(SoftMax(MatMul(Q,K)/dimensionQ), V)
// normedY = Normalize(FFNN(Concat(Attention[])) + X)
// Normalize(FFNN(normedY) + normedY)
// After this I might just concat the outputs and use another FFNN to get desired input shape.
