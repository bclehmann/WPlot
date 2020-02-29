using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Where1.WPlot
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public double[] xs, ys;

		public void ReadCSVFile(string path)
		{
			StreamReader file = new StreamReader(path);
			string[] raw = file.ReadToEnd().Split(new char[] { ',', '\n' });
			List<double> serialData = raw.Where(m => double.TryParse(m, out _)).Select(m => double.Parse(m)).ToList();

			xs = new double[serialData.Count / 2];
			ys = new double[serialData.Count / 2];
			for (int i = 0; i < serialData.Count; i++)
			{
				int row = i / 2;
				int col = i % 2;

				if (col == 0)
				{
					xs[row] = serialData[i];
				}
				else
				{
					ys[row] = serialData[i];
				}
			}
			(this.MainWindow as MainWindow).RenderPlot();
		}
	}
}
