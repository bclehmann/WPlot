using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Where1.WPlot
{
	/// <summary>
	/// Interaction logic for SettingsDialog.xaml
	/// </summary>
	public partial class SettingsDialog : Window
	{
		public System.Drawing.Color plotColour = ((MainWindow)App.Current.MainWindow).NextColour();
		public string errorDataCSV;

		public SettingsDialog(PlotType plotType = PlotType.scatter)
		{
			InitializeComponent();

			Resources["colour"] = ConvertFromSystemDrawingColor(plotColour);
			colourTextBox.Text = plotColour.ToArgb().ToString("X");
			Resources["scatterSettingsVisibility"] = plotType == PlotType.scatter ? Visibility.Visible : Visibility.Collapsed;
			Resources["errorSettingsVisibility"] = plotType == PlotType.scatter ? Visibility.Visible : Visibility.Collapsed;
		}

		private void colourTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (colourTextBox.Text.Length != 8)
			{
				return;
			}
			int colour = 0;
			int.TryParse(colourTextBox.Text, System.Globalization.NumberStyles.HexNumber, null, out colour);
			plotColour = System.Drawing.Color.FromArgb(colour);

			Resources["colour"] = ConvertFromSystemDrawingColor(plotColour);
		}

		private Color ConvertFromSystemDrawingColor(System.Drawing.Color drawingColour)
		{
			Color colour = new Color(); //ScottPlot uses System.Drawing.Color, which is different than what WPF uses :/
			colour.R = drawingColour.R;
			colour.G = drawingColour.G;
			colour.B = drawingColour.B;
			colour.A = drawingColour.A;
			return colour;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void AddErrorCSVButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				errorDataCSV = openFileDialog.FileName;
				CSVFileTextBlock.Text = errorDataCSV;
			}

		}
	}
}
