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
using System.Linq;

using Au.Types;
using Au.Util;

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
	/// 	AOutput.Write(x.wnd);
	/// 	var a = x.GetAcc();
	/// 	AOutput.Write(a);
	/// 	if(x.wnd.ClassNameIs("Shell_TrayWnd")) stop = true;
	/// })) {
	/// 	MessageBox.Show("hook");
	/// 	//or
	/// 	//AWaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for activated taskbar
	/// 	//AOutput.Write("the end");
	/// }
	/// ]]></code>
	/// </example>
	public class AHookAcc : IDisposable
	{
		IntPtr[] _a;
		Api.WINEVENTPROC _proc1; //our intermediate hook proc that calls _proc2
		Action<HookData.AccHookData> _proc2; //caller's hook proc

		/// <summary>
		/// Sets a hook for an event or a range of events.
		/// </summary>
		/// <param name="eventMin">The lowest event constant value in the range of events. Can be AccEVENT.MIN to indicate the lowest possible event value. Events reference: <msdn>SetWinEventHook</msdn>. Value 0 is ignored.</param>
		/// <param name="eventMax">The highest event constant value in the range of events. Can be AccEVENT.MAX to indicate the highest possible event value. If 0, uses <i>eventMin</i>.</param>
		/// <param name="hookProc">The hook procedure (function that handles hook events).</param>
		/// <param name="idProcess">The id of the process from which the hook function receives events. If 0 - all processes on the current desktop.</param>
		/// <param name="idThread">The native id of the thread from which the hook function receives events. If 0 - all threads.</param>
		/// <param name="flags"></param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>See <see cref="AHookAcc"/>.</example>
		public AHookAcc(AccEVENT eventMin, AccEVENT eventMax, Action<HookData.AccHookData> hookProc, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			_proc1 = _HookProc;
			Hook(eventMin, eventMax, idProcess, idThread, flags);
			_proc2 = hookProc;
		}

		/// <summary>
		/// Sets multiple hooks.
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
		/// Sets a hook for an event or a range of events.
		/// </summary>
		/// <exception cref="InvalidOperationException">Hooks are already set and <see cref="Unhook"/> not called.</exception>
		/// <remarks>
		/// Parameters are the same as of the constructor, but values can be different.
		/// </remarks>
		public void Hook(AccEVENT eventMin, AccEVENT eventMax = 0, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			_Throw1();
			_a = new IntPtr[1];
			_SetHook(0, eventMin, eventMax, idProcess, idThread, flags);
		}

		/// <summary>
		/// Sets multiple hooks.
		/// </summary>
		/// <exception cref="InvalidOperationException">Hooks are already set and <see cref="Unhook"/> not called.</exception>
		/// <remarks>
		/// Parameters are the same as of the constructor, but values can be different.
		/// </remarks>
		public void Hook(AccEVENT[] events, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			_Throw1();
			_a = new IntPtr[events.Length];
			for(int i = 0; i < events.Length; i++) _SetHook(i, events[i], 0, idProcess, idThread, flags);
		}

		void _SetHook(int i, AccEVENT eMin, AccEVENT eMax, int idProcess, int idThread, AccHookFlags flags)
		{
			if(eMin == 0) return;
			if(eMax == 0) eMax = eMin;
			var hh = Api.SetWinEventHook(eMin, eMax, default, _proc1, idProcess, idThread, flags);
			if(hh == default) { var ec = ALastError.Code; Unhook(); throw new AuException(ec, "*set hook for " + eMin.ToString()); }
			_a[i] = hh;
		}

		void _Throw1()
		{
			if(_a != null) throw new InvalidOperationException();
			if(_proc1 == null) throw new ObjectDisposedException(nameof(AHookAcc));
		}

		/// <summary>
		/// Adds a hook for an event or a range of events.
		/// Returns an int value greater than 0 that can be used with <see cref="Remove"/>.
		/// </summary>
		/// <exception cref="AuException">Failed.</exception>
		/// <remarks>
		/// Parameters are the same as of the constructor, but values can be different.
		/// 
		/// This function together with <see cref="Remove"/> can be used to temporarily add/remove one or more hooks while using the same <b>AHookAcc</b> variable and hook procedure. Don't need to call <b>Unhook</b> before.
		/// </remarks>
		public int Add(AccEVENT eventMin, AccEVENT eventMax = 0, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			if(_proc1 == null) throw new ObjectDisposedException(nameof(AHookAcc));
			int i = 0;
			if(_a == null) {
				_a = new IntPtr[1];
			} else {
				for(; i < _a.Length; i++) if(_a[i] == default) goto g1;
				Array.Resize(ref _a, i + 1);
			}
			g1:
			_SetHook(i, eventMin, eventMax, idProcess, idThread, flags);
			return i + 1;
		}

		/// <summary>
		/// Removes a hook added by <see cref="Add"/>.
		/// </summary>
		/// <param name="addedId">A return value of <see cref="Add"/>.</param>
		/// <exception cref="ArgumentException"></exception>
		public void Remove(int addedId)
		{
			addedId--;
			if(_a == null || (uint)addedId >= _a.Length || _a[addedId] == default) throw new ArgumentException();
			if(!Api.UnhookWinEvent(_a[addedId])) AWarning.Write("Failed to unhook AHookAcc.");
			_a[addedId] = default;
		}

		///// <summary>
		///// True if hooks are set.
		///// </summary>
		//public bool Installed => _a != null;

		/// <summary>
		/// Removes all hooks.
		/// </summary>
		/// <remarks>
		/// Does nothing if already removed or wasn't set.
		/// Must be called from the same thread that sets the hook.
		/// </remarks>
		public void Unhook()
		{
			if(_a != null) {
				foreach(var hh in _a) {
					if(hh == default) continue;
					if(!Api.UnhookWinEvent(hh)) AWarning.Write("AHookAcc.Unhook failed.");
				}
				_a = null;
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
		~AHookAcc() { AWarning.Write("Non-disposed AHookAcc variable."); } //unhooking makes no sense

		void _HookProc(IntPtr hHook, AccEVENT ev, AWnd wnd, AccOBJID idObject, int idChild, int idThread, int eventTime)
		{
			try {
				_proc2(new HookData.AccHookData(this, ev, wnd, idObject, idChild, idThread, eventTime));
			}
			catch(Exception ex) { AHookWin.OnException_(ex); }
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
