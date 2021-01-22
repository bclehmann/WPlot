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
	/// Interaction logic for SpecificErrorDialog.xaml
	/// </summary>
	public partial class SpecificErrorDialog : Window
	{
		private string errorType;
		private string errorBlurb;
		private Exception error;
		public SpecificErrorDialog(string errorType, string errorBlurb, Exception error)
		{
			InitializeComponent();

			this.errorType = errorType;
			this.errorBlurb = errorBlurb;
			this.error = error;

			ErrorTitle.Text = errorType;
			ErrorBlurb.Text = errorBlurb;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			this.Close();
		}

		private void MoreInfoButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(App.Current.MainWindow, $"Details for nerds:\n{error}", errorType, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
