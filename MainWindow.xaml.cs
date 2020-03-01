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

			//plotFrame.plt.PlotSignal(ScottPlot.DataGen.Sin(50));
		}

		public void ClearPlot()
		{
			plotFrame.plt.Clear();
			plotFrame.Render();
			(App.Current as App).ClearSeries();
		}

		public void RenderPlot()
		{
			plotFrame.plt.Clear();
			foreach (PlotParameters curr in ((App)App.Current).GetSeries())
			{
				
				switch (curr.type)
				{
					case PlotType.scatter:
						plotFrame.plt.PlotScatter((curr.data as double[][])[0], (curr.data as double[][])[1]);
						break;
					case PlotType.signal:
						object sampleRate= 100;
						curr.metaData.TryGetValue("sampleRate", out sampleRate);
						plotFrame.plt.PlotSignal((double[])curr.data, (double)sampleRate);
						break;
				}
			}
			plotFrame.Render();
		}
	}
}
