using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using System.Windows.Media;
using System.Diagnostics;

namespace Au.Controls
{
	/// <summary>
	/// Builds a WPF window menu with submenus and items that execute static methods defined in a class and nested classes.
	/// Supports xaml/png/etc images, key/mouse shortcuts, auto-Alt-underline, easy creating of toolbar buttons and context menus with same/synchronized properties (command, text, image, enabled, checked, etc).
	/// </summary>
	/// <remarks>
	/// Creates submenus from public static nested types with <see cref="CommandAttribute"/>. Creates executable menu items from public static methods with <see cref="CommandAttribute"/>.
	/// From each such type and method creates a <see cref="Command"/> object that you can access through indexer.
	/// Supports methods <c>public static void Method()</c> and <c>public static void Method(object)</c>.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// var cmd=new AuMenuCommands(typeof(Commands), menu);
	/// cmd[nameof(Commands.Edit.Paste)].Enabled = false;
	/// cmd[nameof(Commands.File.Rename)].SetKeys("F12", _window);
	/// ]]></code>
	/// 
	/// <code><![CDATA[
	/// static class Commands {
	/// 	[Command('F')]
	/// 	public static class File {
	/// 		[Command('R')]
	/// 		public static void Rename() {  }
	/// 		
	/// 		[Command('D')]
	/// 		public static void Delete() {  }
	/// 		
	/// 		[Command("_Properties...", image = "properties.xaml")]
	/// 		public static void Properties() {  }
	/// 		
	/// 		[Command('N')]
	/// 		public static class New {
	/// 			[Command('D')]
	/// 			public static void Document() {  }
	/// 			
	/// 			[Command('F')]
	/// 			public static void Folder() {  }
	/// 		}
	/// 		
	/// 		[Command('x', separator = true)]
	/// 		public static void Exit(object param) {  }
	/// 	}
	/// 	
	/// 	[Command('E')]
	/// 	public static class Edit {
	/// 		[Command('t')]
	/// 		public static void Cut() {  }
	/// 		
	/// 		[Command('C')]
	/// 		public static void Copy() {  }
	/// 		
	/// 		[Command('P')]
	/// 		public static void Paste() {  }
	/// 		
	/// 		[Command('D', name = "Edit-Delete")]
	/// 		public static void Delete() {  }
	/// 		
	/// 		[Command('a')]
	/// 		public static void Select_all() {  }
	/// 	}	
	/// }
	/// ]]></code>
	/// </example>
	public class AuMenuCommands
	{
		readonly Dictionary<string, Command> _d = new(200);
		readonly Action<FactoryParams> _itemFactory;
		readonly bool _autoUnderline;
		string _defaultFile, _customizedFile;

		/// <summary>
		/// Builds a WPF window menu with submenus and items that execute static methods defined in a class and nested classes.
		/// See example in class help.
		/// </summary>
		/// <param name="commands">A type that contains nested types with methods. Must be in single source file (not partial class).</param>
		/// <param name="menu">An empty <b>Menu</b> object. This function adds items to it.</param>
		/// <param name="autoUnderline">Automatically insert _ in item text for Alt-underlining where not specified explicitly.</param>
		/// <param name="itemFactory">Optional callback function that is called for each menu item. Can create menu items, set properties, create toolbar buttons, etc.</param>
		/// <exception cref="ArgumentException">Duplicate name. Use <see cref="CommandAttribute.name"/>.</exception>
		public AuMenuCommands(Type commands, Menu menu, bool autoUnderline = true, Action<FactoryParams> itemFactory = null) {
			if (commands == null || menu == null) throw new ArgumentNullException();
			_itemFactory = itemFactory;
			_autoUnderline = autoUnderline;

			_Menu(commands, menu, null);

			void _Menu(Type type, ItemsControl parentMenu, string inheritTarget) {
				var am = type.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

				if (am.Length == 0) { //dynamic submenu
					parentMenu.Items.Add(new Separator());
					return;
				}

				var list = new List<(MemberInfo mi, CommandAttribute a)>(am.Length);
				foreach (var mi in am) {
					var ca = mi.GetCustomAttribute<CommandAttribute>(false);
					//var ca = mi.GetCustomAttributes().OfType<CommandAttribute>().FirstOrDefault(); //CommandAttribute and inherited. Similar speed. Don't need because factory action receives MemberInfo an can get other attributes from it.
					if (ca != null) list.Add((mi, ca));
				}

				var au = new List<char>();

				foreach (var (mi, ca) in list.OrderBy(o => o.a.order_)) {
					string name = ca.name ?? mi.Name;
					var c = new Command(name, mi, ca);
					_d.Add(name, c);

					if (ca.separator && !ca.hide) parentMenu.Items.Add(new Separator());

					ca.target ??= inheritTarget;

					string text = ca.text, dots = null; //menu item text, possibly with _ for Alt-underline
					if (text == "...") { dots = text; text = null; }
					if (text != null) {
						c.ButtonText = AStringUtil.RemoveUnderlineChar(text, '_');
					} else {
						text = mi.Name.Replace('_', ' ') + dots;
						c.ButtonText = text;
						char u = ca.underlined;
						if (u != default) {
							int i = text.IndexOf(u);
							if (i >= 0) text = text.Insert(i, "_"); else AOutput.Write($"Alt-underline character '{u}' not found in \"{text}\"");
						}
					}
					c.ButtonTooltip = ca.tooltip;

					FactoryParams f = null;
					if (_itemFactory != null) {
						f = new FactoryParams(c, mi) { text = text, image = ca.image, param = ca.param };
						_itemFactory(f);
						if (c.MenuItem == null) c.SetMenuItem_(f.text, f.image); //did not call SetMenuItem
					} else {
						if (c.MenuItem == null) c.SetMenuItem_(text, ca.image);
					}
					if (!ca.keysText.NE()) c.MenuItem.InputGestureText = ca.keysText;
					if (_autoUnderline && c.MenuItem.Header is string s && _FindUnderlined(s, out char uc)) au.Add(char.ToLower(uc));
					if (ca.checkable) c.MenuItem.IsCheckable = true;

					if (!ca.hide) parentMenu.Items.Add(c.MenuItem);
					if (mi is TypeInfo ti) _Menu(ti, c.MenuItem, ca.target);
				}

				if (_autoUnderline) {
					foreach (var v in parentMenu.Items) {
						if (v is MenuItem m && m.Header is string s && s.Length > 0 && !_FindUnderlined(s, out _)) {
							int i = 0;
							for (; i < s.Length; i++) {
								char ch = s[i]; if (!char.IsLetterOrDigit(ch)) continue;
								ch = char.ToLower(ch);
								if (!au.Contains(ch)) { au.Add(ch); break; }
							}
							if (i == s.Length) i = 0;
							m.Header = s.Insert(i, "_");
						}
					}
				}

				static bool _FindUnderlined(string s, out char u) {
					u = default;
					int i = 0;
					g1: i = s.IndexOf('_', i) + 1;
					if (i == 0 || i == s.Length) return false;
					u = s[i++];
					if (u == '_') goto g1;
					return true;
				}
			}

			menu.ContextMenuOpening += _ContextMenu;
		}

		/// <summary>
		/// Gets a <b>Command</b> by name.
		/// </summary>
		/// <param name="command">Method name, for example "Select_all". Or nested type name if it's a submenu-item.</param>
		/// <exception cref="KeyNotFoundException"></exception>
		public Command this[string command] => _d[command];

		/// <summary>
		/// Tries to find a <b>Command</b> by name. Returns false if not found.
		/// Same as the indexer, but does not throw exception when not found.
		/// </summary>
		/// <param name="command">Method name, for example "Select_all". Or nested type name if it's a submenu-item.</param>
		/// <param name="c"></param>
		public bool TryFind(string command, out Command c) => _d.TryGetValue(command, out c);

		/// <summary>
		/// Adds to <i>target</i>'s <b>InputBindings</b> all keys etc where <b>CommandAttribute.target</b> == <i>name</i>.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="name"></param>
		public void BindKeysTarget(UIElement target, string name) {
			//AOutput.Write($"---- {name} = {target}");
			foreach (var c in _d.Values) {
				var ca = c.Attribute;
				var keys = ca.keys;
				if (keys != null && ca.target == name) {
					//AOutput.Write(c, keys);
					int i = keys.IndexOf(", ");
					if (i < 0) _Add(keys); else foreach (var v in keys.Split(", ")) _Add(v);
					void _Add(string s) {
						if (!AKeys.More.ParseHotkeyString(s, out var mod, out var key, out var mouse)) {
							AWarning.Write("Invalid key or mouse shortcut: " + s);
							return;
						}
						if (key != default) target.InputBindings.Add(new KeyBinding(c, key, mod));
						else if (target is System.Windows.Interop.HwndHost) AWarning.Write(s + ": mouse shortcuts don't work with HwndHost controls");
						else target.InputBindings.Add(new MouseBinding(c, new MouseGesture(mouse, mod)));

						//FUTURE: support mouse shortcuts in HwndHost
						//if (target is System.Windows.Interop.HwndHost hh) {
						//	hh.MessageHook += _Hh_MessageHook;
						//	//or use native mouse hook
						//} else {
						//	target.InputBindings.Add(new MouseBinding(this, new MouseGesture(mouse, mod)));
						//}
					}
					var mi = c.MenuItem;
					var s = mi.InputGestureText;
					if (s.NE()) s = keys; else s = s + ", " + keys;
					mi.InputGestureText = s;
				}
			}
		}

		/// <summary>
		/// Contains a method delegate and a menu item that executes it. Implements <see cref="ICommand"/> and can have one or more attached buttons etc and key/mouse shortcuts that execute it. All can be disabled/enabled with single function call.
		/// Also used for submenu-items (created from nested types); it allows for example to enable/disable all descendants with single function call.
		/// </summary>
		public class Command : ICommand
		{
			readonly Delegate _del; //null if submenu
			readonly CommandAttribute _ca;
			MenuItem _mi;
			bool _enabled;

			internal Command(string name, MemberInfo mi, CommandAttribute ca) {
				_enabled = true;
				Name = name;
				_ca = ca;
				if (mi is MethodInfo k) _del = k.CreateDelegate(k.GetParameters().Length == 0 ? typeof(Action) : typeof(Action<object>));
			}

			internal void SetMenuItem_(object text, string image, MenuItem miFactory = null) {
				_mi = miFactory ?? new MenuItem { Header = text }; //factory action may have set it
				_mi.Command = this;
				if (image != null && miFactory?.Icon == null) _SetImage(image);
			}

			MenuItem _Mi => _mi ?? throw new InvalidOperationException("Call FactoryParams.SetMenuItem before.");

			///
			public MenuItem MenuItem => _mi;

			/// <summary>
			/// true if this is a submenu-item.
			/// </summary>
			public bool IsSubmenu => _del == null;

			/// <summary>
			/// Method name. If submenu-item - type name. Or <see cref="CommandAttribute.name"/>.
			/// </summary>
			public string Name { get; internal set; }

			///
			public override string ToString() => Name;

			public CommandAttribute Attribute => _ca;

			/// <summary>
			/// Button text or tooltip. Same as menu item text but without _ for Alt-underline.
			/// </summary>
			public string ButtonText { get; set; }

			/// <summary>
			/// <see cref="CommandAttribute.tooltip"/>.
			/// </summary>
			public string ButtonTooltip { get; set; }

			/// <summary>
			/// Setter subscribes to <see cref="MenuItem.SubmenuOpened"/> event.
			/// Will propagate to copied submenus.
			/// Call once.
			/// </summary>
			public RoutedEventHandler SubmenuOpened {
				get => _submenuOpened;
				set {
					Debug.Assert(_submenuOpened == null);
					_Mi.SubmenuOpened += _submenuOpened = value;
				}
			}
			RoutedEventHandler _submenuOpened;

			/// <summary>
			/// Something to attach to this object. Not used by this class.
			/// </summary>
			public object Tag { get; set; }

			/// <summary>
			/// Sets properties of a button to match properties of this menu item.
			/// </summary>
			/// <param name="b">Button or checkbox etc.</param>
			/// <param name="imageAt">If menu item has image, set <b>Content</b> = <b>DockPanel</b> with image and text and dock image at this side. If null (default), sets image without text. Not used if there is no image.</param>
			/// <param name="image">Button image element, if different than menu item image. Must not be a child of something.</param>
			/// <param name="text">Button text, if different than menu item text.</param>
			/// <param name="skipImage">Don't change image.</param>
			/// <exception cref="InvalidOperationException">This is a submenu. Or called from factory action before <see cref="FactoryParams.SetMenuItem"/>.</exception>
			/// <remarks>
			/// Sets these properties:
			/// - <b>Content</b> (image or/and text),
			/// - <b>ToolTip</b>,
			/// - <b>Foreground</b>,
			/// - <b>Command</b> (to execute same method and automatically enable/disable together),
			/// - Automation Name (if with image),
			/// - if checkable, synchronizes checked state (the button should be a ToggleButton (CheckBox or RadioButton)).
			/// </remarks>
			public void CopyToButton(ButtonBase b, Dock? imageAt = null, UIElement image = null, string text = null, bool skipImage = false) {
				if (IsSubmenu) throw new InvalidOperationException("Submenu. Use CopyToMenu.");
				_ = _Mi;
				text ??= ButtonText;

				if (skipImage) {
					switch (b.Content) {
					case DockPanel dp: image = dp.Children[0]; dp.Children.Clear(); break;
					case UIElement ue: image = ue; break;
					default: image = null; break;
					}
					b.Content = null;
				} else {
					image ??= CopyImage();
				}

				if (image == null) {
					b.Content = text;
					b.Padding = new Thickness(4, 1, 4, 2);
					b.ToolTip = ButtonTooltip;
				} else if (imageAt != null) {
					var v = new DockPanel();
					var dock = imageAt.Value;
					DockPanel.SetDock(image, dock);
					v.Children.Add(image);
					var t = new TextBlock { Text = text };
					if (dock == Dock.Left || dock == Dock.Right) t.Margin = new Thickness(2, -1, 2, 1);
					v.Children.Add(t);
					b.Content = v;
					b.ToolTip = ButtonTooltip;
				} else { //only image
					b.Content = image;
					b.ToolTip = ButtonTooltip ?? text;
				}
				b.Foreground = _mi.Foreground;
				if (image != null && !text.NE()) System.Windows.Automation.AutomationProperties.SetName(b, text);
				b.Command = this;

				if (_mi.IsCheckable) {
					if (b is ToggleButton tb) {
						_mi.Checked += (_, _) => tb.IsChecked = true;
						_mi.Unchecked += (_, _) => tb.IsChecked = false;
						tb.Checked += (_, _) => _mi.IsChecked = true;
						tb.Unchecked += (_, _) => _mi.IsChecked = false;
					} else AWarning.Write($"Menu item {Name} is checkable, but button isn't a ToggleButton (CheckBox or RadioButton).");
				}
			}

			//public void CopyToButton<T>(out T b, Dock? imageAt = null) where T : ButtonBase, new() => CopyToButton(b = new T(), imageAt);

			/// <summary>
			/// Sets properties of another menu item (not in this menu) to match properties of this menu item.
			/// If this is a submenu-item, copies with descendants.
			/// </summary>
			/// <param name="m"></param>
			/// <param name="image">Image element (<see cref="MenuItem.Icon"/>), if different. Must not be a child of something.</param>
			/// <param name="text">Text (<see cref="HeaderedItemsControl.Header"/>), if different.</param>
			/// <exception cref="InvalidOperationException">Called from factory action before <see cref="FactoryParams.SetMenuItem"/>.</exception>
			/// <remarks>
			/// Sets these properties:
			/// - <b>Header</b> (if string),
			/// - <b>Icon</b> (if possible),
			/// - <b>InputGestureText</b>,
			/// - <b>ToolTip</b>,
			/// - <b>Foreground</b>,
			/// - <b>Command</b> (to execute same method and automatically enable/disable together),
			/// - <b>IsCheckable</b> (and synchronizes checked state).
			/// </remarks>
			public void CopyToMenu(MenuItem m, UIElement image = null, object text = null) => _CopyToMenu(_Mi, m, image, text);

			static MenuItem _CopyToMenu(MenuItem from, MenuItem to, UIElement image = null, object text = null) {
				if (from.Command is not Command c) return null;
				to ??= new();

				to.Icon = image ?? _CopyImage(from);
				if (text != null) to.Header = text; else if (from.Header is string s) to.Header = s;
				to.InputGestureText = from.InputGestureText;
				to.ToolTip = from.ToolTip;
				to.Foreground = from.Foreground;
				to.Command = c;

				bool checkable = from.IsCheckable;
				to.IsCheckable = checkable;
				if (checkable) {
					if (from.IsChecked) to.IsChecked = true;
					RoutedEventHandler eh1, eh2;
					from.Checked += eh1 = (_, _) => to.IsChecked = true;
					from.Unchecked += eh2 = (_, _) => to.IsChecked = false;
					to.Unloaded += (_, _) => { from.Checked -= eh1; from.Unchecked -= eh2; }; //to avoid memory leak when temp context menu closed
					to.Checked += (_, _) => from.IsChecked = true;
					to.Unchecked += (_, _) => from.IsChecked = false;
				}

				if (from.HasItems) {
					if (c._submenuOpened != null) to.SubmenuOpened += c._submenuOpened;
					_CopyDescendants(from, to);
				}

				return to;
			}

			static void _CopyDescendants(ItemsControl from, ItemsControl to) {
				int n = 0;
				foreach (var v in from.Items) {
					object k;
					switch (v) {
					case Separator:
						k = new Separator();
						break;
					case MenuItem g:
						k = _CopyToMenu(g, null);
						if (k == null) { //not Command. Added dynamically. Will add again.
							if (n == 0) to.Items.Add(new Separator()); //let it be submenu
							return;
						}
						break;
					default: continue;
					}
					to.Items.Add(k);
					n++;
				}
			}

			/// <summary>
			/// Copies descendants of this submenu to a context menu.
			/// </summary>
			/// <exception cref="InvalidOperationException">This is not a submenu. Or called from factory action before <see cref="FactoryParams.SetMenuItem"/>.</exception>
			/// <remarks>
			/// For each new item sets the same properties as other overload.
			/// </remarks>
			public void CopyToMenu(ContextMenu cm) {
				if (!IsSubmenu) throw new InvalidOperationException("Not submenu");
				_CopyDescendants(_Mi, cm);
			}

			/// <summary>
			/// Copies menu item image element. Returns null if no image or cannot copy.
			/// </summary>
			/// <exception cref="InvalidOperationException">Called from factory action before <see cref="FactoryParams.SetMenuItem"/>.</exception>
			public UIElement CopyImage() => _CopyImage(_Mi);

			static UIElement _CopyImage(MenuItem from) {
				switch (from.Icon) {
				case Image im: return new Image { Source = im.Source };
				case UIElement e when e.Uid is string res: //see _SetImage
					if (AResources.HasResourcePrefix(res)) return AResources.GetXamlObject(res) as UIElement;
					if (res.Starts("source:")) return AImageUtil.LoadWpfImageElementFromFileOrResourceOrString(res[7..]);
					break;
				}
				return null;
			}

			bool _SetImage(string image, _CustomizeContext customizing = null) {
				bool custom = customizing != null;
				try {
					var ie = custom
						? AImageUtil.LoadWpfImageElementFromFileOrResourceOrString(image)
						: AResources.GetWpfImageElement(image);
					if (ie is not Image) ie.Uid = (custom ? "source:" : "resource:") + image; //xaml source for _CopyImage
					_mi.Icon = ie;
					return true;
				}
				catch (Exception ex) {
					if (custom) customizing.Error("failed to load image", ex);
					else AOutput.Write($"Failed to load image resource {image}. {ex.ToStringWithoutStack()}");
				}
				return false;
			}

			/// <summary>
			/// Gets or sets enabled/disabled state of this command, menu item and all controls with <b>Command</b> property = this (see <see cref="CopyToButton"/>, <see cref="CopyToMenu"/>).
			/// If submenu-item, the 'set' function also enables/disables all descendants.
			/// </summary>
			/// <exception cref="InvalidOperationException">Called from factory action before <see cref="FactoryParams.SetMenuItem"/>.</exception>
			public bool Enabled {
				get => _enabled;
				set {
					_ = _Mi;
					if (value == _enabled) return;
					_enabled = value;
					CanExecuteChanged?.Invoke(this, EventArgs.Empty); //enables/disables this menu item and all buttons etc with Command=this
					if (IsSubmenu) foreach (var v in _mi.Items) if (v is MenuItem m && m.Command is Command c) c.Enabled = value;
				}
			}

			/// <summary>
			/// Gets or sets checked state of this checkable menu item and all checkable controls with <b>Command</b> property = this (see <see cref="CopyToButton"/>, <see cref="CopyToMenu"/>).
			/// </summary>
			/// <exception cref="InvalidOperationException">Called from factory action before <see cref="FactoryParams.SetMenuItem"/>.</exception>
			public bool Checked {
				get => _Mi.IsChecked;
				set { if (value != _Mi.IsChecked) _mi.IsChecked = value; }
			}

			#region ICommand

			bool ICommand.CanExecute(object parameter) => _enabled;

			void ICommand.Execute(object parameter) {
				switch (_del) {
				case Action a0: a0(); break;
				case Action<object> a1: a1(parameter); break;
					//default: throw new InvalidOperationException("Submenu");
				}
			}

			/// <summary>
			/// When disabled or enabled with <see cref="Enabled"/>.
			/// </summary>
			public event EventHandler CanExecuteChanged;

			#endregion

			//public void Test() {
			//	foreach (var v in CanExecuteChanged.GetInvocationList()) {
			//		AOutput.Write(v.Target);
			//	}
			//}

			internal void Customize_(XElement x, ToolBar toolbar, _CustomizeContext context) {
				context.command = this;

				OverflowMode hide = default;
				bool separator = false;
				string text = null, btext = null;
				Dock? imageAt = null;

				foreach (var a in x.Attributes()) {
					string an = a.Name.LocalName, av = a.Value;
					try {
						switch (an) {
						case "keys":
							_ca.keys = av;
							break;
						case "color":
							_mi.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(av));
							break;
						case "image":
							_SetImage(av, context);
							break;
						case "text":
							_mi.Header = text = av;
							break;
						case "btext" when toolbar != null:
							btext = av;
							break;
						case "separator" when toolbar != null:
							separator = true;
							break;
						case "hide" when toolbar != null:
							hide = Enum.Parse<OverflowMode>(av, true);
							break;
						case "imageAt" when toolbar != null:
							imageAt = Enum.Parse<Dock>(av, true);
							break;
						default:
							context.Error($"attribute '{an}' can't be used here");
							break;
						}
					}
					catch (Exception ex) { context.Error($"invalid '{an}' value", ex); }
				}
				if ((btext ?? text) != null) ButtonText = btext ?? AStringUtil.RemoveUnderlineChar(text, '_');

				if (toolbar != null) {
					try {
						if (separator) toolbar.Items.Add(new Separator());
						DependencyObject o;
						if (IsSubmenu) {
							var b = new MenuItem();
							var image = _mi.Icon;
							bool onlyImage = image != null && imageAt == null;
							if (image == null || onlyImage) b.Padding = new Thickness(3, 1, 3, 2); //make taller. If image+text, button too tall, text too high, icon too low, never mind.
							CopyToMenu(b, text: btext);
							if (onlyImage) { b.Header = b.Icon; b.Icon = null; } //make narrower
							var m = new Menu { UseLayoutRounding = true };
							m.Items.Add(b); //parent must be Menu, else wrong Role (must be TopLevelHeader, we can't change) and does not work
							o = new Border { Child = m }; //workaround for: descendant icon part black when checked, with or without icon
						} else {
							var b = _mi.IsCheckable ? (ButtonBase)new CheckBox() : new Button(); //rejected: support RadioButton
							b.Focusable = false;
							b.Padding = new Thickness(4, 2, 4, 2);
							b.UseLayoutRounding = true;
							CopyToButton(b, imageAt);
							o = b;
						}
						if (hide != default) ToolBar.SetOverflowMode(o, hide);
						toolbar.Items.Add(o);
					}
					catch (Exception ex) { context.Error("failed to create button", ex); }
				}
			}
		}

		/// <summary>
		/// Adds toolbar buttons specified in <i>xmlFileCustomized</i> or <i>xmlFileDefault</i>. Applies customizations specified there.
		/// </summary>
		/// <param name="xmlFileDefault">XML file containing default toolbar buttons. See Default\Commands.xml in editor project.</param>
		/// <param name="xmlFileCustomized">XML file containing user-modified commands and toolbar buttons. Can be null.</param>
		/// <param name="toolbars">Empty toolbars where to add buttons. XML tag = <b>Name</b> property.</param>
		public void InitToolbarsAndCustomize(string xmlFileDefault, string xmlFileCustomized, ToolBar[] toolbars) {
			string xmlFile = _defaultFile = xmlFileDefault;
			_customizedFile = xmlFileCustomized;
			try {
				var a = AExtXml.LoadElem(xmlFileDefault).Elements().ToArray(); //menu and toolbars
				if (xmlFileCustomized != null && AFile.ExistsAsFile(xmlFileCustomized, true)) {
					try { //replace a elements with elements that exist in xmlFileCustomized. If some toolbar does not exist there, use default.
						var ac = AExtXml.LoadElem(xmlFileCustomized).Elements().ToArray();
						for (int i = 0; i < a.Length; i++) {
							var name = a[i].Name.LocalName;
							foreach (var y in ac) if (y.Name.LocalName == name && y.HasElements) { a[i] = y; break; }
						}
						xmlFile = _customizedFile = xmlFileCustomized;
						//FUTURE: auto-update documentation comments in xmlFileCustomized
					}
					catch (Exception ex) { AOutput.Write($"Failed to load file '{xmlFileCustomized}'. {ex.ToStringWithoutStack()}"); }
				}

				var context = new _CustomizeContext { xmlFile = xmlFile };
				foreach (var xtb in a) {
					ToolBar tb = null;
					var tbname = xtb.Name.LocalName;
					if (tbname != "menu") {
						foreach (var v in toolbars) if (v.Name == tbname) { tb = v; goto g1; }
						AOutput.Write($"{xmlFile}: unknown toolbar '{tbname}'. Toolbars: {string.Join(", ", toolbars.Select(o => o.Name))}.");
						continue;
						g1:;
					}
					foreach (var v in xtb.Elements()) {
						var name = v.Name.LocalName;
						if (_d.TryGetValue(name, out var c)) c.Customize_(v, tb, context);
						else AOutput.Write($"{xmlFile}: unknown command '{name}'. Commands: {string.Join(", ", _d.Keys)}.");
					}
				}
			}
			catch (Exception ex) { AOutput.Write($"Failed to load file '{xmlFile}'. {ex.ToStringWithoutStack()}"); }

			foreach (var tb in toolbars) {
				tb.ContextMenuOpening += _ContextMenu;
				tb.PreviewMouseRightButtonDown += (sender, e) //workaround for: on right-down closes overflow. Currently would not need, but then on right-up would open another menu etc in an unrelated element.
					=> e.Handled = _GetCommandFromMouseEventArgs(sender, e, out var command, out var control) && ToolBar.GetIsOverflowItem(control);
			}
		}

		internal class _CustomizeContext
		{
			public string xmlFile;
			public Command command;

			public void Error(string s, Exception ex = null) {
				AOutput.Write($"{xmlFile}, command {command.Name}: {s}. {ex?.ToStringWithoutStack()}");
			}
		}

		void _Customize() {
			if (!AFile.ExistsAsFile(_customizedFile, true)) AFile.Copy(_defaultFile, _customizedFile);
			AFile.SelectInExplorer(_customizedFile);
		}

		private void AProcess_Exit(object sender, EventArgs e) {
			throw new NotImplementedException();
		}

		void _ContextMenu(object sender, ContextMenuEventArgs e) {
			if (_customizedFile == null) return;
			if (_GetCommandFromMouseEventArgs(sender, e, out _, out _)) {
				e.Handled = true;
				if (sender is ToolBar tb) tb.IsOverflowOpen = false; //this was some workaround when using WPF menu, now don't know
				switch (ClassicMenu_.ShowSimple("Edit commands file|Find default commands file", sender as UIElement)) {
				case 1: _Customize(); break;
				case 2: AFile.SelectInExplorer(_defaultFile); break;
				}
			}
		}

		static bool _GetCommandFromMouseEventArgs(object sender, RoutedEventArgs e, out Command command, out Control control) {
			for (var v = e.Source as DependencyObject; v != null && v != sender; v = VisualTreeHelper.GetParent(v)) {
				if (v is ICommandSource u && u.Command is Command c) { command = c; control = (Control)v; return true; }
			}
			command = null; control = null;
			return false;
		}

		/// <summary>
		/// Parameters for factory action of <see cref="AuMenuCommands"/>.
		/// </summary>
		public class FactoryParams
		{
			internal FactoryParams(Command command, MemberInfo member) { this.command = command; this.member = member; }

			/// <summary>
			/// The new command.
			/// <see cref="Command.MenuItem"/> is still null and you can call <see cref="SetMenuItem"/>.
			/// </summary>
			public readonly Command command;

			/// <summary>
			/// <see cref="MethodInfo"/> of method or <see cref="TypeInfo"/> of nested class.
			/// For example allows to get attributes of any type.
			/// </summary>
			public readonly MemberInfo member;

			/// <summary>
			/// Text or a WPF element to add to the text part of the menu item. In/out parameter.
			/// Text may contain _ for Alt-underline, whereas <c>command.Text</c> is without it.
			/// </summary>
			public object text;

			/// <summary><see cref="CommandAttribute.image"/>. In/out parameter.</summary>
			public string image;

			/// <summary><see cref="CommandAttribute.param"/>. In/out parameter. This class does not use it.</summary>
			public object param;

			/// <summary>
			/// Sets <see cref="Command.MenuItem"/> property.
			/// If your factory action does not call this function, the menu item will be created after it returns.
			/// </summary>
			/// <param name="mi">Your created menu item. If null, this function creates standard menu item.</param>
			/// <remarks>
			/// Uses the <i>text</i> and <i>image</i> fields; you can change them before. Sets menu item's <b>Icon</b> property if image!=null and mi?.Image==null. Sets <b>Header</b> property only if creates new item.
			/// The menu item will be added to the parent menu after your factory action returns.
			/// </remarks>
			public void SetMenuItem(MenuItem mi = null) => command.SetMenuItem_(text, image, mi);
		}
	}

	/// <summary>
	/// Used with <see cref="AuMenuCommands"/>.
	/// Allows to add menu items in the same order as methods and nested types, and optionally specify menu item text etc.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CommandAttribute : Attribute
	{
		internal readonly int order_;

		/// <summary>
		/// Command name to use instead of method/type name. Use to resolve duplicate name conflict.
		/// </summary>
		public string name;

		/// <summary>
		/// Menu item text. Use _ to Alt-underline a character. If "...", appends it to default text.
		/// </summary>
		public string text;

		/// <summary>
		/// Alt-underlined character in menu item text.
		/// </summary>
		public char underlined;

		/// <summary>
		/// Add separator before the menu item.
		/// </summary>
		public bool separator;

		/// <summary>
		/// Checkable menu item.
		/// </summary>
		public bool checkable;

		/// <summary>
		/// Default hotkey etc. See <see cref="AuMenuCommands.BindKeysTarget"/>.
		/// </summary>
		public string keys;

		/// <summary>
		/// Element where the hotkey etc (default or customized) will work. See <see cref="AuMenuCommands.BindKeysTarget"/>.
		/// If this property applied to a class (submenu), all descendant commands without this property inherit it from the ancestor class.
		/// </summary>
		public string target;

		/// <summary>
		/// Text for <see cref="MenuItem.InputGestureText"/>. If not set, will use <b>keys</b>.
		/// </summary>
		public string keysText;

		/// <summary>
		/// Image string.
		/// The factory action receives this string in parameters. It can load image and set menu item's <b>Icon</b> property.
		/// If factory action not used or does not set <b>Image</b> property and does not set image=null, this class loads image from exe or script resources and sets <b>Icon</b> property. The resource file can be xaml (for example converted from svg) or png etc. If using Visual Studio, to add an image to resources set its build action = Resource. More info: <see cref="Au.Util.AResources"/>.
		/// </summary>
		public string image;

		/// <summary>
		/// Let <see cref="AuMenuCommands.Command.CopyToButton"/> use this text for tooltip.
		/// </summary>
		public string tooltip;

		/// <summary>
		/// A string or other value to pass to the factory action.
		/// </summary>
		public object param;

		/// <summary>
		/// Don't add the <b>MenuItem</b> to menu.
		/// </summary>
		public bool hide;

		/// <summary>
		/// Sets menu item text = method/type name with spaces instead of _ , like Select_all -> "Select all".
		/// </summary>
		/// <param name="l">Don't use.</param>
		public CommandAttribute([CallerLineNumber] int l = 0) { order_ = l; }

		/// <summary>
		/// Specifies menu item text.
		/// </summary>
		/// <param name="text">Menu item text. Use _ to Alt-underline a character, like "_Copy".</param>
		/// <param name="l">Don't use.</param>
		public CommandAttribute(string text, [CallerLineNumber] int l = 0) { this.text = text; order_ = l; }

		/// <summary>
		/// Specifies Alt-underlined character. Sets menu item text = method/type name with spaces instead of _ , like Select_all -> "Select all".
		/// </summary>
		/// <param name="underlined">Character to underline.</param>
		/// <param name="l">Don't use.</param>
		public CommandAttribute(char underlined, [CallerLineNumber] int l = 0) { this.underlined = underlined; order_ = l; }
	}
}
