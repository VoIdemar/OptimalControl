using System;
using System.Windows.Forms;
using log4net.Config; 

namespace OptimalControl
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			XmlConfigurator.Configure();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
	}
}
