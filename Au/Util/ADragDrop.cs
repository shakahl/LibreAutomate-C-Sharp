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
using System.Windows.Forms;
//using System.Linq;

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// Drag-drop functions.
	/// </summary>
	public static class ADragDrop
	{
		/// <summary>
		/// Simple non-OLE drag operation.
		/// Returns true if dropped, false if cancelled.
		/// </summary>
		/// <param name="window">Window or control that owns the drag operation.</param>
		/// <param name="mouseButton">Mouse button that is used for the drag operation: Left, Right, Middle.</param>
		/// <param name="onMouseKeyMessage">Callback function, called on each received mouse/key message. Optional.</param>
		public static bool SimpleDragDrop(AnyWnd window, MButtons mouseButton = MButtons.Left, Action<MsgArgs> onMouseKeyMessage = null)
		{
			AWnd w = window.Wnd;
			Api.SetCapture(w);

			bool R = false;
			var x = new MsgArgs();
			for(; ; ) {
				if(Api.GetCapture() != w) return false;
				if(Api.GetMessage(out x.Msg) <= 0) {
					if(x.Msg.message == Api.WM_QUIT) Api.PostQuitMessage((int)x.Msg.wParam);
					break;
				}

				bool call = false;
				int m = x.Msg.message;
				if(m >= Api.WM_MOUSEFIRST && m <= Api.WM_MOUSELAST) {
					if(m == Api.WM_LBUTTONUP) {
						if(R = mouseButton.Has(MButtons.Left)) break;
					} else if(m == Api.WM_RBUTTONUP) {
						if(R = mouseButton.Has(MButtons.Right)) break;
					} else if(m == Api.WM_MBUTTONUP) {
						if(R = mouseButton.Has(MButtons.Middle)) break;
					}
					call = true;
				} else if(m == Api.WM_KEYDOWN || m == Api.WM_KEYUP || m == Api.WM_SYSKEYDOWN || m == Api.WM_SYSKEYUP) {
					//on key down/up caller may want to update cursor when eg Ctrl pressed/released
					if(x.Msg.wParam == (byte)KKey.Escape) break;
					call = true;
				}

				if(call && onMouseKeyMessage != null) {
					onMouseKeyMessage(x);
					if(x._stopped) break;
					if(x.Cursor != null) {
						Api.SetCursor(x.Cursor.Handle);
						x.Cursor = null;
					}
				}

				Api.DispatchMessage(x.Msg);
			}

			Api.ReleaseCapture();
			return R;
		}

		/// <summary>
		/// <see cref="SimpleDragDrop(AnyWnd, MButtons, Action{MsgArgs})"/> callback function arguments.
		/// </summary>
		public class MsgArgs
		{
			/// <summary>
			/// Current message retrieved by API <msdn>GetMessage</msdn>.
			/// API <msdn>MSG</msdn>.
			/// </summary>
			public Native.MSG Msg;

			/// <summary>
			/// The callback function can set this to temporarily set cursor.
			/// </summary>
			public Cursor Cursor;

			/// <summary>
			/// The callback function can call this to end the operation.
			/// </summary>
			public void Stop() { _stopped = true; }
			internal bool _stopped;
		}
	}
}
