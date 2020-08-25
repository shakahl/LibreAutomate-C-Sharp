using Au.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
//using System.Linq;

namespace Au.Controls
{
	public class SciHost : HwndHost
	{

		static SciHost() {
			AFile.More.LoadDll64or32Bit("SciLexer.dll");
		}

		#region HwndHost

		protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
			var c = AWnd.More.CreateWindow("Scintilla", null, WS.CHILD | WS.VISIBLE, 0, 0, 0, 200, 100, (AWnd)hwndParent.Handle);

			return new HandleRef(this, c.Handle);
		}

		protected override void DestroyWindowCore(HandleRef hwnd) {
			AWnd.More.DestroyWindow((AWnd)hwnd.Handle);
		}

		#endregion

		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			//AWnd.More.PrintMsg((AWnd)hwnd, msg, wParam, lParam);

			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}
	}
}
