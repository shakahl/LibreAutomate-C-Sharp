using System;
using System.Collections.Generic;
using Au.Types;
using Au.Util;

namespace Au.Tools
{
	static class QuickCapture
	{
		static ARegisteredHotkey _rk;

		//public static void Dialog() {

		//}

		public static void RegisterHotkey(AnyWnd w) {
			_rk.Register((int)ERegisteredHotkeyId.QuickCapture, "Ctrl+Shift+Win+W", w);
		}

		public static void WmHotkey() {
			var p = AMouse.XY;
			AWnd w0 = AWnd.FromXY(p), w = w0.Window, c = w == w0 ? default : w0;
			//int color = 0; using (var dc = new ScreenDC_()) { color = Api.GetPixel(dc, p.x, p.y); }
			string path = AWnd.More.GetWindowsStoreAppId(w, true, true);

			var m = new AMenu();
			m["Find window"] = o => _Insert(_AWnd_Find(w, default, false));
			m["Find control"] = o => _Insert(_AWnd_Find(w, c, false));
			m.Last.IsDisabled = c.Is0;
			m["Wait for window"] = o => _Insert(_AWnd_Find(w, c, true));
			m["Activate window"] = o => _Insert(_AWnd_Find(w, default, false) + "\r\nw.Activate();");
			using (m.Submenu("Click")) {
				m["Window"] = o => _Insert(_AWnd_Find(w, default, false) + _Click(w, "w"));
				m["Control"] = o => _Insert(_AWnd_Find(w, c, false) + _Click(c, "c"));
				m.Last.IsDisabled = c.Is0;
				m["Screen"] = o => _Insert($"AMouse.Click({p.x}, {p.y});");
				string _Click(AWnd w, string v) {
					w.MapScreenToClient(ref p);
					return $"\r\nAMouse.Click({v}, {p.x}, {p.y});";
				}
			}
			//	using(m.Submenu("Get color")) {
			//		m["Window"] = o=> {
			//			
			//		};
			//		m["Control"] = o=> {
			//			
			//		};
			//		m.Last.IsDisabled=c.Is0;
			//		m["Screen"] = o=> {
			//			
			//		};
			//	}
			if (path != null)
				using (m.Submenu("Program path")) {
					m["var s = path;"] = o => _Path(0);
					m["AFile.Run(path);"] = o => _Path(1);
					m["t[name] = o => AFile.Run(path);"] = o => _Path(2);
					void _Path(int what) {
						if (path.Ends(@"\explorer.exe") && w.ClassNameIs("CabinetWClass")) { //if folder window, try to get folder path
							var tb = w.Child("***id 1001", "ToolbarWindow32"); // @"Address: C:\Program Files (x86)\Windows Kits\10\bin\x86"
							if (!tb.Is0 && tb.Name is string sa && sa.RegexMatch(@"^\S+: +(.+)", 1, out RXGroup rg) && AFile.ExistsAsDirectory(sa = rg.Value, useRawPath: true)) path = sa;
						}
						var g = new TUtil.PathInfo(path);
						int f = g.SelectFormatUI(); if (f == 0) return;
						var r = g.GetResult(f);
						var s = _Str(r.path);
						if (what == 2 && path.Starts("shell:")) r.name = w.Name;
						_Insert(what switch { 1 => $"AFile.Run({s});", 2 => $"t[{_Str(r.name)}] = o => AFile.Run({s});", _ => $"var s = {s};" });
					}
				}
			m.Separator();
			m["AWnd dialog"] = o => new DAWnd(w0).Show();
			m["AAcc dialog"] = o => new DAAcc(AAcc.FromXY(p, AXYFlags.NoThrow | AXYFlags.PreferLink)).Show();
			m.Separator();
			m.Add("Cancel");

			m.Show();
		}

		static string _Str(string s) {
			if (s == null) return "null";
			if (!TUtil.MakeVerbatim(ref s)) s = s.Escape(quote: true);
			return s;
		}

		static string _AWnd_Find(AWnd w, AWnd c, bool wait) {
			string wName = w.Name;
			var f = new TUtil.WindowFindCodeFormatter {
				Throw = true,
				NeedWindow = true,
				NeedControl = !c.Is0,
				nameW = TUtil.EscapeWindowName(wName, true),
				classW = TUtil.StripWndClassName(w.ClassName, true),
				programW = wName.NE() ? w.ProgramName : null,
				waitW = wait ? "30" : null,
			};
			if (!c.Is0) {
				string cName = null, cClass = TUtil.StripWndClassName(c.ClassName, true);
				_ = _ConName("", c.Name) || _ConName("***wfName ", c.NameWinforms) || _ConName("***accName ", c.NameAcc);

				bool _ConName(string prefix, string value) {
					if (value.NE()) return false;
					cName = prefix + TUtil.EscapeWildex(value);
					return true;
				}

				if (TUtil.GetUsefulControlId(c, w, out int id)) {
					f.idC = id.ToStringInvariant();
					f.nameC_comments = cName;
					f.classC_comments = cClass;
				} else {
					f.nameC = cName;
					f.classC = cClass;
				}
			}
			return f.Format();
		}

		static void _Insert(string s) {
			//AOutput.Write(s);
			InsertCode.Statements(s);
		}
	}
}
