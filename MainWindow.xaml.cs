using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Where1.WPlot 
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			plotFrame.plt.PlotSignal(ScottPlot.DataGen.Sin(50));
		}

		public void ClearPlot() {
			plotFrame.plt.Clear();
			plotFrame.Render();
		}

		public void RenderPlot() {
			double[] xs = (App.Current as App).xs;
			double[] ys = (App.Current as App).ys;

			plotFrame.plt.PlotScatter(xs, ys);
			plotFrame.Render();
		}
	}
}
