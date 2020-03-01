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
			SettingsDialog settingsDialog = new SettingsDialog();

			DrawSettings drawSettings = new DrawSettings();

			settingsDialog.ShowDialog();

			string plotType = ((MenuItem) e.OriginalSource).Header.ToString();

			PlotType type = new PlotType();
			switch (plotType.ToUpperInvariant())
			{
				case "SCATTER PLOT":
					type = PlotType.scatter;
					break;
				case "SIGNAL":
					type = PlotType.signal;
					break;
			}

			drawSettings.colour = settingsDialog.plotColour;
			drawSettings.drawLine = settingsDialog.drawLine;
			drawSettings.type = type;


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
				(App.Current as App).AddSeriesFromFile(openFileDialog.FileName, drawSettings, metadata);
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

		private void PlotSettings_Click(object sender, RoutedEventArgs e)
		{
			FrameSettingsDialog dlg = new FrameSettingsDialog();
			dlg.ShowDialog();

			MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
			mainWindow.plotTitle = dlg.titleTextBox.Text;
			mainWindow.xLabel = dlg.xLabelTextBox.Text;
			mainWindow.yLabel = dlg.yLabelTextBox.Text;
		}
	}
}
