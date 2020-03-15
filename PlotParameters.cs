using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Where1.WPlot
{
	public class PlotParameters
	{
		public object data; //It has to be able to accept diverse data, e.g. OHLC in addition to just coordinates
		public object errorData;
		public bool hasErrorData;
		public Dictionary<string, object> metaData; //Ditto
		public DrawSettings drawSettings;
	}

	public struct DrawSettings
	{
		public PlotType type;
		public System.Drawing.Color colour;
		public bool drawLine;
		public ScottPlot.MarkerShape markerShape;
		public string label;
		public bool drawLinearRegression;
	}

	public enum PlotType
	{
		scatter,
		signal,
		bar,
		histogram
	}

	public enum WaveType
	{
		sine,
		square
	}

	[Flags]
	public enum HistogramType { 
		count,
		fraction,
		density,
		cumulative,
	}
}
