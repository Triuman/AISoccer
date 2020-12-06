using System;

class Matrix
{

    public static double[] add(double[] v1, double[] v2)
    {
        double[] result = new double[v1.Length];
        for (int i = 0; i < v1.Length; i++)
        {
            result[i] = v1[i] + v2[i];
        }
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="m">dim: axb</param>
    /// <param name="v">dim: b</param>
    /// <returns>dim: a</returns>
    public static double[] multiply(double[,] m, double[] v)
    {
        // row x column
        // m axb    v b 	result a
        int a = m.GetLength(0);
        int b = v.Length;
        int c = 1;

        var result = new double[a];
        for (var cc = 0; cc < c; cc++)
        {
            for (var aa = 0; aa < a; aa++)
            {
                for (var bb = 0; bb < b; bb++)
                {
                    result[aa] += m[aa, bb] * v[bb];
                }
            }
        }

        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="m1">dim: axb</param>
    /// <param name="m2">dim: bxc</param>
    /// <returns>dim: axc</returns>
    public static double[,] multiply(double[,] m1, double[,] m2)
    {
        // row x column
        // m1 axb    m2 bxc 	result axc
        int a = m1.GetLength(0);
        int b = m1.GetLength(1);
        int c = m2.GetLength(1);

        var result = new double[a, c];
        for (var cc = 0; cc < c; cc++)
        {
            for (var aa = 0; aa < a; aa++)
            {
                for (var bb = 0; bb < b; bb++)
                {
                    result[aa, cc] += m1[aa, bb] * m2[bb, cc];
                }
            }
        }
        return result;
    }

    public static double dotProduct(double[] v1, double[] v2)
    {
        double sum = 0;
        for (var i = 0; i < v1.Length; i++)
        {
            sum += v1[i] * v2[i];
        }
        return sum;
    }

    public static void printMatrix(double[,] m)
    {
        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
            {
                Console.Write(string.Format("{0} ", m[i, j]));
            }
            Console.Write(Environment.NewLine);
        }
    }
    public static void printVector(double[] v)
    {
        for (int i = 0; i < v.Length; i++)
        {
            Console.WriteLine(string.Format("{0} ", v[i]));
        }
    }

}

