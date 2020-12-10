#define REJECTED //some features rejected because of WPF bugs

//TODO: try System.Windows.Media.VisualTreeHelper.SetRootDpi(menu)

using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Reflection.Emit;

namespace Au
{
	/// <summary>
	/// Based on WPF <see cref="ContextMenu"/>, makes simpler to use it.
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// var m = new AWpfMenu(inScript: true);
	/// m["One"] = o => AOutput.Write(o);
	/// using(m.Submenu("Sub")) {
	/// 	m["Three"] = o => AOutput.Write(o);
	/// 	m["Four"] = o => AOutput.Write(o);
	/// }
	/// m.Separator();
	/// m["Two"] = o => { AOutput.Write(o); };
	/// m.Show(); //or m.IsOpen=true;
	/// ]]></code>
	/// </example>
	public class AWpfMenu : ContextMenu
	{
		/// <param name="inScript">Sets <see cref="CanEditScript"/> and <see cref="ExtractIconPathFromCode"/>.</param>
		/// <param name="actionThread">Sets <see cref="ActionThread"/>.</param>
		public AWpfMenu(bool inScript = false, bool actionThread = false) {
			CanEditScript = inScript;
			ExtractIconPathFromCode = inScript;
			ActionThread = actionThread;
		}

		/// <summary>
		/// Creates new <see cref="MenuItem"/> and adds to the menu. Returns it.
		/// </summary>
		/// <param name="text">
		/// Label. See <see cref="HeaderedItemsControl.Header"/>.
		/// If contains '\0' character, uses text before it for label and text after it for <see cref="MenuItem.InputGestureText"/>; example: "Text\0" + "Ctrl+E".
		/// </param>
		/// <param name="click">Action called on click.</param>
		/// <param name="icon">See <see cref="this[string, bool, object, string, int]"/>.</param>
		/// <param name="f">[CallerFilePath]</param>
		/// <param name="l">[CallerLineNumber]</param>
		/// <remarks>
		/// Usually it's easier to use the indexer instead. It just calls this function. See example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// m["Example"] = o => AOutput.Write(o);
		/// m.Last.IsChecked=true;
		/// ]]></code>
		/// </example>
		public MenuItem Add(object text, Action<CMActionArgs> click = null, object icon = null, [CallerFilePath] string f = null, [CallerLineNumber] int l = 0) {
			string gest = null;
			if (text is string s && s.Contains('\0')) {
				int j = s.IndexOf('\0');
				text = s[0..j];
				gest = s[++j..];
			}
			var item = new _MenuItem(this) {
				action = click,
				sourceFile = f,
				sourceLine = l,
				exceptOpt = ExceptionHandling,
				startThread = ActionThread,
				Header = text
			};
			if (gest != null) item.InputGestureText = gest;
			item.Icon = MenuItemIcon_(icon, click, ExtractIconPathFromCode);
			CurrentAddMenu.Items.Add(Last = item);
			ItemAdded?.Invoke(item);
			return item;
		}

		/// <summary>
		/// Creates new <see cref="MenuItem"/> and adds to the menu.
		/// </summary>
		/// <param name="text">
		/// Label. See <see cref="HeaderedItemsControl.Header"/>.
		/// If contains '\0' character, uses text before it for label and text after it for <see cref="MenuItem.InputGestureText"/>; example: "Text\0" + "Ctrl+E".
		/// </param>
		/// <param name="enabled">Disabled if false. Default true.</param>
		/// <param name="icon">
		/// Can be:
		/// - <see cref="Image"/> or other WPF control to assign directly to <see cref="MenuItem.Icon"/>.
		/// - string - image file path, or resource path that starts with "resources/" or has prefix "resource:", or png image as Base-64 string with prefix "image:". Supports environment variables. If not full path, looks in <see cref="AFolders.ThisAppImages"/>.
		/// - <see cref="Uri"/> - image file path, or resource pack URI, or URL. Does not support environment variables and <see cref="AFolders.ThisAppImages"/>.
		/// - <see cref="AIcon"/> - icon handle. Example: <c>AIcon.Stock(StockIcon.DELETE)]</c>. This function disposes it.
		/// - <b>IntPtr</b> - icon handle. This function does not dispose it. You can dispose at any time.
		/// - <see cref="ImageSource"/> - a WPF image.
		/// 
		/// Prints warning if failed to find or load image file.
		/// To create Base-64 string, use menu Code -> AWinImage.
		/// To add resource in Visual Studio, use build action "Resource".
		/// </param>
		/// <param name="f">[CallerFilePath]</param>
		/// <param name="l">[CallerLineNumber]</param>
		/// <value>Action called on click.</value>
		/// <remarks>
		/// Calls <see cref="Add(object, Action{CMActionArgs}, object, string, int)"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// m["Example"] = o => AOutput.Write(o);
		/// m.Last.IsChecked=true;
		/// ]]></code>
		/// </example>
		public Action<CMActionArgs> this[string text, bool enabled = true, object icon = null, [CallerFilePath] string f = null, [CallerLineNumber] int l = 0] {
			set {
				var v = Add(text, value, icon, f, l);
				if (!enabled) v.IsEnabled = false;
			}
		}

		/// <summary>
		/// Adds separator.
		/// </summary>
		public void Separator() { CurrentAddMenu.Items.Add(new Separator()); }

		/// <summary>
		/// Creates new <see cref="MenuItem"/> for a submenu and adds to the menu.
		/// </summary>
		/// <param name="text">Label. See <see cref="HeaderedItemsControl.Header"/>.</param>
		/// <param name="icon"><see cref="MenuItem.Icon"/>.</param>
		/// <param name="click">Action called on click. Rarely used.</param>
		/// <param name="f">[CallerFilePath]</param>
		/// <param name="l">[CallerLineNumber]</param>
		/// <remarks>
		/// Then the add-item functions will add items to the submenu, until the returned variable is disposed.
		/// </remarks>
		/// <example><see cref="AWpfMenu"/></example>
		public UsingEndAction Submenu(object text, object icon = null, Action<CMActionArgs> click = null, [CallerFilePath] string f = null, [CallerLineNumber] int l = 0) {
			var mi = Add(text, click, icon, f, l);
			_submenuStack.Push(mi);
			return new UsingEndAction(() => _submenuStack.Pop());
			//CONSIDER: copy some properties of current menu. Or maybe WPF copies automatically, need to test.
		}

		Stack<MenuItem> _submenuStack = new Stack<MenuItem>();
		//	bool _AddingSubmenuItems => _submenuStack.Count > 0;

		/// <summary>
		/// Gets <see cref="ItemsControl"/> of the menu or submenu where new items currently would be added.
		/// </summary>
		public ItemsControl CurrentAddMenu => _submenuStack.Count > 0 ? _submenuStack.Peek() : (ItemsControl)this;

		/// <summary>
		/// Gets the last added <see cref="MenuItem"/>.
		/// </summary>
		public MenuItem Last { get; private set; }

		/// <summary>
		/// Called when added a non-separator item.
		/// </summary>
		public Action<MenuItem> ItemAdded { get; set; }

		/// <summary>
		/// On menu item right-click open the source file and line in editor, if possible.
		/// Recommended for automation scripts.
		/// </summary>
		public bool CanEditScript { get; set; }

		/// <summary>
		/// Execute item actions asynchronously in new threads.
		/// Applied to menu items added afterwards.
		/// </summary>
		/// <remarks>
		/// If current thread is a UI thread (has windows etc) or has triggers or hooks, and item action functions execute some long automations etc in current thread, current thread probably is hung during that time. Set this property = true to avoid it.
		/// </remarks>
		public bool ActionThread { get; set; }

		/// <summary>
		/// Whether/how to handle exceptions in menu item action code. Default: <b>Warning</b>.
		/// Applied to menu items added afterwards.
		/// </summary>
		public CMExceptions ExceptionHandling { get; set; }

		/// <summary>
		/// Sets <see cref="ContextMenu.PlacementTarget"/> = <i>owner</i> and <see cref="ContextMenu.IsOpen"/> = true.
		/// </summary>
		/// <param name="owner"><see cref="ContextMenu.PlacementTarget"/>. The menu uses its DPI. If null, uses DPI of primary screen (WPF bug).</param>
		/// <param name="byCaret">Show by caret (text cursor) position if possible.</param>
		/// <param name="modal">Wait until closed.</param>
		public void Show(UIElement owner, bool byCaret = false, bool modal = false) {
			if (byCaret && AKeys.More.GetTextCursorRect(out RECT cr, out _)) {
				var r = owner == null ? cr : new Rect(owner.PointFromScreen(new Point(cr.left, cr.top)), owner.PointFromScreen(new Point(cr.right, cr.bottom)));
				r.Inflate(30, 2);
				PlacementRectangle = r;
				Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			}
			PlacementTarget = owner;
			IsOpen = true;
			if (modal) {
#if REJECTED
				_dispFrame = new DispatcherFrame();
				Dispatcher.PushFrame(_dispFrame);
#else
				AHookWin kh = null, mh = null;
				var wa = AWnd.ThisThread.Active;
				bool wpfInactive = wa.Is0 || null == HwndSource.FromHwnd(wa.Handle);
				if (wpfInactive)
					kh = AHookWin.Keyboard(k => {
						if (k.IsUp) return;
						//how to enable standard keyboard navigation?
						//			AOutput.Write(k.Key);
						//			switch(k.Key) {
						//			case KKey.Escape: case KKey.Down: case KKey.Up: case KKey.Right: case KKey.Left: case KKey.End: case KKey.Home: case KKey.Enter:
						//				AWnd.More.PostThreadMessage(AThread.NativeId, 0x100, (int)k.Key, 0); //WM_KEYDOWN //does not work, althoug works for an active WPF window.
						//				//var key = Key.Down; RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, key) { RoutedEvent = Keyboard.KeyDownEvent }); //does not work
						//				k.BlockEvent();
						//				break;
						//			}
						switch (k.Key) {
						case KKey.Escape:
							IsOpen = false;
							k.BlockEvent();
							break;
						}
					});
				_mouseWasNear = false;
				if (wpfInactive || MouseClosingDistance > 0)
					mh = AHookWin.Mouse(k => {
						if (k.IsButtonDown && wpfInactive) {
							var w = this.Hwnd();
							var z = AWnd.FromMouse(WXYFlags.NeedWindow);
							if (z == w) return;
							if (z.IsOfThisThread && z.ZorderIsAbove(w)) return; //eg a submenu
							IsOpen = false;
						} else if (k.IsMove && MouseClosingDistance > 0) {
							var w = this.Hwnd(); if (!w.IsAlive) return;
							var r = w.Rect;
							foreach (var t in AWnd.GetWnd.ThreadWindows(AThread.NativeId, true)) if (t.ZorderIsAbove(w)) r.Union(t.Rect); //submenus etc
							var p = AMouse.XY;
							int d = (int)AMath.Distance(r, p), d2 = ADpi.Scale(MouseClosingDistance, w);
							if (!_mouseWasNear) _mouseWasNear = d <= d2 / 2; else if (d > d2) IsOpen = false;
						}
					});
				_dispFrame = new DispatcherFrame();
				try { Dispatcher.PushFrame(_dispFrame); }
				finally {
					kh?.Dispose();
					mh?.Dispose();
				}
#endif
			}
		}
		DispatcherFrame _dispFrame;
#if !REJECTED
		/// <remarks>
		/// Use this function when <see cref="ContextMenu.IsOpen"/> would not work well, for example if this thread does not have an active WPF window. Else use the standard ways - set <b>IsOpen</b> = true or assign this object to a WPF control.
		/// </remarks>
		bool _mouseWasNear;
#endif

		void _EndModal() {
			if (_dispFrame != null) {
				_dispFrame.Continue = false;
				_dispFrame = null;
			}
		}

		///
		protected override void OnClosed(RoutedEventArgs e) {
			_EndModal();
			base.OnClosed(e);
		}

#if !REJECTED
		/// <summary>
		/// Let <see cref="Show"/> close the menu when the mouse cursor moves away from it to this distance.
		/// </summary>
		/// <remarks>
		/// Default = <see cref="DefaultMouseClosingDistance"/>, default 200.
		/// At first the mouse must move at less than half of the distance.
		/// Set = 0 to disable closing.
		/// The unit is WPF logical pixels, ie for 100% DPI. For example, if the value is 200 and screen DPI is 200%, the actual distance is 400 physical pixels.
		/// </remarks>
		/// <seealso cref="DefaultMouseClosingDistance"/>
		public int MouseClosingDistance { get; set; } = DefaultMouseClosingDistance;

		/// <summary>
		/// Default <see cref="MouseClosingDistance"/> value. Default 200.
		/// </summary>
		public static int DefaultMouseClosingDistance { get; set; } = 200;
#endif

		/// <summary>
		/// When adding items without explicitly specified icon, extract icon from item action code.
		/// </summary>
		/// <remarks>
		/// This property is applied to items added afterwards.
		/// </remarks>
		public bool ExtractIconPathFromCode { get; set; }

		/// <summary>
		/// Gets icon path from code that contains string like <c>@"c:\windows\system32\notepad.exe"</c> or <c>@"%AFolders.System%\notepad.exe"</c> or URL/shell.
		/// Also supports code patterns like 'AFolders.System + "notepad.exe"' or 'AFolders.Virtual.RecycleBin'.
		/// Returns null if no such string/pattern.
		/// </summary>
		internal static string IconPathFromCode_(MethodInfo mi) {
			//support code pattern like 'AFolders.System + "notepad.exe"'.
			//	Opcodes: call(AFolders.System), ldstr("notepad.exe"), FolderPath.op_Addition.
			//also code pattern like 'AFolders.System' or 'AFolders.Virtual.RecycleBin'.
			//	Opcodes: call(AFolders.System), FolderPath.op_Implicit(FolderPath to string).
			//also code pattern like 'AFile.TryRun("notepad.exe")'.
			//AOutput.Write(mi.Name);
			int i = 0, patternStart = -1; MethodInfo f1 = null; string filename = null, filename2 = null;
			try {
				var reader = new ILReader(mi);
				foreach (var instruction in reader.Instructions) {
					if (++i > 100) break;
					var op = instruction.Op;
					//AOutput.Write(op);
					if (op == OpCodes.Nop) {
						i--;
					} else if (op == OpCodes.Ldstr) {
						var s = instruction.Data as string;
						//AOutput.Write(s);
						if (i == patternStart + 1) filename = s;
						else {
							if (APath.IsFullPathExpandEnvVar(ref s)) return s; //eg AFile.TryRun(@"%AFolders.System%\notepad.exe");
							if (APath.IsUrl(s) || APath.IsShellPath_(s)) return s;
							filename = null; patternStart = -1;
							if (i == 1) filename2 = s;
						}
					} else if (op == OpCodes.Call && instruction.Data is MethodInfo f && f.IsStatic) {
						//AOutput.Write(f, f.DeclaringType, f.Name, f.MemberType, f.ReturnType, f.GetParameters().Length);
						var dt = f.DeclaringType;
						if (dt == typeof(AFolders) || dt == typeof(AFolders.Virtual)) {
							if (f.ReturnType == typeof(FolderPath) && f.GetParameters().Length == 0) {
								//AOutput.Write(1);
								f1 = f;
								patternStart = i;
							}
						} else if (dt == typeof(FolderPath)) {
							if (i == patternStart + 2 && f.Name == "op_Addition") {
								//AOutput.Write(2);
								var fp = (FolderPath)f1.Invoke(null, null);
								if ((string)fp == null) return null;
								return fp + filename;
							} else if (i == patternStart + 1 && f.Name == "op_Implicit" && f.ReturnType == typeof(string)) {
								//AOutput.Write(3);
								return (FolderPath)f1.Invoke(null, null);
							}
						}
					}
				}
				if (filename2 != null && filename2.Ends(".exe", true)) return AFile.SearchPath(filename2);
			}
			catch (Exception ex) { ADebug.Print(ex); }
			return null;
		}

		internal static object MenuItemIcon_(object icon, Delegate click, bool extractFromCode) {
			if (icon == null && extractFromCode && click != null) {
				var path = IconPathFromCode_(click.Method);
				if (path != null) icon = AIcon.OfFile(path, 16, IconGetFlags.DontSearch);
			}
			if (icon == null) return null;
			try {
				ImageSource iso = null; bool other = false;
				switch (icon) {
				case string s:
					iso = AImageUtil.LoadWpfImageFromFileOrResourceOrString(s);
					break;
				case Uri s:
					iso = BitmapFrame.Create(s);
					break;
				case AIcon h:
					iso = h.ToWpfImage();
					break;
				case IntPtr h:
					iso = new AIcon(h).ToWpfImage(false);
					break;
				case ImageSource s:
					iso = s;
					break;
				default:
					other = true;
					break;
				}
				if (iso != null) icon = new Image { Source = iso };
				else if (!other) icon = null;
			}
			catch (Exception ex) { AWarning.Write(ex.ToStringWithoutStack()); }
			return icon;
			//TODO: support xaml and cache
		}

		/// <summary>
		/// Creates and shows popup menu where items use ids instead of actions.
		/// Returns selected item id, or 0 if cancelled.
		/// </summary>
		/// <param name="items">
		/// Menu items. Can be string[], List&lt;string&gt; or string like "One|Two|Three".
		/// Item id can be optionally specified like "1 One|2 Two|3 Three". If missing, uses id of previous non-separator item + 1. Example: "One|Two|100 Three Four" //1|2|100|101.
		/// For separators use null or empty strings: "One|Two||Three|Four".
		/// </param>
		/// <param name="owner"><see cref="ContextMenu.PlacementTarget"/>. The menu uses its DPI. If null, uses DPI of primary screen (WPF bug).</param>
		/// <param name="byCaret">Show by caret (text cursor) position if possible.</param>
		/// <param name="beforeShow">Called after adding menu items, before showing the menu. For example can set placement properties.</param>
		/// <remarks>
		/// The menu is modal; the function returns when closed.
		/// </remarks>
		/// <seealso cref="ADialog.ShowList"/>
		public static int ShowSimple(DStringList items, UIElement owner, bool byCaret = false, Action<AWpfMenu> beforeShow = null) {
			var a = items.ToArray();
			var m = new AWpfMenu();
			//	var dispFrame = new DispatcherFrame();
			//	m.Closed+=(_,_)=>dispFrame.Continue=false;
			int result = 0, autoId = 0;
			foreach (var v in a) {
				var s = v;
				if (s.NE()) {
					m.Separator();
				} else {
					if (s.ToInt(out int id, 0, out int end)) {
						if (s.Eq(end, ' ')) end++;
						s = s[end..];
						autoId = id;
					} else {
						id = ++autoId;
					}
					m.Add(s, o => result = (int)o.Item.Tag).Tag = id;
					//			m.Add(s, o => {
					//				result = (int)o.Item.Tag;
					//				dispFrame.Continue=false;
					//			}).Tag = id;
				}
			}
			beforeShow?.Invoke(m);
			m.Show(owner, byCaret: byCaret, modal: true);
			//	m.IsOpen=true;
			//	Dispatcher.PushFrame(dispFrame);
			return result;
		}

		class _MenuItem : MenuItem
		{
			AWpfMenu _m;
			public Action<CMActionArgs> action;
			public CMExceptions exceptOpt;
			public bool startThread;
			public string sourceFile;
			public int sourceLine;

			public _MenuItem(AWpfMenu m) { _m = m; }

			protected override void OnClick() {
				_m._EndModal(); //workaround for: OnClosed called with 160 ms delay. Same with native message loop.
				if (action != null) {
					if (startThread) AThread.Start(() => _ExecItem(), background: false); else _ExecItem();
					void _ExecItem() {
						try {
							action(new CMActionArgs(this));
						}
						catch (Exception ex) when (exceptOpt != CMExceptions.Exception) {
							if (exceptOpt == CMExceptions.Warning) AWarning.Write(ex.ToString(), -1);
						}
					}
				}
				base.OnClick();
			}

			protected override void OnPreviewMouseUp(MouseButtonEventArgs e) {
				switch (e.ChangedButton) {
				case MouseButton.Right:
					if (this.HasItems && e.Source != this) break; //workaround for: cannot edit submenu items because then this func at first called for parent item
					e.Handled = true;
					_m.IsOpen = false;
					if (_m.CanEditScript && !sourceFile.NE()) AScriptEditor.GoToEdit(sourceFile, sourceLine);
					//could instead use a AWpfMenu here, but: dangerous; closes this menu before showing it.
					break;
				case MouseButton.Middle:
					_m.IsOpen = false;
					break;
				}
				base.OnPreviewMouseUp(e);
			}
		}
	}
}

namespace Au.Types
{

	/// <summary>
	/// Used with <see cref="AWpfMenu.ExceptionHandling"/>;
	/// </summary>
	public enum CMExceptions
	{
		/// <summary>Handle exceptions. On exception call <see cref="AWarning.Write"/>. This is default.</summary>
		Warning,

		/// <summary>Don't handle exceptions.</summary>
		Exception,

		/// <summary>Handle exceptions. On exception do nothing.</summary>
		Silent,
	}

	/// <summary>
	/// Arguments for <see cref="AWpfMenu"/> item actions.
	/// </summary>
	public class CMActionArgs
	{
		///
		public CMActionArgs(MenuItem item) { Item = item; }

		/// <summary>
		/// The menu item object.
		/// If <see cref="AWpfMenu.ActionThread"/> true, it cannot be used directly. A workaround is <c>o.Item.Dispatcher.Invoke(()=>...);</c>.
		/// </summary>
		public MenuItem Item { get; }

		///
		public override string ToString() {
			var d = Item.Dispatcher;
			if (d.Thread == Thread.CurrentThread) return Item.Header.ToString();
			return d.Invoke(() => Item.Header.ToString());
		}
	}
}