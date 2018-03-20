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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;
using static Au.NoClass;

#pragma warning disable 1591 //XML doc. //TODO

namespace Au
{
	public partial class Input
	{
		/// <summary>
		/// Returns true if Alt key is pressed.
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt).
		/// </summary>
		public static bool IsAlt => Api.GetKeyState(Api.VK_MENU) < 0;

		/// <summary>
		/// Returns true if Ctrl key is pressed.
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt).
		/// </summary>
		public static bool IsCtrl => Api.GetKeyState(Api.VK_CONTROL) < 0;

		/// <summary>
		/// Returns true if Shift key is pressed.
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt).
		/// </summary>
		public static bool IsShift => Api.GetKeyState(Api.VK_SHIFT) < 0;

		/// <summary>
		/// Returns true if Win key is pressed (left or right).
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt).
		/// </summary>
		public static bool IsWin => Api.GetKeyState(Api.VK_LWIN) < 0 || Api.GetKeyState(Api.VK_RWIN) < 0;

		/// <summary>
		/// Returns true if one or more of the specified modifier keys are pressed.
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt), <see cref="IsWin"/>.
		/// </summary>
		/// <param name="modifierKeys">Check only these keys. One or more of these flags: Keys.Control, Keys.Shift, Keys.Menu, Keys_.Windows. Default - all.</param>
		/// <exception cref="ArgumentException">modifierKeys contains non-modifier keys.</exception>
		/// <seealso cref="WaitFor.NoModifierKeys"/>
		public static bool IsModified(Keys modifierKeys = Keys.Control | Keys.Shift | Keys.Menu | Keys_.Windows)
		{
			bool badKeys = modifierKeys != (modifierKeys & (Keys.Control | Keys.Shift | Keys.Menu | Keys_.Windows));
			Debug.Assert(!badKeys); if(badKeys) throw new ArgumentException();
			if(0 != (modifierKeys & Keys.Control) && IsCtrl) return true;
			if(0 != (modifierKeys & Keys.Shift) && IsShift) return true;
			if(0 != (modifierKeys & Keys.Menu) && IsAlt) return true;
			if(0 != (modifierKeys & Keys_.Windows) && IsWin) return true;
			return false;
		}

		/// <summary>
		/// Returns true if the Caps Lock key is in the locked statue.
		/// </summary>
		public static bool IsCapsLock => (Api.GetKeyState(Api.VK_CAPITAL) & 1) != 0;

		/// <summary>
		/// Returns true if the Num Lock key is in the locked statue.
		/// </summary>
		public static bool IsNumLock => (Api.GetKeyState(Api.VK_NUMLOCK) & 1) != 0;

		/// <summary>
		/// Returns true if the Scroll Lock key is in the locked statue.
		/// </summary>
		public static bool IsScrollLock => (Api.GetKeyState(Api.VK_SCROLL) & 1) != 0;

		/// <summary>
		/// Gets current text cursor (caret) rectangle in screen coordinates.
		/// Returns the control that contains it.
		/// If there is no text cursor or cannot get it (eg it is not a standard text cursor), gets mouse pointer coodinates and returns default(Wnd).
		/// </summary>
		public static Wnd GetTextCursorRect(out RECT r)
		{
			if(Wnd.Misc.GetGUIThreadInfo(out var g) && !g.hwndCaret.Is0) {
				if(g.rcCaret.bottom <= g.rcCaret.top) g.rcCaret.bottom = g.rcCaret.top + 16;
				r = g.rcCaret;
				g.hwndCaret.MapClientToScreen(ref r);
				return g.hwndCaret;
			}

			Api.GetCursorPos(out var p);
			r = new RECT(p.X, p.Y, 0, 16, true);
			return default;

			//note: in Word, after changing caret pos, gets pos 0 0. After 0.5 s gets correct. After typing always correct.
			//tested: accessibleobjectfromwindow(objid_caret) is the same, but much slower.
		}

		/// <summary>
		/// This static <see cref="Input"/> instance is used by the static 'send keys' functions - <see cref="NoClass.Key"/>, <see cref="NoClass.Text"/>, <see cref="NoClass.Paste"/> and similar.
		/// Use it to set options for these functions, for example at the start of your script or in the static constructor of script's class. See the first example.
		/// Not used by other Input instances. See the second example.
		/// </summary>
		/// <example>
		/// Use static functions.
		/// <code><![CDATA[
		/// Input.Common.SleepAfter = 100;
		/// ...
		/// Key("Tab Ctrl+V");
		/// ]]></code>
		/// Use an Input instance.
		/// <code><![CDATA[
		/// var k = new Input();
		/// k.SleepAfter = 200;
		/// k.Key("Tab Ctrl+V");
		/// ]]></code>
		/// </example>
		public static Input Common => s_common;
		static Input s_common = new Input();

		/// <summary>
		/// Wait milliseconds after sending each key down and up event.
		/// Default: 0. Valid values: 0 - 1000 (1 second).
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// If 0 (default), just sleeps briefly after some keys.
		/// </remarks>
		public int SleepIn
		{
			get => _sleepIn;
			set => _sleepIn = ((uint)value <= 1000) ? value : throw new ArgumentOutOfRangeException(null, "0-1000");
		}
		int _sleepIn;

		/// <summary>
		/// Wait milliseconds when all keys have been sent (before the function returns).
		/// Default: 50. Valid values: 0 - 5000 (5 seconds).
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int SleepAfter
		{
			get => _sleepAfter;
			set => _sleepAfter = ((uint)value <= 5000) ? value : throw new ArgumentOutOfRangeException(null, "0-5000");
		}
		int _sleepAfter = 50;

		void _KeyText(bool isText, params object[] p)
		{

		}

		public void Key(params object[] keys)
		{
			_KeyText(false, keys);
		}
		//Example:
		//Key("keys", "text", "keys", "text", 500, "keys", "text", Keys.Back)
		//Text sending method: in 'text' parts use 'VK_PACKET; in 'keys' parts use key codes and explicit Shift.
		//rejected, because rarely used, and we have Text/Paste:
		//	Key("keys Text", "text using keys and Shift", "keys Paste", "text using paste").
		//	Key(new KeyOptions(usePaste: true), "keys", "text").
		//	Key(KFlags.Paste, "keys", "text").

		public void Text(string text, string keys = null)
		{
			_KeyText(true, null, text, keys);
		}
		//Text sending method: 'VK_PACKET; other overload allows to specify any method.

		//public void Text(KeyOptions options, string text, string keys = null)
		//{
		//
		//}

		//public class KeyOptions:
		//Allows to specify how to send text: VK_PACKET, Shift+keys, paste, WM_CHAR/WM_UNICHAR.
		//	Can be different for different windows; can use callback for it.

		//CONSIDER:
		//Let Text() in 'paste' mode use Enter for the last newline if text ends with newline.
		//Eg these apps remove the ending newline when pasting: Word, Wordpad, OpenOffice, LibreOffice.
		//Don't use this for Paste().
		//Try to use VK_PACKET, because Enter may close dialog.

		//public class KeysToSend
		//{
		//	List<int> _a;

		//	public int Length => _a.Count;

		//	public KeysToSend Tab { get { _a.Add(9); return this; } }
		//	public KeysToSend Enter { get { _a.Add(13); return this; } }
		//	public KeysToSend Ctrl { get { _a.Add(1); return this; } }
		//	public KeysToSend A { get { _a.Add('A'); return this; } }
		//	//public KeysToSend Tab(int nTimes) { return 0; } //error
		//	public KeysToSend Execute(int nTimes) { _a.Add(-nTimes); return this; }

		//}

		public void Paste(string text, string keys = null)
		{

		}

		public void Paste(Wnd w, string text, string keys = null)
		{

		}

		public void PasteFormat(string text, string format, string keys = null)
		{

		}

		public void PasteFormat(Wnd w, string text, string format = null, string keys = null)
		{

		}

		public string Copy(bool cut = false, string format = null)
		{
			return null;
		}

		public string Copy(Wnd w, bool cut = false, string format = null)
		{
			return null;
		}

		//public static void FocusPasswordField() //use instead of AutoPassword: FocusPasswordField(); Text(password, "Tab"); Text(user, "Enter");

		void Test()
		{
			//Key("text", K.Ctrl.A.Tab.Execute(9).Enter);
		}
	}
}
