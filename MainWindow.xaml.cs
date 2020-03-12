using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Where1.WPlot
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			//plotFrame.plt.PlotSignal(ScottPlot.DataGen.Sin(50));
		}

		public void ClearPlot()
		{
			plotFrame.plt.Clear();
			plotFrame.Render();
			(App.Current as App).ClearSeries();
		}

		public System.Drawing.Color NextColour()
		{
			return plotFrame.plt.GetSettings().GetNextColor();
		}

		public string plotTitle { get; set; }
		public string xLabel { get; set; }
		public string yLabel { get; set; }
		public bool logAxis { get; set; }
		private bool gridLines = true;

		public void RefreshTitleAndAxis(bool shouldRender = true)
		{
			plotFrame.plt.Title(plotTitle);
			plotFrame.plt.XLabel(xLabel);
			plotFrame.plt.YLabel(yLabel);
			if (shouldRender)
			{
				RenderPlot();
			}

		}

		public void RenderPlot()
		{
			plotFrame.plt.Clear();
			RefreshTitleAndAxis(false);
			foreach (PlotParameters curr in ((App)App.Current).GetSeries())
			{

				switch (curr.drawSettings.type)
				{
					case PlotType.scatter:
						double[] xs = ((double[][])curr.data)[0];
						double[] ys = ((double[][])curr.data)[1];
						if (logAxis)
						{
							ys = ScottPlot.Tools.Log10(ys);
						}
						if (!curr.hasErrorData)
						{
							plotFrame.plt.PlotScatter(xs, ys, curr.drawSettings.colour, curr.drawSettings.drawLine ? 1 : 0, label: curr.drawSettings.label, markerShape: curr.drawSettings.markerShape);
						}
						else
						{
							double[] errorX = ((double[][])curr.errorData)[0];
							double[] errorY = ((double[][])curr.errorData)[1];
							plotFrame.plt.PlotScatter(xs, ys, curr.drawSettings.colour, curr.drawSettings.drawLine ? 1 : 0, label: curr.drawSettings.label, markerShape: curr.drawSettings.markerShape, errorX: errorX, errorY: errorY);
						}

						if (curr.drawSettings.drawLinearRegression)
						{
							var model = new ScottPlot.Statistics.LinearRegressionLine(xs, ys);
							double x1 = xs[0];
							double x2 = xs[xs.Length - 1];
							double y1 = model.GetValueAt(x1);
							double y2 = model.GetValueAt(x2);
							plotFrame.plt.PlotLine(x1, y1, x2, y2, lineWidth: 3, label: $"ŷ = {model.offset:f9} + {model.slope:f9}x");
						}
						break;
					case PlotType.signal:
						object sampleRate = 100;
						curr.metaData.TryGetValue("sampleRate", out sampleRate);
						object xOffset = 0;
						curr.metaData.TryGetValue("xOffset", out xOffset);
						double[] data = (double[])curr.data;

						if (logAxis)
						{
							data = ScottPlot.Tools.Log10(data);
						}
						plotFrame.plt.PlotSignalConst(data, (double)sampleRate, (double)xOffset, color: curr.drawSettings.colour, lineWidth: curr.drawSettings.drawLine ? 1 : 0, label: curr.drawSettings.label, markerSize: 0);
						break;
				}
			}
			plotFrame.plt.Legend();
			plotFrame.Render();
		}

		public void SavePlot(string path)
		{
			plotFrame.plt.SaveFig(path, false); //It's already been rendered
		}

		private void GridLineToggle_Click(object sender, RoutedEventArgs e)
		{
			gridLines = !gridLines;
			plotFrame.plt.Grid(gridLines);
			plotFrame.Render();
		}

		private void FrameSettings_Click(object sender, RoutedEventArgs e)
		{
			switch (((string)((MenuItem)e.OriginalSource).Tag).ToUpperInvariant())
			{
				case "NONE":
					plotFrame.plt.Frame(drawFrame: false);
					break;
				case "NORMAL":
					plotFrame.plt.Frame(left: true, bottom: true, right: true, top: true);
					break;
				case "CORNER":
					plotFrame.plt.Frame(left: true, bottom: true, right: false, top: false);
					break;
				case "LEFT":
					plotFrame.plt.Frame(left: true, bottom: false, right: false, top: false);
					break;
				case "BOTTOM":
					plotFrame.plt.Frame(left: false, bottom: true, right: false, top: false);
					break;
			}
			plotFrame.Render();
		}

		private void TickSettings_Click(object sender, RoutedEventArgs e)
		{
			switch (((string)((MenuItem)e.OriginalSource).Tag).ToUpperInvariant())
			{
				case "NONE":
					plotFrame.plt.Ticks(false, false);
					break;
				case "NORMAL":
					plotFrame.plt.Ticks(true, true);
					break;
				case "LEFT":
					plotFrame.plt.Ticks(false, true);
					break;
				case "BOTTOM":
					plotFrame.plt.Ticks(true, false);
					break;
			}

			plotFrame.Render();
		}

		private void ThemeSettings_Click(object sender, RoutedEventArgs e)
		{
			switch (((string)((MenuItem)e.OriginalSource).Header).ToUpperInvariant())
			{
				case "DEFAULT":
					plotFrame.plt.Style(ScottPlot.Style.Default);
					break;
				case "BLUE1":
					plotFrame.plt.Style(ScottPlot.Style.Blue1);
					break;
				case "BLUE2":
					plotFrame.plt.Style(ScottPlot.Style.Blue2);
					break;
				case "BLUE3":
					plotFrame.plt.Style(ScottPlot.Style.Blue3);
					break;
				case "LIGHT1":
					plotFrame.plt.Style(ScottPlot.Style.Light1);
					break;
				case "LIGHT2":
					plotFrame.plt.Style(ScottPlot.Style.Light2);
					break;
				case "GRAY1":
					plotFrame.plt.Style(ScottPlot.Style.Gray1);
					break;
				case "GRAY2":
					plotFrame.plt.Style(ScottPlot.Style.Gray2);
					break;
				case "BLACK":
					plotFrame.plt.Style(ScottPlot.Style.Black);
					break;
				case "CONTROL":
					plotFrame.plt.Style(ScottPlot.Style.Control);
					break;
			}

			plotFrame.Render();
		}

		private void WPlotLink_Click(object sender, RoutedEventArgs e)
		{
			using (Process proc = new Process())
			{
				proc.StartInfo.FileName = "https://github.com/Benny121221/WPlot";
				proc.StartInfo.UseShellExecute = true;
				proc.Start();
			}
		}

		private void ScottPlotLink_Click(object sender, RoutedEventArgs e)
		{
			using (Process proc = new Process())
			{
				proc.StartInfo.FileName = "https://github.com/swharden/ScottPlot";
				proc.StartInfo.UseShellExecute = true;
				proc.Start();
			}
		}
	}
}
