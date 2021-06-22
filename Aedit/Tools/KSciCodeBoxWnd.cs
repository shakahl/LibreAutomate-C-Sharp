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
//using System.Linq;

using Au.Types;
using Au.Tools;
using System.Windows;

namespace Au.Controls
{
	/// <summary>
	/// Scintilla-based control that shows colored C# code. Based on <see cref="KSciCodeBox"/> and adds methods to get code for wnd.find.
	/// </summary>
	class KSciCodeBoxWnd : KSciCodeBox
	{
		/// <summary>
		/// Returns code to find window w and optionally control con in it. Without end newline.
		/// If w/con is same as previous and code of this control is modified and valid, gets code from this code control, from the start to ZReadonlyStart.
		/// Else creates code "var w = +wnd.find(...);". If w is invalid, creates code "wnd w = default;".
		/// The returned wndVar is final wnd variable name (of window or control).
		/// </summary>
		public (string code, string wndVar) ZGetWndFindCode(wnd w, wnd con = default) {
			string R = null, sCode = null, wndVar = "w", conVar = "c";

			if (w != _wnd) _userModified = false; else if (!_userModified) _userModified = 0 != Call(Sci.SCI_GETMODIFY);
			if (!w.Is0) {
				if (_userModified && w == _wnd) {
					sCode = zRangeText(false, 0, _ReadonlyStartUtf8);
					if (sCode.RegexMatch(@"(?s)^(?:var|wnd) (\w+)", out var mw)) { //window
						bool isConCode = sCode.RegexMatch(@"(?s)\R(?:var|wnd) (\w+)", out var mc, 0, mw.End..); //control
						//print.it(isConCode);
						if (con == _con && !con.Is0 == isConCode) {
							//print.it(isConCode ? "same control" : "no control");
							return (sCode, (isConCode ? mc : mw)[1].Value);
						}
						wndVar = mw[1].Value;
						if (isConCode) sCode = sCode[..mc.Start];
						if (con.Is0) {
							//print.it("remove control");
							_con = default;
							return (sCode, wndVar);
						}
						if (isConCode) conVar = mc[1].Value;
						//print.it(isConCode ? "replace control" : "add control");
					} else sCode = null;
				}

				var f = new TUtil.WindowFindCodeFormatter {
					Throw = true,
					VarWindow = wndVar,
					VarControl = conVar,
				};

				if (sCode != null) {
					f.CodeBefore = sCode;
					f.NeedWindow = false;
				} else if (w.ClassName is string cls) {
					f.nameW = TUtil.EscapeWindowName(w.NameTL_, true);
					f.classW = TUtil.StripWndClassName(cls, true);
					if (f.nameW.NE()) f.programW = w.ProgramName;
					f.hiddenTooW = !w.IsVisible;
					f.cloakedTooW = w.IsCloaked;
				} else {
					con = default;
					f.NeedWindow = false;
				}

				if (!con.Is0) {
					bool isId = TUtil.GetUsefulControlId(con, w, out int id);
					string cls = con.ClassName;
					if (isId || cls != null) {
						f.NeedControl = true;
						wndVar = conVar;
						if (isId) {
							f.idC = id.ToS();
							f.classC_comments = cls;
							f.nameC_comments = con.Name;
						} else {
							f.classC = TUtil.StripWndClassName(cls, true);
							string name = con.Name, prefix = null;
							if (name.NE()) {
								name = con.NameWinforms;
								if (!name.NE()) prefix = "***wfName ";
								else {
									var nameElm = con.NameElm;
									//var nameLabel = con.NameLabel;
									if (!nameElm.NE()/* || !nameLabel.NE()*/) {
										//if(nameAcc.NE() || nameLabel == nameAcc) {
										//	name = nameLabel; prefix = "***label ";
										//} else {
										name = nameElm; prefix = "***elmName ";
										//}
									}
								}
							}
							if (wildex.hasWildcardChars(name)) name = "**t " + name;

							f.nameC = prefix + name;
							f.hiddenTooC = !con.IsVisible;
						}
					} else con = default;
				}

				R = f.Format();
			}

			if (R == null) {
				_wnd = default; _con = default;
				return ("wnd w = default;", "w");
			}
			_wnd = w; _con = con;
			return (R, wndVar);
		}
		wnd _wnd, _con;
		bool _userModified;

		//rejected. Better don't update changed window name than overwrite user-edited code.
		///// <summary>
		///// Forget window and control handles. Then <see cref="ZGetWndFindCode"/> will format new code even if the window is the same as previously.
		///// </summary>
		//public void ZResetWndCon()
		//{
		//	_wnd = _con = default;
		//}

		/// <summary>
		/// Shows <see cref="Dwnd"/> and updates text.
		/// </summary>
		public (bool ok, wnd w, wnd con, bool useCon) ZShowWndTool(Window owner, wnd w, wnd con, bool uncheckControl) {
			var d = new Dwnd(con.Is0 ? w : con, uncheckControl) { ZDontInsertCodeOnOK = true };
			d.ShowAndWait(owner, hideOwner: true);
			var code = d.ZResultCode; if (code == null) return default;
			_wnd = d.ZResultWindow;
			_con = d.ZResultUseControl ? d.ZResultControl : default;
			int i = _ReadonlyStartUtf8;
			var code2 = zRangeText(false, i, i + _readonlyLenUtf8);
			zIsReadonly = false;
			zSetText(code + code2);
			return (true, d.ZResultWindow, d.ZResultControl, d.ZResultUseControl);
		}
	}
}
