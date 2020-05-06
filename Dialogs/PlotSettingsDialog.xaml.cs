using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
		PlotType type;

		public SettingsDialog(PlotType plotType = PlotType.scatter)
		{
			InitializeComponent();

			type = plotType;

			Resources["colour"] = ConvertFromSystemDrawingColor(plotColour);
			colourTextBox.Text = plotColour.ToArgb().ToString("X");
			Resources["scatterSettingsVisibility"] = plotType == PlotType.scatter ? Visibility.Visible : Visibility.Collapsed;
			Resources["errorSettingsVisibility"] = plotType == PlotType.scatter || plotType == PlotType.bar || plotType == PlotType.bar_grouped ? Visibility.Visible : Visibility.Collapsed;
			Resources["scatterBarSettingsVisibility"] = plotType == PlotType.scatter || plotType == PlotType.bar ? Visibility.Visible : Visibility.Collapsed;
			Resources["histogramSettingsVisibility"] = plotType == PlotType.histogram ? Visibility.Visible : Visibility.Collapsed;
			Resources["labelSettingsVisibility"] = plotType != PlotType.bar_grouped ? Visibility.Visible : Visibility.Collapsed;
			Resources["colourSettingsVisibility"] = plotType != PlotType.bar_grouped ? Visibility.Visible : Visibility.Collapsed;
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
			DialogResult = true;
			this.Close();
		}

		private void AddErrorCSVButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = type == PlotType.bar_grouped;
			if (openFileDialog.ShowDialog() == true)
			{
				if (type != PlotType.bar_grouped)
				{
					errorDataCSV = openFileDialog.FileName;
				}
				else
				{
					errorDataCSV = String.Join(",", openFileDialog.FileNames);
				}

				CSVFileTextBlock.Text = errorDataCSV;
			}

		}
	}
}
