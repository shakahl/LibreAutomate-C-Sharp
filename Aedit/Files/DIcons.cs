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
using System.Windows;
using System.Windows.Controls;
using System.Linq;

using Au;
using Au.Types;
using Au.More;
using Au.Controls;
using Au.Tools;

class DIcons : KDialogWindow
{
	public static void ZShow() {
		if (s_dialog == null) {
			s_dialog = new();
			s_dialog.Show();
		} else {
			s_dialog.Activate();
		}
	}
	static DIcons s_dialog;

	protected override void OnClosed(EventArgs e) {
		s_dialog = null;
		base.OnClosed(e);
	}

	List<_Item> _a;
	string _color = "#000000";
	Random _random;
	int _dpi;
	bool _withCollection;

	DIcons() {
		Title = "Icons";
		Owner = App.Wmain;

		var b = new wpfBuilder(this).WinSize(600, 600);
		b.WinProperties(WindowStartupLocation.CenterOwner, showInTaskbar: false);
		b.Columns(-1, 0);

		//left - edit control and tree view
		b.Row(-1).StartDock();
		b.Add(out TextBox tName).Dock(Dock.Top).Tooltip("Search. Examples: part, start*, *end.");
		b.xAddInBorder(out KTreeView tv); //tv.SingleClickActivate = true;
		b.End();

		//right - color picker, buttons, etc
		b.StartGrid().Columns(-1);
		b.Add(out KColorPicker colors);
		colors.ColorChanged += color => {
			_random = null;
			_color = _ColorToString(color);
			tv.Redraw();
		};
		b.StartStack();
		TextBox randFromTo = null;
		b.AddButton("Randomize colors", _ => _RandomizeColors());
		b.Add("L %", out randFromTo, "30-70").Width(50);
		b.End();
		b.StartStack<GroupBox>("Set icon of selected files");
		b.AddButton(out var bThis, "This", _ => _SetIcon(tv)).Width(70).Disabled();
		b.AddButton("Default", _ => _SetIcon(null)).Width(70);
		//b.AddButton("Random", null).Width(70); //idea: set random icons for multiple selected files. Probably too crazy.
		b.End();
		b.StartStack<GroupBox>("Insert code for menu/toolbar/etc icon");
		b.AddButton(out var bCodeVar, "Variable", _ => _InsertCode(tv, 0)).Width(70).Disabled();
		b.AddButton(out var bCodeField, "Field", _ => _InsertCode(tv, 1)).Width(70).Disabled();
		b.AddButton(out var bCodeXaml, "XAML", _ => _InsertCode(tv, 2)).Width(70).Disabled();
		b.End();
		b.StartGrid<GroupBox>("List display options");
		//b.Add("Background", out ComboBox cBackground).Items("Default|Control|White|Black)");
		//cBackground.SelectionChanged += (o, e) => _ChangeBackground();
		b.Add(out KCheckBox cCollection, "Collection");
		cCollection.CheckChanged += (_, _) => {
			_withCollection = cCollection.True();
			tv.Redraw();
		};
		b.End();
		b.Row(-1);
		b.R.Add<TextBlock>().Align("R").Text("Thanks to ", "<a>MahApps.Metro.IconPacks", new Action(() => run.it("https://github.com/MahApps/MahApps.Metro.IconPacks")));
		b.End();

		b.End();

		b.Loaded += () => {
			_dpi = Dpi.OfWindow(this);
			_OpenDB();

			_a = new(30000);
			foreach (var (table, _) in s_tables) {
				using var stat = s_db.Statement("SELECT name FROM " + table);
				while (stat.Step()) {
					_a.Add(new(table, stat.GetText(0)));
				}
			}
			_a.Sort((a, b) => string.Compare(a._name, b._name, StringComparison.OrdinalIgnoreCase));
			_RandomizeColors();
			tv.SetItems(_a, false);
		};

		tName.TextChanged += (_, _) => {
			var name = tName.Text;
			bool wild = name.FindAny("*?") >= 0;
			var e = name.NE() ? _a : _a.Where(o => wild ? o._name.Like(name, true) : o._name.Contains(name, StringComparison.OrdinalIgnoreCase));
			tv.SetItems(e, false);
			_EnableControls(false);
		};

		tv.SelectedSingle += (o, i) => {
			_EnableControls(true);
			//var k = _a[i];
			//if(GetIconFromBigDB(k._table, k._name, _ItemColor(k), out var xaml)) {
			//	print.it(xaml);
			//}
		};

		void _EnableControls(bool enable) {
			bThis.IsEnabled = enable;
			bCodeVar.IsEnabled = enable;
			bCodeField.IsEnabled = enable;
			bCodeXaml.IsEnabled = enable;
		}

		void _SetIcon(KTreeView tv) {
			var k = tv?.SelectedItem as _Item;
			string icon = k == null ? null : (k._table + "." + k._name + " " + _ItemColor(k));
			foreach (var v in FilesModel.TreeControl.SelectedItems) {
				v.CustomIcon = icon;
			}
		}

		void _InsertCode(KTreeView tv, int what) {
			var k = tv.SelectedItem as _Item;
			if (GetIconFromBigDB(k._table, k._name, _ItemColor(k), out var s)) {
				s = s.Replace('\"', '\'').RegexReplace(@"\R\s*", "");
				switch (what) {
				case 0: InsertCode.Statements($"string icon{k._name} = \"{s}\";"); break;
				case 1: InsertCode.Statements($"public const string {k._name} = \"{s}\";"); break;
				case 2: InsertCode.TextSimply(s); break;
				}
			}
		}

		void _RandomizeColors() {
			_random = new();
			//perf.first();
			//if (keys.isScrollLock) { //generate random HLS. Colors look not as good, maybe because the green-yellow range is too narrow and red-purple range too wide.
			//	foreach (var v in _a) {
			//		int L = _random.Next(40, 200);
			//		double k = _random.NextDouble() * 15d; int S = 240 - (int)(k * k); //print.it(S); //generate less low-saturation colors
			//		if (S < 60 && L > 60) S += 60; //avoid light-gray, because looks like disabled
			//		v._color = ColorInt.FromHLS(_random.Next(0, 240), L, S, false);
			//	}
			//} else {
			int iFrom = 0, iTo = 100; if (randFromTo.Text.RegexMatch(@"^(\d+) *- *(\d+)", out var m)) { iFrom = m[1].Value.ToInt(); iTo = m[2].Value.ToInt(); }
			float briFrom = Math.Clamp(iFrom / 100f, 0f, 0.9f), briTo = Math.Clamp(iTo / 100f, briFrom + 0.05f, 1f);
			int middleL = ((briTo + briFrom) * 120f).ToInt();
			foreach (var v in _a) {
				int c = _random.Next(0, 0xffffff);
				var (H, L, S) = ColorInt.ToHLS(c, false);
				if (S < 60 && L > 60) c = ColorInt.FromHLS(H, L, S += 60, false); //avoid light-gray, because looks like disabled
				var bri = ColorInt.GetPerceivedBrightness(c, false);
				if (bri < briFrom || bri > briTo) c = ColorInt.FromHLS(H, middleL, S, false);
				v._color = c;
			}
			//}
			//perf.nw(); //4 ms
			tv.Redraw();
		}

		//void _ChangeBackground() {
		//	int i = cBackground.SelectedIndex;
		//	tv.currently does not support custom background color. We could custom-draw, but never mind.
		//}
	}

	static string _ColorToString(int c) => "#" + c.ToStringInvariant("X6");

	string _ItemColor(_Item k) => _random == null ? _color : _ColorToString(k._color);

	protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi) {
		_dpi = newDpi.PixelsPerInchX.ToInt();
		base.OnDpiChanged(oldDpi, newDpi);
	}

	class _Item : ITreeViewItem
	{
		public string _table, _name;
		public int _color;

		public _Item(string table, string name) {
			_table = table; _name = name;
		}

		string ITreeViewItem.DisplayText => s_dialog._withCollection ? (_name + "          (" + _table + ")") : _name;

		System.Drawing.Bitmap ITreeViewItem.Image {
			//note: don't store UIElement or Bitmap. They can use hundreds MB of memory and it does not make faster/better. Let GC dispose unused objects asap.
			get {
				try {
					//using var p1 = perf.local();
					if (GetIconFromBigDB(_table, _name, s_dialog._ItemColor(this), out string xaml)) {
						//p1.Next('d');
						return ImageUtil.LoadGdipBitmapFromXaml(xaml, s_dialog._dpi, (16, 16));
					}
				}
				catch (Exception ex) { Debug_.Print(ex.ToStringWithoutStack()); }
				return null;
			}
		}
	}

	static sqlite s_db;
	static List<(string table, string templ)> s_tables; //~30

	static void _OpenDB() {
		if (s_db == null) {
			var db = s_db = new sqlite(folders.ThisAppBS + "icons.db", SLFlags.SQLITE_OPEN_READONLY);
			process.thisProcessExit += _ => db.Dispose();
			s_tables = new();
			using var st = s_db.Statement("SELECT * FROM _tables");
			while (st.Step()) {
				s_tables.Add((st.GetText(0), st.GetText(1)));
			}
		}
	}

	public static bool GetIconFromBigDB(string table, string name, string color, out string xaml) {
		xaml = null;
		string templ = null; foreach (var v in s_tables) if (v.table == table) { templ = v.templ; break; }
		if (templ == null) return false;
		if (!s_db.Get(out string data, $"SELECT data FROM {table} WHERE name='{name}'")) return false;

		int i = templ.Find(" Data=\"{x:Null}\""); if (i < 0) return false;
		templ = templ.ReplaceAt(i + 7, 8, data);

		if (0 == templ.RegexReplace(@"(?:Fill|Stroke)=""\K[^""]+", color, out templ)) return false;

		if (templ.Contains("\"{")) return false;
		//print.it(templ);
		xaml = $@"<Viewbox Width='16' Height='16' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>{templ}</Viewbox>";
		return true;
	}

	public static bool GetIconFromBigDB(string icon, out string xaml) {
		xaml = null;
		int i = icon.IndexOf('.'); if (i < 0) return false;
		int j = icon.IndexOf(' ', i + 1); if (j < 0) return false;
		_OpenDB();
		return GetIconFromBigDB(icon[..i], icon[++i..j], icon[++j..], out xaml);
	}

#if true
	public static bool TryGetIconFromBigDB(string icon, out string xaml) {
		//using var p1 = perf.local();
		try { return GetIconFromBigDB(icon, out xaml); }
		catch (Exception e1) { Debug_.Print(e1.ToStringWithoutStack()); }
		xaml = null;
		return false;
	}
	//This version gets data directly from the big DB. Slightly slower but simpler.
	//Maybe not good to use an 18 MB DB directly, but I didn't notice something bad.
	//note: the big DB must have PRIMARY KEY. Don't need it with other version.
#else
	//This version stores XAML of used icons in a small DB file. When missing, gets from the big DB and copies to the small.
	//Has advantages and disadvantages.
	//Faster 2 times (when the small DB is WAL). But both versions much faster than converting XAML to GDI+ bitmap. Same memory usage.
	//shoulddo: when deleting a file, delete its icon from the small DB if not used by other files.
	public static bool TryGetIconFromBigDB(string icon, out string xaml) {
		xaml = null;
		//using var p1 = perf.local();
		try {
			if(s_iconsDB == null) {
				var dbFile = folders.ThisAppDataLocal + "icons.db"; //shoulddo: if portable, create in workspace folder
				var db = s_iconsDB = new(dbFile, sql: "PRAGMA journal_mode=WAL; CREATE TABLE IF NOT EXISTS icons (icon TEXT PRIMARY KEY, xaml TEXT)");
				process.thisProcessExit += _ => db.Dispose();
			}
			if (!s_iconsDB.Get(out xaml, "SELECT xaml FROM icons WHERE icon=?", icon)) {
			//if (!s_iconsDB.Get(out xaml, $"SELECT xaml FROM icons WHERE icon='{icon}'")) { //same speed. With same prepared statement same.
				if (!Au.Tools.DIcons.GetIconFromBigDB(icon, out xaml)) return false;
				s_iconsDB.Execute("INSERT INTO icons VALUES (?, ?)", icon, xaml);
			}
			xaml = $@"<Viewbox Width='16' Height='16' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>{xaml}</Viewbox>";
			return true;
		}
		catch (Exception e1) { Debug_.Print(e1.ToStringWithoutStack()); }
		return false;
	}
	static sqlite s_iconsDB;
#endif
}
