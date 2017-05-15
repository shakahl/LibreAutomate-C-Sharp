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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;

namespace G.Controls
{
	using static Sci;

	/// <summary>
	/// This .NET control wraps native Scintilla control.
	/// This is not an universal Scintilla wrapper class. Designed just for purposes of this library and related software.
	/// Responsible for creating and initializing the control. Also used to set/change control properties.
	/// The ST property returns a Catilla object that can be used to work with text, code styling etc.
	/// </summary>
	/// <remarks>
	/// Why don't use ScintillaNET:
	/// 1. Delays to update for the newest Scintilla version.
	/// 2. Possibly will be abandoned some day.
	/// 3. Adds yet another layer of bugs, and I found one.
	/// 4. I don't like some things how it is implemented. Eg aggressively uses "clamping", creates much garbage, eg new Line object for each line-related Scintilla message call.
	/// 5. For me it does not make much easier because I used Scintilla in QM2 (C++) and am familiar with its API (which is well documented). When using ScintillaNET, I often search its source code just to find which function calls the API I need. Now I can simply convert much of QM2 code to C#.
	/// 6. I use modified Scintilla. Would need to synchronize some modifications with ScintillaNET. For example I use a callback function instead of WM_NOTIFY/WM_REFLECT.
	/// </remarks>
	public class SciControl :Control
	{
		static SciFnDirect s_fnDirect;
		SciFnDirect _fnDirect;
		LPARAM _ptrDirect;
		Sci_NotifyCallback _notifyCallback;

		public SciImages Images { get; private set; }
		public SciTags Tags { get; private set; }

		/// <summary>
		/// Gets the Catilla object that contains most Scintilla-related functions.
		/// </summary>
		public SciText ST { get; internal set; }

		///
		public SciControl()
		{
			ST = new SciText(this);

			base.SetStyle(ControlStyles.CacheText, true);

			//this is like TextBoxBase and ScintillaNET do
			base.SetStyle(ControlStyles.StandardClick |
					 ControlStyles.StandardDoubleClick |
					 ControlStyles.UseTextForAccessibility |
					 ControlStyles.UserPaint,
					 false);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				if(s_fnDirect == null) {
					var path = SciLexerDllPath;
					if(!Api.GetDelegate(out s_fnDirect, path, "Scintilla_DirectFunction")) throw new CatException(0, $"*load '{path}'");
				}
				_fnDirect = s_fnDirect;

				var cp = base.CreateParams;
				cp.ClassName = "Scintilla";

				//this is like TextBoxBase and ScintillaNET do
				cp.ExStyle &= (~(int)Native.WS_EX_CLIENTEDGE);
				cp.Style &= (~(int)Native.WS_BORDER);
				switch(InitBorderStyle) {
				case BorderStyle.Fixed3D: cp.ExStyle |= (int)Native.WS_EX_CLIENTEDGE; break;
				case BorderStyle.FixedSingle: cp.Style |= (int)Native.WS_BORDER; break;
				}

				return cp;
			}
		}

		protected unsafe override void OnHandleCreated(EventArgs e)
		{
			_ptrDirect = ((Wnd)Handle).Send(SCI_GETDIRECTPOINTER);
			Call(SCI_SETNOTIFYCALLBACK, 0, Marshal.GetFunctionPointerForDelegate(_notifyCallback = _NotifyCallback));

			bool hasImages = InitImagesStyle != ImagesStyle.NoImages;
			bool hasTags = InitTagsStyle != TagsStyle.NoTags;
			if(InitReadOnlyAlways) {
				MOD mask = 0;
				if(hasImages || hasTags) mask |= MOD.SC_MOD_INSERTTEXT | MOD.SC_MOD_DELETETEXT;
				Call(SCI_SETMODEVENTMASK, (int)mask);
			}
			_InitDocument();
			Call(SCI_SETSCROLLWIDTHTRACKING, 1);
			Call(SCI_SETSCROLLWIDTH, 100);
			if(!InitUseDefaultContextMenu) Call(SCI_USEPOPUP);
			int cw = SystemInformation.CaretWidth; if(cw > 1) Call(SCI_SETCARETWIDTH, cw);
			Call(SCI_SETWRAPVISUALFLAGS, SC_WRAPVISUALFLAG_START | SC_WRAPVISUALFLAG_END);
			Call(SCI_SETWRAPVISUALFLAGSLOCATION, SC_WRAPVISUALFLAGLOC_END_BY_TEXT);
			Call(SCI_SETWRAPINDENTMODE, SC_WRAPINDENT_INDENT);
			if(WrapLines) {
				Call(SCI_SETWRAPMODE, SC_WRAP_WORD);
			}

			//note: cannot set styles here, because later parent will call StyleClearAll. It sets some special styles.

			if(hasImages) Images = new SciImages(this, InitImagesStyle == ImagesStyle.AnyString);
			if(hasTags) Tags = new SciTags(this);

			base.OnHandleCreated(e);
		}

		void _InitDocument()
		{
			//these must be set for each document of this Scintilla window

			Call(SCI_SETCODEPAGE, Api.CP_UTF8);
			Call(SCI_SETTABWIDTH, 4);
			if(InitReadOnlyAlways) {
				Call(SCI_SETREADONLY, 1);
				Call(SCI_SETUNDOCOLLECTION);
				Call(SCI_SETLEXER, (int)LexLanguage.SCLEX_NULL); //don't need style-needed notification, we'll set styles for whole text
			}
		}

		protected override void WndProc(ref Message m)
		{
			//if(this.Parent?.Name == "Output") Wnd.Misc.PrintMsg(ref m, Api.WM_TIMER, Api.WM_MOUSEMOVE, Api.WM_SETCURSOR, Api.WM_NCHITTEST, Api.WM_PAINT, Api.WM_IME_SETCONTEXT, Api.WM_IME_NOTIFY);

			uint msg = (uint)m.Msg; LPARAM wParam = m.WParam, lParam = m.LParam;

			switch(msg) {
			case Api.WM_SETCURSOR:
			case Api.WM_SETFOCUS:
			case Api.WM_KILLFOCUS:
			case Api.WM_LBUTTONUP:
			case Api.WM_LBUTTONDBLCLK:
				_DefWndProc(ref m);
				return;

			//case Api.WM_DESTROY:
			//	break;

			case Api.WM_LBUTTONDOWN:
				if(Api.GetFocus() != (Wnd)Handle) {
					bool setFocus = true;
					Tags?.LibOnLButtonDownWhenNotFocused(ref m, ref setFocus); //Tags may not want to set focus eg when a hotspot clicked
					if(setFocus) Api.SetFocus((Wnd)Handle);
				}

				_DefWndProc(ref m);
				return;
			}

			base.WndProc(ref m);
		}

		void _DefWndProc(ref Message m)
		{
			m.Result = Call(m.Msg, m.WParam, m.LParam);
			//This is faster, why to go through CallWindowProc.
			//Howewer cannot override DefWndProc with this. Then crashes.
		}

		unsafe void _NotifyCallback(void* cbParam, ref SCNotification n)
		{
			var code = n.nmhdr.code;
			if(code == NOTIF.SCN_PAINTED) return;
			//PrintList(code);
			switch(code) {
			case NOTIF.SCN_MODIFIED:
				_NotifyModified(ref n);
				break;
			case NOTIF.SCN_HOTSPOTRELEASECLICK:
				Tags?.LibOnLinkClick(n.position, 0 != (n.modifiers & SCMOD_CTRL));
				break;
			}
		}

		unsafe void _NotifyModified(ref SCNotification n)
		{
			var code = n.modificationType;
			if((code & (MOD.SC_MULTISTEPUNDOREDO | MOD.SC_LASTSTEPINUNDOREDO)) == MOD.SC_MULTISTEPUNDOREDO) return;
			//PrintList(code, n.position);
			if(0 != (code & (MOD.SC_MOD_INSERTTEXT | MOD.SC_MOD_DELETETEXT))) {
				bool ins = 0 != (code & MOD.SC_MOD_INSERTTEXT);
				Images?.LibOnTextChanged(ins, ref n);
				Tags?.LibOnTextChanged(ins, ref n);
			}
			//if(0!=(code& MOD.SC_MOD_CHANGEANNOTATION)) ChangedAnnotation?.Invoke(this, ref n);
		}

		//public delegate void SciEventHandler(SciControl c, ref SCNotification n);

		//public event SciEventHandler ChangedAnnotation;

		//#if DEBUG
		//		public bool Deb { get; set; }
		//#endif

		/// <summary>
		/// Calls a Scintilla message to the control.
		/// Don't call this function from another thread.
		/// </summary>
		public LPARAM Call(int sciMessage, LPARAM wParam, LPARAM lParam)
		{
			if(!IsHandleCreated) {
				Debug.Assert(!Visible);
				CreateHandle(); //does not create handle if initially Visible is false
			}
			return _fnDirect(_ptrDirect, sciMessage, wParam, lParam);
		}

		/// <summary>
		/// Calls a Scintilla message.
		/// Don't call this function from another thread.
		/// </summary>
		public LPARAM Call(int sciMessage, LPARAM wParam)
		{
			return Call(sciMessage, wParam, default(LPARAM));
		}

		/// <summary>
		/// Calls a Scintilla message.
		/// Don't call this function from another thread.
		/// </summary>
		public LPARAM Call(int sciMessage)
		{
			return Call(sciMessage, default(LPARAM), default(LPARAM));
		}

		/// <summary>
		/// Scintilla dll path.
		/// Default is Folders.ThisApp + "SciLexer" + (Ver.Is64BitProcess ? "64" : "32") + ".dll". If you want to change it, call this before creating first control.
		/// </summary>
		public static string SciLexerDllPath
		{
			get
			{
				if(s_dllPath == null) {
					s_dllPath = Folders.ThisApp + "SciLexer" + (Ver.Is64BitProcess ? "64" : "32") + ".dll";
				}
				return s_dllPath;
			}
			set
			{
				s_dllPath = Path_.Normalize(value);
			}
		}
		static string s_dllPath;

		/// <summary>
		/// Border style.
		/// Must be set before creating control handle.
		/// </summary>
		public BorderStyle InitBorderStyle { get; set; }

		/// <summary>
		/// Use the default Scintilla's context menu.
		/// Must be set before creating control handle.
		/// </summary>
		public bool InitUseDefaultContextMenu { get; set; }

		/// <summary>
		/// This control is used just to display text, not to edit.
		/// Must be set before creating control handle.
		/// </summary>
		public bool InitReadOnlyAlways { get; set; }

		/// <summary>
		/// See <see cref="InitImagesStyle"/>.
		/// </summary>
		public enum ImagesStyle
		{
			/// <summary>Don't show images. The Images property is null.</summary>
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
		public ImagesStyle InitImagesStyle { get; set; }

		/// <summary>
		/// See <see cref="InitImagesStyle"/>.
		/// </summary>
		public enum TagsStyle
		{
			/// <summary>Don't support tags. The Images property is null.</summary>
			NoTags,
			/// <summary>Let Text, ST.SetText and ST.AppendText parse tags when the text has prefix "&lt;&gt;".</summary>
			AutoWithPrefix,
			/// <summary>Let Text, ST.SetText and ST.AppendText parse tags always.</summary>
			AutoAlways,
			/// <summary>Tags are parsed only when calling Tags.AddText.</summary>
			User,
		}

		/// <summary>
		/// Whether and when supports tags.
		/// Must be set before creating control handle.
		/// </summary>
		public TagsStyle InitTagsStyle { get; set; }

		/// <summary>
		/// Word-wrap.
		/// </summary>
		public bool WrapLines
		{
			get => _wrapLines;
			set
			{
				if(value != _wrapLines) {
					_wrapLines = value;
					if(IsHandleCreated) Call(SCI_SETWRAPMODE, value ? SC_WRAP_WORD : SC_WRAP_NONE);
				}
			}
		}
		bool _wrapLines;

		/// <summary>
		/// Gets or sets all text.
		/// </summary>
		public override string Text { get => ST.GetText(); set => ST.SetText(value); }
		//info: this class should not contain text functions, but need to override this
	}
}
