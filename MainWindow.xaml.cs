﻿using System;
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
		private bool gridLines = true;

		public void RefreshTitleAndAxisLabels(bool shouldRender = true)
		{
			plotFrame.plt.Title(plotTitle);
			plotFrame.plt.XLabel(xLabel);
			plotFrame.plt.YLabel(yLabel);
			if (shouldRender)
			{
				plotFrame.Render();
			}

		}

		public void RenderPlot()
		{
			plotFrame.plt.Clear();
			RefreshTitleAndAxisLabels(false);
			foreach (PlotParameters curr in ((App)App.Current).GetSeries())
			{

				switch (curr.drawSettings.type)
				{
					case PlotType.scatter:
						double[] xs = ((double[][])curr.data)[0];
						double[] ys = ((double[][])curr.data)[1];
						plotFrame.plt.PlotScatter(xs, ys, curr.drawSettings.colour, curr.drawSettings.drawLine ? 1 : 0);
						break;
					case PlotType.signal:
						object sampleRate = 100;
						curr.metaData.TryGetValue("sampleRate", out sampleRate);
						object xOffset = 0;
						curr.metaData.TryGetValue("xOffset", out xOffset);
						//SignalConst is faster, and they can't change the data anyways
						plotFrame.plt.PlotSignalConst((double[])curr.data, (double)sampleRate, (double)xOffset, color: curr.drawSettings.colour, lineWidth: curr.drawSettings.drawLine ? 1 : 0);
						break;
				}
			}
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
