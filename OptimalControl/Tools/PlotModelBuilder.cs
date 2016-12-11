using System;
using System.Linq;
using log4net;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OptimalControl.Maths;

namespace OptimalControl.Tools
{
    public class PlotModelBuilder
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PlotModelBuilder));
        
        private double[] grid;
        private double[][] dataPoints;
        private PlotModel model;
        private string[] seriesTitles;
        private string mask;
        
        public PlotModelBuilder(double[] grid, double[][] dataPoints)
        {
            this.grid = grid;
            this.dataPoints = dataPoints;
            this.model = new PlotModel();
        }
        
        public PlotModelBuilder SetPlotType(PlotType type)
        {
            model.PlotType = type;
            return this;
        }
        
        public PlotModelBuilder SetBackgroundColor(byte r, byte g, byte b)
        {
            model.Background = OxyColor.FromRgb(r, g, b);
            return this;
        }
        
        public PlotModelBuilder SetTextColor(byte r, byte g, byte b)
        {
            model.TextColor = OxyColor.FromRgb(r, g, b);
            return this;
        }
        
        public PlotModelBuilder SetSeriesTitles(string[] titles)
        {
            seriesTitles = titles;
            return this;
        }
        
        public PlotModelBuilder SetNameMask(string mask)
        {
            this.mask = mask;
            return this;
        }        
        
        public PlotModelBuilder SetTitle(string title)
        {
            model.Title = title;
            return this;
        }    
        
        public PlotModel Build()
        {    
            double minValue = double.MaxValue,
                   maxValue = double.MinValue;
            for (int k = 0; k < dataPoints.Length; k++)
            {
                double[] series = dataPoints[k];
                LineSeries lineSeries = new LineSeries();
                if (seriesTitles != null && seriesTitles.Length != 0) 
                    lineSeries.Title = seriesTitles[k];
                else if (mask != null)
                    lineSeries.Title = string.Format(mask, k+1);
                for (int i = 0; i < series.Length; i++)
                {
                    double currentValue = series[i];
                    if (currentValue < minValue) minValue = currentValue;
                    if (currentValue > maxValue) maxValue = currentValue;
                    lineSeries.Points.Add(new DataPoint(grid[i], currentValue));
                }
                model.Series.Add(lineSeries);
            }
            model.Axes.Add(new LinearAxis(AxisPosition.Bottom, grid.FirstOrDefault(), grid.LastOrDefault()));
            model.Axes.Add(new LinearAxis(AxisPosition.Left, minValue, maxValue));
            
            PlotModel result = model;
            model = new PlotModel();
            return result;
        }
        
        public static LineSeries BuildLineSeries(Function f, double a, double b, int segmentCount)
        {
            double[] grid = MathTools.Linspace(a, b, segmentCount);
            LineSeries series = new LineSeries();
            for (int i = 0; i < grid.Length; i++)
            {
                series.Points.Add(new DataPoint(grid[i], f(grid[i])));
            }
            return series;
        }
        
        public static LineSeries BuildLineSeries(ParametrizedFunction f, double a, double b, int segmentCount)
        {
            double[] grid = MathTools.Linspace(a, b, segmentCount);
            LineSeries series = new LineSeries();
            for (int i = 0; i < grid.Length; i++)
            {
                series.Points.Add(new DataPoint(grid[i], f.Eval(grid[i])));
            }
            return series;
        }
    }
}
