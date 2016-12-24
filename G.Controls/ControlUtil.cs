using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace G.Controls
{
	public static class ControlUtil
	{
		/// <summary>
		/// Simple non-OLE drag operation.
		/// Returns true if dropped, false if cancelled.
		/// </summary>
		/// <param name="w">Window or control that owns the drag operation.</param>
		/// <param name="mouseButton">Mouse button that is used for the drag operation: Left, Right.</param>
		/// <param name="onMouseKeyMessage">Optional callback function, called on each received mouse/key message. Optional.</param>
		public static bool SimpleDragDrop(Wnd w, MouseButtons mouseButton= MouseButtons.Left, Action<SimpleDragDropCallbackArgs> onMouseKeyMessage=null)
		{
			_Api.SetCapture(w);

			bool R = false;
			var x = new SimpleDragDropCallbackArgs();
			for(;;) {
				if(_Api.GetCapture() != w) return false;
				if(Api.GetMessage(out x.Msg, Wnd0, 0, 0) <= 0) {
					if(x.Msg.message == Api.WM_QUIT) Api.PostQuitMessage(x.Msg.wParam);
					break;
				}

				bool call = false;
				uint m =x.Msg.message;
				if(m >= Api.WM_MOUSEFIRST && m <= Api.WM_MOUSELAST) {
					if(m == Api.WM_LBUTTONUP) {
						if(R = (mouseButton & MouseButtons.Left) != 0) break;
					} else if(m == Api.WM_RBUTTONUP) {
						if(R = (mouseButton & MouseButtons.Right) != 0) break;
					}
					call= true;
				} else if(m == Api.WM_KEYDOWN || m == Api.WM_KEYUP || m == Api.WM_SYSKEYDOWN || m == Api.WM_SYSKEYUP) {
					//on key down/up caller may want to update cursor when eg Ctrl pressed/released
					if(x.Msg.wParam == Api.VK_ESCAPE) break;
					call= true;
				}

				if(call && onMouseKeyMessage!=null) {
					onMouseKeyMessage(x);
					if(x._stopped) break;
					if(x.Cursor != null) {
						_Api.SetCursor(x.Cursor.Handle);
						x.Cursor = null;
					}
				}

				Api.DispatchMessage(ref x.Msg);
			}

			_Api.ReleaseCapture();
			return R;
		}

		/// <summary>
		/// <see cref="SimpleDragDrop"/> callback function arguments.
		/// </summary>
		public class SimpleDragDropCallbackArgs
		{
			/// <summary>
			/// Current message retrieved by Api.GetMessage.
			/// </summary>
			public Api.MSG Msg;

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
