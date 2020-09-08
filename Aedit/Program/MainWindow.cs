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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;

using Au;
using Au.Types;
using Au.Controls;
using System.Windows.Controls.Primitives;

public partial class MainWindow : Window
{
	AuPanels _panels = new();
	//AuPanels _docPanels = new();

	public MainWindow() {
		Title = "Aedit";

		//_panels.SplitterBrush = new SolidColorBrush((Color)new ColorInt(0x5D6B99, true)); //like VS
		_panels.BorderBrush = SystemColors.ActiveBorderBrush;
		//_panels.CaptionBrush = _panels.BorderBrush = Brushes.Wheat;
		//_panels.TabBrush = Brushes.MintCream;
		//_panels.CaptionBrush = Brushes.Wheat;

		//_panels.Load(AFolders.ThisAppBS + @"Default\Layout.xml", null);
		_panels.Load(AFolders.ThisAppBS + @"Default\Layout.xml", ProgramSettings.DirBS + "Layout.xml");

		_panels["Files"].Content = new TreeView { BorderThickness = default };
		_panels["Output"].Content = new SciHost() { Focusable = false };
		_panels["Info"].Content = new TextBlock();
		//_panels["Info"].Content = new TextBox(); //test focus
		_panels["Found"].Content = new SciHost() { Focusable = false };
		//_panels["Open"].Content = new ListBox();
		//_panels["Open"].Content = new ListBox { BorderThickness = default };
		_panels["Running"].Content = new ListBox { BorderThickness = default };
		_panels["Find"].Content = new PanelFind();

		_panels["Menu"].Content = new Menu();
		_panels["Tools"].Content = _TB("Tools");
		_panels["Help"].Content = _TB("Help");
		_panels["Custom1"].Content = _TB("Custom1");
		_panels["File"].Content = _TB("File");
		_panels["Run"].Content = _TB("Run");
		_panels["Edit"].Content = _TB("Edit");
		_panels["Custom2"].Content = _TB("Custom2");

		//for (int i = 0; i < 2; i++) {
		//	var newDoc = _panels[i == 0 ? "Document" : "documents"];
		//	//newDoc.Content = new Rectangle { Fill = SystemColors.AppWorkspaceBrush };
		//	newDoc.AddDocument("Document 1", after: true);
		//}

		//_docPanels.Load(AFolders.ThisAppBS + @"Default\DocLayout.xml", null);
		//_docPanels["Document"].Content = new SciHost();
		//_docPanels.Container = g => {
		//	_panels["Document"].Content = g;
		//};

		//Background = SystemColors.AppWorkspaceBrush;
		//this.Content = _panels.RootElem;
		//_panels.Container = g => this.Content = g;
		_panels.Container = g => {
			//g.Background = Brushes.Wheat;
			//g.Background = Brushes.RoyalBlue;
			this.Content = g;
		};

		//ATimer.After(2000, _ => _panels["Find"].Visible = true);
		//ATimer.After(2000, _ => _panels["Find"].Floating = true);
		//ATimer.After(2000, _ => Hide());
		//ATimer.After(4000, _ => Show());

		//TabControl docTC = null;

		ATimer.After(1000, _ => {
			//var v = _panels["Find"].AddSibling(true, AuPanels.PanelType.Panel, "New");
			//var v = _panels["File"].AddSibling(false, AuPanels.PanelType.Panel, "New");
			//var v = _panels["Open"].AddSibling(false, AuPanels.PanelType.Panel, "New");
			//var v = _panels["Info"].AddSibling(false, AuPanels.PanelType.Panel, "New");
			//var v = _panels["Found"].AddSibling(true, AuPanels.PanelType.Panel, "New");
			//v.Content = new TextBox();

			var v = _AddDoc("Document 1");
			_AddDoc("Document 2");
			_AddDoc("Document 3");
			_AddDoc("Document 4");
			v.Visible = true;
			_panels["documents"].Visible = false;
			//_OpenDoc(v);
			v.Content.Focus();

			//docTC.SelectionChanged += (_, e) => {
			//	_OpenDoc((e.AddedItems[0] as TabItem).Tag as AuPanels.ILeaf);
			//};

			//ATimer.After(2000, _ => {
			//	v.Delete();
			//	ATimer.After(2000, _ => _AddDoc("Document 10"));
			//});
			ATimer.After(2000, _ => {
				//v.Rename("Renamed WWWWWWWWW");

				//_panels["Info"].Floating = true;
				//_panels["Info"].Visible = true;
				//ATimer.After(2000, _ => _panels["Info"].Visible = false);
				//_panels["Info"].Visible = false;
				//_panels["Found"].Visible = false;
			});
		});

		void _OpenDoc(AuPanels.ILeaf leaf) {
			if (leaf.Content != null) return;
			leaf.Content = new SciHost();
			//AOutput.Write(leaf);
			//leaf.Content.Focus();
		}

		AuPanels.ILeaf _AddDoc(string name) {
			var docPlaceholder = _panels["documents"]; //in tab
													   //var docPlaceholder = _panels["Open"]; //in stack
			var v = docPlaceholder.AddSibling(false, AuPanels.LeafType.Document, name, true);
			//v.Content = new SciHost();
			//v.Content = new TextBox();
			//v.VisibleChanged += (_,_) => v.Delete();
			v.Closing += (_, e) => { e.Cancel = !ADialog.ShowOkCancel("Close?"); };
			v.ContextMenuOpening += (o, m) => {
				var k = o as AuPanels.ILeaf;
				m.Separator();
				m["Close 2"] = o => k.Delete();
			};
			//docPlaceholder.Visible = false;

			//AOutput.Write(_panels["Find"], _panels["Document 1", true]);

			//var p = v.Parent;
			//AOutput.Write(p.Panel, p.Grid, p.TabControl, p.TabItem, p.Index);
			//v.ParentChanged += (_, _) => AOutput.Write(v.Parent.TabControl);
			//docTC ??= p.TabControl;

			v.TabSelected += (_, _) => _OpenDoc(v);

			return v;
		}

		static ToolBarTray _TB(string name) {
			var t = new ToolBarTray { IsLocked = true };
			var c = new ToolBar();

			switch (name) {
			case "File":
				c.ItemsSource = new Control[] {
					_B("New script", "newfile_16x"),
					_B("File properties", "property_16x"),
					_B("Save now", "saveall_16x"),
				};
				break;
			case "Run":
				c.ItemsSource = new Control[] {
					_B("Compile", "buildselection_16x"),
					_B("Run", "startwithoutdebug_16x"),
					_B("End task", "stop_16x"),
				};
				break;
			case "Edit":
				c.ItemsSource = new Control[] {
					_B("Find", "findinfile_16x"),
					new Separator(),
					_B("Undo", "undo_16x"),
					_B("Redo", "redo_16x"),
					new Separator(),
					_B("Cut", "cut_16x"),
					_B("Copy", "copy_16x"),
					_B("Paste", "paste_16x"),
					new Separator(),
					_B("Wrap lines", "wordwrap_16x", true),
					_B("Images in code", "image_16x", true),
				};
				break;
			case "Tools":
				c.ItemsSource = new Control[] {
					_B("Options", "settingsgroup_16x"),
				};
				break;
			case "Help":
				c.ItemsSource = new Control[] {
					_B("Help", "statushelp_16x"),
					new TextBox { Width = 150 }
				};
				break;
			case "Custom2":
				c.ItemsSource = new Control[] {
					_B("Test", "test"),
				};
				break;
			}
			t.ToolBars.Add(c);
			return t;
		}

		static ButtonBase _B(string tt, string image, bool check = false) {
			//var b = new Button { ToolTip = tt, Focusable = false, Padding = new Thickness(4) };
			var b = check ? (ButtonBase)new CheckBox() : new Button();
			b.ToolTip = tt;
			b.Focusable = false;
			b.Padding = new Thickness(4);

			var f = new Frame { Source = new Uri("/Aedit;component/resources/images/" + image + ".xaml", UriKind.Relative) };
			b.Content = f;
			b.Click += (_, _) => AOutput.Write(tt);
			return b;
		}

		//static ToolBar _TB() {
		//	var t=new ToolBar();
		//	ToolBarTray.SetIsLocked(t, true);
		//	return t;
		//}

		AWnd.More.SavedRect.Restore(this, Program.Settings.wndPos, o => Program.Settings.wndPos = o);
	}

	protected override void OnClosing(CancelEventArgs e) {
		base.OnClosing(e);
		_panels.Save();
	}
}
