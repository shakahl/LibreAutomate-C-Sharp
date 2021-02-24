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
//using System.Linq;
//using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Au;
using Au.Types;
using System.Runtime;
using System.Windows.Input;
//using Au.Controls;
//using static Au.Controls.Sci;
//using Au.Compiler;
//using System.Collections.Immutable;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis.Completion;
//using Microsoft.CodeAnalysis.Text;
//using Microsoft.CodeAnalysis.Host.Mef;

//using DiffMatchPatch;
//using System.Globalization;
//using System.Windows.Media.Imaging;
//using System.Resources;

#if TRACE

#pragma warning disable 169

static class Test
{
	//static KPopup _test;
	//static AHookWin _hook;

	//class Menu1 : AWpfMenu
	//{
	//	~Menu1() { AOutput.Write("dtor"); }
	//}

	public static void FromMenubar() {

		//Action a=() => AFile.Run(@"C:\Program Files\Notepad++\notepad++.exe");
		//Action a=() => ATask.Run(@"\Script387.cs");
		//var s=AIcon.IconPathFromCode_(a.Method, out bool cs);
		//AOutput.Write(s, cs);

		//int i = AMenu.ShowSimple("One\0Tooltip|Two");
		//AOutput.Write(i);

		//SIZE z=new SIZE()

		//AOutput.Write(FocusManager.GetFocusedElement(App.Wmain), Keyboard.FocusedElement);

		//var m = new Menu1();
		////m["Test"] = null;
		//App.Commands[nameof(Menus.Edit.View)].CopyToMenu(m);
		//m.IsOpen = true;

		//App.ImageCache.Get(@"imagefile:Q:\Downloads\VS2019 Image Library\vswin2019\ExpandRight\ExpandRight_md_16x.png", 96, true);

		//Console.WriteLine("console");

		//EdDatabases.CreateRefAndDoc();
		//EdDatabases.CreateWinapi();


		//var h = Panels.Editor.ZActiveDoc.Hwnd;
		//AOutput.Write(h);
		////h.ShowL(false);
		////h.ResizeL(0, 0);
		////h.SetWindowPos(Native.SWP.HIDEWINDOW | Native.SWP.NOMOVE | Native.SWP.NOSIZE | Native.SWP.NOSENDCHANGING);
		////h.Enable(false);
		//h.SetTransparency(true, 0);

		//_hook = AHookWin.ThreadCbt(k => {
		//	if (k.code == HookData.CbtEvent.SETFOCUS) {
		//		AOutput.Write(k.Hwnd);
		//	if (k.Hwnd == App.Hwnd) AOutput.Write(new StackTrace());
		//	}
		//	return false;
		//});


		//if (_test == null) {
		//	var p = new KPopup { Size = (200, 300) };
		//	//var p = new KPopup(WS.CAPTION|WS.MAXIMIZEBOX|WS.MINIMIZEBOX|WS.SYSMENU|WS.THICKFRAME, 0) { Size = (200, 300) };
		//	var c = new Label { Background = Brushes.Wheat, Content = "Test" };
		//	p.Content = c;
		//	//p.HwndSource.SizeToContent = default;
		//	_test = p;
		//}
		////_test.ShowByRect(Panels.Tasks, Dock.Bottom);
		////_test.ShowByRect(Panels.Tasks, Dock.Bottom, (100, 100, 100, 100));
		//_test.ShowByRect(null, Dock.Bottom, (700, 1100, 100, 1));

		//ATimer.After(2000, _ => _test.Hide());

		//var m = new AWpfMenu();
		//m["aaaa"] = null;
		//m.Show(App.Wmain, byCaret: true);
		//m.Show(null, byCaret: true);

		//AOutput.Write(Panels.Files.TreeControl.FocusedItem);
		//AOutput.Write("---");
		//AOutput.Write(Keyboard.FocusedElement);
		//AOutput.Write(FocusManager.GetFocusedElement(App.Wmain));
		//AOutput.Write(Api.GetFocus());

		//foreach(var f in App.Model.Root.Descendants()) {
		//	using var p1 = APerf.Create();
		//	var s = f.ItemPath;
		//	//AOutput.Write(s);
		//}

		//App.Model.NewItem(@"More\Text file.txt", name: "find.bmp");

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
		//	_testCM = new AWpfMenu();
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
	//static AWpfMenu _testCM;

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

	//void _TestGetItemText()
	//{
	//	int n = 0;
	//	APerf.First();
	//	foreach(var f in App.Model.Root.Descendants()) {
	//		if(!f.IsCodeFile) continue;
	//		if(f.Name == "5 M lines.cs") continue;
	//		n++;
	//		//AOutput.Write(f.Name);
	//		//var s = f.GetText();
	//		AFile.GetProperties(f.FilePath, out var p, FAFlags.UseRawPath);
	//	}
	//	APerf.NW();
	//	AOutput.Write(n);
	//}

	//void TestFileNodeTextCache()
	//{
	//	AOutput.Clear();
	//	var f = App.Model.CurrentFile;
	//	APerf.First();
	//	var s = f.GetText(saved: true, cache: true);
	//	APerf.NW();
	//	AOutput.Write(s);
	//}

	//	void TestReplaceTextGently()
	//	{
	//		var doc = Panels.Editor.ZActiveDoc;
	//		var s1 = doc.Text;
	//		int i = s1.Find("//.");
	//		//var s2 = s1 + "added\r\n";
	//		var s2 = s1.Insert(i, "insert\r\n");
	//		doc.ZReplaceTextGently(s2);
	//	}

	//	void TestDiffMatchPatch()
	//	{
	//		var s1 = @"//.
	//using Au; using Au.Types; using System; using System.Collections.Generic;
	//class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;

	//	var s=""one"";
	//";
	//		var s2 = @"/*/ role exeProgram;		outputPath %AFolders.Workspace%\bin; console true; /*/ //.
	//using Au; using Au.Types; using System; using System.Collections.Generic;
	//using My.NS1; //ąčę îôû
	//using My.NS2;
	//class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;
	//	var i=2;
	//";

	//		var dmp = new diff_match_patch();
	//		List<Diff> diff = dmp.diff_main(s1, s2, true);
	//		dmp.diff_cleanupSemantic(diff);
	//		var delta = dmp.diff_toDelta(diff);
	//		AOutput.Write(delta);
	//		AOutput.Write("----");
	//		var d2 = dmp.diff_fromDelta(s1, delta);
	//		//AOutput.Write(d2);
	//		AOutput.Write(dmp.diff_text2(d2));
	//	}

	//	void TestNoGcRegion()
	//	{
	//		for(int i = 0; i < 2; i++) {
	//			ADebug.MemorySetAnchor_();
	//			bool noGC = GC.TryStartNoGCRegion(10_000_000);
	//			var a = new byte[50_000_000];
	//			for(int j = 0; j < a.Length; j++) a[j] = 1;
	//			AOutput.Write(noGC, GCSettings.LatencyMode == GCLatencyMode.NoGCRegion);
	//			if(noGC && GCSettings.LatencyMode == GCLatencyMode.NoGCRegion) try { GC.EndNoGCRegion(); } catch(InvalidOperationException ex) { ADebug.Print(ex.Message); }
	//			ADebug.MemoryPrint_();
	//			GC.Collect();
	//			if(!ADialog.ShowYesNo("Continue?")) break;
	//		}

	//	}

	class TestGC
	{
		~TestGC() {
			if (Environment.HasShutdownStarted) return;
			if (AppDomain.CurrentDomain.IsFinalizingForUnload()) return;
			AOutput.Write("GC", GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
			//ATimer.After(1, _ => new TestGC());
			//var f = App.Wmain; if(!f.IsHandleCreated) return;
			//f.BeginInvoke(new Action(() => new TestGC()));
			new TestGC();
		}
	}
	static bool s_debug2;

	public static void MonitorGC() {
		//if(!s_debug2) {
		//	s_debug2 = true;
		//	new TestGC();

		//	//ATimer.Every(50, _ => {
		//	//	if(!s_debug) {
		//	//		s_debug = true;
		//	//		ATimer.After(100, _ => new TestGC());
		//	//	}
		//	//});
		//}
	}

	//static ushort atom = AWnd.More.RegisterWindowClass("uuuuuuu", (w, m, wp, lp) => {
	//	AWnd.More.PrintMsg(w, m, wp, lp);
	//	return Api.DefWindowProc(w, m, wp, lp);
	//});

	//static bool classRegistered;

	//static LPARAM _Wndproc(AWnd w, int m, LPARAM wp, LPARAM lp)
	//{
	//	AWnd.More.PrintMsg(w, m, wp, lp);
	//	return Api.DefWindowProc(w, m, wp, lp);
	//}

	//_TODO: recompile when a resource file in a folder renamed etc
	//public class ResourceHelper
	//{
	//	ResourceManager _man;

	//	public ResourceHelper(Assembly assembly = null) {
	//		assembly ??= Assembly.GetCallingAssembly();
	//		_man = new ResourceManager(assembly.GetName().Name + ".g", assembly); //_TODO: no ResourceManager in completion list after new
	//	}

	//	//	public ResourceHelper(ResourceManager resMan) {
	//	//		_man=resMan ?? throw new ArgumentNullException();
	//	//	}

	//	public ResourceManager Manager => _man;

	//	public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

	//	public string GetString(string name) => _man.GetString(name, Culture) ?? throw new NotFoundException("Resource not found: " + name);

	//	public byte[] GetBytes(string name) => (_man.GetObject(name, Culture) as byte[]) ?? throw new NotFoundException("Resource not found: " + name);

	//	public UnmanagedMemoryStream GetStream(string name) => _man.GetStream(name, Culture) ?? throw new NotFoundException("Resource not found: " + name);

	//	public object GetObject(string name) => _man.GetObject(name, Culture) ?? throw new NotFoundException("Resource not found: " + name);
	//}


	public static unsafe void TestEditor() {
		//var st = EdResources.GetObjectNoCache("Au");
		//AOutput.Write(st.GetType());

		//var m = new ResourceHelper();
		//AOutput.Write(m.GetObject("resources/hacker.txt").GetType());

		//new System.Windows.Application();
		////var bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Aedit;component/resources/png/folder.png"));
		////var bitmapImage = new BitmapImage(new Uri("pack://application:,,,/resources/png/folder.png"));
		//var bitmapImage = new BitmapImage(new Uri("/resources/png/folder.png", UriKind.Relative));
		//AOutput.Write(bitmapImage);

		//Task.Run(() => {
		//	for (; ; ) {
		//		200.ms();
		//		GC.Collect();
		//	}
		//});

		//InsertCode.UsingDirective("System.Windows.Forms;System.Drawing");

		//InsertCode.ImplementInterfaceOrAbstractClass(AKeys.IsScrollLock);

		var doc = Panels.Editor.ZActiveDoc;
		var Z = doc.Z;
		//var s = doc.Text;
		//AOutput.Write(Z.CurrentPos16);

		//AOutput.Write(SciCode.ZFindScriptHeader(s, out var m));

		//int pos = Z.CurrentPos16;

		//for(int i = pos; i < pos+4; i++) {
		//	AOutput.Write((int)s[i], s[i]>' ' ? s[i] : ' ');
		//}

		//doc.Call(SCI_SETVIEWEOL, 1);
		//doc.Call(SCI_SETREADONLY, 1);

		//doc.Text=s.Replace("\n", "");

		//AOutput.Write(FilesModel.IsWorkspaceZip(@"C:\Users\G\Documents\Au\@Script3.zip"));


		//if(AKeys.IsShift) {
		//	AWnd.Find("*Notepad").Activate();
		//	ATime.SleepDoEvents(100);
		//}

		//var m = new AMenu("name");
		//m["A"] = o => AOutput.Write(o);
		//m.Add(new ToolStripTextBox());
		//var cb = new ToolStripComboBox();
		//for(int i=0;i<60;i++) cb.Items.Add("aaa");
		//m.Add(cb);
		//m["B"] = o => AOutput.Write(o);
		//using(m.Submenu("sub")) {
		//	m["C"] = o => AOutput.Write(o);
		//	m.Add(new ToolStripTextBox());
		//}
		//m.Show(App.Wmain);

		//var task = "_Au.Editor";
		//bool exists = WinTaskScheduler.TaskExists("Au", task);
		//AOutput.Write(exists);
		//if(exists) WinTaskScheduler.DeleteTask("Au", task);
		//else WinTaskScheduler.CreateTaskToRunProgramOnDemand("Au", task, UacIL.System, AFolders.ThisAppBS + "Au.CL.exe", "/s $(Arg0)");
		//AOutput.Write("ok");

		//AOutput.Clear();



		//AOutput.Write(doc.Z.CurrentPos16);

		//z.Select(false, 300, 295);
		//z.ReplaceRange(false, 295, 300, "RE");
		//z.ReplaceRange(false, 300, 295, "RE");
		//z.ReplaceRange(false, 295, 300, "RE", true);
		//z.ReplaceRange(false, 300, 295, "RE", true);
		//z.InsertText(false, 300, "INS");

		//Debug.Assert(false);

		//CodeInfo.Stop();

		//AOutput.Write(z.CurrentPos16);

		//int i = z.SelectionStart8, j = z.SelectionEnd8;
		//doc.Call(SCI_STARTSTYLING, i);
		//doc.Call(SCI_SETSTYLING, j - i, 31);

		//int i = z.LineFromPos(false, z.CurrentPos8);
		//AOutput.Write(i + 1, (uint)doc.Call(SCI_GETFOLDLEVEL, i));
		//AOutput.Write(AWnd.ThisThread.FocusedControl);

		//int line = doc.Call(SCI_GETLINECOUNT);
		////AOutput.Write(doc.Len8, doc.Call(SCI_POSITIONFROMLINE, line), doc.Call(SCI_POSITIONFROMLINE, line+1), doc.Call(SCI_POSITIONFROMLINE, -1));
		//int len = doc.Len8;
		////AOutput.Write(len);
		////AOutput.Write(z.LineStart(false, -1));
		////AOutput.Write(doc.Call(SCI_GETLINECOUNT), doc.Call(SCI_LINEFROMPOSITION, len), doc.Call(SCI_LINEFROMPOSITION, len + 100));
		////AOutput.Write(len, z.LineEnd(false, line + 1), z.LineEnd(false, line + 1, true));
		////AOutput.Write(len, z.LineStart(false, line + 1), z.LineStartFromPos(false, len+1));
		////AOutput.Write(len, z.LineEndFromPos(false, len+1), z.LineEndFromPos(false, len+1, true));
		//AOutput.Write(len, doc.Pos8(len), doc.Pos16(len));
		////AOutput.Write(doc.Pos8(len+100000));
		//AOutput.Write(doc.Pos16(len+1));

		//int pos8 = z.CurrentPos8, pos16= Encoding.UTF8.GetCharCount(Encoding.UTF8.GetBytes(s), 0, pos8);
		//AOutput.Write(pos8, pos16, doc.Pos8(pos16), doc.Pos16(pos8));
		//return;

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 7; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { doc.Pos8(pos16); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { doc.Pos16(pos8); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { doc.TestCreatePosMap(); }
		//	APerf.NW();
		//	Thread.Sleep(100);
		//}

	}
	//SciCode.ITempRange _tr;
	//bool _testOvertype;

	//not finished converting to WPF. Now would need to test only HwndHost controls.
	//static unsafe void SetHookToMonitorCreatedWindowsOfThisThread()
	//{
	//	_hook = AHookWin.ThreadCallWndProcRet(x => {
	//		if(x.msg->message == Api.WM_CREATE) {
	//			if(App.Wmain.IsVisible) return;
	//			var w = x.msg->hwnd;
	//			var p = w.Get.DirectParent; if(p.Is0) p = w.OwnerWindow;
	//			var c = Control.FromHandle(w.Handle); //always null in CBT hook proc
	//			var s = c?.ToString() ?? "";
	//			AOutput.Write($"<><c 0xcc00>{w} ({s}), {p.Handle}</c>");

	//			//if(c is Au.Controls.AToolStrip) { //never mind: .NET bug: if toolstrip Custom1 has overflow and window is maximized, creates parked handle
	//			//	int stop = 0;
	//			//}
	//		}
	//	});
	//	Application.Current.Exit += (_, _) => _hook.Dispose(); //without it at exit crashes (tested with raw API and not with AHookWin) 
	//}
	//static AHookWin _hook;
}
#endif
