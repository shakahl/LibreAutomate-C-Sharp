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
using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au.Types;
using Au.Controls;

namespace Au.Tools
{
	/// <summary>
	/// Scintilla-based control that shows colored C# code created by its parent form (a code tool dialog).
	/// Also can be used anywhere to edit partially styled C# code. To make editable and set text use <see cref="ZSetText"/> with readonlyFrom=-1.
	/// </summary>
	class CodeBoxF : AuScintilla
	{
		public CodeBoxF()
		{
			ZInitUseDefaultContextMenu = true;
			ZInitBorderStyle = BorderStyle.FixedSingle;
		}

		#region default property values for VS form designer

		[DefaultValue(true)]
		public override bool ZInitUseDefaultContextMenu { get => base.ZInitUseDefaultContextMenu; set => base.ZInitUseDefaultContextMenu = value; }

		[DefaultValue(BorderStyle.FixedSingle)]
		public override BorderStyle ZInitBorderStyle { get => base.ZInitBorderStyle; set => base.ZInitBorderStyle = value; }

		#endregion

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e); //note: must be first

			Z.MarginWidth(1, 0);
			//Z.StyleFont(Sci.STYLE_DEFAULT, Font);
			Z.SetLexerCpp();
			Z.Call(Sci.SCI_SETREADONLY, true);
		}

		protected override void ZOnSciNotify(ref Sci.SCNotification n)
		{
			//switch(n.nmhdr.code) {
			//case Sci.NOTIF.SCN_PAINTED: case Sci.NOTIF.SCN_UPDATEUI: break;
			//default: AOutput.Write(n.nmhdr.code, n.modificationType); break;
			//}

			switch(n.nmhdr.code) {
			case Sci.NOTIF.SCN_UPDATEUI:
				//make text after _ReadonlyStartUtf8 readonly
				if(0 != (n.updated & Sci.SC_UPDATE_SELECTION)) { //selection changed
					if(_readonlyLenUtf8 > 0) {
						int i = Call(Sci.SCI_GETSELECTIONEND);
						Z.Call(Sci.SCI_SETREADONLY, i > _ReadonlyStartUtf8 || _LenUtf8 == 0); //small bug: if caret is at the boundary, allows to delete readonly text, etc.
					}
				}
				break;
			}

			//FUTURE: autosize (move splitter of parent splitcontainer).

			base.ZOnSciNotify(ref n);
		}

		/// <summary>
		/// Sets text and makes all or part of it readonly.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="readonlyFrom">If 0, makes all text readonly. If s.Length or -1, makes all text editable. If between 0 and s.Length, makes readonly from this position.</param>
		public void ZSetText(string s, int readonlyFrom = 0)
		{
			Z.Call(Sci.SCI_SETREADONLY, false);
			Z.SetText(s, SciSetTextFlags.NoUndoNoNotify);
			if(readonlyFrom > 0) {
				_readonlyLenUtf8 = _LenUtf8 - Pos8(readonlyFrom);
			} else if(readonlyFrom < 0) {
				_readonlyLenUtf8 = 0; 
			} else {
				Z.Call(Sci.SCI_SETREADONLY, true);
				_readonlyLenUtf8 = -1;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ZReadonlyStart => _readonlyLenUtf8 < 0 ? 0 : Pos16(_ReadonlyStartUtf8); //currently not used

		int _readonlyLenUtf8;

		int _ReadonlyStartUtf8 => _readonlyLenUtf8 < 0 ? 0 : _LenUtf8 - _readonlyLenUtf8;

		int _LenUtf8 => Call(Sci.SCI_GETTEXTLENGTH);

		protected override bool IsInputKey(Keys keyData)
		{
			switch(keyData & Keys.KeyCode) {
			case Keys.Tab: case Keys.Escape: return false;
				//case Keys.Enter: return 0 == Z.Call(Sci.SCI_GETREADONLY);
			}
			return base.IsInputKey(keyData);
		}

		/// <summary>
		/// Returns code to find window wnd and optionally control con in it.
		/// If wnd/con is same as previous and code of this control is valid, gets code from this code control, from the start to ZReadonlyStart.
		/// Else creates code "var w = +AWnd.Find(...);". If wnd is invalid, creates code "AWnd w = default;".
		/// The returned wndVar is final AWnd variable name (of window or control).
		/// </summary>
		public (string code, string wndVar) ZGetWndFindCode(AWnd wnd, AWnd con = default)
		{
			string R = null, sCode = null, wndVar = "w", conVar = "c", cls = null;
			if(!wnd.Is0) {
				if(wnd == _wnd) {
					sCode = Z.RangeText(false, 0, _ReadonlyStartUtf8);
					if(sCode.RegexMatch(@"^(?:var|AWnd) (\w+)((?s).+\R(?:var|AWnd) (\w+).+$)?", out var m)) {
						bool isConCode = m[3].Exists;
						if(con == _con && !con.Is0 == isConCode) return (sCode, m[isConCode ? 3 : 1].Value);
						wndVar = m[1].Value;
						if(isConCode) sCode = sCode.Remove(m[3].Start - 6);
						if(con.Is0) { _con = default; return (sCode, wndVar); }
						if(isConCode) conVar = m[3].Value;
					} else sCode = null;
				}

				var b = new StringBuilder();
				if(sCode != null) b.Append(sCode);
				else if((cls = wnd.ClassName) != null) {
					b.Append("var w = +AWnd.Find(");
					b.AppendStringArg(TUtil.EscapeWindowName(wnd.NameTL_, true), noComma: true);
					b.AppendStringArg(TUtil.StripWndClassName(cls, true));
					string fl = null;
					if(!wnd.IsVisible) fl = "WFlags.HiddenToo";
					if(wnd.IsCloaked) fl = fl == null ? "WFlags.CloakedToo" : "WFlags.HiddenToo|WFlags.CloakedToo";
					if(fl != null) b.AppendOtherArg(fl, "flags");
					b.Append(");");
				} else con = default;

				if(!con.Is0) {
					bool isId = TUtil.GetUsefulControlId(con, wnd, out int id);
					if(isId || (cls = con.ClassName) != null) {
						b.AppendFormat("\r\nvar {0} = +{1}.Child", conVar, wndVar);
						wndVar = conVar;
						if(isId) {
							b.AppendFormat("ById({0});", id);
						} else {
							cls = TUtil.StripWndClassName(cls, true);
							string name = con.Name, prefix = null;
							if(name.NE()) {
								name = con.NameWinforms;
								if(!name.NE()) prefix = "***wfName ";
								else {
									var nameAcc = con.NameAcc;
									//var nameLabel = con.NameLabel;
									if(!nameAcc.NE()/* || !nameLabel.NE()*/) {
										//if(nameAcc.NE() || nameLabel == nameAcc) {
										//	name = nameLabel; prefix = "***label ";
										//} else {
										name = nameAcc; prefix = "***accName ";
										//}
									}
								}
							}
							if(AWildex.HasWildcardChars(name)) name = "**t " + name;
							name = prefix + name;

							b.Append("(");
							b.AppendStringArg(name, noComma: true);
							b.AppendStringArg(cls);
							if(!con.IsVisible) b.AppendOtherArg("WCFlags.HiddenToo", "flags");
							b.Append(");");
						}
					} else con = default;
				}

				if(b.Length != 0) R = b.ToString();
			}

			if(R == null) {
				_wnd = default; _con = default;
				return ("AWnd w = default;", "w");
			}
			_wnd = wnd; _con = con;
			return (R, wndVar);
		}
		AWnd _wnd, _con;

		//rejected. Better don't update changed window name than overwrite user-edited code.
		///// <summary>
		///// Forget window and control handles. Then <see cref="ZGetWndFindCode"/> will format new code even if the window is the same as previously.
		///// </summary>
		//public void ZResetWndCon()
		//{
		//	_wnd = _con = default;
		//}

		/// <summary>
		/// Shows <see cref="FormAWnd"/> and updates text.
		/// </summary>
		public (bool ok, AWnd wnd, AWnd con, bool useCon) ZShowWndTool(AWnd wnd, AWnd con, bool uncheckControl)
		{
			using var f = new FormAWnd(con.Is0 ? wnd : con, uncheckControl);
			if(f.ShowDialog(FindForm()) != DialogResult.OK) return default;
			var code = f.ZResultCode;
			_wnd = f.ZResultWindow;
			_con = f.ZResultUseControl ? f.ZResultControl : default;
			int i = _ReadonlyStartUtf8;
			var code2 = Z.RangeText(false, i, i + _readonlyLenUtf8);
			Z.Call(Sci.SCI_SETREADONLY, false);
			Z.SetText(code + code2);
			return (true, f.ZResultWindow, f.ZResultControl, f.ZResultUseControl);
		}
	}
}
