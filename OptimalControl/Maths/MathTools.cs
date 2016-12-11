using System;

namespace OptimalControl.Maths
{
    public static class MathTools
    {        
        public static double[] Linspace(double a, double b, int n)
        {
            double[] grid = new double[n+1];
            grid[0] = a;
            grid[n] = b;
            double h = (b - a)/n;
            for (int i = 1; i < n; i++)
            {
                grid[i] = grid[i-1] + h;
            }
            return grid;
        }
        
        public static double Sqr(double x)
        {
            return x*x;
        }
    }
}
