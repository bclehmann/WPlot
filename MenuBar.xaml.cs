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

				// Configure the dialog box
				dlg.Owner = App.Current.MainWindow;

				// Open the dialog box modally 
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

		private void ClearPlot(object sender, RoutedEventArgs e)
		{
			((MainWindow) App.Current.MainWindow).ClearPlot();
			statusMessage.Text = "No file loaded";
		}
	}
}
