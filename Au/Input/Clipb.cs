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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Clipboard functions: copy, paste, get and set clipboard text and other data.
	/// </summary>
	/// <remarks>
	/// This class is similar to the .NET <see cref="System.Windows.Forms.Clipboard"/> class, which uses OLE API, works only in STA threads and does not work well in automation scripts. This class uses non-OLE API and works well in automation scripts and any threads.
	/// 
	/// To set/get clipboard data of non-text formats, use class <see cref="Data"/>; to paste, use it with <see cref="PasteData"/>; to copy (get from the active app), use it with <see cref="CopyData"/>.
	/// 
	/// Should not be used to copy/paste in windows of own thread. In most cases it works, but strange problems are possible, because it waits until the window processes the copy/paste request, and while waiting it gets/dispatches all messages/events/etc. It's better to call it from another thread. Example in <see cref="Keyb.Key"/>.
	/// </remarks>
	public static partial class Clipb
	{
		/// <summary>
		/// Clears the clipboard.
		/// </summary>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry).</exception>
		public static void Clear()
		{
			using(new _OpenClipboard(false)) _EmptyClipboard();
		}

		static void _EmptyClipboard()
		{
			if(!Api.EmptyClipboard()) Debug.Assert(false);
		}

		/// <summary>
		/// Gets or sets clipboard text.
		/// </summary>
		/// <exception cref="AuException">Failed to open clipboard (after 10 s of wait/retry) or set clipboard data.</exception>
		/// <exception cref="OutOfMemoryException">The 'set' function failed to allocate memory.</exception>
		/// <remarks>
		/// The 'get' function calls <see cref="Data.GetText"/>. Also can get file paths, as multiline text. Returns null if there is no text/files.
		/// </remarks>
		/// <seealso cref="Clipb.Data"/>
		public static string Text
		{
			get => Data.GetText(0);
			set
			{
				using(new _OpenClipboard(true)) {
					_EmptyClipboard();
					if(value != null) Data.LibSetText(value);
				}
			}
		}

		//Sets text (string) or multi-format data (Data). Clipboard must be open.
		static void _SetClipboard(object data, bool renderLater)
		{
			switch(data) {
			case Data d:
				d.SetOpenClipboard(renderLater);
				break;
			case string s:
				if(renderLater) Api.SetClipboardData(Api.CF_UNICODETEXT, default);
				else Data.LibSetText(s);
				break;
			}
		}

		/// <summary>
		/// Calls API SetClipboardData("Clipboard Viewer Ignore"). Clipboard must be open.
		/// Then clipboard manager/viewer/etc programs that are aware of this convention don't try to get our clipboard data while we are pasting.
		/// Tested apps that support it: Ditto, Clipdiary. Other 5 tested apps don't.
		/// </summary>
		static void _SetClipboardData_ClipboardViewerIgnore()
		{
			Api.SetClipboardData(ClipFormats.ClipboardViewerIgnore, Api.GlobalAlloc(Api.GMEM_MOVEABLE | Api.GMEM_ZEROINIT, 1));
			//tested: hMem cannot be default(IntPtr) or 0 bytes.
		}

		/// <summary>
		/// Gets the selected text from the focused app using the clipboard.
		/// </summary>
		/// <param name="cut">Use Ctrl+X.</param>
		/// <param name="options">
		/// Options. If null (default), uses <see cref="Opt.Key"/>.
		/// Uses <see cref="OptKey.RestoreClipboard"/>, <see cref="OptKey.NoBlockInput"/>, <see cref="OptKey.KeySpeedClipboard"/>. Does not use <see cref="OptKey.Hook"/>.
		/// </param>
		/// <exception cref="AuException">Failed.</exception>
		/// <remarks>
		/// Also can get file paths, as multiline text.
		/// Sends keys Ctrl+C, waits until the focused app sets clipboard data, gets it, finally restores clipboard data.
		/// Fails (exception) if the focused app does not set clipboard text or file paths, for example if there is no selected text/files.
		/// Works with console windows too, even if they don't support Ctrl+C.
		/// </remarks>
		public static string CopyText(bool cut = false, OptKey options = null)
		{
			return _Copy(cut, options, null);
			//rejected: 'format' parameter. Not useful.
		}

		/// <summary>
		/// Gets data of any formats from the focused app using the clipboard and a callback function.
		/// </summary>
		/// <param name="callback">Callback function. It can get clipboard data of any formats. It can use any clipboard functions, for example the <see cref="Data"/> class or the .NET <see cref="System.Windows.Forms.Clipboard"/> class. Don't call copy/paste functions.</param>
		/// <param name="cut">Use Ctrl+X.</param>
		/// <param name="options">See <see cref="CopyText"/>.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <exception cref="Exception">Exceptions thrown by the callback function.</exception>
		/// <remarks>
		/// Sends keys Ctrl+C, waits until the focused app sets clipboard data, calls callback function that gets it, finally restores clipboard data.
		/// Fails (exception) if the focused app does not set clipboard data.
		/// Works with console windows too, even if they don't support Ctrl+C.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// string text = null; Bitmap image = null; string[] files = null;
		/// Clipb.CopyData(() => { text = Clipb.Data.GetText(); image = Clipb.Data.GetImage(); files = Clipb.Data.GetFiles(); });
		/// if(text == null) Print("no text in clipboard"); else Print(text);
		/// if(image == null) Print("no image in clipboard"); else Print(image.Size);
		/// if(files == null) Print("no files in clipboard"); else Print(files);
		/// ]]></code>
		/// </example>
		public static void CopyData(Action callback, bool cut = false, OptKey options = null)
		{
			if(callback == null) throw new ArgumentNullException();
			_Copy(cut, options, callback);
		}

		static string _Copy(bool cut, OptKey options, Action callback)
		{
			string R = null;
			var opt = options ?? Opt.Key;
			bool restore = opt.RestoreClipboard;
			_ClipboardListener listener = null;
			var bi = new BlockUserInput() { ResendBlockedKeys = true };
			var oc = new _OpenClipboard(createOwner: true, noOpenNow: !restore);
			try {
				if(!opt.NoBlockInput) bi.Start(BIEvents.Keys);
				Keyb.Lib.ReleaseModAndDisableModMenu();

				var save = new _SaveRestore();
				if(restore) {
					save.Save();
					oc.Close(false); //close clipboard; don't destroy our clipboard owner window
				}

				Wnd wFocus = Keyb.Lib.GetWndFocusedOrActive();
				listener = new _ClipboardListener(false, null, oc.WndClipOwner, wFocus);

				if(!Api.AddClipboardFormatListener(oc.WndClipOwner)) throw new AuException();
				var ctrlC = new Keyb.Lib.SendCopyPaste();
				try {
					if(wFocus.IsConsole) {
						wFocus.Post(Api.WM_SYSCOMMAND, 65520);
						//system menu -> &Edit -> &Copy; tested on all OS; Windows 10 supports Ctrl+C, but it may be disabled.
					} else {
						ctrlC.Press(cut ? KKey.X : KKey.C, opt, wFocus);
					}

					//wait until the app sets clipboard text
					listener.Wait(ref ctrlC);
				}
				finally {
					ctrlC.Release();
					Api.RemoveClipboardFormatListener(oc.WndClipOwner);
				}

				wFocus.SendTimeout(500, 0); //workaround: in SharpDevelop and ILSpy (both WPF), API GetClipboardData takes ~1 s. Need to sleep min 10 ms or send message.

				if(callback != null) {
					callback();
					if(restore) oc.Reopen();
				} else {
					oc.Reopen();
					R = Data.LibGetText(0);
				}

				if(restore) save.Restore();
			}
			finally {
				oc.Dispose();
				bi.Dispose();
			}
			GC.KeepAlive(listener);
			if(R == null && callback == null) throw new AuException("*copy text"); //no text in the clipboard. Probably not a text control; if text control but empty selection, usually throws in Wait, not here, because the target app then does not use the clipboard.
			return R;
		}

		/// <summary>
		/// Pastes text into the focused app using the clipboard.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="options">
		/// Options. If null (default), uses <see cref="Opt.Key"/>.
		/// Uses <see cref="OptKey.RestoreClipboard"/>, <see cref="OptKey.PasteEnter"/>, <see cref="OptKey.NoBlockInput"/>, <see cref="OptKey.SleepFinally"/>, <see cref="OptKey.Hook"/>, <see cref="OptKey.KeySpeedClipboard"/>.
		/// </param>
		/// <exception cref="AuException">Failed.</exception>
		/// <remarks>
		/// Adds to the clipboard, sends keys Ctrl+V, waits until the focused app gets clipboard data, finally restores clipboard data.
		/// Fails (exception) if nothing gets clipboard data in several seconds.
		/// Works with console windows too, even if they don't support Ctrl+V.
		/// A clipboard viewer/manager program can make this function slower and less reliable, unless it supports <see cref="ClipFormats.ClipboardViewerIgnore"/> or gets clipboard data with a delay.
		/// Possible problems with some virtual PC programs. Either pasting does not work in their windows, or they use a hidden clipboard viewer that makes this function slower and less reliable.
		/// </remarks>
		/// <seealso cref="Keyb.Text"/>
		/// <example>
		/// <code><![CDATA[
		/// Clipb.PasteText("Example\r\n");
		/// Paste("Example\r\n"); //the same as above
		/// ]]></code>
		/// </example>
		public static void PasteText(string text, OptKey options = null)
		{
			if(Empty(text)) return;
			_Paste(text, options);
		}
		//TODO: fails to paste in VMware player. QM2 too. Maybe add an option to not sync etc.

		/// <summary>
		/// Pastes data added to a <see cref="Data"/> variable into the focused app using the clipboard.
		/// More info: <see cref="PasteText"/>.
		/// </summary>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// Paste data of two formats: HTML and text.
		/// <code><![CDATA[
		/// Clipb.PasteData(new Clipb.Data().AddHtml("<b>text</b>").AddText("text"));
		/// ]]></code>
		/// </example>
		public static void PasteData(Data data, OptKey options = null)
		{
			if(data == null) throw new ArgumentNullException();
			_Paste(data, options);
		}

		//rejected. Should use some UI-created/saved data containing all three formats.
		//public static void PasteRichText(string text, string rtf, string html = null, OptKey options = null)
		//{
		//	var a = new List<(int, object)>();
		//	if(!Empty(text)) a.Add((0, text));
		//	if(!Empty(rtf)) a.Add((Lib.RtfFormat, rtf));
		//	if(!Empty(html)) a.Add((Lib.HtmlFormat, html));
		//	if(a.Count == 0) return;
		//	_Paste(a, options);
		//}

		static void _Paste(object data, OptKey options = null)
		{
			var wFocus = Keyb.Lib.GetWndFocusedOrActive();
			var opt = options ?? Opt.Key;
			var bi = new BlockUserInput() { ResendBlockedKeys = true };
			try {
				if(!opt.NoBlockInput) bi.Start(BIEvents.Keys);
				Keyb.Lib.ReleaseModAndDisableModMenu();
				opt = opt.LibGetHookOptionsOrThis(wFocus);
				LibPaste(data, opt, wFocus);
			}
			finally {
				bi.Dispose();
			}

			int sleepFinally = opt.SleepFinally;
			if(sleepFinally > 0) Keyb.Lib.Sleep(sleepFinally);
		}

		/// <summary>
		/// Used by Clipb and Keyb.
		/// The caller should block user input (if need), release modifier keys, get opt/wFocus, sleep finally (if need).
		/// </summary>
		/// <param name="data">string or Data.</param>
		/// <param name="opt"></param>
		/// <param name="wFocus"></param>
		internal static void LibPaste(object data, OptKey opt, Wnd wFocus)
		{
			bool isConsole = wFocus.IsConsole, enter = false;

			if(opt.PasteEnter) {
				string s = data as string;
				if(enter = s != null && s.Ends('\n') && !isConsole) {
					s = s.RemoveSuffix(s.Ends("\r\n") ? 2 : 1);
					if(s.Length == 0) {
						Keyb.Lib.SendCopyPaste.Enter(opt);
						return;
					}
					data = s;
					//rejected: alternative workaround - convert to RTF.
					//	It works in Word, WordPad, OO, LO.
					//	But then eg Word for it uses default formatting instead of current formatting.
					//	Also some apps may not fully support our RTF and add different text, eg '?' for non-ASCII chars.
				}
			}

			bool sync = true; //FUTURE: option to turn off, depending on window.
			_ClipboardListener listener = null;
			using(var oc = new _OpenClipboard(true)) {

				bool restore = opt.RestoreClipboard;
				var save = new _SaveRestore();
				if(restore) save.Save();

				_EmptyClipboard();
				_SetClipboardData_ClipboardViewerIgnore();
				_SetClipboard(data, renderLater: sync);
				oc.Close(false); //close clipboard; don't destroy our clipboard owner window
				if(sync) listener = new _ClipboardListener(true, data, oc.WndClipOwner, wFocus);
				//info:
				//	oc ctor creates a temporary message-only clipboard owner window. Its wndproc initially is DefWindowProc.
				//	listener ctor subclasses it. Its wndproc receives WM_RENDERFORMAT which sets clipboard data etc.

				var ctrlV = new Keyb.Lib.SendCopyPaste();
				try {
					if(isConsole) {
						wFocus.Post(Api.WM_SYSCOMMAND, 65521);
						//system menu -> &Edit -> &Paste; tested on all OS; Windows 10 supports Ctrl+V, but it may be disabled.
					} else {
						ctrlV.Press(KKey.V, opt, wFocus, enter);
					}

					//wait until the app gets clipboard text
					if(sync) {
						listener.Wait(ref ctrlV);
						if(listener.FailedToSetData != null) throw new AuException(listener.FailedToSetData.Message);
						if(listener.IsBadWindow) sync = false;
					}
					if(!sync) {
						Keyb.Lib.Sleep(Keyb.Lib.LimitSleepTime(opt.KeySpeedClipboard)); //if too long, may autorepeat, eg BlueStacks after 500 ms
					}
				}
				finally {
					ctrlV.Release();
				}

				if(restore && !save.IsSaved) restore = false;

				//CONSIDER: opt.SleepClipboard. If 0, uses smart sync, else simply sleeps.
				for(int i = 0, n = sync ? 3 : (restore ? 25 : 15); i < n; i++) {
					wFocus.SendTimeout(1000, 0, flags: 0);
					Keyb.Lib.Sleep(i + 3);

					//info: repeats this min 3 times as a workaround for this Dreamweaver problem:
					//	First time after starting DW, if several Paste called in loop, the first pasted text if of the second Paste.
				}

				if(restore && oc.Reopen(true)) save.Restore();
			}
			GC.KeepAlive(listener);
		}

		/// <summary>
		/// Waits until the target app gets (Paste) or sets (Copy) clipboard text.
		/// For it subclasses our clipboard owner window and uses clipboard messages. Does not unsubclass.
		/// </summary>
		class _ClipboardListener :LibWaitVariable
		{
			bool _paste; //true if used for paste, false if for copy
			object _data; //string or Data. null if !_paste.
			Native.WNDPROC _wndProc;
			//Wnd _wPrevClipViewer;
			Wnd _wFocus;

			/// <summary>
			/// The clipboard message has been received. Probably the target window responded to the Ctrl+C or Ctrl+V.
			/// On Paste it is unreliable because of clipboard viewers/managers/etc. The caller also must check IsBadWindow.
			/// </summary>
			public bool Success => waitVar;

			/// <summary>
			/// On Paste, true if probably not the target process retrieved clipboard data. Probably a clipboard viewer/manager/etc.
			/// Not used on Copy.
			/// </summary>
			public bool IsBadWindow;

			/// <summary>
			/// Exception thrown/catched when failed to set clipboard data.
			/// </summary>
			public Exception FailedToSetData;

			/// <summary>
			/// Subclasses clipOwner.
			/// </summary>
			/// <param name="paste">true if used for paste, false if for copy.</param>
			/// <param name="data">If used for paste, can be string containing Unicode text or int/string dictionary containing clipboard format/data.</param>
			/// <param name="clipOwner">Our clipboard owner window.</param>
			/// <param name="wFocus">The target control or window.</param>
			public _ClipboardListener(bool paste, object data, Wnd clipOwner, Wnd wFocus)
			{
				_paste = paste;
				_data = data;
				_wndProc = _WndProc;
				_wFocus = wFocus;
				clipOwner.SetWindowLong(Native.GWL.WNDPROC, Marshal.GetFunctionPointerForDelegate(_wndProc));

				//rejected: use SetClipboardViewer to block clipboard managers/viewers/etc. This was used in QM2.
				//	Nowadays most such programs don't use SetClipboardViewer. Probably they use AddClipboardFormatListener.
				//	known apps that have clipboard viewer installed with SetClipboardViewer:
				//		OpenOffice, LibreOffice: tested Writer, Calc.
				//		VLC: after first Paste.
				//_wPrevClipViewer = Api.SetClipboardViewer(clipOwner);
				//Print(_wPrevClipViewer);
			}

			/// <summary>
			/// Waits until the target app gets (Paste) or sets (Copy) clipboard text.
			/// Throws AuException on timeout (3 s normally, 28 s if the target window is hung).
			/// </summary>
			/// <param name="ctrlKey">The variable that was used to send Ctrl+V or Ctrl+C. This function may call Release to avoid too long Ctrl down.</param>
			public void Wait(ref Keyb.Lib.SendCopyPaste ctrlKey)
			{
				//Print(Success); //on Paste often already true, because SendInput dispatches sent messages
				for(int n = 6; !Success;) { //max 3 s (6*500 ms). If hung, max 28 s.
					WaitFor.LibWait(500, WHFlags.DoEvents, null, this);

					if(Success) break;
					//is hung?
					if(--n == 0) throw new AuException(_paste ? "*paste" : "*copy");
					ctrlKey.Release();
					_wFocus.SendTimeout(5000, 0, flags: 0);
				}
			}

			LPARAM _WndProc(Wnd w, int message, LPARAM wParam, LPARAM lParam)
			{
				//Wnd.Misc.PrintMsg(w, message, wParam, lParam);

				switch(message) {
				//case Api.WM_DESTROY:
				//	Api.ChangeClipboardChain(w, _wPrevClipViewer);
				//	break;
				case Api.WM_RENDERFORMAT:
					if(_paste && !Success) {
						IsBadWindow = !_IsTargetWindow();

						//note: need to set clipboard data even if bad window.
						//	Else the clipboard program may retry in loop. Eg Ditto. Then often pasting fails.
						//	If IsBadWindow, we'll then sleep briefly.
						//	Clipboard programs get clipboard data with a delay. Eg Ditto default is 100 ms and can be changed. Therefore usually they don't interfere, unless the target app is very slow.
						//	Also, after setting clipboard data we cannot wait for good window, because we'll not receive second WM_RENDERFORMAT.

						try { _SetClipboard(_data, false); }
						catch(Exception ex) { FailedToSetData = ex; } //cannot throw in wndproc, will throw later
						waitVar = true;
					}
					return 0;
				case Api.WM_CLIPBOARDUPDATE:
					//this message was added in Vista. It is posted, not sent. Once, not for each format. QM2 used SetClipboardViewer/WM_DRAWCLIPBOARD.
					if(!_paste) waitVar = true;
					return 0;
				}

				return Api.DefWindowProc(w, message, wParam, lParam);

				//Returns false if probably not the target app reads from the clipboard. Probably a clipboard viewer/manager/etc.
				bool _IsTargetWindow()
				{
					Wnd wOC = Api.GetOpenClipboardWindow();

					//int color = 0; if(wOC != _wFocus) color = wOC.ProcessId == _wFocus.ProcessId ? 0xFF0000 : 0xFF;
					//Print($"<><c {color}>{wOC}</c>");

					if(wOC == _wFocus) return true;
					if(wOC.Is0) return true; //tested: none of tested apps calls OpenClipboard(0)
					if(wOC.ProcessId == _wFocus.ProcessId) return true; //often classnamed "CLIPBRDWNDCLASS". Some clipboard managers are classnamed so too, eg Ditto.
					if(wOC.ProgramName.Eqi("RuntimeBroker.exe")) return true; //Edge, Store apps

					//CONSIDER: option to return true for user-known windows. Also show warning or info that includes wOC info.

					Dbg.Print(wOC.ToString());
					return false;

					//BlueStacks problems:
					//	Uses an aggressive viewer. Always debugprints while it is running, even when other apps are active.
					//	Sometimes pastes old text, especially after starting BlueStacks or after some time of not using it.
					//		With or without clipboard restoring.
					//		Then starts to work correctly always. Difficult to debug.
					//		KeySpeedClipboard=100 usually helps, but sometimes even 300 does not help.
				}
			}
		}

		/// <summary>
		/// Opens and closes clipboard using API OpenClipboard and CloseClipboard.
		/// Constructor tries to open for 10 s, then throws AuException.
		/// If the 'createOwner' parameter is true, creates temporary message-only clipboard owner window.
		/// If the 'noOpenNow' parameter is true, does not open, only creates owner if need.
		/// Dispose() closes clipboard and destroys the owner window.
		/// </summary>
		struct _OpenClipboard :IDisposable
		{
			bool _isOpen;
			Wnd _w;

			public Wnd WndClipOwner => _w;

			public _OpenClipboard(bool createOwner, bool noOpenNow = false)
			{
				_isOpen = false;
				_w = default;
				if(createOwner) {
					_w = Wnd.Lib.CreateMessageWindowDefWndProc();
					//MSDN says, SetClipboardData fails if OpenClipboard called with 0 hwnd. It doesn't, but better use hwnd.
					//Creating/destroying window is the slowest part of SetText().
				}
				if(!noOpenNow) Reopen();
			}

			/// <summary>
			/// Opens again.
			/// Must be closed.
			/// Owner window should be not destroyed; does not create again.
			/// </summary>
			/// <param name="noThrow">If fails, return false, no exception. Also then waits 1 s instead of 10 s.</param>
			/// <exception cref="AuException">Failed to open.</exception>
			public bool Reopen(bool noThrow = false)
			{
				Debug.Assert(!_isOpen);
				var to = new WaitFor.Loop(noThrow ? -1 : -10, 1);
				while(!Api.OpenClipboard(_w)) {
					int ec = WinError.Code;
					if(!to.Sleep()) {
						Dispose();
						if(noThrow) return false;
						throw new AuException(ec, "*open clipboard");
					}
				}
				_isOpen = true;
				return true;
			}

			public void Close(bool destroyOwnerWindow)
			{
				if(_isOpen) {
					Api.CloseClipboard();
					_isOpen = false;
				}
				if(destroyOwnerWindow && !_w.Is0) {
					Api.DestroyWindow(_w);
					_w = default;
				}
			}

			public void Dispose() => Close(true);
		}

		/// <summary>
		/// Saves and restores clipboard data.
		/// Clipboard must be open. Don't need to call EmptyClipboard before Restore.
		/// </summary>
		struct _SaveRestore
		{
			Dictionary<int, byte[]> _data;

			public void Save(bool debug = false)
			{
				var p1 = new Perf.Inst(); //will need if debug==true. Don't delete the Perf statements, they are used by a public function.
				bool allFormats = OptKey.RestoreClipboardAllFormats || debug;
				string[] exceptFormats = OptKey.RestoreClipboardExceptFormats;

				for(int format = 0; 0 != (format = Api.EnumClipboardFormats(format));) {
					bool skip = false; string name = null;
					if(!allFormats) {
						skip = format != Api.CF_UNICODETEXT;
					} else {
						//standard, private
						if(format < Api.CF_MAX) { //standard
							switch(format) {
							case Api.CF_OEMTEXT: //synthesized from other text formats
							case Api.CF_BITMAP: //synthesized from DIB formats
							case Api.CF_PALETTE: //rare, never mind
								skip = true;
								break;
							case Api.CF_METAFILEPICT:
							case Api.CF_ENHMETAFILE:
								skip = true; //never mind, maybe in the future
								break;
							}
						} else if(format < 0xC000) { //CF_OWNERDISPLAY, DSP, GDI, private
							skip = true; //never mind. Not auto-freed, etc. Rare.
						} //else registered

						if(!skip && exceptFormats != null && exceptFormats.Length != 0) {
							name = LibGetFormatName(format);
							foreach(string s in exceptFormats) if(s.Eqi(name)) { skip = true; break; }
						}
					}

					if(debug) {
						if(name == null) name = LibGetFormatName(format);
						if(skip) Print($"{name,-62}  restore=False");
						else p1.First();
						//note: we don't call GetClipboardData for formats in exceptFormats, because the conditions must be like when really saving. Time of GetClipboardData(format2) may depend on whether called GetClipboardData(format1).
					}
					if(skip) continue;

					var data = Api.GetClipboardData(format);

					int size = (data == default) ? 0 : (int)Api.GlobalSize(data);
					if(size == 0 || size > 10 * 1024 * 1024) skip = true;
					//If data == default, probably the target app did SetClipboardData(NULL) but did not render data on WM_RENDERFORMAT.
					//	If we try to save/restore, we'll receive WM_RENDERFORMAT too. It can be dangerous.

					if(debug) {
						p1.Next();
						Print($"{name,-32}  time={p1.TimeTotal,-8}  size={size,-8}  restore={!skip}");
						continue;
					}
					if(skip) continue;

					var b = Api.GlobalLock(data);
					Debug.Assert(b != default); if(b == default) continue;
					try {
						if(_data == null) _data = new Dictionary<int, byte[]>();
						var a = new byte[size];
						Marshal.Copy(b, a, 0, size);
						_data.Add(format, a);
					}
					finally { Api.GlobalUnlock(data); }
				}
			}

			public void Restore()
			{
				if(_data == null) return;
				_EmptyClipboard();
				foreach(var v in _data) {
					var a = v.Value;
					var h = Api.GlobalAlloc(Api.GMEM_MOVEABLE, a.Length);
					var b = Api.GlobalLock(h);
					if(b != default) {
						try { Marshal.Copy(a, 0, b, a.Length); } finally { Api.GlobalUnlock(h); }
						if(default == Api.SetClipboardData(v.Key, h)) b = default;
					}
					Debug.Assert(b != default);
					if(b == default) Api.GlobalFree(h);
				}
			}

			public bool IsSaved => _data != null;
		}

		internal static unsafe string LibGetFormatName(int format)
		{
			//registered
			if(format >= 0xC000) {
				var b = stackalloc char[300];
				int len = Api.GetClipboardFormatName(format, b, 300);
				if(len > 0) return Util.StringCache.LibAdd(b, len);
			}
			//standard
			string s = null;
			switch(format) { case Api.CF_TEXT: s = "CF_TEXT"; break; case Api.CF_BITMAP: s = "CF_BITMAP"; break; case Api.CF_METAFILEPICT: s = "CF_METAFILEPICT"; break; case Api.CF_SYLK: s = "CF_SYLK"; break; case Api.CF_DIF: s = "CF_DIF"; break; case Api.CF_TIFF: s = "CF_TIFF"; break; case Api.CF_OEMTEXT: s = "CF_OEMTEXT"; break; case Api.CF_DIB: s = "CF_DIB"; break; case Api.CF_PALETTE: s = "CF_PALETTE"; break; case Api.CF_RIFF: s = "CF_RIFF"; break; case Api.CF_WAVE: s = "CF_WAVE"; break; case Api.CF_UNICODETEXT: s = "CF_UNICODETEXT"; break; case Api.CF_ENHMETAFILE: s = "CF_ENHMETAFILE"; break; case Api.CF_HDROP: s = "CF_HDROP"; break; case Api.CF_LOCALE: s = "CF_LOCALE"; break; case Api.CF_DIBV5: s = "CF_DIBV5"; break; }
			return s ?? format.ToString();
		}

		internal static void LibPrintClipboard()
		{
			Print("---- Clipboard ----");
			using(var oc = new _OpenClipboard(true)) {
				Api.GetClipboardData(0); //JIT
				var save = new _SaveRestore();
				save.Save(true);
			}
		}
	}

	public static partial class NoClass
	{
		/// <summary>
		/// Calls <see cref="Clipb.PasteText"/>.
		/// </summary>
		/// <exception cref="AuException">Failed.</exception>
		public static void Paste(string text) => Clipb.PasteText(text);

		/// <summary>
		/// Calls <see cref="Clipb.CopyText"/>.
		/// </summary>
		/// <exception cref="AuException">Failed.</exception>
		public static string CopyText() => Clipb.CopyText();
	}
}
