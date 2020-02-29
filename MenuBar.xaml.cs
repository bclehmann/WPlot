using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Where1.WPlot
{
	partial class MenuBar
	{
		private void LoadCSVSeries(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true) {
				(App.Current as App).ReadCSVFile(openFileDialog.FileName);
				statusMessage.Text = $"{openFileDialog.FileName} loaded";
			}
		}

		private void ClearPlot(object sender, RoutedEventArgs e)
		{
			(App.Current.MainWindow as MainWindow).ClearPlot();
			statusMessage.Text = "No file loaded";
		}
	}
}
