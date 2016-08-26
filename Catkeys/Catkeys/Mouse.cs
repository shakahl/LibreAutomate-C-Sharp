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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	//[DebuggerStepThrough]
	public static class Mouse
	{
		public static POINT XY { get { POINT p; Api.GetCursorPos(out p); return p; } }
		public static int X { get { POINT p = XY; return p.x; } }
		public static int Y { get { POINT p = XY; return p.y; } }
	}

	//TODO: create file Input.cs
	public static class Input
	{
		/// <summary>
		/// Gets current text cursor (caret) rectangle in screen coordinates.
		/// Returns the control that contains it.
		/// If there is no text cursor or cannot get it (eg it is not a standard text cursor), gets mouse pointer coodinates and returns Wnd0.
		/// </summary>
		public static Wnd GetTextCursorRect(out RECT r)
		{
			Api.GUITHREADINFO g;
			if(Wnd.Misc.GetGUIThreadInfo(out g) && !g.hwndCaret.Is0) {
				if(g.rcCaret.bottom <= g.rcCaret.top) g.rcCaret.bottom = g.rcCaret.top + 16;
				r = g.rcCaret;
				g.hwndCaret.MapClientToScreen(ref r);
				return g.hwndCaret;
			}

			POINT p; Api.GetCursorPos(out p);
			r = new RECT(p.x, p.y, 0, 16, true);
			return Wnd0;

			//note: in Word, after changing caret pos, gets pos 0 0. After 0.5 s gets correct. After typing always correct.
			//tested: accessibleobjectfromwindow(objid_caret) is the same, but much slower.
		}
	}
}
