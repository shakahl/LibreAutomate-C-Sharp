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
using System.Windows.Input;
//using System.Linq;

namespace Au.Controls
{
	using static Sci;

	public unsafe class SciHost : HwndHost
	{
		AWnd _w;
		LPARAM _sciPtr;
		Sci_NotifyCallback _notifyCallback;

		public LPARAM SciPtr => _sciPtr;

		static SciHost() {
			AFile.More.LoadDll64or32Bit("SciLexer.dll");
		}

		public SciHost() {
			//AOutput.Write(base.Focusable);
			//base.Ta
		}

		#region HwndHost

		protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
			_w = AWnd.More.CreateWindow("Scintilla", null, WS.CHILD | WS.VISIBLE, 0, 0, 0, 200, 100, (AWnd)hwndParent.Handle);

			_sciPtr = _w.Send(SCI_GETDIRECTPOINTER);
			Call(SCI_SETNOTIFYCALLBACK, 0, Marshal.GetFunctionPointerForDelegate(_notifyCallback = _NotifyCallback));

			return new HandleRef(this, _w.Handle);
		}

		protected override void DestroyWindowCore(HandleRef hwnd) {
			AWnd.More.DestroyWindow((AWnd)hwnd.Handle);
			_w = default;
		}

		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			//AWnd.More.PrintMsg((AWnd)hwnd, msg, wParam, lParam);

			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}

		protected override bool TranslateAcceleratorCore(ref MSG msg, ModifierKeys modifiers) {
			//AOutput.Write(msg.message);
			var m = msg.message;
			if (m == Api.WM_KEYDOWN || m == Api.WM_KEYUP /*|| m == Api.WM_SYSKEYDOWN || m == Api.WM_SYSKEYUP*/) {
				//tested: Scintilla has several Alt+ hotkeys. We don't handle WM_SYSKEYDOWN, but eg Scintilla's Alt+Backspace (Undo) works.
				//AOutput.Write((KKey)msg.wParam);
				if (m == Api.WM_KEYDOWN) Api.TranslateMessage(msg); //post WM_CHAR
				Call(msg.message, msg.wParam, msg.lParam); //not DispatchMessage or Send
				return true;
			}
			return base.TranslateAcceleratorCore(ref msg, modifiers);
		}

		#endregion

		void _NotifyCallback(void* cbParam, ref SCNotification n) {
			//var code = n.nmhdr.code;
			////if(code != NOTIF.SCN_PAINTED) AOutput.QM2.Write(code.ToString());
			//switch (code) {
			//case NOTIF.SCN_MODIFIED:
			//	if ((n.modificationType & (MOD.SC_MULTISTEPUNDOREDO | MOD.SC_LASTSTEPINUNDOREDO)) == MOD.SC_MULTISTEPUNDOREDO) return;
			//	_NotifyModified(n);
			//	if (ZDisableModifiedNotifications) return;
			//	break;
			//case NOTIF.SCN_HOTSPOTRELEASECLICK:
			//	ZTags?.OnLinkClick_(n.position, 0 != (n.modifiers & SCMOD_CTRL));
			//	break;
			//}
			//ZOnSciNotify(ref n);
		}

		/// <summary>
		/// Sends a Scintilla message to the control and returns int.
		/// Don't call this function from another thread.
		/// </summary>
		[DebuggerStepThrough]
		public int Call(int sciMessage, LPARAM wParam = default, LPARAM lParam = default) => (int)CallRetPtr(sciMessage, wParam, lParam);

		/// <summary>
		/// Sends a Scintilla message to the control and returns LPARAM.
		/// Don't call this function from another thread.
		/// </summary>
		[DebuggerStepThrough]
		public LPARAM CallRetPtr(int sciMessage, LPARAM wParam = default, LPARAM lParam = default) {
			//#if DEBUG
			//			if (ZDebugPrintMessages) _DebugPrintMessage(sciMessage);
			//#endif

			Debug.Assert(!_w.Is0);
			//if (_w.Is0) {
			//	Debug.Assert(!IsVisible);
			//	CreateHandle(); //because did not create handle if initially Visible is false
			//}
			//Debug.Assert(IsHandleCreated || this.DesignMode);
			//if(!IsHandleCreated) CreateHandle();
			//note: auto-creating handle is not good:
			//	1. May create parked control. Not good for performance.
			//	2. Can be dangerous, eg if passing a reusable buffer that also is used by OnHandleCreated.

			return Sci_Call(_sciPtr, sciMessage, wParam, lParam);
		}
	}
}
