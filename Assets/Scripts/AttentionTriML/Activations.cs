using System;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts
{

    public static class Activations
    {

        public static double LeakyReLU(double x, double a)
        {
            return x < 0 ? a * x : x;
        }
        public static void LeakyReLU(Vector<double> X, double a, Vector<double> result)
        {
            X.Map(x => LeakyReLU(x, a), result);
        }
        public static Matrix<double> LeakyReLU(Matrix<double> X, double a)
        {
            return X.Map(x => LeakyReLU(x, a));
        }
        
        public static Vector<double> Tanh(Vector<double> X)
        {
            return X.Map<double>(x => Math.Tanh(x));
        }

        public static void Sigmoid(Vector<double> X, Vector<double> result)
        {
            X.Map(x => Sigmoid(x), result);
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