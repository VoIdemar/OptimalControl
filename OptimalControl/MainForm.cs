using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using log4net;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;
using OptimalControl.Maths;
using OptimalControl.Maths.Common;
using OptimalControl.Maths.DiffEqu;
using OptimalControl.Maths.Optimization;
using OptimalControl.Tools;
using OptimalControl.Chemical;

namespace OptimalControl
{        
    public partial class MainForm : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainForm));
                
        public MainForm()
        {
            InitializeComponent();
            PerformOptimization();
            //Testing();
        }
        
        private void PerformOptimization()
        {
            ReactorOptimizer optimizer = new ReactorOptimizer(10000);
            
            optimizer.DiffPrecision = ReactorCommons.DIFF_PRECISION;
            optimizer.OptPrecision = ReactorCommons.OPTIMIZATION_PRECISION;
            optimizer.GradOptPrecision = ReactorCommons.GRAD_OPT_PRECISION;
            optimizer.MinDist = ReactorCommons.MIN_DIST;
            optimizer.MaxDist = ReactorCommons.MAX_DIST;
            optimizer.MaxNumberOfOptimizationSteps = ReactorCommons.MAX_NUMBER_OF_STEPS;
            optimizer.InitialX7Parameters = ReactorCommons.INITIAL_X7_PARAMS;
            optimizer.GradOptMin = ReactorCommons.MIN_GRAD_VALUE;
            optimizer.GradOptMax = ReactorCommons.MAX_GRAD_VALUE; 
            optimizer.MinParamValues = ReactorCommons.MIN_X7_PARAMS;
            optimizer.MaxParamValues = ReactorCommons.MAX_X7_PARAMS;
            
            try
            {
                optimizer.Optimize();                
                if (log.IsDebugEnabled)
                    log.Debug("Results: " + optimizer.X7Params.GetContentsString());
                InitializePlotModels(optimizer);                
            }
            catch (Exception ex)
            {
                log.Error("Optimization failed - ", ex);
            }
        }
        
        private void InitializePlotModels(ReactorOptimizer optimizer)
        {
            double[] grid = optimizer.Solver.Grid;
            ParametrizedFunction X7 = ReactorCommons.X7;
            for (int i = 1; i <= optimizer.Approximations.Count; i++)
            {
                PlotView plot = new PlotView();
                plot.Dock = DockStyle.Fill;
                PlotModelBuilder builder = new PlotModelBuilder(grid, optimizer.Approximations[i-1]);
                PlotModel model = builder
                    .SetPlotType(PlotType.XY)
                    .SetTextColor(0, 0, 0)
                    .SetBackgroundColor(255, 255, 255)
                    .SetNameMask("X{0}(t)")
                    .Build();
                plot.Model = model;
                plot.Model.Title = string.Format("Reactor Control Results for X7 parameters = {0}, value = {1}", optimizer.X7Params[i-1].GetContentsString(), optimizer.QualityCriteriaValues[i-1]);
                
                AddTabPageWithPlot("X1-X6, Approx " + i.ToString(), plot);
                
                PlotView x7Plot = new PlotView();
                x7Plot.Dock = DockStyle.Fill;
                PlotModel x7Model = new PlotModel();
                
                X7.Parameters = optimizer.X7Params[i-1];
                LineSeries x7Series = PlotModelBuilder.BuildLineSeries(X7, grid.FirstOrDefault(), grid.LastOrDefault(), grid.Length);
                x7Series.Title = "X7(t)";
                x7Model.Series.Add(x7Series);
                x7Model.Title = string.Format("Temperature of the Reactor Fluid, parameters = {0}", X7.Parameters.GetContentsString());
                x7Plot.Model = x7Model;
                
                AddTabPageWithPlot("X7, Approx " + i.ToString(), x7Plot);
            }
        }
        
        private void AddTabPageWithPlot(String name, PlotView view)
        {
            TabPage tabPage = new TabPage(name);
            tabPage.Controls.Add(view);
            plotTabControl.TabPages.Add(tabPage);
        }
        
        private void Testing()
        {
//            Function f1 = delegate(double[] args)
//            {
//                double t = args[0], x = args[1], y = args[2];
//                return x - y + 8.0*t;
//            };
//            
//            Function f2 = delegate(double[] args)
//            {
//                double t = args[0], x = args[1], y = args[2];
//                return 5*x-y;
//            };
//            
//            Function f3 = delegate(double[] args)
//            {
//                double t = args[0];
//                return -2*Math.Cos(2*t)-Math.Sin(2*t)+2*t+2;
//            };
//            
//            Function f4 = delegate(double[] args)
//            {
//                double t = args[0];
//                return 10*t-5*Math.Sin(2*t);
//            };
//            
//            BasicSolver solver = new RungeKuttaSolver();
//            IList<Function> functions = new List<Function>();
//            functions.Add(f1);
//            functions.Add(f2);
//            solver
//                .SetGridRange(0.0, 1.0)
//                .SetGridSize(100)
//                .SetDim(2)
//                .SetInitialConditions(new double[] {0.0, 0.0})
//                .SetFunctions(functions)
//                .Solve();
//            
//            PlotModelBuilder builder = new PlotModelBuilder(solver.Grid, solver.Solution);
//            PlotModel model = builder
//                .SetPlotType(PlotType.XY)
//                .SetTextColor(0, 0, 0)
//                .SetBackgroundColor(255, 255, 255)
//                .SetSeriesTitles(new string[] {
//                    "x(t)", "y(t)"               
//                })
//                .Build();
//            
//            LineSeries s = PlotModelBuilder.BuildLineSeries(f3, 0.0, 1.0, 100);
//            s.Title = "r(x)";
//            model.Series.Add(s);
//            
//            s = PlotModelBuilder.BuildLineSeries(f4, 0.0, 1.0, 100);
//            s.Title = "r(x)";
//            model.Series.Add(s);
//            
//            PlotView view = new PlotView();
//            view.Model = model;
//            view.Dock = DockStyle.Fill;
//            TabPage t2 = new TabPage("Plots2");
//            t2.Controls.Add(view);
//            plotTabControl.TabPages.Add(t2);
            
            Function f = args => {
                double x1 = args[0], x2 = args[1];
                return 1-(x1-2)*(x1-2) - (x2-3)*(x2-3);
            };
            Function f100 = args => {
                double x = args[0];
                return x*x;
            };
            OptimizationMethods.OptimizationResult<Vector> res = 
                OptimizationMethods.SteepestAscent(f100, Vector.Of(-3), Vector.Of(-100), Vector.Of(100), 0.000001, 0.000001, 0.0000001, new double[] {0.0, 100.0}, 100);
            if (log.IsDebugEnabled)
                log.Debug("optresult = " + res);
        }
    }
}
