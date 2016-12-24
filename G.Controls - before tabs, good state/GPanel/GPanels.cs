using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace G.Controls
{
	//[DebuggerStepThrough]
	public sealed partial class GPanels
	{
		internal readonly Control Host;
		internal readonly DockOverlay Overlay;
		//internal readonly DocIndicator Indicator;

		public GPanels(Control dockHost)
		{
			Host = dockHost;
			Overlay = new DockOverlay();
			Panels = new GPanelCollection();
		}

		/// <summary>
		/// All GPanel controls.
		/// </summary>
		public GPanelCollection Panels { get; private set; }

		public class GPanelCollection :List<GPanel>
		{
			public GPanel Find(GPanel panel)
			{
				foreach(GPanel p in this) {
					if(p.Equals(panel))
						return p;
				}
				return null;
			}
		}

		public void Test()
		{
			foreach(var p in Panels) {
				OutList(p, p.Dock);
				if(p is GPanel.MultiGPanel) {
					foreach(Control pp in p.Controls) {
						OutList("--", pp, pp.Dock);
					}
				}
			}

		}
	}
}
