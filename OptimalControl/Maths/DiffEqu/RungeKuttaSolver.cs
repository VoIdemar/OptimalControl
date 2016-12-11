using System;
using System.Collections.Generic;

using log4net;

using OptimalControl.Maths;
using OptimalControl.Chemical;
using OptimalControl.Tools;

namespace OptimalControl.Maths.DiffEqu
{
    public class RungeKuttaSolver : BasicSolver
     {
        private static readonly ILog log = LogManager.GetLogger(typeof(RungeKuttaSolver));
        
        public RungeKuttaSolver()
            :base() { }
        
        public override void Solve()
        {
            if (log.IsDebugEnabled) log.Debug("Runge-Kutta method started. Initializing...");
            InitGridAndSolutionMatrix();
            if (log.IsDebugEnabled) 
            {
                log.Debug("Initialized.");
                log.DebugFormat("\nRunge-Kutta params:\nGrid range: ({0}, {1})\nStep: {2}\nGrid: {3}" +
                                "\nInitial conditions: {4}",
                    GridMin, GridMax, Step, 
                    Grid.GetContentsString(),
                    InitialConditions.GetContentsString()
                );
            }
            ExecuteMethod();
            if (log.IsDebugEnabled) 
                log.DebugFormat("Runge-Kutta method executed.\nSolution:\n{0}", Solution.DeepGetContentsString());
        }
        
        private void InitGridAndSolutionMatrix()
        {
            Grid = MathTools.Linspace(GridMin, GridMax, GridSize);
            Step = (GridMax - GridMin)/GridSize;
            
            Solution = new double[Dim][];
            for (int i = 0; i < Solution.Length; i++) 
            {
                Solution[i] = new double[GridSize];
                Solution[i].Fill(0.0);
            }
            
            for (int i = 0; i < Dim; i++)
            {
                Solution[i][0] = InitialConditions[i];
            }
        }
        
        private void ExecuteMethod()
        {
            for (int j = 0; j < GridSize - 1; j++)
            {
                double[,] k = new KCalculator(this, j).Calculate();
                for (int i = 0; i < Dim; i++)
                {                    
                    Solution[i][j+1] = Solution[i][j] + (Step/6.0)*(k[i,0] + 2.0*k[i,1] + 2.0*k[i,2] + k[i,3]);
                }
            }
        }
        
        private class KCalculator
        {
            private static readonly int K_COUNT = 4;
            private static readonly double[] C = {0.0, 0.5, 0.5, 1.0};
            
            private double[,] k;
            private int currentIdx;
            private RungeKuttaSolver solver;
            
            public KCalculator(RungeKuttaSolver solver, int currentIdx)
            {
                this.currentIdx = currentIdx;
                this.solver = solver;
                this.k = new double[this.solver.Dim, K_COUNT];
            }
            
            public double[,] Calculate()
            {
                for (int j = 0; j < K_COUNT; j++)
                {
                    for (int i = 0;    i < solver.Dim; i++)
                    {
                        k[i, j] = solver.Functions[i](GetKParams(j));
                    }
                }
                return k;
            }

            private double[] GetKParams(int kIdx)
            {
                double h = solver.Step;
                double[] kParams = new double[solver.Dim+1];
                if (kIdx == 0)
                {
                    kParams[0] = solver.Grid[currentIdx];
                    for (int i = 1; i < kParams.Length; i++)
                    {
                        kParams[i] = solver.Solution[i-1][currentIdx];
                    }
                }
                else
                {
                    double c = C[kIdx];
                    kParams[0] = solver.Grid[currentIdx] + c*h;
                    for (int i = 1; i < kParams.Length; i++)
                    {
                        kParams[i] = solver.Solution[i-1][currentIdx] + c*h*k[i-1, kIdx];
                    }
                }
                return kParams;
            }
        }
    }
}
