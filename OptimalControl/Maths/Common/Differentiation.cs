using System;

namespace OptimalControl.Maths.Common
{
    public static class Differentiation
    {
        public static Vector Gradient(Function f, double e, params double[] x)
        {
            double[] gradComponents = new double[x.Length];
            double[] pArg = (double[])x.Clone(), mArg = (double[])x.Clone();
            for (int i = 0; i < gradComponents.Length; i++)
            {
                pArg[i] += e;
                mArg[i] -= e;            
                gradComponents[i] = (f(pArg) - f(mArg))/(2.0*e);
                pArg[i] = x[i];
                mArg[i] = x[i];
            }
            return Vector.Of(gradComponents);
        }
        
        public static Vector Gradient(Function f, double e, Vector x)
        {
            return Gradient(f, e, x.Data);
        }
    }
}
