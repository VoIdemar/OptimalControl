using System;

using OptimalControl.Maths.Exceptions;
using OptimalControl.Tools;

namespace OptimalControl.Maths.Common
{
    public class Vector
    {
        private int n;
        private double[] x;        
        
        public Vector(params double[] x)
        {
            this.x = x;
            this.n = x.Length;
        }
        
        public Vector(Vector v)
        {
            this.x = (double[])v.Data.Clone();
            this.n = v.Dim;
        }
        
        public Vector(int n)
        {
            this.n = n;
            this.x = new double[n];
            this.x.Fill(0.0);
        }

        public static Vector Of(params double[] x)
        {
            return new Vector(x);
        }
        
        public static Vector Of(Vector v)
        {
            return new Vector(v);
        }
        
        public double Norm()
        {
            double temp = 0;
            foreach (double c in x)
                temp += c * c;
            return Math.Sqrt(temp);
        }

        public int Dim
        {
            get { return n; }
            set { if (value > 0) n = value; }
        }

        public double this[int index]
        {
            get { return x[index]; }
        }

        public double[] Data
        {
            get { return x; }
        }
        
        public static double operator*(Vector v, Vector w)
        {
            if (v.Dim != w.Dim)
                throw new NonMatchingDimensionsException(string.Format("Dimensions not equal: {0}, {1}", v.Dim, w.Dim));
            int n = v.Dim;
            double result = 0;
            for (int i = 0; i < n; i++)
                result += v[i]*w[i];
            return result;
        }

        public static Vector operator*(double c, Vector v)
        {
            int n = v.Dim;
            double[] temp = new double[n];
            for (int i = 0; i < n; i++)
                temp[i] = c*v[i];
            return new Vector(temp);
        }

        public static Vector operator+(Vector v, Vector w)
        {
            if (v.Dim != w.Dim)
                throw new NonMatchingDimensionsException(string.Format("Dimensions not equal: {0}, {1}", v.Dim, w.Dim));
            int n = v.Dim;
            double[] temp = new double[n];
            for (int i = 0; i < n; i++)
                temp[i] = v[i] + w[i];
            return new Vector(temp);
        }

        public static Vector operator-(Vector v)
        {
            return (-1.0)*v;
        }

        public static Vector operator-(Vector v, Vector w)
        {
            if (v.Dim != w.Dim)
                throw new NonMatchingDimensionsException(string.Format("Dimensions not equal: {0}, {1}", v.Dim, w.Dim));
            int n = v.Dim;
            double[] temp = new double[n];
            for (int i = 0; i < n; i++)
                temp[i] = v[i] - w[i];
            return new Vector(temp);
        }
        
        public override string ToString()
        {
            return string.Format("({0})", string.Join(", ", x));
        }
    }
}
