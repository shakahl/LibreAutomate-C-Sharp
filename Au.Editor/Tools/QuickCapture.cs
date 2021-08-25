//CONSIDER: disable hotkeys when editor hidden.

namespace Au.Tools
{
	static class QuickCapture
	{
		static keys.more.Hotkey _rk1, _rk2, _rk3;
		static popupMenu _m;

		public static void Info() {
			print.it($@"Hotkeys for quick capturing:
	{App.Settings.hotkeys.capture_menu} - capture window from mouse and show menu to insert code to find it etc.
	{App.Settings.hotkeys.capture_wnd} - capture window from mouse and show dialog 'Find window or control'.
	{App.Settings.hotkeys.capture_elm} - capture UI element from mouse and show dialog 'Find UI element'.
");
		}

		public static void RegisterHotkeys() {
			_Register(ref _rk1, ERegisteredHotkeyId.QuickCaptureMenu, App.Settings.hotkeys.capture_menu);
			_Register(ref _rk2, ERegisteredHotkeyId.QuickCaptureDwnd, App.Settings.hotkeys.capture_wnd);
			_Register(ref _rk3, ERegisteredHotkeyId.QuickCaptureDelm, App.Settings.hotkeys.capture_elm);

			void _Register(ref keys.more.Hotkey rk, ERegisteredHotkeyId id, string keys) { //ref, not in!
				try {
					if (!rk.Register((int)id, keys, App.Hwnd))
						print.warning($"Failed to register hotkey {keys} for quick capturing. You can set a hotkey in Options.", -1);
				}
				catch (Exception ex) { print.it(ex); }
			}
		}

		public static void UnregisterHotkeys() {
			_rk1.Unregister();
			_rk2.Unregister();
			_rk3.Unregister();
		}

		//CONSIDER simplified tool version. Without tree and capturing but with option to show full.
		//CONSIDER: the 'click' items should show a dialog to select Coord format. Or add tool in editor.
		//CONSIDER: use another hotkey/menu for click. With several such hotkeys/menus would even not need a recorder.
		//CONSIDER: while showing menu, show on-screen rectangles of the window, control and elm.
		//CONSIDER: recording by image: on every clicks show eg 400x400 window with captured area and titled "Capture image". Let user select image rect etc.
		//CONSIDER: instead of this hotkey/menu:
		//	While editor visible and CapsLock toggled:
		//		on single F3 show simplified Dwnd with options to click/activate/etc;
		//		on 2 F3 show simplified Delm with options to click/focus/etc;
		//		on 3 F3 show Duiimage.

		public static void Menu() {
			_m?.Close();

			var p = mouse.xy;
			wnd w0 = wnd.fromXY(p), w = w0.Window, c = w == w0 ? default : w0;
			//int color = 0; using (var dc = new ScreenDC_()) { color = Api.GetPixel(dc, p.x, p.y); }
			string path = WndUtil.GetWindowsStoreAppId(w, true, true);

			const int sh = 30;
			var screenshot = App.Settings.edit_noImages ? null : ColorQuantizer.MakeScreenshotComment(new(p.x - sh, p.y - sh / 2, sh * 2, sh), dpi: App.Hwnd);

			var m = new popupMenu();
			m["Find window"] = o => _Insert(_Wnd_Find(w, default));
			m["Activate window"] = o => _Insert(_Wnd_Find(w, default, activate: true));
			m.Submenu("Click", m => {
				m["Window"] = o => _Insert(_Wnd_Find(w, default) + _Click(w, "w"));
				m["Control"] = o => _Insert(_Wnd_Find(w, c) + _Click(c, "c"));
				m.Last.IsDisabled = c.Is0;
				//CONSIDER: UI element
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
							if (!tb.Is0 && tb.Name is string sa && sa.RxMatch(@"^\S+: +(.+)", 1, out RXGroup rg) && filesystem.exists(sa = rg.Value, useRawPath: true).isDir) path = sa;
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
			m["About"] = _ => Info();
			m.Add("Cancel");

			_m = m;
			m.Show();
			_m = null;
		}

		static string _Str(string s) {
			if (s == null) return "null";
			if (!TUtil.MakeVerbatim(ref s)) s = s.Escape(quote: true);
			return s;
		}

		static string _Wnd_Find(wnd w, wnd c, bool activate = false) {
			string wName = w.Name;
			var f = new TUtil.WindowFindCodeFormatter {
				Throw = true,
				Activate = activate,
				NeedWindow = true,
				NeedControl = !c.Is0,
				nameW = TUtil.EscapeWindowName(wName, true),
				classW = TUtil.StripWndClassName(w.ClassName, true),
				programW = wName.NE() ? w.ProgramName : null,
				waitW = "1",
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

		public static void ToolDwnd() {
			new Dwnd(wnd.fromMouse()).Show();
		}

		public static void ToolDelm() {
			Delm.Dialog(p: mouse.xy);
		}
	}
}
