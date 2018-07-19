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
using static Au.NoClass;

namespace Au.Util
{
	internal static partial class LibWorkarounds
	{
		internal struct ProcessVariables
		{
			public bool doneWaitCursorWhenShowingMenuEtc;
		}
		static bool _workaroundWaitCursor;

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
			//Perf.First();
			//tested: this workaround can be executed once in process, in any thread.
			if(_workaroundWaitCursor) return; _workaroundWaitCursor = true;
			var onceInProcess = &LibProcessMemory.Ptr->workarounds.doneWaitCursorWhenShowingMenuEtc;
			if(*onceInProcess) return; *onceInProcess = true;
			//Print("workaround", AppDomain.CurrentDomain.Id);

			//Print(Api.GetActiveWindow());
			if(Wnd.Active.IsOfThisProcess) return;
#if true
			Wnd w = Wnd.Misc.CreateMessageWindow("#32770");
			//info: HWND_MESSAGE makes much faster; WS_EX_NOACTIVATE makes 20% faster and prevents setting foreground; empty class same speed.
			Api.SetActiveWindow(w); //sets foreground only if a window of this thread is the foreground window. SetFocus too, but slightly slower.

			//Api.SetForegroundWindow(w);
			//Print(Wnd.Active);
			Wnd.Misc.DestroyWindow(w);
			//Print(Wnd.Active);
			//Perf.NW(); //1 ms when ngened, else 2 ms
#else
			//This makes startup faster. Also, if in same thread, it can take much more time, don't know why, depending on where called.
			ThreadStart d = () =>
			{
				Wnd w = Api.CreateWindowEx(Native.WS_EX.NOACTIVATE, "#32770", null, Native.WS.POPUP, 0, 0, 0, 0, Native.HWND_MESSAGE, 0, default, 0);
				//info: HWND_MESSAGE makes much faster; WS_EX_NOACTIVATE makes 20% faster; empty class same speed.
				//w.FocusControlOfThisThread();
				Api.SetActiveWindow(w); //sets foreground only if a window of this thread is the foreground window. SetFocus too, but slightly slower.
				//Print(Wnd.Active);
				//Perf.NW();
				Wnd.Misc.DestroyWindow(w);
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

	/// <summary>
	/// Activates our manifest which tells to use comctl32.dll version 6.
	/// The manifest is embedded in this dll, resource id 2.
	/// This code is from .NET MessageBox.Show.
	/// </summary>
	internal static class LibActCtx
	{
		static Api.ACTCTX _actCtx;
		static IntPtr _hActCtx;
		static bool _contextCreationSucceeded;

		static LibActCtx()
		{
			_actCtx.cbSize = Api.SizeOf<Api.ACTCTX>();
			_actCtx.lpSource = Assembly.GetExecutingAssembly().Location;
			_actCtx.lpResourceName = (IntPtr)2;
			_actCtx.dwFlags = Api.ACTCTX_FLAG_RESOURCE_NAME_VALID;

			_hActCtx = Api.CreateActCtx(_actCtx);
			_contextCreationSucceeded = (_hActCtx != new IntPtr(-1));
		}

		static bool IsContextActive()
		{
			IntPtr current = default;

			if(_contextCreationSucceeded && Api.GetCurrentActCtx(out current)) {
				return current == _hActCtx;
			}
			return false;
		}

		internal static IntPtr Activate()
		{
			IntPtr R = default;

			if(_contextCreationSucceeded) {
				if(!IsContextActive()) {
					if(!Api.ActivateActCtx(_hActCtx, out R)) R = default;
				}
			}

			return R;
		}

		internal static void Deactivate(IntPtr userCookie)
		{
			if(userCookie != default) Api.DeactivateActCtx(0, userCookie);
		}
	}

}
