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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// This class contains functions to get and set clipboard text using non-OLE API.
	/// </summary>
	/// <remarks>
	/// Unlike the .NET <see cref="Clipboard"/> class, works in any thread (don't need STA). Also, don't need to process Windows messages while waiting.
	/// </remarks>
	public static unsafe class Clipb
	{
		/// <summary>
		/// Gets text from the clipboard.
		/// Returns null if there is no text in the clipboard.
		/// </summary>
		/// <exception cref="AuException">Failed to open clipboard. Throws after 10 s of wait/retry.</exception>
		public static string GetText()
		{
			using(new _Clipb(false)) {
				var h = Api.GetClipboardData(13); //CF_UNICODETEXT
				if(h == default) return null;
				var v = (char*)Api.GlobalLock(h);
				if(v == null) return null;
				try {
					int len = Api.GlobalSize(h);
					if(len < 2 || (len & 1) != 0) return null;
					len /= 2; if(v[len - 1] == '\0') len--;
					return new string(v, 0, len);
				}
				finally { Api.GlobalUnlock(h); }
			}
		}

		/// <summary>
		/// Puts text in the clipboard.
		/// </summary>
		/// <param name="text">Text. If null, clears the clipboard.</param>
		/// <exception cref="AuException">
		/// Failed to open clipboard. Throws after 10 s of wait/retry.
		/// Or failed to set clipboard data. Unlikely.
		/// </exception>
		/// <exception cref="OutOfMemoryException">Failed to allocate memory.</exception>
		public static void SetText(string text)
		{
			using(new _Clipb(true)) {
				if(text == null) return;
				var h =_StringToClipData(text);
				if(default == Api.SetClipboardData(13, h)) {
					int ec = Native.GetError();
					Api.GlobalFree(h);
					throw new AuException(ec, "set clipboard data");
				}
			}
		}

		/// <summary>
		/// Clears the clipboard.
		/// </summary>
		/// <exception cref="AuException">Failed to open clipboard. Throws after 10 s of wait/retry.</exception>
		public static void Clear() => SetText(null);

		/// <summary>
		/// Opens and closes clipboard using API OpenClipboard and CloseClipboard.
		/// Constructor tries to open for 10 s, then throws AuException.
		/// If the 'empty' parameter is true, creates temporary hidden clipboard owner window. Also then calls EmptyClipboard.
		/// Dispose() closes clipboard and destroys the owner window.
		/// </summary>
		struct _Clipb :IDisposable
		{
			bool _isOpen;
			Wnd _w;

			public _Clipb(bool empty)
			{
				_isOpen = false;
				_w = default;
				if(empty) {
					_w = Wnd.Lib.CreateMessageWindowDefWndProc();
					//MSDN says, SetClipboardData fails if OpenClipboard called with 0 hwnd. It doesn't, but better use hwnd.
					//Creating/destroying window is the slowest part of SetText().
				}
				var to = new WaitFor.LibTimeout(-10);
				while(!Api.OpenClipboard(_w)) {
					int ec = Native.GetError();
					if(!to.Sleep()) {
						Dispose();
						throw new AuException(ec, "*open clipboard");
					}
				}
				_isOpen = true;
				if(empty) Api.EmptyClipboard();
			}

			public void Dispose()
			{
				if(_isOpen) {
					Api.CloseClipboard();
					_isOpen = false;
				}
				if(!_w.Is0) {
					Api.DestroyWindow(_w);
					_w = default;
				}
			}
		}

		/// <summary>
		/// Allocates memory with API GlobalAlloc, copies s to it, and returns the memory handle.
		/// The handle can be stored in clipboard with API SetClipboardData(CF_UNICODETEXT). Else call API GlobalFree.
		/// </summary>
		static IntPtr _StringToClipData(string s)
		{
			Debug.Assert(s != null);
			int memSize = (s.Length + 1) * 2;
			var h = Api.GlobalAlloc(0x2002, memSize); //GMEM_MOVEABLE | GMEM_DDESHARE
			if(h == default) goto ge;
			var v = Api.GlobalLock(h);
			if(v == null) { Api.GlobalFree(h); goto ge; }

			try { fixed (char* p = s) Buffer.MemoryCopy(p, v, memSize, memSize); }
			finally { Api.GlobalUnlock(h); }

			return h;
			ge: throw new OutOfMemoryException();
		}
	}

	//TODO: move to Util or somewhere
	//internal struct LibTempNativeWindow :IDisposable
	//{
	//	Wnd _w;

	//	public LibTempNativeWindow(Wnd w)
	//	{
	//		_w = w;
	//	}

	//	public static implicit operator Wnd(LibTempNativeWindow w) => w._w;

	//	public void Dispose()
	//	{
	//		if(!_w.Is0) {
	//			Api.DestroyWindow(_w);
	//			_w = default;
	//		}
	//	}
	//}
}
