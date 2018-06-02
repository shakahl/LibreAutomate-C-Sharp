using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Util;

namespace Au.Util
{
	/// <summary>
	/// Helps with accessible object event hooks. See <msdn>SetWinEventHook</msdn>.
	/// </summary>
	/// <remarks>
	/// The thread that uses hooks must process Windows messages. For example have a window/dialog/messagebox, or use a 'wait-for' function that dispatches messages or has such option (see <see cref="Opt.WaitFor"/>).
	/// The variable must be disposed, either explicitly (call <b>Dispose</b> or <b>Uninstall</b>) or with the 'using' pattern.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// //using Au.Util;
	/// bool stop = false;
	/// using(new AccHook(AccEVENT.SYSTEM_FOREGROUND, 0, x =>
	/// {
	/// 	Print(x.wnd);
	/// 	var a = x.GetAcc();
	/// 	Print(a);
	/// 	if(x.wnd.ClassNameIs("Shell_TrayWnd")) stop = true;
	/// })) {
	/// 	MessageBox.Show("hook");
	/// 	//or
	/// 	//WaitFor.Message(-10, () => stop); //wait max 10 s for activated taskbar
	/// 	//Print("the end");
	/// }
	/// ]]></code>
	/// </example>
	public class AccHook :IDisposable
	{
		IntPtr _hh; //HHOOK
		Api.WINEVENTPROC _proc1; //our intermediate hook proc that calls _proc2
		Action<HookData.AccHookData> _proc2; //caller's hook proc

		/// <summary>
		/// Installs hook.
		/// Calls API <msdn>SetWinEventHook</msdn>.
		/// </summary>
		/// <param name="eventMin">Specifies the event constant for the lowest event value in the range of events that are handled by the hook function. This parameter can be set to AccEVENT.MIN to indicate the lowest possible event value. Events reference: <msdn>SetWinEventHook</msdn>.</param>
		/// <param name="eventMax">Specifies the event constant for the highest event value in the range of events that are handled by the hook function. This parameter can be set to AccEVENT.MAX to indicate the highest possible event value. If 0, uses <paramref name="eventMin"/>.</param>
		/// <param name="hookProc">Delegate of the hook procedure.</param>
		/// <param name="idProcess">Specifies the id of the process from which the hook function receives events. If 0 - all processes on the current desktop.</param>
		/// <param name="idThread">Specifies the native id of the thread from which the hook function receives events. If 0 - all threads.</param>
		/// <param name="flags"></param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example><inheritdoc cref="AccHook"/></example>
		public AccHook(AccEVENT eventMin, AccEVENT eventMax, Action<HookData.AccHookData> hookProc, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			if(eventMax == 0) eventMax = eventMin;
			_hh = Api.SetWinEventHook(eventMin, eventMax, default, _proc1 = _HookProc, idProcess, idThread, flags);
			if(_hh == default) throw new AuException("*set hook");
			_proc2 = hookProc;
		}

		/// <summary>
		/// Uninstalls the hook if installed.
		/// </summary>
		/// <remarks>
		/// Must be called from the same thread that installed the hook.
		/// </remarks>
		public void Unhook()
		{
			if(_hh != default) {
				bool ok = Api.UnhookWinEvent(_hh);
				if(!ok) PrintWarning("Failed to unhook AccHook.");
				_hh = default;
			}
		}

		/// <summary>
		/// Calls <see cref="Unhook"/>.
		/// </summary>
		public void Dispose() { Unhook(); GC.SuppressFinalize(this); }

		//MSDN: UnhookWinEvent fails if called from a thread different from the call that corresponds to SetWinEventHook.
		///
		~AccHook() { PrintWarning("Non-disposed AccHook variable."); } //unhooking makes no sense

		void _HookProc(IntPtr hHook, AccEVENT aEvent, Wnd wnd, int idObject, int idChild, int idThread, int eventTime)
		{
			_proc2(new HookData.AccHookData(this, aEvent, wnd, idObject, idChild, idThread, eventTime));
		}
	}
}

namespace Au.Types
{
	public static partial class HookData
	{
		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="AccHook"/>.
		/// More info: <msdn>WinEventProc</msdn>.
		/// </summary>
		public unsafe struct AccHookData
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AccHook hook;

			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly AccEVENT aEvent;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly Wnd wnd;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly int idObject;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly int idChild;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly int idThread;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly int eventTime;

			internal AccHookData(AccHook hook, AccEVENT aEvent, Wnd wnd, int idObject, int idChild, int idThread, int eventTime)
			{
				this.hook = hook;
				this.aEvent = aEvent;
				this.wnd = wnd;
				this.idObject = idObject;
				this.idChild = idChild;
				this.idThread = idThread;
				this.eventTime = eventTime;
			}

			///<inheritdoc cref="Acc.FromEvent"/>
			public Acc GetAcc()
			{
				return Acc.FromEvent(wnd, idObject, idChild);
			}
		}
	}
}
