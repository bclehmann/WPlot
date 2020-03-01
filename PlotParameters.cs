using System;
using System.Collections.Generic;
using System.Text;

namespace Where1.WPlot
{
	public struct PlotParameters
	{
		public object data; //It has to be able to accept diverse data, e.g. OHLC in addition to just coordinates
		public Dictionary<string, object> metaData; //Ditto
		public PlotType type;
	}

	public enum PlotType { 
		scatter,
		signal
	}
}
