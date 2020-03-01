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
		private void LoadCSVSeries_Click(object sender, RoutedEventArgs e)
		{
			string plotType = ((MenuItem) e.OriginalSource).Header.ToString().ToUpperInvariant();

			PlotType type = new PlotType();
			switch (plotType)
			{
				case "SCATTER PLOT":
					type = PlotType.scatter;
				break;
				case "SIGNAL":
					type = PlotType.signal;
				break;
			}

			SettingsDialog settingsDialog = new SettingsDialog(type==PlotType.signal);

			DrawSettings drawSettings = new DrawSettings();

			settingsDialog.ShowDialog();

			drawSettings.colour = settingsDialog.plotColour;
			drawSettings.drawLine = settingsDialog.shouldDrawLine.IsChecked == true; //Because it is nullable
			drawSettings.drawLinearRegression = settingsDialog.linreg.IsChecked == true; //Because it is nullable
			drawSettings.type = type;
			drawSettings.label = settingsDialog.plotNameTextBox.Text;

			string markerTypeName = settingsDialog.markerTypeComboBox.Text.ToUpperInvariant();
			switch (markerTypeName) {
				case "FILLED CIRCLE":
					drawSettings.markerShape = ScottPlot.MarkerShape.filledCircle;
					break;
				case "NONE":
					drawSettings.markerShape = ScottPlot.MarkerShape.none;
					break;
			}


			Dictionary<string, object> metadata = new Dictionary<string, object>();

			if (plotType.ToUpperInvariant() == "SIGNAL") {
				SignalFrequencyDialog dlg = new SignalFrequencyDialog();
				dlg.Owner = App.Current.MainWindow;
				dlg.ShowDialog();

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
				PlotParameters plotParams=(App.Current as App).AddSeriesFromCSVFile(openFileDialog.FileName, drawSettings, metadata);
				if (settingsDialog.errorDataCSV != null) {
					((App)App.Current).AddErrorFromCSVFile(plotParams, settingsDialog.errorDataCSV);
				}
				statusMessage.Text = $"{openFileDialog.FileName} loaded";
			}
		}

		private void ClearPlot_Click(object sender, RoutedEventArgs e)
		{
			((MainWindow) App.Current.MainWindow).ClearPlot();
			statusMessage.Text = "No file loaded";
		}

		private void PlotSave_Click(object sender, RoutedEventArgs e) {
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.FileName = "plot";
			dlg.DefaultExt = ".png";

			bool? result = dlg.ShowDialog();

			if (result == true) {
				((MainWindow)App.Current.MainWindow).SavePlot(dlg.FileName);
			}
		}

		private void FrameSettings_Click(object sender, RoutedEventArgs e)
		{
			FrameSettingsDialog dlg = new FrameSettingsDialog();
			MainWindow mainWindow = (MainWindow)App.Current.MainWindow;

			dlg.titleTextBox.Text = mainWindow.plotTitle;
			dlg.xLabelTextBox.Text = mainWindow.xLabel;
			dlg.yLabelTextBox.Text = mainWindow.yLabel;
			dlg.logAxis.IsChecked = mainWindow.logAxis;
			dlg.ShowDialog();

			mainWindow.plotTitle = dlg.titleTextBox.Text;
			mainWindow.xLabel = dlg.xLabelTextBox.Text;
			mainWindow.yLabel = dlg.yLabelTextBox.Text;
			mainWindow.logAxis = dlg.logAxis.IsChecked==true; //It is nullable
			mainWindow.RefreshTitleAndAxis();
		}

		private void WindowSettings_Click(object sender, RoutedEventArgs e) {
			WindowSettingsDialog dlg = new WindowSettingsDialog();
			dlg.ShowDialog();

			MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
			double xMin, xMax, yMin, yMax;
			double.TryParse(dlg.xMin.Text, out xMin);
			double.TryParse(dlg.xMax.Text, out xMax);
			double.TryParse(dlg.yMin.Text, out yMin);
			double.TryParse(dlg.yMax.Text, out yMax);

			mainWindow.plotFrame.plt.Axis(xMin,xMax,yMin,yMax);
			mainWindow.plotFrame.Render();
		}
	}
}
