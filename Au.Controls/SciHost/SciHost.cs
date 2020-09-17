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

		//public SciHost() {
		//}

		#region HwndHost

		protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
			_w = AWnd.More.CreateWindow("Scintilla", null, WS.CHILD, 0, 0, 0, 100, 100, (AWnd)hwndParent.Handle);
			//note: no WS_VISIBLE. WPF will manage it. It can cause visual artefacts occasionally, eg scrollbar in WPF area. Maybe also should be size = 0, but then eg invisible in aguibuilder.

			_sciPtr = _w.Send(SCI_GETDIRECTPOINTER);
			Call(SCI_SETNOTIFYCALLBACK, 0, Marshal.GetFunctionPointerForDelegate(_notifyCallback = _NotifyCallback));

			if (_isReadOnly) Call(SCI_SETREADONLY, 1);

			var hwndSource = HwndSource.FromHwnd(hwndParent.Handle);
			var wpf = (Window)hwndSource.RootVisual;
			if (this == FocusManager.GetFocusedElement(wpf)) Api.SetFocus(_w);

			return new HandleRef(this, _w.Handle);
		}

		protected override void DestroyWindowCore(HandleRef hwnd) {
			//AOutput.Write("DESTROY");
			AWnd.More.DestroyWindow((AWnd)hwnd.Handle);
			_w = default;
		}

		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			//AWnd.More.PrintMsg((AWnd)hwnd, msg, wParam, lParam);

			switch (msg) {
			case Api.WM_LBUTTONDOWN:
			case Api.WM_RBUTTONDOWN:
			case Api.WM_MBUTTONDOWN:
				this.Focus();
				break;
			case Api.WM_SETFOCUS:
				_OnWmSetFocus();
				break;
			}

			var R = base.WndProc(hwnd, msg, wParam, lParam, ref handled);

			return R;
		}

		#region problems with focus, keyboard, destroying

		//Somehow WPF does not care about native control focus, normal keyboard work, destroying, etc.
		//1. No Tab key navigation. Also does not set focus when parent tab item selected.
		//	Workaround: override TabIntoCore and call API SetFocus.
		//2. Does not set logical focus to HwndHost when its native control is really focused. Then eg does not restore real focus after using menu.
		//	Workaround: set focus on WM_LBUTTONDOWN etc. Also on WM_SETFOCUS, but temporarily set Focusable=false to avoid kill focus.
		//3. Steals arrow keys, Tab and Enter from native control and sets focus to other controls or closes dialog.
		//	Workaround: override TranslateAcceleratorCore, pass the keys to the control and return true.
		//4. When closing parent window, moves the control to a hidden parking window.
		//	If ShowDialog, does not call DestroyWindowCore and never destroys, even on GC.
		//	Workaround: let app dispose the HwndHost in OnClosing. I did not find an automatic reliable workaround. Can't know when can destroy.
		//5. When closing parent window, briefly tries to show native control, and focus if was focused.
		//	Workaround: same as 4.
		//Never mind: after SetFocus, Keyboard.FocusedElement is null.

		void _OnWmSetFocus() {
			//keep logical focus on HwndHost, else will not work eg restoring of real focus when closing menu.
			if (IsVisible && Focusable) { //info: !IsVisible when closing window without disposing this (WPF bug)
				var fs = FocusManager.GetFocusScope(this);
				if (fs != null && FocusManager.GetFocusedElement(fs) != this) { //focused not by WPF
					this.Focusable = false; //prevent kill focus because SetFocusedElement sets real focus = parent window
					FocusManager.SetFocusedElement(fs, this); //in some cases would work better than this.Focus()
					this.Focusable = true;
				}
			}
		}

		//Makes _w focused when called this.Focus() or Keyboard.Focus(this).
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			Api.SetFocus(_w);
			base.OnGotKeyboardFocus(e);
		}

		//Sets focus when tabbed to this or when clicked the parent tab item. Like eg WPF TextBox.
		protected override bool TabIntoCore(TraversalRequest request) {
			if (!base.Focusable) return false;
			Focus();
			return true;
			//base.TabIntoCore(request); //empty func, returns false
		}

		protected override bool TranslateAcceleratorCore(ref MSG msg, ModifierKeys modifiers) {
			var m = msg.message;
			var k = (KKey)msg.wParam;
			//if (m == Api.WM_KEYDOWN) AOutput.Write(m, k);
			if (m == Api.WM_KEYDOWN || m == Api.WM_KEYUP /*|| m == Api.WM_SYSKEYDOWN || m == Api.WM_SYSKEYUP*/)
				if (!modifiers.HasAny(ModifierKeys.Alt | ModifierKeys.Windows))
					if (k == KKey.Left || k == KKey.Right || k == KKey.Up || k == KKey.Down
						|| (!IsReadOnly && ((k == KKey.Enter && modifiers == 0) || (k == KKey.Tab && !modifiers.Has(ModifierKeys.Control))))) {
						Call(msg.message, msg.wParam, msg.lParam); //not DispatchMessage or Send
						return true;
					}

			return base.TranslateAcceleratorCore(ref msg, modifiers);
		}

		#endregion

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

		public bool IsReadOnly {
			get => _isReadOnly;
			set {
				if (value != _isReadOnly) {
					_isReadOnly = value;
					if (!_w.Is0) Call(SCI_SETREADONLY, _isReadOnly);
				}
			}
		}
		bool _isReadOnly;
	}
}
