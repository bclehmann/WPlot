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
	/// Interaction logic for WindowSettingsDialog.xaml
	/// </summary>
	public partial class WindowSettingsDialog : Window
	{
		public WindowSettingsDialog()
		{
			InitializeComponent();

			MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
			xMin.Text = $"{mainWindow.plotFrame.plt.GetSettings().axes.x.min:f9}";
			xMax.Text = $"{mainWindow.plotFrame.plt.GetSettings().axes.x.max:f9}";
			yMin.Text = $"{mainWindow.plotFrame.plt.GetSettings().axes.y.min:f9}";
			yMax.Text = $"{mainWindow.plotFrame.plt.GetSettings().axes.y.max:f9}";
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
