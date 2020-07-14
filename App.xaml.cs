using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

		public void AddSeries(PlotParameters plotParams)
		{
			series.Add(plotParams);
			((MainWindow)this.MainWindow).RenderPlot();
		}

		public PlotParameters AddSeriesFromString(string dataString, DrawSettings drawSettings, Dictionary<string, object> metadata)
		{
			object data = new object();

			string[] raw = dataString.Split(new char[] { ',', '\n' });

			if (!drawSettings.dateXAxis)
			{
				List<double> serialData = raw.Where(m => double.TryParse(m, out _)).Select(m => double.Parse(m)).ToList();

				if (drawSettings.type == PlotType.scatter || drawSettings.type == PlotType.bar)
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
				else if (drawSettings.type == PlotType.signal || drawSettings.type == PlotType.histogram)
				{
					data = serialData.ToArray();
				}
				else if (drawSettings.type == PlotType.box_whisker)
				{
					ScottPlot.Statistics.Population dataPopulation = new ScottPlot.Statistics.Population(serialData.ToArray());
					data = dataPopulation;
				}

				if (drawSettings.type == PlotType.scatter && drawSettings.polarCoordinates)
				{
					(((double[][])data)[0], ((double[][])data)[1]) = ScottPlot.Tools.ConvertPolarCoordinates(((double[][])data)[0], ((double[][])data)[1]);
				}
			}
			else
			{
				if (drawSettings.type == PlotType.scatter || drawSettings.type == PlotType.bar)
				{
					List<DateTime> dataX = new List<DateTime>();
					List<double> dataY = new List<double>();

					int successfullyParsed = 0;
					for (int i = 0; i < raw.Length; i++)
					{
						if (successfullyParsed % 2 == 0)
						{
							DateTime temp;
							if (DateTime.TryParse(raw[i], out temp))
							{
								dataX.Add(temp);
								successfullyParsed++;
							}
						}
						else
						{
							double temp;
							if (double.TryParse(raw[i], out temp))
							{
								dataY.Add(temp);
								successfullyParsed++;
							}
						}
					}

					long totalDistanceTicks = 0;
					for (int i = 0; i < dataX.Count; i++)
					{
						if (i == dataX.Count - 1)
						{
							break;
						}

						totalDistanceTicks += Math.Abs(dataX[i].Ticks - dataX[i + 1].Ticks);
					}
					long averageTickDistance = totalDistanceTicks / (dataX.Count - dataX.Count % 2); //The fact that this is rounded doesn't matter, bc ticks are so obsenely small
					long averageSecondsDistance = averageTickDistance / 10_000_000;

					metadata["numTimeUnits"] = 1;

					if (averageSecondsDistance > 0.75 * 86400 * 365)
					{
						metadata["timeUnit"] = ScottPlot.Config.DateTimeUnit.Year;
					}
					else if (averageSecondsDistance > 0.75 * 86400 * 30)
					{
						metadata["timeUnit"] = ScottPlot.Config.DateTimeUnit.Month;
					}
					else if (averageSecondsDistance > 0.75 * 86400)
					{
						metadata["timeUnit"] = ScottPlot.Config.DateTimeUnit.Day;
					}
					else if (averageSecondsDistance > 0.75 * 3600)
					{
						metadata["timeUnit"] = ScottPlot.Config.DateTimeUnit.Hour;
					}
					else if (averageSecondsDistance > 0.75 * 60)
					{
						metadata["timeUnit"] = ScottPlot.Config.DateTimeUnit.Minute;
					}
					else
					{
						metadata["timeUnit"] = ScottPlot.Config.DateTimeUnit.Second;
					}
					metadata["startDate"] = dataX[0];

					metadata["numTimeUnits"] = (dataX.Count / 30) + 1;

					double[] dataXDouble = dataX.Select((dt, i) => dt.ToOADate()).ToArray();


					data = new double[][] { dataXDouble, dataY.ToArray() };
				}
			}

			PlotParameters plotParams = new PlotParameters { data = data, drawSettings = drawSettings, metaData = metadata };
			series.Add(plotParams);

			((MainWindow)this.MainWindow).RenderPlot();

			return plotParams;
		}

		public PlotParameters AddSeriesFromString(string[] raw, DrawSettings drawSettings, Dictionary<string, object> metadata)
		{
			object data = new object();

			if (drawSettings.type == PlotType.bar_grouped)
			{
				double[][] dataList = new double[raw.Length][];

				for (int i = 0; i < raw.Length; i++)
				{
					string[] temp = raw[i].Split(',', '\n');
					dataList[i] = new double[temp.Length];
					for (int j = 0; j < temp.Length; j++)
					{
						double parseOut;
						if (double.TryParse(temp[j], out parseOut))
						{
							dataList[i][j] = (parseOut);
						}
					}
				}

				data = dataList;
			}
			else
			{
				throw new NotImplementedException();
			}

			if (!metadata.ContainsKey("group_names"))
			{
				metadata.Add("group_names", Enumerable.Range(1, ((double[][])data)[0].Count()).Select(i => i + "").ToArray());
			}

			if (!metadata.ContainsKey("series_names"))
			{
				metadata.Add("series_names", Enumerable.Range(1, ((double[][])data).Count()).Select(i => char.ConvertFromUtf32(64 + i)).ToArray());
			}


			PlotParameters plotParams = new PlotParameters { data = data, drawSettings = drawSettings, metaData = metadata };
			series.Add(plotParams);

			((MainWindow)this.MainWindow).RenderPlot();

			return plotParams;

		}

		public PlotParameters AddSeriesFromCSVFile(string filePathInfo, DrawSettings drawSettings, Dictionary<string, object> metadata = null)
		{
			if (filePathInfo.Contains(','))
			{
				return AddSeriesFromCSVFile(filePathInfo.Split(','), drawSettings, metadata);
			}
			using (StreamReader file = new StreamReader(filePathInfo))
			{
				string raw = file.ReadToEnd();
				return AddSeriesFromString(raw, drawSettings, metadata);
			}

		}

		public PlotParameters AddSeriesFromCSVFile(string[] filenames, DrawSettings drawSettings, Dictionary<string, object> metadata = null)
		{

			if (drawSettings.type == PlotType.bar_grouped)
			{
				string[] raw = new string[filenames.Length];

				for (int i = 0; i < raw.Length; i++)
				{
					using (StreamReader file = new StreamReader(filenames[i]))
					{
						raw[i] = (file.ReadToEnd());
					}
				}


				return AddSeriesFromString(raw, drawSettings, metadata);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		public void AddErrorFromCSVFile(PlotParameters plotParams, string path)
		{
			object data = new object();

			if (plotParams.drawSettings.type != PlotType.bar_grouped)
			{
				using (StreamReader file = new StreamReader(path))
				{
					string[] raw = file.ReadToEnd().Split(new char[] { ',', '\n' });
					List<double> serialData = raw.Where(m => double.TryParse(m, out _)).Select(m => double.Parse(m)).ToList();

					if (plotParams.drawSettings.type == PlotType.scatter)
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
					else if (plotParams.drawSettings.type == PlotType.bar)
					{
						data = serialData.ToArray();
					}
				}
			}
			else
			{
				string[] paths = path.Split(',');
				data = new double[paths.Length][];

				for (int i = 0; i < paths.Length; i++)
				{
					using (StreamReader file = new StreamReader(paths[i]))
					{
						string[] raw = file.ReadToEnd().Split(new char[] { ',', '\n' });
						((double[][])data)[i] = raw.Where(m => double.TryParse(m, out _)).Select(m => double.Parse(m)).ToArray();
					}

				}
			}
			plotParams.errorData = data;
			plotParams.hasErrorData = true;

			((MainWindow)this.MainWindow).RenderPlot();
		}
	}
}
