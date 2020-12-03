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
using Au.Util;
using System.Windows.Automation.Peers;
using System.Windows.Automation;
//using System.Linq;

namespace Au.Controls
{
	using static Sci;

	//Why don't use ScintillaNET:
	// 1. Delays to update for the newest Scintilla version.
	// 2. Possibly will be abandoned some day.
	// 3. Adds yet another layer of bugs.
	// 4. I don't like some things how it is implemented. Eg aggressively uses "clamping", creates much garbage, eg new Line object for each line-related Scintilla message call.
	// 5. For me it does not make easier because I used Scintilla in QM2 (C++) and am familiar with its API (which is well documented). When using ScintillaNET, I often search its source code just to find which function calls the API I need. Now I can simply convert much of QM2 code to C#.
	// 6. I use modified Scintilla. Would need to synchronize some modifications with ScintillaNET. For example I use a callback function instead of WM_NOTIFY/WM_REFLECT.

	/// <summary>
	/// This .NET control wraps native Scintilla control.
	/// This is not a universal Scintilla wrapper class. Designed just for purposes of this library and related software.
	/// Creates and initializes the control. Also used to set/change control properties.
	/// The Z property returns a SciText object that can be used to work with text, code styling etc.
	/// </summary>
	public unsafe class SciHost : HwndHost //named not Scintilla, to avoid confusion with native Scintilla control //TODO: rename to AuScintilla when the winforms version removed
	{
		AWnd _w;
		LPARAM _sciPtr;
		Sci_NotifyCallback _notifyCallback;
		int _dpi;

		public LPARAM SciPtr => _sciPtr;

		[Browsable(false)]
		public SciImages ZImages { get; private set; }

		[Browsable(false)]
		public SciTags ZTags { get; private set; }

		/// <summary>
		/// Gets the SciText object that contains most Scintilla-related functions.
		/// </summary>
		public readonly SciText Z;

		public AWnd Hwnd => _w;

		static SciHost() {
			if (default == Api.GetModuleHandle("SciLexer.dll")) //TODO: remove this line when removed the winforms version
				AFile.More.LoadDll64or32Bit("SciLexer.dll");
		}

		public SciHost() {
			Z = new SciText(this);
		}

		#region HwndHost

		protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
			var wParent = (AWnd)hwndParent.Handle;
			_dpi = ADpi.OfWindow(wParent);
			WS style = WS.CHILD; if (ZInitBorder) style |= WS.BORDER;
			//note: no WS_VISIBLE. WPF will manage it. It can cause visual artefacts occasionally, eg scrollbar in WPF area.
			_w = AWnd.More.CreateWindow("Scintilla", null, style, 0, 0, 0, 0, 0, wParent);
			//size 0 0 is not the best, but it is a workaround for WPF bugs

			_sciPtr = _w.Send(SCI_GETDIRECTPOINTER);
			Call(SCI_SETNOTIFYCALLBACK, 0, Marshal.GetFunctionPointerForDelegate(_notifyCallback = _NotifyCallback));

			bool hasImages = ZInitImagesStyle != ZImagesStyle.NoImages;
			bool hasTags = ZInitTagsStyle != ZTagsStyle.NoTags;
			if (ZInitReadOnlyAlways) {
				MOD mask = 0;
				if (hasImages || hasTags) mask |= MOD.SC_MOD_INSERTTEXT | MOD.SC_MOD_DELETETEXT;
				Call(SCI_SETMODEVENTMASK, (int)mask);
			}
			_InitDocument();
			Call(SCI_SETSCROLLWIDTHTRACKING, 1);
			Call(SCI_SETSCROLLWIDTH, ADpi.Scale(100, _dpi));
			if (!ZInitUseDefaultContextMenu) Call(SCI_USEPOPUP);
			Call(SCI_SETCARETWIDTH, ADpi.Scale(2, _dpi));
			if (ZInitWrapVisuals) {
				Call(SCI_SETWRAPVISUALFLAGS, SC_WRAPVISUALFLAG_START | SC_WRAPVISUALFLAG_END);
				Call(SCI_SETWRAPVISUALFLAGSLOCATION, SC_WRAPVISUALFLAGLOC_END_BY_TEXT);
				Call(SCI_SETWRAPINDENTMODE, SC_WRAPINDENT_INDENT);
			}
			if (ZWrapLines) {
				Call(SCI_SETWRAPMODE, SC_WRAP_WORD);
			}

			//note: cannot set styles here, because later inherited class will call StyleClearAll, which sets some special styles.

			if (hasImages) ZImages = new SciImages(this, ZInitImagesStyle == ZImagesStyle.AnyString);
			if (hasTags) ZTags = new SciTags(this);

			if (this.AllowDrop) Api.RevokeDragDrop(_w);//TODO

			if (FocusManager.GetFocusScope(this) is Window fs && FocusManager.GetFocusedElement(fs) == this && Api.GetFocus() == wParent)
				Api.SetFocus(_w);

			OnHandleCreated();

			return new HandleRef(this, _w.Handle);
		}

		protected virtual void OnHandleCreated() => ZHandleCreated?.Invoke();

		public event Action ZHandleCreated;

		void _InitDocument() {
			//these must be set for each document of this Scintilla window

			Call(SCI_SETCODEPAGE, Api.CP_UTF8);
			Call(SCI_SETTABWIDTH, 4);
			if (ZInitReadOnlyAlways) {
				Call(SCI_SETREADONLY, 1);
				Call(SCI_SETUNDOCOLLECTION);
				Call(SCI_SETLEXER, (int)LexLanguage.SCLEX_NULL); //don't need style-needed notification, we'll set styles for whole text
			} //else if (_isReadOnly) Call(SCI_SETREADONLY, 1);
		}

		protected override void DestroyWindowCore(HandleRef hwnd) {
			//AOutput.Write("DESTROY");
			AWnd.More.DestroyWindow((AWnd)hwnd.Handle);
		}

		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParamIP, IntPtr lParam, ref bool handled) {
			nint wParam = wParamIP; //C# compiler bug: if using nint parameters instead of IntPtr, inherited classes cannot override this method.

			//if(Tag is string s1 && s1 == "test") AWnd.More.PrintMsg(_w, msg, wParam, lParam);
			//if(this.Parent?.Name == "Output") AWnd.More.PrintMsg(_w, msg, wParam, lParam, Api.WM_TIMER, Api.WM_MOUSEMOVE, Api.WM_SETCURSOR, Api.WM_NCHITTEST, Api.WM_PAINT, Api.WM_IME_SETCONTEXT, Api.WM_IME_NOTIFY);
			//if(IsFocused) AWnd.More.PrintMsg(_w, msg, wParam, lParam, Api.WM_TIMER, Api.WM_MOUSEMOVE, Api.WM_SETCURSOR, Api.WM_NCHITTEST, Api.WM_PAINT, Api.WM_IME_SETCONTEXT, Api.WM_IME_NOTIFY);

			bool call = false;
			switch (msg) {
			case Api.WM_DESTROY:
				_w = default;
				break;
			case Api.WM_LBUTTONDOWN:
				if (Api.GetFocus() != _w) {
					bool setFocus = true;
					ZTags?.OnLButtonDownWhenNotFocused_(wParam, lParam, ref setFocus); //Tags may not want to set focus eg when a hotspot clicked
					if (call = (!setFocus || ZNoMouseSetFocus.Has(MButtons.Left))) wParam |= MK_SCI_NOFOCUS;
				}
				break;
			case Api.WM_RBUTTONDOWN:
				if (call = ZNoMouseSetFocus.Has(MButtons.Right)) wParam |= MK_SCI_NOFOCUS;
				break;
			case Api.WM_MBUTTONDOWN:
				if (!ZNoMouseSetFocus.Has(MButtons.Middle)) this.Focus();
				break;
			case Api.WM_SETFOCUS:
				_OnWmSetFocus();
				break;
			case Api.WM_GETOBJECT:
				handled = true;
				return (_acc ??= new _Accessible(this)).WmGetobject(wParam, lParam);
			}

			if (call) {
				handled = true;
				return (nint)Call(msg, wParam, lParam);
			}

			nint R = base.WndProc(hwnd, msg, wParam, lParam, ref handled);

			switch (msg) {
			case Api.WM_CREATE: //after inherited classes set styles etc
				if (!_text.NE()) Z.SetText(_text, SciSetTextFlags.NoUndoNoNotify);
				break;
			}

			return R;
		}

		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi) {
			_dpi = newDpi.PixelsPerInchY.ToInt();
			Call(SCI_SETCARETWIDTH, ADpi.Scale(2, _dpi));
			base.OnDpiChanged(oldDpi, newDpi);
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
			Focus();
			return true;
			//base.TabIntoCore(request); //empty func, returns false
		}

		protected override bool TranslateAcceleratorCore(ref MSG msg, ModifierKeys modifiers) {
			var m = msg.message;
			var k = (KKey)msg.wParam;
			//if (m == Api.WM_KEYDOWN) AOutput.Write(m, k);
			if (m is Api.WM_KEYDOWN or Api.WM_KEYUP /*or Api.WM_SYSKEYDOWN or Api.WM_SYSKEYUP*/)
				if (!modifiers.Has(ModifierKeys.Alt))
					if (k == KKey.Left || k == KKey.Right || k == KKey.Up || k == KKey.Down
						|| (!Z.IsReadonly && ((k == KKey.Enter && modifiers == 0) || (k == KKey.Tab && !modifiers.Has(ModifierKeys.Control))))) {
						Call(msg.message, msg.wParam, msg.lParam); //not DispatchMessage or Send
						return true;
					}

			return base.TranslateAcceleratorCore(ref msg, modifiers);
		}

		#endregion

		#endregion

		void _NotifyCallback(void* cbParam, ref SCNotification n) {
			var code = n.nmhdr.code;
			//if(code != NOTIF.SCN_PAINTED) AOutput.QM2.Write(code.ToString());
			switch (code) {
			case NOTIF.SCN_MODIFIED:
				if ((n.modificationType & (MOD.SC_MULTISTEPUNDOREDO | MOD.SC_LASTSTEPINUNDOREDO)) == MOD.SC_MULTISTEPUNDOREDO) return;
				_NotifyModified(n);
				if (ZDisableModifiedNotifications) return;
				break;
			case NOTIF.SCN_HOTSPOTRELEASECLICK:
				ZTags?.OnLinkClick_(n.position, 0 != (n.modifiers & SCMOD_CTRL));
				break;
			}
			ZOnSciNotify(ref n);
		}

		//void _Print(object text)
		//{
		//	var t = Name ?? GetType().ToString();
		//	if(Name != "Status_text") AOutput.QM2.Write($"{t}: {text}");
		//}

		unsafe void _NotifyModified(in SCNotification n) {
			var code = n.modificationType;
			//if(this.Name!= "Output_text") AOutput.Write(code, n.position);
			if (0 != (code & (MOD.SC_MOD_INSERTTEXT | MOD.SC_MOD_DELETETEXT))) {
				_text = null;
				_posState = default;
				_aPos.Clear();

				bool ins = 0 != (code & MOD.SC_MOD_INSERTTEXT);
				ZImages?.OnTextChanged_(ins, n);
				ZTags?.OnTextChanged_(ins, n);
			}
			//if(0!=(code& MOD.SC_MOD_CHANGEANNOTATION)) ChangedAnnotation?.Invoke(this, ref n);
		}

		/// <summary>
		/// Raises the <see cref="ZNotify"/> event.
		/// </summary>
		protected virtual void ZOnSciNotify(ref SCNotification n) {
			ZNotify?.Invoke(this, ref n);
			var e = ZTextChanged;
			if (e != null && n.nmhdr.code == NOTIF.SCN_MODIFIED && 0 != (n.modificationType & (MOD.SC_MOD_INSERTTEXT | MOD.SC_MOD_DELETETEXT))) e(this, EventArgs.Empty);
		}

		public delegate void ZEventHandler(SciHost c, ref SCNotification n);

		/// <summary>
		/// Occurs when any Scintilla notification is received.
		/// </summary>
		public event ZEventHandler ZNotify;

		/// <summary>
		/// Occurs when text changed.
		/// </summary>
		public event EventHandler ZTextChanged;

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
#if DEBUG
			if (ZDebugPrintMessages) _DebugPrintMessage(sciMessage);
#endif

			Debug.Assert(!_w.Is0);
			//Debug.Assert(!_w.Is0 || this.DesignMode);
			//if(!IsHandleCreated) CreateHandle();
			//note: auto-creating handle is not good:
			//	1. May create parked control. Not good for performance.
			//	2. Can be dangerous, eg if passing a reusable buffer that also is used when creating handle.

			return Sci_Call(_sciPtr, sciMessage, wParam, lParam);
		}

		//public bool IsReadonly {
		//	get => _isReadOnly;
		//	set {
		//		if (value != _isReadOnly) {
		//			_isReadOnly = value;
		//			if (!_w.Is0) Call(SCI_SETREADONLY, _isReadOnly);
		//		}
		//	}
		//}
		//bool _isReadOnly;

		/// <summary>
		/// Gets or sets text.
		/// Uses caching, therefore the 'get' function is fast and garbage-free when calling multiple times.
		/// </summary>
		/// <remarks>
		/// The 'get' function gets cached text if called not the first time after setting or modifying control text.
		/// The 'set' function calls <see cref="SciTextF.SetText"/> when need. Uses default parameters (with undo and notifications, unless ZInitReadOnlyAlways).
		/// Unlike the above methods, this property can be used before creating handle.
		/// </remarks>
		public string Text {
			get {
				//AOutput.QM2.Write($"Text: cached={_text != null}");
				if (_text == null && !_w.Is0) _text = Z.GetText_(); //_NotifyModified sets _text=null
				return _text;
			}
			set {
				if (!_w.Is0) Z.SetText(value); //_NotifyModified sets _text=null. Control text can be != value, eg when tags parsed.
				else _text = value; //will set control text on WM_CREATE
			}
		}
		string _text;

		#region len, pos

		/// <summary>
		/// UTF-8 text length.
		/// </summary>
		public int Len8 => _posState == _PosState.Ok ? _len8 : Call(SCI_GETTEXTLENGTH);

		/// <summary>
		/// UTF-16 text length.
		/// </summary>
		public int Len16 {
			get {
				if (_text != null) return _text.Length;
				if (_posState == default) _CreatePosMap();
				if (_posState == _PosState.Ok) return _len16;
				return Pos16(Call(SCI_GETTEXTLENGTH));
			}
		}

#if true
		/// <summary>
		/// Converts UTF-16 position to UTF-8 position. Fast.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Negative or greater than <see cref="Len16"/>.</exception>
		public int Pos8(int pos16) {
			Debug.Assert((uint)pos16 <= Len16);
			if (pos16 < 0) throw new ArgumentOutOfRangeException();
			if (_posState == default) _CreatePosMap();
			if (_posState == _PosState.Ok) {
				if (pos16 > _len16) throw new ArgumentOutOfRangeException();
				//using binary search find max _aPos[r].i16 that is < pos16
				int r = -1, from = 0, to = _aPos.Count;
				while (to > from) {
					int m = (from + to) / 2;
					if (_aPos[m].i16 < pos16) from = (r = m) + 1; else to = m;
				}
				if (r < 0) return pos16; //_aPos is empty (ASCII text) or pos16 <= _aPos[0].i16 (before first non-ASCII character)
				var p = _aPos[r];
				return p.i8 + Math.Min(pos16 - p.i16, p.len16) * p.charLen + Math.Max(pos16 - (p.i16 + p.len16), 0); //p.i8 + utf + ascii
			} else {
				var s = Text;
				return Encoding.UTF8.GetByteCount(s, 0, pos16);
				//note: don't use SCI_POSITIONRELATIVECODEUNITS, it is very slow.
			}
		}

		/// <summary>
		/// Converts UTF-8 position to UTF-16 position. Fast.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Negative or greater than <see cref="Len8"/>.</exception>
		public unsafe int Pos16(int pos8) {
			Debug.Assert((uint)pos8 <= Len8);
			if (pos8 < 0) throw new ArgumentOutOfRangeException();
			if (_posState == default) _CreatePosMap();
			if (_posState == _PosState.Ok) {
				if (pos8 > _len8) throw new ArgumentOutOfRangeException();
				//using binary search find max _aPos[r].i8 that is < pos8
				int r = -1, from = 0, to = _aPos.Count;
				while (to > from) {
					int m = (from + to) / 2;
					if (_aPos[m].i8 < pos8) from = (r = m) + 1; else to = m;
				}
				if (r < 0) return pos8; //_aPos is empty (ASCII text) or pos8 <= _aPos[0].i8 (before first non-ASCII character)
				var p = _aPos[r];
				int len8 = p.len16 * p.charLen;
				return p.i16 + Math.Min(pos8 - p.i8, len8) / p.charLen + Math.Max(pos8 - (p.i8 + len8), 0); //p.i16 + utf + ascii
			} else {
				int gap = Sci_Range(_sciPtr, 0, pos8, out var p1, out var p2);
				int R = Encoding.UTF8.GetCharCount(p1, p2 == null ? pos8 : gap);
				if (p2 != null) R += Encoding.UTF8.GetCharCount(p2, pos8 - gap);
				return R;
				//note: don't use SCI_COUNTCODEUNITS, it is very slow.
			}
		}

		//public void TestCreatePosMap()
		//{
		//	_CreatePosMap();
		//	//foreach(var v in _aPos) AOutput.Write(v.i8, v.i16);
		//}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		unsafe void _CreatePosMap() {
			//This func is fast and garbageless. For code edit controls don't need to optimize to avoid calling it frequently, eg for each added character.
			//Should not be used for output/log controls if called on each "append text". Or then need to optimize.
			//AOutput.QM2.Write(this.Name);

			_aPos.Clear();

			int textLen;
			int gap = Sci_Range(_sciPtr, 0, -1, out var p, out var p2, &textLen);
			int to8 = p2 == null ? textLen : gap;
			int i8 = 0, i16 = 0;
			g1:
			int asciiStart8 = i8;
			i8 = _SkipAscii(p, i8, to8);
			i16 += i8 - asciiStart8;
			if (i8 < to8) {
				int utfStart8 = i8, utfStart16 = i16, c = p[i8];
				if (c < 0xE0) { //2-byte UTF-8 chars
					for (; i8 < to8 && (c = p[i8]) >= 0xC2 && c < 0xE0; i8 += 2) i16++;
				} else if (c < 0xF0) { //3-byte UTF-8 chars
					for (; i8 < to8 && (c = p[i8]) >= 0xE0 && c < 0xF0; i8 += 3) i16++;
				} else { //4-byte UTF-8 chars
					for (; i8 < to8 && (c = p[i8]) >= 0xF0 && c < 0xF8; i8 += 4) i16 += 2;
				}
				int len16 = i16 - utfStart16;
				if (len16 > 0) _aPos.Add(new _PosUtfRange(utfStart8, utfStart16, len16, (i8 - utfStart8) / len16));
				if (i8 < to8) {
					if (c >= 0x80) { if (c < 0xC2 || c > 0xF8) goto ge; }
					goto g1;
				}
				if (i8 > to8) goto ge;
			}

			if (p2 != null) {
				p = p2 - i8;
				p2 = null;
				to8 = textLen;
				goto g1;
			}

			_posState = _PosState.Ok;
			_len8 = textLen;
			_len16 = i16;
			return;
			ge:
			_posState = _PosState.Error;
			_aPos.Clear();
			ADebug.Print("Invalid UTF-8 text");
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		static unsafe int _SkipAscii(byte* bp, int i, int len) {
			for (; i < len && (i & 7) != 0; i++) if (bp[i] >= 0x80) return i;
			var up = (ulong*)(bp + i);
			int j = 0;
			for (int n = (len - i) / 8; j < n; j++) if ((up[j] & 0x8080808080808080) != 0) break;
			for (i += j * 8; i < len; i++) if (bp[i] >= 0x80) break;
			return i;
		}

		struct _PosUtfRange
		{
			public int i8, i16, len16, charLen; //note: len16 is UTF-16 code units; if surrogate pairs, charLen is 2 (2 bytes for each UTF-16 code unit)
			public _PosUtfRange(int i8, int i16, int len16, int charLen) {
				this.i8 = i8; this.i16 = i16; this.len16 = len16; this.charLen = charLen;
			}
		}
		List<_PosUtfRange> _aPos = new List<_PosUtfRange>();
#else //this code is slightly simpler, but may need big array (1 element for 1 non-ASCII char vs 1 element for 1 non-ASCII range). Speed similar.
		/// <summary>
		/// Converts UTF-16 position to UTF-8 position. Fast.
		/// </summary>
		public int Pos8(int pos16)
		{
			Debug.Assert((uint)pos16 <= Len16);
			if(_posState == default) _CreatePosMap();
			if(_posState == _PosState.Ok) {
				//using binary search find max _aPos[r].i16 that is <= pos16
				int r = -1, from = 0, to = _aPos.Count;
				while(to > from) {
					int m = (from + to) / 2;
					if(_aPos[m].i16 > pos16) to = m; else from = (r = m) + 1;
				}
				if(r < 0) return pos16; //_aPos is empty or pos16 < _aPos[0].i16
				return _aPos[r].i8 + (pos16 - _aPos[r].i16);
			} else {
				var s = Text;
				return Encoding.UTF8.GetByteCount(s, 0, pos16);
				//note: don't use SCI_POSITIONRELATIVECODEUNITS, it is very slow.
			}
		}

		/// <summary>
		/// Converts UTF-8 position to UTF-16 position. Fast.
		/// </summary>
		public unsafe int Pos16(int pos8)
		{
			Debug.Assert((uint)pos8 <= Len8);
			if(_posState == default) _CreatePosMap();
			if(_posState == _PosState.Ok) {
				//using binary search find max _aPos[r].i8 that is <= pos8
				int r = -1, from = 0, to = _aPos.Count;
				while(to > from) {
					int m = (from + to) / 2;
					if(_aPos[m].i8 > pos8) to = m; else from = (r = m) + 1;
				}
				if(r < 0) return pos8; //_aPos is empty or pos8 < _aPos[0].i8
				return _aPos[r].i16 + (pos8 - _aPos[r].i8);
			} else {
				int gap = Sci_Range(_sciPtr, 0, pos8, out var p1, out var p2);
				int R = Encoding.UTF8.GetCharCount(p1, p2 == null ? pos8 : gap);
				if(p2 != null) R += Encoding.UTF8.GetCharCount(p2, pos8 - gap);
				return R;
				//note: don't use SCI_COUNTCODEUNITS, it is very slow.
			}
		}

		public void TestCreatePosMap()//_TODO
		{
			_CreatePosMap();
			//foreach(var v in _aPos) AOutput.Write(v.i8, v.i16);
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		unsafe void _CreatePosMap()
		{
			_aPos.Clear();

			int textLen;
			int gap = Sci_Range(_sciPtr, 0, -1, out var p, out var p2, &textLen);
			//AOutput.Write(textLen);
			int to8 = p2 == null ? textLen : gap;
			int i8 = 0, i16 = 0;
			g1:
			int asciiStart8 = i8;
			i8 = _SkipAscii(p, i8, to8);
			i16 += i8 - asciiStart8;
			if(i8 < to8) {
				while(i8 < to8) {
					byte c = p[i8];
					if(c < 0x80) break;
					if(c < 0xC2) goto ge;
					if(c < 0xE0) i8++;
					else if(c < 0xF0) i8 += 2;
					else if(c < 0xF8) { i8 += 3; i16++; } //UTF-16 surrogate pair
					else goto ge;
					_aPos.Add(new _Pos8_16(++i8, ++i16));
				}
				if(i8 < to8) goto g1;
				if(i8 > to8) goto ge;
			}

			if(p2 != null) {
				p = p2 - i8;
				p2 = null;
				to8 = textLen;
				goto g1;
			}

			_posState = _PosState.Ok;
			_len8 = textLen;
			_len16 = i16;
			return;
			ge:
			_posState = _PosState.Error;
			_aPos.Clear();
			ADebug.Print("Invalid UTF-8 text");
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		static unsafe int _SkipAscii(byte* bp, int i, int len)
		{
			for(; i < len && (i & 7) != 0; i++) if(bp[i] >= 0x80) return i;
			var up = (ulong*)(bp + i);
			int j = 0;
			for(int n = (len - i) / 8; j < n; j++) if((up[j] & 0x8080808080808080) != 0) break;
			for(i += j * 8; i < len; i++) if(bp[i] >= 0x80) break;
			return i;
		}

		struct _Pos8_16
		{
			public int i8, i16;
			public _Pos8_16(int eith, int sixteen) { i8 = eith; i16 = sixteen; }
		}
		List<_Pos8_16> _aPos = new List<_Pos8_16>();
#endif

		enum _PosState { Default, Ok, Error }
		_PosState _posState;

		int _len8, _len16;

		#endregion

#if DEBUG
		void _DebugPrintMessage(int sciMessage) {
			if (sciMessage < Sci.SCI_START) return;
			switch (sciMessage) {
			case SCI_COUNTCODEUNITS:
			case SCI_POSITIONRELATIVECODEUNITS:
			case SCI_CANUNDO:
			case SCI_CANREDO:
			case SCI_GETREADONLY:
			case SCI_GETSELECTIONEMPTY:
				//case SCI_GETTEXTLENGTH:
				return;
			}
			if (s_debugPM == null) {
				s_debugPM = new Dictionary<int, string>();
				foreach (var v in typeof(Sci).GetFields()) {
					var s = v.Name;
					//AOutput.Write(v.Name);
					if (s.Starts("SCI_")) s_debugPM.Add((int)v.GetRawConstantValue(), s);
				}
			}
			if (!s_debugPM.TryGetValue(sciMessage, out var k)) {
				k = sciMessage.ToString();
			}
			AOutput.QM2.Write(k);
		}
		static Dictionary<int, string> s_debugPM;

		[Browsable(false), DefaultValue(false)]
		public bool ZDebugPrintMessages { get; set; }
#endif

		#region properties

		/// <summary>
		/// Border style.
		/// Must be set before creating control handle.
		/// </summary>
		[DefaultValue(false)]
		public virtual bool ZInitBorder { get; set; }

		/// <summary>
		/// Use the default Scintilla's context menu.
		/// Must be set before creating control handle.
		/// </summary>
		[DefaultValue(false)]
		public virtual bool ZInitUseDefaultContextMenu { get; set; }

		/// <summary>
		/// This control is used just to display text, not to edit.
		/// Must be set before creating control handle.
		/// </summary>
		[DefaultValue(false)]
		public virtual bool ZInitReadOnlyAlways { get; set; }

		/// <summary>
		/// See <see cref="ZInitImagesStyle"/>.
		/// </summary>
		public enum ZImagesStyle
		{
			/// <summary>Don't show images. The <see cref="ZImages"/> property is null.</summary>
			NoImages,

			/// <summary>Display only images specified in tags like &lt;image "image file path"&gt;, including icons of non-image file types.</summary>
			ImageTag,

			/// <summary>Display images specified in any string like "image file path", and only of image file types. Then limits image height to 10 lines.</summary>
			AnyString
		}

		/// <summary>
		/// Whether and how to show images.
		/// Must be set before creating control handle.
		/// </summary>
		[DefaultValue(ZImagesStyle.NoImages)]
		public virtual ZImagesStyle ZInitImagesStyle { get; set; }

		/// <summary>
		/// See <see cref="ZInitImagesStyle"/>.
		/// </summary>
		public enum ZTagsStyle
		{
			/// <summary>Don't support tags. The <see cref="ZTags"/> property is null.</summary>
			NoTags,

			/// <summary>Let <see cref="Text"/>, Z.SetText and Z.AppendText parse tags when the text has prefix "&lt;&gt;".</summary>
			AutoWithPrefix,

			/// <summary>Let <see cref="Text"/>, Z.SetText and Z.AppendText parse tags always.</summary>
			AutoAlways,

			/// <summary>Tags are parsed only when calling Tags.AddText.</summary>
			User,
		}

		/// <summary>
		/// Whether and when supports tags.
		/// Must be set before creating control handle.
		/// </summary>
		[DefaultValue(ZTagsStyle.NoTags)]
		public virtual ZTagsStyle ZInitTagsStyle { get; set; }

		/// <summary>
		/// Whether to show arrows etc to make wrapped lines more visible.
		/// Must be set before creating control handle.
		/// </summary>
		[DefaultValue(true)]
		public virtual bool ZInitWrapVisuals { get; set; } = true;

		/// <summary>
		/// Word-wrap.
		/// </summary>
		[DefaultValue(false)]
		public virtual bool ZWrapLines {
			get => _wrapLines;
			set {
				if (value != _wrapLines) {
					_wrapLines = value;
					if (!_w.Is0) Call(SCI_SETWRAPMODE, value ? SC_WRAP_WORD : SC_WRAP_NONE);
				}
			}
		}
		bool _wrapLines;

		/// <summary>
		/// Whether uses Enter key.
		/// If null (default), false if <see cref="ZInitReadOnlyAlways"/> is true.
		/// </summary>
		[DefaultValue(null)]
		public bool? ZAcceptsEnter { get; set; }

		/// <summary>
		/// On SCN_MODIFIED notifications suppress <see cref="ZOnSciNotify"/>, <see cref="ZNotify"/> and <see cref="ZTextChanged"/>.
		/// Use to temporarily disable 'modified' notifications. Never use SCI_SETMODEVENTMASK, because then the control would stop working correctly.
		/// </summary>
		[DefaultValue(false)]
		public bool ZDisableModifiedNotifications { get; set; }

		/// <summary>
		/// Don't set focus on mouse left/right/middle button down.
		/// </summary>
		[DefaultValue(default(MButtons))]
		public MButtons ZNoMouseSetFocus { get; set; }

		#endregion

		#region acc

		_Accessible _acc;

		class _Accessible : HwndHostAccessibleBase_
		{
			readonly SciHost _sci;

			internal _Accessible(SciHost sci) : base(sci, sci.Hwnd) {
				_sci = sci;
			}

			public override AccROLE Role(int child) => _sci.ZAccessibleRole;

			public override string Name(int child) => _sci.ZAccessibleName;

			public override string Description(int child) => _sci.ZAccessibleDescription;

			public override string Value(int child) => _sci.ZAccessibleValue;

			public override AccSTATE State(int child) {
				var r = base.State(child);
				if (_sci.Z.IsReadonly) r |= AccSTATE.READONLY;
				return r;
			}
		}

		protected virtual AccROLE ZAccessibleRole => AccROLE.TEXT; //_sci.ZInitReadOnlyAlways ? AccROLE.STATICTEXT : AccROLE.TEXT;

		protected virtual string ZAccessibleName => Name;

		protected virtual string ZAccessibleDescription => null;

		protected virtual string ZAccessibleValue => ZInitReadOnlyAlways ? Text?.Limit(0xffff) : null;

		#endregion
	}
}
