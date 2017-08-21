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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// TODO
	/// </summary>
	public class Acc
	{
		internal Api.IAccessible a;
		internal int elem;

		public Wnd WndDirectParent
		{
			get
			{
				CatException.ThrowIfFailed(Api.WindowFromAccessibleObject(a, out var w), "*get object's parent window");
				return w;
			}
		}

		public RECT Rect
		{
			get
			{
				CatException.ThrowIfFailed(a.accLocation(out var x, out var y, out var cx, out var cy, elem), "*get object's rectangle");
				return new RECT(x, y, cx, cy, true);
			}
		}

		public RECT RectInClientOf(Wnd w)
		{
			var r = Rect;
			if(!w.MapScreenToClient(ref r)) w.ThrowUseNative();
			return r;
		}
	};

	//internal struct LibAccVariant
	//{

	//}
}
