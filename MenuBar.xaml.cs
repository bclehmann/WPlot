using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Where1.WPlot
{
	partial class MenuBar
	{
		private DrawSettings FetchSettingsFromDialog(SettingsDialog settingsDialog, PlotType type)
		{
			DrawSettings drawSettings = new DrawSettings();

			if (settingsDialog.ShowDialog() != true)
			{//it is nullable
				throw new InvalidOperationException();
			}

			drawSettings.colour = settingsDialog.plotColour;
			drawSettings.drawLine = settingsDialog.shouldDrawLine.IsChecked == true; //Because it is nullable
			drawSettings.drawLinearRegression = settingsDialog.linreg.IsChecked == true; //Because it is nullable
			drawSettings.type = type;
			drawSettings.label = settingsDialog.plotNameTextBox.Text;

			string markerTypeName = settingsDialog.markerTypeComboBox.Text.ToUpperInvariant();
			switch (markerTypeName)
			{
				case "FILLED CIRCLE":
					drawSettings.markerShape = ScottPlot.MarkerShape.filledCircle;
					break;
				case "FILLED SQUARE":
					drawSettings.markerShape = ScottPlot.MarkerShape.filledSquare;
					break;
				case "OPEN CIRCLE":
					drawSettings.markerShape = ScottPlot.MarkerShape.openCircle;
					break;
				case "OPEN SQUARE":
					drawSettings.markerShape = ScottPlot.MarkerShape.openSquare;
					break;
				case "FILLED DIAMOND":
					drawSettings.markerShape = ScottPlot.MarkerShape.filledDiamond;
					break;
				case "OPEN DIAMOND":
					drawSettings.markerShape = ScottPlot.MarkerShape.openDiamond;
					break;
				case "ASTERISK":
					drawSettings.markerShape = ScottPlot.MarkerShape.asterisk;
					break;
				case "HASHTAG":
					drawSettings.markerShape = ScottPlot.MarkerShape.hashTag;
					break;
				case "CROSS":
					drawSettings.markerShape = ScottPlot.MarkerShape.cross;
					break;
				case "EKS":
					drawSettings.markerShape = ScottPlot.MarkerShape.eks;
					break;
				case "VERTICAL BAR":
					drawSettings.markerShape = ScottPlot.MarkerShape.verticalBar;
					break;
				case "TRI UP":
					drawSettings.markerShape = ScottPlot.MarkerShape.triUp;
					break;
				case "TRI DOWN":
					drawSettings.markerShape = ScottPlot.MarkerShape.triDown;
					break;
				case "NONE":
					drawSettings.markerShape = ScottPlot.MarkerShape.none;
					break;
			}
			return drawSettings;
		}
		private void PrefabSeries_Click(object sender, RoutedEventArgs e)
		{
			string plotType = ((MenuItem)e.OriginalSource).Header.ToString().ToUpperInvariant();

			PlotType type = new PlotType();
			switch (plotType)
			{
				case "SIGNAL":
					type = PlotType.signal;
					break;
			}

			SettingsDialog settingsDialog = new SettingsDialog(type);
			settingsDialog.Owner = App.Current.MainWindow;

			DrawSettings drawSettings;

			try
			{
				drawSettings = FetchSettingsFromDialog(settingsDialog, type);
			}
			catch
			{
				return;
			}

			Dictionary<string, object> metadata = new Dictionary<string, object>();

			if (type == PlotType.signal)
			{
				PrefabSignalDialog prefabSignalDialog = new PrefabSignalDialog();
				prefabSignalDialog.Owner = App.Current.MainWindow;
				if (prefabSignalDialog.ShowDialog() != true) //Nullable
				{
					return;
				}

				double cycleCount = 10;
				double frequency = 1000;
				WaveType waveType = WaveType.sine;

				if (!double.TryParse(prefabSignalDialog.cycleCountTextBox.Text, out cycleCount))
				{
					return;
				}

				if (!double.TryParse(prefabSignalDialog.frequencyTextBox.Text, out frequency))
				{
					return;
				}

				switch (prefabSignalDialog.waveTypeComboBox.Text.ToUpperInvariant())
				{
					case "SINE WAVE":
						waveType = WaveType.sine;
						break;
					case "SQUARE WAVE":
						waveType = WaveType.square;
						break;
				}

				SignalFrequencyDialog dlg = new SignalFrequencyDialog(true);
				dlg.Owner = App.Current.MainWindow;
				if (dlg.ShowDialog() != true)
				{ //Nullable
					return;
				}

				double sampleRate = 100;
				double.TryParse(dlg.frequency, out sampleRate);

				double xOffset = 0;
				double.TryParse(dlg.xOffsetTextBox.Text, out xOffset);

				metadata.Add("xOffset", xOffset);

				StringBuilder dataStr = new StringBuilder();

				int upperBound = (int)Math.Ceiling(cycleCount * frequency);
				double[] data = new double[upperBound];


				metadata.Add("sampleRate", frequency * frequency);

				if (waveType == WaveType.sine)
				{
					data = ScottPlot.DataGen.Sin(upperBound, cycleCount);
				}
				else if (waveType == WaveType.square)
				{
					bool high = false;
					for (int i = 0; i < upperBound; i++)
					{
						if(i % (frequency / 2) == 0)
						{
							high = !high;
						}

						data[i] = high ? 1 : 0;
					}
				}

				for (int i = 0; i < upperBound; i++)
				{
					dataStr.Append(data[i]);
					if (i != upperBound - 1)
					{
						dataStr.Append(',');
					}
				}


				((App)App.Current).AddSeriesFromString(dataStr.ToString(), drawSettings, metadata);
			}
		}
		private void LoadCSVSeries_Click(object sender, RoutedEventArgs e)
		{
			string plotType = ((MenuItem)e.OriginalSource).Header.ToString().ToUpperInvariant();

			PlotType type = new PlotType();
			switch (plotType)
			{
				case "SCATTER PLOT":
					type = PlotType.scatter;
					break;
				case "SIGNAL":
					type = PlotType.signal;
					break;
				case "BAR PLOT":
					type = PlotType.bar;
					break;
			}

			SettingsDialog settingsDialog = new SettingsDialog(type);
			settingsDialog.Owner = App.Current.MainWindow;

			DrawSettings drawSettings;

			try
			{
				drawSettings = FetchSettingsFromDialog(settingsDialog, type);
			}
			catch
			{
				return;
			}

			Dictionary<string, object> metadata = new Dictionary<string, object>();

			if (type == PlotType.signal)
			{
				SignalFrequencyDialog dlg = new SignalFrequencyDialog();
				dlg.Owner = App.Current.MainWindow;
				if (dlg.ShowDialog() != true)
				{ //Nullable
					return;
				}

				double sampleRate = 100;
				double.TryParse(dlg.frequency, out sampleRate);

				double xOffset = 0;
				double.TryParse(dlg.xOffsetTextBox.Text, out xOffset);

				metadata.Add("sampleRate", sampleRate);
				metadata.Add("xOffset", xOffset);
			}


			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				PlotParameters plotParams = (App.Current as App).AddSeriesFromCSVFile(openFileDialog.FileName, drawSettings, metadata);
				if (settingsDialog.errorDataCSV != null)
				{
					((App)App.Current).AddErrorFromCSVFile(plotParams, settingsDialog.errorDataCSV);
				}
				statusMessage.Text = $"{openFileDialog.FileName} loaded";
			}
		}

		private void ClearPlot_Click(object sender, RoutedEventArgs e)
		{
			((MainWindow)App.Current.MainWindow).ClearPlot();
			statusMessage.Text = "No file loaded";
		}

		private void PlotSave_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.FileName = "plot";
			dlg.DefaultExt = ".png";

			bool? result = dlg.ShowDialog();

			if (result == true)
			{ //Nullable
				((MainWindow)App.Current.MainWindow).SavePlot(dlg.FileName);
			}
		}

		private void FrameSettings_Click(object sender, RoutedEventArgs e)
		{
			FrameSettingsDialog dlg = new FrameSettingsDialog();
			dlg.Owner = App.Current.MainWindow;
			MainWindow mainWindow = (MainWindow)App.Current.MainWindow;

			dlg.titleTextBox.Text = mainWindow.plotTitle;
			dlg.xLabelTextBox.Text = mainWindow.xLabel;
			dlg.yLabelTextBox.Text = mainWindow.yLabel;
			dlg.logAxis.IsChecked = mainWindow.logAxis;

			if (dlg.ShowDialog() != true)
			{ //Nullable
				return;
			}

			mainWindow.plotTitle = dlg.titleTextBox.Text;
			mainWindow.xLabel = dlg.xLabelTextBox.Text;
			mainWindow.yLabel = dlg.yLabelTextBox.Text;
			mainWindow.logAxis = dlg.logAxis.IsChecked == true; //It is nullable
			mainWindow.RefreshTitleAndAxis();
		}

		private void WindowSettings_Click(object sender, RoutedEventArgs e)
		{
			WindowSettingsDialog dlg = new WindowSettingsDialog();
			dlg.Owner = App.Current.MainWindow;

			if (dlg.ShowDialog() != true)
			{ //Nullable
				return;
			}

			MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
			double xMin, xMax, yMin, yMax;
			double.TryParse(dlg.xMin.Text, out xMin);
			double.TryParse(dlg.xMax.Text, out xMax);
			double.TryParse(dlg.yMin.Text, out yMin);
			double.TryParse(dlg.yMax.Text, out yMax);

			mainWindow.plotFrame.plt.Axis(xMin, xMax, yMin, yMax);
			mainWindow.plotFrame.Render();
		}
	}
}
