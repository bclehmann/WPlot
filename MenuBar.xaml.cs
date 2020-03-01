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
			OpenFileDialog openFileDialog = new OpenFileDialog();
			string plotType = ((MenuItem) e.OriginalSource).Header.ToString();
			Dictionary<string, object> metadata = new Dictionary<string, object>();

			if (plotType.ToUpperInvariant() == "SIGNAL") {
				SignalFrequencyDialog dlg = new SignalFrequencyDialog();
				dlg.Owner = App.Current.MainWindow;
				dlg.ShowDialog();

				double sampleRate = 100;
				double.TryParse(dlg.frequency, out sampleRate);
				metadata.Add("sampleRate", sampleRate);
			}
			if (openFileDialog.ShowDialog() == true)
			{
				(App.Current as App).AddSeriesFromFile(openFileDialog.FileName, plotType, metadata);
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
	}
}
