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

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	AuPanels _panels = new();
	//AuPanels _docPanels = new();

	public MainWindow() {
		InitializeComponent();

		//_panels.SplitterBrush = new SolidColorBrush((Color)new ColorInt(0x5D6B99, true)); //like VS
		_panels.BorderBrush = SystemColors.ActiveBorderBrush;
		//_panels.CaptionBrush = _panels.BorderBrush = Brushes.Wheat;
		//_panels.CaptionBrush = Brushes.Wheat;

		_panels.Load(AFolders.ThisAppBS + @"Default\Layout.xml", null);
		//_panels.Load(AFolders.ThisAppBS + @"Default\Layout.xml", ProgramSettings.DirBS + "Layout.xml");

		_panels["Files"].Content = new TreeView { BorderThickness = default };
		_panels["Output"].Content = new SciHost() { Focusable = false };
		_panels["Info"].Content = new TextBlock();
		//_panels["Info"].Content = new TextBox(); //test focus
		_panels["Found"].Content = new SciHost() { Focusable = false };
		//_panels["Open"].Content = new ListBox();
		_panels["Open"].Content = new ListBox { BorderThickness = default };
		_panels["Running"].Content = new ListBox { BorderThickness = default };
		_panels["Find"].Content = new PanelFind();

		_panels["Menu"].Content = new Menu();
		_panels["Help"].Content = _TB();
		_panels["Tools"].Content = _TB();
		_panels["Custom1"].Content = _TB();
		_panels["File"].Content = _TB();
		_panels["Run"].Content = _TB();
		_panels["Edit"].Content = _TB();
		_panels["Custom2"].Content = _TB();

		//for (int i = 0; i < 2; i++) {
		//	var newDoc = _panels[i == 0 ? "Document" : ""];
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
			_panels[""].Visible = false;
			v.Visible = true;
			v.Content.Focus(); //handle not created
			//ATimer.After(1, _ => v.Content.Focus());
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

		AuPanels.ILeaf _AddDoc(string name) {
			var docPlaceholder = _panels[""]; //in tab
			//var docPlaceholder = _panels["Open"]; //in stack
			var v = docPlaceholder.AddSibling(false, AuPanels.LeafType.Document, name, true);
			v.Content = new SciHost();
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

			return v;
		}

		static ToolBarTray _TB() {
			var t = new ToolBarTray { IsLocked = true };
			var c=new ToolBar();

			var b = new Button { Content = "A", Focusable = false };
			b.Click += (_, _) => AOutput.Write("click");
			c.ItemsSource = new Control[] { b, new Button { Content = "B" }, new TextBox { Width = 50 } };
			t.ToolBars.Add(c);
			return t;
		}

		//static ToolBar _TB() {
		//	var t=new ToolBar();
		//	ToolBarTray.SetIsLocked(t, true);
		//	return t;
		//}
	}

	protected override void OnClosing(CancelEventArgs e) {
		base.OnClosing(e);
		_panels.Save();
	}
}
