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
using System.Linq;

using Au.Types;
using Au.Util;
using static Au.AStatic;

namespace Au
{
	/// <summary>
	/// Helps with accessible object event hooks. See API <msdn>SetWinEventHook</msdn>.
	/// </summary>
	/// <remarks>
	/// The thread that uses hooks must process Windows messages. For example have a window/dialog/messagebox, or use a 'wait-for' function that dispatches messages or has such option (see <see cref="AOpt.WaitFor"/>).
	/// The variable must be disposed, either explicitly (call <b>Dispose</b> or <b>Unhook</b>) or with the 'using' pattern.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// bool stop = false;
	/// using(new AHookAcc(AccEVENT.SYSTEM_FOREGROUND, 0, x =>
	/// {
	/// 	Print(x.wnd);
	/// 	var a = x.GetAcc();
	/// 	Print(a);
	/// 	if(x.wnd.ClassNameIs("Shell_TrayWnd")) stop = true;
	/// })) {
	/// 	MessageBox.Show("hook");
	/// 	//or
	/// 	//AWaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for activated taskbar
	/// 	//Print("the end");
	/// }
	/// ]]></code>
	/// </example>
	public class AHookAcc : IDisposable
	{
		IntPtr _hh; //HHOOK
		IntPtr[] _ahh; //multiple HHOOK
		Api.WINEVENTPROC _proc1; //our intermediate hook proc that calls _proc2
		Action<HookData.AccHookData> _proc2; //caller's hook proc

		/// <summary>
		/// Sets a hook for an event or a range of events.
		/// Calls API <msdn>SetWinEventHook</msdn>.
		/// </summary>
		/// <param name="eventMin">The lowest event constant value in the range of events. Can be AccEVENT.MIN to indicate the lowest possible event value. Events reference: <msdn>SetWinEventHook</msdn>.</param>
		/// <param name="eventMax">The highest event constant value in the range of events. Can be AccEVENT.MAX to indicate the highest possible event value. If 0, uses <i>eventMin</i>.</param>
		/// <param name="hookProc">The hook procedure (function that handles hook events).</param>
		/// <param name="idProcess">The id of the process from which the hook function receives events. If 0 - all processes on the current desktop.</param>
		/// <param name="idThread">The native id of the thread from which the hook function receives events. If 0 - all threads.</param>
		/// <param name="flags"></param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>See <see cref="AHookAcc"/>.</example>
		public AHookAcc(AccEVENT eventMin, AccEVENT eventMax, Action<HookData.AccHookData> hookProc, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			if(eventMax == 0) eventMax = eventMin;
			_proc1 = _HookProc;
			Hook(eventMin, eventMax, idProcess, idThread, flags);
			_proc2 = hookProc;
		}

		/// <summary>
		/// Sets a hook for multiple events.
		/// Calls API <msdn>SetWinEventHook</msdn>.
		/// </summary>
		/// <param name="events">Events. Reference: API <msdn>SetWinEventHook</msdn>. Elements with value 0 are ignored.</param>
		/// <param name="hookProc">The hook procedure (function that handles hook events).</param>
		/// <param name="idProcess">The id of the process from which the hook function receives events. If 0 - all processes on the current desktop.</param>
		/// <param name="idThread">The native id of the thread from which the hook function receives events. If 0 - all threads.</param>
		/// <param name="flags"></param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>See <see cref="AHookAcc"/>.</example>
		public AHookAcc(AccEVENT[] events, Action<HookData.AccHookData> hookProc, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			_proc1 = _HookProc;
			Hook(events, idProcess, idThread, flags);
			_proc2 = hookProc;
		}

		/// <summary>
		/// Sets hooks again after <see cref="Unhook"/>.
		/// </summary>
		/// <remarks>
		/// Parameters are the same as of the constructor, but values can be different.
		/// </remarks>
		public void Hook(AccEVENT eventMin, AccEVENT eventMax, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			_Throw1();
			_hh = Api.SetWinEventHook(eventMin, eventMax, default, _proc1, idProcess, idThread, flags);
			if(_hh == default) throw new AuException(0, "*set hook");
		}

		/// <summary>
		/// Sets hooks again after <see cref="Unhook"/>.
		/// </summary>
		/// <remarks>
		/// Parameters are the same as of the constructor, but values can be different.
		/// </remarks>
		public void Hook(AccEVENT[] events, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			_Throw1();
			_ahh = new IntPtr[events.Length];
			for(int i = 0; i < events.Length; i++) {
				var e = events[i]; if(e == 0) continue;
				var hh = Api.SetWinEventHook(e, e, default, _proc1, idProcess, idThread, flags);
				if(hh == default) { var ec = ALastError.Code; Unhook(); throw new AuException(ec, "*set hook for " + e.ToString()); }
				_ahh[i] = hh;
			}
		}

		void _Throw1()
		{
			if(_hh != default || _ahh != null) throw new InvalidOperationException();
			if(_proc1 == null) throw new ObjectDisposedException(nameof(AHookAcc));
		}

		/// <summary>
		/// Removes the hook.
		/// </summary>
		/// <remarks>
		/// Does nothing if already removed or wasn't set.
		/// Must be called from the same thread that sets the hook.
		/// </remarks>
		public void Unhook()
		{
			if(_hh != default) {
				if(!Api.UnhookWinEvent(_hh)) PrintWarning("Failed to unhook AHookAcc.");
				_hh = default;
			} else if(_ahh != null) {
				foreach(var hh in _ahh) {
					if(hh == default) continue;
					if(!Api.UnhookWinEvent(hh)) PrintWarning("Failed to unhook AHookAcc.");
				}
				_ahh = null;
			}
		}

		/// <summary>
		/// Calls <see cref="Unhook"/>.
		/// </summary>
		public void Dispose()
		{
			Unhook();
			_proc1 = null;
			GC.SuppressFinalize(this);
		}

		//MSDN: UnhookWinEvent fails if called from a thread different from the call that corresponds to SetWinEventHook.
		///
		~AHookAcc() { PrintWarning("Non-disposed AHookAcc variable."); } //unhooking makes no sense

		void _HookProc(IntPtr hHook, AccEVENT ev, AWnd wnd, AccOBJID idObject, int idChild, int idThread, int eventTime)
		{
			try {
				_proc2(new HookData.AccHookData(this, ev, wnd, idObject, idChild, idThread, eventTime));
			}
			catch(Exception ex) { AHookWin.LibOnException(ex, this); }
		}
	}
}

namespace Au.Types
{
	public static partial class HookData
	{
		/// <summary>
		/// Hook data for the hook procedure set by <see cref="AHookAcc"/>.
		/// More info: API <msdn>WinEventProc</msdn>.
		/// </summary>
		public unsafe struct AccHookData
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AHookAcc hook;

			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly AWnd wnd;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly AccEVENT ev;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly AccOBJID idObject;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly int idChild;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly int idThread;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly int eventTime;

			internal AccHookData(AHookAcc hook, AccEVENT ev, AWnd wnd, AccOBJID idObject, int idChild, int idThread, int eventTime)
			{
				this.hook = hook;
				this.ev = ev;
				this.wnd = wnd;
				this.idObject = idObject;
				this.idChild = idChild;
				this.idThread = idThread;
				this.eventTime = eventTime;
			}

			/// <summary>
			/// Calls <see cref="AAcc.FromEvent"/>.
			/// </summary>
			public AAcc GetAcc()
			{
				return AAcc.FromEvent(wnd, idObject, idChild);
			}
		}
	}
}
