using Au.Types;
using Au.Util;
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
//using System.Linq;

namespace Au
{
	/// <summary>
	/// Provides functions to get miscellaneous system information or current state. Text cursor rectangle, input desktop, etc.
	/// </summary>
	/// <seealso cref="AVersion"/>
	/// <seealso cref="AFolders"/>
	/// <seealso cref="AProcess"/>
	/// <seealso cref="AThread"/>
	/// <seealso cref="AScreen"/>
	/// <seealso cref="ATask"/>
	/// <seealso cref="ATime"/>
	/// <seealso cref="AUac"/>
	/// <seealso cref="ADpi"/>
	/// <seealso cref="Environment"/>
	/// <seealso cref="System.Windows.Forms.SystemInformation"/>
	/// <seealso cref="System.Windows.SystemParameters"/>
	public static class AMiscInfo
	{
		/// <summary>
		/// Calls API <msdn>GetGUIThreadInfo</msdn>. It gets info about mouse capturing, menu mode, move/size mode, focus, caret, etc.
		/// </summary>
		/// <param name="g">API <msdn>GUITHREADINFO</msdn>.</param>
		/// <param name="idThread">Thread id. If 0 - the foreground (active window) thread. See <see cref="AThread.Id"/>, <see cref="AWnd.ThreadId"/>.</param>
		public static bool GetGUIThreadInfo(out Native.GUITHREADINFO g, int idThread = 0) {
			g = new Native.GUITHREADINFO(); g.cbSize = Api.SizeOf(g);
			return Api.GetGUIThreadInfo(idThread, ref g);
		}

		/// <summary>
		/// Gets text cursor (caret) position and size.
		/// Returns false if fails.
		/// </summary>
		/// <param name="r">Receives the rectangle, in screen coordinates.</param>
		/// <param name="w">Receives the control that contains the text cursor.</param>
		/// <param name="orMouse">If fails, get mouse pointer coodinates.</param>
		/// <remarks>
		/// Can get only standard text cursor. Many apps use non-standard cursor; then fails.
		/// Also fails if the text cursor currently is not displayed.
		/// </remarks>
		public static bool GetTextCursorRect(out RECT r, out AWnd w, bool orMouse = false) {
			if (GetGUIThreadInfo(out var g) && !g.hwndCaret.Is0) {
				if (g.rcCaret.bottom <= g.rcCaret.top) g.rcCaret.bottom = g.rcCaret.top + 16;
				r = g.rcCaret;
				g.hwndCaret.MapClientToScreen(ref r);
				w = g.hwndCaret;
				return true;
			}
			if (orMouse) {
				Api.GetCursorPos(out var p);
				r = new RECT(p.x, p.y, 0, 16);
			} else r = default;
			w = default;
			return false;

			//note: in Word, after changing caret pos, gets pos 0 0. After 0.5 s gets correct. After typing always correct.
			//tested: accessibleobjectfromwindow(objid_caret) is the same, but much slower.
		}

		/// <summary>
		/// Returns true if current thread is on the input desktop and therefore can use mouse, keyboard, clipboard and window functions.
		/// </summary>
		/// <param name="detectLocked">Return false if the active window is a full-screen window of LockApp.exe on Windows 10. It is when computer has been locked but still not displaying the password field. Slower.</param>
		/// <remarks>
		/// Usually this app is running on default desktop. Examples of other desktops: the Ctrl+Alt+Delete screen, the PC locked screen, screen saver, UAC consent, custom desktops. If one of these is active, this app cannot use many mouse, keyboard, clipboard and window functions. They either throw exception or do nothing.
		/// </remarks>
		public static unsafe bool IsInputDesktop(bool detectLocked = false) {
			var w = AWnd.Active;
			if (w.Is0) { //tested: last error code 0
				int i = 0;
				if (!Api.GetUserObjectInformation(Api.GetThreadDesktop(AThread.Id), Api.UOI_IO, &i, 4, out _)) return true; //slow
				return i != 0;
				//also tested several default screensavers on Win10 and 7. Goes through this branch. When closed, works like when locked (goes through other branch until next input).
			} else {
				//TODO: in lock screen SHQueryUserNotificationState returns QUNS_NOT_PRESENT.
				//	Also documented but not tested: screen saver.
				//	tested: QUNS_ACCEPTS_NOTIFICATIONS (normal) when Ctrl+Alt+Delete.
				if (detectLocked && AVersion.MinWin10) {
					var rw = w.Rect;
					if (rw.left == 0 && rw.top == 0) {
						var rs = AScreen.Primary.Rect;
						if (rw == rs) {
							var s = w.ProgramName;
							if (s.Eqi("LockApp.exe")) return false;
						}
					}
				}
				return true;
			}
		}

		//public static unsafe string GetInputDesktopName() {
		//	var hd = Api.OpenInputDesktop(0, false, Api.GENERIC_READ); //error "Access is denied" when this process is admin. Need SYSTEM.
		//	//if (hd == default) throw new AuException(0);
		//	if (hd == default) return null;
		//	string s = null;
		//	var p = stackalloc char[300];
		//	if (Api.GetUserObjectInformation(hd, Api.UOI_NAME, p, 600, out int len) && len >= 4) s = new(p, 0, len / 2 - 1);
		//	Api.CloseDesktop(hd);
		//	return s;
		//}
	}
}
