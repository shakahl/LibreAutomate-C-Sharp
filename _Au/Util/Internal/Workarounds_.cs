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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
//using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au.Util
{
	static partial class Workarounds_
	{
		static bool s_workaroundWaitCursor;

		/// <summary>
		/// A workaround for:
		/// 	When showing a ContextMenuStrip menu or a submenu, something briefly sets the AppStarting cursor.
		///		Only if this process does/did not have active windows.
		/// 	Only if current cursor is arrow. Works well if eg I or hand.
		/// 	Starts before first WM_NCPAINT (after WM_SHOWWINDOW+WM_WINDOWPOSCHANGING) and stops before WM_PAINT.
		/// Call this before showing a menu. Possibly also will need for some other windows.
		/// </summary>
		internal static unsafe void WaitCursorWhenShowingMenuEtc()
		{
			//APerf.First();
			if(s_workaroundWaitCursor) return; s_workaroundWaitCursor = true;

			//Print(Api.GetActiveWindow());
			if(AWnd.Active.IsOfThisProcess) return;
#if true
			AWnd w = AWnd.More.CreateMessageOnlyWindow("#32770");
			//info: HWND_MESSAGE makes much faster; WS_EX_NOACTIVATE makes 20% faster and prevents setting foreground; empty class same speed.
			Api.SetActiveWindow(w); //sets foreground only if a window of this thread is the foreground window. SetFocus too, but slightly slower.

			//Api.SetForegroundWindow(w);
			//Print(AWnd.Active);
			AWnd.More.DestroyWindow(w);
			//Print(AWnd.Active);
			//APerf.NW(); //1 ms when ngened, else 2 ms
#else
			//This makes startup faster. Also, if in same thread, it can take much more time, don't know why, depending on where called.
			ThreadStart d = () =>
			{
				AWnd w = Api.CreateWindowEx(WS_EX.NOACTIVATE, "#32770", null, WS.POPUP, 0, 0, 0, 0, Native.HWND_MESSAGE, 0, default, 0);
				//info: HWND_MESSAGE makes much faster; WS_EX_NOACTIVATE makes 20% faster; empty class same speed.
				//w.Focus();
				Api.SetActiveWindow(w); //sets foreground only if a window of this thread is the foreground window. SetFocus too, but slightly slower.
				//Print(AWnd.Active);
				//APerf.NW();
				AWnd.More.DestroyWindow(w);
			};
			if(Environment.ProcessorCount > 1) {
				var t = new Thread(d);
				t.SetApartmentState(ApartmentState.STA);
				t.Priority = ThreadPriority.Highest;
				t.Start();
				//t.Join(); //not so important. Normally the new thread finishes its work faster than we'll need the result.
				Thread.Yield(); //because: 1. All logical processors may be busy. 2. Environment.ProcessorCount does not know about process affinity, we actually may have just 1 logical processor.
			} else d();
#endif
		}
	}
}
