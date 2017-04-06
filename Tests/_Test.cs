using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

using Catkeys;
using static Catkeys.NoClass;


//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

using Catkeys.Triggers;


//using Cat = Catkeys.Input;
//using Meow = Catkeys.Show;

//using static Catkeys.Show;

using System.Xml.Serialization;
using System.Xml.Schema;

using Microsoft.VisualBasic.FileIO;
using System.Globalization;

//for LikeEx_
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

using VB = Microsoft.VisualBasic.FileIO;

//using ImapX;
//using System.Data.SQLite;
using SQLite;

//using CsvHelper;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

using static Test.CatAlias;

[module: DefaultCharSet(CharSet.Unicode)]
//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

#pragma warning disable 162, 168, 219, 649 //unreachable code, unused var/field


static partial class Test
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		//Perf.Next();
		//Perf.Write();

#if true
		TestMain();
#elif true
		var d = AppDomain.CreateDomain("AppDomain");
		TestAppDomainDoCallback(d);
		TestAppDomainUnload(d);
		//#else
		//		var d = AppDomain.CreateDomain("AppDomain");

		//		new Thread(() =>
		//		{
		//			Wait(1);
		//			Print("unload");
		//			TestAppDomainUnload(d);
		//		}).Start();

		//		TestAppDomainDoCallback(d);
#endif

		//for(int i = 0; i < 1; i++) {
		//	var t = new Thread(() =>
		//	  {
		//		  var d = AppDomain.CreateDomain("AppDomain" + i);
		//		  TestAppDomainDoCallback(d);
		//		  TestAppDomainUnload(d);
		//	  });
		//	t.SetApartmentState(ApartmentState.STA);
		//	t.Start();
		//}
		//Wait(10);
	}

	static void TestAppDomainDoCallback(AppDomain d)
	{
		try {
			d.DoCallBack(TestMain);
		}
		catch(ThreadAbortException e) { Thread.ResetAbort(); }
		catch(Exception e) { PrintList("exception executing AppDomain", e); }
		Print("DoCallBack ended");
	}

	static void TestAppDomainUnload(AppDomain d)
	{
		try { AppDomain.Unload(d); } catch(Exception e) { PrintList("exception unloading AppDomain", e); }
		Print("Unload ended");
	}

	#region old_test_functions

	//This class used to test serialization.
	//Now probably does not work, must be not a nested class, but I want to collapse the code.
	public class Ooo
	{
		[XmlAttribute]
		public uint style, styleMask, exStyle, exStyleMask;
		//[XmlAttribute(DataType ="string")] //cannot be applied to object
		public object x, y; //int or double, not used if null
							//[XmlAttribute] //cannot be applied to nullable
							//public int? x, y;
		[XmlAttribute]
		public string control, propName, wfName;
		//[XmlAttribute] //cannot be applied to LPARAM
		//public LPARAM propValue_;
		[XmlAttribute]
		public long propValue;

		public Ooo() { }

		//speed: TextFieldParser is 8 times slower than XML serialization
		public Ooo(string csv)
		{
			using(var tr = new StringReader(csv)) { //the usings make 30% faster, but still very slow, much slower than XML serialization
				using(var p = new TextFieldParser(tr)) {
					p.SetDelimiters(new string[] { "=" });

					while(!p.EndOfData) {
						string[] a = p.ReadFields();
						if(a.Length != 2) break;
						//Print(a, "|");
						string s = a[1];
						switch(a[0]) {
						case "style": style = (uint)s.ToInt32_(); break;
						case "control": control = s; break;
						case "x": x = s.ToInt32_(); break;
						case "propValue": propValue = s.ToInt32_(); break;
						}
					}
				}
			}
		}
	}

	static void TestCsvSerialization()
	{
		string s;
		s = "<style>0x10</style><control>CCCCCCCCCCCCCC</control><x>5</x><propValue>100</propValue>";
		s = "style=0x10\ncontrol=CCCCCCCCCCCCCC\nx=5\npropValue=100";
		s = "style=0x10 \n control=CCCCCCCCCCCCCC \n x=5 \n propValue=100";
		s = "style=0x10[]control=CCCCCCCCCCCCCC[]x=5[]propValue=100";
		s = "style=0x10|control=CCCCCCCCCCCCCC|x=5|propValue=100";
		//the winners
		s = "<style>0x10</style>  <control>CCCCCCCCCCCCCC</control>  <x>5</x>  <propValue>100</propValue>";
		s = "style='0x10' control='CCCCCCCCCCCCCC' x='5' propValue='100'";
		s = "style=0x10 |control=CCCCCCCCCCCCCC |x=5 |propValue=100";


		s = "style=0x10 \n control=CCCCCCCCCCCCCC \n x=5 \n propValue=100";
		Ooo v = null;

		var a1 = new Action(() => { v = new Ooo(s); });

		for(int i = 0; i < 5; i++) {
			Perf.First();
			Perf.Execute(1000, a1);
			Perf.Write();
		}

		PrintList(v.style, v.x, v.control, v.propValue);
	}

	static void TestCsv()
	{
		string s = @"a1,b1, c1
a2, ""b2 """" aaa
bbb"", b3
";
		var tr = new StringReader(s);
		var p = new TextFieldParser(tr);
		//Print(p.HasFieldsEnclosedInQuotes);
		//Print(p.TrimWhiteSpace);
		p.SetDelimiters(new string[] { "," });

		while(!p.EndOfData) {
			string[] a = p.ReadFields();
			Print(a.Length);
			Print(a, "|");
		}
	}

	static void TestSerialization()
	{
		//var m = new Wnd._FindProperties();
		//m.style = "0x12 0x12";
		//var x3 = new XmlSerializer(typeof(Wnd._FindProperties));
		//var t3 = new StringWriter();
		//x3.Serialize(t3, m);
		//Print(t3.ToString());
		//return;

		//var v = new Ooo() { style = 0x123, x = 5, control = "Ccccc", propValue = 10 };
		var v = new Ooo() { style = 0x12, control = "Ccccc", x = 5 };

		var x = new XmlSerializer(typeof(Ooo));
		var t = new StringWriter();
		x.Serialize(t, v);
		string s = t.ToString();
		Print(s);

		//s = "<Ooo><control>hhhhh</control></Ooo>";
		s = "<Ooo control='CCCCCCCCCCCCC' style='12'/>";

		//Deserialization ok if no <?xml...>, ok if no xmlns, ok if some members missing, ok if there are unknown members.
		//Fails if type name does not match (my func can add the root tag automatically; also there are attributes to set XML element name).

		var x2 = new XmlSerializer(typeof(Ooo));
		Ooo v2 = null;
		var a1 = new Action(() => v2 = (Ooo)x2.Deserialize(new StringReader(s)));
		a1();

		for(int i = 0; i < 5; i++) {
			Perf.First();
			Perf.Execute(1000, a1);
			Perf.Write();
		}
		//PrintList(v2.style, v2.x, v2.control, v2.propValue);
		PrintList(v2.style, v2.x, v2.control);

	}

	static void TestTaskDialog()
	{
		//TaskDialog.Show(Wnd0, "text", icon:TDIcon.Warning);
		//TaskDialog.Show("text", "", "!");
		//TaskDialog.Show("text", "", new System.Drawing.Icon(@"q:\app\find.ico"));

		//TaskDialog.Show(Wnd0, "text", flags: TDFlags.RawXY);
		//TaskDialog.ShowEx(Wnd0, "text", flags: TDFlags.RawXY, x:-100);
		//TaskDialog.Show("text", null, "r");
		//TaskDialog.ShowEx("text", null, "r", x:-100);

		//Task.Run(() =>
		//{
		//	Wait(5);
		//	//Script.Option.RtlLayout = true;
		//	//Script.Option.ScreenIfNoOwner = 2;
		//	TestDialogScreen("thread");
		//      });

		////Script.Option.ScreenIfNoOwner = 1;
		////TestDialogScreen("main");

		////var f = new Form();
		////f.ShowDialog();

		//Wait(10);


		//TaskDialog.ShowNoWait(null, "Text."); //simplest example
		//var td=TaskDialog.ShowNoWait(ed => { Print(ed); }, "Text.", style: "OCi");
		//var td=TaskDialog.ShowNoWaitEx(ed => { Print(ed); }, "Text.", "text", "OCi", Wnd0, "1 Cust", "1 ra", "Check", "exp", "foo", "Tii", 100, -100, 30);
		//Wait(3); //do something while the dialog is open in other thread
		//td.ThreadWaitClosed(); //wait until dialog closed (optional, but if the main thread will exit before closing the dialog, dialog's thread then will be aborted)


		//TaskDialog.Show("aaa");

		//bool marquee = false;
		//var pd = TaskDialog.ShowProgress(marquee, "Working", customButtons: "1 Stop", y: -1);
		////var pd = TaskDialog.ShowProgressEx(marquee, "Working", "ttt", "a", Wnd0, "1 Stop", "1 r1|2 r2", "Check", "exp", "foo", "Tii", 100, -1, 30);
		//for(int i = 1; i <= 100; i++) {
		//	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		//	if(!marquee) pd.Send.Progress(i);
		//	Thread.Sleep(50); //do something in the loop
		//}
		//pd.Send.Close();

		//Print(TaskDialog.ShowList("1 one|2 two|3 three\r\n").ToString());
		//Print(TaskDialog.ShowList("1 One|2 Two|3 Three|Cancel", "Main instruction.", "More info.", "Cxb").ToString());
		//Print(TaskDialog.ShowList("1 One|2 Two|3 Three|Cancel", "Main instruction.", "More info.", TDIcon.App).ToString());
		//Print(TaskDialog.ShowListEx("1 One|2 Two|3 Three|Cancel", "Main instruction.", "More info.", TDIcon.App, Wnd0, "", "exp\r\n<a href=\"mmm\">link</a>", "foo: <a href=\"mmm\">link</a>", "Moo", -1, 100, 30, (ed)=>Print(ed.linkHref)).ToString());

		//TaskDialog.ShowList("1|2|3|4|5|6|7|8|9|10|11|12|13|14|15|16|17|18|19|20|21|22|23|24|25|26|27|28|29|30");
		//TaskDialog.ShowList("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA BBBBBBBBBBBBBBBBBBBBBBBBB");
		//Print(TaskDialog.ShowListEx("1 one|2 two|3 three\r\n", footerText: "!|foo").ToString());
		//return;

		//var f = new Form();
		//f.Show();

		//Script.Option.RtlLayout=true;
		//string s; //int i;
		//if(!TaskDialog.ShowInput(out s)) return;
		//if(!TaskDialog.ShowInput(out s, "Text.", ownerWindow: Wnd.Find("Untitled - Notepad"))) return;
		//if(!TaskDialog.ShowInput(out s, "Text gggggggggggg.")) return;
		//if(!TaskDialog.ShowInput(out s, "Text.", "Default")) return;
		//if(!TaskDialog.ShowInputEx(out s, "Text.", "0", editType: TDEdit.Number, expandedText:"exp")) return;
		//if(!TaskDialog.ShowInput(out i, "Text.", 5)) return; Print(i); return;
		//if(!TaskDialog.ShowInputEx(out s, "Text.", "pas", editType: TDEdit.Password)) return;
		//if(!TaskDialog.ShowInput(out s, "Text.", "one\r\ntwo\r\nthree", editType: TDEdit.Multiline)) return;
		//if(!TaskDialog.ShowInput(out s, "Text.", "def\none\r\ntwo\nthree", editType: TDEdit.Combo)) return;
		//if(!TaskDialog.ShowInputEx(out s, "Text.", "def\none\r\ntwo\nthree", Wnd0, TDEdit.Combo, "i", "exp", "foo", "Tii", 200, -100, 30, "1 Browse...", ed => { if(ed.wParam == 1) { string _s; if(TaskDialog.ShowInput(out _s, ownerWindow:ed.hwnd)) ed.dialog.EditControl.SetControlText(_s); ed.returnValue = 1; } })) return;
		//if(!TaskDialog.ShowInputEx(out s, "Text.", "def\none\r\ntwo\nthree", Wnd0, TDEdit.Combo, "i", "exp", "foo", "Tii", 200, -100, 30, "", ed => { if(ed.wParam == TDResult.OK) { string _s=ed.dialog.EditControl.Name; if(Empty(_s)) { TaskDialog.Show("Text cannot be empty.", ownerWindow: ed.hwnd); ed.returnValue = 1; } } })) return;
		//if(!TaskDialog.ShowInputEx(out s, "Text.", "def\none\r\ntwo\nthree", Wnd0, TDEdit.Combo, "i", "exp", "foo", "Tii", 200, -100, 30, "1 Browse...", ed=>
		//{
		//	if(ed.wParam != 1) return;
		//	string _s; if(TaskDialog.ShowInput(out _s, ownerWindow:ed.hwnd)) ed.dialog.EditControl.SetControlText(_s);
		//	ed.returnValue = 1;
		//})) return;
		//if(!TaskDialog.ShowInputEx(out s, "Text.")) return;
		//if(!TaskDialog.ShowInputEx(out s, "Text.", footerText:"a|Foooooo.")) return;
		//if(!TaskDialog.ShowInputEx(out s, "Text.", footerText:"a|Foooooo.", timeoutS:30)) return;
		//bool ch;
		//if(!TaskDialog.ShowInputEx(out s, out ch, "Check", "Text.", "one\r\ntwo\r\nthree", editType:TDEdit.Multiline, expandedText:"More\ntext.", timeoutS: 60)) return;
		//if(!TaskDialog.ShowInput(out s, out ch, "Check", "Text.", "txt", editType:TDEdit.Multiline)) return;
		//Print(s);
		//Print(ch);

		//if(false) {
		//	d.SetCustomButtons("1 Browse...", true, true);
		//	d.ButtonClicked += e => { if(e.wParam == 1) { Print("Browse"); e.returnValue = 1; } };
		//}

		//MessageBox.Show("ddddddddddddddddddd");
		//TaskDialog.MessageDialog("fffffffffff");

		//return;


		//TaskDialogAsync("async"); //warning "consider applying await"
		//TDResult rr=await TaskDialogAsync("async"); //error, the caller must be marked with async. But then fails to run altogether.

		//Task<TDResult> t=TaskDialogAsync("async");
		//TaskDialog.Show("continue");
		//t.Wait();
		//Print(t.Result);

		//var pd = ShowNoWait("async", y=>Print(y));
		//var pd = ShowNoWait("async");
		//Wait(2);
		//if(pd.IsOpen) pd.Send.Close();
		//Print(pd.Result);

		//var td = new TaskDialog("dddd");
		//Task.Run(() => td.ShowDialog());
		////Perf.First();
		////td.ThreadWaitOpen();
		////Perf.NW();
		//td.ThreadWaitClosed();
		////Task.Run(() => td.ShowDialog());
		////td.ThreadWaitOpen();
		////td.ThreadWaitClosed();
		//Print(td.Result);

		//TaskDialog.Show("continue", y:300);


		//Print("finished");

		//Task t = Task.Run(() =>
		//{
		//	//Thread.Sleep(100);

		//	//Print("run");
		//	Print(TaskDialog.Show("async", style: "OC", x: 1));
		//	//MessageBox.Show("another thread");
		//	//TaskDialog.MessageDialog("async",style:"OC");
		//	//TD("async", true);
		//}
		//);

		////Thread.Sleep(7);

		//Print(TaskDialog.Show("continue", style: "OC"));
		////TD("continue", false);
		////TaskDialog.MessageDialog("continue",style:"OC");
		////Thread.Sleep(1000);
		//t.Wait();
		//Print("after all");

		//for(int i=0; i<5; i++) TD("continue", false);

		//Print(GetThemeAppProperties());
		//MessageBox.Show("sss");
		//Print(TaskDialog.MessageDialog("test", MDButtons.OKCancel, MDIcon.App, MDFlag.DefaultButton2));
		//Print(TaskDialog.MessageDialog("One\ntwooooooooooooo."));
		//Print(TaskDialog.MessageDialog("One\ntwooooooooooooo.", "YNC!t2"));
		//TaskDialog.MessageDialog("One\ntwooooooooooooo.");
		//Print(Wnd.ActiveWindow);

		//Print(TaskDialog.Show(Wnd0, "Head1\nHead2.", "Text1\nText2.", TDButtons.OKCancel, TDIcon.App, TDFlags.CommandLinks, TDResult.Cancel, "1 one|2 two", new string[] { "101 r1|||", "102 r2" }, "Chick|check", "expanded", "", "TTT", 0, 0, 20).ToString());
		//Print(TaskDialog.Show("Head1\nHead2.", "Text1\nText2.", "OCd2!t", Wnd0, "1 one|2 two", null, null, "expanded", "foo", 60, "TTT").ToString());
		//Print(TaskDialog.Show(Wnd0, "Head1\nHead2.", "Text1\nText2.", TDButtons.OKCancel|TDButtons.YesNo|TDButtons.Retry|TDButtons.Close, TDIcon.Info).ToString());
		//Print(TaskDialog.Show(Wnd0, "Head1\nHead2.", "Text1\nText2.", TDButtons.OKCancel|TDButtons.YesNo|TDButtons.Retry|TDButtons.Close, (TDIcon)0xfff0).ToString());
		//Print(TaskDialog.Show("head", "content", "OCYNLRio", ownerWindow: Wnd.Find("Untitled - Notepad")).ToString());
		//Print(TaskDialog.Show("head", "content", "OCYNLRi", x:100, y:-11, timeoutS:15).ToString());
		//Print(TaskDialog.Show("", "<a href=\"example\">link</a>.", onLinkClick: ed => { Print(ed.linkHref); }).ToString());
		//Print(TaskDialog.Show("head", "content", "i", customButtons: "-1 Mo OK|-2 My Cancel").ToString());

		//Print(TaskDialog.ShowList("1 one| 2 two| 3three|4 four\nnnn|5 five|6 six|7 seven|8 eight|9 nine|10Ten|0Cancel|1 one|2 two|3three|4 four\nnnn|5 five|6 six|7 seven|8 eight|9 nine|10Ten", "Main", "More."));
		//Print(TaskDialog.ShowList(new string[] { "1 one", "2 two", "Cancel" }, "Main", "More").ToString());
		//Print(TaskDialog.ShowList(new List<string> { "1 one", "2 two", "Cancel" }, "Main", "More").ToString());
		////		Print(TaskDialog.ShowList(@"
		////|1 one
		////|2 two
		////comments
		////|3 three
		////" , "Main", "More\r\nmore"));

		////		Print(TaskDialog.ShowList("1 one|2 two\nN|3 three\r\nRN|4 four"));
		//return;

		//var d = new TaskDialog("Head", "Text <A HREF=\"xxx\">link</A>.", TDButtons.OKCancel|TDButtons.Retry, TDIcon.Shield, "Title");
		//var d = new TaskDialog("Head", "Text <A HREF=\"xxx\">link</A>.", TDButtons.OKCancel|TDButtons.Retry, (TDIcon)0xfff0, "Title");
		//var d = new TaskDialog("Head", "Text <A HREF=\"xxx\">link</A>.", (TDButtons)111);
		//var d = new TaskDialog("Head Text.", null, 0, TDIcon.Shield);
		//var d = new TaskDialog("", "More text.", 0, TDIcon.Shield);
		//var d = new TaskDialog();
		var d = new TaskDialog();

		d.SetTitleBarText("MOO");

		d.SetText("Main text.", "More text.\nSupports <A HREF=\"link data\">links</A> if you subscribe to HyperlinkClick event.");

		//d.SetButtons(TDButtons.OKCancel | TDButtons.Retry);

		//d.SetIcon(TDIcon.Warning);
		//d.SetIcon(new System.Drawing.Icon(@"Q:\app\copy.ico", 32, 32)); //OK
		//d.SetIcon(Catkeys.Tasks.Properties.Resources.output); //OK
		//d.SetIcon(new System.Drawing.Icon(Catkeys.Tasks.Properties.Resources.output, 16, 16)); //OK
		//d.SetIcon(new System.Drawing.Icon("Resources/output.ico")); //OK
		//d.SetIcon(new System.Drawing.Icon("Resources/output.ico", 16, 16)); //OK
		//d.SetIcon(new System.Drawing.Icon(typeof(Test), "output.ico")); //exception
		//Print(Catkeys.Tasks.Properties.Resources.output.Width);
		//d.SetIcon(new System.Drawing.Icon(TaskDialog.Resources.AppIconHandle32));
		//d.SetIcon(TDIcon.App);

		Wnd w = Wnd.Find("Untitled - Notepad");
		//d.SetOwnerWindow(w);

		//Script.Option.ScreenIfNoOwner=2;

		d.SetXY(100, 100);

		d.SetCheckbox("Checkbox", false);

		TaskDialog.Options.TopmostIfNoOwnerWindow = true;
		//d.FlagTopmost=true;
		d.FlagXCancel = true;
		d.FlagCanBeMinimized = true;
		//d.FlagRtlLayout=true;
		//d.FlagPositionRelativeToWindow=true;
		//d.FlagNoTaskbarButton = true;
		//d.FlagNeverActivate = true;

		//Script.Option.ScreenIfNoOwner=2;
		//d.Screen=2;

		d.SetExpandedText("Expanded info\nand more info.", true);

		d.SetExpandControl(true, "Show more info", "Hide more info");

		//d.SetFooterText("Footer text.", TDIcon.Warning);
		//d.SetFooterText("Footer.");
		//d.SetFooterText("Footer.", Catkeys.Tasks.Properties.Resources.output); //icon 32x32, srinked
		//d.SetFooterText("Footer.", new System.Drawing.Icon(Catkeys.Tasks.Properties.Resources.output, 16, 16)); //icon OK
		//d.SetFooterText("Footer text.", new System.Drawing.Icon(@"q:\app\wait.ico", 16, 16));

		//d.Width=700;

		//d.SetButtons(new string[] { "101 one", "102 two" });
		d.SetButtons("1 one|2 two\nzzz", true);
		//d.SetCustomButtons(new string[] { "5", "102 two" }, true);
		//d.SetCustomButtons("101|102 two\nzzz", true);
		d.DefaultButton = 2;
		//d.SetDefaultButton(2);
		//d.SetDefaultButton(TDResult.Cancel);
		//d.SetDefaultButton(TDResult.Retry);

		//d.SetRadioButtons(new string[] { "1001 r1", "1002 r2" }, 1002);
		d.SetRadioButtons("1001 r1|1002 r2");

		//d.SetTimeout(10, "Cancel");
		//d.SetTimeout(10, null, true);

		//d.Created += ed => { Print($"{ed.message} {ed.wParam} {ed.linkHref}"); };
		d.Created += ed => { ed.dialog.Send.EnableButton(1, false); };
		//d.Created += ed => { ed.OwnerWindow.Enabled=true; };
		//d.Created += ed => { ed.hwnd.Owner.Enabled=true; };
		//d.Created += ed => { Wnd.Get.Owner(ed.hwnd).Enabled=true; };
		//d.Created += ed => { w.Enabled=true; };
		//d.Destroyed += ed => { Print($"{ed.message} {ed.wParam} {ed.linkHref}"); };
		d.ButtonClicked += ed => { Print($"{ed.message} {ed.wParam} {ed.LinkHref}"); if(ed.wParam == 2) ed.returnValue = 1; };
		d.HyperlinkClicked += ed => { Print($"{ed.message} {ed.wParam} {ed.LinkHref}"); };
		//d.OtherEvents += ed => { Print($"{ed.message} {ed.wParam} {ed.linkHref}"); };
		//d.Timer += ed => { Print($"{ed.message} {ed.wParam} {ed.linkHref}"); };
		//d.HelpF1 += ed => { Print($"{ed.message} {ed.wParam} {ed.linkHref}"); };

		//d.FlagShowProgressBar = true; d.Timer += ed => ed.dialog.Send.Progress(ed.wParam / 100);

		//Perf.First();
		TDResult r = d.ShowDialog();
		//Perf.NW();

		Print(r.ToString());

		//} catch(ArgumentException e) { Print($"ArgumentException: {e.ParamName}, '{e.Message}'"); } catch(Exception e) { Print($"Exception: '{e.Message}'"); }
		//#endif
	}

	static void TestFolders()
	{

		//Print(Path_.IsFullPath(@""));
		//Print(Path_.IsFullPath(@"\"));
		//Print(Path_.IsFullPath(@"\\"));
		//Print(Path_.IsFullPath(@"c:"));
		//Print(Path_.IsFullPath(@"c:\"));
		//Print(Path_.IsFullPath(@"c:aa"));
		//Print(Path_.IsFullPath(@"c\dd"));
		//Print(Path_.IsFullPath(@"%aa"));
		//Print(Path_.IsFullPath(@"<ff"));
		//Print(Path_.IsFullPath(@"%temp%"));
		//Print(Path_.IsFullPath(@"<ff>"));

		//Print(Path_.Combine(@"%temp%\..", null));
		//Print(Path_.Combine(@"%emp%\..", null));
		//Print(Path_.Combine(@"%temp", null));
		//Print(Path_.Combine(@"<ccc>", null));
		//Print(Path_.Combine(@"<ccc", null));

		//Print(Path_.Combine(@"c:\one", "two"));
		//Print(Path_.Combine(@"c:one", "two"));
		//Print(Path_.Combine(@"c:", "two"));
		//Print(Path_.Combine(@"\\one", "two"));

		//Print(Path_.Combine(null, @"c:\one"));
		//Print(Path_.Combine(null, @"c:one"));
		//Print(Path_.Combine(null, @"c:"));
		//Print(Path_.Combine(null, @"\\one"));
		//Print(1);
		//Print(Path_.Combine("one", "two"));
		//Print(Path_.Combine("one", null));
		//Print(Path_.Combine(null, "two"));
		//Print(Path_.Combine(@"one\", null));
		//Print(Path_.Combine(null, @"\two"));
		//Print(Path_.Combine(@"c:\one\", null));
		//Print(2);
		//Print(Path_.Combine("one", @"\two"));
		//Print(Path_.Combine(@"one\", "two"));
		//Print(Path_.Combine(@"one\", @"\two"));
		//Print(Path_.Combine("one", @"a:\two"));
		//Print(Path_.Combine("one", @"a:two"));
		//Print(3);
		//Print(Path_.Combine(null, @"C:\PROGRA~2\Acer\LIVEUP~1\updater.exe"));
		//Print(Path_.Combine(null, @"C:PROGRA~2\Acer\LIVEUP~1\updater.exe"));
		//Print(Path_.Combine(null, @"..\aaa.bbb"));
		//Print(Path_.Combine(@"C:\PROGRA~2\Acer\LIVEUP~1\..\updater.exe", null));
		//Print(Path_.Combine("C:\\PROGRA~2\\Acer\\LIVEUP~1\nupdater.exe", null));
		//Print(Path_.Combine(@"c:\one~", @" space "));

		//Output.Write(Folders.GetKnownFolders());

		//Print(Folders.Desktop);
		//Print(Folders.Desktop + "app.end");
		//Print(Folders.Desktop + "..\\file.txt");
		//Print(Folders.Desktop.ToString());
		//Print(Folders.Virtual.ComputerFolder + "mmm");
		//Print(Folders.Desktop + "app" + ".end");
		//Print(Folders.Profile);
		//Print(Folders.Virtual.ControlPanelFolder);
		//Print(Folders.CommonPrograms + "append.this");
		//Print(Folders.ApplicationShortcuts_Win8);
		//Print(Folders.Temp + "file.c");
		//Print(Folders.ThisApp + "file.c");
		//Print(Folders.ThisAppTemp + "file.c");
		//Print(Folders.ThisAppDocuments + "file.c");
		//Print(1);
		//Print(Folders.GetFolder("Start menu") + "append.nnn");
		//Print(Folders.GetFolder("APp") + "append.nnn");
		//Print(Folders.EnvVar("Temp") + "append.txt");
		//Print(Folders.GetFolder("UnknownFolder") + "append.nnn"); //throws
		//Print(Folders.EnvVar("unknown") + "append.txt"); //throws
		//Print(2);
		//Print(Folders.CDDrive());
		//Print(Folders.CDDrive() + "in CDDrive.txt");
		//Print(Folders.RemovableDrive(0));
		//Print(Folders.RemovableDrive(0) + "in Removable.txt");
		//Print(Folders.RemovableDrive(1) + "in Removable.txt");
		//Print(Folders.RemovableDrive("PortableApp") + "in Removable.txt");
		//Print(Folders.RemovableDrive("PortableApps.com") + "in Removable.txt");
		//Print(3);
		//Print(Folders.Desktop + @"\file.txt");
		//Print($@"{Folders.Desktop}\file.txt");
		//Print(Folders.Desktop + @"\file.txt");
		//Print(Folders.Desktop + @"C:\file.txt");
		//Print(Folders.Desktop);
		//Print(Folders.Desktop + "file.txt");
		//Print(Folders.Desktop + "..\\file.txt");

		//Print(Path.GetFullPath(@"c:\a\b\c."));


		//Process.Start("notepad.exe");
		//Process.Start(@"::{26EE0668-A00A-44D7-9371-BEB064C98683}\0\::{A3DD4F92-658A-410F-84FD-6FBBBEF2FFFE}");

		//var pi = new ProcessStartInfo("notepad.exe");
		//pi.


		//Guid g=default(Guid); string s=null/*, s2=null*/;
		//var a1 = new Action(() => { g = new Guid("{82A5EA35-D9CD-47C5-9629-E15D2F714E6E}"); });
		//var a2 = new Action(() => { g = new Guid(0xB4BFCC3A, 0xDB2C, 0x424C, 0xB0, 0x29, 0x7F, 0xE9, 0x9A, 0x87, 0xC6, 0x41); });
		//var a3 = new Action(() => { s = _Get(ref g); });
		////var a4 = new Action(() => { s2 = _Get(man, ref g); });
		////var a4 = new Action(() => { s = $"{_Get(ref g)}\\etc\\etc.etc"; });
		//var a4 = new Action(() => { s = Folders.Desktop; });
		//var a5 = new Action(() => { s = $"{Folders.Desktop}\\file.txt"; });
		//var a6 = new Action(() => { s = Folders.DesktopAnd("file.txt"); });
		//var a7 = new Action(() => { s = Path.Combine(Folders.Desktop, "file.txt"); });

		//a1();
		//a2();
		//a3();
		//a4();
		//a5();
		//a6();

		//Print(s);
		////Print(s2);

		//for(int i=0; i<5; i++) {
		//	Perf.First();
		//	Perf.Execute(1000, a1);
		//	Perf.Execute(1000, a2);
		//	Perf.Execute(1000, a3);
		//	Perf.Execute(1000, a4);
		//	Perf.Execute(1000, a5);
		//	Perf.Execute(1000, a6);
		//	Perf.Execute(1000, a7);
		//	Perf.Write();
		//}

		//Task.Run(()=> Directory.SetCurrentDirectory("c:\\windows\\system32"));
		//Wait(1);
		//Print(Path.GetFullPath("..\\nnn.ttt"));

		//Print(File.Exists(@"%windir%\System32\notepad.exe"));
		//Print(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
	}

	static void TestWndAll()
	{
		foreach(Wnd w in Wnd.Misc.AllWindows()) {
			Print(w);
		}
		Print("ok");

		//Wnd wq = Wnd.Find(null, "QM_Editor");
		//Print(wq.GetClassLong(Api.GCW_ATOM));

		//Print(Wnd.GetClassAtom("Edit"));
		//Print(Wnd.GetClassAtom("QM_Editor"));
		//Print(Wnd.GetClassAtom("QM_Editor", Util.Misc.GetModuleHandleOfProcessExe()));

		//var ac = new string[] { "IME", "MSCTFIME UI", "tooltips_class32", "ComboLBox", "WorkerW", "VBFloatingPalette" };
		//foreach(string s in ac) Print(Wnd.GetClassAtom(s));

		//return;

		//List<Wnd> a = null;

		////var a1 = new Action(() => { a = Wnd.AllWindows(); });
		////var a1 = new Action(() => { a = Wnd.AllWindows(null, true); });
		////var a1 = new Action(() => { a = wy.AllChildren(); });
		////var a2 = new Action(() => { wy.AllChildren((c, e) => { }); });
		//var a1 = new Action(() => { a = Wnd.AllWindows(); });
		//var a2 = new Action(() => { Wnd.AllWindows((w3, e) => { }); });

		////wy.AllChildren((c, e)=> { Print(c); }, null, true); return;
		////Wnd.AllWindows((c, e)=> { Print(c); }, "QM_*"); return;
		////Wnd.AllWindows((c, e)=> { Print(c); }, null, true); return;
		////foreach(Wnd w3 in Wnd.AllWindows(onlyVisible:true)) { Print(w3); }; return;
		////a1();

		////var a2 = new Action(() =>
		////{
		////	foreach(Wnd t in a) {
		////		string s = t.ClassName;
		////	}
		////});

		//int n = 10;

		//for(int k = 0; k < 5; k++) {
		//	Perf.First();
		//	Perf.Execute(n, a1);
		//	Perf.Execute(n, a2);
		//	Perf.Write();
		//}
		//Print(a.Count);

		//Wnd wq = Wnd.Find(null, "QM_Editor");

		//var f = new Form();
		//for(int i=0; i<20; i++) {
		//	var b = new Button();
		//	b.Location = new POINT(0, i * 20);
		//	b.Size = new SIZE(50, 18);
		//	f.Controls.Add(b);
		//}

		//f.Click += (o, e) =>
		//{
		//	Wnd hform = (Wnd)((Form)o).Handle;

		//	List<Wnd> a = null;

		//	var a1 = new Action(() => { a=hform.AllChildren(); });
		//	//var a1 = new Action(() => { a=wq.AllChildren(); });
		//	//a1();

		//	for(int k=0; k<5; k++) {
		//		Perf.First();
		//		Perf.Execute(1000, a1);
		//		Perf.Write();
		//	}
		//	Print(a);
		//      };

		//f.ShowDialog();


		////Output.Write(Wnd.All.ThreadWindows(wq.ThreadId));
		////Output.Write(Wnd.All.ThreadWindows(wq.ThreadId, "QM_toolbar"));
		//Output.Write(Wnd.All.ThreadWindows(wq.ThreadId, "", true));

		//TaskDialog.ShowEx("", "<a href=\"test\">test</a>", onLinkClick: ed =>
		//{
		//	var a = Wnd.All.ThreadWindows();
		//	Output.Write(a);
		//});

		//return;



		//var gu= new Guid("{82A5EA35-D9CD-47C5-9629-E15D2F714E6E}");
		//Print(gu);

		//Print(SpecFolder.CommonStartup);
		//Print(SpecFolder.Desktop);



		////string g;
		//////g = "ąčęėįšųūž абв αβγ";
		////g = "ĄČĘĖĮŠŲŪŽ АБВ ΑΒΓ";

		//////Print(g.ToUpper());
		//////Print(g.ToUpper_());
		////Print(g.ToLower());
		////Print(g.ToLower_());



		////return;

		////Wnd w=(Wnd)1245464;
		////List<Wnd> e1 = w.AllChildren();
		////Print(e1.Count);
		//////IEnumerable<Wnd> e= w.DirectChildControlsEnum();
		////List<Wnd> e2 = w.DirectChildControlsFastUnsafe();
		////Print(e2.Count);

		////Perf.First(100);
		////for(int uu=0; uu<5; uu++) {
		////	Perf.First();
		////	Perf.Execute(1000, () => { e2 =w.DirectChildControlsFastUnsafe(); });
		////	Perf.Execute(1000, () => { e1 =w.AllChildren(); });
		////	Perf.Execute(1000, () => { e1 = w.AllChildren("Button", true); });
		////	Perf.Execute(1000, () => { w.AllChildren(c=> { /*Print(c);*/ return false; }; });
		////	Perf.Write();
		////}

	}

	static void TestProcesses()
	{
		Wnd w = Wnd0;
		WFFlags hiddenToo = WFFlags.HiddenToo;

		//Process_.EnumProcesses(p =>
		//{
		//	Print(p.ProcessName);
		//	return false;
		//});
		//return;

		//w =Wnd.Find("", program: "YouTubeDownloader");
		//w =Wnd.Find("", program: @"C:\Program Files (x86)\Free YouTube Downloader\YouTubeDownloader.exe");
		//Print(w);
		//Print(w.ProcessName);
		//Print(w.ProcessPath);
		//return;

		//Print(Process_.GetProcessName(7140));
		//Print(Process_.GetProcessName(1988));
		//Print(Process_.GetProcessName(1988, true));

		//string pn1=null, pn2 = null;
		//var a11 = new Action(() => { pn1=Process_.GetProcessName(1988); }); //qmserv
		//var a12 = new Action(() => { pn2=Process_.GetProcessName(7140); }); //firefox
		//Perf.ExecuteMulti(5, 1000, a11, a12);
		//Print(pn1); Print(pn2);
		//return;

		var a1 = new Action(() => { w = Wnd.Find("*Notepad", flags: hiddenToo); });
		var a2 = new Action(() => { w = Wnd.Find("*Notepad", "Notepad", flags: hiddenToo); });
		var a3 = new Action(() => { w = Wnd.Find("*Notepad", "Notepad", "Notepad", flags: hiddenToo); });
		var a4 = new Action(() => { w = Wnd.Find("*Notepad", "Notepad", "NotepaD.exE", flags: hiddenToo); });
		var a5 = new Action(() => { w = Wnd.Find("*Notepad", "Notepad", @"c:\windows\syswow64\Notepad.exe", flags: hiddenToo); });
		//var a6 = new Action(() => { w = Wnd.Find("", "", "NotepaD.exE", flags: hiddenToo); });
		//var a7 = new Action(() => { w = Wnd.Find("", "", @"c:\windows\syswow64\Notepad.exe", flags: hiddenToo); });
		var a6 = new Action(() => { w = Wnd.Find("", "", "no NotepaD.exE", flags: hiddenToo); });
		var a7 = new Action(() => { w = Wnd.Find("", "", @"c:\no windows\syswow64\Notepad.exe", flags: hiddenToo); });
		var a8 = new Action(() => { w = Wnd.Find("", "", "youtubedownloader", flags: hiddenToo); });

		//a6(); Print(w); return;

		a1(); Print(w);
		a2(); Print(w);
		a3(); Print(w);
		a4(); Print(w);
		a5(); Print(w);
		a6(); Print(w);
		a7(); Print(w);
		a8(); Print(w);

		Perf.ExecuteMulti(5, 1, a1, a2, a3, a4, a5, a6, a7, a8);

		//Wnd w = (Wnd)12582978; //Notepad

		//string s = null;

		//s = w.ProcessName;
		//Print(s);
		//s = w.ProcessPath;
		//Print(s);

		//var a1 = new Action(() => { s = w.ProcessName; });
		//var a2 = new Action(() => { s = w.ProcessPath; });

		//Perf.ExecuteMulti(5, 1, a1, a2);

		//Process[] a=null;

		//Perf.First(100);
		//a = Process.GetProcesses();
		//Perf.Next();
		//a = Process.GetProcesses();
		//Perf.Next();
		//a = Process.GetProcesses();
		//Perf.Next();
		//a = Process.GetProcessesByName("NoTepad");
		//Perf.Next();
		//a = Process.GetProcessesByName("NoTepad");
		//Perf.Next();
		//a = Process.GetProcessesByName("NoTepad");
		//Perf.Next();
		//Perf.Write();

		//int pid = a[0].Id;

		//Process u = null;
		//string name = null;

		//Perf.First();
		//u=Process.GetProcessById(pid);
		//Perf.Next();
		//name = u.ProcessName;
		//Perf.Next();
		//u=Process.GetProcessById(pid);
		//Perf.Next();
		//name = u.ProcessName;
		//Perf.Next();
		//u=Process.GetProcessById(pid);
		//Perf.Next();
		//name = u.ProcessName;
		//Perf.Next();
		//Perf.Write();

		//Print(name);

		//a = Process.GetProcesses();
		//Print(a.Length);
		//a = Process.GetProcessesByName("NoTepad");
		//Print(a.Length);
		//a = Process.GetProcessesByName("regedit");
		//Print(a.Length);
		//foreach(Process p in a) {
		//	Print(p.Id);
		//	//Print(p.Handle);
		//}
	}

	static void TestProcessUacInfo()
	{
		////Print(Process_.UacInfo.ThisProcess.IntegrityLevel);
		////Print(Api.GetModuleHandle("shell32"));
		////Print(Api.GetModuleHandle("advapi32"));
		//bool is1 = false, is2 = false, is3=false;
		//Perf.SpinCPU(200);
		//Perf.ExecuteMulti(5, 1, () => { is1 = Process_.UacInfo.IsThisProcessAdmin; }, () => { is2 = Api.IsUserAnAdmin(); }, () => { is3 = IsUserAdmin; });
		//PrintList(is1, is2, is3);
		////Print(Api.GetModuleHandle("shell32"));
		////Print(Api.GetModuleHandle("advapi32"));

		//Print(Process_.UacInfo.IsUacDisabled);
		Print(Process_.UacInfo.IsAdmin);
		return;

		Print(Process_.UacInfo.IsAdmin);

		Process[] a = Process.GetProcesses();
		for(int i = -5; i < a.Length; i++) {
			Process x = null;
			Process_.UacInfo p = null;

			Perf.First();
			if(i < 0) p = Process_.UacInfo.ThisProcess;
			else {
				x = a[i];
				Print($"<><Z 0x80c080>{x.ProcessName}</Z>");
				p = Process_.UacInfo.GetOfProcess((uint)x.Id);
			}
			if(p == null) { Print("failed"); continue; }
			Perf.Next();
			var elev = p.Elevation; if(p.Failed) Print("<><c 0xff>Elevation failed</c>");
			Perf.Next();
			var IL = p.IntegrityLevel; if(p.Failed) Print("<><c 0xff>IntegrityLevel failed</c>");
			Perf.Next();
			var IL2 = p.IntegrityLevelAndUIAccess;
			Perf.Next();
			bool uiaccess = p.IsUIAccess; if(p.Failed) Print("<><c 0xff>IsUIAccess failed</c>");
			Perf.Next();
			bool appcontainer = p.IsAppContainer; if(p.Failed) Print("<><c 0xff>IsAppContainer failed</c>");
			Perf.Next();
			Print($"elev={elev}, uiAccess={uiaccess}, AppContainer={appcontainer}, IL={IL}, IL+uiaccess={IL2}");
			//Print($"uiAccess={uiaccess}, IL={IL}");
			//Perf.Write();
		}
	}

	static void TestRegexSpeed()
	{
		Regex r = null;
		string p = @"^\w:\\.+\\\w+\.\w\w\w$";
		string s = @"c:\jshgdjhasdhjahjdashjdjhas\ksjhdkjshdkj\file.txt";
		bool yes = false;

		var a1 = new Action(() => { r = new Regex(p); });
		var a2 = new Action(() => { yes = r.IsMatch(s); });
		var a1i = new Action(() => { r = new Regex(p, RegexOptions.IgnoreCase); });
		var a2i = new Action(() => { yes = r.IsMatch(s); });
		var a1o = new Action(() => { r = new Regex(p, RegexOptions.CultureInvariant); });
		var a2o = new Action(() => { yes = r.IsMatch(s); });
		var a1oi = new Action(() => { r = new Regex(p, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase); });
		var a2oi = new Action(() => { yes = r.IsMatch(s); });

		a1(); a2(); Print(yes);
		a1i(); a2i(); Print(yes);
		a1o(); a2o(); Print(yes);
		a1oi(); a2oi(); Print(yes);

		Perf.ExecuteMulti(5, 1000, a1, a2, a1i, a2i, a1o, a2o, a1oi, a2oi);

		//compiling: Regex 4500, PCRE 450.
		//match (invariant, match case): Regex 1200, PCRE 300.
		//match (invariant, ignore case): Regex 1700, PCRE 300.
	}

	static void TestProcessMemory()
	{
		//var w = Wnd.FindFast("QM_Editor");
		var w = Wnd.FindFast(null, "Notepad");
		Print(w);
		Process_.Memory x = null;
		try {
			x = new Process_.Memory(w, 1000);
			//x = new Process_.Memory(w, 0);

		}
		catch(CatException e) { Print(e); return; }

		//Print(1);
		//Print(x.WriteUnicodeString("Unicode"));
		//Print(2);
		//Print(x.WriteAnsiString("ANSI", 100));
		//Print(3);
		//Print(x.ReadUnicodeString(7));
		//Print(4);
		//Print(x.ReadAnsiString(4, 100));
		//Print(5);

		unsafe
		{
			int i1 = 5, i2 = 0;
			//Print(x.Write(&i1, 4));
			//Print(x.Read(&i2, 4));
			//Print(x.Write(&i1, 4, 50));
			//Print(x.Read(&i2, 4, 50));
			Print(x.WriteOther(x.Mem, &i1, 4));
			Print(x.ReadOther(x.Mem, &i2, 4));
			Print(i2);
		}
	}

	static void TestDotNetControls()
	{
		var w = Wnd.Find("Free YouTube Downloader", "*.Window.*");
		//var w = Wnd.Find("Keyboard Layout*", "*.Window.*");
		//var c = w.Child("", "*.SysListView32.*");
		Print(w);
		if(w.Is0) return;
		//Print(c);
		var x = new Wnd.Misc.WinFormsControlNames(w);

		////Print(x.GetControlNameOrText(c));
		//Print(x.GetControlNameOrText(w));

		var a = w.AllChildren();
		foreach(Wnd k in a) {
			Print("---");
			Print(k);
			Print(Wnd.Misc.WinFormsControlNames.IsWinFormsControl(k));
			Print(x.GetControlName(k));
		}
	}

	static void TestDotNetControls2()
	{
		//Wnd w = Wnd.Find("Keyboard*").Child("Caps");
		//Wnd w = Wnd.Find("Keyboard*");
		//Wnd w = Wnd.Find("Quick*");
		Wnd w = Wnd.Find("Free YouTube*").Child("My*");
		Print(w);
		//Print(WinFormsControlNames.GetSingleControlName(w));

		string s1 = null, s2 = null;
		var a1 = new Action(() =>
		{
			try {
				using(var x = new Wnd.Misc.WinFormsControlNames(w)) { s1 = x.GetControlName(w); }
			}
			catch { s1 = null; }
		});
		var a2 = new Action(() => { s2 = Wnd.Misc.WinFormsControlNames.GetSingleControlName(w); });

		Perf.ExecuteMulti(5, 10, a1, a2);
		PrintList(s1, s2);
	}

	static void TestWndFind()
	{
		Wnd w = Wnd0, c = Wnd0;
		//w = Wnd.Find("", "", "dwm", true);
		w = Wnd.Find(null, "QM_Editor");
		//w = Wnd.Find("Quick Macros*");
		//w = Wnd.Find("*[*]");
		//w = Wnd.Find("Quick Macros - ok - [Macro2773]");
		//w = Wnd.Find("[f]Quick Macros - ok - [Macro2773]");
		//w = Wnd.Find("[f]Quick Macros - ");
		//w = Wnd.Find("[f]Quick Macros - *");
		//w = Wnd.Find("[p]Quick Macros - ");
		//w = Wnd.Find("[r]^Quick Macros - ");
		//w = Wnd.Find("[r]^quick macros - ");
		//w = Wnd.Find("[ri]^quick macros - ");
		//w = Wnd.Find("[ric]^quick macros - ");
		//w = Wnd.Find(null, "qm_Editor");
		//w = Wnd.Find("", "qm_Editor");
		//w = Wnd.Find("", "[-i]qm_Editor");
		//w = Wnd.Find(null, "QM_Editor"); //test when hidden
		//w = Wnd.Find(null, "QM_Editor", "", true); //test when hidden
		//w = Wnd.Find(WFFlags.HiddenToo, null, "QM_Editor"); //test when hidden
		//w = Wnd.Find(null, "QM_*");
		//w = Wnd.Find(null, null, "*env");
		//w = Wnd.Find(null, null, "Q:\\*");
		//w = Wnd.Find(WFFlags.ProgramPath, null, null, "Q:\\*");
		//w = Wnd.Find(WFFlags.ProgramPath, null, null, "c:\\*");
		//w = Wnd.Find(null, "QM_*", matchIndex:2);
		//w = Wnd.Find(null, "QM_*", prop:new Wnd.WinProp() {childClass="QM_Code" } );
		//w = Wnd.Find(null, "QM_*", prop:new Wnd.WinProp() {owner=Wnd.Find("Quick Macros*") } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {processId= Wnd.Find("Quick Macros*").ProcessId } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {threadId=Wnd.Find("*Notepad").ThreadId } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {propName= "qmshex" } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {propName= "qmshex", propValue=1 } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {style=Native.WS_CAPTION, styleMask=Native.WS_CAPTION } );
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() { styleHas = Native.WS_CAPTION });
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() { styleNot = Native.WS_CAPTION });
		//w = Wnd.Find("* Notepad", null, prop: new Wnd.WinProp() { styleNot = Native.WS_DISABLED, styleHas = Native.WS_CAPTION });
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() { exStyleHas = Native.WS_EX_NOREDIRECTIONBITMAP });
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() { exStyleNot = Native.WS_EX_TOPMOST });
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() {  });
		//w = Wnd.Find(null, "QM_*", also:e=> { Print(e.w); if(e.w.Name == "TB MSDEV") e.Stop(); });
		//w = Wnd.Find(null, "QM_*", also:e=> { Print(e.w); e.w = Wnd0; e.Stop(); });
		//w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childClass="QM_Code" });
		//w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childName="Te&xt" });
		//w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childId = 2202 });
		//w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childClass = "Button", childName = "Te&xt" });
		//w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childClass = "*Edit", childName = "sea" });
		//w = Wnd.Find("Free YouTube Downloader", "*.Window.*");
		//w = Wnd.Find("Keyboard Layout*", "*.Window.*");
		//w = Wnd.Find("Catkeys -*");
		//w = Wnd.Find("", prop: new Wnd.WinProp() { x=Screen_.Width-1, y=Screen_.Height-10 });
		//w = Wnd.Find("", prop: new Wnd.WinProp() { x=0.5, y=1.1 });
		//w = Wnd.FromXY(1532, 1224, null);
		//w = Wnd.FromXY(0.1, 0.1, null, true, true);

		//Perf.ExecuteMulti(5, 100, () => { Wnd.FindFast("QM_Editor"); }, () => { Wnd.Find(null, "QM_Editor"); });

		//w = Wnd.FindFast("QM_Editor");
		Print(w);
		//return;

		//c = w.Child(2202);
		//c = w.Child("Open*");
		//c = w.Child(null, "SysListView32", 2212);
		//c = w.Child(null, "SysListView32");
		//c = w.Child(null, "", 2212);
		//c = w.Child(null, "SysListView32", 0, true);
		//c = w.Child(null, "SysListView32", 0, true, matchIndex:2);
		//c = w.Child(null, "SysListView32", matchIndex:2);
		//c = w.Child(WCFlags.HiddenToo, null, "SysListView32", 0);
		//c = w.Child("sea");
		//c = w.Child("Regex*");
		//c = w.Child(WCFlags.ControlText, "sea");
		//c = w.Child(WCFlags.DirectChild, "", matchIndex:4);
		//c = w.Child(null, "Button", also: e => { Print(e.w); });
		//c = w.Child(null, "Button", also: e => { Print(e.w); if(e.w.Name == "Te&xt") e.Stop(); });
		//c = w.Child(null, "", prop:new Wnd.ChildEtc() { exStyleHas=Native.WS_EX_CLIENTEDGE});
		//c = w.Child(null, "", prop:new Wnd.ChildEtc() { styleHas=Native.WS_BORDER});
		//c = w.Child(null, "Button", prop:new Wnd.ChildEtc() { x=344, y=448});
		//c = w.Child(null, "", prop:new Wnd.ChildEtc() { x=0.5, y=0.03});
		//c = w.Child(null, "", prop:new Wnd.ChildEtc() { childId=1028 });
		//c = w.Child(null, "", prop: new Wnd.ChildEtc() { child = new Wnd.ChildParams(WCFlags.DirectChild, "Reg*") });
		//c = w.Child("Regex*", "Button");
		//c = w.Child(null, "", prop: new Wnd.ChildEtc() { wfName = "textBoxUrl" });
		//try {
		//	Wnd w = Wnd.Find("Keyboard*");
		//	Wnd c = w.Child("", prop: new Wnd.ChildEtc() { wfName = "ckControl" });
		//	Print(c);
		//} catch(CatException e) {
		//	Print(e);
		//}
		//c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildEtc() { x=1 });
		//c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildEtc() { x=1276});
		//c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildEtc() { x=w.ClientWidth-1 });
		//c = w.Child(null, "QM_*", prop:new Wnd.ChildEtc() { y=0.99 });
		//c = w.Child("Find &Previous");
		//c = w.Child("Find Previous");

		Print(c);
		return;

		//Wnd c1 = Wnd0, c2 = Wnd0, c3 = Wnd0, c4 = Wnd0, c5 = Wnd0, c6 = Wnd0, c7 = Wnd0;
		//var a1 = new Action(() => { c1 = w.Child("sea"); });
		//var a2 = new Action(() => { c2 = w.Child("sea", flags: WCFlags.NameIsValue); });
		//var a3 = new Action(() => { c3 = w.Child("sea", "*Edit"); });
		//var a4 = new Action(() => { c4 = w.Child("sea", "*Edit", WCFlags.NameIsValue); });
		//var a5 = new Action(() => { c5 = w.Child("Regex*"); });
		//var a6 = new Action(() => { c6 = w.Child("Regex*", "Button"); });
		////var a7 = new Action(() => { c7 = w.Child("1028", flags: WCFlags.NameIsId); });
		//var a7 = new Action(() => { c7 = w.Child(1028); });
		//Perf.ExecuteMulti(5, 100, a1, a2, a3, a4, a5, a6, a7);
		//Output.WriteListSep("\n", c1, c2, c3, c4, c5, c6, c7);

		Wnd c1 = Wnd0, c2 = Wnd0, c3 = Wnd0, c4 = Wnd0, c5 = Wnd0, c6 = Wnd0, c7 = Wnd0;
		var a1 = new Action(() => { c1 = w.Child("sea"); });
		var a2 = new Action(() => { c2 = w.Child("**text:sea"); });
		var a3 = new Action(() => { c3 = w.Child("sea", "*Edit"); });
		var a4 = new Action(() => { c4 = w.Child("**text:sea", "*Edit"); });
		var a5 = new Action(() => { c5 = w.Child("Regex*"); });
		var a6 = new Action(() => { c6 = w.Child("Regex*", "Button"); });
		//var a7 = new Action(() => { c7 = w.Child("1028", flags: WCFlags.NameIsId); });
		var a7 = new Action(() => { c7 = w.Child("**id:1028"); });
		Perf.ExecuteMulti(5, 100, a1, a2, a3, a4, a5, a6, a7);
		Output.WriteListSep("\n", c1, c2, c3, c4, c5, c6, c7);

		//Print(Process.GetProcessesByName("qm")[0].MainWindowTitle); //TB INTERNET

		//var w = Wnd.Find(null, "QM_Editor");
		//Print(Wnd.Get.NextMainWindow());
		//Print(Wnd.Get.NextMainWindow(w));
		//Print(Wnd.Get.NextMainWindow(w));

		//var a=Wnd.All.MainWindows();
		//foreach(Wnd w in a) {
		//	Print(w);
		//}

		//var x = new Wnd.WinProp() { owner = Wnd0, exStyle = 8 };
		//var y = new Wnd.WinProp(owner: Wnd0, style: 8);
		//TestProp(new Wnd.WinProp() { owner = Wnd0, exStyle = 8 });
		//TestProp(new Wnd.WinProp(owner: Wnd0, style: 8));

		//var a1 = new Action(() => { Wnd.AllWindows(e => { }); });
		//var a2 = new Action(() => { var a = Wnd.All.ThreadWindows(7192); /*Print(a == null); Print(a.Count);*/ });
		////a2();

		//Print("---");
		//Perf.ExecuteMulti(5, 1000, a1, a2);

		//Wnd w = (Wnd)1705264;

		//var a1 = new Action(() => { RECT r = w.Rect; });
		//var a2 = new Action(() => { uint st = w.Style; });
		//var a3 = new Action(() => { uint est = w.ExStyle; });
		//var a4 = new Action(() => { string name = w.Name; });
		//var a5 = new Action(() => { string cn = w.ClassName; });
		//var a6 = new Action(() => { uint tid = w.ThreadId; });
		//var a7 = new Action(() => { Wnd own = w.Owner; });
		//var a8 = new Action(() => { bool clo=w.IsCloaked; });

		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6, a7, a8);
	}

	static void TestWndFromXY()
	{
		Wnd w1 = Wnd0, w2 = Wnd0, w3 = Wnd0, w4 = Wnd0, w5 = Wnd0, w6 = Wnd0;

		var a1 = new Action(() => { var p = Mouse.XY; w1 = Api.WindowFromPoint(p); });
		var a2 = new Action(() => { w2 = Wnd.FromMouse(); });
		//var a3 = new Action(() => { w3 = Wnd.FromXY2(Mouse.XY); });
		//var a4 = new Action(() => { w4 = Wnd.FromXY3(Mouse.XY); });

		for(;;) {
			Wait(1);
			if(Mouse.X == 0 && Mouse.Y == 0) break;
			//a4(); Print(w4); continue;
			Print("---------------------------");
			a1(); a2(); //a3(); a4();
			Perf.ExecuteMulti(3, 1, a1, a2);
			Print(w1);
			Print(w2);
			//Print(w3);
			//Print(w4);
		}

	}

	static void TestChildFromXY()
	{
		Wnd w1 = Wnd.Find("Options");
		Print(w1);
		Print(w1.ChildFromXY(43, 132));
		Print(w1.ChildFromXY(43, 132, true));
		Print(w1.ChildFromXY(1265, 1243, false, true));
		Print(w1.ChildFromXY(1, 1)); //coord not in a child
		Print(w1.ChildFromXY(43, 932)); //coord outside
	}

	[DllImport("user32.dll", EntryPoint = "InternalGetWindowText", SetLastError = true)]
	public static extern int InternalGetWindowTextSB(Wnd hWnd, [Out] StringBuilder pString, int cchMaxCount);

	static void TestMemory2(Wnd w)
	{
		var sb = new StringBuilder(1000);
		InternalGetWindowTextSB(w, sb, 1000);
		sb.Clear();
		//sb.Length = 0;
		//sb.Capacity = 0;

	}

	static void TestMemory3(Wnd w)
	{
		var sb = new StringBuilder();
		for(int i = 16; i < 1000000; i *= 2) {
			sb.Capacity = i;
			InternalGetWindowTextSB(w, sb, i);

		}
		string R = sb.ToString();
		Perf.First();
		sb.Clear();
		sb.Length = 0;
		sb.Capacity = 0;
		Perf.NW();
		Print(R);
	}

	static void TestMemory()
	{
		//Wnd w = Wnd.Find("", "QM_Editor");
		//Print(w);
		////Print(w.Name);
		////Print(w.ControlText);
		////return;

		//while(TaskDialog.Show("test", "", TDButtons.OKCancel) == TDResult.OK) {
		//	for(int i = 0; i < 1; i++) {
		//		TestMemory3(w);
		//	}
		//	TaskDialog.Show("allocated 2 MB");
		//}
	}

	static void TestStringPlusConcatInterpolation()
	{
		string s;
		if(Time.Milliseconds > 1000) s = "sjhdkjshdjkshjkdhsjkhdjkshdjkhsjkdhjskhdjksh"; else s = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
		string s1 = null, s2 = null, s3 = null, s4 = null, s5 = null, s6 = null;

		var a1 = new Action(() => { s1 = "<IDLIST:" + s + ">"; });                          //40
		var a2 = new Action(() => { s2 = string.Concat("<IDLIST:", s, ">"); });             //40
		var a3 = new Action(() => { s3 = "<IDLIST:" + s + ">" + s + " tail"; });            //125
		var a4 = new Action(() => { s4 = string.Concat("<IDLIST:", s, ">", s, " tail"); }); //125
		var a5 = new Action(() => { s5 = $"<IDLIST:{s}>"; });                               //160
		var a6 = new Action(() => { s6 = $"<IDLIST:{s}>{s} tail"; });                       //220

		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6);
		Print(s1);
		Print(s2);
		Print(s3);
		Print(s4);
		Print(s5);
		Print(s6);

		//Print(Folders.Virtual.ControlPanelFolder);
	}

	static void TestArrayAndList(IList<string> a)
	{
		Print(a.Count);
		//for(int)
	}

	static void TestWndFindAll()
	{
		//Print(Wnd.FindAll(0, "*I*"));
		Wnd w = Wnd.Find("Quick*");
		Print(w.ChildAll("", "QM*"));
	}

	static bool _Activate(Wnd w)
	{
		if(!Wnd.Misc.AllowActivate()) return false;
		Api.SetForegroundWindow(Wnd.WndActive);
		Api.SetForegroundWindow(Wnd.WndActive);
		Api.SetForegroundWindow(Wnd.WndActive);
		Perf.First();
		if(!Api.SetForegroundWindow(w)) return false;
		Perf.Next();
		for(int i = 0; i < 10; i++) { Print(Wnd.WndActive); Thread.Sleep(1); }
		//Thread.Sleep(100);
		Perf.Write();
		if(w.IsActive) return true;

		uint tid = w.ThreadId;
		if(tid != 0 && tid == Wnd.WndActive.ThreadId && Api.SetForegroundWindow(Wnd.Misc.WndRoot)) {
			Api.SetForegroundWindow(w);
			Wnd t = Wnd.WndActive;
			Print(t);
			if(t.ThreadId == tid) return true;
		}

		return false;
	}

	static void TestWndActivateFocus()
	{
		//Wnd.ActiveWindow.ShowMinimized();
		//Print(Wnd.ActiveWindow);
		//Print(Wnd.SwitchActiveWindow(true));
		//return;

		//Wait(2);
		//var a = new Api.INPUTKEY[] { new Api.INPUTKEY(65, 0), new Api.INPUTKEY(65, 0, Api.IKFlag.Up), new Api.INPUTKEY(66, 0), new Api.INPUTKEY(66, 0, Api.IKFlag.Up)};
		//Api.SendInputKey(a);
		//Print("ok");
		//return;

		//Thread.CurrentThread.Priority = ThreadPriority.Highest;
		Wait(2);

		//Print(Wnd.LockActiveWindow(true));
		//return;

		//Wnd w=Wnd.Find("Quick*");
		//Wnd w=Wnd.Find("*Notepad");
		//Wnd w=Wnd.Find("[p]Paint");
		Wnd w = Wnd.Find("Options");
		//Wnd w=Wnd.Find(Wnd.Find("Microsoft Excel*").Name.EndsWith_("dictionary.xls") ? "Book1.xls" : "dictionary.xls");
		Print(w);
		//Print(w.ActivateLL());
		w.Activate();
		//Print(Api.SetForegroundWindow(Wnd.Get.WndRoot));
		//Thread.Sleep(100);
		//Print(Api.SetForegroundWindow(w));
		//Print(_Activate(w));
		//Thread.Sleep(100);
		Print(Wnd.WndActive);

		Wnd c = w.Child("Show*");
		c.Focus();
		Print(Wnd.WndFocused);
		Print(Wnd.WndFocusedLL);

		//TaskDialog.Show("a");
		return;
		Wait(2);
		w = Wnd.Find("[p]Notepad");
		Print(w.ActivateLL());
		Print(Wnd.WndActive);
	}

	static void TestWndMinMaxRes()
	{
		//Wnd w = Wnd.Find("", "XLMAIN");
		//Wnd w = Wnd.Find("Book1.xls");
		//Wnd w = Wnd.Find("[p]Dreamweaver");
		//Wnd w = Wnd.Find("app -*", "wndclass_desked_gsk");
		//Wnd w = Wnd.Find("* Notepad");
		Wnd w = Wnd.Find("Registry*");
		//Print(w);

		//w.Show(false);
		//Thread.Sleep(1000);

		//w.ActivateLL(); return;

		//w.Activate(); Wait(1); //return;

		//Wnd w = Wnd.Find("Dialog MM");
		//w.ShowMinimized();
		//w.ShowMinimized(true);
		//w.ShowMaximized(); //Registry does not work. Access denied.
		//w.ShowNotMinMax();
		w.ShowNotMinimized();
		//Wnd.Misc.Arrange.MinimizeWindows();

		Print(Wnd.WndActive);
		return;


		//if(false) {
		//	w.ShowNotMinMax();
		//	Wait(1);
		//	w.ShowMaximized();
		//	return;

		//	w.ShowMinimized();
		//	//Print(w.IsMinimized);
		//	Print(Wnd.ActiveWindow);
		//	Wait(1);
		//	w.ShowNotMinimized();
		//	//Print(w.IsMinimized);
		//	Print(Wnd.ActiveWindow);

		//	//w.ShowMaximized();
		//	//w.ShowNotMinMax();
		//} else {
		//	var m = true;

		//	//Print(w.ShowNotMinMax(m));
		//	//Wait(1);
		//	//Print(w.ShowMaximized(m));
		//	//return;

		//	w.ShowMinimized(m);
		//	//Print(w.IsMinimized);
		//	Print(Wnd.ActiveWindow);
		//	Wait(1);
		//	w.ShowNotMinimized(m);
		//	//Print(w.IsMinimized);
		//	Print(Wnd.ActiveWindow);

		//	//Print(w.ShowMaximized(m));
		//	//Print(w.ShowNotMinMax(m));
		//}

		//Print("ok");
	}

	static void TestWindowDimensions()
	{
		//Wnd w=Wnd.Find("Quick*", "QM_*");
		Wnd w = Wnd.Find("* Notepad");
		//Wnd w=Wnd.Find("Registry*");
		//Wnd w=Wnd.Find(null, "Dwm", flags:WFFlags.HiddenToo);
		Print(w);

		//Print(w.MoveInScreen(0, 0));
		//Print(w.MoveToScreenCenter(2));

		//Print(w.Child("", "*Tree*", prop: new Wnd.ChildEtc() { y=0.5 }));
		//Print(Wnd.Find("", "QM_*", prop: new Wnd.WinProp() { x=0.5 }));

		w.Activate();
		//w.MoveLL(300, 100, 600, 200);
		//w.Width = 500;
		//w.MoveLL(100, 30, 500, 300);
		//w.MoveLL(100, 30, null, null);
		//w.MoveLL(null, null, 500, 300);
		//w.MoveLL(null, null, 500, null);
		//w.MoveLL(null, null, null, 500);
		//w.MoveLL(100, null, null, null);
		//w.MoveLL(null, 100, null, null);
		//w.MoveLL(null, 100, null, 300);
		//w.MoveLL(300, 100);
		//w.MoveLL(null, 100);
		//w.ResizeLL(300, 100);
		//w.ResizeLL(null, 100);
		//w.ResizeLL(300, null);

		//w.Move(300, 100, 600, 200);
		//w.Move(100, 30, 500, 300);
		//w.Move(100, 30, null, null);
		//w.Move(null, null, 500, 300);
		//w.Move(null, null, 500, null);
		//w.Move(null, null, null, 500);
		//w.Move(100, null, null, null);
		//w.Move(null, 100, null, null);
		//w.Move(null, 100, null, 300);
		//w.Move(500, 100);
		//w.Move(null, 100);
		//w.Resize(300, 100);
		//w.Resize(null, 100);
		//w.Resize(300, null);

		//w = Wnd0;
		//Print(w.Height);
		//Print(w.IsNotMinMax);
		//Print(ThreadError.ErrorText);


		PrintList(w.X, w.Y, w.Width, w.Height);
		PrintList(w.ClientWidth, w.ClientHeight);

		//w.X = 500;
		//w.Y = 500;
		//w.Width = 500;
		//w.Height = 500;

		//RECT r = w.Rect;
		//r.Inflate(20, 20);
		//w.Rect = r;

		//RECT r = w.ClientRect;
		//Print(r);
		//r.Inflate(20, 20);
		//w.ClientRect = r;

		//w.ClientWidth = 300;
		//Print(w.ClientWidth);
		//w.ClientHeight = 300;
		//Print(w.ClientHeight);

		//RECT rw, rc;
		//if(w.GetWindowAndClientRectInScreen(out rw, out rc)) PrintList(rw, rc);
	}

	static void TestWndtaskbarButton()
	{
		//Wnd w=Wnd.Find("Quick*", "QM_*");
		Wnd w = Wnd.Find("* Notepad");
		//Wnd w=Wnd.Find("Registry*");
		//Wnd w=Wnd.Find(null, "Dwm", flags:WFFlags.HiddenToo);
		Print(w);

		//Wnd.Misc.TaskbarButton.Flash(w, 0);
		//Wait(3);
		//Wnd.Misc.TaskbarButton.Flash(w, 1);

		//Wnd.Misc.TaskbarButton.SetProgressState(w, Wnd.Misc.TaskbarButton.ProgressState.Error);
		//for(int i = 0; i < 100; i++) {
		//	Wnd.Misc.TaskbarButton.SetProgressValue(w, i + 1);
		//	Thread.Sleep(100);
		//}
		//Wnd.Misc.TaskbarButton.SetProgressState(w, Wnd.Misc.TaskbarButton.ProgressState.NoProgress);

		Wnd.Misc.TaskbarButton.Delete(w);
		Wait(2);
		Wnd.Misc.TaskbarButton.Add(w);
	}

	static void TestWndClose()
	{
		//Wnd w = Wnd.Find("*Notepad*");
		//Wnd w = Wnd.Find("* Internet Explorer*");
		//Wnd w = Wnd.Find("*Dreamweaver*");
		Wnd w = Wnd.Find("Registry*");
		Print(w);
		//Script.Speed = 200;
		Print(w.Close());
		//Wnd.CloseAll("*Notepad*");
	}

	static void TestWndArrange()
	{
		Wnd w = Wnd.Find("*Notepad");
		//Print(w);
		Wnd.Misc.Desktop.ToggleShowDesktop();
		Print("ok");
		Wait(1);
		Wnd.Misc.Desktop.ToggleShowDesktop();
		//Wait(1);
		//Wnd.Misc.Arrange.MinimizeWindows(true);
		//Wait(1);
		//Wnd.Misc.Arrange.MinimizeWindows(false);
		//Wnd.Misc.Arrange.CascadeWindows();
		//Wait(1);
		//Wnd.Misc.Arrange.TileWindows(true);
		//Wait(1);
		//Wnd.Misc.Arrange.TileWindows(false);
	}

	static void TestWndShowHide()
	{
		bool vis = false;
		Wnd w = Wnd.Find("*Notepad", flags: WFFlags.HiddenToo);

		var a1 = new Action(() => { vis = w.IsVisible; });
		var a2 = new Action(() => { Api.ShowWindow(w, vis ? 0 : Api.SW_SHOWNA); });
		//var a2 = new Action(() => { Api.ShowWindow(w, vis?0:Api.SW_SHOW); });
		var a3 = new Action(() => { Api.SetWindowPos(w, Wnd0, 0, 0, 0, 0, (vis ? Native.SWP_HIDEWINDOW : Native.SWP_SHOWWINDOW) | Native.SWP_NOMOVE | Native.SWP_NOSIZE | Native.SWP_NOZORDER | Native.SWP_NOOWNERZORDER | Native.SWP_NOACTIVATE); });
		//var a3 = new Action(() => { Api.SetWindowPos(w, Wnd0, 0, 0, 0, 0, (vis ? Native.SWP_HIDEWINDOW: Native.SWP_SHOWWINDOW)|Native.SWP_NOMOVE|Native.SWP_NOSIZE|Native.SWP_NOZORDER|Native.SWP_NOOWNERZORDER|Native.SWP_NOACTIVATE|Native.SWP_NOSENDCHANGING); });

		//Perf.ExecuteMulti(5, 1000, a1, a2);

		Wnd[] a = new Wnd[10];
		var f = new Form();
		for(int i = 0; i < 10; i++) {
			var b = new Button();
			b.Location = new POINT(0, i * 20);
			b.Size = new SIZE(50, 18);
			f.Controls.Add(b);
			a[i] = (Wnd)b.Handle; Print(a[i]);
		}

		var a10 = new Action(() => { for(int j = 0; j < 10; j++) { w = a[j]; a3(); } });

		f.Click += (o, e) =>
		{
			Perf.ExecuteMulti(5, 10, a1, a10);
			//Perf.ExecuteMulti(5, 1000, a10);
		};

		f.ShowDialog();

	}

	static unsafe void TestRegistry()
	{
		bool ok;

		//Print(Registry_.CatkeysKey);

		//Print(Registry_.CanOpen(@"SOFTWARE\Microsoft"));
		//Print(Registry_.CanOpen(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft"));
		//Print(Registry_.CanOpen(@"SOFTWARE\GinDi", Registry.CurrentUser));
		//Print(Registry_.CanOpen(@"GinDi", Registry.CurrentUser.OpenSubKey("SOFTWARE")));
		//Print(Registry_.CanOpen(null, Registry.CurrentUser.OpenSubKey("SOFTWARE")));
		//return;

		Print("---- int ----");

		Registry_.SetInt(5, "ii", "Test");

		ok = Registry_.GetInt(out int i, "ii", "Test");
		if(!ok) { Print("no"); return; }
		Print(i);

		Print("---- long ----");

		Registry_.SetLong(5, "LLL", "Test");

		ok = Registry_.GetLong(out long L, "LLL", "Test");
		if(!ok) { Print("no"); return; }
		Print(L);

		Print("---- string ----");

		Registry_.SetString("stttttttttttrrrrrr", "SSS", "Test");

		ok = Registry_.GetString(out string s, "SSS", "Test");
		if(!ok) { Print("no"); return; }
		Print(s);

		Print("---- multi string ----");

		Registry_.SetStringArray(new string[] { "one", "two", "three" }, "AAA", "Test");

		ok = Registry_.GetStringArray(out string[] a, "AAA", "Test");
		if(!ok) { Print("no"); return; }
		Print(a);

		Print("---- binary ----");

		var r = new RECT(1, 2, 3, 4, false);
		int n = Marshal.SizeOf(r);
		Registry_.SetBinary(&r, n, "BB", "Test");
		//Registry_.SetBinary(&r, n, "rect2", @"HKEY_CURRENT_USER\Test");
		//Registry_.SetBinary(&r, n, "rect2", @"HKEY_LOCAL_MACHINE\Software\Test");

		RECT r2;
		n = Registry_.GetBinary(&r2, n, "BB", "Test");
		Print(n);
		if(n == 0) { Print("no"); return; }
		Print(r2);
	}

	static void TestWndTransparency()
	{
		Wnd w = Wnd.Find("FileZilla");
		Wnd c = w.ChildById(-31781);
		Print(c);
		c.SetTransparency(true, 0.5);


		//Wnd.Find("Dialog").SetTransparency(true, 0.5, 0xFF);

		//Wnd w = Wnd.Find("*Notepad", "Notepad");
		////w = w.Child(15);
		//Print(w);
		////w.SetTransparency(true, null, 0);
		////w.SetTransparency(true, 0.3);
		////w.SetTransparency(true, 0.3, 0);
		////w.SetTransparency(true);
		//w.SetTransparency(false);
	}

	[ComImport, Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ITaskbarList3
	{
		// ITaskbarList
		[PreserveSig]
		int HrInit();
	}

	static char[] TextFileToCharArray(string file)
	{
		try {
			using(var reader = new StreamReader(file)) {
				long n = reader.BaseStream.Length;
				if(n <= int.MaxValue / 4) {
					var s = new char[n];
					reader.Read(s, 0, (int)n);
					return s;
				}
			}
		}
		catch { }
		return null;
	}

	class Parser
	{
		char[] s, d; //source, destination
		int i, j, n; //index in source, in destination, length

		/// <summary>
		/// i must be at the starting ". Moves i to the ending ".
		/// </summary>
		void SkipString()
		{
			Debug.Assert(s[i] == '\"');
			for(++i; i < n; i++) {
				if(s[i] == '\"') {
					int k = i; while(s[k - 1] == '\\') k--;
					if((k & 1) == 0) break;
				} else d[j++] = s[i];
			}
		}
	}

	//static _StringFold(char s, int i, int )

	static void TestStringFold()
	{
		var s = TextFileToCharArray(@"c:\test\a.txt");
		if(s == null) return;

		int i = 0, j = 0, n = s.Length;
		var d = new char[n];

		for(; i < n; i++) {
			char c = s[i];
			switch(c) {
			case '\"':
				d[j++] = c;
				for(++i; i < n; i++) {
					if(s[i] == '\"') {
						int k = i; while(s[k - 1] == '\\') k--;
						if((k & 1) == 0) break;
					} else d[j++] = s[i];
				}
				break;
			}
		}

		Print(d, "");
	}

	static void TestStrToI()
	{
		var s = "1234567";
		//var s = "0xffffffff";
		var c = s.ToCharArray();

		unsafe
		{
			fixed (char* p = c) {
				Print(s.ToInt32_());
				char* t = p, e;
				Print(Api.strtoi(t, out e));
				Print(Api.strtoui(t, out e));
				Print(Api.strtoi64(t, out e));
				int n;
				Print(Api.strtoi(s, 0, out n)); Print(n);
				Print(Api.strtoui(s, 0, out n)); Print(n);
				Print(Api.strtoi64(s, 0, out n)); Print(n);

				var a1 = new Action(() => { s.ToInt32_(); });
				var a2 = new Action(() => { Api.strtoi(t, out e); });
				var a3 = new Action(() => { Api.strtoui(t, out e); });
				var a4 = new Action(() => { Api.strtoi64(t, out e); });
				var a5 = new Action(() => { Api.strtoi64(s, 0, out n); });

				Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5);
			}
		}
	}

	static void TestStrToI2()
	{
		//var s = "12 ";
		//int len;
		//Print(Api.strtoi(s, 0, out len));
		//Print(len);
		//Print(Api.strtoi(s));

		//Print(Api.strtoi("0x7ffffffe"));
		//Print(Api.strtoi("0x8ffffffe"));
		//Print(Api.strtoi("-0x8ffffffe"));
		//PrintHex(Api.strtoui("0xfffffffe"));
		//PrintHex(Api.strtoui("0xffffffff1"));
		//PrintHex(Api.strtoui("-2"));
		//PrintHex(Api.strtoui("-0x7fffffff"));
		//PrintHex(Api.strtoui("-0x80000000"));

		Print(Api.strtoi64("0x7ffffffffffffffe"));
		Print(Api.strtoi64("0x8000000000000000"));
	}

	static void TestRegexAgain()
	{
		//string p = @"(\d)\d+", r = "$1R";
		//string s = "aaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mm";

		////Print(s.RegexReplace_(out s, p, r));
		////Print(s);

		//string s2 = null; int n = 0;
		//var a1 = new Action(() => { s2 = s.RegexReplace_(p, r); });
		//var a2 = new Action(() => { n = s.RegexReplace_(out s2, p, r); });

		//Perf.ExecuteMulti(5, 1000, a1, a2);

		//PrintList(n, s2);

		//var k = new UNS.TDARRAY();
		////k.a = new int[5];
		//k[0] = 5;
		//Print(k[0]);
	}

	public unsafe class UNS
	{
		public struct HASARRAY
		{
			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public fixed int a[5];
			public fixed char s[5];
			//fixed public char s[5]; //error

			//public fixed POINT p[6]; //error
		}

		public struct USEHASARRAY
		{
			public HASARRAY* p;
		}


		[DllImport("user32.dll", SetLastError = true)]
		public static extern Wnd SetFocus2(Wnd hWnd);

		public struct TDARRAY
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			public int[] a;

			public int this[int i] { get { return a[i]; } set { _Init(); a[i] = value; } }
			void _Init() { if(a == null) a = new int[5]; }
		}
	}

	static unsafe void TestFixedArrayMember()
	{
		//	PrintList(sizeof(UNS.HASARRAY), Marshal.SizeOf(typeof(UNS.HASARRAY)));

		//	var k = new UNS.HASARRAY();

		//	Print(k.a[0]);
		//	k.a[0]=5;
		//	Print(k.a[0]);

		//	//Print(k.a); //error
		//	//Print(k.s); //error

		//	//char[] s = k.s; //error
		//	k.s[0] = 'A';
		//	string s = new string(k.s, 0, 5);
		//	Print(s);
	}

	[DllImport(@"Q:\app\Catkeys\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void TestSimple();

	[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void TestExceptions()
	{
		Print(1);
		try {
			TestSimple();
		}
		catch { Print("exc"); }
		Print(2);

	}

	[ComImport, Guid("3AB5235E-2768-47A2-909A-B5852A9D1868"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface ITest
	{
		[PreserveSig]
		int Test1(int i);
		[PreserveSig]
		int TestOL(ref int i);
		[PreserveSig]
		int TestOL(string s);
		[PreserveSig]
		int TestNext(ref sbyte p);
	};

	[DllImport(@"Q:\app\Catkeys\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern ITest CreateTestInterface();

	static void TestInterfaceMethodOverload()
	{
		var x = CreateTestInterface();
		int k = 0; sbyte b = 0;
		x.TestOL(ref k);
		x.TestOL("ddd"); //calls TestNext

		//x.TestNext(ref b);
		Print("fin");
	}

	[DllImport(@"Q:\app\Catkeys\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
	//static extern void CreateTestInterface2(out ITest t);
	//static extern void CreateTestInterface2(out IntPtr t); //with Marshal.GetTypedObjectForIUnknown is missing 1 Release
	static extern void CreateTestInterface2([MarshalAs(UnmanagedType.IUnknown)] out object t); //the same number of QI etc as with 'out ITest t'
																							   //static extern void CreateTestInterface2(out IUnknown t); //more QI etc

	[ComImport, Guid("00000000-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IUnknown
	{
		//[PreserveSig]
		//IntPtr QueryInterface(ref Guid riid, out IntPtr pVoid);

		//[PreserveSig]
		//IntPtr AddRef();

		//[PreserveSig]
		//IntPtr Release();
	}

	static void TestOutAnyInterface()
	{
		ITest t; IntPtr p; object o; IUnknown u;

		//CreateTestInterface2(out t);

		//CreateTestInterface2(out p);
		//t=(ITest)Marshal.GetTypedObjectForIUnknown(p, typeof(ITest));

		//void* v = null;
		//t = (ITest)v;

		CreateTestInterface2(out o);
		//t = (ITest)o;
		t = o as ITest;

		//CreateTestInterface2(out u);
		//t = (ITest)u;

		t.Test1(4);

		Marshal.FinalReleaseComObject(t);
		t = null;
		Print("fin");
	}

	static void TestWndGetIcon()
	{
		//Wnd w = Wnd.Find("*Notepad");
		Wnd w = Wnd.Find("Calculator");
		if(w.Is0) { //on Win8 cannot find window, probably must be uiAccess. Find in QM and copy-paste.
			string s;
			if(!TaskDialog.ShowInput(out s, "hwnd")) return;
			w = (Wnd)(LPARAM)s.ToInt32_();
		}
		Print(w);
		IntPtr hi16 = Wnd.Misc.GetIconHandle(w);
		IntPtr hi32 = Wnd.Misc.GetIconHandle(w, true);
		PrintList(hi16, hi32);
		if(hi32 == Zero) return;
		var d = new TaskDialog("big icon");
		d.SetIcon(hi32);
		d.SetFooterText("small icon", hi16);
		d.ShowDialog();
		Api.DestroyIcon(hi16);
		Api.DestroyIcon(hi32);

		var a2 = new Action(() => { hi32 = Wnd.Misc.GetIconHandle(w, true); Api.DestroyIcon(hi32); });
		Perf.ExecuteMulti(5, 10, a2);
	}

	static void TestFileIcon()
	{
		//IntPtr hi = Files._IconCreateEmpty(16, 16);

		string s;

		//s = @"q:\app\qm.exe,-133";
		//int i = Files._IconGetIndex(ref s);
		//PrintList(i, s);
		//return;

		s = @"q:\app\paste.ico";
		s = @"q:\app\qm.exe";
		s = @"q:\app\qm.exe,1";
		s = @"q:\app\qm.exe,-133";
		s = @"q:\app\app.cpp";
		s = @".dll";
		s = "CatkeysTasks.exe";
		s = @"Properties\app.config";
		s = "http://ddd.com";
		s = @"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";
		//s = "Control Panel"; //no
		s = "::{20d04fe0-3aea-1069-a2d8-08002b30309d}";
		//s = "%TEMP%";
		//s = @"C:\Program Files\WindowsApps\Microsoft.WindowsCalculator_10.1605.1582.0_x64__8wekyb3d8bbwe\Calculator.exe";

		IntPtr hi = Icons.GetFileIconHandle(s, 32);
		//IntPtr hi = Files.GetIconHandle(Folders.VirtualITEMIDLIST.ControlPanelFolder, 32);
		Print(hi);
		if(hi == Zero) return;
		var d = new TaskDialog("text"); d.SetIcon(hi);
		Api.DestroyIcon(hi);
	}

	//static void TestCoord(int x, int y)
	//{
	//	Print("int");
	//}

	static void TestCoord(float x, float y)
	{
		PrintList("float", x > 0.0 && x < 1.1);
	}

	//static void TestCoord(double x, double y)
	//{
	//	Print("double");
	//}

	static void TestCoord()
	{
		//var w = Wnd.Find("*Notepad");
		//w.Activate();
		////w.Move(0.5f, 1f);
		////w.Resize(500, 200);
		//w.Resize(0.5f, 1f, true);

		//var w = Wnd.Find("Quick Macros *");
		//w.Activate();
		//w.Move(0.02f, 0.02f, 0.96f, 0.96f, true);
		////Print(w.ChildFromXY(0.97f, 100));
		////Print(w.ChildFromXY(0.3f, 1.05f, screenXY:true));
		//var c = w.ChildById(2212);
		//c.ZorderTop();
		//c.Move(0.1f, 0.1f, 0.8f, 0.8f);

		//var w = Wnd.Find(prop:new Wnd.WinProp() { x=100, y=0.5f});
		//Print(w);

		Print(Wnd.FromXY(0.5f, 0.99f, workArea: true));
	}

	//static IEnumerable<int> TestYield()
	//{
	//	for(int i=0; i<5; i++) {
	//		int k = ToInt("5");
	//		yield return i+k;
	//	}
	//}

	static unsafe void TestStringZeroTerm(string s)
	{
		fixed (char* p = s) {
			if((int)p[s.Length] != 0 || (int)p[s.Length + 1] != 0 || (int)p[s.Length + 2] != 0 || (int)p[s.Length + 3] != 0)
				PrintList((long)p, (int)p[s.Length], (int)p[s.Length + 1], (int)p[s.Length + 2], (int)p[s.Length + 3]);
		}
	}

	static void TestStringMisc()
	{
		//Print(int.Parse("+16"));

		//TestStringZeroTerm("aaaaaaaa");
		//TestStringZeroTerm("aabaaaaa");
		//TestStringZeroTerm("aacaaaaa");
		//TestStringZeroTerm("aadaaaaa");
		//TestStringZeroTerm($"aa{1}aaaaa");
		//TestStringZeroTerm($"aa{2}aaaaa");

		//for(int i=1; i<100000000; i++) {
		//	TestStringZeroTerm(new string('a', i&0xff));
		//      }
		//Print("fin");

		//foreach(int i in TestYield()) {
		//	Print(i);
		//}
		//Print("fin");

		//string s = "abc";
		//fixed(char* p= s)
		//{
		//	p[0] = 'A';
		//}
		//Print(s);

		//string s = "abc";
		//CharUpperBuff(s, 2);
		//Print(s);

		PrintList(int.MaxValue, uint.MaxValue, long.MaxValue, ulong.MaxValue);

		string s;
		s = "mm  18446744073709551615 kk";
		//s = "mm  9999999999999999999 kk";
		//s = "mm  0xFfffffffffffffff kk";

		s = "mm  4294967295 kk";
		//s = "mm  999999999 kk";
		//s = "mm  0xFfffffff kk";

		int iEnd;
		int R = s.ToInt32_(2, out iEnd);
		uint u = (uint)R;
		//long R = s.ToInt64_(2, out iEnd);
		//ulong u = (ulong)R;
		PrintList(s, R, u, iEnd);


		//int i1 = 0, i2 = 0, i3 = 0, i4=0;

		////i3 = ToInt(s);

		//Perf.SpinCPU(200);
		//Action a1 = new Action(() => { i1 = int.Parse(s); });
		//Action a2 = new Action(() => { i2 = s.ToInt_(); });
		//Action a3 = new Action(() => { i3 = ToInt(s); });
		//Action a4 = new Action(() => { i4 = ToInt2(s); });

		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);
		//PrintList(i1, i2, i3, i4);

	}

	static void TestWndMisc()
	{
		Wnd w;
		Wnd w2 = Wnd.Find("Quick*");
		//w = Wnd.Get.WndRoot;
		//Print(w);
		//w = Wnd.Get.Desktop;
		//Print(w);
		//w = Wnd.Get.DesktopListview;
		//Print(w);
		Print(w2.IsTopmost);

		Action a1 = new Action(() => { w = Wnd.Misc.WndRoot; });
		Action a2 = new Action(() => { w = Wnd.Misc.WndDesktop; });
		Action a3 = new Action(() => { w = Wnd.Misc.WndDesktopControl; });
		Action a4 = new Action(() => { bool fs = w2.IsFullScreen; });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		//w = Wnd.Find("Quick*");
		//w = Wnd.Find("My QM");
		//w = Wnd.Get.Desktop;
		//w = Wnd.Get.DesktopListview;
		//w = Wnd.Find("* Firefox");
		//w = Wnd.Get.WndRoot;
		////w = Wnd.Get.WndShell;
		//w = Wnd.Find("*- Google Chrome");
		//Print(w);
		////PrintList(w.IsOfShellThread, w.IsOfShellProcess);
		//Print(w.IsFullScreen);

		//while(true) {
		//	Wait(1);
		//	w = Wnd.FromMouse(false);
		//	if(w.ClassNameIs("WorkerW")) break;
		//	Print(w);
		//	PrintList(w.IsFullScreen, w.Rect);
		//}
	}

	#region TestWndRegisterClass
	static Wnd.Misc.WindowClass _wndRC, _wndRCSuper;
	static void TestWndRegisterClass()
	{
		_wndRC = Wnd.Misc.WindowClass.Register("Cat_Test", Cat_Test_WndProc, IntPtr.Size/*, Api.CS_GLOBALCLASS*/);
		Wnd w = Api.CreateWindowEx(0, _wndRC.Name, _wndRC.Name, Native.WS_OVERLAPPEDWINDOW | Native.WS_VISIBLE, 300, 100, 300, 200, Wnd0, 0, Zero, 0);
		if(w.Is0) return;

		_wndRCSuper = Wnd.Misc.WindowClass.Superclass("Edit", "Edit_Super", Cat_Test_WndProcSuper, IntPtr.Size);
		Wnd w2 = Api.CreateWindowEx(0, _wndRCSuper.Name, _wndRCSuper.Name, Native.WS_CHILD | Native.WS_VISIBLE, 0, 0, 200, 30, w, 3, Zero, 0);
		if(w2.Is0) return;

		//Print(Wnd.WindowClass.GetClassAtom("Cat_Test", Api.GetModuleHandle(null)));

		//Api.SetTimer(w, 1, 1000, null);

		Native.MSG m;
		while(Api.GetMessage(out m, Wnd0, 0, 0) > 0) {

			//if(m.message == Api.WM_TIMER && m.hwnd==w) {
			//	Print("timer");
			//	GC.Collect();
			//}

			Api.TranslateMessage(ref m);
			Api.DispatchMessage(ref m);
		}
	}
	static LPARAM Cat_Test_WndProc(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		switch(msg) {
		case Api.WM_DESTROY:
			Api.PostQuitMessage(0);
			break;
		case Api.WM_CREATE:
			_wndRC.SetMyLong(w, 1);
			break;
		case Api.WM_LBUTTONDOWN:
			Print(_wndRC.GetMyLong(w));
			break;
		}

		return Api.DefWindowProc(w, msg, wParam, lParam);
	}
	static LPARAM Cat_Test_WndProcSuper(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		switch(msg) {
		case Api.WM_DESTROY:
			return Api.DefWindowProc(w, msg, wParam, lParam);
		case Api.WM_CREATE:
			_wndRCSuper.SetMyLong(w, 2);
			break;
		case Api.WM_LBUTTONDOWN:
			Print(_wndRCSuper.GetMyLong(w));
			break;
		}

		return Api.CallWindowProc(_wndRCSuper.BaseClassWndProc, w, msg, wParam, lParam);
	}
	#endregion

	static void TestIsGhost()
	{
		Wnd w = Wnd.Find("Hung");
		Print(w);
		Print(w.IsHung);
		w = Wnd.Find("Hung*", "Ghost");
		Print(w);
		PrintList(w.IsHung, w.IsHungGhost);

		//bool y;
		//var a1 = new Action(() => { y = w.ClassNameIs("Ghost"); });
		//var a2 = new Action(() => { y = w.ProcessName.Equals_("DWM", true); });
		//var a3 = new Action(() => { y = w.IsHung; });
		//Perf.ExecuteMulti(5, 100, a1, a2, a3);

		//Perf.SpinCPU(100);
		//var a = Wnd.AllWindows(null, true);
		//foreach(Wnd t in a) {
		//	Print(t);
		//	Perf.First();
		//	y = t.IsHung;
		//	Perf.Next();
		//	y=t.ClassNameIs("Ghost");
		//	Perf.NW();
		//}
	}

	static void TestWndGetPropList()
	{
		foreach(Wnd w in Wnd.Misc.AllWindows(true)) {
			Print(w);
			foreach(var k in w.PropList()) {
				Print(k);
			}
			Print("---------");
		}
	}

	static void TestWndMapPoints()
	{
		Wnd w = Wnd.Find("", "QM_Editor");
		Print(w);
		RECT r = new RECT(1, 2, 3, 4, false);
		RECT rr;
		rr = r; w.MapClientToScreen(ref rr);
		Print(rr);
		rr = r; w.MapScreenToClient(ref rr);
		Print(rr);

		Wnd c = w.Child("", "QM_Code");
		Print(c);
		//rr = c.Rect;
		rr = r; c.MapClientToClientOf(w, ref rr);
		Print(rr);

		c.GetRectInClientOf(w, out rr);
		Print(rr);

		Print(Wnd.FromXY(1460, 1400));

		//rr = r; c.MapClientToClientOf(w, ref rr);
		//Print(rr);
		//rr = r; c.MapClientToClientOf(w, ref rr);
		//Print(rr);
		POINT p = new POINT(1, 2), pp;
		pp = p; w.MapClientToWindow(ref pp);
		Print(pp);
		pp = p; w.MapWindowToClient(ref pp);
		Print(pp);

		rr = r; w.MapClientToWindow(ref rr);
		Print(rr);
		rr = r; w.MapWindowToClient(ref rr);
		Print(rr);

		Print(Wnd.FromMouse().MouseClientXY);
	}

	static void TestWndIsAbove()
	{
		Wnd w1 = Wnd.Find("", "QM_Editor");
		Wnd w2 = Wnd.Find("", "CabinetWClass");
		PrintList(w1.ZorderIsBefore(w2), w2.ZorderIsBefore(w1));
	}

	static void TestWndSetParent()
	{
		//note: SetParent removed, it did not work well, anyway probably will need to change parent only of form controls, then Form.Parent should be used and works well, even with topmost windows.

		//Wnd w1 = Wnd.Find("", "QM_Editor");
		//Wnd w2 = w1.Child("Running items");
		//Wnd w3 = w2.WndDirectParentOrOwner;
		//Print(w3);

		//PrintList("SetParent", w2.SetParent(Wnd0));
		//TaskDialog.Show("");
		//PrintList("SetParent", w2.SetParent(w3));

		var f = new Form();
		var b = new Button();
		f.Controls.Add(b);

		f.Click += (o, e) =>
		{
			Form f2 = null;
			f2 = new Form();
			f2.Show();
			f2.Left = 500;
			((Wnd)f).ZorderTopmost();
			//Print(b.Handle);

			b.Parent = f2;
			//Print(b.Handle);
			//Wnd w = (Wnd)b;
			//w.SetParent(Wnd0, true);
			TaskDialog.Show("");
			b.Parent = f;
			//w.SetParent((Wnd)f, true);

			f2.Close();
		};

		f.ShowDialog();
	}

	static void TestWndBorder()
	{
		Wnd w = Wnd.Find("", "QM_Editor");
		//w = Wnd.Find("", "QM_Toolbar");
		Print(w);
		Print(Wnd.Misc.BorderWidth(w));
		Print(Wnd.Misc.BorderWidth(w.Style, w.ExStyle));

		RECT r = w.ClientRect;
		Print(r);
		Wnd.Misc.WindowRectFromClientRect(ref r, w.Style, w.ExStyle, false);
		Print(r);
	}

	static void TestWndStoreApp()
	{
		foreach(Wnd w in Wnd.Misc.AllWindows()) {
			bool isWin8Metro = w.IsWin8MetroStyle;
			int isWin10StoreApp = w.IsWin10StoreApp;
			string prefix = null, suffix = null;
			if(isWin8Metro || isWin10StoreApp != 0) { prefix = isWin8Metro ? "<><c 0xff>" : "<><c 0xff0000>"; suffix = "</c>"; }
			Print($"{prefix}metro={isWin8Metro}, win10={isWin10StoreApp}, cloaked={w.IsCloaked},    {w.ProcessName}  {w}  {w.Rect}{suffix}");
		}
	}

	static void TestWndControlCast()
	{
		var f = new Form();

		f.Click += (o, e) =>
		{
			if(true) {
				var b = new Button();
				f.Controls.Add(b);
				Wnd t = (Wnd)b;
				var c = (Control)t;
				Print(c == null); //False
			} else {
				Wnd t = Api.CreateWindowEx(0, "Edit", "Edit", Native.WS_CHILD | Native.WS_VISIBLE, 0, 0, 200, 30, (Wnd)f, 3, Zero, 0);
				if(t.Is0) return;
				var c = (Control)t;
				Print(c == null); //True
								  //Print(c.Bounds);
				f.Controls.Add(c);
			}
		};

		f.ShowDialog();

	}

	static void TestWndRect()
	{
		//var w=Wnd.Find("*Notepad");
		var w = Wnd.Find("Options");
		w = w.Child("OK");
		Print(w);
		RECT r;

		r = w.Rect;
		Print(r);
		//w.Rect = new RECT(100, 1300, 300, 500, true);
		//w.Rect = new RECT(100, 320, 80, 50, true);

		r = w.ClientRect;
		Print(r);
		//r.Inflate(2, 2); w.ClientRect=r;

		//r =w.ClientRectInScreen;
		//Print(r);
		//w.ClientRectInScreen = new RECT(100, 320, 80, 50, true);

		//r.Inflate(2, 2);
		//w.ClientRectInScreen = r;

		r = w.RectInParent;
		Print(r);
	}

	static void TestWndMoveMisc()
	{
		var w = Wnd.Find("*Notepad");
		//w.Move(200, 1500, 400, 200);

		//w.MoveToScreenCenter(2);
		w.MoveInScreen(-100, -20, 2);
		w.ActivateLL();
	}

	static void _TestWndZorder(Wnd w, Wnd w2)
	{
		//Print(w.ZorderTop());
		//Print(w.ZorderBottom());

		//Print(w.ZorderTopmost());
		////TaskDialog.Show("");
		//Wait(1);
		//Print(w.ZorderNotopmost(true));
		////Print(w.ZorderBottom());

		Print(w.ZorderBelow(w2));
		w2.ActivateLL(); Thread.Sleep(500);
		Print(w.ZorderAbove(w2));

	}

	static void TestWndZorder()
	{
		var w = Wnd.Find("*Notepad");
		//var w = Wnd.Find("*WordPad");
		//var w = Wnd.Find("Options");
		//var w = Wnd.Find("Font");
		//var w = Wnd.Find("", "QM_Editor");
		var w2 = Wnd.Find("* - Paint");
		//PrintList(w, w2);

		_TestWndZorder(w, w2);

		//var f = new Form();
		//f.Click += (o, e) =>
		//{
		//	_TestWndZorder((Wnd)f.Handle);
		//      };
		//f.ShowDialog();
	}

	static void TestWndIsUacDenied()
	{
		//var w = Wnd.Find("*Notepad");
		var w = Wnd.Find("", "QM_Editor");
		//var w = Wnd.Find("Calculator");
		//w = w.Child("", "Windows.UI.Core.*");
		if(w.Is0) {
			int i;
			if(!TaskDialog.ShowInput(out i, "Handle")) return;
			w = (Wnd)(LPARAM)i;
		}

		Print(w);
		Print(w.IsUacAccessDenied);
		//PrintList(w.IsUacAccessDenied, w.IsUacAccessDenied2);
		//Print(w.SetProp("abc", 1)); //fails
		//Print(w.RemoveProp("abcde"));
		//Print(w.SetWindowLong());
		//Print(w.)


		//var a1 = new Action(() => { bool b = w.IsUacAccessDenied; });
		//var a2 = new Action(() => { bool b = w.IsUacAccessDenied2; });
		//Perf.ExecuteMulti(5, 1000, a1, a2);
	}

	static void TestTaskDialogActivationEtc()
	{
		Wait(3);
		//Script.Option.TopmostIfNoOwnerWindow = true;
		//Script.Option.dialogAlwaysActivate = true;
		//Print(Wnd.AllowActivate());
		//Print(Wnd.Find("*Notepad").ActivateLL());
		//Wait(1);
		//Print(Wnd.ActiveWindow);
		TaskDialog.Show("test");
		//Wait(3);
	}

	static void TestWndFindFast()
	{
		Wnd w = Wnd0;
		for(;;) {
			w = Wnd.FindFast(null, "QM_Toolbar", w);
			if(w.Is0) break;
			Print(w);
		}
		Print(Wnd.FindFast(null, "QM_Editor").ChildFast(null, "QM_Scc"));
	}

	static void TestWndGetGUIThreadInfo()
	{
		Wait(3);
		Print(Wnd.WndFocused);
		RECT r;
		Print(Input.GetTextCursorRect(out r));
		Print(r);
	}

	static class TestStaticInitClass
	{
		static int _x = _InitX();
		static int _InitX() { PrintFunc(); return 1; }
		public static int GetX()
		{
			PrintFunc();
			return _x;
		}
		public static int Other()
		{
			PrintFunc();
			return -1;
		}
	}

	static void TestStaticInit1()
	{
		PrintFunc();
		Print(TestStaticInitClass.GetX());
	}

	static void TestStaticInit2()
	{
		PrintFunc();
		Print(TestStaticInitClass.Other());
	}

	static string BytesToHexString_BitConverter(byte[] a)
	{
		return BitConverter.ToString(a).Replace("-", "");
	}

	static void TestHash()
	{
		Perf.SpinCPU(100);

		//var b = new byte[] { 0, 1, 0xA, 0xF, 0xBE, 0x59 };
		//string s;
		//s = Calc.BytesToHexString(b);
		//Print(s);
		////s = Calc.BytesToHexString2(b);
		////Print(s);
		//s = BytesToHexString_BitConverter(b);
		//Print(s);

		//var a1 = new Action(() => { s = BytesToHexString_BitConverter(b); });
		//var a2 = new Action(() => { s = Calc.BytesToHexString(b); });
		////var a3 = new Action(() => { s = Calc.BytesToHexString2(b); });
		//Perf.ExecuteMulti(5, 1000, a1, a2);


		//var b = new byte[] { 0, 1, 0xA, 0xF, 0xBE, 0x59, 0, 0xff, 0x58, 0xD7, 0, 1, 0xA, 0xF, 0xBE, 0x59, 0, 0xff, 0x58, 0xD7 };
		//var s = Calc.BytesToHexString(b);
		//Print(s);
		//Print(Calc.BytesToHexString(Calc.BytesFromHexString(s)));
		//string sB64 = Convert.ToBase64String(b);
		//Print(sB64);
		//Print(Convert.ToBase64String(Convert.FromBase64String(sB64)));

		//var a1 = new Action(() => { Calc.BytesToHexString(b); });
		//var a2 = new Action(() => { Calc.BytesFromHexString(s); });
		//var a3 = new Action(() => { Convert.ToBase64String(b); });
		//var a4 = new Action(() => { Convert.FromBase64String(sB64); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);


		string s = @"Q:\app\catkeys\tasks\CatkeysTasks.exe";
		string hash;
		Perf.First();
		hash = Calc.HashMD5Hex(s);
		Perf.NW(); //1700 (.NET 3700)
		Print(hash);
		hash = Calc.HashHex(s, "MD5");
		Print(hash);
		hash = Calc.HashHex(s, "SHA256");
		Print(hash);
		int hashInt = Calc.HashFnv1(s);
		Print(hashInt);
		unsafe { fixed (char* p = s) { hashInt = Calc.HashFnv1(p, s.Length); } }
		Print(hashInt);
		unsafe { fixed (char* p = s) { hashInt = Calc.HashFnv1((byte*)p, s.Length * 2); } }
		Print(hashInt);

		byte[] a = Encoding.UTF8.GetBytes(s);

		var a1 = new Action(() => { hash = Calc.HashMD5Hex(s); }); //440 (QM2 350, .NET 2000 with static var, 4000 with local var)
		var a2 = new Action(() => { hash = Calc.HashHex(s, "MD5"); }); //4200
		var a3 = new Action(() => { hash = Calc.HashHex(s, "SHA256"); }); //2500
		var a4 = new Action(() => { hash = Calc.HashMD5Hex(a); }); //320
		var a5 = new Action(() => { hashInt = Calc.HashFnv1(s); }); //40
		var a6 = new Action(() => { unsafe { fixed (char* p = s) { hashInt = Calc.HashFnv1((byte*)p, s.Length * 2); } } }); //84 (40 if no *2)
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6);
	}

	static void TestNewPerf()
	{
		Perf.SpinCPU(100);

		//Perf.First();
		//Thread.Sleep(1);
		//Perf.Next();
		//Thread.Sleep(10);
		//Perf.NW();

		var t = new Perf.Inst();
		var t2 = new Perf.Inst();
		for(int j = 0; j < 5; j++) {
			t.First();
			for(int i = 0; i < 1000; i++) { t2.First(); }
			t.Next();
			for(int i = 0; i < 1000; i++) { t2.First(); t2.Next(); }
			t.Next();
			for(int i = 0; i < 1000; i++) { Perf.First(); }
			t.Next();
			for(int i = 0; i < 1000; i++) { Perf.First(); Perf.Next(); }
			t.Next();
			t.Write();
		}
	}

	#region test timer

	static void TestTimer()
	{
		//Time.SetTimer(1000, (t, p) => { Print(1); });
		Perf.First();
		//Time.SetTimer(1000, t => { Print(t.Tag); Application.ExitThread(); }, false, "test 1");
		//Time.SetTimer(1000, t => { Print(t.Tag); }, true, "test 1");
		//Perf.Next();
		//Time.SetTimer(1100, t => { Print(t.Tag); Application.ExitThread(); }, true, "test 2");

		//Api.SetTimer(Wnd0, 1, 1000, (w, m, i, t)=>{ Api.KillTimer(w, i); Print(1); Application.ExitThread(); });
		//Perf.Next();
		//Api.SetTimer(Wnd0, 1, 900, (w, m, i, t)=>{ Api.KillTimer(w, i); Print(2); });

		//var t=new System.Threading.Timer()

		//var comp = new Container();
		//Perf.Next();
		//var t = new System.Windows.Forms.Timer(comp);
		//t.Tick += (s, d) => { Print(1); Application.ExitThread(); };
		//t.Interval = 1000; t.Start();
		//Perf.Next();
		//var tt = new System.Windows.Forms.Timer(comp);
		//tt.Tick += (s, d) => { Print(2); };
		//tt.Interval = 900; tt.Start();

		//var t1=Time.SetTimer(1000, true, t => { Print(t.Tag); }, "test 1");
		//Perf.Next();
		//var t2=Time.SetTimer(1100, true, t => { Print(t.Tag); Application.ExitThread(); }, "test 2");

		Time.SetTimer(1100, false, t => { Print(t.Tag); Application.ExitThread(); }, "test 2");
		//GC.Collect();

		//t1.Stop();
		//t2.Tag = "new tag";
		//t2.Start(5000, true);

		Perf.NW();
		//PrintList(t1, t2);
		//Application.Run();
		TaskDialog.Show("message loop");

	}

	static void _StartFormsTimer()
	{
		var t = new System.Windows.Forms.Timer();
		t.Interval = 1000;
		t.Tick += (a, b) => Print("tick");
		t.Start();
	}

	static void TestTimerThread()
	{
		//Time.Timer_ u = null;

		var T = new Thread(() =>
		{
			Time.SetTimer(1000, true, t => { Print(t.Tag); }, "test 2");
			//Time.SetTimer(1000, true, t => { Print(t.Tag); t.Start(1000, true); }, "test 2");
			//u=Time.SetTimer(1000, false, t => { Print(t.Tag); }, "test 2");
			//_StartFormsTimer();

			//u.Start(3000, false);
			//u.Stop();

			GC.Collect();
			TaskDialog.Show("timer thread");
			GC.Collect();
			//TaskDialog.Show("timer thread");
		});
		T.Start();
		T.Join(1500);
		//u.Stop(); //test assert
		T.Join();
		Print("thread ended");
		Thread.Sleep(1000);
		//u = null;
		GC.Collect();
		TaskDialog.Show("main thread");

	}

	#endregion

	#region test async

	static Task<int> SomeOperationAsync2()
	{
		return Task.Run(() => { Thread.Sleep(800); return 5; });
	}

	static async Task<int> SomeOperationAsync()
	{
		return await SomeOperationAsync2();
	}

	static async void TestAsyncAsync()
	{
		int timeout = 1000;
		var task = SomeOperationAsync2();
#if true
		await task;
		Print(task.Result);
#else //test with timeout
		if(await Task.WhenAny(task, Task.Delay(timeout)) == task) {
			Print(task.Result);
		} else {
			Print("timeout");
		}
#endif
	}

	static void TestAsync()
	{
		Perf.First();
		TestAsyncAsync();
		Perf.NW();
		Print("END");
		TaskDialog.Show("waiting");
	}

	#endregion

	#region test native thread

	public delegate uint PTHREAD_START_ROUTINE(IntPtr lpThreadParameter);
	[DllImport("kernel32.dll")]
	public static extern IntPtr CreateThread(IntPtr lpThreadAttributes, LPARAM dwStackSize, PTHREAD_START_ROUTINE lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);

	[Flags]
	public enum COINIT :uint
	{
		COINIT_APARTMENTTHREADED = 0x2,
		COINIT_MULTITHREADED = 0x0,
		COINIT_DISABLE_OLE1DDE = 0x4,
		COINIT_SPEED_OVER_MEMORY = 0x8
	}

	[DllImport("ole32.dll", PreserveSig = true)]
	public static extern int CoInitializeEx(IntPtr pvReserved, COINIT dwCoInit);
	[DllImport("ole32.dll")]
	public static extern void CoUninitialize();

	static Thread _iconThread;

	static uint _ThreadProc(IntPtr param)
	{
		CoInitializeEx(Zero, COINIT.COINIT_APARTMENTTHREADED | COINIT.COINIT_DISABLE_OLE1DDE); //if before Thread.CurrentThread etc, then Thread.GetApartmentState will get STA. Without this .NET would make MTA.
		_iconThread = Thread.CurrentThread; //tested: it seems this auto-creates and correctly initializes .NET Thread object etc. Sets IsBackground true.

		//Print(_iconThread.IsBackground);
		//_iconThread.IsBackground = false;
		//Print(_iconThread.GetApartmentState());
		//Print(_iconThread.ThreadState);

		TaskDialog.Show("thread");
		//Wait(30);

		return 0;
	}

	static PTHREAD_START_ROUTINE _threadProc = _ThreadProc;

	static void TestNativeThread()
	{
		//This works, but probably not useful.
		//It would be useful only if we want to share the thread between appdomains.
		//But it is probably impossible because .NET ends threads when the appdomain exits. Also deletes the delegate.
		//To share a thread, create it in default appdomain.

		uint tid;
		var ht = CreateThread(Zero, 0, _threadProc, Zero, 0, out tid);
		PrintList(ht, tid);
		if(ht == Zero) return;
		Api.CloseHandle(ht);
		TaskDialog.ShowEx("main", y: -300);
	}

	#endregion

	static void TestExpandPath()
	{
		string s1 = @"Q:\app\Catkeys\Tasks\System.Collections.Immutable.dll";
		string s2 = @"%ProgramFiles%\Quick Macros 2\qm.exe";

		string r1 = null, r2 = null, r3 = null, r4 = null;

		//Print(ExpandPath(s2)); return;

		var a1 = new Action(() => { r1 = Environment.ExpandEnvironmentVariables(s1); });
		var a2 = new Action(() => { r2 = Environment.ExpandEnvironmentVariables(s2); });
		var a3 = new Action(() => { r3 = Path_.ExpandEnvVar(s1); });
		var a4 = new Action(() => { r4 = Path_.ExpandEnvVar(s2); });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		Print(r1);
		Print(r2);
		Print(r3);
		Print(r4);
	}

	static void TestAssoc()
	{
		//var s = Files.Misc.GetFileTypeOrProtocolRegistryKey(".txt");
		//var s = Files.Misc.GetFileTypeOrProtocolRegistryKey(".cs");
		//var s = Files.Misc.GetFileTypeOrProtocolRegistryKey("http:");
		//var s = Files.Misc.GetFileTypeOrProtocolRegistryKey("http://hdhdhdh");
		//var s = Files.Misc.GetFileTypeOrProtocolRegistryKey("shell:hdhdhdh");
		//var s = Files.Misc.GetFileTypeOrProtocolRegistryKey("c:\\file.bmp");
		//var s = Files.Misc.GetFileTypeOrProtocolRegistryKey("invalid");
		//if(s != null) Print(s); else Print("null");
	}

	static void TestSearchPath()
	{
		string s = null;
		//s = "pythonwin.exe";
		//s = "blend.exe";
		//s = "cmmgr32.exe";
		//s = "excel.exe";
		//s = "mip.exe";
		//s = "tests.exe";
		//s = "notepad.exe";
		//s = "calc.exe";
		//s = "typescript.js"; //in PATH
		//Environment.CurrentDirectory = @"q:\app";
		//s = "qmdd.exe";
		//s = "csc.exe";
		//s = @"C:\windows";
		//s = @"C:\windows\system32\notepad.exe";
		//s = @"%SystemRoot%\system32\notepad.exe";
		//s = @"%SystemRoot%";
		s = @"\\q7c\q\downloads";
		s = @"\\q7c\q";
		s = @"q:\";
		s = @"q:";
		s = ":: {jfjfjfjf}";
		s = @"http://www.quickmacros.com";
		s = "notepad"; //not found, its OK

		var r = Files.SearchPath(s);
		//var r =Files.SearchPath(s, @"q:\app");
		if(r != null) Print(r); else Print("not found");
	}

	static void TestSynchronizationContext1()
	{
		//WindowsFormsSynchronizationContext.AutoInstall = false;
		//Print(WindowsFormsSynchronizationContext.AutoInstall);

		Print(SynchronizationContext.Current);
		//Time.SetTimer(100, true, t =>
		//{
		//	Print(SynchronizationContext.Current);
		//	Application.ExitThread();
		//});
		//Application.Run();
		//var f = new Form();
		//Print(SynchronizationContext.Current);
		//SynchronizationContext.Current.Post(state => { PrintList("posted", SynchronizationContext.Current); }, null);
		Time.SetTimer(1, true, t => { PrintList("posted", SynchronizationContext.Current); });
		Thread.Sleep(100);
		//WindowsFormsSynchronizationContext.AutoInstall = false;
		Print(1);
		Application.DoEvents();
		Print(2);
		Print(SynchronizationContext.Current);
	}

	static void TestSynchronizationContext2()
	{
		PrintList("main 1", SynchronizationContext.Current);
		var f = new Form();
		PrintList("main 2", SynchronizationContext.Current);
		WindowsFormsSynchronizationContext.AutoInstall = false;

		var thread = new Thread(() =>
		{
			PrintList("thread 1", SynchronizationContext.Current);
			var ff = new Form();
			PrintList("thread 2", SynchronizationContext.Current);
		});
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		thread.Join();
	}

	//static void TestSynchronizationContext3()
	//{
	//	//Application.DoEvents();
	//	//var f = new Form();
	//	PrintList(SynchronizationContext.Current, WindowsFormsSynchronizationContext.AutoInstall);
	//	using(new Util.LibEnsureWindowsFormsSynchronizationContext()) {
	//		PrintList(SynchronizationContext.Current, WindowsFormsSynchronizationContext.AutoInstall);
	//		Application.DoEvents();
	//		PrintList(SynchronizationContext.Current, WindowsFormsSynchronizationContext.AutoInstall);
	//	}
	//	PrintList(SynchronizationContext.Current, WindowsFormsSynchronizationContext.AutoInstall);
	//}

	static void TestInterDomain()
	{
		//var d=Util.AppDomain_.GetDefaultDomain();
		////var s = d.GetData("testData") as string;
		////if(s == null) d.SetData("testData", $"dddata {8}");
		//var s = d.GetData("testData") as List<int>;
		//if(s == null) d.SetData("testData", new List<int> { 1, 2, 3 });
		//Print(s);

		//string s = InterDomain.GetVariable("str") as string;
		//if(s==null) InterDomain.SetVariable("str", "VALUE");
		//Print(s);

		//int? i = InterDomain.GetVariable("str") as int?;
		//if(i==null) InterDomain.SetVariable("str", 55);
		//Print(i);
		//i = 8;

		//var a = InterDomain.GetVariable("list") as List<int>;
		//if(a == null) InterDomain.SetVariable("list", new List<int> { 1, 2, 3 });
		//Print(a);
		//a.Add(100);

		//var a = InterDomain.GetVariable("list") as string[];
		//if(a == null) InterDomain.SetVariable("list", new string[] { "one","two","three" });
		//Print(a);

		//var a = InterDomain.GetVariable("list") as Dictionary<int, string>;
		//if(a == null) {
		//	var b = new Dictionary<int, string>();
		//	b.Add(5, "gg"); b.Add(6, "hh");
		//	InterDomain.SetVariable("list", b);
		//}
		//Print(a);

		//InterDomain.SetVariable("{D6349CB1-0E29-4AFC-B172-2A1D3CCB8A32}", "vvvvv");
		//Print(InterDomain.GetVariable("{D6349CB1-0E29-4AFC-B172-2A1D3CCB8A32}"));

		//var a1 = new Action(() => { Environment.SetEnvironmentVariable("EnvironmentVariable", "valuevaluevalue"); string s = Environment.GetEnvironmentVariable("EnvironmentVariable"); });
		//var a2 = new Action(() => { InterDomain.SetVariable("InterDomainVariable", "valuevaluevalue"); string s = InterDomain.GetVariable("InterDomainVariable") as string; });
		//Perf.ExecuteMulti(5, 1000, a1, a2);

		//var k = new InterDomainData(10, "test");
		//PrintList(k._i, k._s);
		//InterDomain.SetVariable("k", k);
		//var b = InterDomain.GetVariable("k") as InterDomainData;
		//PrintList(b._i, b._s);

		//var b = InterDomain.GetVariable("k") as InterDomainData;
		//if(b == null) {
		//	Print("null");
		//	var k = new InterDomainData(10, "test");
		//	InterDomain.SetVariable("k", k);
		//	k._i = 11;
		//} else PrintList(b._i, b._s);

		//var k = new InterDomainData(10, "test");
		//InterDomain.SetVariable("k", k);

		//var o = InterDomain.Get2("test");
		//if(o == null) { Print("null"); InterDomain.Set2("test", "kkk"); }
		//Print(o);

		//string x;
		//if(InterDomain.Get3("nm", out x)) Print(x);
		//else { Print("no"); InterDomain.Set3("nm", "DATA"); }
		////else { Print("no"); InterDomain.Set3("nm", (string)null); }

		//int x;
		//if(InterDomain.GetVariable("nm", out x)) Print(x);
		//else { Print("no"); InterDomain.SetVariable("nm", 8); }
		//else { Print("no"); InterDomain.SetVariable("nm", "DATA"); }
		//else { Print("no"); InterDomain.SetVariable("nm", null); }

		//var a1 = new Action(() => { Environment.SetEnvironmentVariable("EnvironmentVariable", "valuevaluevalue"); string s = Environment.GetEnvironmentVariable("EnvironmentVariable"); });
		//var a2 = new Action(() => { InterDomain.SetVariable("InterDomainVariable", "valuevaluevalue"); string s = InterDomain.GetVariable("InterDomainVariable") as string; });
		//var a3 = new Action(() => { InterDomain.Set2("InterDomainVariable2", "valuevaluevalue"); string s = InterDomain.Get2("InterDomainVariable2") as string; });
		//var k = new Dictionary<string, object>();
		//var a4 = new Action(() => { k["InterDomainVariable"]="valuevaluevalue"; string s = k["InterDomainVariable"] as string; });
		//var a5 = new Action(() => { InterDomain.Set3("InterDomain_int1", 5); int s; InterDomain.Get3("InterDomain_int1", out s); });
		//var a6 = new Action(() => { InterDomain.Set2("InterDomain_int2", 5); int s = (int)InterDomain.Get2("InterDomain_int2"); });
		//var a7 = new Action(() => { InterDomain.SetVariable("InterDomain_int3", 5); int s = (int)InterDomain.GetVariable("InterDomain_int3"); });
		////var a8 = new Action(() => { InterDomain.Set2("InterDomain_int4", 5); object s; InterDomain.TryGet("InterDomain_int4", out s); });
		////var a9 = new Action(() => { InterDomain.Set2("InterDomain_int5", 5); int s; InterDomain.Get4("InterDomain_int5", out s); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6, a7/*, a8, a9*/);


		//POINT x;
		//if(InterDomain.GetVariable("nm", out x)) Print(x);
		//else { Print("no"); InterDomain.SetVariable("nm", new POINT(5,6)); }

		//IntPtr x;
		//if(InterDomain.GetVariable("nm", out x)) Print(x);
		//else { Print("no"); InterDomain.SetVariable("nm", new IntPtr(6)); }

		//LPARAM x;
		//if(InterDomain.GetVariable("nm", out x)) Print(x);
		//else { Print("no"); InterDomain.SetVariable("nm", (LPARAM)9); }

		//Wnd x;
		//if(InterDomain.GetVariable("nm", out x)) Print(x);
		//else { Print("no"); InterDomain.SetVariable("nm", Wnd.FindFast("QM_Editor")); }

		//InterDomainData x;
		//if(InterDomain.GetVariable("nm", out x)) Print(x._s);
		//else { Print("no"); InterDomain.SetVariable("nm", new InterDomainData(5, "five")); }

		//TODO: move TDx to TaskDialog. Or not.

		//PrintList("before", AppDomain.CurrentDomain.IsDefaultAppDomain());
		//var x = InterDomain.DefaultDomainVariable("goo", () => { PrintList("delegate", AppDomain.CurrentDomain.IsDefaultAppDomain()); return new InterDomainData(5, "fff"); });
		InterDomainData x, y, z;
		Perf.First();
		InterDomain.DefaultDomainVariable("goox", out x);
		Perf.Next();
		InterDomain.DefaultDomainVariable("gooy", out y);
		Perf.Next();
		InterDomain.DefaultDomainVariable("gooz", out z);
		Perf.NW();
		x.Method();
		Print(x._s);
		//TaskDialog.Show("");
	}

	//[Serializable]
	class InterDomainData :MarshalByRefObject
	{
		public int _i;
		public string _s;

		public InterDomainData() { _i = 3; _s = "def"; }
		public InterDomainData(int i, string s) { _i = i; _s = s; PrintList("ctor", AppDomain.CurrentDomain.IsDefaultAppDomain()); }

		public void Method()
		{
			//PrintList("method", AppDomain.CurrentDomain.IsDefaultAppDomain());
		}
	}

	public static void TestPidlToString()
	{
		var a = Directory.GetFiles(@"q:\app");
		//var a = new string[] { "http://www.quickmacros.com" };

		var ai = new IntPtr[a.Length];
		var af = new string[a.Length];

		Perf.First();
		for(int i = 0; i < a.Length; i++) {
			ai[i] = Files.Misc.PidlFromString(a[i]);
			if(ai[i] == Zero) PrintList("PidlFromString", a[i]);
		}
		Perf.Next();
		for(int i = 0; i < a.Length; i++) {
			af[i] = Files.Misc.PidlToString(ai[i], Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING);
			if(af[i] == null) PrintList("PidlToString", a[i]);
		}
		Perf.NW();
		for(int i = 0; i < a.Length; i++) Marshal.FreeCoTaskMem(ai[i]);
		Print(af);
	}

	static void TestGetIconsOfAllFileTypes()
	{
		var d = new Dictionary<string, string>();
		string s;
		using(var k1 = Registry_.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts")) {
			string[] sub1 = k1.GetSubKeyNames();
			//Print(sub1);
			foreach(var s1 in sub1) {
				if(!Registry_.GetString(out s, "ProgId", s1 + @"\UserChoice", k1)) { /*PrintList(s1, "-");*/ continue; }
				//PrintList(s1, s);
				d.Add(s1, s);
			}
		}

		{
			string[] sub1 = Registry.ClassesRoot.GetSubKeyNames();
			//Print(sub1);
			foreach(var s1 in sub1) {
				if(s1[0] != '.') continue;
				if(d.ContainsKey(s1)) continue;
				if(!Registry_.GetString(out s, "", s1, Registry.ClassesRoot)) { /*PrintList(s1, "-");*/ continue; }
				//PrintList(s1, s);
				d.Add(s1, s);
			}
		}

		//Print(d);

		foreach(var v in d) {
			Print($"<><Z 0x80E080>{v.Key}</Z>");
			//var hi = Icons.GetIconHandle(v.Value + ":", 16, 0);
			var hi = Icons.GetFileIconHandle(v.Key, 16, 0);

			if(hi == Zero) {
				PrintList("<><c 0xff>", v.Key, v.Value, "</c>");
				continue;
			}

			Api.DestroyIcon(hi);
		}
	}

	static void TestPerfIncremental()
	{
		//var perf = new Perf.Inst();
		//perf.First();
		//Thread.Sleep(1);
		//perf.Next();
		//Thread.Sleep(5);
		//perf.Next();
		//perf.Write();

		var perf = new Perf.Inst();
		perf.Incremental = true;
		for(int i = 0; i < 5; i++) {
			perf.First();
			perf.Next();
			perf.Next();
			Api.GetFileAttributes("c:\nofile.tct");
			perf.Next();
			perf.Next();
			//Api.PathIsURL("jdjdjdj:jdjdj");
			//Api.CloseHandle(Zero);
			//Api.GetFileAttributes("c:\nofile.tct");
			perf.Next();
			perf.Next();
			Api.GetFileAttributes("c:\nofile.tct");
			perf.Next();
			perf.Next();
			//perf.Write();

			Thread.Sleep(50);
		}
		perf.Write();
		perf.Incremental = false;

		//var perf = new Perf.Inst();
		//perf.Incremental = true;
		//for(int i = 0; i < 5; i++) {
		//	Thread.Sleep(100); //not included in the measurement
		//	perf.First();
		//	Thread.Sleep(30); //will make sum ~150000
		//	perf.Next();
		//	Thread.Sleep(10); //will make sum ~50000
		//	perf.Next();
		//	Thread.Sleep(100); //not included in the measurement
		//}
		//perf.Write(); //speed:  154317  51060  (205377)
		//perf.Incremental = false;

		//Perf.Incremental = true;
		//for(int i = 0; i < 5; i++) {
		//	Thread.Sleep(100); //not included in the measurement
		//	Perf.First();
		//	Thread.Sleep(30); //will make sum ~150000
		//	Perf.Next();
		//	Thread.Sleep(10); //will make sum ~50000
		//	Perf.Next();
		//	Thread.Sleep(100); //not included in the measurement
		//}
		//Perf.Write(); //speed:  154317  51060  (205377)
		//Perf.Incremental = false;
	}

	#region test thread pool

	static int _nTasks = 30;

	//static void TestTasksDefault()
	//{
	//	Perf.Next();
	//	using(new Util.LibEnsureWindowsFormsSynchronizationContext()) {
	//		_n = _nTasks;
	//		for(int i = 0; i < _nTasks; i++) _TestTasksDefault();
	//	}
	//	Time.SetTimer(500, true, t => _loop.Stop());
	//	_loop.Loop();
	//}

	//static async void _TestTasksDefault()
	//{
	//	var task = Task.Run(() =>
	//	{
	//		Thread.Sleep(_random.Next(1, 10));
	//		return 0;
	//	});
	//	await task;
	//	int hi = task.Result;
	//	if(--_n < 1) Perf.NW();
	//}

	//static void TestTasksSta()
	//{
	//	_staTaskScheduler = new System.Threading.Tasks.Schedulers.StaTaskScheduler(4);
	//	Perf.Next();
	//	using(new Util.LibEnsureWindowsFormsSynchronizationContext()) {
	//		_n = _nTasks;
	//		for(int i = 0; i < _nTasks; i++) _TestTasksSta();
	//	}
	//	Time.SetTimer(500, true, t => _loop.Stop());
	//	_loop.Loop();
	//}

	//static System.Threading.Tasks.Schedulers.StaTaskScheduler _staTaskScheduler;

	//static async void _TestTasksSta()
	//{
	//	var task = Task.Factory.StartNew(() =>
	//	{
	//		Thread.Sleep(_random.Next(1, 10));
	//		return 0;
	//	}, CancellationToken.None, TaskCreationOptions.None, _staTaskScheduler);
	//	await task;
	//	int hi = task.Result;
	//	if(--_n < 1) Perf.NW();
	//}

	static void TestThreadPoolSTA()
	{
		Print("BEGIN");
		//Print(Api.GetCurrentThreadId());
		Task.Run(() => { for(;;) { Thread.Sleep(100); GC.Collect(); } });
		Perf.Next();
		//using(new Util.LibEnsureWindowsFormsSynchronizationContext()) {
		_nTasks = 30;
		_n = _nTasks;
		for(int i = 0; i < _nTasks; i++) _TestThreadPoolSTA2();
		//}
		Time.SetTimer(1000, true, t => _loop.Stop());
		_loop.Loop();
		Print("END");
	}

	static void _TestThreadPoolSTA()
	{
		Catkeys.Util.ThreadPoolSTA.SubmitCallback(null, o =>
		{
			PrintList("worker", Api.GetCurrentThreadId());
			//Thread.Sleep(_random.Next(1, 10));
			Thread.Sleep(_random.Next(10, 100));
			//Thread.Sleep(2000);
			//Thread.Sleep(16000);
			//if(0==Interlocked.Decrement(ref _n)) Perf.NW();
		}, o =>
		{
			//PrintList("completion", Api.GetCurrentThreadId());
			if(--_n < 1) Perf.NW();
		});
	}

	static void _TestThreadPoolSTA2()
	{
		var work = Catkeys.Util.ThreadPoolSTA.CreateWork(null, o =>
		{
			PrintList("worker", Api.GetCurrentThreadId());
			//Thread.Sleep(_random.Next(1, 10));
			Thread.Sleep(_random.Next(10, 100));
			//Thread.Sleep(2000);
			//Thread.Sleep(16000);
			//if(0==Interlocked.Decrement(ref _n)) Perf.NW();
		}, o =>
		{
			//PrintList("completion", Api.GetCurrentThreadId());
			if(--_n < 1) Perf.NW();
		});

		work.Submit();
		//for(int i=0; i<10; i++) work.Submit();
		//Thread.Sleep(10); work.Cancel();
		//work.Submit();
		//work.Wait();
		//work.Submit();
		work.Wait();
		work.Dispose();
		//work.Submit();
	}

	#endregion

	//public static string[] Receive(string user, string password, string filter = "ALL", bool markSeen = false)
	//{
	//	using(var client = new ImapClient("imap.googlemail.com", true)) {
	//		//client.Port=993; client.UseSsl=true; //default
	//		if(!client.Connect()) { Print("failed to connect"); return null; }
	//		if(!client.Login(user, password)) { Print("failed to login"); return null; }
	//		var folder = client.Folders.Inbox;
	//		List<string> a = new List<string>();
	//		foreach(var m in folder.Search(filter, ImapX.Enums.MessageFetchMode.Tiny)) {
	//			Print(m.From);
	//			if(markSeen) m.Seen = true;
	//			//a.Add(m.ToEml());
	//			a.Add(m.DownloadRawMessage());
	//		}
	//		return a.ToArray();
	//	}
	//}

	//static void TestImapX()
	//{
	//	var a = Receive("qmgindi@gmail.com", "jucakgoogle", "UNSEEN", false);
	//	Print(a);
	//}

	static unsafe LPARAM _WndProc(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		//Wnd.Misc.PrintMsg(w, msg, wParam, lParam);

		var R = Api.DefWindowProc(w, msg, wParam, lParam);

		switch(msg) {
		case Api.WM_DESTROY:
			//case Api.WM_LBUTTONUP:
			Application.ExitThread();
			break;
		}

		return R;
	}
	static Native.WNDPROC _wndProcDelegate = _WndProc;

	static void TestWindowClassInterDomain()
	{
		var atom = Wnd.Misc.WindowClass.InterDomainRegister("InterDomain", _WndProc);
		Print(atom);
		Wnd w = Wnd.Misc.WindowClass.InterDomainCreateWindow(0, "InterDomain", "InterDomain", Native.WS_OVERLAPPEDWINDOW, 100, 100, 300, 100);
		Print(w);
		w.Show(true);
		Application.Run();
		Print("exit");
	}

	static void TestCorrectFileName()
	{
		string s = null;
		s = "valid";
		s = "a ?*<>\"/\\| \x01 \x1f \x00 b";
		//s = ".txt"; //valid
		//s = "a.";
		//s = " ab ";
		//s = "CON";
		s = "con";
		//s = "LPT5.txt";
		//s = "qwertyuiopasdfghjklzxcvbnm";
		Print(Path_.CorrectFileName(s));

		//var a1 = new Action(() => { s = Path_.CorrectFileName(s); });
		//Perf.ExecuteMulti(5, 1000, a1);

	}

	static void TestLnkShortcut()
	{
		string s;
		s = Folders.Programs + @"QTranslate\QTranslate.lnk"; //target is in the 32-bit PF
		s = Folders.CommonPrograms + @"Microsoft Office\Microsoft Office Access 2003.lnk"; //test MSI
		s = Folders.Desktop + @"ClassicStartMenu.exe - Shortcut.lnk"; //test the PF (x86) problem
		s = Folders.Programs + @"Test\test.lnk";
		//s = Folders.Programs + @"Test\virtual.lnk";
		//s = Folders.Programs + @"Test\URL.lnk";
		s = Folders.CommonPrograms + @"Accessories\Math Input Panel.lnk"; //test the PF (x86) problem in icon path (env var)

		try {
			//Print(Files.LnkShortcut.GetTarget(s));

			//Files.LnkShortcut.Delete(s);
			//return;

			Files.LnkShortcut x;

#if true
			x = Files.LnkShortcut.Open(s);
			Print("TargetPath: " + x.TargetPath);
			Print("TargetPathMSI: " + x.TargetPathRawMSI);
			Print("URL: " + x.TargetURL);
			Print("Shell name: " + x.TargetAnyType);
			var pidl = x.TargetIDList; Print("Name from IDList: " + Files.Misc.PidlToString(pidl, Native.SIGDN.SIGDN_NORMALDISPLAY)); Marshal.FreeCoTaskMem(pidl);
			Print("Hotkey: " + x.Hotkey);
			int ii; var iloc = x.GetIconLocation(out ii); Print($"Icon: {iloc}          ii={ii}");
			Print("Arguments: " + x.Arguments);
			Print("Description: " + x.Description);
			Print("WorkingDirectory: " + x.WorkingDirectory);
			Print("ShowState: " + x.ShowState);
#else
			x = Files.Shortcut.Create(s);
			x.Target = Folders.System + "notepad.exe";
			//x.SetIconLocation(@"q:\app\paste.ico");
			//Print(x.Hotkey);
			x.Hotkey = Keys.O | Keys.Control | Keys.Alt;
			//x.Hotkey = 0;
			x.Arguments = @"""q:\test\a.txt""";
			x.Description = "comments mmm";
			x.WorkingDirectory = @"c:\Test";
			x.Save();

			//x = Files.Shortcut.OpenOrCreate(s);
			////x.Target = Folders.System + "notepad.exe";
			//x.SetIconLocation(@"q:\app\paste.ico");
			//Print(x.Hotkey);
			//x.Hotkey = Keys.E | Keys.Control | Keys.Alt;
			////x.Hotkey = 0;
			//x.Save();

			//x = Files.Shortcut.Create(s);
			//x.TargetIDList = Folders.VirtualITEMIDLIST.ControlPanelFolder;
			////x.SetIconLocation(@"q:\app\paste.ico");
			//x.Save();

			//x = Files.Shortcut.OpenOrCreate(s);
			//x.SetIconLocation(@"q:\app\run.ico");
			//x.Save();

			//x = Files.Shortcut.Create(s);
			//x.TargetURL = "http://www.quickmacros.com";
			//x.SetIconLocation(Folders.System+"shell32.dll", 10);
			//x.Save();
#endif
			Print("fin");
		}
		catch(Exception e) { Print(e.Message); }

	}

	static void TestLnkShortcut2()
	{
		string folder = Folders.CommonPrograms;
		foreach(var f in Directory.EnumerateFiles(folder, "*.lnk", System.IO.SearchOption.AllDirectories)) {
			var x = Files.LnkShortcut.Open(f);
			string s = x.TargetAnyType;
			//s = x.TargetPath;
			//s = x.TargetPathRawMSI;
			//s = x.TargetURL;
			//var pidl = x.TargetIDList; s=Files.Misc.PidlToString(pidl, Native.SIGDN.SIGDN_NORMALDISPLAY); Marshal.FreeCoTaskMem(pidl);
			//s = x.Arguments;
			//s = x.Description;
			//s = x.WorkingDirectory;
			Print($"{Path.GetFileNameWithoutExtension(f),-50} {s}");

			//int ii; s = x.GetIconLocation(out ii); Print($"{Path.GetFileNameWithoutExtension(f),-50} {s}____{ii}");
			//Print($"{Path.GetFileNameWithoutExtension(f),-50} {x.Hotkey} {x.ShowState}");
		}
	}

	static void TestCsvCatkeys()
	{
		//		string s = @"A1,B1,C1
		//A2,B2,C2
		//A3,B3,C3
		//A4,B4,C4
		//A5,B5,C5
		//A6,B6,C6
		//A7,B7,C7
		//A8,B8,C8
		//A9,""a,b
		//c"",C9
		//A,""B """"Q"""" Z"",C
		//";

		string s = @"  A1  ,  B1  ,  Ŧͷת  
A2,
A9,  ""a,b""  ,
""new
line""	,	m	,		"" n ""	
A,""B """"Q"""" Z"",C
";
		var file = Folders.Temp + "csv.csv";
		File.WriteAllText(file, s);

		//s = @"  A1  ;  B1  ;  C1  
		//A2
		//A9;  ""a;b""  ;
		//""new
		//line"";m;"" n ""
		//A;""B """"Q"""" Z"";C
		//";

		//		s = @"  A1  ,  B1  ,  Ŧͷת  
		//A2,
		//A9,  'a,b'  ,
		//'new
		//line',m,' n '
		//A,'B ''Q'' Z',C
		//";

		try {
			//var x = new CsvTable(s);
			var x = new CsvTable();
			//x.TrimSpaces = false;
			//x.Separator = ';';
			//x.Quote = '\'';
			x.FromString(s);
			//x.FromString(File.ReadAllText(file));
			//x[-1, 0] = "A1";
			//x[-1, 2] = "B3";
			//x.ColumnCount = 6;
			//x.ColumnCount = 2;
			//x.RowCount = 2;
			////x.ColumnCount = 6;
			//x[0,3]="r";
			//x[0] = new string[] { "a", "b" };
			//x[-1] = new string[] { null, "", "c", "d" };
			//Print(x[0]);
			//x.InsertRow(2);
			//x.InsertRow(2, new string[] { "a", "b" });
			//x.InsertRow(2, new string[] { null, "", "c", "d" });
			//x.RemoveRow(1, 3);
			//x.Data.Sort((a,b) => string.CompareOrdinal(a[0], b[0]));
			//x.Data.RemoveRange(0, 4);
			//x[0, 0] = 20.ToString();
			//int y = x[0, 0].ToInt_(); Print(y);
			//x.SetInt(0, 0, 20, true);
			//int y = x.GetInt(0, 0)); Print(y);
			//x.SetDouble(0, 0, 0.00055);
			//Print(x.GetDouble(0, 0));
			//x.InsertRow(0, new string[] { "g", "h" });
			//x.InsertRow(0, "n", "m");
			//x.InsertRow(-1);
			//x.Data[0] = new string[] { "x", "y", "z" };
			//x.ColumnCount = 3;

			PrintList(x.RowCount, x.ColumnCount);
			Print("----------");
			for(int r = 0; r < x.RowCount; r++) {
				Print(r);
				for(int c = 0; c < x.ColumnCount; c++) {
					string f = x[r, c];
					Print(f == null ? "<NULL>" : f);
				}
			}
			Print("----------");
			Print(x);
		}
		catch(Exception e) { Print(e.Message); }



		//p.Dispose();
	}

	#endregion

	#region test end back thread

	class MyAppContext :ApplicationContext
	{
		protected override void ExitThreadCore()
		{
			PrintFunc();
			base.ExitThreadCore();
		}
	}

	static void _BackThread()
	{
		//Print(Api.GetCurrentThreadId());
		//Application.ThreadExit += Application_ThreadExit;
		Print(1);
		try {
			//Application.Run(_appContext);
			Application.Run();
			//Application.Run(new Form());
			//_loop.Loop();
			//Wait(1000);
			//Native.MSG m; while(Api.GetMessage(out m, Wnd0, 0, 0) > 0) Api.DispatchMessage(ref m);
		}
		catch(ThreadAbortException) { Print("abort exception"); }
		Print(2);
	}

	static Catkeys.Util.MessageLoop _loop = new Catkeys.Util.MessageLoop();
	//static MyAppContext _appContext=new MyAppContext();

	private static void Application_ThreadExit1(object sender, EventArgs e)
	{
		PrintFunc();
	}

	private static void Application_ThreadExit2(object sender, EventArgs e)
	{
		PrintFunc();
	}

	private static void Application_ApplicationExit(object sender, EventArgs e)
	{
		PrintFunc();
		//Application.ApplicationExit -= Application_ApplicationExit;
		//Print(Api.GetCurrentThreadId());
	}

	static Thread _thread;

	static void TestBackThreadEnd()
	{
		_thread = new Thread(_BackThread);
		_thread.IsBackground = true;
		_thread.SetApartmentState(ApartmentState.STA);
		_thread.Start();

		//GC.Collect();
		//TaskDialog.Show("main");
		//MessageBox.Show("");
		//MessageBoxX(Wnd0, "", "", 0);

		//Time.SetTimer(1000, true, t => { Application.ExitThread(); });
		//Application.Run();
		//Print("after main loop");

		//Print(Api.GetCurrentThreadId());
		//_thread.Abort();
		//Application.Exit();
	}

	[DllImport("user32.dll", EntryPoint = "MessageBoxW")]
	public static extern int MessageBoxX(Wnd hWnd, string lpText, string lpCaption, uint uType);

	//class ExitClass
	//{
	//	public ExitClass()
	//	{
	//		Print("ctor");
	//	}

	//	~ExitClass()
	//	{
	//		//Print(AppDomain.CurrentDomain.IsFinalizingForUnload());
	//		Print("dtor");
	//	}
	//}

	//static ExitClass _exit=new ExitClass();

	#endregion

	#region test icons

	static void TestIcons2()
	{
		//Print("start");
		//Thread.Sleep(1000);

		var a = new List<string>();
		int n = 0;
#if true
		foreach(var f in Directory.EnumerateFiles(@"q:\app")) {
			//Print(f);
			a.Add(f);
			//if((n & 1) == 0) a.Add(f);
			//if(++n == 30) break;
		}
#endif
#if false
		a.Add("mailto:");
		a.Add(@"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
		a.Add(@"q:\app");
		a.Add(Folders.Favorites);
		a.Add("http://www.quickmacros.com/");
		a.Add("::{21EC2020-3AEA-1069-A2DD-08002B30309D}");
		a.Add(@"C:\Users\G\Desktop\QM in PF.lnk");
#endif
#if false
		a.Add("q:\\app\\Cut.ico");
		a.Add(@"Q:\Programs\ILSpy\ILSpy.exe");
		a.Add(Folders.System + "notepad.exe");
		a.Add("q:\\app\\Copy.ico");
		a.Add("q:\\app\\Paste.ico");
		a.Add("q:\\app\\Run.ico");
		a.Add("q:\\app\\Tip.ico");
		//a.Add("notepad.exe");
		a.Add(Folders.ProgramFilesX86 + @"PicPick\picpick.exe");
		a.Add(@"Q:\Programs\DebugView\Dbgview.exe");
		a.Add(@"Q:\Programs\ProcessExplorer\procexp.exe");
		a.Add(Folders.ProgramFilesX86 + @"Inno Setup 5\Compil32.exe");
		a.Add(Folders.ProgramFilesX86 + @"HTML Help Workshop\hhw.exe");
		a.Add(Folders.ProgramFilesX86 + @"FileZilla FTP Client\filezilla.exe");
		a.Add(Folders.ProgramFilesX86 + @"Internet Explorer\IEXPLORE.EXE");
		a.Add(@"Q:\Programs\ProcessMonitor\Procmon.exe");
		a.Add(Folders.ProgramFilesX86 + @"Resource Hacker\ResourceHacker.exe");
		a.Add(@"Q:\programs\Autoruns\autoruns.exe");
		//a.Add(Folders.ProgramFilesX86 + @"SyncBackFree\SyncBackFree.exe");
		a.Add(@"Q:\Programs\PeView\PEview.exe");
		a.Add(Folders.System + @"shell32.dll,25");
#endif

		//Print(Api.GetCurrentThreadId());
		var F = new Form();
		F.Click += (unu, sed) =>
		{
			_n = a.Count;
			Perf.First();
			foreach(var s in a) {
				//_TestIconsSync(s);
				_TestIconsAsync(s);
			}
		};
		F.ShowDialog();
		//Print(2);

		//var m = new CatMenu();
		//m["aaaaaaaaa"] = null;
		//m.Show();
		//TaskDialog.ShowEx("", timeoutS: 1);
		//new Form().ShowDialog();
		//Time.SetTimer(1000, true, t => _loop2.Stop()); _loop2.Loop();
		//Time.SetTimer(1000, true, t => Application.ExitThread()); Application.Run();
		//PrintList("end", _n);
	}

	static Catkeys.Util.MessageLoop _loop2 = new Catkeys.Util.MessageLoop();

	static int _n;
	static Random _random = new Random();

	static async void _TestIconsAsync(string s)
	{
		uint tid = Api.GetCurrentThreadId();
#if true
		var task = Task.Run(() =>
		{
			//PrintList(Api.GetCurrentThreadId(), s);
			//var perf = new Perf.Inst(true);
			var R = Icons.GetFileIconHandle(s, 16, 0);
			//var R = Zero; Thread.Sleep(s.Length * s.Length / 100);
			//var R = Zero; Thread.Sleep(_random.Next(1, 10));
			//perf.Next(); PrintList(perf.Times, s);
			return R;
		});
#else
		var task = Task.Factory.StartNew(() =>
		{
			//PrintList(Api.GetCurrentThreadId(), s);
			//var perf = new Perf.Inst(true);
			var R = Icons.GetIconHandle(s, 16, 0);
			//var R = Zero; Thread.Sleep(_random.Next(1, 10));
			//perf.Next(); PrintList(perf.Times, s);
			return R;
		}, CancellationToken.None, TaskCreationOptions.None, _staTaskScheduler);
#endif
		await task;
		IntPtr hi = task.Result;

		//Interlocked.Decrement(ref _n);
		_n--;
		if(_n < 1) Perf.NW();
		//PrintList(tid, Api.GetCurrentThreadId());

		if(hi == Zero) {
			PrintList("failed", s);
			return;
		}
		Api.DestroyIcon(hi);
	}

	//static readonly System.Threading.Tasks.Schedulers.StaTaskScheduler _staTaskScheduler = new System.Threading.Tasks.Schedulers.StaTaskScheduler(4); //tested: without StaTaskScheduler would be 4 threads. With 3 the UI thread is slightly faster.

	static void _TestIconsSync(string s)
	{
		//var perf = new Perf.Inst(true);
		var hi = Icons.GetFileIconHandle(s, 16, 0);
		//perf.Next(); PrintList(perf.Times, s);

		if(--_n == 0) Perf.NW();

		if(hi == Zero) {
			PrintList("failed", s);
			return;
		}
		Api.DestroyIcon(hi);
	}

	[DllImport("kernel32.dll")]
	public static extern bool SetProcessAffinityMask(IntPtr hProcess, LPARAM dwProcessAffinityMask);

	static void TestIcons()
	{
		//SetProcessAffinityMask(Api.GetCurrentProcess(), 9); //like without hyperthreading

		var a = new List<string>();
		int i, n = 0;

#if true
		bool lnk = true;
		string folder, pattern = "*"; bool recurse = true;

		if(lnk) {
			folder = Folders.CommonPrograms;
			foreach(var f in Directory.EnumerateFiles(folder, "*.lnk", System.IO.SearchOption.AllDirectories)) {
				//Print(f);
				a.Add(f);
				n++;
				//if(n == 44) break;
			}
		} else {
			folder = @"q:\app"; recurse = false;
			//folder = @"q:\app"; pattern = "*.ico";
			//folder = @"q:\app"; pattern = "*.cur";
			//folder = @"c:\windows\cursors"; pattern = "*.ani";
			//folder =@"q:\app\catkeys\tasks";
			//folder = @"c:\program files (x86)";
			//folder = @"c:\program files";
			//folder = @"c:\programdata";
			//folder = @"c:\users";
			//folder = @"c:\windows";
			//folder = @"c:\windows\system32"; pattern = "*.exe"; recurse = false;
			//folder = @"c:\windows\system32"; pattern = "*.msc";
			//folder = @"c:\windows"; pattern = "*.scr";
			//folder = @"c:\windows"; pattern = "*.lnk";
			//folder = @"c:\windows"; pattern = "*.cpl";
			//folder = @"q:\";
			//folder = @"q:\downloads"; pattern = "*.exe"; recurse = false;

			var oneExt = new HashSet<string>();
			//foreach(var f in Directory.EnumerateFiles(folder)) {
			//foreach(var f in Directory.EnumerateFileSystemEntries(folder, pattern, System.IO.SearchOption.AllDirectories)) {
			foreach(var f in LibTest.EnumerateFiles(folder, pattern, recurse)) {
				//Print(f);
				//if(pattern == "*") {
				//	var ext = Path.GetExtension(f).ToLower();
				//	if(oneExt.Contains(ext)) continue; else oneExt.Add(ext);
				//}
				var s = Path.GetFileName(f);
				//if(0 != s.Like_(true, "*.aps", "*.tss", "*.bin", "*.wal", "*.???_?*", "*.????_?*")) continue;
				if(0 != s.RegexIs_(RegexOptions.IgnoreCase, @"\.\w+?[_\-][^\.]+$", @", PublicKeyToken", @"^(LanguageService|TextEditor|WindowManagement)", @"\bVisualStudio\b")) continue;
				//if(n>=3000)
				a.Add(f);
				//var k = f.RegexReplace_(@"(?i)^C:\\windows\\system32", @"C:\Users\G\Desktop\system64");
				//Print($"{s} : * {k}");
				n++;
				if(n == 15) break;
				//break;
			}
		}
#elif false
		//a.Add(@"c:\windows\Boot\DVD\PCAT\etfsboot.com");
		//a.Add(@"c:\windows\System32\Bubbles.scr");

		//a.Add(@"c:\test\Z.appcontent-ms");
		//a.Add(@"c:\test\Z.appref-ms");
		//a.Add(@"c:\test\Z.as");
		//a.Add(@"c:\test\Z.asa");
		//a.Add(@"c:\test\Z.asp");
		//a.Add(@"c:\test\Z.axd");
		//a.Add(@"c:\test\Z.cdx");
		//a.Add(@"c:\test\Z.cdxml");
		//a.Add(@"c:\test\Z.cfm");
		//a.Add(@"c:\test\Z.chk");

		//a.Add(@".appcontent-ms");
		//a.Add(@".appref-ms");
		//a.Add(@".as");
		//a.Add(@".asa");
		//a.Add(@".asp");
		//a.Add(@".axd");
		//a.Add(@".cdx");
		//a.Add(@".cdxml");
		//a.Add(@".cfm");
		//a.Add(@".chk");
		////a.Add(@".txt");

		a.Add(@".com");
		a.Add(@".p7s");
		a.Add(@".pano");
		a.Add(@".sst");
#else //all registered file types
		var d = new SortedDictionary<string, string>();
		string s;
		using(var k1 = Registry_.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts")) {
			string[] sub1 = k1.GetSubKeyNames();
			//Print(sub1);
			foreach(var s1 in sub1) {
				if(!Registry_.GetString(out s, "ProgId", s1 + @"\UserChoice", k1)) { /*PrintList(s1, "-");*/ continue; }
				//PrintList(s1, s);
				d.Add(s1, s);
			}
		}

		{
			string[] sub1 = Registry.ClassesRoot.GetSubKeyNames();
			//Print(sub1);
			foreach(var s1 in sub1) {
				if(s1[0] != '.') continue;
				if(d.ContainsKey(s1)) continue;
				if(!Registry_.GetString(out s, "", s1, Registry.ClassesRoot)) { /*PrintList(s1, "-");*/ continue; }
				//PrintList(s1, s);
				d.Add(s1, s);
			}
		}

		//Print(d);

		foreach(var v in d) {
			//Print($"<><Z 0x80E080>{v.Key}</Z>");
			//Print($"{v.Key} : * {v.Key}");
			a.Add(v.Key);
			//a.Add(v.Value+":");
		}
		//return;
#endif

		Print(n);
		//FileIconInit(false);

		int size = 0;
		//size = Icons.GetShellIconSize(Icons.ShellSize.Small);
		//size = Icons.GetShellIconSize(Icons.ShellSize.Large);
		//size = Icons.GetShellIconSize(Icons.ShellSize.ExtraLarge);
		//size = Icons.GetShellIconSize(Icons.ShellSize.Jumbo);

#if true
		var m = new CatBar();
		if(size > 0) m.Ex.ImageScalingSize = new Size(size, size);
		//m.IconFlags = Icons.IconFlags.Shell;

		//m.Ex.AutoSize = false;
		m.Ex.LayoutStyle = ToolStripLayoutStyle.Flow;

		m.ItemAdded += g => { g.Margin = new Padding(4, 0, 0, 0); };

		//m["cut", @"q:\app\cut.ico"] = null;
		//m["copy", @"q:\app\copy.ico"] = null;
		//m["paste", @"q:\app\paste.ico"] = null;

		//m["Calculator", @"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"] = null;
		//m["Virt folder", @"::{20d04fe0-3aea-1069-a2d8-08002b30309d}"] = null;
		//m[".cs", @".cs"] = null;

		for(i = 0; i < a.Count; i++) {
			if(size == 256 && i < 12) continue;
			var u = a[i];
			m[Path.GetFileName(u), u] = null;
			//m[Path.GetFileName(u), @"q:\app\paste.ico"] = null;
		}

		//m.Ex.Click += (unu, sed) => _mlTb.Stop();
		m.Ex.MouseUp += (unu, sed) => _mlTb.Stop();
		Perf.First();
		m.Visible = true;
		//m.Visible = false; m["cut", @"q:\app\cut.ico"] = null; m.Visible = true;
		_mlTb.Loop();
		m.Close();
		//Print("exit");
#elif true
		for(int c = 0; c < 1; c++) {
			var m = new CatMenu();
			//if(size > 0) m.CMS.ImageScalingSize = new Size(size, size);
			if(size > 0) m.IconSize = size;
			//m.ActivateMenuWindow = true;

			for(i = 0; i < a.Count; i++) {
				var s = a[i];
				m[Path.GetFileName(s), s] = null;

				//if(i == 0) m.LastItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
			}

			using(m.Submenu("sub")) {
				m.LastMenuItem.DropDown.ImageScalingSize = new Size(24, 24);
				m["two", a[0]] = null;
			}

			Perf.First();
			m.Show();
		}
#elif false
		var m = new CatMenu();
		m.CMS.ImageScalingSize = new Size(size, size);

		Perf.First();
		for(i = 0; i < a.Count; i++) {
			var hi = Icons.GetIconHandle(a[i], size);
			if(hi != Zero) {
				//var ic = Icon.FromHandle(hi);
				//var im = ic.ToBitmap();
				//m[Path.GetFileName(a[i]), im] = null;

				m[Path.GetFileName(a[i]), hi] = null;

				Api.DestroyIcon(hi);
			}
		}
		Perf.NW();

		m.Show();
#else
		var ai = new IntPtr[a.Count];
		Print(a.Count);

		var m = new CatMenu();
		m.CMS.ImageScalingSize = new Size(size, size);

		try {
			Perf.First();
			for(i = 0; i < a.Count; i++) {
				ai[i] = Icons.GetIconHandle(a[i], size);
				//Print(ai[i]);
				//Api.DestroyIcon(ai[i]); ai[i] = Zero;
            }
			Perf.NW();

			for(i = 0; i < a.Count; i++) {
				var s = a[i];
				//Print(i);
				//m[Path.GetFileName(s), ai[i]] = null;
				m[Path.GetFileName(s)] = null;
			}
		}
		finally {
			for(i = 0; i < ai.Length; i++) Api.DestroyIcon(ai[i]);
		}

		m.Show();
#endif
	}

	//[DllImport("shell32.dll", EntryPoint ="#660")]
	//static extern bool FileIconInit(bool restorFromDisk);

	#endregion test icons

	#region test sqlite

#if false
	static void TestSqlite()
	{
		var sb = new StringBuilder();

		Perf.First();
		var file = Folders.Temp + "test.db3";
		bool isNew = !Files.FileExists(file);
		if(isNew) SQLiteConnection.CreateFile(file);
		Perf.Next();

		SQLiteConnection m_dbConnection;
		m_dbConnection = new SQLiteConnection($"Data Source={file};Version=3;");
		m_dbConnection.Open();
		Perf.Next();
		try {
			string sql;

			if(isNew) {
				sql = "create table highscores (name varchar(20), score int)";
				using(var c = new SQLiteCommand(sql, m_dbConnection)) c.ExecuteNonQuery();

				using(var tran = m_dbConnection.BeginTransaction()) {
					sql = "insert into highscores (name, score) values ('Me', 9001)";
					using(var c = new SQLiteCommand(sql, m_dbConnection)) c.ExecuteNonQuery();

					tran.Commit();
				}
				Perf.Next();
			}

			for(int i = 0; i < 5; i++) {
				sql = "select * from highscores order by score desc";
				using(var c = new SQLiteCommand(sql, m_dbConnection)) {
					using(var reader = c.ExecuteReader())
						while(reader.Read())
							sb.AppendLine("Name: " + reader["name"] + "\tScore: " + reader["score"]);
				}
				Perf.Next();
			}
		}
		finally {
			m_dbConnection.Close();
		}
		Perf.NW();
		Print(sb);
	}
#elif false
	static void TestSqlite()
	{
		var sb = new StringBuilder();

		Perf.First();
		var file = Folders.Temp + "test3.db3";
		bool isNew = !Files.FileExists(file);
		//if(isNew) SQLiteConnection.CreateFile(file);
		Perf.Next();

		Perf.NW();
		Print(sb);
	}
#else

	public class Stock
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[MaxLength(8)]
		public string Symbol { get; set; }
	}

	const string LibraryPath = "Sqlite.Interop.dll";
	static void TestSqlite()
	{
		var sb = new StringBuilder();

		Perf.First();
		var file = Folders.Temp + "test2.db3";
		bool isNew = !Files.ExistsAsFile(file);
		Perf.Next();

		using(var db = new SQLiteConnection(file)) {
			Perf.Next();

			if(isNew) {
				db.CreateTable<Stock>();

				var s = db.Insert(new Stock() {
					Symbol = "one"
				});
				Perf.Next();
			}

			for(int i = 0; i < 5; i++) {
#if true
				//var query = db.Table<Stock>().Where(v => v.Symbol.StartsWith("A"));

				var query = db.Table<Stock>();
				foreach(var stock in query)
					sb.AppendLine("Stock: " + stock.Symbol);
#else
				foreach(var stock in db.Query<Stock>("select * from Stock"))
					sb.AppendLine("Stock: " + stock.Symbol);
#endif
				Perf.Next();
			}
		}
		Perf.NW();

		Print(sb);
	}
#endif

	static void TestSqlite2()
	{
	}

	#endregion

	static void TestCatkeysListFileCSV()
	{
		int size = 0, nRows = 0;
		List<string> k = null;
		for(int i = 0; i < 5; i++) {
			Perf.First();
			var s = File.ReadAllText(@"q:\test\ok\LIST.csv");
			size = s.Length;
			Perf.Next();
			var x = new CsvTable(); x.Separator = '|';
			x.FromString(s);
			nRows = x.RowCount;
			//Perf.Next();
			//k = new List<string>(nRows);
			//for(int j = 0; j < nRows; j++) {
			//	k.Add(x[j, 0]);
			//}
			Perf.NW();
		}
		Print(k);
		Print($"{size / 1024.0:F3} KB, {nRows} rows");
	}

	static void TestCatkeysListFileXML()
	{
		int size = 0, nRows = 0;
		List<string> k = null;
		for(int i = 0; i < 5; i++) {
			Perf.First();
#if true
			var s = "";
			var x = new XmlDocument();
			x.Load(@"q:\test\ok\LIST.xml");
#else
			var s = File.ReadAllText(@"q:\test\ok\LIST.xml");
			Perf.Next();
			var x = new XmlDocument();
			x.LoadXml(s);
#endif
			Perf.Next();
			//nRows = x.FirstChild.ChildNodes.Count;
			var f = x.FirstChild.ChildNodes;
			nRows = f.Count;
			k = new List<string>(nRows);

			//for(int j = 0; j<nRows; j++) {
			//	//k.Add(f[j].Attributes["n"].Value);
			//	//size += f[j].Attributes["n"].Value.Length;
			//	var p =f[j]; //very slow
			//	k.Add(p.InnerText);
			//	//size += f[j].Value.Length;
			//}

			foreach(XmlNode p in f) {
				k.Add(p.InnerText); //quite fast but 3-4 times slower than CSV
			}

			Perf.NW();
			size = s.Length;
		}
		Print(k);
		Print($"{size / 1024.0:F3} KB, {nRows} rows");
	}



	//using l = Catkeys;
	//using static Catkeys.NoClass;
	////
	//using System.Collections.Generic;
	//using SysText = System.Text;
	//using SysRX = System.Text.RegularExpressions;
	//using SysDiag = System.Diagnostics;
	//using SysInterop = System.Runtime.InteropServices;
	//using SysCompil = System.Runtime.CompilerServices;
	//using SysIO = System.IO;
	//using SysThread = System.Threading;
	//using SysTask = System.Threading.Tasks;
	//using SysReg = Microsoft.Win32;
	//using SysForm = System.Windows.Forms;
	//using SysDraw = System.Drawing;
	////using System.Linq;



	//using SysColl = System.Collections.Generic; //add directly, because often used, and almost everything is useful
	//using SysCompon = System.ComponentModel;
	//using SysExcept = System.Runtime.ExceptionServices; //only HandleProcessCorruptedStateExceptionsAttribute

	public static class CatAlias
	{
		public static int speed { get { return Script.Speed; } set { Script.Speed = value; } }
	}

	static unsafe void TestSharedMemory()
	{
		try {
			var m1 = (int*)Catkeys.Util.SharedMemory.CreateOrGet("test", 1000, true);
			Print((long)m1);
			*m1 = 7;
			//var m2 = (int*)Catkeys.Util.SharedMemory.CreateOrGet("test", 1000000);
			//Print((long)m2);
			//Print(*m2);



			TaskDialog.Show("");
		}
		catch(Exception e) { Print(e); }
	}


	//[DllImport("kernel32.dll")]
	//public static unsafe extern int CompareStringOrdinal(char* lpString1, int cchCount1, char* lpString2, int cchCount2, bool bIgnoreCase);

	//public const uint FIND_FROMSTART = 0x400000;
	//public const uint FIND_FROMEND = 0x800000;
	//public const uint FIND_STARTSWITH = 0x100000;
	//public const uint FIND_ENDSWITH = 0x200000;

	//[DllImport("kernel32.dll")]
	//public static unsafe extern int FindStringOrdinal(uint dwFindStringOrdinalFlags, char* lpStringSource, int cchSource, char* lpStringValue, int cchValue, bool bIgnoreCase);

	//public static unsafe int IndexOf2_(this string t, string s, bool ignoreCase = false)
	//{
	//	//speed: similar to string.IndexOf. In some cases faster, in others slower.
	//	fixed (char* s1 = t, s2 = s) {
	//		return FindStringOrdinal(FIND_FROMSTART, s1, t.Length, s2, s.Length, ignoreCase);
	//	}
	//}

	static void _LikeAssert(string p, string s, bool r)
	{
		Debug.Assert(s.Like_(p) == r);
		//Debug.Assert(s.LikeEx_(p) == r);
	}

	static bool TestLikeEx_(this string t, string pattern, bool ignoreCase = false)
	{
		return Operators.LikeString(t, pattern, ignoreCase ? CompareMethod.Text : CompareMethod.Binary);
	}

	static void TestWildcard()
	{
		string s = null, p = null;
		//s = "Microsoft Help Ąč Viewer 2.2 - Visual Studio Documentation Ąč";
		//s = "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD Documentation Ąč";
		s = "Microsoft Help Ąč Viewer 2.2 - Visual Studio Documentation Ąč";
		//s = "short";
		//s += s;s += s;
		p = "* Documentation Ąč";
		p = "*ocumentation Ąč";
		p = "Microsoft Help Ąč *";
		p = "*?ocumentation*";
		p = "* Help Ąč Viewer ?.? -*";
		p = "* Help Ąč Viewer 2.2 - Visual Studio Documentation *";
		p = "* Help Ąč Viewer ?.? -* Documentation Ąč";
		p = "*ocumentation*";

		//s = "{267F16E4-020F-445C-9380-EA4D94291F77}";
		//p = "{*-*-*-*-*}";
		//p = s.Insert(0, " ").Remove(0, 1); //use this instead of p = s;, which makes Equals_ etc much faster, cannot compare speed
		//p = "*Microsoft Help Ąč Viewer 2.2+ - Visual Studio Documentation Ąč";
		//Debug.Assert((object)p != (object)s);
		//p = p.ToLower_();
		//p = p + "*";

		//s = "A 	B 	C 	Ç 	D 	E 	F 	G 	Ğ 	H 	I 	İ 	J 	K 	L 	M 	N 	O 	Ö 	P 	R 	S 	Ş 	T 	U 	Ü 	V 	Y 	Z";
		//p = "a 	b 	c 	ç 	d 	e 	f 	g 	ğ 	h 	ı 	i 	j 	k 	l 	m 	n 	o 	ö 	p 	r 	s 	ş 	t 	u 	ü 	v 	y 	z";

		bool ignoreCase = false;

		//Print(s.Like_(p, ignoreCase)); return;

#if !DEBUG
		bool b1 = false, b2 = false, b3 = false, b4 = false, b5 = false, b6 = false;
		//var a1 = new Action(() => { b1 = s.LikeEx_(p, ignoreCase); });
		var a2 = new Action(() => { b2 = s.Like_(p, ignoreCase); });
		//var a6 = new Action(() => { b6 = s.Like2_(p, ignoreCase); });
		var p2 = p.Replace("*", "");
		var a3 = new Action(() => { b3 = s.Equals_(p2, ignoreCase); });
		var a4 = new Action(() => { b4 = s.StartsWith_(p2, ignoreCase); });
		var a5 = new Action(() => { b5 = s.IndexOf_(p2, ignoreCase) >= 0; });
		Perf.ExecuteMulti(5, 1000, a2, a3, a4, a5);
		PrintList(b2, b3, b4, b5);
#else
		//_LikeAssert("A*?", "AB", false);

		//_LikeAssert("A**", "A*", true);
		//_LikeAssert("**A", "*A", true);
		//_LikeAssert("A**B", "A*B", true);
		//_LikeAssert("A**B*C*?D", "A*BZZZC?D", true);
		//_LikeAssert("A**", "AB", false);
		//_LikeAssert("**A", "BA", false);
		//_LikeAssert("A**B", "ABB", false);

		//_LikeAssert("A*?", "A?", true);
		//_LikeAssert("*?A", "?A", true);
		//_LikeAssert("A*?B", "A?B", true);
		//_LikeAssert("A*?", "AB", false);
		//_LikeAssert("*?A", "BA", false);
		//_LikeAssert("A*?B", "ABB", false);

		_LikeAssert("A*", "A", true);

		_LikeAssert("A?*", "A", false);
		_LikeAssert("A?*", "ABBCC", true);
		_LikeAssert("A?*", "BAA", false);
		_LikeAssert("A?*B", "AZZB", true);
		_LikeAssert("A?*B", "AAABBB", true);

		_LikeAssert("A*ZB", "AZBnnnAB", false);

		_LikeAssert("A*B", "ABAB", true);
		//_LikeAssert("A*?", "ABAB", true);
		_LikeAssert("A?", "AAB", false);
		_LikeAssert("A*B", "AABA", false);

		_LikeAssert("", "", true);
		_LikeAssert("*", "", true);
		_LikeAssert("*", "A", true);
		_LikeAssert("", "A", false);
		_LikeAssert("A*", "", false);
		_LikeAssert("A*", "AAB", true);
		_LikeAssert("A*", "BAA", false);
		_LikeAssert("A*B", "", false);
		_LikeAssert("A*B", "AAB", true);
		_LikeAssert("A*B", "AB", true);
		_LikeAssert("A*B", "ABBBB", true);
		_LikeAssert("A*B*C", "", false);
		_LikeAssert("A*B*C", "ABC", true);
		_LikeAssert("A*B*C", "ABCC", true);
		_LikeAssert("A*B*C", "ABBBC", true);
		_LikeAssert("A*B*C", "ABBBBCCCC", true);
		_LikeAssert("A*B*C", "ABCBBBCBCCCBCBCCCC", true);
		_LikeAssert("A*B*", "AB", true);
		_LikeAssert("A*B*", "AABA", true);
		_LikeAssert("A*B*", "ABAB", true);
		_LikeAssert("A*B*", "ABBBB", true);
		_LikeAssert("A*B*C*", "", false);
		_LikeAssert("A*B*C*", "ABC", true);
		_LikeAssert("A*B*C*", "ABCC", true);
		_LikeAssert("A*B*C*", "ABBBC", true);
		_LikeAssert("A*B*C*", "ABBBBCCCC", true);
		_LikeAssert("A*B*C*", "ABCBBBCBCCCBCBCCCC", true);
		_LikeAssert("A?B", "AAB", true);

		_LikeAssert("*ZZ*", "AZZB", true);
		_LikeAssert("*AZZ*", "AZZB", true);
		_LikeAssert("*ZZB*", "AZZB", true);

		_LikeAssert("A**B", "AZB", true);
		_LikeAssert("**AB", "AB", true);
		_LikeAssert("AB**", "AB", true);
		_LikeAssert("A*B**", "AZB", true);
		_LikeAssert("**A*B", "AZB", true);

		_LikeAssert("A*ABAZB", "AZB", false);

		Print("ok");
#endif
	}

	static void TestRegexEtcSpeed()
	{
		bool b1 = false, b2 = false, b3 = false, b4 = false, b5 = false, b6 = false, b7 = false, b8 = false;

		var rx = @"One Two Three Four Five One Two Three Four Five One Two Three Four Five One Two Three Four Five One Two Three Four Five ";
		var r1 = new Regex(rx);
		var r2 = new Regex(rx, RegexOptions.CultureInvariant);
		var r3 = new Regex(rx, RegexOptions.IgnoreCase);
		var r4 = new Regex(rx, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		string s = (" " + rx).Remove(0, 1);

		var a1 = new Action(() => { b1 = r1.IsMatch(s); });
		var a2 = new Action(() => { b2 = r2.IsMatch(s); });
		var a3 = new Action(() => { b3 = r3.IsMatch(s); });
		var a4 = new Action(() => { b4 = r4.IsMatch(s); });
		var a5 = new Action(() => { b5 = s.Like_(rx); });
		var a6 = new Action(() => { b6 = s.Equals_(rx); });
		var a7 = new Action(() => { b6 = s.Equals_(rx, true); });
		var a8 = new Action(() => { b7 = s.TestLikeEx_(rx); });
		var a9 = new Action(() => { b8 = s.TestLikeEx_(rx, true); });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6, a7, a8, a9);

		PrintList(b1, b2, b3, b4, b5, b6, b7, b8);
	}

	static void TestWildex()
	{
		////Find item whose name contains "example" and date starts with "2017-". Case-insensitive.
		//var x = Find3("[P]example", "2017-*");

		//Print(Find3("two", ""));
		//Print("end");

		//TODO: test Wnd.GetAll with LINQ from etc.

		string s = null, p = null;

#if false
		s = @"C";
		//p = s.Insert(0, "m").Remove(0, 1);
		p = s;

		bool b1 = false, b2 = false, b3 = false, b4 = false;
		string s1 = null, s2 = null;

		var w1 = new Wildex(p);
		var w2 = new String_.Wildex3(p);

		int n = 1000;
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b1 = w1.Match(s); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b2 = w2.Match(s); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b1 = Cmp1(s, p); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b2 = Cmp2(s, p); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { s1 = w1.Text; } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { s2 = w2.Text; } Perf.Next(); } Perf.Write();

		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { var t=new TOM1() { w = new Wildex(p) }; b3 = t.w.Match(s); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { var t = new TOM2() { w = new String_.Wildex3(p) }; b4 = t.w.Match(s); } Perf.Next(); } Perf.Write();

		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b1 = Cmp3(s, p); } Perf.Next(); } Perf.Write();
		Perf.First(); for(int i1 = 0; i1 < 5; i1++) { for(int i2 = 0; i2 < n; i2++) { b2 = Cmp4(s, p); } Perf.Next(); } Perf.Write();


		//var a1 = new Action(() => { b1 = w1.Match(s); });
		//var a2 = new Action(() => { b2 = w2.Match(s); });
		//var a3 = new Action(() => { var w3 = new Wildex(p); b3 = w3.Match(s); });
		//var a4 = new Action(() => { var w4 = new String_.Wildex3(p); b4 = w4.Match(s); });
		////var a4 = new Action(() => { String_.Wildex3 w4 = p; b4 = w4.Match(s); });
		//var a5 = new Action(() => { s1 = w1.Text; });
		//var a6 = new Action(() => { s2 = w2.Text; });
		////Perf.ExecuteMulti(5, n, a1, a2, a3, a4, a5, a6);
		//Perf.ExecuteMulti(5, n, a1, a3);
		//Print("");
		//Perf.ExecuteMulti(5, n, a2, a4);

		PrintList(b1, b2, b3, b4, s1, s2);
#else
		s = @"C:\*a\b.exe";
		p = @"*.exE";
		p = @"[c]*.exE";
		p = @"C:\*\b.exe";
		p = @"[L]C:\*\b.exe";
		p = @"[R]^c.+\.exe$";
		p = @"[n]*.exE";
		p = @"[m]one[]*.exE[][n]d*";
		//p = ""; s = "";

		var x = new Wildex(p);
		PrintList($"i={x.IgnoreCase}, n={x.Not}, type={x.TextType}, {x.ToString()}");
		Print(x.Match(s));
#endif
	}

	static void TestWndWithWildex()
	{

		//Wnd w = Wnd.Find("[E]Notepad", "NotepaD", "notepaD");
		//Wnd w = Wnd.Find("[E]Notepad", "NotepaD", Folders.SystemX86+"notepaD.exe", WFFlags.ProgramPath);
		//Print(w);
		//Wnd w = Wnd.Find("qm message", prop: new Wnd.WinProp() { childName = "one" });
		//Wnd w = Wnd.Find("qm message", also: e => e.Child("two") != Wnd0);
		//Wnd w = Wnd.Find("qm message", also: e => e.HasChild("two"));
		//Wnd w = Wnd.Misc.AllWindows

		var s = "Help - Sandcastle Help File Builder";
		//Wildex x = "**c|*sandcastle*";
		//Print(x.Match(s));
		//Print(s.Like_("*sandcastle*"));
		//return;

		//var w = Wnd.Find("**c|*Sandcastle*", null, "SAND*");
		//var w = Wnd.Find("**t|" + s, null, "SAND*");
		var w = Wnd.Find(@"**r|\bsandcastle\b.+$", null, "SAND*");
		Print(w);
		if(w.Is0) return;
		Wnd c;
		//c = w.ChildByClass("*COMBO*");
		//Print(c);
		c = w.ChildById(67022);
		Print(c);
		c = w.Child("*chm*", "*static*");
		//c = w.Child(null, "*combo*");
		Print(c);

		//w = Wnd.Find(prop: new Wnd.WinProp() { childName = "Open *" });
		//w = Wnd.Find(prop: new Wnd.WinProp() { childClass = "QM_Code" });
		Print(w);
		Print("----");
		//Print(Wnd.ThreadWindows(w.ThreadId, "**m|**n|tooltips_class32[]**nc|*IME*"));
		//Print(Wnd.ThreadWindows(w.ThreadId, "**mn|tooltips_class32[]**c|*IME*"));

		//Print(Process_.GetProcessesByName("explorer"));
		//Print(Process_.GetProcessesByName(new Wildex("[m]notepad[]explorer[]qm")));
		//Print(Process_.GetProcessesByName(Wildex.OptimizedCreate("[m]notepad[]explorer[]qm")));

		//Print(TaskDialog.ShowInputEx("aa"));

		//Print(w);
		//w.Activate();

		//Print(Wnd.AllWindows("[m]*o*[][nP][", true));
		//Print(Wnd.
		//String_.Join(",", "", "");
		Print("fin");
	}

	static void Nooook()
	{
		if(true) {
			Print(1);
		}
	}

#if tuples
	//static (int, int) TestReturnTuple()
	static (int a, int b) TestReturnTuple()
	{
		//var r = (3, 4);
		//var r = (c: 3, d: 4);
		(int a, int b) r = (3, 4);
		r = (5, 6);
		return r;
		return (1, 2);
	}
#endif

	static void TestCSharpSeven()
	{


#if pattern_matching
		object o = "text";
		if(o is string s) Print(s);

		switch(o) {
		case string k:
			Print(k);
			break;
		}
#endif

#if nested_functions
		int i = 5;
		Print(NestedFunc(5));

		int NestedFunc(int c) { return c * c + i; }
#endif

#if tuples
		//var t = TestReturnTuple();
		////PrintList(t.Item1, t.Item2);
		//PrintList(t.a, t.b);

		var (a, b) = TestReturnTuple();
		PrintList(a, b);

		//var v = Catkeys.Util.AppDomain_.GetDefaultDomain();
#endif

#if binary_literals_and_digit_separators
		int i = 0b10_1;
		Print(i);
#endif

	}

	static void TestTaskDialog2()
	{
		//if(TaskDialog.Show("Text", null, TDButtons.OKCancel, TDIcon.Info) != TDResult.OK) return;
		Print("ok");

		//Wnd w = Wnd.Find("");
		//int i = 123;
		//Print(i);
		//Regex rx;

		//switch(TaskDialog.Show("Save changes?", "More info.", customButtons: "1 Save|2 Don't Save|Cancel")) {
		//case 1: Print("save"); break;
		//case 2: Print("don't"); break;
		//default: Print("cancel"); break;
		//}


		//switch(BaskDialog.Show("Save changes?|More info.", "1 Save|2 Don't Save|3 Cancel")) {
		//switch(BaskDialog.Show("Save changes?|More info.", "Yes|No|Cancel")) {
		////switch(BaskDialog.Show("Save changes?|More info.", BaskDialog.YesNoCancel)) {
		//case 1: Print("save"); break;
		//case 2: Print("don't"); break;
		//default: Print("cancel"); break;
		//}
		//switch(BaskDialog.Show("Save changes?", "info", 0, "1 Save|2 Don't Save|Cancel")) {
		//case 1: Print("save"); break;
		//case 2: Print("don't"); break;
		//default: Print("cancel"); break;
		//}

		//TaskDialog.Show("text", icon: TDIcon.App);

		//switch(TaskDialog.Show("Save changes?",	"info", customButtons:"1 Save|2 Don't Save|Cancel")) {
		//case 1: Print("save"); break;
		//case 2: Print("don't"); break;
		//default: Print("cancel"); break;
		//}

		//TaskDialog.Show("test", null, TDButtons.OKCancel | TDButtons.YesNo | TDButtons.Retry | TDButtons.Close);
		////TaskDialog.Show("test", customButtons:"1 Gerai|2 Kartoti");

		//switch(TaskDialog.Show("test", null, TDButtons.OKCancel)) {
		//case TDResult.OK: break;
		//}

		//switch(TaskDialog.Show("test", null, "1 OK|2 Cancel")) {
		//case 1: break;
		//}

		//DebugDialog("aaa");

		//switch(TaskDialog.Show("Header|Comment", "1 OK|2 Cancel")) {
		//switch(TaskDialog.Show("Header|Comment", "1 O|2 C|3 X")) {
		////switch(TaskDialog.Show("Header|Comment", "-1 OK|-2 Cancel")) {
		//case 1: Print(1); break;
		//case 2: Print(2); break;
		//}
		//TDButtons.

		//if(!TaskDialog.Debug("text", TDButtons.OKCancel)) return;
		//Print("OK");

		//TaskDialog.ShowInputEx("Big text.", "Small text bbbbhhhh gggg ccc.");
		//TaskDialog.ShowInputEx("Big text.");
		//TaskDialog.ShowInputEx("Big text.", editType:TDEdit.Multiline);
		//TaskDialog.ShowInputEx("Big text.", "Small jjj.", editType:TDEdit.Multiline);

		//string s;
		//if(!TaskDialog.ShowInput(out s, "Example")) return;
		//Print(s);

		//int i;
		//if(!TaskDialog.ShowInput(out i, "Example.")) return;
		//Print(i);

		//var r = TaskDialog.ShowInputEx("Header.", "Comments.", checkBox: "Check");
		//if(r.Button != TDResult.OK) return;
		//PrintList(r.EditText, r.IsChecked);


		//var d = new OpenFileDialog();
		//if(d.ShowDialog() != DialogResult.OK) return;
		//Print(d.FileName);

		//var d = new FolderBrowserDialog();
		//if(d.ShowDialog() != DialogResult.OK) return;
		//Print(d.SelectedPath);

		//var d = new TaskDialog();
		//d.ShowDialog();

		//var ad = AppDomain.CreateDomain("test");
		//ad.ExecuteAssembly(@"Q:\app\Catkeys\Test Projects\Test\Test.exe");

		//TaskDialog.Options.DefaultTitle = "DEFAULT";
		//TaskDialog.Options.RtlLayout = true;
		//TaskDialog.Options.ScreenIfNoOwner = 2;
		//TaskDialog.Options.TopmostIfNoOwnerWindow = true;
		//TaskDialog.Options.UseAppIcon = true;
		//TaskDialog.Show("text");

		//TaskDialog.Show("", "TODO: start to drag panel only when mouse moved outside its caption. Move tab button without undocking.", 0, TDIcon.Info, TDFlags.Wider);
		//TaskDialog.Show("", "WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW123456789 EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR");

		//TaskDialog.ShowNoWaitEx(null, "Text.", backgroundThread:true);
		//TaskDialog.ShowNoWaitEx(null, "Text.");

		//TaskDialog.Show("one");
		//Print(TaskDialog.Show("Text1", "Text2", TDButtons.OKCancel, TDIcon.Info, TDFlags.CommandLinks|TDFlags.OwnerCenter, TDResult.Cancel, "1 one|2 two", Wnd.FindFast("Notepad")));
		//Print(TaskDialog.ShowEx("Text1", "Text2", TDButtons.OKCancel, TDIcon.Info, TDFlags.CommandLinks, 0,
		//	"1 one|2 two", "11 rone|12 rtwo", "check|checked", "expanded", "x|footer", "TITLE", null, 100, -10, 10));
		//TaskDialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: ed => { Print(ed.LinkHref); });

		//string s; int i;

		//var d = new TaskDialog("", "Aby", TDButtons.OKCancel, TDIcon.Info, flags: TDFlags.CommandLinks, radioButtons: "One", customButtons: "One", expandedText: "Exp");
		//d.SetExpandedText()
		//d.FlagShowProgressBar = true;
		//d.ShowDialog();

		//if(Util.LibDebug_.IsScrollLock) Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Combo, expandedText: "one\ntwo"));
		//else Print(TaskDialog.ShowInputEx("test", "Aby"));

		//TaskDialog.Options.RtlLayout = true;
		//Print(TaskDialog.ShowInputEx("test", "Aby"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Multiline, TDFlags.Wider));
		//Print(TaskDialog.ShowInputEx("test", "Aby", style:TDFlags.CommandLinks, customButtons:"One\r\ntwo"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Combo, expandedText:"one\ntwo", radioButtons:"10One|11Two"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Multiline, expandedText: "one\ntwo"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Multiline, radioButtons:"10One|11Two"));
		//Print(TaskDialog.ShowInputEx("test", "Aby", TDEdit.Multiline, customButtons:"One\r\ntwo", style:TDFlags.CommandLinks, expandedText:"one\ntwo"));
		//TaskDialog.ShowInputEx("test", "Aby", TDEdit.Combo, expandedText:"exp", footerText:"foo", onButtonClick: e =>
		// {
		//	 if(e.Button == TDResult.OK) {
		//		 e.DoNotCloseDialog = true;
		//		 //e.dialog.Send.ChangeText1("Header", true);
		//		  e.dialog.Send.ChangeText2("new\ntext\netc\netc", true);
		//		 //e.dialog.Send.ChangeExpandedText("AAA\nBBB", true);
		//		 //e.dialog.Send.ChangeFooterText("new\ntext", true);

		//		 //e.dialog.SetText("Header", "text");
		//		 //e.dialog.SetEditControl(TDEdit.None);
		//		 //e.dialog.SetEditControl(TDEdit.Text);
		//		 //e.dialog.Send.Reconstruct();
		//	 }
		// });
		//Print(s);
		//Print(TaskDialog.ShowInputEx("test", "one\nzero\none\ntwo", TDEdit.Combo, checkBox:"check", expandedText:"expanded", footerText:"footer",
		//	x:-1,y:-100, timeoutS: 15, customButtons:"1one|2two"));
		//Print(TaskDialog.ShowInputEx("test", customButtons:"1one|2two", onButtonClick: e => { if(e.Button == 1) { e.DoNotCloseDialog = true; Print(e.EditText); e.EditText="nnnnnnnnnnnnnnn"; } }));

		//Print(TaskDialog.ShowList("1one|2two|3three|Cancel", "Infooooo"));
		//Print(TaskDialog.ShowListEx("1one|2two|3three|Cancel", "Infooooo", checkBox:"Check", x:-10, timeoutS:10));


		//Print(Api.GetCurrentThreadId());
		////TaskDialog.ShowNoWait(e => { Print(Api.GetCurrentThreadId()); }, "Tesxt");

		//var f = new Form();
		//var c = new Button(); c.Text = "test";
		//f.Controls.Add(c);
		//f.Click += (unu, sed) =>
		//  {
		//	  //TaskDialog.Show("one", ownerWindow:f);
		//	  TaskDialog.ShowNoWait(e => { Print(Api.GetCurrentThreadId()); }, true, "Tesxt", ownerWindow: c);
		//	  //TaskDialog.ShowNoWait(e => { Print(Api.GetCurrentThreadId()); }, false, "Tesxt", ownerWindow: f);
		//	  //TaskDialog.ShowNoWait(null, true, "Tesxt", ownerWindow: f);
		//	  //TaskDialog.ShowNoWait(null, true, "Tesxt");

		//	  //var t = new Thread(() =>
		//	  //{
		//	  //  var ff = new Form();
		//	  //	ff.ShowDialog(f);
		//	  //});
		//	  //t.SetApartmentState(ApartmentState.STA);
		//	  //t.Start();
		//  };
		//f.ShowDialog();



		//try {
		////Print(TaskDialog.Show("Text1", "Text2", TDButtons.OKCancel, TDIcon.Info, TDFlags.CommandLinks));

		//}catch(Exception e) { Print(e); }

		//TaskDialog.Show("one");

		//int i;
		//if(!TaskDialog.ShowInput(out i, "Text1", 5, TDEdit.Text)) return;
		//Print(i);

		//string s;
		//if(!TaskDialog.ShowInput(out s, "Text2")) return;
		//PrintList(s, s!=null);

		//string s;
		//if(!TaskDialog.ShowInput(out s, "Text2", editType: TDEdit.Multiline)) return;
		////TODO: need resizable
		//PrintList(s, s != null);

		//Print(Application.StartupPath);
		//Print(Application.UserAppDataPath);

	}


#if true

#endif

	[DllImport("user32.dll")]
	public static extern int MessageBoxW(Wnd hWnd, string lpText, string lpCaption, uint uType);

	static void TestTaskDialog3()
	{


		//var d = new TaskDialog("dddd");
		////d.SetIcon(TDIcon.Info);
		////var ic =Icons.GetFileIconHandle(@"q:\app\macro.ico", 16);
		//var ic = Icon.ExtractAssociatedIcon(@"q:\app\macro.ico");
		//Print(ic);
		//d.SetIcon(ic);
		//d.ShowDialog();
		////Icons.DestroyIconHandle(ic);


		//TaskDialog.Show("Owned", owner:Wnd.Find(null, "Notepad"));

		//var f = new Form();
		//f.Click += (unu, sed) => TaskDialog.Show("Owned", owner: f);
		//f.ShowDialog();

		//Time.SetTimer(2000, true, t => MessageBox.Show("kkkk"));
		//TaskDialog.Options.AutoOwnerWindow = true;
		//Time.SetTimer(2000, true, t => TaskDialog.Show("kkkk"));
		//var f = new Form();
		//f.ShowDialog();

		//TaskDialog d = new TaskDialog("aa", "bb", "1 One|4 Four");

		//d.Created += e => { e.dialog.Send.EnableButton(4, false); };

		//d.ShowDialog();

		//Print("fin");
		//TaskDialog.Show("Simple");

		//Time.SetTimer(50, false, t => GC.Collect());

		//Print(TaskDialog.Show("Simple"));
		//Print(TaskDialog.Show("Simple", "", "Yes|No"));
		//Print(TaskDialog.Show("Simple", "", "-1 One|-2 Two"));
		//TaskDialog.Options.UseAppIcon = true;
		//Print(TaskDialog.Show("Simple", "", "A123456789123456789|BBBBBBBBBBB|CCCCCCCCCCCCC|DDDDDDDDDDDD|EEEEEEEEEEEEE|FFFFFFFFFFFFFFF"));

		//Print(TaskDialog.ShowList("One|Two|Three|100 Four\r\naa\rbb\ncc"));
		//Print(TaskDialog.ShowList(5));
		//Print(TaskDialog.ShowList("One|Two|Three|100 Four", "Simple"));
		//var a = new string[100]; for(int i = 0; i < a.Length; i++) a[i] = (i+1).ToString();
		//Print(TaskDialog.ShowList(a, "Simple"));

		//Print(TaskDialog.ShowOKCancel("Text1"));
		//Print(TaskDialog.ShowOKCancel("Text1", "Text2.", TDIcon.Info, TDFlags.Wider));
		//Print(TaskDialog.ShowYesNo("Text1", "Text2.", TDIcon.Info, TDFlags.Wider));
		//TaskDialog.ShowInfo("Text1", "Text2.", TDFlags.Wider);
		//TaskDialog.ShowWarning("Text1", "Text2.", TDFlags.Wider);
		//TaskDialog.ShowError("Text1", "Text2.", TDFlags.Wider);

		//Print(TaskDialog.ShowInputEx("Text1", "Text2."));
		//Print(TaskDialog.ShowInput(out string s, "Simple", "aaaa", TDEdit.Combo, new string[] { "text", "A", "B", "C" }));
		//Print(s);


#if false
mes "downloaded"
mes "Display settings will be changed after you restart computer." "Display Settings" "i"
mes F"Func1 result={r}" "QM function" "i"
mes var
mes F"Failed to install {localfile}." title "x"
mes F"{localfile} successfully downloaded{ai}." title "i"
mes F"Failed to download {localfile}." title "x"
mes- "failed to create file"
err+ mes _error.description "Error" "x"
mes _error.description "Error" "x"
mes "Too many controls for this page" "Error" "x"
mes "QM cannot create controls of this class." "Error" "x"
mes "Failed to set styles." "Dialog Editor" "!"
mes "This control class is not supported." "" "!"
mes "Macro changed." "" "x"
mes "Need a window." "" "i"
mes "Failed to register hotkey Shift+F11."
mes "failed to update the grid, please retry"
mes _s "" "i" (_s is looong info text")
mes F"{sWarn}[][]You see this warning because 'Use acc' is checked in Options of this dialog." "" "!"
mes "Need a window." "" "i"
mes "Window not selected."
mes "Text empty."
mes _error.description
mes "Not found."
mes F"This 'background' option will not be applied for this window on this computer.{k}" "" "i"
mes "Failed"
mes "Failed"
mes "This dialog creates basic code to call function ListDialog. The function also allows you to set dialog position, size, owner window and item images. Read ListBox help and edit the inserted code." "" "i"
mes F"Error: {_error.description}"
mes "To add or remove a dialog (or an action from a dialog) to the Favorites toolbar, at first open the dialog and select the action." "" "i"
mes "Already exists. Use different name."
mes F"Failed. {_error.description}" "" "!"
mes F"Failed. {_error.description}" "" "!"
mes F"Failed. {_error.description}" "" "!"
mes "The icon already exists in the imagelist." "" "!"
mes "Failed.[][]The bitmap must contain 1 or more images horizontally, each of equal width and height. Image size must be 8-256, divisible by 8. Colors must be 4, 8, 24 or 32 bit." "" "!"
mes F"Failed. {_error.description}" "" "!"
mes F"Failed. {_error.description}" "" "!"
mes F"Failed. {_error.description}" "" "!"
mes "Please select 1 image." "" "!"
mes _error.description
mes _error.description "Error" "x"
mes _error.description "Error" "x"
mes "QM must run as administrator." "" "x"
mes "Failed" "" "x"
mes "QM must run as administrator." "" "!"
mes "Failed" "" "!"
mes sErr
mes "The event procedure must have sel wParam."
mes s "" "i"
mes "Portable QM warning: This will make changes in registry or file system." "" "!"
mes "Invalid class name."
mes F"A QM item '{_s}' already exists."
mes "Invalid base class name."
mes F"Invalid function name {vn}"
sub_sys.MsgBox t_hDlg _error.description "Regular expression error" "i"
sub_sys.MsgBox hEdit _error.description "" "i"
sub_sys.MsgBox hDlg F"Action '{s}' will be selected whenever you open this dialog." "" "i"
sub_sys.MsgBox hDlg F"{statement}[][]{st}" "Object not found" "!"
sub_sys.MsgBox hDlg statement "Element not found" "!"
sub_sys.MsgBox _hwndqm "Failed to save."
sub_sys.MsgBox hDlg iif(s.len s "Not found.") "" "i"
sub_sys.MsgBox hDlg statement "Not found" "!"
sub_sys.MsgBox hDlg "This macro is read-only." "" "!"
MsgBoxAsync "Used variables or incorrect expressions." "Cannot test" "x"


OKCancel
if(mes(F"Component not found. Download now?[][]Description: {description}[]Required file: {localfile}[]Download from: {url}[]File size: {filesizeKB} KB." title "OC?")!='O') ret
if(mes("Save the dialog in current macro?" "Dialog Editor" "OC?")!='O')
sel mes(F"{_error.description}[][]Failed to update dialog in current macro. Possibly it is read-only. Create new macro for it?" "Error" "OCx")
if(mes("This will search for controls reference.[]In the list of results:[]click 'Control Library (Windows)',[]click a control link,[]scroll to Styles." "" "OCi")!='O')
if(mes("This will search for controls reference.[]In the list of results:[]click 'Control Library (Windows)',[]click a control link,[]scroll to Notifications." "" "OCi")!='O')
if('O'=mes("Select an icon in the Icons dialog. OK will open the dialog.[][]Other ways to add icons:[]Drag and drop files.[]Copy/paste files.[]Copy/paste list of file paths.[][]Icons are added to the end, except when pasting. Use Cut/Paste to reorder." "" "OCi"))
if('O'!=mes("Are you sure?" "" "!OC"))
if('O'!=mes("Are you sure?" "" "!OC"))
if(mes("Open the topic, then click OK." "" "OCn")!='O')
if(mes("Save the menu in current macro?" "Menu Editor" "OC?")!='O')
sel mes(F"{_error.description}[][]Failed to update menu in current macro. Possibly it is read-only. Create new macro for it?" "Error" "OCx")
if(mes("Save the menu in current macro?" "Menu Editor" "OC?")!='O')
mes(F"{_error.description}[][]Failed to update menu in current macro. Possibly it is read-only. Create new macro for it?" "Error" "OCx")
if('O'!mes(F"Restores default key actions for {s}." "" "OCi"))

YesNo
if(mes("You can find more functions in the forum. You can request to create a function for you.[][]Go to the forum?" "" "YNi")='Y')
if(sub_sys.MsgBox(hDlg "Are you sure?[][]Note: These email account settings are shared with Outlook Express (Windows XP)." "" "YN!")!='Y')

YesNoCancel
sel mes("Do you want to save changes?" "Dialog Editor" "YNC!")
sel mes("Save changes?" "" "YNC?")
sel(sub_sys.MsgBox(_hwndqm F"{s}[][]This file already exists. Delete?[][]Click No to make unique filename." "" "YNC!"))

Hyperlink
i=mes(F"<>Image not found.{_s}" "Test - not found" "!")
i=mes(F"<>{_error.description}{_s}" "Test - error" "!")






#endif
	}

	//TODO: maybe .NET has such types. Look in System namespace, eg where is Tuple<T>, Func<T> etc.


	//static void Example1(Types<string, IEnumerable<string>> param)
	//{
	//	switch(param.obj) {
	//	case string s: PrintList("string", s); break;
	//	case IEnumerable<string> a: PrintList("IEnumerable<string>", a); break;
	//	case null: Print("null"); break;
	//		//default: throw new ArgumentException(); break; //don't need this, it's impossible
	//	}
	//}

	//static void Example2(string requiredParam, Types<int, double>? optionalParam = null)
	//{
	//	switch(optionalParam?.obj) {
	//	case null: Print("null"); break;
	//	case int i: PrintList("int", i); break;
	//	case double d: PrintList("double", d); break;
	//		//default: throw new ArgumentException(); break; //don't need this, it's impossible
	//	}
	//}

	//static void TestTypes(Types<string, IEnumerable<string>> x)
	//{
	//	switch(x.obj) {
	//	case string s: PrintList("string", s); break;
	//	case IEnumerable<string> a: PrintList("IEnumerable<string>", a); break;
	//	case null: Print("null"); break;
	//	default: throw new ArgumentException(); break; //don't need this, it's impossible
	//	}
	//}

	//static void TestTypes2(Types<int, double>? x = null)
	//{
	//	switch(x?.obj) {
	//	case null: Print("null"); break;
	//	case int i: PrintList("int", i); break;
	//	case double d: PrintList("double", d); break;
	//	default: throw new ArgumentException(); break; //don't need this, it's impossible
	//	}
	//}

	static void TestValueTypes(Types<int, double> x)
	{
		switch(x.type) {
		case 0: Print("none"); break;
		case 1: PrintList("int", x.v1); break;
		case 2: PrintList("double", x.v2); break;
		default: throw new ArgumentException(); break; //don't need this, it's impossible
		}
	}

	static void TestValueTypesRef(Types<string, IEnumerable<string>> x)
	{
		switch(x.type) {
		case 0: Print("none"); break;
		case 1: PrintList("string", x.v1); break;
		case 2: PrintList("IEnumerable<string>", x.v2); break;
		default: throw new ArgumentException(); break; //don't need this, it's impossible
		}
	}

	static void TestValueTypesNullable(Types<int, double>? x)
	{
		switch(x?.type) {
		case null: Print("null"); break;
		case 0: Print("none"); break;
		case 1: PrintList("int", x?.v1); break;
		case 2: PrintList("double", x?.v2); break;
		default: throw new ArgumentException(); break; //don't need this, it's impossible
		}
	}

	static void TestValueTypesRefNullable(Types<string, IEnumerable<string>>? x)
	{
		switch(x?.type) {
		case null: Print("null"); break;
		case 0: Print("none"); break;
		case 1: PrintList("string", x?.v1); break;
		case 2: PrintList("IEnumerable<string>", x?.v2); break;
		default: throw new ArgumentException(); break; //don't need this, it's impossible
		}
	}




	#region speed

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static bool Speed1(Types<string, IEnumerable<string>> x)
	//{
	//	switch(x.obj) {
	//	case string s: return s is null;
	//	case IEnumerable<string> a: return a is null;
	//	default: return false;
	//	}
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static bool Speed2(Types<int, double> x)
	//{
	//	switch(x.obj) {
	//	case int s: return s == 0;
	//	case double a: return a == 0.0;
	//	default: return false;
	//	}
	//}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static bool Speed3(Types<int, double> x)
	{
		switch(x.type) {
		case 1: return x.v1 == 0;
		case 2: return x.v2 == 0.0;
		default: return false;
		}
	}

	static void TestMultiTypeParamSpeed()
	{
		int r = 0;
		string s = "kkkk";
		var a = new string[] { "A", "B" };
		int m = 7;
		double d = 3.4;

		for(int i = 0; i < 5; i++) {
			Perf.First();
			//for(int j = 0; j < 1000; j++) {
			//	if(Speed1(s)) r++;
			//	if(Speed1(a)) r++;
			//}
			//Perf.Next();
			//for(int j = 0; j < 1000; j++) {
			//	if(Speed2(m)) r++;
			//	if(Speed2(d)) r++;
			//}
			//Perf.Next();
			for(int j = 0; j < 1000; j++) {
				if(Speed3(m)) r++;
				if(Speed3(d)) r++;
			}
			Perf.NW();
		}

		Print(r);
	}

	#endregion

	static void TestMultiTypeParam()
	{
		//TestExamples();
		return;

		//TestTypes("S");
		//TestTypes(new string[] { "S0", "S1" });
		//TestTypes(new List<string>() { "L0", "L1" });
		////TestTypes(null);
		////TestMultiTypeParam(5);
		//TestTypes((string)null);
		//TestTypes(default(Types<string, IEnumerable<string>>));
		//Print("----");

		//TestTypes2(3);
		//TestTypes2(3.5);
		//TestTypes2(null);
		//TestTypes2();
		////int? n = 7;
		////TestTypes2(n);
		//Print("----");

		TestValueTypes(7);
		TestValueTypes(7.5);
		TestValueTypes(default(Types<int, double>));
		Print("----");

		TestValueTypesRef("S");
		TestValueTypesRef(new string[] { "S0", "S1" });
		TestValueTypesRef(new List<string>() { "L0", "L1" });
		//TestValueTypesRef(null);
		//TestValueTypesRef(5);
		TestValueTypesRef((string)null);
		TestValueTypesRef(default(Types<string, IEnumerable<string>>));
		Print("----");

		TestValueTypesNullable(7);
		TestValueTypesNullable(7.5);
		TestValueTypesNullable(default(Types<int, double>));
		TestValueTypesNullable(null);
		Print("----");

		TestValueTypesRefNullable("S");
		TestValueTypesRefNullable(new string[] { "S0", "S1" });
		TestValueTypesRefNullable(new List<string>() { "L0", "L1" });
		TestValueTypesRefNullable(null);
		//TestValueTypesRefNullable(5);
		TestValueTypesRefNullable((string)null);
		TestValueTypesRefNullable(default(Types<string, IEnumerable<string>>));
	}

	static void TypesFunc(Types<int, double> a1, Types<int, double>? a2, Types<string, int> a3, Types<string, int>? a4)
	{
		PrintList(a1.type, a1.v1, a1.v2);
		var t2 = a2.GetValueOrDefault();
		PrintList(t2.type, t2.v1, t2.v2);
		PrintList(a3.type, a3.v1, a3.v2);
		var t4 = a4.GetValueOrDefault();
		PrintList(t4.type, t4.v1, t4.v2);
	}

	static void TestTypesAgain()
	{
		//TypesFunc(1, null, "S", null);
		//TypesFunc(1, 2.5, "S", 4);
		//TypesFunc(1, 2.5, "S", "K");
		//TypesFunc(null, 2.5, "S", "K"); //error
		//TypesFunc(1, 2.5, null, "K"); //error
	}

	static void TestPrintIEnumerable()
	{
		var a = new string[] { "a", "b" };
		IEnumerable<string> e = a;
		Print(a);
		Print(e);
	}

	static void TestSystemTimePeriod()
	{
		_Test("before");
		using(new Time.SystemWaitPrecision(2)) {
			_Test("in");
		}
		_Test("after");

		void _Test(string name)
		{
			Print(name);
			Perf.First();
			for(int i = 0; i < 10; i++) { Thread.Sleep(1); Perf.Next(); }
			Perf.Write();
			//DebugDialog(Perf.Times);
		}

		//using(new Time.SystemWaitPrecision(5)) {
		//	int i = 0;
		//	Time.SetTimer(10, false, t => { Perf.Next(); if(++i > 10) { Perf.Write(); t.Stop(); } });
		//	TaskDialog.Show("");
		//}
	}

	static bool TestExc2()
	{
		//return true || throw new Win32Exception(5);

		return false ? true : throw new Win32Exception(5);

		//if(!true) throw new Win32Exception(5); return true;
	}

	static void TestNewExceptions()
	{
		//Print(Native.GetErrorMessage(2));

		//if(!TestExc()) Print(ThreadError.ErrorText);

		//TestExc2();

	}

	static void TestNewRegistry()
	{
		using(var k = Registry_.Open("invalid/key")) {
			Print(k);

		}
	}

	static void TestWndException()
	{
		//throw new NullReferenceException();
		//throw new WndException();
		//throw new WndException(3);
		//throw new WndException("Custom string");
		//throw new WndException(3, "Custom string");
		//throw new WndException(0, "Custom string");
	}

	public static void Move(Types<int, float>? x, Types<int, float>? y, Types<int, float>? width, Types<int, float>? height, bool workArea = false)
	{
		Print("Move Types<int, float>?");
	}

	public static void Move(int? x, int? y, int? width, int? height, uint swpFlagsToAdd = 0)
	{
		Print("Move int?");
	}

	public static void Move(int x, int y, int width, int height, uint swpFlagsToAdd = 0)
	{
		Print("Move int");
	}

	static void TestMoveOverloads()
	{
		Move(1, 2, 3, 4);
		Move(1, null, 3, 4);
		Move(1, 2.5f, 3, 4);

		Move(null, null, 3, null);
		Move(null, 2.5f, null, null);
	}

	static void TestWndPropErrors()
	{
		//var w = Wnd.Find("Quick*");
		var w = Wnd.Find("* Notepad");
		Native.ClearError();
		//var a = w.PropList();
		//var p = w.GetProp("asd");
		//var p = w.GetProp("qmshex");
		//w.SetProp("sett", 5);
		w.PropRemove("sett");
		PrintList(Native.GetError(), Native.GetErrorMessage());
		//Print(p);
		Print(w.PropGet("sett"));
		//Print(a.Count);
		//Print(a);
	}

	static void TestWinErrorMessageAndExceptions()
	{
		//for(int i = 1; i < 10000; i++) {
		//	var s = Native.GetErrorMessage(i);
		//	if(s.StartsWith_("Unknown error (0x") || s.Length >500) continue;
		//	Print(i);
		//	Print(s);
		//}

		Wnd w = Wnd0;
		Native.ClearError();
		//if(!w.IsChildOf(Wnd0)) Print(Native.GetError());
		//var a = w.PropList();
		//if(a.Count==0) Print(Native.GetError());

		try {
			bool b = w.IsEnabled;
			if(Native.GetError() != 0) {
				//throw new CatException();
				//throw new CatException("Messssage");
				//throw new CatException(5);
				//throw new CatException(5, "Messssage");
				throw new CatException("Messsssage", new FileNotFoundException());
			}
		}
		catch(CatException e) {
			PrintList(e.NativeErrorCode, e.Message);
		}
	}

	static void TestWndAccessDenied()
	{
		Wnd w = Wnd.Find("Registry *");
		//Wnd c = w.Child("", "Edit");
		//Print(c);
		//c.FocusControl();
		w.SetTransparency(true, 50);

		//Wnd w = Wnd.Find("Quick*");
		//Print(w);
		//Wnd c = w.ChildById(2216);
		//Print(c);
		//Native.ClearError();
		//string s = c.ControlText;
		//if(s == null) Print(Native.GetErrorMessage());
		//else Print(s);

		//Wnd w = Wnd.Find("*YouTube Downl*");
		//Print(w);
		//Wnd c = w.ChildByClass("*SysListview32*");
		//Print(c);
		//Native.ClearError();
		//string s = c.ControlNameWinForms;
		//if(s == null) Print(Native.GetErrorMessage());
		//else Print(s);

		//Wnd w = Wnd.Find("*YouTube Downl*");
		//Print(w);
		//Wnd c = w.Child("**wfName:olvDownloads");
		//Print(c);

	}


	//int thisLen = t.Length, endof = index + length;
	//if(index < 0 || endof < index || endof > thisLen) throw new ArgumentException();
	//if(Empty(value)) return t;
	//var s = new StringBuilder(t);
	//s.

	static void TestStringReplaceAt()
	{
		var s = "/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>";
		//var s = "/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>/// <remarks>Supports <see cref='Native.GetError'/>.</remarks>";
		string s2 = null;

		Print(s.ReplaceAt_(27, 4, "HREF_ETC"));

		//int i = s.Length / 2;
		//var a1 = new Action(() => { s2 = s.ReplaceAt_(i, 4, "HREF_ETC"); });
		//var a2 = new Action(() => { s2 = s.ReplaceAt_2(i, 4, "HREF_ETC"); });
		//var a3 = new Action(() => { s2 = s.ReplaceAt_3(i, 4, "HREF_ETC"); });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(8, 1000, a1, a2, a3, a4);

	}

	static LPARAM WndProc33(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		Wnd.Misc.PrintMsg(w, msg, wParam, lParam);
		return Api.DefWindowProc(w, msg, wParam, lParam);
	}

	static void TestCreateWindow()
	{
		//var wc = Wnd.Misc.WindowClass.Register("TTTTest", WndProc33);
		//Wnd w = Wnd.Misc.CreateWindow(0, wc.Name, "Name", Native.WS_POPUPWINDOW | Native.WS_VISIBLE|Native.WS_CAPTION, 500, 100, 300, 200);

		Wnd.Misc.WindowClass.InterDomainRegister("TTTTest", WndProc33);
		Wnd w = Wnd.Misc.WindowClass.InterDomainCreateWindow(0, "TTTTest", "Name", Native.WS_POPUPWINDOW | Native.WS_VISIBLE | Native.WS_CAPTION, 500, 100, 300, 200);

		var ic = Icons.GetFileIconHandle(@"q:\app\paste.ico", 16);
		Wnd.Misc.SetIconHandle(w, ic);
		Wnd c = Wnd.Misc.CreateWindowAndSetFont(0, "Edit", "Text", Native.WS_CHILD | Native.WS_VISIBLE, 10, 10, 100, 20, w, 3);
		TaskDialog.ShowInput(out string s, "td", editText: "TaskDialog");
		Wnd.Misc.DestroyWindow(w);
		Icons.DestroyIconHandle(ic);
	}

	static void MinimizeAll()
	{
		var a = Wnd.Misc.MainWindows();
		for(int i = 0; i < a.Count; i++) {
			//for(int i=a.Count-1; i>=0; i--) {
			a[i].ShowMinimized(true);
		}
	}

	static void SendAltTab()
	{
		var k = new Api.INPUTKEY(Api.VK_MENU, 0);
		Api.SendInputKey(ref k);
		k.wVk = Api.VK_TAB;
		Api.SendInputKey(ref k);
		k.dwFlags = Api.IKFlag.Up;
		Api.SendInputKey(ref k);
		k.wVk = Api.VK_MENU;
		Api.SendInputKey(ref k);
	}

	static void TestSwitchActiveWindow()
	{
		Print(Wnd.Misc.SwitchActiveWindow());
		return;

		//Wnd w = Wnd.Find("Microsoft Excel - Book1");
		//Wnd w = Wnd.Find("id.xls");
		//Wnd w = Wnd.Find("Quick M*");
		//Wnd w = Wnd.Find("Registry*");
		//Wnd w = Wnd.Find("* Notepad");
		//Wnd w = Wnd.Find("*Dream*");
		//Wnd w = Wnd.Find("*Help Viewer*");
		//Wnd w = Wnd.Find("*Visual Studio");
		//Wnd w = Wnd.Find("*Firefox*", "MozillaWindowClass");
		//Wnd w = Wnd.Find("* Microsoft Document Explorer");
		//Print(w);
		//if(w.Is0) { Print("not found"); return; }
		////w.ShowNotMinimized(true); return;
		//Print("-----");
		//w.Activate(); Thread.Sleep(500);
		////return;

		////Wnd.Misc.AllowActivate();

		////Print(Wnd.MainWindows());
		////return;

		//Wnd.ShowDesktop();
		////Wnd.ShowDesktop(true);
		//////Thread.Sleep(2000);
		//////Wnd.ShowDesktop(false);
		////MinimizeAll();
		//w.ShowMinimized();

		Thread.Sleep(500);
		Print(Wnd.Misc.MainWindows());
		//return;

		//Print(Wnd.ActiveWindow);
		//Thread.Sleep(500);
		//Print(Wnd.ActiveWindow);
		////return;
		//Thread.Sleep(1000);

		//Print("-----");
		//Print(Wnd.MainWindows());
		//Print("-----");
		////return;

		Print(Wnd.Misc.SwitchActiveWindow());
		//SendAltTab();
		Thread.Sleep(500);
		Print(Wnd.WndActive);
	}

	static void TestStringEqualsPart()
	{
		string s = "**id:text";
		Print(s.EqualsPart_(2, "id:"));
		Print(s.EqualsPart_(2, "ID:", true));
		Print(s.EqualsPart_(2, false, "id:", "text:", "wfName:"));
	}

	static void TestChildAcc()
	{
		Wnd w = Wnd.Find("FileZilla");
		Wnd c = w.Child("Quickconnect");
		Print(c);
		Print(c.NameAcc);
	}

	static void TestWndNextMainWindow()
	{
		int f = TaskDialog.ShowList("1 default|2 allDesktops|3 likeAltTab|4 retryFromTop|5 skipMinimized");
		if(f == 0) return;
		Wnd w = Wnd.Misc.WndTop;
		int n = 0;
		while(!w.Is0) {
			Print(w);
			switch(f) {
			case 1:
				w = w.WndNextMain();
				break;
			case 2:
				w = w.WndNextMain(allDesktops: true);
				break;
			case 3:
				w = w.WndNextMain(likeAltTab: true);
				break;
			case 4:
				w = w.WndNextMain(retryFromTop: true); if(++n > 20) return;
				break;
			case 5:
				w = w.WndNextMain(skipMinimized: true);
				break;
			}
		}
	}

	static unsafe void TestBytesToHexString()
	{
		byte[] a = new byte[4] { 65, 66, 67, 68 };
		var s = Calc.BytesToHexString(a, true);
		Print(s);
		Print(Calc.BytesFromHexString(s));
		fixed (byte* p = a) {
			var s2 = Calc.BytesToHexString(p, 4, true);
			Print(s2);
			int a2;
			Print(Calc.BytesFromHexString(s2, &a2, 4));
			PrintHex(a2);
		}
	}

	class FormB :Form
	{
		protected override void WndProc(ref Message m)
		{
			if((uint)m.Msg == Api.WM_APP) {
				PrintList("WM_APP", m.WParam);
			}
			base.WndProc(ref m);
		}
	}

	[DllImport("winmm.dll")]
	internal static extern uint timeBeginPeriod(uint uPeriod);
	[DllImport("winmm.dll")]
	internal static extern uint timeEndPeriod(uint uPeriod);

	static void WaitEx(int timeMS)
	{
		int sysTimer = 0;
		if(timeMS > 0) {
			if(timeMS < 15) sysTimer = timeMS;
			else if(timeMS > 16 && timeMS < 30) sysTimer = (timeMS + 1) / 2;
			else if(timeMS == 16) timeMS = 15;
		}

		PrintList(timeMS, sysTimer);
	}

	static void TestWndAgain()
	{


		//var w = Wnd.Find("FileZilla");
		//var w = Wnd.Find("Find");
		//w.AllChildren(false, true).ForEach(c => { Print(c); /*c.Show(false);*/ c.Focus(); Wait(0.5); });
		//w.Child(null, "SysTreeView32", skip: 1).Focus();
		//w.Child("Find Next").Focus();

		//Wnd.Misc.WaitForAnActiveWindow();
		//Wnd w = Wnd.Find("* Notepad");
		//w.Activate(); Wait(0.5);
		////w.Close(true);
		//w.Send(Api.WM_CLOSE);
		////Wnd.Misc.WndRoot.Activate();
		//Perf.First();
		//var yes=Wnd.Misc.WaitForAnActiveWindow();
		//Perf.NW();
		//Print(yes);



		//Wnd.FindAll("* Notepad", "Notepad").ForEach(t => { t.Activate(); Print(t.Close()); Print(Wnd.WndActive); });
		//Wnd.FindAll("* Visual Studio*", "wndclass_desked_gsk").ForEach(t => { t.Activate(); Print(t.Close()); Print(Wnd.WndActive); });
		//Wnd.FindAll("* Visual Studio*", "wndclass_desked_gsk").ForEach(t => { t.Activate(); Print(t.Close(useXButton: true)); Print(Wnd.WndActive); });
		//Wnd.FindAll("* Firefox").ForEach(t => { Print(t); t.Activate(); Print(t.Close()); Print(Wnd.WndActive); });
		//Wnd.FindAll("* Thunderbird").ForEach(t => { t.Activate(); Print(t.Close()); Print(Wnd.WndActive); });
		//Wnd.FindAll(className:"QM_toolbar").ForEach(t => { Print(t.Close()); });
		//Wnd.Misc.MainWindows(likeAltTab:true).ForEach(t => {
		//	if(0!=t.Name.Like_(false, "Catkeys -*", "Quick Macros -*", "Process Expl*")) return;
		//	Print(t);
		//	t.Activate();
		//	long t1 = Time.Milliseconds;
		//	Print(t.Close());
		//	//Print(t.Close(useXButton: true));
		//	long t2 = Time.Milliseconds-t1;
		//	if(t2 > 300) Print(t2);
		//	Print(Wnd.WndActive);
		//	Print("---");
		//});
		//Wnd.Misc.MainWindows().ForEach(t => { Print(t); t.Activate(); Wait(0.5); });

		//Wnd.Find("*something");
		//Wnd.Find("*something", flags: WFFlags.HiddenToo);
		//Wnd.Find("*something");

		//Perf.First();
		//Wnd w;
		//////w=(Wnd)328956;
		//////w=(Wnd)263214;
		//////w=Wnd.Find("id.xls");
		////w=Wnd.Find("*Excel*");
		//////w = Wnd.Find("* Notepad");
		//////w = Wnd.Find("Quick Ma*");
		//////w = Wnd.Find("*Inno *");
		//w = Wnd.Find("PicPick");
		////Perf.Next();
		////w.Activate();
		//bool ok=w.Close();
		//Perf.NW();
		//Print(ok);



		//Wnd w = Wnd.Find("SQLite E*");
		//Wnd w = Wnd.Find("* Inno Setup*");
		//Print(w);
		//w.Activate();

		//var c = Wnd.Misc.WindowClass.InterDomainRegister("ffoo", null);
		////Wnd w = Wnd.Misc.CreateMessageWindow("#32770");
		//Wnd w = Wnd.Misc.CreateMessageWindow("ffoo");
		//Print(w);
		//w.Close();
		////Wnd.Misc.DestroyWindow(w);
		////w.Close(true);MessageBox.Show("fff");
		//Print(w);

		//for(int i = 1; i < 50; i++) WaitEx(i);
		//return;

		//using(new Time.SystemWaitPrecision(2)) {
		//	Perf.First();
		//	for(int i = 1; i < 17; i++) {
		//		//timeBeginPeriod(5);
		//		////Thread.Sleep(1);
		//		//timeEndPeriod(5);
		//		//Perf.Next();
		//		//timeBeginPeriod(2);
		//		////Thread.Sleep(1);
		//		//timeEndPeriod(2);
		//		//Perf.Next();

		//		//Thread.Sleep(i);
		//		Thread.Sleep(2);
		//		Perf.Next((char)('0' + i % 10));
		//	}
		//	Perf.Write();

		//}

		//var f = new FormB();
		//f.Text = "FindMe";
		//f.Deactivate += (unu, sed) => Print("deact");
		////f.ShowDialog();return;
		//f.Show();

		////Thread.Sleep(5000);
		//Thread.CurrentThread.Join(5000);


		//TaskDialog.Show("");
		//return;

		//Wnd.Misc.WndRoot.ActivateLL();
		//Print(Wnd.WndActive);

		//var w1 = Wnd.Find("*- Notepad");
		//var w2 = Wnd.Find("*Excel*");

		//Perf.First();
		//for(int i = 0; i < 3; i++) {
		//	//w1.Activate();
		//	Perf.Next();
		//	Thread.Sleep(10);
		//	//w2.Activate();
		//	Perf.Next();
		//}
		////Perf.Write();
		//Trace.WriteLine(Perf.Times);

		//var w = Wnd.Find("* Notepad");
		//var s = w.SavePlacement();
		//Print(s);
		//MessageBox.Show("");
		//w.RestorePlacement(s);


		//Wnd w = Wnd.Find("Dialog");
		//w.Send(Api.WM_APP);

		//Time.SetTimer(500, true, t => { Thread.CurrentThread.Abort(); });
		//Time.SetTimer(500, true, t => { throw new IndexOutOfRangeException(); });
		//Time.SetTimer(500, true, t => { Api.PostQuitMessage(1); });
		//Time.SetTimer(500, false, t => {  });
		//Thread.Sleep(3000);
		//Application.Run();
		//while(Time.DoEvents()) {  }
		//Api.WaitMessage();
		//MessageBox.Show("text", "Tests.exe");
		//new FolderBrowserDialog().ShowDialog();
		//new OpenFileDialog().ShowDialog();
		//new Form().ShowDialog();
		//TaskDialog.Show("1");
		//TaskDialog.Show("2");
		//TaskDialog.ShowEx("2", timeoutS:30);
		//TaskDialog.ShowEx("2", buttons:"OK|Cancel", timeoutS:30);
		//TaskDialogAPI(Wnd0, Zero, "t", "m", null, 0, null, out int b);

		//var d = new TaskDialog("aa", null, "1 Test", timeoutS: 15);
		//d.ButtonClicked += e =>
		//{
		//	e.DoNotCloseDialog = true;
		//	//throw new IndexOutOfRangeException();
		//	Thread.CurrentThread.Abort();
		//};
		//d.ShowDialog();

		//var f = new Form();
		//f.Click += (unu, sed) =>
		//  {
		//	  //throw new IndexOutOfRangeException();
		//	  Thread.CurrentThread.Abort();
		//  };
		//f.ShowDialog();

		//Print("fin");

		//Wnd w = Wnd.Find("*Notepad", flags:WFFlags.HiddenToo);
		////Print(w.IsEnabled);
		////w.Close();
		////Wait(5);
		//w.Show(true);

		//Wnd w = Wnd.Find("*Notepad");
		//w.ZorderTopmost();
		//w.MoveToScreenCenter();
		//w.MoveToScreenCenter(2);
		//w.MoveToScreenCenter(Screen_.Primary);
		//w.MoveToScreenCenter(Screen_.OfMouse);
		//w.MoveToScreenCenter(Screen.PrimaryScreen);
		//w.MoveToScreenCenter(new POINT(1, 10000));
		//w.MoveToScreenCenter(new Rectangle(300, 1000, 100, 10000));
		//w.MoveToScreenCenter(Wnd.WndActive);
		//w.MoveToScreenCenter(Wnd.FindFast(null, "QM_Editor"));

		//var r = new RECT(1000, 2000, 500, 500, true);
		//Print(r);
		//r.EnsureInScreen();
		//Print(r);
		//r.EnsureInScreen(1);
		//Print(r);


		//if(TaskDialog.ShowInput(out string s)) Print(s);
		//var d = new TaskDialog(text2: "small");
		//Print(d.ShowDialog());

		//Wnd w = Wnd.FindFast("Options", "#32770");
		//Print(w);
		//Wnd w1 = Wnd0, w2 = Wnd0, w3 = Wnd0, w4 = Wnd0;

		//var a1 = new Action(() => { w1 = Wnd.Find(null, "WordPadClass"); });
		//var a2 = new Action(() => { w2 = Wnd.FindFast(null, "WordPadClass"); });
		//var a3 = new Action(() => { w3 = w.Child("Apply", "Button"); });
		//var a4 = new Action(() => { w4 = w.ChildFast("&Apply", "Button"); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		//Print(w1);
		//Print(w2);
		//Print(w3);
		//Print(w4);

		//Wnd.CloseAll("* Notepad", "Notepad");
		//foreach(var v in Wnd.FindAll("* Notepad", "Notepad")) v.Close();
		//close all Notepad windows
		//Wnd.FindAll("* Notepad", "Notepad").ForEach(t => t.Close());


		//var f = new Form();
		//f.Click += (unu, sed) =>
		//{
		//	//((Wnd)f).ShowMinimized();
		//	f.Wnd_().ShowMinimized();
		//};
		//f.ShowDialog();

		//return;

		////Wnd w = Wnd.Find("Dialog");
		////w = w.ChildById(1);
		////Print(c.ClassName);
		////Print(c.Name);
		////Print(c.ControlText);
		//////return;
		////Wait(0.2);

		//Wnd w = Wnd.Find("Options");
		//w = w.Child("Unicode");

		//PrintList("self", w);
		//PrintList("parent", w.WndDirectParent);
		//PrintList("par or own", w.WndDirectParentOrOwner);
		//PrintList("owner", w.WndOwner);
		//PrintList("first child", w.WndFirstChild);
		//PrintList("last child", w.WndLastChild);
		//PrintList("child 2", w.WndChild(2));
		//PrintList("first sib", w.WndFirstSibling);
		//PrintList("last sib", w.WndLastSibling);
		//PrintList("next", w.WndNext);
		//PrintList("prev", w.WndPrev);
		//PrintList("window", w.WndWindow);

		//Print("----");
		////Print(Wnd.Find("Options").Child("Layout*").WndNext);
		//Print(Wnd.Find("Options").WndFirstChild.WndChild(3));

		//return;

		//var a = Wnd.Misc.AllWindows(true);
		//int i, n = a.Count;
		//for(i = 0; i < n; i++) {
		//	var b = a[i].AllChildren();
		//	a.AddRange(b);
		//}
		//Print(a.Count); //1924

		//string s = null;
		//Perf.First();
		//for(i = 0; i < 5; i++) {
		//	foreach(var v in a) {
		//		//s = v.ClassName;
		//		//s = v.Name;
		//		s = v.ControlText;
		//	}
		//	Perf.Next();
		//}
		//Perf.Write();

		//Print(s);

		//foreach(var v in a) {
		//	var name = v.Name;
		//	var text = v.ControlText;
		//	Wnd.Misc.StringRemoveUnderlineAmpersand(ref text);

		//	//if(name == null) Print($"name null: {v}"); //0
		//	//if(text == null) Print($"text null: {v}"); //1: Windows.UI.Core.CoreWindow "Settings"

		//	//if(Empty(name) && Empty(text)) continue;

		//	//if(Empty(name)) Print($"----\n{v}\n{text}"); //almost all: Edit, Combo, StatusBar

		//	//if(Empty(name) && 0==v.ClassNameIs("*Edit*", "*ComboBox*", "*StatusBar*", "*Memo*")) Print($"----\n{v}\n{text}"); //1: VMwareStaticLink

		//	//if(Empty(text)) Print($"----\n{v}\n{name}"); //6 (3 classes)

		//	//if(text == name) PrintList(v, name);

		//	//if(text != name && !(Empty(name) || Empty(text))) Print($"----\n{v}\n{name}\n{text}"); //5 (3 classes)

		//	//if(Empty(name)) {
		//	//	var v2 = v.WndPrev;
		//	//	if(v2.Is0) {
		//	//		v2 = v.WndDirectParent;
		//	//		if(v2)
		//	//	}
		//	//}
		//}



		//Wnd w = Wnd.Find("FileZilla");
		//Wnd c = w.ChildById(-31804);
		//Wnd w = Wnd.Find("Dialog");
		//Wnd c = w.ChildById(3);
		//Print(c.ControlText);
		//Wait(0.2);
		//string s = "";

		//var a1 = new Action(() => { s = c.ControlText; });
		//var a2 = new Action(() => { });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 100, a1, a2, a3, a4);

		//Print(s);

		//Wnd w=Wnd.Find(a, b, c, d, e); if(!w.Is0) Print(w);

		//var p = new Wnd.FindParams(a, b, c, d, e); if(p.Find()) Print(p.Result);

		//WFFlags

		//Wnd w = Wnd.Find("FileZilla");
		//Wnd c;
		//c = w.Child("Port:");
		//Print(c);
		//c = w.Child(null, "Button");
		//Print(c);
		//c = w.Child("**id:-31782");
		//Print(c);
		//c = w.Child("777");
		//Print(c);
		//c = w.Child("**text:777");
		//Print(c);
		//c = w.Child("**text:Port:");
		//Print(c);
		//c = w.Child("**accName:Port:");
		//Print(c);

		//w = Wnd.Find("Free YouTube*");
		//c = w.Child("**wfName:olvDownloads");
		//Print(c);

		//w = Wnd.Find("Options");
		//c = w.Child("On error");
		//Print(c);
		//c = w.Child("**text:On error");
		//Print(c);
		//c = w.Child("Administrator");
		//Print(c);
		//c = w.Child("**text:Administrator");
		//Print(c);

		//c = w.Child("**id:15");
		//c = w.Child("**text:control text");
		//c = w.Child("**wfName:myControl");
		//c = w.Child("**accName:acc name");

		//c = w.ChildByClass("Button");
		//Print(c);
		//c = w.ChildById(-31930);
		//Print(c);
		//c = w.Child("-31930", flags: WCFlags.NameIsId);
		//Print(c);
		//Thread.Sleep(200);

		//var a1 = new Action(() => { c = w.Child(null, "Button"); });
		//var a2 = new Action(() => { c = w.ChildByClass("Button"); });
		//var a3 = new Action(() => { c = w.ChildById(-31930); });
		//var a4 = new Action(() => { c = w.Child("-31930", flags: WCFlags.NameIsId); });
		//Perf.ExecuteMulti(5, 100, a1, a2, a3, a4);


		//Wnd w = Wnd.Find("* Notepad");
		//Print(w);
		//w.SetProp("test", 1);
		//w = Wnd.Find(also: t => t.GetProp("test") != 0);
		//Print(w);


		//Wnd w = Wnd.Find(also: t => t.ContainsScreenXY(0.3f, 1.2f));
		//Print(w);

		//Wnd w = Wnd.Find("Options");
		//Print(w);
		//Wnd c = w.Child(null, "Button", also: t => t.ContainsWindowXY(257, 216));
		////Wnd c = w.Child(null, "Button", also: t => t.ContainsParentXY(0.7f, 0.3f));
		//Print(c);

		//Wait(0.2);

		//var a1 = new Action(() => { c = w.Child(null, "Button", also: t => t.ContainsWindowXY(w, 257, 216)); });
		//var a2 = new Action(() => { c = w.Child(null, "Button", also: t => t.ContainsWindowXY(257, 216)); });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);


		//PrintList(Process_.CurrentProcessHandle, Process_.CurrentThreadHandle, Process_.CurrentProcessId, Process_.CurrentThreadId);

		//Wnd w = Wnd0;
		//Perf.First();
		//for(int i = 0; i < 5; i++) {
		//	//w = Wnd.Find("* notepad", "notepad");
		//	w = Wnd.Find(programEtc: "notepad");
		//	//w = Wnd.Find(programEtc: "*\\notepad.exe", flags: WFFlags.ProgramPath);
		//	//w = Wnd.Find(programEtc: "notepag");
		//	//w = Wnd.Find(programEtc: "*\\notepag.exe", flags: WFFlags.ProgramPath);
		//	Perf.Next();
		//}
		//Perf.Write();
		//Print(w);

		//return;

		//Wnd w = Wnd.Find(prop: new Wnd.WinProp() { childName = "Open items" });

		//var c = new Wnd.ChildParams("Open items");
		//Wnd w = Wnd.Find(also: t => c.Find(t));


		////Wnd w = Wnd.Find(prop: new Wnd.WinProp() { styleHas = Native.WS_CAPTION });

		////Wnd w = Wnd.Find(also: t => t.HasStyle(Native.WS_CAPTION));


		//Print(w);
		//Print(c.Result);


	}

	static void TestLnkShortcutExceptions()
	{
		//var x = Files.LnkShortcut.Open(@"q:\test\test.lnk");
		var x = Files.LnkShortcut.Create(@"q:\test\test.lnk");
		//x.TargetPath = @"c:\windows";
		x.TargetPath = "|?:*gg";
		//x.TargetURL = "|?:*gg";
		//x.TargetURL = "http://www.quickmacros.com";
		//x.TargetIDList = Marshal.AllocCoTaskMem(100);
		//x.TargetAnyType = "|?:*gg";
		x.Save();
	}

	[DllImport("kernel32.dll")]
	internal static extern void Sleep(int dwMilliseconds);
	[DllImport("kernel32.dll")]
	internal static extern uint SleepEx(int dwMilliseconds, bool bAlertable);
	[DllImport("kernel32.dll")]
	internal static extern uint WaitForMultipleObjectsEx(uint nCount, [In] IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds, bool bAlertable);
	[DllImport("kernel32.dll")]
	internal static extern uint WaitForMultipleObjectsEx(uint nCount, ref IntPtr lpHandle, bool bWaitAll, uint dwMilliseconds, bool bAlertable);

	static void SpinMCS(int timeMCS)
	{
		var t = Time.Microseconds;
		while(Time.Microseconds - t < timeMCS) { }
	}

	static void TestAbortThreadAndWaitFunctions()
	{
		//var h = Api.CreateEvent(Zero, false, false, null);
		//using(new Time.SystemWaitPrecision(1)) {
		//	Perf.First();
		//	for(int i = 0; i < 5; i++) {
		//		//SpinMCS(15000);
		//		//Perf.Next();
		//		Thread.Sleep(1);
		//		//Thread.CurrentThread.Join(1);
		//		//Time.SleepDoEvents(1);
		//		//Api.MsgWaitForMultipleObjectsEx(0, null, 15, 0);
		//		//Api.WaitForSingleObject(h, 15);
		//		//WaitForMultipleObjectsEx(1, ref h, false, 15, false);
		//		//Sleep(1);
		//		Perf.Next();
		//	}
		//	Perf.Write();
		//}
		//Api.CloseHandle(h);
		//return;

		Catkeys.Util.AppDomain_.Exit += (unu, ded) => { Print("exit"); };

		new Thread(o =>
		{
			Thread.Sleep(1002);
			Print("abort");
			var t = o as Thread;
			Perf.First();
			//t.Abort();
			//t.Interrupt();
			//t.Suspend();
			//Wait(10);
			//t.Resume();
			//TODO: test whether .NET wait functions support APC
			//Environment.Exit(1);
			//var a=Wnd.Misc.ThreadWindows(t.)
			//var w = Wnd.Find("Tests.exe");
			//Print(w);
			//w.CloseLL();
			//w.Close(false);
		}).Start(Thread.CurrentThread);

		//Time.SetTimer(1000, true, t => { Print("timer"); });

		try {
			//Print(Api.MsgWaitForMultipleObjectsEx())
			Wait(5);
			//Print(Time.WaitNoBlockMS(5000));
			//Sleep(5000);
			//SleepEx(5000, true);
			//for(int i = 0; i < 50; i++) Sleep(100);
			//Thread.CurrentThread.Join(5000);

			//using(var e = new EventWaitHandle(false, EventResetMode.AutoReset)) {
			//	e.WaitOne(5000);
			//}

			//Task.Delay(5000);

			//var m=new Catkeys.Util.MessageLoop();
			//Time.SetTimer(1000, true, t => { Print("timer"); m.Stop(); });
			//m.Loop();

			Print("after sleep");
		}
		catch(ThreadInterruptedException e) { Print("interrupted"); }
		finally {
			Perf.NW();
			Print("finally");
		}

		Print("return");
	}

	static LPARAM TestHookProc(int code, LPARAM wParam, LPARAM lParam)
	{
		Print(wParam);
		return Api.CallNextHookEx(Zero, code, wParam, lParam);
	}
	static Api.HOOKPROC s_testHookProc = TestHookProc;

	static void TestHooks()
	{
		var hh = Api.SetWindowsHookEx(Api.WH_KEYBOARD_LL, s_testHookProc, Zero, 0);
		//TaskDialog.Show();
		//Thread.Sleep(10000);
		Thread.CurrentThread.Join(10000); //OK
		Api.UnhookWindowsHookEx(hh);
	}

	[DllImport("kernel32.dll")]
	internal static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
	internal const uint THREAD_ALL_ACCESS = 0x1FFFFF;
	[DllImport("kernel32.dll")]
	internal static extern uint QueueUserAPC(PAPCFUNC pfnAPC, IntPtr hThread, LPARAM dwData);
	internal delegate void PAPCFUNC(LPARAM Parameter);
	internal const uint WAIT_IO_COMPLETION = 0xC0;

	static void TestApcProc(LPARAM Parameter)
	{
		Print(Parameter);
	}
	static PAPCFUNC _testApcProc = TestApcProc;

	static void TestWait()
	{
		//var f = new Form();
		//f.Click += async (unu, sed) =>
		//  {
		//	  //Print(WaitFor.Window(0, "* Notepad"));
		//	  Print(await WaitFor.WindowAsync(0, "* Notepad"));
		//  };
		////f.Click += F_Click;
		//f.ShowDialog();

		//async void F_Click(object sender, EventArgs e)
		//{
		//	Print(await WaitFor.WindowAsync(0, "* Notepad"));
		//}

		//Time.SetTimer(1000, true, t =>
		//{
		//	Print("begin");
		//	for(int i = 0; i < 1000; i++) {
		//		//Application.DoEvents();
		//		Time.DoEvents();
		//		Thread.Sleep(10);
		//	}
		//	Print("end");
		//}
		//);
		//var f = new Form();
		////var b = new Button();
		////b.KeyUp
		////b.MouseUp
		//f.Click
		//	+= (unu, sed) =>
		//  {
		//	  Print("begin");
		//	  //for(int i = 0; i < 1000; i++) {
		//	  // //Application.DoEvents();
		//	  // if(!Time.DoEvents()) break;
		//	  // Thread.Sleep(10);
		//	  //}
		//	  //Time.SleepDoEvents(10000);
		//	  Application.Exit();

		//	  //await Task.Delay(3000);
		//	  //await Task.WhenAll

		//	  Print("end");
		//  };
		//f.KeyUp += (unu, sed) => Print("key");
		////f.Controls.Add(b);
		//f.ShowDialog();
		////f.Show();
		////var m = new Native.MSG();
		////while(Api.GetMessage(out m, Wnd0, 0, 0) > 0) {
		////	Wnd.Misc.PrintMsg(ref m);
		////	Api.TranslateMessage(ref m);
		////	Api.DispatchMessage(ref m);
		////}
		////Wnd.Misc.PrintMsg(ref m);

		//Print("exit");
		//return;

		//var mt = Thread.CurrentThread;
		//var th = OpenThread(THREAD_ALL_ACCESS, false, Api.GetCurrentThreadId());
		////Print(th);
		//new Thread(o =>
		//{
		//	for(int i = 0; i < 3; i++) {
		//		Wait(1);
		//		QueueUserAPC(_testApcProc, th, i);
		//	}

		//	//Wait(1);
		//	//mt.Suspend();
		//	//Wait(7);
		//	//mt.Resume();

		//}).Start();
		//Perf.First();
		////while(SleepEx(7000, true) == WAIT_IO_COMPLETION) { Print("WAIT_IO_COMPLETION"); }
		////Thread.Sleep(5000);
		//Time.SleepDoEvents(5000);

		////Thread.CurrentThread.Join(7000);
		//Api.CloseHandle(th);
		//Perf.NW();

		//Print("end");

		//var t1 = Time.Milliseconds;
		//Thread.Sleep(1000 * 30);
		//Print(Time.Milliseconds - t1);

		//var a = new List<int>(100);
		//using(new Time.SystemWaitPrecision(1)) {
		//	for(int i = 1; i < 50; i++) {
		//		var t1 = Time.Milliseconds;
		//		Thread.Sleep(i);
		//		var t2 = Time.Milliseconds;
		//		a.Add((int)(t2 - t1));
		//	}
		//}
		//Print(a);
	}

	static void TestWaitFor()
	{
		//Print(WaitFor.WindowClosed(-5, Wnd.Find("* Notepad")));
		Print(WaitFor.WindowNotExists(0, "* Notepad"));
		Print("ok");

		//Wnd w = WaitFor.WindowExists(10, "* Notepad");
		//Print(w);

		//var f = new Form();
		//f.Click += async (unu, sed) =>
		//  {
		//	  Print("waiting for Notepad...");
		//	  Wnd w = await Task.Run(() => WaitFor.WindowExists(-10, "* Notepad"));
		//	  if(w.Is0) Print("timeout"); else Print(w);
		//  };
		//f.ShowDialog();
	}

	//private static async void F_Click(object sender, EventArgs e)
	//{
	//	Print(await WaitFor.WindowAsync(0, "* Notepad"));
	//}

	[DllImport("comctl32.dll", EntryPoint = "#344", PreserveSig = true)]
	internal static extern int TaskDialogAPI(Wnd hwndOwner, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, int dwCommonButtons, string pszIcon, out int pnButton);

	public static async Task<TDResult> ShowAsync(string text1)
	{
		var td = new TaskDialog(text1);
		return await Task.Run(() => td.ShowDialog());
	}

	static void TestTaskDialogAsyncAwait()
	{
		//var pd = TaskDialog.ShowProgress(false, "Working");
		//for(int i = 1; i <= 100; i++) {
		//	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		//	pd.Send.Progress(i); //don't need this if marquee
		//	Thread.Sleep(50); //do something in the loop
		//}
		//pd.Send.Close();

		var td = TaskDialog.ShowNoWaitEx("Another example", "text", "1 OK|2 Cancel", y: -1, timeoutS: 30);
		Print(td.DialogWindow);
		Wait(2); //do something while the dialog is open
		td.Send.ChangeText2("new text", false);
		td.Send.Close();
		Wait(2); //do something while the dialog is open
		td.ThreadWaitClosed(); Print(td.Result); //wait until the dialog is closed and get result. Optional, just an example.

		//Print(Api.GetCurrentThreadId());
		//var r=await ShowAsync("fff");
		//Print(Api.GetCurrentThreadId());

		//var f = new Form();
		//f.Click


		//Time.SetTimer(1000, false, tt => { });
		//Thread.CurrentThread.Abort();
		//TaskDialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: e =>
		//{
		//	Print(e.LinkHref);
		//	Thread.CurrentThread.Abort();
		//}
		////,timeoutS:30
		//);
		//Print("after dialog");

		//var pd = TaskDialog.ShowProgressEx(false, "Working", buttons: "1 Stop", y: -1);
		//for(int i = 1; i <= 100; i++) {
		//	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		//	pd.Send.Progress(i); //don't need this if marquee
		//	Thread.Sleep(50); //do something in the loop
		//}
		////pd.Send.Close();
		//return;

		//TaskDialog.Show("Simple example");
		//var td=TaskDialog.ShowNoWait(null, "Simple example");
		//var td = TaskDialog.ShowNoWaitEx(null, "Simple example", timeoutS: 3);
		//var td=TaskDialog.ShowNoWait(null, "Simple example");
		//Print(1);
		//td.ThreadWaitClosed();
		//Print(2);
		//Task.Run(() => { Thread.Sleep(10000); });
		//Thread th = null;
		//Task.Run(() =>
		//{
		//	//try {
		//	//th = Thread.CurrentThread;
		//	//Time.SetTimer(300, false, tt => { Print("timer"); });
		//	//MessageBox.Show("dd");
		//	//TaskDialog.Show("dd");
		//	TaskDialog.ShowEx("dd", timeoutS: 15);
		//	//Thread.CurrentThread.Abort();
		//	//Wait(0.1);
		//	//throw new Exception("dd");
		//	//}
		//	//catch(Exception e) { Print(e); }
		//});

		//throw new Exception("dd");
		//new Thread(() =>
		//{
		//	Wait(0.1);
		//	throw new Exception("dd");
		//}
		//).Start();

		//var th = new Thread(() =>
		//  {
		//	  //MessageBox.Show("ff");
		//	  //TaskDialog.Show("ff");
		//	  TaskDialog.ShowEx("dd", timeoutS: 15);
		//  });
		//th.IsBackground = true;
		//th.Start();

		//Print(1);
		//Wait(1);
		//Print(2);
		//th.Abort();
		//Wait(1);
		//Print(3);

		//var td = TaskDialog.ShowNoWaitEx(e => { Print(e); }, "Another example", buttons: "1 OK|2 Cancel", y: -1, timeoutS: 30);
		//Wait(2); //do something while the dialog is open in other thread
		//td.ThreadWaitClosed(); //wait until dialog closed (optional, just an example)

		//return;

		//Task.Run(() => TaskDialog.Show("example"));
		//Task.Run(() => TaskDialog.ShowEx("example", timeoutS: 15));
		//Print(1);
		//Wait(3); //do something in this thread
		//Print(2);

		//var t = Task.Run(() => TaskDialog.ShowEx("example", "<a href=\"example\">link</a>", buttons: "Yes|No|CL\nnn", icon: TDIcon.App, flags: TDFlags.CommandLinks, timeoutS: 15, onLinkClick: e => Print(e.LinkHref)));
		//var t = Task.Run(() => TaskDialogAPI(Wnd0, Zero, "ti", "s1", "s2", 0, null, out int b));
		//Print(1);
		//Wait(3);
		//Print(2);
		//t.Wait();
		//Print(t.Result);


		//var f = new Form();
		////f.Click += (unu, sed) =>
		////{
		////	var r = TaskDialog.Show("sync");
		////	var r = TaskDialog.Show("sync", owner:f);
		////	Print(r);
		////};
		//f.Click += async (unu, sed) =>
		//{
		//	Print(Api.GetCurrentThreadId());
		//	//var r = await Task.Run(() => TaskDialog.Show("async"));
		//	var r = await Task.Run(() => TaskDialog.Show("async", owner: f));
		//	Print(r);
		//	Print(Api.GetCurrentThreadId());
		//};
		//f.ShowDialog();

	}

	public static int LoadFromImage_(this ImageList t, Image pngImage)
	{
		t.ColorDepth = ColorDepth.Depth32Bit;
		int k = pngImage.Height; t.ImageSize = new Size(k, k); //AddStrip throws exception if does not match
		int R = t.Images.AddStrip(pngImage);
		var h = t.Handle; //workaround for the lazy ImageList behavior that causes exception later because the Image is disposed when actually used
		return R;
	}

	//class ImageFromStrip
	//{
	//	Graphics _g, _g2;

	//	public ImageFromStrip(Bitmap b)
	//	{
	//		_g = Graphics.FromImage(b);
	//		int h = b.Height;
	//		var b2 = new Bitmap(h, h);
	//		_g2 = Graphics.FromImage(b2);
	//		b2.LockBits
	//	}

	//	Image GetImage(int i)
	//	{
	//		_g2.DrawImageUnscaled()
	//	}
	//}

	static void TestImagelistSpeed()
	{
		var f = new Form();
		f.Width = 1000;
		f.Click += (unu, sed) =>
		  {
			  var file = @"Q:\app\Catkeys\Editor\Resources\il_tb_24.png";
			  Perf.First();
			  var img = (Bitmap)Image.FromFile(file);
			  int size = img.Height;
			  Perf.Next();
			  var a = new Image[40];
#if true
			  ImageList il = new ImageList();
			  il.LoadFromImage_(img);
			  Perf.Next();
#endif
			  for(int i = 0; i < 40; i++) {
				  int x = i * size;
				  //a[i] = il.Images[i];
				  //a[i] = Icons.HandleToImage(Api.ImageList_GetIcon(il.Handle, i, 0));
				  a[i] = img.Clone(new Rectangle(x, 0, size, size), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			  }
			  Perf.Next();
			  using(var g = f.CreateGraphics()) {
				  g.Clear(Color.White);
				  Perf.Next();
				  for(int i = 0; i < 40; i++) {
					  int x = i * size;
					  //il.Draw(g, x, 0, i);
					  g.DrawImage(a[i], x, 0);
				  }
				  Perf.NW();
			  }
		  };
		f.ShowDialog();
	}

	static void TestImagelistSpeed2()
	{
		var f = new Form();
		f.Width = 1000;
		f.Click += (unu, sed) =>
		  {
			  var files = new string[]
			  {
@"Q:\app\Catkeys\Editor\Resources\TB\WriteBackPartition.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XMLDocumentTypeDefinitionFile.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XMLFile.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XMLSchema.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XMLTransformation.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XnaLogo.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XPath.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XSLTTemplate.png",
@"Q:\app\Catkeys\Editor\Resources\TB\XSLTTransformFile.png",
@"Q:\app\Catkeys\Editor\Resources\TB\ZoomIn.png",
@"Q:\app\Catkeys\Editor\Resources\TB\VSDatasetInternalInfoFile.png",
@"Q:\app\Catkeys\Editor\Resources\TB\VSShell.png",
@"Q:\app\Catkeys\Editor\Resources\TB\VSTAAbout.png",
@"Q:\app\Catkeys\Editor\Resources\TB\VSThemeEditor.png",
@"Q:\app\Catkeys\Editor\Resources\TB\Watch.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WCF.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WCFDataService.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WeakHierarchy.png",
@"Q:\app\Catkeys\Editor\Resources\TB\Web.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebAdmin.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebConfiguration.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebConsole.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebCustomControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebCustomControlASCX.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebMethodAction.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebPart.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebPhone.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebService.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebSetupProject.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebTest.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WebUserControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WeightMember.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WeightMemberFormula.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WF.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WFC.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WFService.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsForm.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsLogo_Cyan.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsService.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsServiceStop.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WindowsServiceWarning.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WinformToolboxControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WMIConnection.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WorkAsSomeoneElse.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WorkflowAssociationForm.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WorkflowInitiationForm.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WorkItemQuery.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFApplication.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFCustomControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFDesigner.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFFlowDocument.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFLibrary.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFPage.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFPage_gray.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFPageFunction.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFResourceDictionary.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFToolboxControl.png",
@"Q:\app\Catkeys\Editor\Resources\TB\WPFUserControl.png"
			  };

			  Perf.First();
			  int size = 24;
			  var a = new Image[40];
			  for(int i = 0; i < 40; i++) {
				  a[i] = Image.FromFile(files[i]);
			  }
			  Perf.Next();
			  using(var g = f.CreateGraphics()) {
				  g.Clear(Color.White);
				  Perf.Next();
				  for(int i = 0; i < 40; i++) {
					  int x = i * size;
					  g.DrawImage(a[i], x, 0);
				  }
				  Perf.NW();
			  }
		  };
		f.ShowDialog();
	}

	static void TestImagelistSpeed3()
	{
		var f = new Form();
		f.Width = 1000;
		f.Click += (unu, sed) =>
		  {
			  var files = new string[]
			  {
@"WriteBackPartition",
@"XMLDocumentTypeDefinitionFile",
@"XMLFile",
@"XMLSchema",
@"XMLTransformation",
@"XnaLogo",
@"XPath",
@"XSLTTemplate",
@"XSLTTransformFile",
@"ZoomIn",
@"VSDatasetInternalInfoFile",
@"VSShell",
@"VSTAAbout",
@"VSThemeEditor",
@"Watch",
@"WCF",
@"WCFDataService",
@"WeakHierarchy",
@"Web",
@"WebAdmin",
@"WebConfiguration",
@"WebConsole",
@"WebCustomControl",
@"WebCustomControlASCX",
@"WebMethodAction",
@"WebPart",
@"WebPhone",
@"WebService",
@"WebSetupProject",
@"WebTest",
@"WebUserControl",
@"WeightMember",
@"WeightMemberFormula",
@"WF",
@"WFC",
@"WFService",
@"WindowsForm",
@"WindowsLogo_Cyan",
@"WindowsService",
@"WindowsServiceStop",
@"WindowsServiceWarning",
@"WinformToolboxControl",
@"WMIConnection",
@"WorkAsSomeoneElse",
@"WorkflowAssociationForm",
@"WorkflowInitiationForm",
@"WorkItemQuery",
@"WPFApplication",
@"WPFCustomControl",
@"WPFDesigner",
@"WPFFlowDocument",
@"WPFLibrary",
@"WPFPage",
@"WPFPage_gray",
@"WPFPageFunction",
@"WPFResourceDictionary",
@"WPFToolboxControl",
@"WPFUserControl"
			  };

			  Perf.First();
			  //var resMan = new System.Resources.ResourceManager("Resources.Images", Assembly.GetExecutingAssembly());
			  var resMan = Tests.Properties.Resources.ResourceManager;
			  Perf.Next();
			  int size = 24;
			  var a = new Image[40];
			  for(int i = 0; i < 40; i++) {
				  a[i] = (Image)resMan.GetObject(files[i]);
				  if(i < 2) Perf.Next();
			  }
			  Perf.Next();
			  using(var g = f.CreateGraphics()) {
				  g.Clear(Color.White);
				  Perf.Next();
				  for(int i = 0; i < 40; i++) {
					  int x = i * size;
					  g.DrawImage(a[i], x, 0);
				  }
				  Perf.NW();
			  }
		  };
		f.ShowDialog();
	}

	static void TestCreateManyPngFromIcons()
	{
		var a = new string[]
			{
@"Q:\app\new.ico",
@"Q:\app\properties.ico",
@"Q:\app\save.ico",
@"Q:\app\icons\run.ico",
@"Q:\app\icons\compile.ico",
@"Q:\app\deb next.ico",
@"Q:\app\icons\deb into.ico",
@"Q:\app\icons\deb out.ico",
@"Q:\app\deb cursor.ico",
@"Q:\app\deb run.ico",
@"Q:\app\deb end.ico",
@"Q:\app\undo.ico",
@"Q:\app\redo.ico",
@"Q:\app\cut.ico",
@"Q:\app\copy.ico",
@"Q:\app\paste.ico",
@"Q:\app\icons\back.ico",
@"Q:\app\icons\active_items.ico",
@"Q:\app\icons\images.ico",
@"Q:\app\icons\annotations.ico",
@"Q:\app\help.ico",
@"Q:\app\droparrow.ico",
@"Q:\app\icons\record.ico",
@"Q:\app\find.ico",
@"Q:\app\icons\mm.ico",
@"Q:\app\icons\tags.ico",
@"Q:\app\icons\resources.ico",
@"Q:\app\icons\icons.ico",
@"Q:\app\options.ico",
@"Q:\app\icons\output.ico",
@"Q:\app\tip.ico",
@"Q:\app\icons\tip_book.ico",
@"Q:\app\delete.ico",
@"Q:\app\icons\back2.ico",
@"Q:\app\open.ico",
@"Q:\app\icons\floating.ico",
@"Q:\app\icons\clone dialog.ico",
@"Q:\app\dialog.ico",
		   };

		var destDir = @"Q:\app\Catkeys\Editor\Resources\png icons";
		Directory.CreateDirectory(destDir);
		foreach(var s in a) {
			using(var im = Icons.GetFileIconImage(s, 16)) {
				var dest = destDir + "\\" + Path.GetFileNameWithoutExtension(s) + ".png";
				//Print(dest);
				File.Delete(dest);
				im.Save(dest);
			}
		}
	}

	static void TestResizePngIconBigger()
	{
		var f = new Form();
		f.BackColor = Color.White;

		var img = Image.FromFile(@"Q:\Downloads\famfamfam_silk_icons_v013\icons\application_edit.png");
		var t = new ToolStrip();
		t.Items.Add(img);
		t.ImageScalingSize = new Size(32, 32);

		f.Controls.Add(t);

		f.ShowDialog();
	}

	static string g_name = "Output";
	static void TestXDocument()
	{
		Perf.First();
		var x = XElement.Load(@"Q:\app\catkeys\editor\default\Panels.xml");
		Perf.Next();
		//var n =x.Elements().Count();
		var v1 = x.Descendants("panel").FirstOrDefault(el => el.Attribute("name")?.Value == "Output");
		var v2 = x.XPathSelectElement("//panel[@name=\"" + g_name + "\"]");
		Perf.NW();
		//Print(v2);
		Print(x.Descendant_("panel", "name"));
		Print(x.Descendant_("panel", "name", "Find"));

		var a1 = new Action(() => { v1 = x.Descendants("panel").FirstOrDefault(el => el.Attribute("name")?.Value == "Output"); });
		var a2 = new Action(() => { v2 = x.XPathSelectElement("//panel[@name=\"" + g_name + "\"]"); });
		var a3 = new Action(() => { });
		var a4 = new Action(() => { });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	}

	static void TestMenuStripShortcuts()
	{
		var f = new FormMenu();
		//var ms = new MenuStrip();
		//var dd = new ToolStripDropDownMenu();
		//var k=dd.Items.Add("Test", null, (unu, sed) => Print("test")) as ToolStripMenuItem;
		//var ddi = new ToolStripMenuItem("File");
		//ddi.DropDown = dd;
		//ms.Items.Add(ddi);

		//k.ShortcutKeys = Keys.Control | Keys.Z;

		//f.Controls.Add(ms);
		//f.MainMenuStrip = ms;




		Application.Run(f);
	}

	static void TestKeysFromString()
	{
		Keys k;
		string s;
		s = " Ctrl + Shift + Left ";
		//s = "ctrl+shift+left";
		s = "Ctrl+Shift+Left";
		s = "A";
		s = "F12";
		s = ",";
		s = "5";
		s = "z";
		s = "Sleep";
		s = "Ctrl+Alt+Shift+Space";
		s = "  Ctrl  +  D  ";
		s = "Ctrl+Alt+Left";
		s = "End";
		s = "Win+R";
		s = "Ctrl+D";
		s = "5";

		//s = "K+D";
		//s = " C trl + F 2 ";
		//s = "Ctrl+Alt";
		//s = "Ctrl";
		//s = "Ctrl+A+K";
		//s = "A+Ctrl";
		//s = "+A";
		//s = "A+";
		//s = "Ctrl++D";

		Perf.First();
		if(!Input.Misc.ReadHotkeyString(s, out k)) { Print("failed"); return; }
		Perf.NW();
		PrintHex(k);
		Print(k);
		//return;
		Sleep(100);

		var a1 = new Action(() => { Input.Misc.ReadHotkeyString(s, out k); });
		var a2 = new Action(() => { Input.Misc.ReadHotkeyString(" Ctrl+F ", out k); });
		var a3 = new Action(() => { Input.Misc.ReadHotkeyString(" Ctrl + F ", out k); });
		var a4 = new Action(() => { Input.Misc.ReadHotkeyString("", out k); });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);


		//var w = Wnd.Find("* Notepad");
		//w.Activate();
		//w = w.ChildById(15);

		//w.Post(Api.WM_KEYDOWN, (int)Keys.OemCloseBrackets);

		//PrintHex(Keys.Alt);

		//Perf.First();
		//var a =Input.Misc._CreateKeyMap();
		////Perf.NW();

		//////Print(a);
		////Print(a.Count);

		//Perf.First();
		//var k = a["Tab"];
		//Perf.NW();

		//var a1 = new Action(() => { k = a["Ctrl"]; });
		//var a2 = new Action(() => { k = a["Tab"]; });
		//var a3 = new Action(() => { k = a["VolumeUp"]; });
		//var a4 = new Action(() => { k = a[";"]; });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);


		//Print(k);
	}

	static void TestImageSerialize()
	{
		var x = new XElement("test");
		for(int i = 0; i < 5; i++) {
			Perf.First();
			Bitmap b = Icons.GetFileIconImage(@"q:\app\qm.exe", 16);
			Perf.Next();
			//Print(b != null);
			using(var ms = new MemoryStream()) {
				b.Save(ms, ImageFormat.Png);
				Perf.Next();
				var s = Convert.ToBase64String(ms.ToArray());
				Perf.Next();
				//Print(s);
				var d = new XElement("i", s);
				x.Add(d);
			}
			Perf.NW();
		}
		Print(x);

		var f = @"q:\test\image2.png";
		Bitmap bb = null;
		Perf.First();
		foreach(var k in x.Elements("i")) {
			//Print(k.Value);
			using(var ms = new MemoryStream(Convert.FromBase64String(k.Value))) {
				bb = new Bitmap(ms);
			}
			Perf.Next();
		}
		Perf.Write();

		bb.Save(f);
		Process.Start(f);

		//Bitmap bb = new Bitmap(ms);
		//Print(bb != null);
		//var f = @"q:\test\image.png";
		//bb.Save(f);
		//Process.Start(f);
	}

	static void TestCatExceptioNewOverload()
	{
		Wnd w = Wnd.Find("ILSpy");
		Api.SetLastError(10);
		try {
			try {
				throw new CatException(1, "Inner");
			}
			catch(Exception e) {
				//throw new CatException("Outer", e);
				//throw new CatException(5, "Outer", e);
				//throw new WndException(w, "Outer", e);
				throw new WndException(w, 5, "Outer", e);
			}
		}
		catch(CatException e) {
			PrintList(e.NativeErrorCode, e.Message);
		}
	}

	static void TestPathNormalize()
	{
		string s;
		s = @"c:\a\..\..\b.txt";
		s = @"c:\a\b";
		s = @"c:\";
		s = @"c:\a\b..";
		s = @"c:\progrm files\some folder\..\some file.txt";
		s = @"c:\progrm files\some folder\some file.txt";
		s = @"\\?\c:";
		s = @"c:\a\.\b.txt";
		s = @"c:\a\b\.";
		s = @"file.name";
		s = @"c:\a\..";
		s = @"\\?\\\\c:////kk";
		s = @"c:";
		s = @"c:\progra~1";
		s = @"c:\a\b. ";
		s = @"%programfiles%\etc\";
		s = @"c:/progrm files/some folder/some file.txt";
		s = @"\\server\share\a\b";
		s = @"c:\oo~nooo1";
		s = @"c:\a\..\b.txt";
		s = @"\\?\c:\progra~1";
		s = @"\\.\etc";
		s = @"\\?\etc";
		s = @"c:etc";
		s = @"\\?\c:etc";

		//Print(Path.GetFullPath(s));
		var u = Path_.Normalize(s);
		//var u=Path_.Normalize(s, @"c:\def dir/");
		Print(u);
		//Print(Path_.PrefixLongPath(u));
		return;

		Wait(0.2);
		Perf.SpinCPU(100);
		var a1 = new Action(() => { Path_.Normalize(s); });
		var a2 = new Action(() => { Path.GetFullPath(s); });
		var a3 = new Action(() => { });
		var a4 = new Action(() => { });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);
	}

	[DllImport("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError = true)]
	internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, uint dwFlags);

	static void TestFileOp()
	{
		//Print(Path_.ExpandEnvVar("%appdata%\\more"));
		//Print(Path_.IsFullPath("%appdata%\\more"));
		//Print(Files.SearchPath("%appdata%\\Microsoft\\"));
		//Print(Path_.LibGetEnvVar("appdata"));
		//Print(Folders.EnvVar("appdata")+"sub");

		//Environment.SetEnvironmentVariable("test", "%appdata%");
		//Print(Environment.GetEnvironmentVariable("test"));
		//Print(Environment.ExpandEnvironmentVariables("%test%"));
		//Print(Path_.ExpandEnvVar("%test%\\more"));

		//var a1 = new Action(() => { Path_.ExpandEnvVar("%appdata%\\more"); });
		//var a2 = new Action(() => { Path_.ExpandEnvVar("%no%\\more"); });
		//var a3 = new Action(() => { Path_.IsFullPath("%appdata%\\more"); });
		//var a4 = new Action(() => { Path_.IsFullPath("c:\\more"); });
		//var a5 = new Action(() => { Environment.ExpandEnvironmentVariables("%appdata%\\more"); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5);

		//Directory.SetCurrentDirectory(@"q:\test");
		//Print(Files.FileOrDirectory(@"q:\test\test.cs"));
		//Print(Files.FileOrDirectory(@"test.cs"));
		//Print(Files.FileOrDirectory(@"test.cs", true));
		//Print(Files.SearchPath(@"test.cs"));

		//return;

		//if(!Files.GetFileId(@"q:\test", out var k)) throw new CatException(0, "failed");
		////if(!Files.GetFileId(@"q:\test\test.cs", out var k)) throw new CatException(0, "failed");
		////if(!Files.GetFileId(@"q:\test\no.cs", out var k)) throw new CatException(0, "failed");
		//PrintList(k.VolumeSerialNumber, k.FileIndex);

		//if(!Files.GetFileId(@"//Q7c/q/test", out var k2)) throw new CatException(0, "failed");
		//PrintList(k2.VolumeSerialNumber, k2.FileIndex);
		//Print(k2 == k);
		//Print(k.Equals(k2));

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Files.GetFileId(@"q:\test", out var kk); });
		//var a2 = new Action(() => { });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);
		//33 mcs

		//return;

		//Print(Files._IsDestInSrc(@"c:\A\b", @"c:\a"));

		//s = "name.txt.";
		//s = null;
		//s = "name.txt";
		//s = "na\\me.txt";
		//s = " ";
		//s = "CON.txt";
		//Print(Path_.IsInvalidFileName(s));

		//Files.Move(@"Q:\Test", @"Q:\Test\Find", true);
		//Files.Move(@"Q:\Test\test.cs", @"Q:\Test\test2.cs", true);

		Perf.First();
		//Directory.Move(@"q:\test\a", @"q:\test\c");
		//Directory.Move(@"q:\test\a", @"d:\test\a");
		//VB.FileSystem.MoveDirectory(@"q:\test\a", @"d:\test\a", true);
		//VB.FileSystem.MoveDirectory(@"q:\test\a", @"q:\test\a2", true);
		//VB.FileSystem.MoveDirectory(@"q:\test\a", @"q:\test\c", true);
		//VB.FileSystem.CopyDirectory(@"q:\test\a", @"q:\test\c", true);
		//VB.FileSystem.CopyDirectory(@"q:\test\a", @"d:\test\a", true);
		//VB.FileSystem.CopyDirectory(@"d:\test\x", @"d:\test\z", true);

		//Files.Copy2(@"d:\test\x", @"d:\test\z");
		//Files.Copy2(@"\\?\d:\x\", @"c:/test/~");

		//Files.Move(@"d:\test\x", @"d:\test\z", true);
		//Files.Move(@"d:\test\z\sub\z", @"d:\test\z", true);
		//Files.Move(@"q:\test\test2.cs", @"d:\test\test2.cs", true);
		//Files.Move(@"q:\test\z", @"d:\test\zz", true);

		//if(MoveFileEx(@"d:\test\z", @"d:\test\Z", 3)) return; //OK
		//if(MoveFileEx(@"d:\test\x", @"d:\test\z", 3)) return; //Access is denied (if exists)
		//if(MoveFileEx(@"d:\test", @"d:\test\z", 3)) return; //The process cannot access the file because it is being used by another process.
		//if(MoveFileEx(@"d:\test\z", @"d:\test", 3)) return; //Access is denied
		//if(MoveFileEx(@"d:\test\z\sub", @"d:\test", 3)) return; //Access is denied
		//if(MoveFileEx(@"q:\test\z", @"d:\test\z", 2)) return; //Access is denied
		//var ec = Native.GetError();
		//throw new CatException(ec, "*move");

		//Files.Test();
		//Files.Delete(@"d:\no file", true);

		var ifExist = Files.IfExists.Fail;
		ifExist = Files.IfExists.Delete;
		//ifExist = Files.IfExists.RenameExisting;
		ifExist = Files.IfExists.MergeDirectory;

		//Files.Copy(@"q:\test\copy.txt", @"d:\test\copy.txt", ifExist);
		//Files.Copy(@"d:\test\z\copy.txt", @"d:\test\copy.txt", ifExist);
		//Files.Copy(@"q:\test\copy dir", @"d:\test\copy dir", ifExist);
		//Files.Copy(@"d:\test\copy.txt", @"d:\test\COPY.txt", ifExist);
		//Files.Copy(@"d:\test\copy dir", @"d:\test\COPY dir", ifExist);
		//Files.Move(@"d:\test\copy.txt", @"d:\test\COPY.txt", ifExist);
		//Files.Move(@"d:\test\copy dir", @"d:\test\COPY dir", ifExist);
		//Files.Rename(@"d:\test\copy.txt", @"COPY.txt", ifExist);
		//Files.Rename(@"d:\test\copy dir", @"COPY dir", ifExist);
		//Files.Move(@"q:\test\copy.txt", @"d:\test\copy.txt", ifExist);
		//Files.Move(@"q:\test\copy dir", @"d:\test\copy dir", ifExist);
		//Files.Move(@"d:\test\z\copy.txt", @"d:\test\copy.txt", ifExist);
		//Files.Move(@"d:\test\z\copy dir", @"d:\test\copy dir", ifExist);
		//Files.Move(@"d:\test\z\copy dir", @"d:\test\copy dir", ifExist);

		//copying drives
		//Files.CopyTo(@"E:\", @"d:\test\E", ifExist); //fails, it's OK
		//Files.Copy(@"E:\", @"d:\test\E", ifExist);
		//Files.Copy(@"E:\", @"G:\", ifExist);
		//Files.Move(@"E:\", @"G:\", ifExist);

		//if(Files.GetAttributes(@"E:\", out var att)) Print(att);

		Perf.NW();
		//return;


		//string s1, s2;
		//s1 = @"Q:\Test\test.cs";
		//s2 = @"Q:\Test\test2.cs";
		//s1 = @"Q:\Test\Find";
		//s2 = @"Q:\Test\Find2";
		////s1 = @"Q:\Test\test.cs";
		//////s2 = @"Q:\Test\Find\test.cs";
		////s2 = @"Q:\Test\new dir\subdir\test.cs";

		////File.SetAttributes(s1, FileAttributes.ReadOnly);
		//Perf.First();
		//for(int i = 0; i < 2; i++) {
		//	var t1 = ((i & 1) == 0) ? s1 : s2;
		//	var t2 = ((i & 1) == 0) ? s2 : s1;
		//	//File.Move(t1, t2);

		//	//Files.Rename(t1, Path.GetFileName(t2), true);
		//	//Files.Move(t1, t2);
		//	//Files.MoveTo(t1, Path.GetDirectoryName(t2));
		//	Files.Copy(t1, t2, true);


		//	Perf.Next();

		//	break;
		//	if(i == 0) DebugDialog(i);
		//}
		//Perf.Write();
	}

	static void TestFilesDelete()
	{
		//string s1, s2;
		//s1 = @"Q:\Test\test.cs";
		//s2 = @"Q:\Test\test2.cs";
		//for(int i = 0; i < 4; i++) {
		//	File.Copy(s1, s2);
		//	Perf.First();
		//	Files.Delete(s2, false);
		//	Perf.NW();
		//}

		//for(int i = 0; i < 3; i++) {
		//	string s = @"Q:\Test\New Folder";
		//	string s2 = s + "\\sub";
		//	string s3 = s2 + "\\file.txt";
		//	if(true) {
		//		Directory.CreateDirectory(s2);
		//		if(!Files.FileExists(s3)) File.WriteAllText(s3, "aaa");
		//		//File.SetAttributes(s3, FileAttributes.ReadOnly);
		//		//File.SetAttributes(s3, FileAttributes.Hidden);
		//		File.SetAttributes(s2, FileAttributes.Directory | FileAttributes.ReadOnly);
		//		Process.Start(s2);
		//	} else {
		//		Directory.CreateDirectory(s);
		//	}
		//	Wait(2);
		//	//TaskDialog.Show();

		//	Perf.First(100);
		//	Files.Delete(s, false);
		//	Perf.NW();
		//}

		Files.Delete(@"D:\test\mount", false);

		Print("ok");
	}

	static void TestEnumDirectory()
	{
		int n = 0;
		string dir = @"D:\Test";
		//dir = @"C:\Program Files";
		//dir = @"C:\Program Files (x86)";
		dir = @"C:\Windows\System32";
		//dir = @"C:\Windows";
		//dir = @"C:\Windows.old";
		//dir = @"C:\";
		//dir = @"F:\";
		//dir = @"Q:\";
		//dir = @"Q:\app";
		//dir = @"\\?\D:\Test";
		//dir= @"D:\Test\..";
		//dir = @"D:\";
		//dir= @"D:\Test\mount";
		//dir= @"D:\Test\test2.cs";
		//dir = @"\\.\";
		//dir = @"U:\";
		//dir = @"\\WIN-BJHQMCDNJJR\Users\Public\Recorded TV";
		//dir = @"\\WIN-BJHQMCDNJJR\Users";
		//dir = @"\\WIN-BJHQMCDNJJR";
		//dir = @"\\hhhhhh";
		//dir = @"\\WIN-BJHQMCDNJJR\Moooo";
		//dir = @"Q:\<>";
		//dir = @"\\.\Device";
		//dir = @"//Q7c/q/test";
		//dir = @"//Q7c/d$/test";
		//dir = Folders.SystemX86;

		Files.EDFlags fl = 0;
		fl = Files.EDFlags.AndSubdirectories;
		//fl = Files.EDFlags.AndSubdirectories | Files.EDFlags.AndSymbolicLinkSubdirectories;
		//fl |= Files.EDFlags.SkipHidden;
		//fl |= Files.EDFlags.SkipHiddenSystem;
		//fl |= Files.EDFlags.RawPath;
		//fl |= Files.EDFlags.DisableRedirection;

		Perf.SpinCPU(200);
		Perf.First();
		//Files.EnumDirectory(@"D:\Test", fl);
#if false
		//DirectoryInfo.EnumerateFileSystemInfos and other .NET directory enumeration methods are useless.
		//They cannot be used to enumerate subdirectories because:
		//	Throw UnauthorizedAccessException etc if subfolder access denied.
		//	Don't have option 'don't get symlink directory content'.
		//	Not tested, but probably fail if path length > 257.
		//	When testing, failed for most directories in C:\ (Windows, Program Files etc).
		//Also >2 times slower. Most of its code consists of security checks.
		var d = new DirectoryInfo(dir);
		foreach(var v in d.EnumerateFileSystemInfos("*", System.IO.SearchOption.AllDirectories)) {
			Print(v.FullName);
			n++;
		}
#elif true
		int n1 = 0, n2 = 0;
		//Files.EnumDirectory2(dir, fl);
		foreach(var f in Files.EnumDirectory(dir, fl)) {
			//n1 += f.FullPath.Length + 8; n2 += f.Name.Length + 8;
			//if(f.Name.StartsWith_('$')) Print(f.FullPath);
			PrintList(f.FullPath, f.Attributes.ToString("X"), f.Size, f.LastWriteTimeUtc.ToLocalTime(), f.Level);
			//if(f.FullPath.StartsWith_(@"C:\Windows")) f.SkipThisDirectory();
			////else if(f.FullPath.StartsWith_(@"C:\Program Files")) f.SkipThisDirectory();
			n++;
		}
#else
		foreach(var f in Files.EnumDirectory_old(dir, fl)) {
			//Print(f.FullPath);
			//if(f.FullPath.StartsWith_(@"C:\Windows") || f.FullPath.StartsWith_(@"C:\Program Files")) f.SkipThisDirectory();
			//if(!f.FullPath.StartsWith_(@"C:\$Recycle.Bin", true)) f.SkipThisDirectory();
			n++;
		}
#endif
		Perf.NW();
		Print(n);
		//PrintList(n1 / 1024, n2 / 1024);
	}

	[DllImport("shlwapi.dll", EntryPoint = "PathIsDirectoryEmptyW")]
	internal static extern bool PathIsDirectoryEmpty(string pszPath);

	public static bool IsEmptyDirectory(string path)
	{
		return !Files.EnumDirectory(path, Files.EDFlags.FailIfAccessDenied).Any();

		//PathIsDirectoryEmpty faster by several %. But then we don't know whether is not empty or it does not exist or is file.
	}

	static void TestDirectoryIsEmpty()
	{
		var s = @"D:\Test\empty";
		s = @"D:\Test\mount"; //False, True when target missing; False, False when target exists
		s = @"D:\Test\x\sym-link-to-y"; //False, False when target not empty; True, True when target empty
		var s2 = @"D:\Test\x";
		s2 = @"\\?\D:\Test\x";
		s2 = @"D:\Test\protected";
		PrintList(PathIsDirectoryEmpty(s), PathIsDirectoryEmpty(s2));
		PrintList(IsEmptyDirectory(s), IsEmptyDirectory(s2));
		Perf.SpinCPU(100);
		var a1 = new Action(() => { PathIsDirectoryEmpty(s); });
		var a2 = new Action(() => { IsEmptyDirectory(s); });
		var a3 = new Action(() => { PathIsDirectoryEmpty(s2); });
		var a4 = new Action(() => { IsEmptyDirectory(s2); });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	}

	static void TestFileProperties()
	{
		//Files.ExistsAsResult u = 0;
		//if(u > 0) Print(1);
		//if(u < 0) Print(-1);

		//var s = @"E:\setup.exe";
		////var s =@"D:\Test\protected";
		////var s =@"D:\Test\protected\no.txt";
		////var s =@"D:\Test\protected\f.txt";

		////Native.SetError(5);
		////if(Files.FileExists(s)) { Print("exists"); return; }
		////Print(Native.GetErrorMessage());
		//////if(Files.GetAttributes(s, out var attr)) PrintHex(attr); else Print("failed") ;
		////if(Files.GetAttributes(s, out var attr, Files.GAFlags.DoNotThrow)) PrintHex(attr); else Print("failed") ;

		////Print(File.Exists(s));
		////Print(File.GetAttributes(s));

		////Print(Path_.Normalize(@"C:/aaaaaaaaa/bbbbb/../aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
		//return;

		//string s;
		//s = @"D:\Test\protected";
		//s = @"D:\Test\protected\no.txt"; //false
		//s = @"D:\Test\protected\f.txt"; //exception 'access denied'
		//s = @"D:\";
		//s = @"K:\"; //false
		//s = @"D:\<>\mm";
		//Print(Files.GetProperties(s, out var x));
		//PrintList(x.Attributes.ToString("X"), x.Size, x.LastWriteTimeUtc, x.CreationTimeUtc, x.LastAccessTimeUtc);

		foreach(var f in Files.EnumDirectory(@"C:", Files.EDFlags.AndSubdirectories)) {
			string k = f;

			var ea = Files.ExistsAs2(k);
			switch(ea) {
			case Files.ItIs2.File: case Files.ItIs2.Directory: break;
			default:
				PrintList(ea, k);
				break;
			}

			//Print(k); continue;
			//if(Files.GetProperties(k, out var x, Files.GAFlags.DoNotThrow)) {
			//	PrintList(x.Attributes.ToString("X"), x.Size, x.LastWriteTimeUtc, x.CreationTimeUtc, x.LastAccessTimeUtc, k);
			//} else {
			//	var es = Native.GetErrorMessage();
			//	PrintList("FAILED", k, Files.ExistsAsAny(k), f.FullPath.Length, es);
			//}
		}

		Print("end");
	}

	static void TestCalculateDriveSize()
	{
		foreach(var f in Files.EnumDirectory(@"Q:\")) {
			if(!f.IsDirectory) continue;
			PrintList(Files.CalculateDirectorySize(f)/1024/1024, f);
		}
		Print("END");
	}

	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
		Output.Clear();
		Thread.Sleep(100);

		//TODO: InitLibrary. Optional but recommended. Would set error mode, default domain data, etc.

		try {
			TestCalculateDriveSize();
			//TestFileProperties();
			//TestDirectoryIsEmpty();
			//TestEnumDirectory();
			//TestFilesDelete();
			//TestFileOp();
			//TestPathNormalize();
			//TestCatExceptioNewOverload();
			//TestImageSerialize();
			//TestKeysFromString();
			//TestMenuStripShortcuts();
			//TestXDocument();
			//TestResizePngIconBigger();
			//TestCreateManyPngFromIcons();
			//TestImagelistSpeed3();
			//TestRegistry();
			//MessageBox.Show("ff");
			//TestTaskDialogAsyncAwait();
			//TestWaitFor();
			//TestHooks();
			//TestAbortThreadAndWaitFunctions();
			//TestLnkShortcutExceptions();
			//TestWndAgain();
			//TestWndNextMainWindow();
		}
		catch(CatException ex) { PrintList(ex.NativeErrorCode, ex); }
		catch(ArgumentException ex) { PrintList(ex); }
		catch(Win32Exception ex) { PrintList(ex.NativeErrorCode, ex); }
		catch(Exception ex) when(!(ex is ThreadAbortException)) { PrintList(ex.GetType(), ex); }
		//catch(Exception ex) { }
		//Why try/catch creates 3 more threads? Does not do it if there is only catch(Exception ex). Only if STA thread.


		/*

		using static CatAlias;

		say(...); //Output.Write(...); //or print
		key(...); //Input.Keys(...);
		tkey(...); //Input.TextKeys(...); //or txt
		paste(...); //Input.Paste(...);
		msgbox(...); //TaskDialog.Show(...);
		wait(...); //Time.Wait(...);
		click(...); //Mouse.Click(...);
		mmove(...); //Mouse.Move(...);
		run(...); //Shell.Run(...);
		act(...); //Wnd.Activate(...);
		win(...); //Wnd.Find(...);
		speed=...; //Script.Speed=...;

		using(Script.TempOptions(speed

		*/

		//[SysCompil.MethodImpl(SysCompil.MethodImplOptions.NoOptimization)]

		//SysColl.List<int> a;
		//SysText.StringBuilder sb;
		//SysRX.Regex rx;
		//SysDiag.Debug.Assert(true);
		//SysInterop.SafeHandle se;
		//SysIO.File.Create("");
		//SysThread.Thread th;
		//SysTask.Task task;
		//SysReg.RegistryKey k;
		//SysForm.Form form;
		//SysDraw.Rectangle rect;
		//System.Runtime.CompilerServices



		//l.Perf.First();
		//l.TaskDialog.Show("f");
		//l.Util.LibDebug_.PrintLoadedAssemblies();
		//Print(l.TDIcon.Info);

	}
}

