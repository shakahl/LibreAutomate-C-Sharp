using Au.Types;
using Au.Util;
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
using System.Linq;
using System.Drawing;

namespace Au
{
	/// <summary>
	/// Popup menu.
	/// </summary>
	/// <remarks>
	/// Can be used everywhere: in automation scripts, WPF apps, Winforms apps, etc.
	/// Uses Windows native menu API (<msdn>TrackPopupMenuEx</msdn>, etc).
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// var m = new AMenu("example");
	/// m["One"] = o => AOutput.Write(o);
	/// m["Two\0Tooltip", image: AIcon.Stock(StockIcon.DELETE)] = o => { AOutput.Write(o); ADialog.Show(o.ToString()); };
	/// using(m.Submenu("Submenu")) {
	/// 	m["Three"] = o => AOutput.Write(o);
	/// 	m["Four"] = o => AOutput.Write(o);
	/// }
	/// m["notepad"] = o => AFile.TryRun(AFolders.System + "notepad.exe");
	/// m.Show();
	/// ]]></code>
	/// </example>
	public unsafe class AMenu : MTBase
	{
		/// <summary>
		/// Represents a menu item in <see cref="AMenu"/>.
		/// </summary>
		public class MenuItem : MTItem
		{
			/// <summary>Gets menu item action.</summary>
			public Action<MenuItem> Clicked => base.clicked as Action<MenuItem>;

			/// <summary>Gets menu item id.</summary>
			public int Id { get; init; }

			/// <summary>Gets or sets checked state.</summary>
			public bool IsChecked { get; set; }

			/// <summary>Gets or sets disabled state.</summary>
			public bool IsDisabled { get; set; }

			/// <summary>Gets or sets whether to use bold font.</summary>
			public bool IsBold { get; set; }

			/// <summary>true if is a submenu-item.</summary>
			internal bool IsSubmenu { get; init; }

			internal int level; //0 or level of parent submenu
			internal bool submenuFilled; //submenu is filled once and don't do it again until reopening
			internal bool separator;
			internal bool column; //add MFT_MENUBARBREAK
			internal byte checkType; //1 checkbox, 2 radio
			internal IntPtr hbitmap; //item bitmap; disposed when menu closed
			internal Action<AMenu> submenuOpening; //for lazy submenu
		}

		readonly List<MenuItem> _a = new(); //we'll convert it to native menu items in Show() and WM_INITMENUPOPUP (for submenus). In Show() we'll finally destroy the native popup menu.
		readonly Dictionary<IntPtr, int> _dSub = new(); //HMENU and its submenu-item index in _a; -1 for the main menu. To recognize our HMENUs in WM_INITMENUPOPUP etc.
		int _level; //0 or level of current submenu
		int _lastId; //to auto-generate item ids

		/// <summary>
		/// Use this constructor for various context menus of your app.
		/// </summary>
		/// <remarks>
		/// Users cannot right-click a menu item and open/select it in editor.
		/// </remarks>
		public AMenu() { }

		/// <summary>
		/// Use this constructor in scripts.
		/// </summary>
		/// <param name="name">Menu name. Must be a unique valid filename. Currently not used.</param>
		/// <param name="f">Don't use.</param>
		/// <param name="l">Don't use.</param>
		/// <remarks>
		/// This overload sets <see cref="MTBase.ExtractIconPathFromCode"/> = true.
		/// 
		/// Users can right-click an item to open/select it in editor. To disable this feature, explicitly set <i>f</i> = null; or use other overload.
		/// </remarks>
		public AMenu(string name, [CallerFilePath] string f = null, [CallerLineNumber] int l = 0) : base(name, f, l) {
		}

		/// <summary>
		/// Gets the last added menu item.
		/// </summary>
		public MenuItem Last { get; private set; }

		MenuItem _Add(int id, string text, MTImage image, bool disable, int l, Action<MenuItem> click = null, byte checkType = 0, bool submenu = false) {
			MenuItem r = new() {
				Id = click == null && checkType == 0 ? id : -_a.Count - 2, //note: don't use id -1. Eg GetMenuItemID returns -1 if id is 0 etc.
				checkType = checkType,
				IsDisabled = disable,
				IsSubmenu = submenu,
				level = _level,
				column = _column,
			};
			_column = false;
			r.Set_(this, text, click, image, l);
			_a.Add(r);
			Last = r;
			return r;
		}

		/// <summary>
		/// Adds menu item with explicitly specified id.
		/// </summary>
		/// <param name="id">Item id that <see cref="Show"/> will return if clicked this item. Cannot be negative.</param>
		/// <param name="text">Item text, or "Text\0 Tooltip". Character with prefix &amp; will be underlined (depends on Windows settings) and can be used to select the item with keyboard. Tab character separates left-aligned text and right-aligned text.</param>
		/// <param name="image">Item image. Read here: <see cref="MTBase"/>.</param>
		/// <param name="disable">Disabled state.</param>
		/// <param name="l">Don't use.</param>
		/// <exception cref="ArgumentException">Negative id.</exception>
		public MenuItem Add(int id, string text, MTImage image = default, bool disable = false, [CallerLineNumber] int l = 0) {
			if (id < 0) throw new ArgumentException(); //we use negative ids for action items and check/radio items
			return _Add(_lastId = id, text, image, disable, l);
		}

		/// <summary>
		/// Adds menu item with auto-generated id.
		/// </summary>
		/// <param name="text">Item text, or "Text\0 Tooltip". Character with prefix &amp; will be underlined (depends on Windows settings) and can be used to select the item with keyboard. Tab character separates left-aligned text and right-aligned text.</param>
		/// <param name="image">Item image. Read here: <see cref="MTBase"/>.</param>
		/// <param name="disable">Disabled state.</param>
		/// <param name="l">Don't use.</param>
		/// <remarks>
		/// Assigns id = the last specified or auto-generated id + 1. If not using explicitly specified ids, auto-generated ids are 1, 2, 3... Submenu-items, separators and items with action don't auto-generate ids.
		/// </remarks>
		public MenuItem Add(string text, MTImage image = default, bool disable = false, [CallerLineNumber] int l = 0)
			=> _Add(++_lastId, text, image, disable, l);

		/// <summary>
		/// Adds menu item with action (callback function) that is executed on click.
		/// </summary>
		/// <param name="text">Item text, or "Text\0 Tooltip". Character with prefix &amp; will be underlined (depends on Windows settings) and can be used to select the item with keyboard. Tab character separates left-aligned text and right-aligned text.</param>
		/// <param name="click">Callback function that is executed on click.</param>
		/// <param name="image">Item image. Read here: <see cref="MTBase"/>.</param>
		/// <param name="disable">Disabled state.</param>
		/// <param name="l">Don't use.</param>
		/// <remarks>
		/// This function is the same as the indexer. The difference is, <b>Add</b> returns <b>MenuItem</b> object of the added item. When using the indexer, to access the item use <see cref="Last"/>. These codes are the same: <c>var v=m.Add("text", o=>{});"</c> and <c>m["text"]=o=>{}; var v=m.Last;</c>.
		/// </remarks>
		public MenuItem Add(string text, Action<MenuItem> click, MTImage image = default, bool disable = false, [CallerLineNumber] int l = 0)
			=> _Add(0, text, image, disable, l, click);

		/// <summary>
		/// Adds menu item with action (callback function) that is executed on click.
		/// </summary>
		/// <param name="text">Item text, or "Text\0 Tooltip". Character with prefix &amp; will be underlined (depends on Windows settings) and can be used to select the item with keyboard. Tab character separates left-aligned text and right-aligned text.</param>
		/// <param name="image">Item image. Read here: <see cref="MTBase"/>.</param>
		/// <param name="disable">Disabled state.</param>
		/// <param name="l">Don't use.</param>
		/// <value>Callback function that is executed on click.</value>
		/// <remarks>
		/// This function is the same as <see cref="Add(string, Action{MenuItem}, MTImage, bool, int)"/>. The difference is, <b>Add</b> returns <b>MenuItem</b> object of the added item. When using the indexer, to access the item use <see cref="Last"/>. These codes are the same: <c>var v=m.Add("text", o=>{});"</c> and <c>m["text"]=o=>{}; var v=m.Last;</c>.
		/// </remarks>
		public Action<MenuItem> this[string text, MTImage image = default, bool disable = false, [CallerLineNumber] int l = 0] {
			set { _Add(0, text, image, disable, l, value); }
		}

		/// <summary>
		/// Adds menu item to be used as a checkbox.
		/// </summary>
		/// <param name="text">Item text, or "Text\0 Tooltip". Character with prefix &amp; will be underlined (depends on Windows settings) and can be used to select the item with keyboard. Tab character separates left-aligned text and right-aligned text.</param>
		/// <param name="check">Checked state.</param>
		/// <param name="click">Callback function that is executed on click.</param>
		/// <param name="disable">Disabled state.</param>
		/// <param name="image">Item image. Read here: <see cref="MTBase"/>.</param>
		/// <param name="l">Don't use.</param>
		/// <remarks>
		/// When clicked, <see cref="MenuItem.IsChecked"/> state is changed.
		/// </remarks>
		public MenuItem AddCheck(string text, bool check = false, Action<MenuItem> click = null, bool disable = false, MTImage image = default, [CallerLineNumber] int l = 0) {
			var r = _Add(0, text, image, disable, l, click, 1);
			r.IsChecked = check;
			return r;
		}

		/// <summary>
		/// Adds menu item to be used as a radio button in a group of such items.
		/// </summary>
		/// <param name="text">Item text, or "Text\0 Tooltip". Character with prefix &amp; will be underlined (depends on Windows settings) and can be used to select the item with keyboard. Tab character separates left-aligned text and right-aligned text.</param>
		/// <param name="check">Checked state.</param>
		/// <param name="click">Callback function that is executed on click.</param>
		/// <param name="disable">Disabled state.</param>
		/// <param name="image">Item image. Read here: <see cref="MTBase"/>.</param>
		/// <param name="l">Don't use.</param>
		/// <remarks>
		/// When clicked an unchecked radio item, its <see cref="MenuItem.IsChecked"/> state becomes true; <b>IsChecked</b> of other group items become false.
		/// </remarks>
		public MenuItem AddRadio(string text, bool check = false, Action<MenuItem> click = null, bool disable = false, MTImage image = default, [CallerLineNumber] int l = 0) {
			var r = _Add(0, text, image, disable, l, click, 2);
			r.IsChecked = check;
			return r;
		}

		/// <summary>
		/// Adds menu item that opens a submenu.
		/// Used like <c>using (m.Submenu("Example")) { /* add submenu items */ }</c>.
		/// </summary>
		/// <param name="text">Item text, or "Text\0 Tooltip". Character with prefix &amp; will be underlined (depends on Windows settings) and can be used to select the item with keyboard. Tab character separates left-aligned text and right-aligned text.</param>
		/// <param name="image">Item image. Read here: <see cref="MTBase"/>.</param>
		/// <param name="disable">Disabled state.</param>
		/// <param name="l">Don't use.</param>
		public UsingEndAction Submenu(string text, MTImage image = default, bool disable = false, [CallerLineNumber] int l = 0) {
			_Add(0, text, image, disable, l, submenu: true);
			_level++;
			return new UsingEndAction(() => _level--);
		}

		/// <summary>
		/// Adds menu item that opens a submenu that will be filled by a callback function when opening the submenu.
		/// Used like <c>m.Submenu("Example", m => { /* add submenu items */ });</c>.
		/// </summary>
		/// <param name="text">Item text, or "Text\0 Tooltip". Character with prefix &amp; will be underlined (depends on Windows settings) and can be used to select the item with keyboard. Tab character separates left-aligned text and right-aligned text.</param>
		/// <param name="opening">Callback function that is called when opening the submenu and should add items to it.</param>
		/// <param name="image">Item image. Read here: <see cref="MTBase"/>.</param>
		/// <param name="disable">Disabled state.</param>
		/// <param name="l">Don't use.</param>
		/// <example>
		/// <code><![CDATA[
		/// var m=new AMenu();
		/// _Dir(new DirectoryInfo(@"C:\"));
		/// m.Show();
		/// 
		/// void _Dir(DirectoryInfo dir) {
		/// 	foreach (var v in dir.EnumerateFileSystemInfos()) {
		/// 		if(v.Attributes.Has(FileAttributes.System|FileAttributes.Hidden)) continue;
		/// 		if(v.Attributes.Has(FileAttributes.Directory)) {
		/// 			m.Submenu(v.Name, m=> _Dir(v as DirectoryInfo));
		/// 		} else {
		/// 			m[v.Name]=o=>AOutput.Write(v.FullName);
		/// 		}
		/// 		m.Last.File = v.FullName;
		/// 	}
		/// }
		/// ]]></code>
		/// </example>
		public MenuItem Submenu(string text, Action<AMenu> opening, MTImage image = default, bool disable = false, [CallerLineNumber] int l = 0) {
			var v = _Add(0, text, image, disable, l, submenu: true);
			v.submenuOpening = opening;
			return v;
		}

		/// <summary>
		/// Adds horizontal or vertical separator.
		/// </summary>
		/// <param name="vertical">Add other items in new column.</param>
		/// <remarks>
		/// Menus with vertical separators look differently.
		/// </remarks>
		public void Separator(bool vertical = false) {
			if (vertical) _column = true;
			else _a.Add(new() { separator = true, level = _level });
		}
		bool _column;

		//rejected. Menu items without icon are OK. Need only for toolbars.
		//	/// <summary>
		//	/// Image for items that don't have an image specified or auto-extracted from code.
		//	/// </summary>
		//	public MTImage DefaultImage { get; set; }
		//	
		//	/// <summary>
		//	/// Image for submenu-items that don't have an image specified.
		//	/// </summary>
		//	public MTImage DefaultSubmenuImage { get; set; }

		/// <summary>
		/// Shows the menu and waits until closed.
		/// Returns id of the selected item when closed, or 0 if cancelled or failed.
		/// </summary>
		/// <param name="owner">Owner window. Optional. Receives menu messages (rarely useful). Also, closing it will close the menu too. Must belong to this thread.</param>
		/// <param name="flags"></param>
		/// <param name="xy">Menu position. If null (default), uses mouse position by default. It depends on flags.</param>
		/// <param name="excludeRect">The menu should not overlap this rectangle in screen.</param>
		/// <exception cref="ArgumentException"><i>owner</i> does not belong to this thread or is invalid window handle.</exception>
		/// <remarks>
		/// To show menu, uses API <msdn>TrackPopupMenuEx</msdn>. It fails if this thread is in menu mode (showing any standard Windows menu), unless called with flag <see cref="MSFlags.Recurse"/>. Therefore, if in menu mode and no flag, this function just calls API <msdn>EndMenu</msdn> and returns 0.
		/// </remarks>
		public int Show(AnyWnd owner = default, MSFlags flags = 0, POINT? xy = null, RECT? excludeRect = null) {
			if (IsVisible || (!flags.Has(MSFlags.Recurse) && (AWnd.More.GetGUIThreadInfo(out var gti, AThread.Id) && gti.flags.Has(Native.GUI.INMENUMODE)))) {
				//TPM fails, unless with TPM_RECURSE
#if false
			//this works, but after 30 times the process hangs and cannot be terminated, need to reboot. Maybe stack overflow. Removing hook/timer/subclass does not help.
			Api.EndMenu();
			_Clear();
			flags|=MSFlags.Recurse;
#else
				//throw new InvalidOperationException("thread is in menu mode");
				Api.EndMenu();
				return 0;
#endif
			}

			Api.TPMPARAMS* tpp = null;
			var tp = new Api.TPMPARAMS { cbSize = sizeof(Api.TPMPARAMS) };
			if (excludeRect != null) {
				tp.rcExclude = excludeRect.Value;
				tpp = &tp;
			}

			POINT p;
			if (flags.Has(MSFlags.ByCaret) && AKeys.More.GetTextCursorRect(out RECT cr, out _)) {
				//p = new POINT(cr.left - ADpi.Scale(32, AScreen.Of(cr).Dpi), cr.bottom); //no, menu margins are not DPI-scaled. Its width depends on dpi-scaled image width, but only if using images.
				p = new POINT(cr.left - 32, cr.bottom);
				if (tpp == null) {
					cr.Inflate(4, 0);
					tp.rcExclude = cr;
					tpp = &tp;
				}
			} else {
				p = xy ?? AMouse.XY;
				if (flags.Has(MSFlags.ScreenCenter)) {
					var rs = AScreen.Of(p).WorkArea;
					p = new(rs.CenterX, rs.CenterY);
				}
			}

			_dpi = AScreen.Of(p).Dpi;

			var ow = owner.Hwnd;
			bool noOwner = ow.Is0;
			if (noOwner) ow = AWnd.More.CreateMessageOnlyWindow("#32770");
			else if (!ow.IsOfThisThread) throw new ArgumentException("menu owner must be in current thread");

			AHookWin hKey = null;
			ATimer timer = null;
			Api.SUBCLASSPROC subclassProc = null;
			var hmenu = Api.CreatePopupMenu();
			int resultId = 0; MenuItem resultItem = null;
			try {
				_dSub.Add(hmenu, -1);
				_FillPopupMenu(hmenu, 0, 0);

				if (!flags.Has(MSFlags_Raw_)) {
					if (!AWnd.Active.IsOfThisThread) {
						//never mind: hooks don't work if active window has higher UAC IL. Then use timer and mouse/Esc toggle state.
						if (!flags.Has(MSFlags_NoKeyHook_)) {
							hKey = AHookWin.Keyboard(g => {
								if (g.Mod != 0 || (g.Key >= KKey.F1 && g.Key <= KKey.F24) || g.Key == KKey.PrintScreen || g.Key == KKey.Pause || g.Key == KKey.CapsLock || g.Key == KKey.NumLock || g.Key == KKey.ScrollLock || AKeys.IsMod()) return;
								g.BlockEvent();
								Api.PostMessage(default, g.IsUp ? Api.WM_KEYUP : Api.WM_KEYDOWN, (int)g.Key, default);
								if (!g.IsUp) Api.TranslateMessage(new() { hwnd = ow, message = Api.WM_KEYDOWN, wParam = (int)g.Key });
							});
						}

						//to close with mouse use timer. Mouse hook may not work because of UAC.
						byte mouseState = _GetMouseState();
						timer = new(t => {
							//close if mouse clicked a window of another thread
							byte ms = _GetMouseState();
							bool clicked = ms != mouseState;
							mouseState = ms;
							if (AWnd.Active.IsOfThisThread) return;
							if (clicked && !AWnd.FromMouse(WXYFlags.NeedWindow).IsOfThisThread) Close();

						});
						timer.Every(30);

						static byte _GetMouseState() => (byte)(
							(AKeys.UI.GetKeyState(KKey.MouseLeft) & 1)
							| ((AKeys.UI.GetKeyState(KKey.MouseRight) & 1) << 1)
							| ((AKeys.UI.GetKeyState(KKey.MouseMiddle) & 1) << 2)
							);
					}

					Api.SetWindowSubclass(ow, subclassProc = _SubclassProc, 1);
					//never mind: may not need to subclass. Need if inactive thread or _sourceFile != null or has submenus or has tooltips.
				} else {
					ADebug.PrintIf(_dSub.Count > 1, "raw");
				}

				if (flags.Has(MSFlags_SelectFirst_)) {
					Api.PostMessage(default, Api.WM_KEYDOWN, (int)KKey.Down, 0);
					//never mind: Does not work if a mouse button is pressed, for example on double click.
					//Tried undocumented MN_SELECTFIRSTVALIDITEM = 0x1E7, but works only if thread active. Not tested when mouse button pressed.
				}
				//never mind: (flag) if pressed a key not used for the menu, close the menu and don't eat the key.
				//	Would be useful for autotext confirm.
				//	But &underlined chars make it difficult.


				//never mind: by default does not draw underlines. Users can set it in Windows Settings -> Ease of Access -> Keyboard.
				//	Tested: can underline with SendInput(Alt), but SetKeyboardState doesn't work.

				resultId = Api.TrackPopupMenuEx(hmenu, ((uint)flags & 0xffffff) | Api.TPM_RETURNCMD, p.x, p.y, ow, tpp);
				//CONSIDER: if 0, throw exception if failed

				if (resultId < 0) { //has action or/and is check/radio
					int i = -resultId - 2;
					var v = resultItem = _a[i];

					if (v.checkType == 1) {
						v.IsChecked ^= true;
					} else if (v.checkType == 2) { //radio
						int level = v.level;
						bool _SameGroup(int j) => (uint)j < _a.Count && _a[j].checkType == 2 && _a[j].level == level;
						for (int j = i; _SameGroup(--j);) _a[j].IsChecked = false;
						for (int j = i; _SameGroup(++j);) _a[j].IsChecked = false;
						v.IsChecked = true;
					}
				}
			}
			finally {
				hKey?.Dispose();
				timer?.Stop();
				Api.DestroyMenu(hmenu);
				if (subclassProc != null) Api.RemoveWindowSubclass(ow, subclassProc, 1);
				if (noOwner) Api.DestroyWindow(ow);
				if (!_tt.tt.Is0) { Api.DestroyWindow(_tt.tt); _tt = default; }

				int lazyFrom = 0;
				for (int i = 0; i < _a.Count; i++) {
					var v = _a[i];
					if (v.hbitmap != default) { Api.DeleteObject(v.hbitmap); v.hbitmap = default; }
					v.submenuFilled = false;
					if (v.level < 0 && lazyFrom == 0) lazyFrom = i;
				}
				if (lazyFrom > 0) _a.RemoveRange(lazyFrom, _a.Count - lazyFrom);
				_dSub.Clear();
			}

			if (resultItem?.Clicked != null) {
				if (resultItem.actionThread) AThread.Start(() => _ExecItem(), background: false); else _ExecItem();
				void _ExecItem() {
					try { resultItem.Clicked(resultItem); }
					catch (Exception ex) when (!resultItem.actionException) { AWarning.Write(ex.ToString(), -1); }
				}
			}
			return resultId;
		}

		/// <summary>
		/// Adds native menu items to the main menu or to a submenu (on WM_INITMENUPOPUP).
		/// Loads images etc.
		/// </summary>
		/// <param name="h">Menu handle (main or submenu).</param>
		/// <param name="level">0 or submenu level.</param>
		/// <param name="i">First item index in _a.</param>
		void _FillPopupMenu(IntPtr h, int level, int i) {
			AMemoryBitmap mb = null;
			Graphics gr = null;
			int imageSize = ADpi.Scale(16, _dpi);
			//bool hasColumns=false, hasCheck=false;
			try {
				for (; i < _a.Count; i++) {
					var v = _a[i];
					if (v.level < level) break;
					if (v.level > level) continue;
					if (v.separator) {
						Api.AppendMenu(h);
					} else {
						uint state = 0;
						if (v.IsDisabled) state |= Api.MFS_DISABLED;
						if (v.IsChecked) state |= Api.MFS_CHECKED;
						//if (v.IsHilited) state |= Api.MFS_HILITE; //hilites but does not make focused. Also tested HiliteMenuItem, but it works only with menubar.
						if (v.IsBold) state |= Api.MFS_DEFAULT;
						var x = new Api.MENUITEMINFO(Api.MIIM_ID | Api.MIIM_DATA | Api.MIIM_FTYPE | Api.MIIM_STRING | Api.MIIM_STATE) { wID = v.Id, dwItemData = i + 1, fState = state };
						if (v.checkType == 2) x.fType |= Api.MFT_RADIOCHECK;
						if (v.column) x.fType |= Api.MFT_MENUBARBREAK;
						//if(v.column) hasColumns=true;
						//if(v.checkType!=0) hasCheck=true;
						if (v.IsSubmenu) {
							x.fMask |= Api.MIIM_SUBMENU;
							x.hSubMenu = Api.CreatePopupMenu();
							_dSub.Add(x.hSubMenu, i);
						}

						//APerf.First();
						var (im, dispose) = _GetImage(v);
						//APerf.Next();
						if (im != null) {
							if (mb == null) {
								mb = new AMemoryBitmap(imageSize, imageSize);
								gr = Graphics.FromHdc(mb.Hdc);
								gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
							}
							gr.Clear(Color.Transparent);
							gr.DrawImage(im, 0, 0, imageSize, imageSize);
							if (dispose) im.Dispose();
							//x.hbmpItem=(IntPtr)Api.HBMMENU_CALLBACK; //no theme
							v.hbitmap = Api.CopyImage(mb.Hbitmap, 0, 0, 0, Api.LR_CREATEDIBSECTION); //LR_CREATEDIBSECTION with alpha
							if (v.hbitmap != default) {
								x.fMask |= Api.MIIM_BITMAP;
								x.hbmpItem = v.hbitmap;
							}
						}

						fixed (char* pt = v.Text) {
							x.dwTypeData = pt;
							Api.InsertMenuItem(h, -1, true, x);
						}
						//APerf.Next(); AOutput.Write(APerf.ToString(), v);
					}
				}
			}
			finally {
				gr?.Dispose();
				mb?.Dispose();
			}

			//not good anyway:
			//if(hasColumns && !hasCheck) api.SetMenuInfo(h, new() { cbSize=sizeof(api.MENUINFO), fMask=api.MIM_STYLE, dwStyle=api.MNS_NOCHECK });
			//if(hasColumns && !hasCheck) api.SetMenuInfo(h, new() { cbSize=sizeof(api.MENUINFO), fMask=api.MIM_STYLE, dwStyle=api.MNS_CHECKORBMP});
		}

		LPARAM _SubclassProc(AWnd w, int msg, LPARAM wParam, LPARAM lParam, LPARAM uIdSubclass, nint dwRefData) {
			//AWnd.More.PrintMsg(w, msg, wParam, lParam);

			switch (msg) {
			case Api.WM_INITMENUPOPUP:
				if (_dSub.TryGetValue(wParam, out int i) && i >= 0) {
					var v = _a[i];
					if (!v.submenuFilled) {
						v.submenuFilled = true;
						if (v.submenuOpening != null) {
							i = _a.Count;
							_a.Add(new() { level = -1 }); //1. Mark to remove lazily added items. 2. Separate this group from other items.
							_level = v.level + 1;
							try { v.submenuOpening(this); }
							finally { _level = 0; }
						}
						_FillPopupMenu(wParam, v.level + 1, i + 1);
					}
				}
				break;
			case Api.WM_MENURBUTTONUP:
				//AOutput.Write("WM_MENURBUTTONUP"); //missing for expanded submenu-item, but user can press Esc and then works
				if (_dSub.ContainsKey(lParam)) {
					var item = _IndexToItem(lParam, (int)wParam);
					if (item != null) {
						bool canEdit = _sourceFile != null && AScriptEditor.Available, canFile = item.extractIconPath == 2;
						if (canEdit || canFile) {
							var m = new AMenu();
							if (canEdit) m.Add(1, "Edit menu item");
							if (canFile) m.Add(2, "Find file");
							int k = m.Show(w, MSFlags.Recurse | MSFlags_Raw_);
							if (k == 1 || k == 2) {
								if (k == 1) AScriptEditor.GoToEdit(_sourceFile, item.sourceLine);
								if (k == 2) AFile.SelectInExplorer(item.image as string);
								msg = Api.WM_CANCELMODE; wParam = 0; lParam = 0;
							}
						}
					}
				}
				break;
			case Api.WM_ENTERIDLE when wParam == 2: //MSGF_MENU

				//never mind: this message may be frequent, eg on wm_mousemove etc anywhere in this thread. This code is fast.
				//	Alternatively use a hook.

				var w1 = (AWnd)lParam; //popup menu window
				IntPtr hm = w1.Send(Api.MN_GETHMENU);
				if (_dSub.ContainsKey(hm)) {
					//prevent activating the menu window on click when current thread isn't the foreground thread
					if (!w1.HasExStyle(WS2.NOACTIVATE)) w1.SetExStyle(WS2.NOACTIVATE, WSFlags.Add);

					//tooltip
					var xy = AMouse.XY;
					int index = Api.MenuItemFromPoint(default, hm, xy);
					var item = _IndexToItem(hm, index);
					bool failed = false;
					if (item != null && item != _tt.item && !item.Tooltip.NE()) {
						failed = !_GetRect(w1, hm, index, out var r);
						if (!failed) {
							_tt.item = item;
							_tt.hm = hm;
							_tt.index = index;
							_tt.rect = r;
							if (_tt.tt.Is0) {
								_tt.tt = Api.CreateWindowEx(WS2.TOPMOST | WS2.TRANSPARENT, "tooltips_class32", null, Api.TTS_ALWAYSTIP | Api.TTS_NOPREFIX | Api.TTS_BALLOON, 0, 0, 0, 0, w);
								_tt.tt.Send(Api.TTM_ACTIVATE, true);
								_tt.tt.Send(Api.TTM_SETMAXTIPWIDTH, 0, AScreen.Of(w1).WorkArea.Width / 3);
							}
							fixed (char* ps = item.Tooltip) {
								var g = new Api.TTTOOLINFO { cbSize = sizeof(Api.TTTOOLINFO), hwnd = w1, uId = 1, lpszText = ps, rect = r };
								_tt.tt.Send(Api.TTM_DELTOOL, 0, &g);
								_tt.tt.Send(Api.TTM_ADDTOOL, 0, &g);
							}
						}
					} else if (_tt.item != null && hm == _tt.hm) { //update tooltip tool rect when scrolled
						failed = !_GetRect(w1, hm, _tt.index, out var rr) || rr != _tt.rect;
					}
					if (failed && _tt.item != null) {
						_tt.item = null;
						var g = new Api.TTTOOLINFO { cbSize = sizeof(Api.TTTOOLINFO), hwnd = w1, uId = 1 };
						_tt.tt.Send(Api.TTM_DELTOOL, 0, &g);
					}

					if (_tt.item != null) { //use TTM_RELAYEVENT on mouse move etc. Somehow TTF_SUBCLASS does not work.
						w1.MapScreenToClient(ref xy);
						var v = new Native.MSG { hwnd = w1, message = Api.WM_MOUSEMOVE, lParam = AMath.MakeUint(xy.x, xy.y) };
						_tt.tt.Send(Api.TTM_RELAYEVENT, 0, &v);
					}
				}
				break;
			//case Api.WM_MENUSELECT when 0 != ((uint)wParam & 0x80000000): //MF_MOUSESELECT
			//	if (_dSub.ContainsKey(lParam)) {
			//		var item = _IndexToItem(lParam, Api.MenuItemFromPoint(default, lParam, AMouse.XY));
			//		//AOutput.Write("WM_MENUSELECT", AMath.LoWord(wParam), AMath.HiWord(wParam), item);
			//	}
			//	break;
			case Api.WM_MENUCHAR when AMath.LoWord(wParam) is '\t' or ' ':
				if (_dSub.ContainsKey(lParam)) {
					//return AMath.MakeUint(0, 2); //MNC_EXECUTE. But how to find which item is selected?
					Api.PostMessage(default, Api.WM_KEYDOWN, (int)KKey.Enter, default);
					return default;
				}
				break;
			}

			MenuItem _IndexToItem(IntPtr hmenu, int index) {
				if (index >= 0) {
					var v = new Api.MENUITEMINFO(Api.MIIM_DATA);
					if (Api.GetMenuItemInfo(hmenu, index, true, ref v)) {
						int j = (int)v.dwItemData - 1;
						if ((uint)j < _a.Count) return _a[j]; //not a separator
					}
				}
				return null;
			}

			static bool _GetRect(AWnd wm, IntPtr hm, int index, out RECT r) {
				r = default;
				if (!Api.GetMenuItemRect(default, hm, index, out r)) return false;

				//workaround for GetMenuItemRect bug: when scrolled, get nonscrolled rect. MSAA and UIA too.
				int i = Api.MenuItemFromPoint(default, hm, new(r.left, r.top));
				if (i != index) {
					var rc = wm.ClientRectInScreen;
					i = Api.MenuItemFromPoint(default, hm, new(rc.left, rc.top));
					if (!Api.GetMenuItemRect(default, hm, i, out var rr)) return false;
					wm.MapScreenToClient(ref rr);
					r.Offset(0, -rr.top);
				}

				wm.MapScreenToClient(ref r);
				return true;
			}

			return Api.DefSubclassProc(w, msg, wParam, lParam);
		}

		(AWnd tt, MenuItem item, IntPtr hm, int index, RECT rect) _tt;

		/// <summary>
		/// Closes this popup menu, including submenus.
		/// </summary>
		public void Close() {
			Api.EndMenu();
		}

		/// <summary>
		/// true if this thread is in <b>Show</b> function of this menu.
		/// </summary>
		public bool IsVisible => _dSub.Count > 0;

		/// <summary>
		/// Gets added items.
		/// </summary>
		/// <param name="submenu">Get only items of this submenu.</param>
		/// <param name="skipSubmenus">Skip items in submenus.</param>
		/// <exception cref="ArgumentException"><i>submenu</i> isn't in this menu.</exception>
		/// <remarks>
		/// Skips separators and items in submenus filled by <see cref="Submenu(string, Action{AMenu}, MTImage, bool, int)"/> callback function.
		/// </remarks>
		public IEnumerable<MenuItem> Items(MenuItem submenu = null, bool skipSubmenus = false) {
			int i = 0, level = 0;
			if (submenu != null) {
				i = _a.IndexOf(submenu);
				if (i++ < 0) throw new ArgumentException();
				if (submenu.submenuOpening != null) yield break;
				level = submenu.level + 1;
			}
			for (; i < _a.Count; i++) {
				var v = _a[i];
				if (v.level < level) break;
				if (v.level > level && skipSubmenus) continue;
				if (v.separator) continue;
				yield return v;
			}
		}

		/// <summary>
		/// Creates and shows simple popup menu. Without images, actions, submenus.
		/// Returns selected item id, or 0 if cancelled.
		/// </summary>
		/// <param name="items">
		/// Menu items. Can be string[], List&lt;string&gt; or string like "One|Two|Three".
		/// Item id can be optionally specified like "1 One|2 Two|3 Three". If missing, uses id of previous non-separator item + 1. Example: "One|Two|100 Three Four" //1|2|100|101.
		/// For separators use null or empty strings: "One|Two||Three|Four".
		/// </param>
		/// <param name="owner">Owner window. Optional.</param>
		/// <param name="flags"></param>
		/// <param name="xy">Menu position. If null (default), uses mouse position by default. It depends on flags.</param>
		/// <param name="excludeRect">The menu should not overlap this rectangle in screen.</param>
		/// <exception cref="ArgumentException"><i>owner</i> does not belong to this thread or is invalid window handle.</exception>
		/// <remarks>
		/// The function adds menu items and calls <see cref="Show"/>. Returns when menu closed. The last 4 parameters are same as of <b>Show</b>.
		/// </remarks>
		/// <seealso cref="ADialog.ShowList"/>
		public static int ShowSimple(DStringList items, AnyWnd owner = default, MSFlags flags = 0, POINT? xy = null, RECT? excludeRect = null) {
			var a = items.ToArray();
			var m = new AMenu();
			foreach (var v in a) {
				var s = v;
				if (s.NE()) {
					m.Separator();
				} else {
					if (s.ToInt(out int id, 0, out int end)) {
						if (s.Eq(end, ' ')) end++;
						s = s[end..];
						m.Add(id, s);
					} else {
						m.Add(s);
					}
				}
			}
			return m.Show(owner, flags, xy, excludeRect);
		}

		/// <summary>
		/// No hook/timer/subclass.
		/// Will not work submenus, context menu, tooltips, closing with Tab/Space, partially in inactive thread.
		/// </summary>
		const MSFlags MSFlags_Raw_ = (MSFlags)0x40000000;
		internal const MSFlags MSFlags_NoKeyHook_ = (MSFlags)0x20000000;
		internal const MSFlags MSFlags_SelectFirst_ = (MSFlags)0x10000000;
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="AMenu"/> <b>ShowX</b> methods.
	/// </summary>
	/// <remarks>
	/// Most flags are for API <msdn>TrackPopupMenuEx</msdn>.
	/// </remarks>
	[Flags]
	public enum MSFlags
	{
		/// <summary>Show by caret (text cursor) position. If not possible, depends on flag <b>ScreenCenter</b> or parameter <i>xy</i>.</summary>
		ByCaret = 0x1000000,

		/// <summary>Show in center of screen containing mouse pointer.</summary>
		ScreenCenter = 0x2000000,

		//TPM_ flags

		/// <summary>Let <b>ShowX</b> show menu when this thread is showing another menu. Without this flag then closes that menu and returns 0. Note: avoid multi-level recursion; the process may hang etc.</summary>
		Recurse = 0x1,

		/// <summary>Horizontally align the menu so that the show position would be in its center.</summary>
		AlignCenterH = 0x4,

		/// <summary>Horizontally align the menu so that the show position would be at its right side.</summary>
		AlignRight = 0x8,

		/// <summary>Vertically align the menu so that the show position would be in its center.</summary>
		AlignCenterV = 0x10,

		/// <summary>Vertically align the menu so that the show position would at in its bottom.</summary>
		AlignBottom = 0x20,

		/// <summary>Show at bottom or top of <i>excludeRect</i>, not at righ/left.</summary>
		AlignRectBottomTop = 0x40,

		/// <summary>Displays menu without animation.</summary>
		NoAnimation = 0x4000,

		/// <summary>Text layout from right-to-left.</summary>
		LayoutRtl = 0x8000,
	}
}