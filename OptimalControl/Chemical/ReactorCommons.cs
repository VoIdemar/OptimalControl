using System;

using log4net;

using OptimalControl.Tools;
using OptimalControl.Maths;
using OptimalControl.Maths.Common;

namespace OptimalControl.Chemical
{
    public static class ReactorCommons
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReactorCommons));
        
        public static readonly int[] M = {18, 84, 56, 42, 28, 92, 16};
        
        public static readonly double[] U = {15.19, 8.18, 13.198, 3.543, 4723.7, 423.7, 204.41,
                                             1.466E-6, 0.013, 0.09, 5.428E-6, 0.024, 5.92E-6};
        
        public static readonly double[] E = {25E+3, 25E+3, 25E+3, 25E+3, 40E+3, 40E+3, 40E+3,
                                             20E+3, 20E+3, 20E+3, 20E+3, 20E+3, 20E+3};
        
        public static readonly double[] Q = {0.0, 7.8, 14.0, 14.0};
        
        public static readonly double
            G_STEAM = 3500.0,
              G_GASOLINE = 1750.0;
        
        public static readonly double 
            MIN_DIST = 0.0,
            MAX_DIST = 180.0,
            MIN_GRAD_VALUE = 0.0,
            MAX_GRAD_VALUE = 100.0;
                             
        public static readonly double
            DIFF_PRECISION = 1E-5,
            OPTIMIZATION_PRECISION = 1E-6,
            GRAD_OPT_PRECISION = 1E-5;
            
        public static readonly int
            MAX_NUMBER_OF_STEPS = 1000;
                
        public static readonly Vector
            INITIAL_X7_PARAMS = Vector.Of(0.5, 300),
            MIN_X7_PARAMS = Vector.Of(0.0, 100),
            MAX_X7_PARAMS = Vector.Of(10.0, 1400);
        
        public static double p(double t) 
        {
            return 5.0 - t/60.0;    
        }

        public static double F(int i, params double[] args)
        {
            switch (i)
            {
                case 1:
                    return F1(args);
                case 2:
                    return F2(args);
                case 3:
                    return F3(args);
                case 4:
                    return F4(args);
                case 5:
                    return F5(args);
                case 6:
                    return F6(args);
                default:
                    throw new ArgumentException(string.Format("F{0} is not defined", i));
            }
        }
        
        public static double F1(params double[] args)
        {
            ReactorFunctionArguments X = new ReactorFunctionArguments(args);
            return -(R(1, X.x7) + R(2, X.x7) + R(3, X.x7) + R(4, X.x7))*X.x1*V(args);
        }
        
        public static double F2(params double[] args)
        {
            ReactorFunctionArguments X = new ReactorFunctionArguments(args);
            return (R(3, X.x7)*X.x1 - (R(6, X.x7) + R(7, X.x7) + R(10, X.x7) + R(13, X.x7))*X.x2)*V(args);
        }
        
        public static double F3(params double[] args)
        {
            ReactorFunctionArguments X = new ReactorFunctionArguments(args);
            return (R(2, X.x7)*X.x1 + R(6, X.x7)*X.x2 - (R(5, X.x7) + R(9, X.x7) + R(12, X.x7))*X.x3)*V(args);
        }
        
        public static double F4(params double[] args)
        {
            ReactorFunctionArguments X = new ReactorFunctionArguments(args);
            return (R(1, X.x7)*X.x1 + R(7, X.x7)*X.x2 + R(5, X.x7)*X.x3 - (R(8, X.x7) + R(11, X.x7))*X.x4)*V(args);
        }
        
        public static double F5(params double[] args)
        {
            ReactorFunctionArguments X = new ReactorFunctionArguments(args);
            return (R(10, X.x7)*X.x2 + R(9, X.x7)*X.x3 + R(8, X.x7)*X.x4)*V(args);
        }
        
        public static double F6(params double[] args)
        {
            ReactorFunctionArguments X = new ReactorFunctionArguments(args);
            return (R(4, X.x7)*X.x1 + R(13, X.x7)*X.x2 + R(12, X.x7)*X.x3 + R(11, X.x7)*X.x4)*V(args);
        }
        
        public static double V(params double[] args)
        {
            ReactorFunctionArguments X = new ReactorFunctionArguments(args);
            double s1 = 0;
            for (int i = 1; i <= 6; i++)
            {
                s1 += (M[i]*X.x(i));
            }
            double s2 = X.x7*(G_GASOLINE + G_STEAM*s1/M[0]);
            return 509.209*p(X.t)*s1/s2;
        }
        
        public static double QualityCriteria(params double[] args)
        {
            ReactorFunctionArguments X = new ReactorFunctionArguments(args);
            if (log.IsDebugEnabled)
                log.Debug("QualityCriteria: " + args.GetContentsString());
            double upperSum = 0;
            for (int i = 2; i <= 4; i++)
            {
                //upperSum += (Q[i-1]*M[i]*F(i, args));
                upperSum += (Q[i-1]*M[i]*X.x(i));
            }
            double lowerSum = 0;
            for (int i = 1; i <= 6; i++)
            {
                lowerSum += (M[i]*X.x(i));
            }
            if (log.IsDebugEnabled)
                log.DebugFormat("QualityCriteria: upperSum = {0}, lowerSum = {1}", upperSum, lowerSum);
            return upperSum/lowerSum;
        }
        
        public static readonly ParametrizedFunction X7 = new ParametrizedFunction(
            args =>
            {
                double t = args[0], a = args[1], b = args[2];
                if (0 <= t && t < 90) return (b-373.0)*t/90.0 + 373.0;
                else return (1500.0-b)*Math.Pow((t-90.0)/90.0, a) + b;
//                double t = args[0], a = args[1];
//                return 373.0 + 1127.0*Math.Pow(t/180.0, a);
//                double t = args[0], a = args[1];
//                return 1127.0*Math.Pow(t/180.0, a) + 373.0;
//                double t = args[0], a = args[1];
//                return 1100 + a;
            }, 2
        );
        
        public static double R(int i, double x7) 
        {
            return U[i-1]*Math.Exp(23.0 - E[i-1]/x7);
        }    

        private class ReactorFunctionArguments
        {                
            public ReactorFunctionArguments(params double[] args)
            {
                int length = args.Length;
                t = args[0];
                if (length >= 2) x1 = args[1];
                if (length >= 3) x2 = args[2];
                if (length >= 4) x3 = args[3];
                if (length >= 5) x4 = args[4];
                if (length >= 6) x5 = args[5];
                if (length >= 7) x6 = args[6];
                x7 = X7.Eval(t);
            }
            
            public double t
            {
                get; set;
            }
            
            public double x1
            {
                get; set;
            }
            
            public double x2
            {
                get; set;
            }
            
            public double x3
            {
                get; set;
            }
            
            public double x4
            {
                get; set;
            }
            
            public double x5
            {
                get; set;
            }
            public double x6
            {
                get; set;
            }
            
            public double x7
            {
                get; set;
            }
            
            public double x(int i)
            {
                switch (i)
                {
                    case 1:
                        return x1;
                    case 2:
                        return x2;
                    case 3:
                        return x3;
                    case 4:
                        return x4;
                    case 5:
                        return x5;
                    case 6:
                        return x6;
                    case 7:
                        return x7;
                    default:
                        throw new ArgumentException(string.Format("x{0} is not defined", i));
                }
            }
        }
    }
}
