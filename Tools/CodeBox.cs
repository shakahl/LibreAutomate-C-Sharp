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
	/// Scintilla-based control that shows colored C# code created by its parent form (a code tool dialog).
	/// </summary>
	public class CodeBox : AuScintilla
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

			//FUTURE: autosize (move splitter of parent splitcontainer).

			base.OnSciNotify(ref n);
		}

		/// <summary>
		/// Sets text and makes all or part of it readonly.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="readonlyFrom"></param>
		public void ZSetText(string s, int readonlyFrom = 0)
		{
			ST.Call(Sci.SCI_SETREADONLY, false);
			ST.SetText(s, noUndo: true, noNotif: true);
			if(readonlyFrom > 0) {
				_readonlyLenUtf8 = _LenUtf8 - _LenToUtf8(0, readonlyFrom);
			} else {
				ST.Call(Sci.SCI_SETREADONLY, true);
				_readonlyLenUtf8 = -1;
			}
		}

		public event EventHandler ZTextChanged;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ZReadonlyStart => _readonlyLenUtf8 < 0 ? 0 : _LenFromUtf8(0, _ReadonlyStartUtf8);
		int _readonlyLenUtf8;
		int _ReadonlyStartUtf8 => _readonlyLenUtf8 < 0 ? 0 : _LenUtf8 - _readonlyLenUtf8;

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
		/// Returns code to find window wnd and optionally control con in it.
		/// If wnd/con is same as previous and code of this control is valid, gets code from this code control, from the start to ZReadonlyStart.
		/// Else creates code "var w = Wnd.Find(...).OrThrow();". If wnd is invalid, creates code "Wnd w = default;".
		/// The returned wndVar is final Wnd variable name (of window or control).
		/// </summary>
		public (string code, string wndVar) ZGetWndFindCode(Wnd wnd, Wnd con = default)
		{
			string R = null, sCode = null, wndVar = "w", conVar = "c", cls = null;
			if(!wnd.Is0) {
				if(wnd == _wnd) {
					sCode = ST.RangeText(0, _ReadonlyStartUtf8);
					if(sCode.RegexMatch_(@"^(?:var|Wnd) (\w+)((?s).+\r\n(?:var|Wnd) (\w+).+$)?", out var m)) {
						bool isConCode = m[3].Exists;
						if(con == _con && !con.Is0 == isConCode) return (sCode, m[isConCode ? 3 : 1].Value);
						wndVar = m[1].Value;
						if(isConCode) sCode = sCode.Remove(m[3].Index - 6);
						if(con.Is0) { _con = default; return (sCode, wndVar); }
						if(isConCode) conVar = m[3].Value;
					} else sCode = null;
				}

				var b = new StringBuilder();
				if(sCode != null) b.Append(sCode);
				else if((cls = wnd.ClassName) != null) {
					b.Append("var w = Wnd.Find(");
					b.AppendStringArg(TUtil.EscapeWindowName(wnd.Name, true), noComma: true);
					b.AppendStringArg(TUtil.StripWndClassName(cls, true));
					string fl = null;
					if(!wnd.IsVisible) fl = "WFFlags.HiddenToo";
					if(wnd.IsCloaked) fl = fl == null ? "WFFlags.CloakedToo" : "WFFlags.HiddenToo|WFFlags.CloakedToo";
					if(fl != null) b.AppendOtherArg(fl, "flags");
					b.Append(").OrThrow();");
				} else con = default;

				if(!con.Is0) {
					bool isId = TUtil.GetUsefulControlId(con, wnd, out int id);
					if(isId || (cls = con.ClassName) != null) {
						b.AppendFormat("\r\nvar {0} = {1}.Child", conVar, wndVar);
						wndVar = conVar;
						if(isId) {
							b.AppendFormat("ById({0});", id);
						} else {
							cls = TUtil.StripWndClassName(cls, true);
							string name = con.Name, prefix = null;
							if(Empty(name)) {
								name = con.NameWinForms;
								if(!Empty(name)) prefix = "***wfName ";
								else {
									var nameAcc = con.NameAcc;
									//var nameLabel = con.NameLabel;
									if(!Empty(nameAcc)/* || !Empty(nameLabel)*/) {
										//if(Empty(nameAcc) || nameLabel == nameAcc) {
										//	name = nameLabel; prefix = "***label ";
										//} else {
										name = nameAcc; prefix = "***accName ";
										//}
									}
								}
							}
							if(Wildex.HasWildcards(name)) name = "**t " + name;
							name = prefix + name;

							b.Append("(");
							b.AppendStringArg(name, noComma: true);
							b.AppendStringArg(cls);
							if(!con.IsVisible) b.AppendOtherArg("WCFlags.HiddenToo", "flags");
							b.Append(").OrThrow();");
						}
					} else con = default;
				}

				if(b.Length != 0) R = b.ToString();
			}

			if(R == null) {
				_wnd = default; _con = default;
				return ("Wnd w = default;", "w");
			}
			_wnd = wnd; _con = con;
			return (R, wndVar);
		}
		Wnd _wnd, _con;

		/// <summary>
		/// Shows <see cref="Form_Wnd"/> and updates text.
		/// </summary>
		public (bool ok, Wnd wnd, Wnd con, bool useCon) ZShowWndTool(Wnd wnd, Wnd con, bool uncheckControl)
		{
			using(var f = new Form_Wnd(con.Is0 ? wnd : con, uncheckControl)) {
				if(f.ShowDialog(FindForm()) != DialogResult.OK) return default;
				var code = f.ResultCode;
				_wnd = f.ResultWindow;
				_con = f.ResultUseControl ? f.ResultControl : default;
				int i = _ReadonlyStartUtf8;
				var code2 = ST.RangeText(i, i + _readonlyLenUtf8);
				ST.Call(Sci.SCI_SETREADONLY, false);
				ST.SetText(code + code2);
				return (true, f.ResultWindow, f.ResultControl, f.ResultUseControl);
			}
		}
	}
}
