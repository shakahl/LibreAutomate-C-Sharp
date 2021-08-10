namespace Au.Tools
{
	static class QuickCapture
	{
		static keys.more.Hotkey _rk;

		//public static void Dialog() {

		//}

		public static void RegisterHotkey(AnyWnd w) {
			_rk.Register((int)ERegisteredHotkeyId.QuickCapture, "Ctrl+Shift+Win+W", w);
		}

		public static void WmHotkey() {
			var p = mouse.xy;
			wnd w0 = wnd.fromXY(p), w = w0.Window, c = w == w0 ? default : w0;
			//int color = 0; using (var dc = new ScreenDC_()) { color = Api.GetPixel(dc, p.x, p.y); }
			string path = WndUtil.GetWindowsStoreAppId(w, true, true);

			const int sh = 30;
			var screenshot = App.Settings.edit_noImages ? null : ColorQuantizer.MakeScreenshotComment(new(p.x - sh, p.y - sh / 2, sh * 2, sh), dpi: App.Hwnd);

			var m = new popupMenu();
			m["Find window"] = o => _Insert(_Wnd_Find(w, default, false));
			m["Find control"] = o => _Insert(_Wnd_Find(w, c, false));
			m.Last.IsDisabled = c.Is0;
			m["Wait for window"] = o => _Insert(_Wnd_Find(w, c, true));
			m["Activate window"] = o => _Insert(_Wnd_Find(w, default, false) + "\r\nw.Activate();");
			m.Submenu("Click", m => {
				m["Window"] = o => _Insert(_Wnd_Find(w, default, false) + _Click(w, "w"));
				m["Control"] = o => _Insert(_Wnd_Find(w, c, false) + _Click(c, "c"));
				m.Last.IsDisabled = c.Is0;
				m["Screen"] = o => _Insert($"mouse.click({p.x}, {p.y});{screenshot}");
				string _Click(wnd w, string v) {
					w.MapScreenToClient(ref p);
					return $"\r\nmouse.click({v}, {p.x}, {p.y});{screenshot}";
				}
			});
			//	m.Submenu("Get color", m => {
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
			//	});
			if (path != null)
				m.Submenu("Program path", m => {
					m["var s = path;"] = o => _Path(0);
					m["run.it(path);"] = o => _Path(1);
					m["t[name] = o => run.it(path);"] = o => _Path(2);
					void _Path(int what) {
						if (path.Ends(@"\explorer.exe") && w.ClassNameIs("CabinetWClass")) { //if folder window, try to get folder path
							var tb = w.Child("***id 1001", "ToolbarWindow32"); // @"Address: C:\Program Files (x86)\Windows Kits\10\bin\x86"
							if (!tb.Is0 && tb.Name is string sa && sa.RMatch(@"^\S+: +(.+)", 1, out RXGroup rg) && filesystem.exists(sa = rg.Value, useRawPath: true).isDir) path = sa;
						}
						var g = new TUtil.PathInfo(path);
						int f = g.SelectFormatUI(); if (f == 0) return;
						var r = g.GetResult(f);
						var s = r.path;
						if (what == 2 && path.Starts("shell:")) r.name = w.Name;
						_Insert(what switch { 1 => $"run.it({s});", 2 => $"t[{_Str(r.name)}] = o => run.it({s});", _ => $"var s = {s};" });
					}
				});
			m.Separator();
			m["wnd dialog"] = o => new Dwnd(w0).Show();
			m["elm dialog"] = o => Delm.Dialog(p: p);
			m.Separator();
			m.Add("Cancel");

			m.Show();
		}

		static string _Str(string s) {
			if (s == null) return "null";
			if (!TUtil.MakeVerbatim(ref s)) s = s.Escape(quote: true);
			return s;
		}

		static string _Wnd_Find(wnd w, wnd c, bool wait) {
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
				_ = _ConName("", c.Name) || _ConName("***wfName ", c.NameWinforms) || _ConName("***elmName ", c.NameElm);

				bool _ConName(string prefix, string value) {
					if (value.NE()) return false;
					cName = prefix + TUtil.EscapeWildex(value);
					return true;
				}

				if (TUtil.GetUsefulControlId(c, w, out int id)) {
					f.idC = id.ToS();
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
			//print.it(s);
			InsertCode.Statements(s, fold: s.Contains("/*image:\r\n"));
		}
	}
}
