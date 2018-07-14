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
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Controls;

namespace Au.Tools
{
	/// <summary>
	/// Scintilla-based control to show code created by its parent form (a code tool dialog).
	/// </summary>
	internal class CodeBox :AuScintilla
	{
		public CodeBox()
		{
			InitUseDefaultContextMenu = true;
			InitBorderStyle = BorderStyle.FixedSingle;
		}

		#region default property values for VS form designer

		[DefaultValue(true)]
		public override bool InitUseDefaultContextMenu { get => base.InitUseDefaultContextMenu; set => base.InitUseDefaultContextMenu = value; }

		[DefaultValue(BorderStyle.FixedSingle)]
		public override BorderStyle InitBorderStyle { get => base.InitBorderStyle; set => base.InitBorderStyle = value; }

		#endregion

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e); //note: must be first

			ST.MarginWidth(1, 0);
			//ST.StyleFont(Sci.STYLE_DEFAULT, Font);
			ST.SetLexerCpp();
			ST.Call(Sci.SCI_SETREADONLY, true);
		}

		protected override void OnSciNotify(ref Sci.SCNotification n)
		{
			if(!_noNotify) {
				//switch(n.nmhdr.code) {
				//case Sci.NOTIF.SCN_PAINTED: case Sci.NOTIF.SCN_UPDATEUI: break;
				//default: Print(n.nmhdr.code, n.modificationType); break;
				//}

				switch(n.nmhdr.code) {
				case Sci.NOTIF.SCN_MODIFIED:
					//Print(n.modificationType);
					if(n.modificationType.HasAny_(Sci.MOD.SC_MOD_INSERTTEXT | Sci.MOD.SC_MOD_DELETETEXT)) ZTextChanged?.Invoke(this, null);
					break;
				case Sci.NOTIF.SCN_UPDATEUI:
					//make text after _ReadonlyStartUtf8 readonly
					if(0 != (n.updated & Sci.SC_UPDATE_SELECTION)) { //selection changed
						if(_readonlyLenUtf8 > 0) {
							int i = Call(Sci.SCI_GETSELECTIONEND);
							ST.Call(Sci.SCI_SETREADONLY, i > _ReadonlyStartUtf8 || _LenUtf8 == 0);
						}
					}
					break;
				}
			}

			base.OnSciNotify(ref n);
		}
		bool _noNotify;

		public void ZSetText(string s, int readonlyFrom = 0)
		{
			ST.Call(Sci.SCI_SETREADONLY, false);
			_noNotify = true;
			base.Text = s;
			Call(Sci.SCI_EMPTYUNDOBUFFER);
			_noNotify = false;
			if(readonlyFrom > 0) {
				_readonlyLenUtf8 = _LenUtf8 - _LenToUtf8(0, readonlyFrom);
			} else {
				ST.Call(Sci.SCI_SETREADONLY, true);
				_readonlyLenUtf8 = -1;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get => base.Text;
			set => ZSetText(value);
		}

		public event EventHandler ZTextChanged;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ZReadonlyStart => _readonlyLenUtf8 < 0 ? 0 : _LenFromUtf8(0, _ReadonlyStartUtf8);
		int _readonlyLenUtf8;
		int _ReadonlyStartUtf8 => _LenUtf8 - _readonlyLenUtf8;

		int _LenUtf8 => Call(Sci.SCI_GETLENGTH);
		int _LenFromUtf8(int start, int end) => Call(Sci.SCI_COUNTCHARACTERS, start, end);
		int _LenToUtf8(int start, int end) => Call(Sci.SCI_POSITIONRELATIVE, start, end - start);

		protected override bool IsInputKey(Keys keyData)
		{
			switch(keyData & Keys.KeyCode) {
			case Keys.Tab: case Keys.Escape: return false;
				//case Keys.Enter: return 0 == ST.Call(Sci.SCI_GETREADONLY);
			}
			return base.IsInputKey(keyData);
		}

		/// <summary>
		/// Returns code to find window w.
		/// If newWindow==true, gets code from this control, from the start to ZReadonlyStart.
		/// Else creates code "var w = Wnd.Find(...).OrThrow();". If w is invalid, returns "var w = Wnd.WndActive;".
		/// </summary>
		public (string code, string wndVar) FormatWndFindCode(Wnd w, bool newWindow)
		{
			if(!newWindow) {
				var sCode = base.Text;
				if(!sCode.RegexMatch_(@"^(?:var|Wnd) (\w+)", 1, out var wndVar)) wndVar = "w";
				return (sCode.Remove(ZReadonlyStart), wndVar);
			}

			string R = null;
			if(!w.Is0) {
				var b = new StringBuilder("var w = Wnd.Find(");

				var s = TUtil.EscapeWindowName(w.Name, true);
				b.AppendStringArg(s, noComma: true);

				s = w.ClassName;
				if(s != null) {
					b.AppendStringArg(TUtil.StripWndClassName(s, true));

					if(!w.IsVisibleEx) b.AppendOtherArg("WFFlags.HiddenToo", "flags");

					b.Append(").OrThrow();");
					R = b.ToString();
				}
			}
			return (R ?? "var w = Wnd.WndActive;", "w");
		}
	}
}
