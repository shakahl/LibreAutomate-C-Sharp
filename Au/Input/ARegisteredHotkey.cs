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
//using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au
{
	/// <summary>
	/// Registers a hotkey using API <msdn>RegisterHotKey</msdn>. Unregisters when disposing.
	/// </summary>
	/// <remarks>
	/// Can be used as a lightweight alternative to hotkey triggers.
	/// 
	/// The variable must be disposed, either explicitly (call <b>Dispose</b> or <b>Unregister</b>) or with the 'using' pattern.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// static void TestRegisterHotkey()
	/// {
	/// 	var f = new FormRegisterHotkey();
	/// 	f.ShowDialog();
	/// }
	/// 
	/// class FormRegisterHotkey :Form
	/// {
	/// 	ARegisteredHotkey _hk1, _hk2;
	/// 
	/// 	protected override void WndProc(ref Message m)
	/// 	{
	/// 		switch(m.Msg) {
	/// 		case WM_CREATE: //0x1
	/// 			bool r1 = _hk1.Register(1, "Ctrl+Alt+F10", this);
	/// 			bool r2 = _hk2.Register(2, (KMod.Ctrl | KMod.Shift, KKey.D), this); //Ctrl+Shift+D
	/// 			Print(r1, r2);
	/// 			break;
	/// 		case WM_DESTROY: //0x2
	/// 			_hk1.Unregister();
	/// 			_hk2.Unregister();
	/// 			break;
	/// 		case ARegisteredHotkey.WM_HOTKEY:
	/// 			Print(m.WParam);
	/// 			break;
	/// 		}
	/// 		base.WndProc(ref m);
	/// 	}
	/// }
	/// ]]></code>
	/// </example>
	public struct ARegisteredHotkey : IDisposable
	{
		AWnd _w;
		int _id;

		///// <summary>The hotkey.</summary>
		//public KHotkey Hotkey { get; private set; }

		/// <summary>
		/// Registers a hotkey using API <msdn>RegisterHotKey</msdn>.
		/// Returns false if fails. Supports <see cref="ALastError"/>.
		/// </summary>
		/// <param name="id">Hotkey id. Must be 0 to 0xBFFF. It will be <i>wParam</i> of the <msdn>WM_HOTKEY</msdn> message.</param>
		/// <param name="hotkey">Hotkey. Can be: string like "Ctrl+Shift+Alt+Win+K", tuple (KMod, KKey), enum KKey, enum Keys, struct KHotkey.</param>
		/// <param name="window">Window/form that will receive the <msdn>WM_HOTKEY</msdn> message. Must be of this thread. If default, the message must be retrieved in the message loop of this thread.</param>
		/// <exception cref="ArgumentException">Error in hotkey string.</exception>
		///	<exception cref="InvalidOperationException">This variable already registered a hotkey.</exception>
		/// <remarks>
		/// Fails if the hotkey is currently registered by this or another application or used by Windows. Also if F12.
		/// A single variable cannot register multiple hotkeys simultaneously. Use multiple variables, for example array.
		/// </remarks>
		/// <seealso cref="AKeyboard.WaitForHotkey"/>
		/// <example>See <see cref="ARegisteredHotkey"/>.</example>
		public bool Register(int id, KHotkey hotkey, AnyWnd window = default)
		{
			var (mod, key) = hotkey;
			if(_id != 0) throw new InvalidOperationException("This variable already registered a hotkey. Use multiple variables or call Unregister.");
			var m = mod & ~(KMod.Alt | KMod.Shift);
			if(mod.Has(KMod.Alt)) m |= KMod.Shift;
			if(mod.Has(KMod.Shift)) m |= KMod.Alt;
			var w = window.Wnd;
			if(!Api.RegisterHotKey(w, id, (uint)m, key)) return false;
			_w = w; _id = id;
			//Hotkey = hotkey;
			return true;
		}

		/// <summary>
		/// Unregisters the hotkey.
		/// </summary>
		/// <remarks>
		/// Called implicitly when disposing this variable.
		/// Must be called from the same thread as when registering, and the window must be still alive.
		/// If fails, calls <see cref="PrintWarning"/>.
		/// </remarks>
		public void Unregister()
		{
			if(_id != 0) {
				if(!Api.UnregisterHotKey(_w, _id)) {
					var es = ALastError.Message;
					PrintWarning($"Failed to unregister hotkey, id={_id.ToString()}. {es}");
					return;
				}
				_id = 0; _w = default;
				//Hotkey = default;
			}
		}

		/// <summary>
		/// Calls <see cref="Unregister"/>.
		/// </summary>
		public void Dispose() => Unregister();

		//~ARegisteredHotkey() => Unregister(); //makes no sense. Called from wrong thread and when the window is already destroyed.

		/// <summary>
		/// This message is posted to the window or to the thread's message loop.
		/// More info: <msdn>WM_HOTKEY</msdn>.
		/// </summary>
		public const int WM_HOTKEY = (int)Api.WM_HOTKEY;
	}
}
