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
	/// Interaction logic for FrameSettingsDialog.xaml
	/// </summary>
	public partial class FrameSettingsDialog : Window
	{
		public FrameSettingsDialog()
		{
			InitializeComponent();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
