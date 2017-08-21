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

#pragma warning disable 1591 //XML doc. //TODO

namespace Catkeys
{
	public static partial class Input
	{
		/// <summary>
		/// Returns true if Alt key is pressed.
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt).
		/// </summary>
		public static bool IsAlt { get => Api.GetKeyState(Api.VK_MENU) < 0; }

		/// <summary>
		/// Returns true if Ctrl key is pressed.
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt).
		/// </summary>
		public static bool IsCtrl { get => Api.GetKeyState(Api.VK_CONTROL) < 0; }

		/// <summary>
		/// Returns true if Shift key is pressed.
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt).
		/// </summary>
		public static bool IsShift { get => Api.GetKeyState(Api.VK_SHIFT) < 0; }

		/// <summary>
		/// Returns true if Win key is pressed (left or right).
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt).
		/// </summary>
		public static bool IsWin { get => Api.GetKeyState(Api.VK_LWIN) < 0 || Api.GetKeyState(Api.VK_RWIN) < 0; }

		/// <summary>
		/// Returns true if one or more of the specified modifier keys are pressed.
		/// See also: <see cref="Control.ModifierKeys"/> (gets Ctrl, Shift and Alt), <see cref="IsWin"/>.
		/// </summary>
		/// <param name="modifierKeys">Check only these keys. One or more of these flags: Keys.Control, Keys.Shift, Keys.Menu, Keys_.Windows. Default - all.</param>
		/// <exception cref="ArgumentException">modifierKeys contains non-modifier keys.</exception>
		/// <seealso cref="WaitFor.NoModifierKeys"/>
		public static bool IsModified(Keys modifierKeys= Keys.Control | Keys.Shift | Keys.Menu | Keys_.Windows)
		{
			bool badKeys = modifierKeys!=(modifierKeys & (Keys.Control | Keys.Shift | Keys.Menu | Keys_.Windows));
			Debug.Assert(!badKeys); if(badKeys) throw new ArgumentException();
			if(0 != (modifierKeys & Keys.Control) && IsCtrl) return true;
			if(0 != (modifierKeys & Keys.Shift) && IsShift) return true;
			if(0 != (modifierKeys & Keys.Menu) && IsAlt) return true;
			if(0 != (modifierKeys & Keys_.Windows) && IsWin) return true;
			return false;
		}

		//TODO: IsCapsLocked, IsNumLocked, IsScrollLocked

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
			return default(Wnd);

			//note: in Word, after changing caret pos, gets pos 0 0. After 0.5 s gets correct. After typing always correct.
			//tested: accessibleobjectfromwindow(objid_caret) is the same, but much slower.
		}


		public static void Key(params string[] keys)
		{
			//better name would be Keys, but it conflicts with the .NET Forms Keys enum that is used in this class and everywhere.
			//	The WPF Key enum is rarely used.
			//	Anyway, in scripts everybody will use the alias K().

			//idea: params object[] keys. Then the last parameter can be eg ScriptOptions.

			if(keys == null) return;
		}

		public static void Text(params string[] text)
		{
			if(text == null) return;
		}
		//note:
		//	Don't use the hybrid option in Catkeys. In many apps sending keys for text snippets etc is too slow, better to paste always.
		//	Then probably don't need Text(). In that rare cases when need, can use Keys("", "text");

		//public static void Text(ScriptOptions options, params string[] text_keys_text_keys_andSoOn)
		//{
		//	var keys = text_keys_text_keys_andSoOn;
		//	if(keys == null) return;
		//}


		//public class KeysToSend
		//{
		//	List<int> _a;

		//	public int Length { get => _a.Count; }

		//	public KeysToSend Tab { get { _a.Add(9); return this; } }
		//	public KeysToSend Enter { get { _a.Add(13); return this; } }
		//	public KeysToSend Ctrl { get { _a.Add(1); return this; } }
		//	public KeysToSend A { get { _a.Add('A'); return this; } }
		//	//public KeysToSend Tab(int nTimes) { return 0; } //error
		//	public KeysToSend Execute(int nTimes) { _a.Add(-nTimes); return this; }

		//}

		//public static KeysToSend K { get => new KeysToSend(); }

		public static void Paste(string text)
		{
			if(text == null) return;
		}


		static void Test()
		{
			//Key("text", K.Ctrl.A.Tab.Execute(9).Enter);
		}
	}
}
