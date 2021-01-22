using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Where1.WPlot
{
	public static class DialogUtilities
	{
		public static void ShowGenericPlotError()
		{
			MessageBox.Show(App.Current.MainWindow, "Something went wrong. Your plot was not added. Make sure your data is of the right format for the settings you chose.", "Unknown Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public static void ShowSpecificPloterror(string errorType, Exception error, bool shouldExit = false, string errorBlurb = "Something went wrong.")
		{
			var dlg = new SpecificErrorDialog(errorType, errorBlurb, error);
			dlg.Owner = App.Current.MainWindow;
			dlg.ShowDialog();

			if (shouldExit)
			{
				App.Current.MainWindow.Close();
			}
		}

	}
}
