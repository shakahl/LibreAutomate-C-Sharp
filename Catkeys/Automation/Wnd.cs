using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;
using System.Windows.Forms;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Automation
{
	public struct Wnd :IWin32Window
	{
		IntPtr _h;
		public static readonly Wnd Zero = default(Wnd);

		//public static readonly Wnd HWND_TOP = (Wnd)0; //SetWindowPos(HWND_TOP)
		//public static readonly Wnd HWND_BOTTOM = (Wnd)1; //SetWindowPos(HWND_BOTTOM)
		//public static readonly Wnd HWND_TOPMOST = (Wnd)(-1); //SetWindowPos(HWND_TOPMOST)
		//public static readonly Wnd HWND_NOTOPMOST = (Wnd)(-2); //SetWindowPos(HWND_NOTOPMOST)
		//public static readonly Wnd HWND_MESSAGE = (Wnd)(-3); //CreateWindowEx(HWND_MESSAGE)
		//public static readonly Wnd HWND_BROADCAST = (Wnd)0xffff; //SendMessage(HWND_BROADCAST)
		//or
		//public enum WndSpec
		//{
		//	Zero = 0, //or Wnd.Zero
		//	Top = 0, //SetWindowPos(HWND_TOP)
		//	Bottom = 1, //SetWindowPos(HWND_BOTTOM)
		//	Topmost = -1, //SetWindowPos(HWND_TOPMOST)
		//	NoTopmost = -2, //SetWindowPos(HWND_NOTOPMOST)
		//	Message = -3, //CreateWindowEx(HWND_MESSAGE)
		//	Broadcast = 0xffff //SendMessage(HWND_BROADCAST)
		//}

		public Wnd(IntPtr hwnd) { _h=hwnd; }
		public Wnd(string name) { _h=Find(name); } //if need class etc, use Find() instead.

		public static implicit operator Wnd(IntPtr hwnd) { return new Wnd(hwnd); } //Hwnd=IntPtr
		public static implicit operator IntPtr(Wnd w) { return w._h; } //IntPtr=Hwnd
																	   //public static explicit operator Wnd(int hwnd) { return new Wnd((IntPtr)hwnd); } //Wnd=(Wnd)int

		public IntPtr Handle { get { return _h; } } //IWin32Window

		public static Wnd Find(string name)
		{
			return Api.FindWindow(null, name);
			// return Zero;
		}

		public override string ToString() { return _h.ToString(); }

		public string ToStringEx() { return _h.ToString(); } //TODO: get name class etc, like outw; support Out(Hwnd).

		public void ZorderAfter(Wnd anoterWindow) { }
		public void ZorderTop() { }
		public void ZorderBottom() { }
		public void ZorderTopmost() { }
		public void ZorderNotopmost() { }
	}
}
