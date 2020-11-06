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
using System.Windows.Input;

using Au;
using Au.Types;
using Au.Controls;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.ObjectModel;

static class Menus
{
	[Command]
	public static class File
	{
		[Command(keys = "F2")]
		public static void Rename() { }

		[Command(keys = "Delete")]
		public static void Delete() { }

		[Command("...", image = "resources/images/property_16x.xaml")]
		public static void Properties() { }

		[Command("Open, close")]
		public static class OpenClose
		{
			[Command]
			public static void Open() { }

			[Command]
			public static void Open_in_default_app() { }

			[Command]
			public static void Select_in_explorer() { }

			[Command(separator = true)]
			public static void Close() { }

			[Command]
			public static void Cloase_all() { }

			[Command]
			public static void Collapse_folders() { }

			[Command(separator = true)]
			public static void Previous_document() { }
		}

		[Command]
		public static class More
		{
			[Command("Cu_t", keysText = "Ctrl+X")]
			public static void Cut_file() { }

			[Command("Copy", keysText = "Ctrl+C")]
			public static void Copy_file() { }

			[Command("Paste", keysText = "Ctrl+V")]
			public static void Paste_file() { }

			[Command('r', separator = true)]
			public static void Copy_relative_path() { }

			[Command('f')]
			public static void Copy_full_path() { }

			//[Command("...", separator = true)]
			//public static void Print_setup() { }

			//[Command("...")]
			//public static void Print() { }

			[Command(separator = true, keys = "Ctrl+S", image = "resources/images/saveall_16x.xaml")]
			public static void Save_now() { }
		}

		[Command("Export, import", separator = true)]
		public static class ExportImport
		{
			[Command("Export as .zip...")]
			public static void Export_as_zip() { }

			[Command("...")]
			public static void Export_as_workspace() { }

			[Command("Import .zip...", separator = true)]
			public static void Import_zip() { }

			[Command("...")]
			public static void Import_workspace() { }

			[Command("...")]
			public static void Import_files() { }
		}

		[Command]
		public static class Workspace
		{
			[Command]
			public static class Recent_workspaces
			{

			}

			[Command("...")]
			public static void Open_workspace() { FilesModel.OpenWorkspaceUI(); }

			[Command("...")]
			public static void New_workspace() { FilesModel.NewWorkspaceUI(); }
		}

		[Command(separator = true, keysText = "Alt+F4")]
		public static void Close_window() { }

		[Command]
		public static void Exit() { }
	}

	[Command]
	public static class New
	{
		static void _New(string name) { App.Model.NewItem(name, beginRenaming: true); }

		[Command('s', keys = "Ctrl+N", image = "resources/images/newfile_16x.xaml")]
		public static void New_script() { _New("Script.cs"); }

		[Command('c')]
		public static void New_class() { _New("Class.cs"); }

		[Command('p')]
		public static void New_partial() { _New("Partial.cs"); }

		[Command('f')]
		public static void New_folder() { _New(null); }
	}

	[Command]
	public static class Edit
	{
		[Command(keysText = "Ctrl+Z", image = "resources/images/undo_16x.xaml")]
		public static void Undo() { }

		[Command(keysText = "Ctrl+Y", image = "resources/images/redo_16x.xaml")]
		public static void Redo() { }

		[Command('t', separator = true, keysText = "Ctrl+X", image = "resources/images/cut_16x.xaml")]
		public static void Cut() { }

		[Command(keysText = "Ctrl+C", image = "resources/images/copy_16x.xaml")]
		public static void Copy() { }

		[Command(keysText = "Ctrl+V", image = "resources/images/paste_16x.xaml")]
		public static void Paste() { }

		[Command()]
		public static void Forum_copy() { }

		[Command(separator = true, keys = "Ctrl+F", image = "resources/images/findinfile_16x.xaml")]
		public static void Find() { }

		[Command(separator = true, keys = "Ctrl+Space")]
		public static void Autocompletion_list() { }

		[Command(keys = "Ctrl+Shift+Space")]
		public static void Parameter_info() { }

		[Command(keys = "F12", keysText = "F12, Ctrl+click")]
		public static void Go_to_definition() { }

		[Command(separator = true)]
		public static class Selection
		{
			[Command]
			public static void Comment() { }

			[Command]
			public static void Uncomment() { }

			[Command]
			public static void Indent() { }

			[Command]
			public static void Unindent() { }

			[Command]
			public static void Select_all() { }
		}

		[Command]
		public static class View
		{
			[Command(checkable = true, keys = "Ctrl+W", image = "resources/images/wordwrap_16x.xaml")]
			public static void Wrap_lines() { }

			[Command(checkable = true, image = "resources/images/image_16x.xaml")]
			public static void Images_in_code() { }
		}
	}

	[Command]
	public static class Code
	{
		[Command('W')]
		public static void AWnd() { }

		[Command('A')]
		public static void AAcc() { }

		[Command('I')]
		public static void AWinImage() { }

		[Command(separator = true, keysText = "Ctrl+Space in string")]
		public static void Keys() { }

		[Command(keysText = "Ctrl+Space in string")]
		public static void Regex() { }

		[Command(separator = true)]
		public static void Windows_API() { }
	}

	[Command]
	public static class Run
	{
		[Command(keys = "F5", image = "resources/images/startwithoutdebug_16x.xaml")]
		public static void Start() { }

		[Command(image = "resources/images/stop_16x.xaml")]
		public static void End() { }

		//[Command(image = "resources/images/pause_16x.xaml")]
		//public static void Pause() { }

		[Command(keys = "F7", image = "resources/images/buildselection_16x.xaml")]
		public static void Compile() { }

		[Command("...")]
		public static void Recent() { }

		[Command(separator = true)]
		public static void Debug_break() { }
	}

	[Command]
	public static class TT
	{
		[Command("...")]
		public static void Add_trigger() { }

		[Command("...")]
		public static void Add_toolbar() { }

		[Command('k', separator = true)]
		public static void Edit_hotkey_triggers() { }

		[Command('a')]
		public static void Edit_autotext_triggers() { }

		[Command('m')]
		public static void Edit_mouse_triggers() { }

		[Command('w')]
		public static void Edit_window_triggers() { }

		[Command(separator = true)]
		public static void Edit_common_toolbars() { }

		[Command()]
		public static void Edit_window_toolbars() { }

		[Command(separator = true)]
		public static void Edit_TT_script() { }

		[Command()]
		public static void Restart_TT_script() { }

		[Command(separator = true)]
		public static void Disable_triggers() { }

		[Command("...")]
		public static void Active_triggers() { }

		[Command("...")]
		public static void Active_toolbars() { }
	}

	[Command]
	public static class Tools
	{
		[Command(image = "resources/images/settingsgroup_16x.xaml")]
		public static void Options() { }

		[Command(separator = true)]
		public static class Output
		{
			[Command(keysText = "M-click")]
			public static void Clear() { }

			[Command("Copy", keysText = "Ctrl+C")]
			public static void Copy_output() { }

			[Command(keysText = "Ctrl+F")]
			public static void Find_selected_text() { }

			[Command()]
			public static void History() { }

			[Command("Wrap lines", separator = true, checkable = true)]
			public static void Wrap_lines_in_output() { }

			[Command("White space", checkable = true)]
			public static void White_space_in_output() { }

			[Command(checkable = true)]
			public static void Topmost_when_floating() { }
		}
	}

	[Command]
	public static class Help
	{
		[Command]
		public static void Program_help() { }

		[Command]
		public static void Library_help() { }

		[Command(keys = "F1", image = "resources/images/statushelp_16x.xaml")]
		public static void Context_help() { }

		//[Command(separator = true)]
		//public static void Forum() { }

		[Command]
		public static void Email() { }

		//[Command(separator = true)]
		//public static void About() { }
	}

#if TRACE
	[Command]
	public static void TEST() {
		//_TestLV();
		//_TestNat();

		//var tv = new ListBox { BorderThickness = default, SelectionMode = SelectionMode.Extended };
		//VirtualizingPanel.SetVirtualizationMode(tv, VirtualizationMode.Recycling);
		//var a = new string[900];
		//for (int i = 0; i < a.Length; i++) a[i] = "Texthjhjhjhjhjh " + i;
		//tv.ItemsSource = a;
		//App.Panels["Files"].Content = tv;



		//APerf.First();
		//App.Panels["Files"].Content = new Nstest.Script().Test();
		////App.Panels["Files"].Content = new Nstest.WriteableBitmap_and_GDI_text().Test();
		//APerf.Next();
		//ATimer.After(1, _ => APerf.NW());

		//if (_testCM == null) {
		//	_testCM = new AContextMenu();
		//	//App.Commands["Wrap_lines"].CopyToMenu(_testCM.Add(null));
		//	App.Commands["View"].CopyToMenu(_testCM);
		//}
		//_testCM.IsOpen = true;

		//var m = new Menu();
		//var b = new MenuItem();
		//App.Commands["View"].CopyToMenu(b);
		//m.Items.Add(b);
		////App.Toolbars[0].Items.Add(m);
		////(App.Toolbars[0].Parent as ToolBarTray).ToolBars.Add(m);
		////var p = (App.Toolbars[0].Parent as ToolBarTray).Parent as DockPanel;

		////var p = new DockPanel();
		////p.Children.Add(m);

		//var p = new Border();
		//p.Child = m;
		//App.Toolbars[0].Items.Add(p);
	}
	//static AContextMenu _testCM;

//	static void _TestLV() {
//		var k=new ListBox { BorderThickness = default, SelectionMode = SelectionMode.Extended };
//		k.UseLayoutRounding = true;

//		string xaml2 = @"<Style TargetType='{x:Type ListBoxItem}' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
//			<Setter Property='Height' Value='18' />
//			<Setter Property='Padding' Value='0' />
//            <Setter Property='IsSelected' Value='{Binding IsSelected, Mode=TwoWay}' />
//            <Setter Property='FontWeight' Value='Normal' />
//            <Style.Triggers>
//                <Trigger Property='IsSelected' Value='True'>
//                    <Setter Property='FontWeight' Value='Bold' />
//                </Trigger>
//            </Style.Triggers>
//        </Style>";
//		k.ItemContainerStyle = XamlReader.Parse(xaml2) as Style;

//		string xaml1 = @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
//<StackPanel Orientation='Horizontal'>
//<Image Source='{Binding Image}' Stretch='None' Height='16' Width='16'/>
//<TextBlock Text='{Binding Text}' Margin='6,-1,0,0'/>
//</StackPanel>
//</DataTemplate>";
//		k.ItemTemplate = XamlReader.Parse(xaml1) as DataTemplate;

//		VirtualizingPanel.SetVirtualizationMode(k, VirtualizationMode.Recycling);
//		VirtualizingPanel.SetScrollUnit(k, ScrollUnit.Item);

//		var im = BitmapFrame.Create(new Uri(@"Q:\app\Au\_Au.Editor\Resources\png\fileClass.png"));
//		int n = 1000;
//		var a = new List<TextImage>(n);
//		for (int i = 0; i < n; i++) {
//			a.Add(new TextImage("Abcdefghij " + i.ToString(), im));
//		}
//		APerf.Next('d');
//		k.ItemsSource = a;

//		App.Panels["Files"].Content = k;
//	}

	//public class TextImage : INotifyPropertyChanged
	//{
	//	public string Text { get; set; }
	//	public ImageSource Image { get; set; }
	//	//	public List<TextImage> Items { get;set; }
	//	public ObservableCollection<TextImage> Items { get; set; }
	//	//	public bool IsExpanded { get;set; }

	//	bool _isExpanded;
	//	public bool IsExpanded {
	//		get => _isExpanded;
	//		set {
	//			//			AOutput.Write(_isExpanded, value);
	//			if (value != _isExpanded) {
	//				_isExpanded = value;
	//				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsExpanded"));
	//			}
	//		}
	//	}

	//	bool _isSelected;
	//	public bool IsSelected {
	//		get => _isSelected;
	//		set {
	//			//			AOutput.Write(_isSelected, value);
	//			if (value != _isSelected) {
	//				_isSelected = value;
	//				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
	//			}
	//		}
	//	}

	//	public TextImage(string text, ImageSource image) {
	//		Text = text; Image = image;
	//	}

	//	#region INotifyPropertyChanged

	//	public event PropertyChangedEventHandler PropertyChanged;

	//	#endregion
	//}

	//static void _TestNat() {
	//	//App.Panels["Files"].Content = new Nstest.Script().Test();

	//}

	[Command]
	public static void gc() {
		GC.Collect();
	}
#endif
}
