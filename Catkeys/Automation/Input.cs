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
		/// </summary>
		public static bool IsAlt { get => Api.GetKeyState(Api.VK_MENU) < 0; }

		/// <summary>
		/// Returns true if Ctrl key is pressed.
		/// </summary>
		public static bool IsCtrl { get => Api.GetKeyState(Api.VK_CONTROL) < 0; }

		/// <summary>
		/// Returns true if Shift key is pressed.
		/// </summary>
		public static bool IsShift { get => Api.GetKeyState(Api.VK_SHIFT) < 0; }

		/// <summary>
		/// Returns true if the Windows logo key is pressed (left or right).
		/// </summary>
		public static bool IsWin { get => Api.GetKeyState(Api.VK_LWIN) < 0 || Api.GetKeyState(Api.VK_RWIN) < 0; }

		//EditorBrowsableAttribute does not work for Equals and ReferenceEquals in intellisense lists, although correct Options are used.
		//[EditorBrowsable(EditorBrowsableState.Never)]
		////[EditorBrowsable(EditorBrowsableState.Advanced)]
		//static new bool Equals(object a, object b) { return false; }

		//[EditorBrowsable(EditorBrowsableState.Never)]
		////[EditorBrowsable(EditorBrowsableState.Advanced)]
		//static new bool ReferenceEquals(object a, object b) { return false; }

		////[EditorBrowsable(EditorBrowsableState.Never)]
		//////[EditorBrowsable(EditorBrowsableState.Advanced)]
		////public override bool Equals(object a) { return false; }

		/// <summary>
		/// Gets current text cursor (caret) rectangle in screen coordinates.
		/// Returns the control that contains it.
		/// If there is no text cursor or cannot get it (eg it is not a standard text cursor), gets mouse pointer coodinates and returns Wnd0.
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
			r = new RECT(p.x, p.y, 0, 16, true);
			return Wnd0;

			//note: in Word, after changing caret pos, gets pos 0 0. After 0.5 s gets correct. After typing always correct.
			//tested: accessibleobjectfromwindow(objid_caret) is the same, but much slower.
		}


		public static void Key(params string[] keys_text_keys_text_andSoOn)
		{
			var keys = keys_text_keys_text_andSoOn;
			if(keys == null) return;
		}

		//note:
		//	Don't use the hybrid option in Catkeys. In many apps sending keys for text snippets etc is too slow, better to paste always.
		//	Then probably don't need Text(). In that rare cases when need, can use Keys("", "text");
		//public static void Text(params string[] text_keys_text_keys_andSoOn)
		//{
		//	var keys = text_keys_text_keys_andSoOn;
		//	if(keys == null) return;
		//}

		//public static void Text(bool hybrid, params string[] text_keys_text_keys_andSoOn)
		//{
		//	var keys = text_keys_text_keys_andSoOn;
		//	if(keys == null) return;
		//}

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


		static void Test()
		{
			//Key("text", K.Ctrl.A.Tab.Execute(9).Enter);
		}
	}
}
