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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace G.Controls
{
	partial class GDockPanels
	{
		class GPanel :GContentNode
		{
			internal Control Content;
			internal Image CaptionImage;
			internal GTab ParentTab;

			internal GPanel(_InitData d, XmlElement x, GSplit parentSplit) : base(d.manager, parentSplit)
			{
				d.manager._aPanel.Add(this);

				var name = x.Attributes["name"].Value;
				var c = d.controls[name];
				this.Content = c.c;
				if(c.img != null) this.CaptionImage = c.img; else if(d.imageList != null && c.ii >= 0) this.CaptionImage = d.imageList.Images[c.ii];
			}

			internal override string Name { get { return this.Content.Text; } }
		}

		class GDocument :GPanel
		{
			internal GDocument(_InitData d, XmlElement x, GSplit parentSplit) : base(d, x, parentSplit)
			{

			}

			internal override string Name { get { return "document.cs"; } }
		}
	}
}
