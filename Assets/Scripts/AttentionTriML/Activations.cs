
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts
{

    public static class Activations
    {

        public static double LeakyReLU(double x, double a)
        {
            return x < 0 ? a * x : x;
        }
        public static Vector<double> LeakyReLU(Vector<double> X, double a)
        {
            return X.Map(x => LeakyReLU(x, a));
        }
        public static Matrix<double> LeakyReLU(Matrix<double> X, double a)
        {
            return X.Map(x => LeakyReLU(x, a));
        }

    }
}