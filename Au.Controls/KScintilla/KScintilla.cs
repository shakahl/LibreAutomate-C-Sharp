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
//using System.Linq;

namespace Au.Controls
{
	using static Sci;

	/// <summary>
	/// This .NET control wraps native Scintilla control.
	/// It is not a universal Scintilla wrapper class. Just for this library and related software.
	/// </summary>
	/// <remarks>
	/// Most functions throw ArgumentOutOfRangeException when: 1. A position or line index argument is negative. 2. Scintilla returned a negative position or line index.
	/// If a position or line index argument is greater than text length or the number of lines, some functions return the text length or the last line, and it is documented; for other functions the behaviour is undefined, eg ArgumentOutOfRangeException or Scintilla's return value or like of the documented methods.
	/// 
	/// Almost all function/event names start with z or Z, because VS intellisense cannot group by inheritance and would mix with 300 WPF functions/events. Most public and internal Scintilla API wrapper functions have prefix z, others Z. Derived classes can use prefix for example zz and ZZ.
	/// </remarks>
	public unsafe partial class KScintilla : HwndHost
	{
		AWnd _w;
		LPARAM _sciPtr;
		Sci_NotifyCallback _notifyCallback;
		internal int _dpi;

		static KScintilla() {
			//if (default == Api.GetModuleHandle("SciLexer.dll"))
			AFile.More.LoadDll64or32Bit("SciLexer.dll");
		}

		public LPARAM ZSciPtr => _sciPtr;

		[Browsable(false)]
		public SciImages ZImages { get; private set; }

		[Browsable(false)]
		public SciTags ZTags { get; private set; }

		#region HwndHost

		public AWnd Hwnd => _w; //not ZHwnd, to avoid accidental use of extension method Hwnd()

		public event Action ZHandleCreated;

		protected virtual void ZOnHandleCreated() => ZHandleCreated?.Invoke();

		protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
			var wParent = (AWnd)hwndParent.Handle;
			_dpi = ADpi.OfWindow(wParent);
			WS style = WS.CHILD; if (ZInitBorder) style |= WS.BORDER;
			//note: no WS_VISIBLE. WPF will manage it. It can cause visual artefacts occasionally, eg scrollbar in WPF area.
			_w = AWnd.More.CreateWindow("Scintilla", null, style, 0, 0, 0, 0, 0, wParent);
			//size 0 0 is not the best, but it is a workaround for WPF bugs

			//CONSIDER: register window class "KScintilla"

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

			//note: cannot set styles here, because later inherited class will call zStyleClearAll, which sets some special styles.

			if (hasImages) ZImages = new SciImages(this, ZInitImagesStyle == ZImagesStyle.AnyString);
			if (hasTags) ZTags = new SciTags(this);

			if (FocusManager.GetFocusScope(this) is Window fs && FocusManager.GetFocusedElement(fs) == this && Api.GetFocus() == wParent)
				Api.SetFocus(_w);

			ZOnHandleCreated();

			if (!_text.NE()) zSetText(_text, SciSetTextFlags.NoUndoNoNotify); //after inherited classes set styles etc

			return new HandleRef(this, _w.Handle);
		}

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
			AWnd.More.DestroyWindow((AWnd)hwnd.Handle);
			_w = default;
			_acc?.Dispose(); _acc = null;
		}

		//protected override void Dispose(bool disposing) {
		//	base.Dispose(disposing); //then follows DestroyWindowCore, probably base calls it
		//}

		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParamIP, IntPtr lParam, ref bool handled) {
			nint wParam = wParamIP; //C# compiler bug: if using nint parameters instead of IntPtr, inherited classes cannot override this method.

			//if(Tag is string s1 && s1 == "test") AWnd.More.PrintMsg(_w, msg, wParam, lParam);
			//if(this.Parent?.Name == "Output") AWnd.More.PrintMsg(_w, msg, wParam, lParam, Api.WM_TIMER, Api.WM_MOUSEMOVE, Api.WM_SETCURSOR, Api.WM_NCHITTEST, Api.WM_PAINT, Api.WM_IME_SETCONTEXT, Api.WM_IME_NOTIFY);
			//if () AWnd.More.PrintMsg(_w, msg, wParam, lParam);

			bool call = false;
			switch (msg) {
			//case Api.WM_DESTROY:
			//	AOutput.Write("destroy"); //no. It seems WPF removes the hook before destroying. Cleanup in DestroyWindowCore.
			//	break;
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

			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}

		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi) {
			if (!_w.Is0 && newDpi.PixelsPerDip != oldDpi.PixelsPerDip) {
				_dpi = newDpi.PixelsPerInchY.ToInt();
				Call(SCI_SETCARETWIDTH, ADpi.Scale(2, _dpi));
				zMarginWidthsDpiChanged_();
			}
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
		//4. When closing parent window, does not destroy hwnhosted controls. Instead moves to a hidden parking window, and destroys later on GC if you are careful.
		//	Need to always test whether hwnhosted controls are destroyed on GC, to avoid leaked windows + many managed objects.
		//	Eg to protect wndproc delegate from GC don't add it to a thread-static array until destroyed; let it be a field of the wrapper class.
		//	Or let app dispose the HwndHost in OnClosing. But control itself cannot reliably know when to self-destroy.
		//5. When closing parent window, briefly tries to show native control, and focus if was focused.
		//	Workaround: let app dispose the HwndHost in OnClosing.
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
						|| (!zIsReadonly && ((k == KKey.Enter && modifiers == 0) || (k == KKey.Tab && !modifiers.Has(ModifierKeys.Control))))) {
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

		public delegate void ZEventHandler(KScintilla c, ref SCNotification n);

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
			if (ZDebugPrintMessages_) _DebugPrintMessage(sciMessage);
#endif

			Debug.Assert(!_w.Is0);
			//Debug.Assert(!_w.Is0 || this.DesignMode);
			//if(!IsHandleCreated) CreateHandle();
			//note: auto-creating handle is not good:
			//	1. May create parked control. Not good for performance.
			//	2. Can be dangerous, eg if passing a reusable buffer that also is used when creating handle.

			return Sci_Call(_sciPtr, sciMessage, wParam, lParam);
		}

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

		internal bool ZDebugPrintMessages_ { get; set; }
#endif

		#region properties

		/// <summary>
		/// Border style.
		/// Must be set before creating control handle.
		/// </summary>
		public virtual bool ZInitBorder { get; set; }

		/// <summary>
		/// Use the default Scintilla's context menu.
		/// Must be set before creating control handle.
		/// </summary>
		public virtual bool ZInitUseDefaultContextMenu { get; set; }

		/// <summary>
		/// This control is used just to display text, not to edit.
		/// Must be set before creating control handle.
		/// </summary>
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
		public virtual ZImagesStyle ZInitImagesStyle { get; set; }

		/// <summary>
		/// See <see cref="ZInitImagesStyle"/>.
		/// </summary>
		public enum ZTagsStyle
		{
			/// <summary>Don't support tags. The <see cref="ZTags"/> property is null.</summary>
			NoTags,

			/// <summary>Let <see cref="zText"/>, zSetText and zAppendText parse tags when the text has prefix "&lt;&gt;".</summary>
			AutoWithPrefix,

			/// <summary>Let <see cref="zText"/>, zSetText and zAppendText parse tags always.</summary>
			AutoAlways,

			/// <summary>Tags are parsed only when calling Tags.AddText.</summary>
			User,
		}

		/// <summary>
		/// Whether and when supports tags.
		/// Must be set before creating control handle.
		/// </summary>
		public virtual ZTagsStyle ZInitTagsStyle { get; set; }

		/// <summary>
		/// Whether to show arrows etc to make wrapped lines more visible.
		/// Must be set before creating control handle.
		/// </summary>
		public virtual bool ZInitWrapVisuals { get; set; } = true;

		/// <summary>
		/// Word-wrap.
		/// </summary>
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
		public bool? ZAcceptsEnter { get; set; }

		/// <summary>
		/// On SCN_MODIFIED notifications suppress <see cref="ZOnSciNotify"/>, <see cref="ZNotify"/> and <see cref="ZTextChanged"/>.
		/// Use to temporarily disable 'modified' notifications. Never use SCI_SETMODEVENTMASK, because then the control would stop working correctly.
		/// </summary>
		public bool ZDisableModifiedNotifications { get; set; }

		/// <summary>
		/// Don't set focus on mouse left/right/middle button down.
		/// </summary>
		public MButtons ZNoMouseSetFocus { get; set; }

		#endregion

		#region acc

		_Accessible _acc;

		class _Accessible : HwndHostAccessibleBase_
		{
			readonly KScintilla _sci;

			internal _Accessible(KScintilla sci) : base(sci, sci.Hwnd) {
				_sci = sci;
			}

			public override AccROLE Role(int child) => _sci.ZAccessibleRole;

			public override string Name(int child) => _sci.ZAccessibleName;

			public override string Description(int child) => _sci.ZAccessibleDescription;

			public override string Value(int child) => _sci.ZAccessibleValue;

			public override AccSTATE State(int child) {
				var r = base.State(child);
				if (_sci.zIsReadonly) r |= AccSTATE.READONLY;
				return r;
			}
		}

		protected virtual AccROLE ZAccessibleRole => AccROLE.TEXT; //_sci.ZInitReadOnlyAlways ? AccROLE.STATICTEXT : AccROLE.TEXT;

		protected virtual string ZAccessibleName => Name;

		protected virtual string ZAccessibleDescription => null;

		protected virtual string ZAccessibleValue => ZInitReadOnlyAlways ? zText?.Limit(0xffff) : null;

		#endregion
	}
}
