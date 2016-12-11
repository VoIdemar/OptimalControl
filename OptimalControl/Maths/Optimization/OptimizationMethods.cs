using System;

using log4net;

using OptimalControl.Maths.Common;
using Diff = OptimalControl.Maths.Common.Differentiation;

namespace OptimalControl.Maths.Optimization
{
    public static class OptimizationMethods
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OptimizationMethods));
        
        private static readonly double 
            GS_RATIO1 = (Math.Sqrt(5) + 1.0)/2.0,
            GS_RATIO2 = (Math.Sqrt(5) - 1.0)/2.0;
        
        /// <summary>
        /// Finds maximum value of the specified 1D function in the [a, b] segment.
        /// </summary>
        /// <param name="f">Function to be maximized.</param>
        /// <param name="a">Left edge of the segment.</param>
        /// <param name="b">Right edge of the segment.</param>
        /// <param name="eps">Required precision.</param>
        public static OptimizationResult<double> GoldenSectionMethod(Function f, double a, double b, double eps, OptimizationType type)
        {
            double v = a + MathTools.Sqr(GS_RATIO2)*(b-a), w = a + GS_RATIO2*(b-a);
            double fv = f(v), fw = f(w);
            double L = GS_RATIO2*(b-a);
            while (L >= eps)
            {
                if ((type == OptimizationType.MAXIMIZE && fv < fw) || 
                    (type == OptimizationType.MINIMIZE && fv > fw))
                {
                    a = v;
                    v = w;
                    fv = fw;
                    w = a + GS_RATIO2*L;
                    fw = f(w);
                }
                else
                {
                    b = w;
                    w = v;
                    fw = fv;
                    v = a + MathTools.Sqr(GS_RATIO2)*L;
                    fv = f(v);
                }
                L = GS_RATIO2*(b-a);
            }
            double res = a + L/2.0;
            return new OptimizationResult<double>(res, f(res));
        }
        
        public static OptimizationResult<Vector> SteepestAscent(
            Function f, Vector x0, 
            double[] minValues, double[] maxValues,
            double globalPrecision, double gradOptPrecision, double gradPrecision,
            double[] gradOptRange, int maxNumberOfSteps)
        {
            return GradientOptimization(f, x0, OptimizationType.MAXIMIZE, 
                                        Vector.Of(minValues), Vector.Of(maxValues),
                                        globalPrecision, gradOptPrecision,
                                        gradPrecision, gradOptRange, maxNumberOfSteps);
        }
        
        public static OptimizationResult<Vector> SteepestDescent(
            Function f, Vector x0, 
            double[] minValues, double[] maxValues,
            double globalPrecision, double gradOptPrecision, double gradPrecision,
            double[] gradOptRange, int maxNumberOfSteps)
        {
            return GradientOptimization(f, x0, OptimizationType.MINIMIZE,
                                        Vector.Of(minValues), Vector.Of(maxValues), 
                                        globalPrecision, gradOptPrecision,
                                        gradPrecision, gradOptRange, maxNumberOfSteps);
        }
        
        public static OptimizationResult<Vector> SteepestAscent(
            Function f, Vector x0, 
            Vector minValues, Vector maxValues,
            double globalPrecision, double gradOptPrecision, double gradPrecision,
            double[] gradOptRange, int maxNumberOfSteps)
        {
            return GradientOptimization(f, x0, OptimizationType.MAXIMIZE,
                                        minValues, maxValues, 
                                        globalPrecision, gradOptPrecision,
                                        gradPrecision, gradOptRange, maxNumberOfSteps);
        }
        
        public static OptimizationResult<Vector> SteepestDescent(
            Function f, Vector x0, 
            Vector minValues, Vector maxValues,
            double globalPrecision, double gradOptPrecision, double gradPrecision,
            double[] gradOptRange, int maxNumberOfSteps)
        {
            return GradientOptimization(f, x0, OptimizationType.MINIMIZE,
                                        minValues, maxValues,
                                        globalPrecision, gradOptPrecision,
                                        gradPrecision, gradOptRange, maxNumberOfSteps);
        }
        
        public static OptimizationResult<Vector> GradientOptimization(
            Function f, Vector x0, OptimizationType type,
            Vector minValues, Vector maxValues,
            double globalPrecision, double gradOptPrecision, double gradPrecision,
            double[] gradOptRange, int maxNumberOfSteps)
        {
            double directionCoef = (type == OptimizationType.MAXIMIZE) ? 1.0 : -1.0;
            Vector xj = new Vector(x0);
            Vector xjGrad = Diff.Gradient(f, gradPrecision, xj);
            Function direction = (args => {
                 double L = args[0];
                return f((xj + directionCoef*L*xjGrad).Data);
             });
            double gradOptMin = gradOptRange[0], gradOptMax = gradOptRange[1];
            double lambda = GoldenSectionMethod(direction, gradOptMin, gradOptMax, gradOptPrecision, type).Argument;
            if (log.IsDebugEnabled) 
                log.Debug(lambda);
            xj = xj + directionCoef*lambda*xjGrad;
            xjGrad = Diff.Gradient(f, gradPrecision, xj);
            int n = 1;
            while (n < maxNumberOfSteps && 
                   IsAllowedVector(xj, minValues, maxValues) &&
                   xjGrad.Norm() > globalPrecision)
            {
                lambda = GoldenSectionMethod(direction, gradOptMin, gradOptMax, gradOptPrecision, type).Argument;
                xj = xj + directionCoef*lambda*xjGrad;
                xjGrad = Diff.Gradient(f, gradPrecision, xj);
                n++;
            }
            return new OptimizationResult<Vector>(xj, f(xj.Data));
        }
        
        private static bool IsAllowedVector(Vector v, Vector vMin, Vector vMax)
        {
            for (int i = 0; i < v.Dim; i++)
            {
                if (log.IsDebugEnabled)
                    log.DebugFormat("value = {0}, min value = {1}, maxValue = {2}", v[i], vMin[i], vMax[i]);
                if (!(vMin[i] <= v[i] && v[i] <= vMax[i]))
                {
                    return false;
                }
            }
            return true;
        }
        
        public class OptimizationResult<T>
        {
            public OptimizationResult(T arg, double val)
            {
                Argument = arg;
                Value = val;
            }
            
            public T Argument
            {
                get; private set;
            }
            
            public double Value
            {
                get; private set;
            }
            
            public override string ToString()
            {
                return string.Format("[OptimizationResult Argument={0}, Value={1}]", Argument, Value);
            }

        }
        
        public enum OptimizationType
        {
            MAXIMIZE,
            MINIMIZE
        }
    }
}
