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
	/// Scintilla-based control that shows colored C# code. Based on <see cref="KSciCodeBox"/> and adds methods to get code for AWnd.Find.
	/// </summary>
	class KSciCodeBoxAWnd : KSciCodeBox
	{
		/// <summary>
		/// Returns code to find window wnd and optionally control con in it. Without end newline.
		/// If wnd/con is same as previous and code of this control is modified and valid, gets code from this code control, from the start to ZReadonlyStart.
		/// Else creates code "var w = +AWnd.Find(...);". If wnd is invalid, creates code "AWnd w = default;".
		/// The returned wndVar is final AWnd variable name (of window or control).
		/// </summary>
		public (string code, string wndVar) ZGetWndFindCode(AWnd wnd, AWnd con = default) {
			string R = null, sCode = null, wndVar = "w", conVar = "c";

			if (wnd != _wnd) _userModified = false; else if (!_userModified) _userModified = 0 != Call(Sci.SCI_GETMODIFY);
			if (!wnd.Is0) {
				if (_userModified && wnd == _wnd) {
					sCode = zRangeText(false, 0, _ReadonlyStartUtf8);
					if (sCode.RegexMatch(@"(?s)^(?:var|AWnd) (\w+)", out var mw)) { //window
						bool isConCode = sCode.RegexMatch(@"(?s)\R(?:var|AWnd) (\w+)", out var mc, 0, mw.End..); //control
						//AOutput.Write(isConCode);
						if (con == _con && !con.Is0 == isConCode) {
							//AOutput.Write(isConCode ? "same control" : "no control");
							return (sCode, (isConCode ? mc : mw)[1].Value);
						}
						wndVar = mw[1].Value;
						if (isConCode) sCode = sCode[..mc.Start];
						if (con.Is0) {
							//AOutput.Write("remove control");
							_con = default;
							return (sCode, wndVar);
						}
						if (isConCode) conVar = mc[1].Value;
						//AOutput.Write(isConCode ? "replace control" : "add control");
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
				} else if (wnd.ClassName is string cls) {
					f.nameW = TUtil.EscapeWindowName(wnd.NameTL_, true);
					f.classW = TUtil.StripWndClassName(cls, true);
					if (f.nameW.NE()) f.programW = wnd.ProgramName;
					f.hiddenTooW = !wnd.IsVisible;
					f.cloakedTooW = wnd.IsCloaked;
				} else {
					con = default;
					f.NeedWindow = false;
				}

				if (!con.Is0) {
					bool isId = TUtil.GetUsefulControlId(con, wnd, out int id);
					string cls = con.ClassName;
					if (isId || cls != null) {
						f.NeedControl = true;
						wndVar = conVar;
						if (isId) {
							f.idC = id.ToStringInvariant();
							f.classC_comments = cls;
							f.nameC_comments = con.Name;
						} else {
							f.classC = TUtil.StripWndClassName(cls, true);
							string name = con.Name, prefix = null;
							if (name.NE()) {
								name = con.NameWinforms;
								if (!name.NE()) prefix = "***wfName ";
								else {
									var nameAcc = con.NameAcc;
									//var nameLabel = con.NameLabel;
									if (!nameAcc.NE()/* || !nameLabel.NE()*/) {
										//if(nameAcc.NE() || nameLabel == nameAcc) {
										//	name = nameLabel; prefix = "***label ";
										//} else {
										name = nameAcc; prefix = "***accName ";
										//}
									}
								}
							}
							if (AWildex.HasWildcardChars(name)) name = "**t " + name;

							f.nameC = prefix + name;
							f.hiddenTooC = !con.IsVisible;
						}
					} else con = default;
				}

				R = f.Format();
			}

			if (R == null) {
				_wnd = default; _con = default;
				return ("AWnd w = default;", "w");
			}
			_wnd = wnd; _con = con;
			return (R, wndVar);
		}
		AWnd _wnd, _con;
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
		/// Shows <see cref="DAWnd"/> and updates text.
		/// </summary>
		public (bool ok, AWnd wnd, AWnd con, bool useCon) ZShowWndTool(Window owner, AWnd wnd, AWnd con, bool uncheckControl) {
			var d = new DAWnd(con.Is0 ? wnd : con, uncheckControl) { ZDontInsertCodeOnOK = true };
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
