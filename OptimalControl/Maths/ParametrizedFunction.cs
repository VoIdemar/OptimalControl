using System;

using log4net;

using OptimalControl.Tools;

namespace OptimalControl.Maths
{
    public class ParametrizedFunction
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ParametrizedFunction));
        
        private double[] parameters;
        private int paramsCount;
        private Function function;        
        
        public ParametrizedFunction(Function function, int paramsCount)
        {
            this.function = function;
            this.paramsCount = paramsCount;
            this.parameters = new double[paramsCount];            
        }
        
        public ParametrizedFunction(Function function)
            :this(function, 0) { }
    
        public ParametrizedFunction(Function function, double[] parameters)
        {
            this.function = function;
            this.Parameters = parameters;
        }
        
        public double[] Parameters
        {
            get { return parameters; }
            set 
            { 
                parameters = (value == null ? new double[0] : value);
                paramsCount = (value == null ? 0 : value.Length);
            }
        }
        
        public int ParametersCount
        {
            get { return paramsCount; }
        }
        
        public void SetParameter(int idx, double val)
        {
            Parameters[idx] = val;
        }
        
        public double Eval(params double[] args)
        {
            double[] realArgs = args.Append(Parameters);
            return function(realArgs);
        }
    }
}
