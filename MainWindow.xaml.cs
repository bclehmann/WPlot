using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
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
		PlottableAnnotation snappedCoordinates;
		private List<Plottable> rawPlottables = new List<Plottable>();

		public MainWindow()
		{
			InitializeComponent();
			snappedCoordinates = plotFrame.plt.PlotAnnotation("");
			snappedCoordinates.visible = false;

			rawPlottables.Add(snappedCoordinates);
		}

		public void ClearPlot()
		{
			(App.Current as App).ClearSeries();
			plotFrame.plt.Ticks(dateTimeX: false);
			plotFrame.plt.Clear();
			plotFrame.Render();
		}

		public System.Drawing.Color NextColour()
		{
			return plotFrame.plt.GetSettings().GetNextColor();
		}

		public string plotTitle { get; set; }
		public string xLabel { get; set; }
		public string yLabel { get; set; }
		public bool logAxis { get; set; }
		public bool showCoordinates { get; set; }

		private bool isDateAxis = false;
		private double? dateUnitSpacing;
		private ScottPlot.Config.DateTimeUnit? dateUnit;
		private bool gridLines = true;
		private DateTime beginningOfOADate = new DateTime(1899, 12, 30);

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
			bool containsDateAxis = false;
			foreach (PlotParameters curr in ((App)App.Current).GetSeries())
			{
				if (curr.drawSettings.label == "")
				{
					curr.drawSettings.label = null;//Prevents it from showing up in the legend
				}

				if (curr.drawSettings.dateXAxis)
				{
					containsDateAxis = true;
					object timeUnit;
					object numTimeUnitsObj;
					curr.metaData.TryGetValue("timeUnit", out timeUnit);
					curr.metaData.TryGetValue("numTimeUnits", out numTimeUnitsObj);
					ScottPlot.Config.DateTimeUnit dateTimeUnit = (ScottPlot.Config.DateTimeUnit)timeUnit;
					int numTimeUnits = (int)numTimeUnitsObj;

					dateUnit = dateTimeUnit;
					dateUnitSpacing = numTimeUnits;

					if (dateTimeUnit == ScottPlot.Config.DateTimeUnit.Year)//Grid spacing of one year is currently unsupported -_-
					{
						plotFrame.plt.Grid(xSpacing: 12, xSpacingDateTimeUnit: ScottPlot.Config.DateTimeUnit.Month);
					}
					else
					{
						plotFrame.plt.Grid(xSpacing: numTimeUnits, xSpacingDateTimeUnit: dateTimeUnit);
					}
					plotFrame.plt.Ticks(dateTimeX: true, xTickRotation: 30, fontSize: 10); // Default fontSize is 12
				}

				switch (curr.drawSettings.type)
				{
					case PlotType.scatter:
						double[] xsScatter = ((double[][])curr.data)[0];
						double[] ysScatter = ((double[][])curr.data)[1];
						if (logAxis)
						{
							ysScatter = ScottPlot.Tools.Log10(ysScatter);
						}

						if (!curr.hasErrorData)
						{
							plotFrame.plt.PlotScatterHighlight(xsScatter, ysScatter, curr.drawSettings.colour, curr.drawSettings.drawLine ? 1 : 0, label: curr.drawSettings.label, markerShape: curr.drawSettings.markerShape, highlightedColor: curr.drawSettings.colour);
						}
						else
						{
							double[] errorX = ((double[][])curr.errorData)[0];
							double[] errorY = ((double[][])curr.errorData)[1];
							plotFrame.plt.PlotScatterHighlight(xsScatter, ysScatter, curr.drawSettings.colour, curr.drawSettings.drawLine ? 1 : 0, label: curr.drawSettings.label, markerShape: curr.drawSettings.markerShape, errorX: errorX, errorY: errorY, highlightedColor: curr.drawSettings.colour);
						}

						if (curr.drawSettings.drawLinearRegression)
						{
							var model = new ScottPlot.Statistics.LinearRegressionLine(xsScatter, ysScatter);
							double x1 = xsScatter[0];
							double x2 = xsScatter[xsScatter.Length - 1];
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
					case PlotType.bar:
						double[] xsBar = ((double[][])curr.data)[0];
						double[] ysBar = ((double[][])curr.data)[1];
						if (logAxis)
						{
							ysBar = ScottPlot.Tools.Log10(ysBar);
						}
						if (!curr.hasErrorData)
						{
							plotFrame.plt.PlotBar(xsBar, ysBar, fillColor: curr.drawSettings.colour, outlineColor: curr.drawSettings.colour, errorColor: curr.drawSettings.colour, label: curr.drawSettings.label);
						}
						else
						{
							double[] errorY = ((double[])curr.errorData);
							plotFrame.plt.PlotBar(xsBar, ysBar, fillColor: curr.drawSettings.colour, outlineColor: curr.drawSettings.colour, errorColor: curr.drawSettings.colour, label: curr.drawSettings.label, errorY: errorY);
						}
						break;
					case PlotType.histogram:
						ScottPlot.Statistics.Histogram histogram = new ScottPlot.Statistics.Histogram((double[])curr.data);
						double[] yData = histogram.counts;

						switch (curr.drawSettings.histogramType)
						{
							case HistogramType.fraction | HistogramType.density:
								yData = histogram.countsFrac;
								break;
							case HistogramType.fraction | HistogramType.cumulative:
								yData = histogram.cumulativeFrac;
								break;
							case HistogramType.count | HistogramType.cumulative:
								yData = histogram.cumulativeCounts;
								break;
						}


						plotFrame.plt.PlotBar(histogram.bins, yData, fillColor: curr.drawSettings.colour, outlineColor: curr.drawSettings.colour, errorColor: curr.drawSettings.colour, label: curr.drawSettings.label);
						break;
					case PlotType.horizontal_line:
						double hLineData = (double)curr.data;
						plotFrame.plt.PlotHLine(hLineData, color: curr.drawSettings.colour, label: curr.drawSettings.label);
						break;
					case PlotType.vertical_line:
						double vLineData = (double)curr.data;
						plotFrame.plt.PlotVLine(vLineData, color: curr.drawSettings.colour, label: curr.drawSettings.label);
						break;
					case PlotType.horizontal_span:
						(double hSpanMin, double hSpanMax) = (ValueTuple<double, double>)curr.data;
						plotFrame.plt.PlotHSpan(hSpanMin, hSpanMax, color: curr.drawSettings.colour, label: curr.drawSettings.label);
						break;
					case PlotType.vertical_span:
						(double vSpanMin, double vSpanMax) = (ValueTuple<double, double>)curr.data;
						plotFrame.plt.PlotVSpan(vSpanMin, vSpanMax, color: curr.drawSettings.colour, label: curr.drawSettings.label);
						break;
					case PlotType.box_whisker:
						var plotObj = plotFrame.plt.PlotPopulations((ScottPlot.Statistics.Population)curr.data, label: curr.drawSettings.label);
						plotObj.boxStyle = PlottablePopulations.BoxStyle.BoxMedianQuartileOutlier;
						plotObj.displayDistributionCurve = false;
						break;
					case PlotType.function:
						var f = (Func<double, double?>)curr.data;
						plotFrame.plt.PlotFunction(f, label: curr.drawSettings.label, markerShape: MarkerShape.none, lineStyle: LineStyle.Dash);
						break;
					case PlotType.bar_grouped:
						var plotData = ((double[][])curr.data);
						string[] groupLabels = (string[])curr.metaData.GetValueOrDefault("group_names");
						string[] seriesLabels = (string[])curr.metaData.GetValueOrDefault("series_names");
						if (!curr.hasErrorData)
						{
							plotFrame.plt.PlotBarGroups(groupLabels, seriesLabels, plotData);
						}
						else
						{
							plotFrame.plt.PlotBarGroups(groupLabels, seriesLabels, plotData, (double[][])curr.errorData);
						}
						break;
				}
			}

			isDateAxis = containsDateAxis;
			plotFrame.plt.Legend();

			(double highlightX, double highlightY) = (plotFrame.plt.Axis()[0], plotFrame.plt.Axis()[3]);

			foreach (var curr in rawPlottables)
			{
				if (curr != snappedCoordinates || showCoordinates)
				{
					plotFrame.plt.Add(curr);//Got axed when we cleared everything
				}
			}

			plotFrame.Render();
		}

		public void SavePlot(string path)
		{
			foreach (var currPlottable in plotFrame.plt.GetPlottables())
			{
				if (currPlottable is ScottPlot.PlottableScatterHighlight scatter)
				{
					scatter.HighlightClear();
				}
			}
			bool wasVisible = snappedCoordinates.visible;
			snappedCoordinates.visible = false;
			plotFrame.Render();
			plotFrame.plt.SaveFig(path, false);
			snappedCoordinates.visible = wasVisible;
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

		private void plotFrame_MouseMove(object sender, MouseEventArgs e)
		{
			(double x, double y) = plotFrame.GetMouseCoordinates();
			double? snappedX = null;
			double? snappedY = null;
			List<Plottable> list = plotFrame.plt.GetPlottables();
			Settings settings = plotFrame.plt.GetSettings();

			double minDistanceSquared = double.PositiveInfinity;
			int minPlottableIndex = 0;
			for (int i = 0; i< list.Count; i++)
			{
				if (list[i] is ScottPlot.PlottableScatterHighlight scatter)
				{
					scatter.HighlightClear();
					int currIndex;
					double currX, currY;
					(currX, currY, currIndex) = scatter.GetPointNearest(x, y);
					double distanceSquared = (currX - x) * (currX - x) + (currY - y) * (currY - y);
					if (distanceSquared < minDistanceSquared) {
						minDistanceSquared = distanceSquared;
						minPlottableIndex = i;
					}
				}
			}
			if (minDistanceSquared != double.PositiveInfinity) {
				(snappedX, snappedY, _) = ((PlottableScatterHighlight)list[minPlottableIndex]).HighlightPointNearest(x, y);
			}

			if (snappedX.HasValue)
			{
				string coordinateAnnotation = $"Closest Point: ({snappedX.Value:f3},{snappedY.Value:f3})";
				if (isDateAxis)
				{
					DateTime xDate = DateTime.FromOADate(snappedX.Value);
					coordinateAnnotation = $"Closest point: \n" +
													$"x: {xDate}\n" +
													$"y: {snappedY.Value:n3}";

				}

				snappedCoordinates.label = coordinateAnnotation;
				snappedCoordinates.visible = true;
			}
			else
			{
				snappedCoordinates.visible = false;
			}

			plotFrame.Render();
		}

		private void ToggleShowCoordinates_Click(object sender, RoutedEventArgs e)
		{
			this.showCoordinates = !this.showCoordinates;
			RenderPlot();
		}

		public void CopyToClipboard()
		{
			foreach (var currPlottable in plotFrame.plt.GetPlottables())
			{
				if (currPlottable is ScottPlot.PlottableScatterHighlight scatter)
				{
					scatter.HighlightClear();
				}
			}
			bool wasVisible = snappedCoordinates.visible;
			snappedCoordinates.visible = false;
			plotFrame.Render();

			using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
			{
				memory.Position = 0;
				System.Drawing.Bitmap bmp = plotFrame.plt.GetBitmap(false);
				bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);

				BitmapImage bmpImage = new BitmapImage();
				bmpImage.BeginInit();
				bmpImage.StreamSource = memory;
				bmpImage.EndInit();
				bmpImage.Freeze();
				Clipboard.SetImage(bmpImage);
			}
			snappedCoordinates.visible = wasVisible;
			plotFrame.Render();

		}
	}
}
