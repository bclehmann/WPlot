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
		private List<PlotParameters> series = new List<PlotParameters>();

		public List<PlotParameters> GetSeries() => series;
		public void ClearSeries() => series = new List<PlotParameters>();

		public void AddSeriesFromFile(string path, DrawSettings drawSettings, Dictionary<string, object> metadata = null)
		{
			object data = new object();

			using (StreamReader file = new StreamReader(path))
			{
				string[] raw = file.ReadToEnd().Split(new char[] { ',', '\n' });
				List<double> serialData = raw.Where(m => double.TryParse(m, out _)).Select(m => double.Parse(m)).ToList();

				if (drawSettings.type == PlotType.scatter)
				{
					double[] xs = new double[serialData.Count / 2];
					double[] ys = new double[serialData.Count / 2];
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

						data = new double[][] { xs, ys };
					}
				}
				else if (drawSettings.type == PlotType.signal)
				{
					data = serialData.ToArray();
				}
				series.Add(new PlotParameters { data = data, drawSettings = drawSettings, metaData = metadata });

				((MainWindow)this.MainWindow).RenderPlot();
			}
		}
	}
}
