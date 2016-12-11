using System;
using System.Collections.Generic;
using OptimalControl.Maths;

namespace OptimalControl.Maths.DiffEqu
{
    public abstract class BasicSolver
    {
        private double[] grid;
        private double a, b, h;
        private int gridSize;
        private IList<Function> functions;
        private double[][] s;
        private int systemDim;
        private double[] initialConditions;
        
        public abstract void Solve();
        
        public BasicSolver()
        {
            grid = null;
            functions = new List<Function>();
            s = null;
        }
        
        public double[] Grid
        {
            get { return grid; }
            protected set { grid = value; } 
        }
        
        public int GridSize
        {
            get { return gridSize; }
        }
        
        public int Dim
        {
            get { return systemDim; }
        }
        
        public double Step
        {
            get { return h; }
            protected set { h = value; }
        }
        
        public double GridMin
        {
            get { return a; }
        }
        
        public double GridMax
        {
            get { return b; }
        }
        
        public double[] InitialConditions
        {
            get { return initialConditions; }
        }
        
        public double[][] Solution
        {
            get
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Solution not yet found. Call Solve() to find the solution of the equation system");
                }
                return s;
            }
            protected set { s = value; }
        }
        
        public IList<Function> Functions
        {
            get { return functions; }
        }
        
        public BasicSolver SetGridRange(double a, double b)
        {
            this.a = a;
            this.b = b;
            return this;            
        }
        
        public BasicSolver SetGridSize(int n)
        {
            gridSize = n;
            return this;
        }
        
        public BasicSolver SetDim(int dim)
        {
            systemDim = dim;
            return this;
        }
        
        public BasicSolver SetFunctions(IList<Function> functions)
        {
            this.functions = functions;
            return this;
        }
        
        public BasicSolver SetInitialConditions(double[] conditions)
        {
            this.initialConditions = conditions;
            return this;
        }
    }
}
