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

		public System.Drawing.Color NextColour() {
			return plotFrame.plt.GetSettings().GetNextColor();
		}

		public void RenderPlot()
		{
			plotFrame.plt.Clear();
			foreach (PlotParameters curr in ((App)App.Current).GetSeries())
			{
				
				switch (curr.drawSettings.type)
				{
					case PlotType.scatter:
						double[] xs = ((double[][])curr.data)[0];
						double[] ys = ((double[][])curr.data)[1];
						plotFrame.plt.PlotScatter(xs, ys, curr.drawSettings.colour, curr.drawSettings.drawLine ? 1 : 0);
						break;
					case PlotType.signal:
						object sampleRate= 100;
						curr.metaData.TryGetValue("sampleRate", out sampleRate);
						object xOffset = 0;
						curr.metaData.TryGetValue("xOffset", out xOffset);
						plotFrame.plt.PlotSignal((double[])curr.data, (double)sampleRate, (double)xOffset, color:curr.drawSettings.colour, lineWidth: curr.drawSettings.drawLine ? 1: 0);
						break;
				}
			}
			plotFrame.Render();
		}

		public void SavePlot(string path) {
			plotFrame.plt.SaveFig(path, false); //It's already been rendered
		}
	}
}
