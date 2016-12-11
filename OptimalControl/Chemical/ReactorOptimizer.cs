using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using log4net;
using OptimalControl.Tools;
using OptimalControl.Maths;
using OptimalControl.Maths.Common;
using OptimalControl.Maths.DiffEqu;
using OptimalControl.Maths.Optimization;

namespace OptimalControl.Chemical
{
    public class ReactorOptimizer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReactorOptimizer));
        
        private static readonly double EQUALITY_PRECISION_CHECK = 1E-5;
        
        private static readonly int NUMBER_OF_EQUATIONS = 6;
        
        private int 
            numberOfSamples,
            maxNumberOfSteps;
        private double? minDist, maxDist;
        private double
            optPrecision,
            diffPrecision,
            gradOptPrecision,
            gradOptMin,
            gradOptMax;
        private Vector
            minParamValues,
            maxParamValues;
            
        private BasicSolver solver;
        private IList<double[][]> approximations;
        private IList<double[]> x7Params;
        private IList<double> qualityCriteriaValues;
        private Vector initialX7Parameters;
        
        private OptimizationMethods.OptimizationResult<Vector> optResult;
        
        public ReactorOptimizer(int numberOfSamples)
        {
            NumberOfSamples = numberOfSamples;
            minDist = new Nullable<double>();
            maxDist = new Nullable<double>();
            approximations = new List<double[][]>();
            x7Params = new List<double[]>();
            solver = null;
            minParamValues = null;
            maxParamValues = null;
            qualityCriteriaValues = new List<double>();
        }
        
        public int NumberOfSamples 
        {
            get { return numberOfSamples; }
            set { numberOfSamples = value; }
        }    
        
        public double OptPrecision 
        {
            get { return optPrecision; }
            set { optPrecision = value; }
        }

        public double DiffPrecision 
        {
            get { return diffPrecision; }
            set { diffPrecision = value; }
        }

        public double GradOptPrecision 
        {
            get { return gradOptPrecision; }
            set { gradOptPrecision = value; }
        }

        public double GradOptMin 
        {
            get { return gradOptMin; }
            set { gradOptMin = value; }
        }

        public double GradOptMax 
        {
            get { return gradOptMax; }
            set { gradOptMax = value; }
        }
        
        public int MaxNumberOfOptimizationSteps 
        {
            get { return maxNumberOfSteps; }
            set { maxNumberOfSteps = value; }
        }
        
        public BasicSolver Solver
        {
            get { return solver; } 
        }
        
        public double MinDist 
        {
            get { return minDist.Value; }
            set { minDist = value; }
        }

        public double MaxDist 
        {
            get { return maxDist.Value; }
            set { maxDist = value; }
        }
        
        public IList<double[][]> Approximations
        {
            get { return approximations; }
        }
        
        public IList<double[]> X7Params
        {
            get { return x7Params; }
        }
        
        public IList<double> QualityCriteriaValues
        {
            get { return qualityCriteriaValues; }
        }
        
        public Vector InitialX7Parameters
        {
            get { return initialX7Parameters; } 
            set { initialX7Parameters = Vector.Of(value); }
        }
        
        public Vector MinParamValues 
        {
            get { return minParamValues; }
            set { minParamValues = value; }
        }

        public Vector MaxParamValues 
        {
            get { return maxParamValues; }
            set { maxParamValues = value; }
        }
        
        public OptimizationMethods.OptimizationResult<Vector> OptResult
        {
            get { return optResult; }
        }
        
        public void Optimize()
        {
            CheckPreConditions();
            InitializeSolver();
            
            ParametrizedFunction X7 = ReactorCommons.X7;    
            
            Function f = args => 
            {
                X7.Parameters = args;
                if (log.IsDebugEnabled)
                    log.DebugFormat("X7 params = {0}", X7.Parameters.GetContentsString());
                bool areNewArguments = !args.EqualsElementwiseWithPrecision(x7Params.LastOrDefault(), EQUALITY_PRECISION_CHECK);
                if (areNewArguments)
                {
                    solver.Solve();
                    approximations.Add(solver.Solution);
                    x7Params.Add(args);
                }
                double[] finalValues = GetSolutionLastColumn();
                double result = ReactorCommons.QualityCriteria(finalValues);
                if (areNewArguments)
                    qualityCriteriaValues.Add(result);
                if (log.IsDebugEnabled)
                    log.Debug("Optimized function value: " + result.ToString());
                return result;
            };
            
            LogOptimizationStart();
            
            optResult = OptimizationMethods.SteepestAscent(f, InitialX7Parameters, MinParamValues, MaxParamValues,
               OptPrecision, GradOptPrecision, DiffPrecision, new double[] {GradOptMin, GradOptMax}, MaxNumberOfOptimizationSteps);
        }
        
        private double[] GetSolutionLastColumn()
        {
            double[][] currentSolution = solver.Solution;
            double[] finalValues = new double[currentSolution.Length + 1];
            finalValues[0] = MaxDist;
            for (int i = 1; i < finalValues.Length; i++)
            {
                finalValues[i] = currentSolution[i-1].Last();
            }
            return finalValues;
        }
        
        private void CheckPreConditions()
        {
            if (InitialX7Parameters == null)
                throw new InvalidOperationException("Initial X7 parameters have not been specified.");
            if (MinParamValues == null)
                throw new InvalidOperationException("Minimal values of X7 parameters have not been specified.");
            if (MaxParamValues == null)
                throw new InvalidOperationException("Maximal values of X7 parameters have not been specified.");
        }
        
        private void LogOptimizationStart()
        {
            if (log.IsDebugEnabled)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Reactor optimization started, parameters:\n")
                    .Append("Initial X7 params: ").Append(InitialX7Parameters).Append("\n")
                    .Append("Optimization precision: ").Append(OptPrecision).Append("\n")
                    .Append("Gradient optimization precision: ").Append(DiffPrecision).Append("\n")
                    .Append("Gradient calculation precision: ").Append(GradOptPrecision).Append("\n")
                    .Append("Max number of optimization steps: ").Append(MaxNumberOfOptimizationSteps).Append("\n")
                    .AppendFormat("Min distance, max distance: ({0}, {1})\n", MinDist, MaxDist)
                    .AppendFormat("Gradient optimization range: ({0}, {1})\n", GradOptMin, GradOptMax);
                log.Debug(sb.ToString());
            }
        }
        
        private void InitializeSolver()
        {
            solver = new RungeKuttaSolver();
            IList<Function> functions = PrepareFunctions();
            double[] initialConditions = PrepateInitialConditions();
            
            if (!minDist.HasValue) minDist = ReactorCommons.MIN_DIST;
            if (!maxDist.HasValue) maxDist = ReactorCommons.MAX_DIST;
            
            solver
                .SetGridSize(NumberOfSamples)
                .SetFunctions(functions)
                .SetInitialConditions(initialConditions)
                .SetGridRange(minDist.Value, maxDist.Value)
                .SetDim(NUMBER_OF_EQUATIONS);
        }
                
        private IList<Function> PrepareFunctions()
        {
            IList<Function> functions = new List<Function>()
            {
                ReactorCommons.F1,
                ReactorCommons.F2,
                ReactorCommons.F3,
                ReactorCommons.F4,
                ReactorCommons.F5,
                ReactorCommons.F6
            };
            return functions;
        }
        
        private double[] PrepateInitialConditions()
        {
            return new double[] {1.0, 0.0, 0.0, 0.0, 0.0, 0.0};
        }
    }
}
