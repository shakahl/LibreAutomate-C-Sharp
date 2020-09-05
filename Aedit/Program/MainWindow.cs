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
		_panels["Output"].Content = new SciHost();
		_panels["Info"].Content = new TextBlock();
		//_panels["Info"].Content = new TextBox(); //test focus
		_panels["Found"].Content = new SciHost();
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
		//_panels["Custom2"].Content = _TB();

		//var t1 = new StackPanel { Orientation = Orientation.Horizontal };
		//var b1 = new Button { Content = "A", Focusable = false };
		//b1.Click += (_, _) => AOutput.Write("click");
		//t1.Children.Add(b1);
		//t1.Children.Add(new Button { Content = "B" });
		//t1.Children.Add(new TextBox { Width = 50 });
		//_panels["Custom1"].Content = t1;

		var t2 = _TB();
		var b2 = new Button { Content = "A", Focusable = false };
		b2.Click += (_, _) => AOutput.Write("click");
		//t2.ItemsSource = new Control[] { b2, new Button { Content = "B" }, new TextBox { Width = 50 } };
		t2.ToolBars[0].ItemsSource = new Control[] { b2, new Button { Content = "B" }, new TextBox { Width = 50 } };
		_panels["Custom2"].Content = t2;

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

		static ToolBarTray _TB() {
			var t = new ToolBarTray { IsLocked = true };
			var c=new ToolBar();
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
