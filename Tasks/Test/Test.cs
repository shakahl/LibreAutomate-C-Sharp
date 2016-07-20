using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.Linq;

using System.Diagnostics;
using System.Threading;

using System.IO;
//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Drawing;
using K = System.Windows.Forms.Keys;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;
using static Catkeys.Automation.NoClass;
using Catkeys.Triggers;


//using Cat = Catkeys.Automation.Input;
//using Meow = Catkeys.Show;

//using static Catkeys.Show;

using System.Xml.Serialization;

using System.Xml;
using System.Xml.Schema;

using Microsoft.VisualBasic.FileIO;
using System.Globalization;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

#pragma warning disable 162, 168, 219, 649 //unreachable code, unused var/field


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
					//Out(a, "|");
					string s = a[1];
					switch(a[0]) {
					case "style": style = (uint)s.ToInt_(); break;
					case "control": control = s; break;
					case "x": x = s.ToInt_(); break;
					case "propValue": propValue = s.ToInt_(); break;
					}
				}
			}
		}
	}
}

public partial class Test
{
	#region old_test_functions

	static void TestCurrentCulture()
	{
		string s = "FILE";
		//s = "FİLE";

		string pat = "file";
		pat = "fi*";
		//pat = "fi?*";

		string f = pat;

		for(int i = 0; i < 2; i++) {
			if(i == 1) Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");

			var w1 = new WildString(pat);
			var w2 = new WildString(pat, 0, true);
			var w3 = new WildString(pat, 0, false, true);
			var w4 = new WildString(pat, 0, true, true);

			OutList("wild", w1.Match(s), w2.Match(s), w3.Match(s), w4.Match(s));

			//WildStringType t = WildStringType.Regex;
			//var f1 = new WildString(f, t, false, false);
			//var f2 = new WildString(f, t, true, false);
			//var f3 = new WildString(f, t, false, true);
			//var f4 = new WildString(f, t, true, true);

			//f = "fi.+";
			var f1 = new WildString(f);
			var f2 = new WildString("[i]" + f);
			var f3 = new WildString("[c]" + f);
			var f4 = new WildString("[ci]" + f);
			//var f2 = new WildString("[fi]"+f);
			//var f3 = new WildString("[fc]"+f);
			//var f4 = new WildString("[fci]"+f);
			//var f2 = new WildString("[ri]"+f);
			//var f3 = new WildString("[rc]"+f);
			//var f4 = new WildString("[rci]"+f);

			OutList("find", f1.Match(s), f2.Match(s), f3.Match(s), f4.Match(s));
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

		OutList(v.style, v.x, v.control, v.propValue);
	}

	static void TestCsv()
	{
		string s = @"a1,b1, c1
a2, ""b2 """" aaa
bbb"", b3
";
		var tr = new StringReader(s);
		var p = new TextFieldParser(tr);
		//Out(p.HasFieldsEnclosedInQuotes);
		//Out(p.TrimWhiteSpace);
		p.SetDelimiters(new string[] { "," });

		while(!p.EndOfData) {
			string[] a = p.ReadFields();
			Out(a.Length);
			Out(a, "|");
		}
	}

	static void TestSerialization()
	{
		//var m = new Wnd._FindProperties();
		//m.style = "0x12 0x12";
		//var x3 = new XmlSerializer(typeof(Wnd._FindProperties));
		//var t3 = new StringWriter();
		//x3.Serialize(t3, m);
		//Out(t3.ToString());
		//return;

		//var v = new Ooo() { style = 0x123, x = 5, control = "Ccccc", propValue = 10 };
		var v = new Ooo() { style = 0x12, control = "Ccccc", x = 5 };

		var x = new XmlSerializer(typeof(Ooo));
		var t = new StringWriter();
		x.Serialize(t, v);
		string s = t.ToString();
		Out(s);

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
		//OutList(v2.style, v2.x, v2.control, v2.propValue);
		OutList(v2.style, v2.x, v2.control);

	}

	static void TestShow()
	{
		//Show.TaskDialog(Wnd0, "text", icon:TDIcon.Warning);
		//Show.TaskDialog("text", "", "!");
		//Show.TaskDialog("text", "", new System.Drawing.Icon(@"q:\app\find.ico"));

		//Show.TaskDialog(Wnd0, "text", flags: TDFlag.RawXY);
		//Show.TaskDialogEx(Wnd0, "text", flags: TDFlag.RawXY, x:-100);
		//Show.TaskDialog("text", null, "r");
		//Show.TaskDialogEx("text", null, "r", x:-100);

		//Task.Run(() =>
		//{
		//	Wait(5);
		//	//Script.Option.dialogRtlLayout = true;
		//	//Script.Option.dialogScreenIfNoOwner = 2;
		//	TestDialogScreen("thread");
		//      });

		////Script.Option.dialogScreenIfNoOwner = 1;
		////TestDialogScreen("main");

		////var f = new Form();
		////f.ShowDialog();

		//Wait(10);


		//Show.TaskDialogNoWait(null, "Text."); //simplest example
		//var td=Show.TaskDialogNoWait(ed => { Out(ed); }, "Text.", style: "OCi");
		//var td=Show.TaskDialogNoWaitEx(ed => { Out(ed); }, "Text.", "text", "OCi", Wnd0, "1 Cust", "1 ra", "Check", "exp", "foo", "Tii", 100, -100, 30);
		//Wait(3); //do something while the dialog is open in other thread
		//td.ThreadWaitClosed(); //wait until dialog closed (optional, but if the main thread will exit before closing the dialog, dialog's thread then will be aborted)


		//Show.TaskDialog("aaa");

		//bool marquee = false;
		//var pd = Show.ProgressDialog(marquee, "Working", customButtons: "1 Stop", y: -1);
		////var pd = Show.ProgressDialogEx(marquee, "Working", "ttt", "a", Wnd0, "1 Stop", "1 r1|2 r2", "Check", "exp", "foo", "Tii", 100, -1, 30);
		//for(int i = 1; i <= 100; i++) {
		//	if(!pd.IsOpen) { Out(pd.Result); break; } //if the user closed the dialog
		//	if(!marquee) pd.Send.Progress(i);
		//	WaitMS(50); //do something in the loop
		//}
		//pd.Send.Close();

		//Out(Show.ListDialog("1 one|2 two|3 three\r\n").ToString());
		//Out(Show.ListDialog("1 One|2 Two|3 Three|Cancel", "Main instruction.", "More info.", "Cxb").ToString());
		//Out(Show.ListDialog("1 One|2 Two|3 Three|Cancel", "Main instruction.", "More info.", TDIcon.App).ToString());
		//Out(Show.ListDialogEx("1 One|2 Two|3 Three|Cancel", "Main instruction.", "More info.", TDIcon.App, Wnd0, "", "exp\r\n<a href=\"mmm\">link</a>", "foo: <a href=\"mmm\">link</a>", "Moo", -1, 100, 30, (ed)=>Out(ed.linkHref)).ToString());

		//Show.ListDialog("1|2|3|4|5|6|7|8|9|10|11|12|13|14|15|16|17|18|19|20|21|22|23|24|25|26|27|28|29|30");
		//Show.ListDialog("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA BBBBBBBBBBBBBBBBBBBBBBBBB");
		//Out(Show.ListDialogEx("1 one|2 two|3 three\r\n", footerText: "!|foo").ToString());
		//return;

		//var f = new Form();
		//f.Show();

		//Script.Option.dialogRtlLayout=true;
		//string s; //int i;
		//if(!Show.InputDialog(out s)) return;
		//if(!Show.InputDialog(out s, "Text.", owner: Wnd.Find("Untitled - Notepad"))) return;
		//if(!Show.InputDialog(out s, "Text gggggggggggg.")) return;
		//if(!Show.InputDialog(out s, "Text.", "Default")) return;
		//if(!Show.InputDialogEx(out s, "Text.", "0", editType: TDEdit.Number, expandedText:"exp")) return;
		//if(!Show.InputDialog(out i, "Text.", 5)) return; Out(i); return;
		//if(!Show.InputDialogEx(out s, "Text.", "pas", editType: TDEdit.Password)) return;
		//if(!Show.InputDialog(out s, "Text.", "one\r\ntwo\r\nthree", editType: TDEdit.Multiline)) return;
		//if(!Show.InputDialog(out s, "Text.", "def\none\r\ntwo\nthree", editType: TDEdit.Combo)) return;
		//if(!Show.InputDialogEx(out s, "Text.", "def\none\r\ntwo\nthree", Wnd0, TDEdit.Combo, "i", "exp", "foo", "Tii", 200, -100, 30, "1 Browse...", ed => { if(ed.wParam == 1) { string _s; if(Show.InputDialog(out _s, owner:ed.hwnd)) ed.obj.EditControl.SetControlText(_s); ed.returnValue = 1; } })) return;
		//if(!Show.InputDialogEx(out s, "Text.", "def\none\r\ntwo\nthree", Wnd0, TDEdit.Combo, "i", "exp", "foo", "Tii", 200, -100, 30, "", ed => { if(ed.wParam == TDResult.OK) { string _s=ed.obj.EditControl.Name; if(Empty(_s)) { Show.TaskDialog("Text cannot be empty.", owner: ed.hwnd); ed.returnValue = 1; } } })) return;
		//if(!Show.InputDialogEx(out s, "Text.", "def\none\r\ntwo\nthree", Wnd0, TDEdit.Combo, "i", "exp", "foo", "Tii", 200, -100, 30, "1 Browse...", ed=>
		//{
		//	if(ed.wParam != 1) return;
		//	string _s; if(Show.InputDialog(out _s, owner:ed.hwnd)) ed.obj.EditControl.SetControlText(_s);
		//	ed.returnValue = 1;
		//})) return;
		//if(!Show.InputDialogEx(out s, "Text.")) return;
		//if(!Show.InputDialogEx(out s, "Text.", footerText:"a|Foooooo.")) return;
		//if(!Show.InputDialogEx(out s, "Text.", footerText:"a|Foooooo.", timeoutS:30)) return;
		//bool ch;
		//if(!Show.InputDialogEx(out s, out ch, "Check", "Text.", "one\r\ntwo\r\nthree", editType:TDEdit.Multiline, expandedText:"More\ntext.", timeoutS: 60)) return;
		//if(!Show.InputDialog(out s, out ch, "Check", "Text.", "txt", editType:TDEdit.Multiline)) return;
		//Out(s);
		//Out(ch);

		//if(false) {
		//	d.SetCustomButtons("1 Browse...", true, true);
		//	d.ButtonClicked += e => { if(e.wParam == 1) { Out("Browse"); e.returnValue = 1; } };
		//}

		//MessageBox.Show("ddddddddddddddddddd");
		//Show.MessageDialog("fffffffffff");

		//return;


		//TaskDialogAsync("async"); //warning "consider applying await"
		//TDResult rr=await TaskDialogAsync("async"); //error, the caller must be marked with async. But then fails to run altogether.

		//Task<TDResult> t=TaskDialogAsync("async");
		//Show.TaskDialog("continue");
		//t.Wait();
		//Out(t.Result);

		//var pd = TaskDialogNoWait("async", y=>Out(y));
		//var pd = TaskDialogNoWait("async");
		//Wait(2);
		//if(pd.IsOpen) pd.Send.Close();
		//Out(pd.Result);

		//var td = new TaskDialogObject("dddd");
		//Task.Run(() => td.Show());
		////Perf.First();
		////td.ThreadWaitOpen();
		////Perf.NextWrite();
		//td.ThreadWaitClosed();
		////Task.Run(() => td.Show());
		////td.ThreadWaitOpen();
		////td.ThreadWaitClosed();
		//Out(td.Result);

		//Show.TaskDialog("continue", y:300);


		//Out("finished");

		//Task t = Task.Run(() =>
		//{
		//	//Thread.Sleep(100);

		//	//Out("run");
		//	Out(Show.TaskDialog("async", style: "OC", x: 1));
		//	//MessageBox.Show("another thread");
		//	//Show.MessageDialog("async",style:"OC");
		//	//TD("async", true);
		//}
		//);

		////Thread.Sleep(7);

		//Out(Show.TaskDialog("continue", style: "OC"));
		////TD("continue", false);
		////Show.MessageDialog("continue",style:"OC");
		////Thread.Sleep(1000);
		//t.Wait();
		//Out("after all");

		//for(int i=0; i<5; i++) TD("continue", false);

		//Out(GetThemeAppProperties());
		//MessageBox.Show("sss");
		//Out(Show.MessageDialog("test", MDButtons.OKCancel, MDIcon.App, MDFlag.DefaultButton2));
		//Out(Show.MessageDialog("One\ntwooooooooooooo."));
		//Out(Show.MessageDialog("One\ntwooooooooooooo.", "YNC!t2"));
		//Show.MessageDialog("One\ntwooooooooooooo.");
		//Out(Wnd.ActiveWindow);

		//Out(Show.TaskDialog(Wnd0, "Head1\nHead2.", "Text1\nText2.", TDButton.OKCancel, TDIcon.App, TDFlag.CommandLinks, TDResult.Cancel, "1 one|2 two", new string[] { "101 r1|||", "102 r2" }, "Chick|check", "expanded", "", "TTT", 0, 0, 20).ToString());
		//Out(Show.TaskDialog("Head1\nHead2.", "Text1\nText2.", "OCd2!t", Wnd0, "1 one|2 two", null, null, "expanded", "foo", 60, "TTT").ToString());
		//Out(Show.TaskDialog(Wnd0, "Head1\nHead2.", "Text1\nText2.", TDButton.OKCancel|TDButton.YesNo|TDButton.Retry|TDButton.Close, TDIcon.Info).ToString());
		//Out(Show.TaskDialog(Wnd0, "Head1\nHead2.", "Text1\nText2.", TDButton.OKCancel|TDButton.YesNo|TDButton.Retry|TDButton.Close, (TDIcon)0xfff0).ToString());
		//Out(Show.TaskDialog("head", "content", "OCYNLRio", owner: Wnd.Find("Untitled - Notepad")).ToString());
		//Out(Show.TaskDialog("head", "content", "OCYNLRi", x:100, y:-11, timeoutS:15).ToString());
		//Out(Show.TaskDialog("", "<a href=\"example\">link</a>.", onLinkClick: ed => { Out(ed.linkHref); }).ToString());
		//Out(Show.TaskDialog("head", "content", "i", customButtons: "-1 Mo OK|-2 My Cancel").ToString());

		//Out(Show.ListDialog("1 one| 2 two| 3three|4 four\nnnn|5 five|6 six|7 seven|8 eight|9 nine|10Ten|0Cancel|1 one|2 two|3three|4 four\nnnn|5 five|6 six|7 seven|8 eight|9 nine|10Ten", "Main", "More."));
		//Out(Show.ListDialog(new string[] { "1 one", "2 two", "Cancel" }, "Main", "More").ToString());
		//Out(Show.ListDialog(new List<string> { "1 one", "2 two", "Cancel" }, "Main", "More").ToString());
		////		Out(Show.ListDialog(@"
		////|1 one
		////|2 two
		////comments
		////|3 three
		////" , "Main", "More\r\nmore"));

		////		Out(Show.ListDialog("1 one|2 two\nN|3 three\r\nRN|4 four"));
		//return;

		//var d = new TaskDialogObject("Head", "Text <A HREF=\"xxx\">link</A>.", TDButton.OKCancel|TDButton.Retry, TDIcon.Shield, "Title");
		//var d = new TaskDialogObject("Head", "Text <A HREF=\"xxx\">link</A>.", TDButton.OKCancel|TDButton.Retry, (TDIcon)0xfff0, "Title");
		//var d = new TaskDialogObject("Head", "Text <A HREF=\"xxx\">link</A>.", (TDButton)111);
		//var d = new TaskDialogObject("Head Text.", null, 0, TDIcon.Shield);
		//var d = new TaskDialogObject("", "More text.", 0, TDIcon.Shield);
		//var d = new TaskDialogObject();
		var d = new TaskDialogObject();

		d.SetTitleBarText("MOO");

		d.SetText("Main text.", "More text.\nSupports <A HREF=\"link data\">links</A> if you subscribe to HyperlinkClick event.");

		d.SetButtons(TDButton.OKCancel | TDButton.Retry);

		d.SetStyle("YNC");

		//d.SetIcon(TDIcon.Warning);
		//d.SetIcon(new System.Drawing.Icon(@"Q:\app\copy.ico", 32, 32)); //OK
		//d.SetIcon(Catkeys.Tasks.Properties.Resources.output); //OK
		//d.SetIcon(new System.Drawing.Icon(Catkeys.Tasks.Properties.Resources.output, 16, 16)); //OK
		//d.SetIcon(new System.Drawing.Icon("Resources/output.ico")); //OK
		//d.SetIcon(new System.Drawing.Icon("Resources/output.ico", 16, 16)); //OK
		//d.SetIcon(new System.Drawing.Icon(typeof(Test), "output.ico")); //exception
		//Out(Catkeys.Tasks.Properties.Resources.output.Width);
		//d.SetIcon(new System.Drawing.Icon(Show.Resources.AppIconHandle32));
		//d.SetIcon(TDIcon.App);

		Wnd w = Wnd.Find("Untitled - Notepad");
		//d.SetOwnerWindow(w);

		//Script.Option.dialogScreenIfNoOwner=2;

		d.SetXY(100, 100);

		d.SetCheckbox("Checkbox", false);

		Script.Option.dialogTopmostIfNoOwner = true;
		//d.FlagTopmost=true;
		d.FlagAllowCancel = true;
		d.FlagCanBeMinimized = true;
		//d.FlagRtlLayout=true;
		//d.FlagPositionRelativeToWindow=true;
		//d.FlagNoTaskbarButton = true;
		//d.FlagNeverActivate = true;

		//Script.Option.dialogScreenIfNoOwner=2;
		//d.Screen=2;

		d.SetExpandedText("Expanded info\nand more info.", true);

		d.SetExpandControl(true, "Show more info", "Hide more info");

		//d.SetFooterText("Footer text.", TDIcon.Warning);
		//d.SetFooterText("Footer.");
		//d.SetFooterText("Footer.", Catkeys.Tasks.Properties.Resources.output); //icon 32x32, srinked
		//d.SetFooterText("Footer.", new System.Drawing.Icon(Catkeys.Tasks.Properties.Resources.output, 16, 16)); //icon OK
		//d.SetFooterText("Footer text.", new System.Drawing.Icon(@"q:\app\wait.ico", 16, 16));

		//d.Width=700;

		//d.SetCustomButtons(new string[] { "101 one", "102 two" });
		d.SetCustomButtons("1 one|2 two\nzzz", true);
		//d.SetCustomButtons(new string[] { "5", "102 two" }, true);
		//d.SetCustomButtons("101|102 two\nzzz", true);
		d.DefaultButton = TDResult.No;
		//d.SetDefaultButton(2);
		//d.SetDefaultButton(TDResult.Cancel);
		//d.SetDefaultButton(TDResult.Retry);

		//d.SetRadioButtons(new string[] { "1001 r1", "1002 r2" }, 1002);
		d.SetRadioButtons("1001 r1|1002 r2");

		//d.SetTimeout(10, "Cancel");
		//d.SetTimeout(10, null, true);

		//d.Created += ed => { Out($"{ed.message} {ed.wParam} {ed.linkHref}"); };
		d.Created += ed => { ed.obj.Send.EnableButton(TDResult.Yes, false); };
		//d.Created += ed => { ed.OwnerWindow.Enabled=true; };
		//d.Created += ed => { ed.hwnd.Owner.Enabled=true; };
		//d.Created += ed => { Wnd.Get.Owner(ed.hwnd).Enabled=true; };
		//d.Created += ed => { w.Enabled=true; };
		//d.Destroyed += ed => { Out($"{ed.message} {ed.wParam} {ed.linkHref}"); };
		d.ButtonClicked += ed => { Out($"{ed.message} {ed.wParam} {ed.linkHref}"); if(ed.wParam == TDResult.No) ed.returnValue = 1; };
		d.HyperlinkClicked += ed => { Out($"{ed.message} {ed.wParam} {ed.linkHref}"); };
		//d.OtherEvents += ed => { Out($"{ed.message} {ed.wParam} {ed.linkHref}"); };
		//d.Timer += ed => { Out($"{ed.message} {ed.wParam} {ed.linkHref}"); };
		//d.HelpF1 += ed => { Out($"{ed.message} {ed.wParam} {ed.linkHref}"); };

		//d.FlagShowProgressBar = true; d.Timer += ed => ed.obj.Send.Progress(ed.wParam / 100);

		//Perf.First();
		TDResult r = d.Show();
		//Perf.NextWrite();

		Out(r.ToString());

		//} catch(ArgumentException e) { Out($"ArgumentException: {e.ParamName}, '{e.Message}'"); } catch(Exception e) { Out($"Exception: '{e.Message}'"); }
		//#endif
	}

	static void TestFolders()
	{

		//Out(Folders.IsFullPath(@""));
		//Out(Folders.IsFullPath(@"\"));
		//Out(Folders.IsFullPath(@"\\"));
		//Out(Folders.IsFullPath(@"c:"));
		//Out(Folders.IsFullPath(@"c:\"));
		//Out(Folders.IsFullPath(@"c:aa"));
		//Out(Folders.IsFullPath(@"c\dd"));
		//Out(Folders.IsFullPath(@"%aa"));
		//Out(Folders.IsFullPath(@"<ff"));
		//Out(Folders.IsFullPath(@"%temp%"));
		//Out(Folders.IsFullPath(@"<ff>"));

		//Out(Folders.Combine(@"%temp%\..", null));
		//Out(Folders.Combine(@"%emp%\..", null));
		//Out(Folders.Combine(@"%temp", null));
		//Out(Folders.Combine(@"<ccc>", null));
		//Out(Folders.Combine(@"<ccc", null));

		//return;

		//Output.Write(Folders.GetKnownFolders());

		//Out(Folders.Desktop);
		//Out(Folders.Desktop + "app.end");
		//string path = Folders.Desktop + "file.txt";
		//Out(path);
		//path = Folders.Desktop;
		//Out(path);
		//path = Folders.Desktop + "..\\file.txt";
		//Out(path);
		//Out(Folders.Virtual.ComputerFolder + "mmm");
		//Out(Folders.Desktop + "app" + ".end");

		//Out(Folders.Combine(@"c:\one", "two"));
		//Out(Folders.Combine(@"c:one", "two"));
		//Out(Folders.Combine(@"c:", "two"));
		//Out(Folders.Combine(@"\\one", "two"));

		//Out(Folders.Combine(null, @"c:\one"));
		//Out(Folders.Combine(null, @"c:one"));
		//Out(Folders.Combine(null, @"c:"));
		//Out(Folders.Combine(null, @"\\one"));
		//Out(1);
		//Out(Folders.Combine("one", "two"));
		//Out(Folders.Combine("one", null));
		//Out(Folders.Combine(null, "two"));
		//Out(Folders.Combine(@"one\", null));
		//Out(Folders.Combine(null, @"\two"));
		//Out(Folders.Combine(@"c:\one\", null));
		//Out(2);
		//Out(Folders.Combine("one", @"\two"));
		//Out(Folders.Combine(@"one\", "two"));
		//Out(Folders.Combine(@"one\", @"\two"));
		//Out(Folders.Combine("one", @"a:\two"));
		//Out(Folders.Combine("one", @"a:two"));
		//Out(3);
		//Out(Folders.Combine(null, @"C:\PROGRA~2\Acer\LIVEUP~1\updater.exe"));
		//Out(Folders.Combine(null, @"C:PROGRA~2\Acer\LIVEUP~1\updater.exe"));
		//Out(Folders.Combine(null, @"..\aaa.bbb"));
		//Out(Folders.Combine(@"C:\PROGRA~2\Acer\LIVEUP~1\..\updater.exe", null));
		//Out(Folders.Combine("C:\\PROGRA~2\\Acer\\LIVEUP~1\nupdater.exe", null));
		//Out(Folders.Combine(@"c:\one~", @" space "));
		//return;
		//Out(Folders.Profile);
		//Out(Folders.Virtual.ControlPanelFolder);
		//Out(Folders.CommonPrograms + "append.this");
		//Out(Folders.ApplicationShortcuts_Win8);
		//Out(Folders.Temp + "file.c");
		//Out(Folders.App + "file.c");
		//Out(Folders.AppTemp + "file.c");
		//Out(Folders.AppDoc + "file.c");
		//Out(Folders.AppRoot + "file.c");
		//Out(1);
		//Out(Folders.GetFolder("Start menu") + "append.nnn");
		//Out(Folders.GetFolder("APp") + "append.nnn");
		//Out(Folders.GetFolder("%temp%") + "append.nnn");
		//Out(Folders.GetFolder("UnknownFolder") + "append.nnn");
		//Out(Folders.EnvVar("Temp") + "append.txt");
		//Out(Folders.ExpandEnvVar("%Temp%") + "append.txt");
		//Out(Folders.EnvVar("unknown") + "append.txt");
		//Out(Folders.ExpandEnvVar("%unknown%") + "append.txt");
		//Out(2);
		//Out(Folders.CDDrive() + "in CDDrive.txt");
		//Out(Folders.RemovableDrive(0) + "in Removable.txt");
		//Out(Folders.RemovableDrive(1) + "in Removable.txt");
		//Out(Folders.RemovableDrive("PortableApp") + "in Removable.txt");
		//Out(Folders.RemovableDrive("PortableApps.com") + "in Removable.txt");
		//Out(3);
		//Out(Folders.Desktop + @"\file.txt");
		//Out($@"{Folders.Desktop}\file.txt");
		//Out(Folders.Desktop + @"\file.txt");
		//Out(Folders.Desktop + @"\\file.txt");
		//Out(Folders.Desktop + @"C:\file.txt");
		//Out(Folders.Desktop);
		//Out(Folders.Desktop + "file.txt");
		//Out(Folders.Desktop + "..\\file.txt");

		//Out(Path.GetFullPath(@"c:\a\b\c."));


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

		//Out(s);
		////Out(s2);

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
		//Out(Path.GetFullPath("..\\nnn.ttt"));

		//Out(File.Exists(@"%windir%\System32\notepad.exe"));
		//Out(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
	}

	static void TestWndAll()
	{
		foreach(Wnd w in Wnd.AllWindows("QM_*")) {
			Out(w);
		}
		Out("ok");

		//Wnd wq = Wnd.Find(null, "QM_Editor");
		//Out(wq.GetClassLong(Api.GCW_ATOM));

		//Out(Wnd.GetClassAtom("Edit"));
		//Out(Wnd.GetClassAtom("QM_Editor"));
		//Out(Wnd.GetClassAtom("QM_Editor", Util.Misc.GetModuleHandleOfExe()));

		//var ac = new string[] { "IME", "MSCTFIME UI", "tooltips_class32", "ComboLBox", "WorkerW", "VBFloatingPalette" };
		//foreach(string s in ac) Out(Wnd.GetClassAtom(s));

		//return;

		//List<Wnd> a = null;

		////var a1 = new Action(() => { a = Wnd.AllWindows(); });
		////var a1 = new Action(() => { a = Wnd.AllWindows(null, true); });
		////var a1 = new Action(() => { a = wy.ChildAllRaw(); });
		////var a2 = new Action(() => { wy.ChildAllRaw((c, e) => { }); });
		//var a1 = new Action(() => { a = Wnd.AllWindows(); });
		//var a2 = new Action(() => { Wnd.AllWindows((w3, e) => { }); });

		////wy.ChildAllRaw((c, e)=> { Out(c); }, null, true); return;
		////Wnd.AllWindows((c, e)=> { Out(c); }, "QM_*"); return;
		////Wnd.AllWindows((c, e)=> { Out(c); }, null, true); return;
		////foreach(Wnd w3 in Wnd.AllWindows(onlyVisible:true)) { Out(w3); }; return;
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
		//Out(a.Count);

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

		//	var a1 = new Action(() => { a=hform.ChildAllRaw(); });
		//	//var a1 = new Action(() => { a=wq.ChildAllRaw(); });
		//	//a1();

		//	for(int k=0; k<5; k++) {
		//		Perf.First();
		//		Perf.Execute(1000, a1);
		//		Perf.Write();
		//	}
		//	Out(a);
		//      };

		//f.ShowDialog();


		////Output.Write(Wnd.All.ThreadWindows(wq.ThreadId));
		////Output.Write(Wnd.All.ThreadWindows(wq.ThreadId, "QM_toolbar"));
		//Output.Write(Wnd.All.ThreadWindows(wq.ThreadId, "", true));

		//Show.TaskDialogEx("", "<a href=\"test\">test</a>", onLinkClick: ed =>
		//{
		//	var a = Wnd.All.ThreadWindows();
		//	Output.Write(a);
		//});

		//return;



		//var gu= new Guid("{82A5EA35-D9CD-47C5-9629-E15D2F714E6E}");
		//Out(gu);

		//Out(SpecFolder.CommonStartup);
		//Out(SpecFolder.Desktop);



		////string g;
		//////g = "ąčęėįšųūž абв αβγ";
		////g = "ĄČĘĖĮŠŲŪŽ АБВ ΑΒΓ";

		//////Out(g.ToUpper());
		//////Out(g.ToUpper_());
		////Out(g.ToLower());
		////Out(g.ToLower_());



		////return;

		////Wnd w=(Wnd)1245464;
		////List<Wnd> e1 = w.ChildAllRaw();
		////Out(e1.Count);
		//////IEnumerable<Wnd> e= w.DirectChildControlsEnum();
		////List<Wnd> e2 = w.DirectChildControlsFastUnsafe();
		////Out(e2.Count);

		////Perf.First(100);
		////for(int uu=0; uu<5; uu++) {
		////	Perf.First();
		////	Perf.Execute(1000, () => { e2 =w.DirectChildControlsFastUnsafe(); });
		////	Perf.Execute(1000, () => { e1 =w.ChildAllRaw(); });
		////	Perf.Execute(1000, () => { e1 = w.ChildAllRaw("Button", true); });
		////	Perf.Execute(1000, () => { w.ChildAllRaw(c=> { /*Out(c);*/ return false; }; });
		////	Perf.Write();
		////}

	}

	static void TestProcesses()
	{
		Wnd w = Wnd0;
		Wnd.WinFlag hiddenToo = Wnd.WinFlag.HiddenToo;

		//Process_.EnumProcesses(p =>
		//{
		//	Out(p.ProcessName);
		//	return false;
		//});
		//return;

		//w =Wnd.Find("", program: "YouTubeDownloader");
		//w =Wnd.Find("", program: @"C:\Program Files (x86)\Free YouTube Downloader\YouTubeDownloader.exe");
		//Out(w);
		//Out(w.ProcessName);
		//Out(w.ProcessPath);
		//return;

		//Out(Process_.GetProcessName(7140));
		//Out(Process_.GetProcessName(1988));
		//Out(Process_.GetProcessName(1988, true));

		//string pn1=null, pn2 = null;
		//var a11 = new Action(() => { pn1=Process_.GetProcessName(1988); }); //qmserv
		//var a12 = new Action(() => { pn2=Process_.GetProcessName(7140); }); //firefox
		//Perf.ExecuteMulti(5, 1000, a11, a12);
		//Out(pn1); Out(pn2);
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

		//a6(); Out(w); return;

		a1(); Out(w);
		a2(); Out(w);
		a3(); Out(w);
		a4(); Out(w);
		a5(); Out(w);
		a6(); Out(w);
		a7(); Out(w);
		a8(); Out(w);

		Perf.ExecuteMulti(5, 1, a1, a2, a3, a4, a5, a6, a7, a8);

		//Wnd w = (Wnd)12582978; //Notepad

		//string s = null;

		//s = w.ProcessName;
		//Out(s);
		//s = w.ProcessPath;
		//Out(s);

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

		//Out(name);

		//a = Process.GetProcesses();
		//Out(a.Length);
		//a = Process.GetProcessesByName("NoTepad");
		//Out(a.Length);
		//a = Process.GetProcessesByName("regedit");
		//Out(a.Length);
		//foreach(Process p in a) {
		//	Out(p.Id);
		//	//Out(p.Handle);
		//}
	}

	static void TestProcessUacInfo()
	{
		////Out(UacInfo.ThisProcess.IntegrityLevel);
		////Out(Api.GetModuleHandle("shell32"));
		////Out(Api.GetModuleHandle("advapi32"));
		//bool is1 = false, is2 = false, is3=false;
		//Perf.SpinCPU(200);
		//Perf.ExecuteMulti(5, 1, () => { is1 = UacInfo.IsThisProcessAdmin; }, () => { is2 = Api.IsUserAnAdmin(); }, () => { is3 = IsUserAdmin; });
		//OutList(is1, is2, is3);
		////Out(Api.GetModuleHandle("shell32"));
		////Out(Api.GetModuleHandle("advapi32"));

		//Out(UacInfo.IsUacDisabled);
		Out(UacInfo.IsAdmin);
		return;

		Out(UacInfo.IsAdmin);

		Process[] a = Process.GetProcesses();
		for(int i = -5; i < a.Length; i++) {
			Process x = null;
			UacInfo p = null;

			Perf.First();
			if(i < 0) p = UacInfo.ThisProcess;
			else {
				x = a[i];
				Out($"<><Z 0x80c080>{x.ProcessName}</Z>");
				p = UacInfo.GetOfProcess((uint)x.Id);
			}
			if(p == null) { Out("failed"); continue; }
			Perf.Next();
			var elev = p.Elevation; if(p.Failed) Out("<><c 0xff>Elevation failed</c>");
			Perf.Next();
			var IL = p.IntegrityLevel; if(p.Failed) Out("<><c 0xff>IntegrityLevel failed</c>");
			Perf.Next();
			var IL2 = p.IntegrityLevelAndUIAccess;
			Perf.Next();
			bool uiaccess = p.IsUIAccess; if(p.Failed) Out("<><c 0xff>IsUIAccess failed</c>");
			Perf.Next();
			bool appcontainer = p.IsAppContainer; if(p.Failed) Out("<><c 0xff>IsAppContainer failed</c>");
			Perf.Next();
			Out($"elev={elev}, uiAccess={uiaccess}, AppContainer={appcontainer}, IL={IL}, IL+uiaccess={IL2}");
			//Out($"uiAccess={uiaccess}, IL={IL}");
			//Perf.Write();
		}
	}

	static void TestWildString()
	{
		//string ss = "gggggg.txt";
		//Out(ss.Like_("*.Txt"));
		//Out(ss.Like_("*.Txt", true));
		//Out(ss.Like_(false, "*.exe", "*.txt"));

		//string ss = "5ggggg.txt";
		//Out(ss.LikeEx_(false, "#*.exe", "#*.txt"));

		//var y = new WildString("asd[fgh");
		//Out(y.Match("asd[fgh"));
		////var a3 = new Action(() => y.Match("asd[fgh"));

		string s, p;

		//s = "qwertyuiopasdfghjklzxcvbnm.txt";
		s = "qwertyuiopasdfghjklZxcvbnm.Txt";
		//s = "";
		//s = null;
		//s = "A";

		p = "";
		//p = null;
		//p = "*";
		p = "*.txt";
		//p = "qwer*";
		//p = "*zxcv*";
		//p = s;
		//p = "qwerty*.txt";
		//p = "qwerty*g*.txt";
		//p = "*werty*.txt";
		//p = "qwerty*.tx*";
		//p = "?werty*.txt";
		//p = "qwerty?i*.txt";
		//p = "*erty?i*.txt";
		//p = "qw**rty*.txt";
		//p = "qw*?rty*.txt";
		//p = "qw?*rty*.txt";
		//p = "qw#rty*.tx?";
		//p = "qw[rty*.tx?";
		//p = "qwertyuiopasdfghjklzxcvbnm.???";
		//p = "?*";
		//p = "?";

		p = "*.txt";
		p = "[]*.txt";
		p = "[d]*.txt";
		p = "[i]*.txt";
		p = "[-i]*.txt";
		p = "[c]*.txt";
		p = "[f]*.txt";
		p = $"[f]{s}";
		p = "[f]bnm.";
		p = "[p]bnm.t";
		p = @"[r]bnm\.t";

		bool ignoreCase = true;

		//var x = new WildString(p, ignoreCase);

		WildString x = p;
		WildStringI y = p;

		//x = new Regex("^q.+t$");
		//x = new WildString(new Regex("^q.+t$"));
		//x = new WildString(new Regex("^q.+t$"), "*v*");
		//y = new Regex("^q.+t$");
		//y = new WildStringI(new Regex("^q.+t$"));
		//y = new WildStringI(new Regex("^q.+t$"), "*v*");
		//x = new WildString(p, ignoreCase, false, "*v*");
		x = new WildString(s, WildStringType.Full, ignoreCase);
		//x = new WildString(s, WildStringType.Part, ignoreCase);
		//x = new WildString(".txt", WildStringType.Part, ignoreCase);
		//x = new WildString(".txt", WildStringType.Part, ignoreCase, false, "*v*");
		//x = new WildString(@"\.txt$", WildStringType.Regex);
		//x = new WildString(@"\.txt$", WildStringType.Regex, ignoreCase);
		//y = new WildStringI(s.ToUpper(), WildStringType.Full);
		//y = new WildStringI(s.ToUpper(), WildStringType.Part);
		//y = new WildStringI(".txt", WildStringType.Part);
		//y = new WildStringI(".txt", WildStringType.Part, ignoreCase, false, "*v*");
		//y = new WildStringI(@"\.txt$", WildStringType.Regex);
		x = new WildString(".Txt");
		Out(x.StringType);
		Out(x.WildcardType);
		Out(x.Match(".Txt"));
		//Out(y.ToString());
		return;


		if(x == null) Out("null"); else Out(x.Match(s));
		if(y == null) Out("null"); else Out(y.Match(s));
		//if(x == null) Out("null"); else Out(x.Match(s, true));
		//if(y == null) Out("null"); else Out(y.Match(s, true));

		////OutList(x.WildcardType, x.Pattern, x.AltPattern, "   ", s.LikeEx_(p, ignoreCase), x.Match(s));
		//bool like = false, excep = false; try { like = s.LikeEx_(p, ignoreCase); } catch { excep = true; Out("exception"); }
		////bool like = false, excep=false; try { like = s.LikeEx_(x.Pattern, ignoreCase); } catch { excep = true; Out("exception"); }
		//OutList(x.WildcardType, x.Text, "   ", like, x.Match(s));

		//var a1 = new Action(() => s.LikeEx_(p, ignoreCase));
		////var a1 = new Action(() => s.LikeEx_(x.Pattern, ignoreCase));
		//var a2 = new Action(() => x.Match(s));
		//var a3 = new Action(() => { var y = new WildString(p, ignoreCase); y.Match(s); });

		////Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

		//if(excep) Perf.First(100, a2, a2);
		//else Perf.First(100, a1, a2, a2);
		//for(int i = 0; i < 4; i++) {
		//	Perf.First();
		//	if(!excep) Perf.Execute(1000, a1);
		//	Perf.Execute(1000, a2);
		//	Perf.Execute(1000, a3);
		//	//for(int j = 0; j < 1000; j++) s.LikeEx_(p);
		//	//Perf.Next();
		//	//for(int j = 0; j < 1000; j++) x.Match(s);
		//	//Perf.Next();
		//	Perf.Write();
		//}

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

		a1(); a2(); Out(yes);
		a1i(); a2i(); Out(yes);
		a1o(); a2o(); Out(yes);
		a1oi(); a2oi(); Out(yes);

		Perf.ExecuteMulti(5, 1000, a1, a2, a1i, a2i, a1o, a2o, a1oi, a2oi);

		//compiling: Regex 4500, PCRE 450.
		//match (invariant, match case): Regex 1200, PCRE 300.
		//match (invariant, ignore case): Regex 1700, PCRE 300.
	}

	static void TestProcessMemory()
	{
		//var w = Wnd.FindRaw("QM_Editor");
		var w = Wnd.FindRaw("Notepad");
		Out(w);
		ProcessMemory x = null;
		try {
			x = new ProcessMemory(w, 1000);
			//x = new ProcessMemory(w, 0);

		}
		catch(CatkeysException e) { Out(e); return; }

		//Out(1);
		//Out(x.WriteUnicodeString("Unicode"));
		//Out(2);
		//Out(x.WriteAnsiString("ANSI", 100));
		//Out(3);
		//Out(x.ReadUnicodeString(7));
		//Out(4);
		//Out(x.ReadAnsiString(4, 100));
		//Out(5);

		unsafe
		{
			int i1 = 5, i2 = 0;
			//Out(x.Write(&i1, 4));
			//Out(x.Read(&i2, 4));
			//Out(x.Write(&i1, 4, 50));
			//Out(x.Read(&i2, 4, 50));
			Out(x.WriteOther(x.Mem, &i1, 4));
			Out(x.ReadOther(x.Mem, &i2, 4));
			Out(i2);
		}
	}

	static void TestDotNetControls()
	{
		var w = Wnd.Find("Free YouTube Downloader", "*.Window.*");
		//var w = Wnd.Find("Keyboard Layout*", "*.Window.*");
		//var c = w.Child("", "*.SysListView32.*");
		Out(w);
		if(w.Is0) return;
		//Out(c);
		var x = new WindowsFormsControlNames(w);

		////Out(x.GetControlName(c));
		//Out(x.GetControlName(w));

		var a = w.ChildAllRaw();
		foreach(Wnd k in a) {
			Out("---");
			Out(k);
			Out(WindowsFormsControlNames.IsWindowsForms(k));
			Out(x.GetControlName(k));
		}
	}

	static void TestDotNetControls2()
	{
		//Wnd w = Wnd.Find("Keyboard*").Child("Caps");
		//Wnd w = Wnd.Find("Keyboard*");
		//Wnd w = Wnd.Find("Quick*");
		Wnd w = Wnd.Find("Free YouTube*").Child("My*");
		Out(w);
		//Out(WindowsFormsControlNames.CachedGetControlName(w));

		string s1 = null, s2 = null;
		var a1 = new Action(() =>
		{
			try {
				using(var x = new WindowsFormsControlNames(w)) { s1 = x.GetControlName(w); }
			}
			catch { s1 = null; }
		});
		var a2 = new Action(() => { s2 = WindowsFormsControlNames.CachedGetControlName(w); });

		Perf.ExecuteMulti(5, 10, a1, a2);
		OutList(s1, s2);
	}

	static void TestWildNot()
	{
		string s;
		s = "one two";
		//var x = new WildString("[pn]two!!one*");
		//var x = new WildString("[n]*two!![r]one.+");
		var x = new WildString("[n]*two");
		//var x = new WildString("[pn]two");
		//var x = new WildString("[rn]^.+two$");
		//var x = new WildStringI("[pn]TWO");

		Out(x.Match(s));
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
		//w = Wnd.Find(Wnd.WinFlag.HiddenToo, null, "QM_Editor"); //test when hidden
		//w = Wnd.Find(null, "QM_*");
		//w = Wnd.Find(null, null, "*env");
		//w = Wnd.Find(null, null, "Q:\\*");
		//w = Wnd.Find(Wnd.WinFlag.ProgramPath, null, null, "Q:\\*");
		//w = Wnd.Find(Wnd.WinFlag.ProgramPath, null, null, "c:\\*");
		//w = Wnd.Find(null, "QM_*", matchIndex:2);
		//w = Wnd.Find(null, "QM_*", prop:new Wnd.WinProp() {childClass="QM_Code" } );
		//w = Wnd.Find(null, "QM_*", prop:new Wnd.WinProp() {owner=Wnd.Find("Quick Macros*") } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {processId= Wnd.Find("Quick Macros*").ProcessId } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {threadId=Wnd.Find("*Notepad").ThreadId } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {propName= "qmshex" } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {propName= "qmshex", propValue=1 } );
		//w = Wnd.Find(null, null, prop:new Wnd.WinProp() {style=Api.WS_CAPTION, styleMask=Api.WS_CAPTION } );
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() { styleHas = Api.WS_CAPTION });
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() { styleNot = Api.WS_CAPTION });
		//w = Wnd.Find("* Notepad", null, prop: new Wnd.WinProp() { styleNot = Api.WS_DISABLED, styleHas = Api.WS_CAPTION });
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() { exStyleHas = Api.WS_EX_NOREDIRECTIONBITMAP });
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() { exStyleNot = Api.WS_EX_TOPMOST });
		//w = Wnd.Find(null, null, prop: new Wnd.WinProp() {  });
		//w = Wnd.Find(null, "QM_*", f:e=> { Out(e.w); if(e.w.Name == "TB MSDEV") e.Stop(); });
		//w = Wnd.Find(null, "QM_*", f:e=> { Out(e.w); e.w = Wnd0; e.Stop(); });
		//w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childClass="QM_Code" });
		//w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childName="Te&xt" });
		w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childId = 2202 });
		w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childClass = "Button", childName = "Te&xt" });
		w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childClass = "*Edit", childName = "sea" });
		w = Wnd.Find(null, "", prop: new Wnd.WinProp() { child = new Wnd.ChildDefinition("*ame") });
		w = Wnd.Find("Free YouTube Downloader", "*.Window.*");
		//w = Wnd.Find("Keyboard Layout*", "*.Window.*");
		//w = Wnd.Find("Catkeys -*");
		//w = Wnd.Find("", prop: new Wnd.WinProp() { x=Screen_.Width-1, y=Screen_.Height-10 });
		//w = Wnd.Find("", prop: new Wnd.WinProp() { x=0.5, y=1.1 });
		//w = Wnd.FromXY(1532, 1224, null);
		//w = Wnd.FromXY(0.1, 0.1, null, true, true);

		//Perf.ExecuteMulti(5, 100, () => { Wnd.FindRaw("QM_Editor"); }, () => { Wnd.Find(null, "QM_Editor"); });

		//w = Wnd.FindRaw("QM_Editor");
		Out(w);
		//return;

		//c = w.Child(2202);
		//c = w.Child("Open*");
		//c = w.Child(null, "SysListView32", 2212);
		//c = w.Child(null, "SysListView32");
		//c = w.Child(null, "", 2212);
		//c = w.Child(null, "SysListView32", 0, true);
		//c = w.Child(null, "SysListView32", 0, true, matchIndex:2);
		//c = w.Child(null, "SysListView32", matchIndex:2);
		//c = w.Child(Wnd.ChildFlag.HiddenToo, null, "SysListView32", 0);
		//c = w.Child("sea");
		//c = w.Child("Regex*");
		//c = w.Child(Wnd.ChildFlag.ControlText, "sea");
		//c = w.Child(Wnd.ChildFlag.DirectChild, "", matchIndex:4);
		//c = w.Child(null, "Button", f: e => { Out(e.w); });
		//c = w.Child(null, "Button", f: e => { Out(e.w); if(e.w.Name == "Te&xt") e.Stop(); });
		//c = w.Child(null, "", prop:new Wnd.ChildProp() { exStyleHas=Api.WS_EX_CLIENTEDGE});
		//c = w.Child(null, "", prop:new Wnd.ChildProp() { styleHas=Api.WS_BORDER});
		//c = w.Child(null, "Button", prop:new Wnd.ChildProp() { x=344, y=448});
		//c = w.Child(null, "", prop:new Wnd.ChildProp() { x=0.5, y=0.03});
		//c = w.Child(null, "", prop:new Wnd.ChildProp() { childId=1028 });
		//c = w.Child(null, "", prop: new Wnd.ChildProp() { child = new Wnd.ChildDefinition(Wnd.ChildFlag.DirectChild, "Reg*") });
		//c = w.Child("Regex*", "Button");
		//c = w.Child(null, "", prop: new Wnd.ChildProp() { wfName = "textBoxUrl" });
		//try {
		//	Wnd w = Wnd.Find("Keyboard*");
		//	Wnd c = w.Child("", prop: new Wnd.ChildProp() { wfName = "ckControl" });
		//	Out(c);
		//} catch(CatkeysException e) {
		//	Out(e);
		//}
		//c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildProp() { x=1 });
		//c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildProp() { x=1276});
		//c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildProp() { x=w.ClientWidth-1 });
		//c = w.Child(null, "QM_*", prop:new Wnd.ChildProp() { y=0.99 });
		//c = w.Child("Find &Previous");
		//c = w.Child("Find Previous");

		Out(c);
		return;

		Wnd c1 = Wnd0, c2 = Wnd0, c3 = Wnd0, c4 = Wnd0, c5 = Wnd0, c6 = Wnd0, c7 = Wnd0;
		var a1 = new Action(() => { c1 = w.Child("sea"); });
		var a2 = new Action(() => { c2 = w.Child("sea", flags: Wnd.ChildFlag.ControlText); });
		var a3 = new Action(() => { c3 = w.Child("sea", "*Edit"); });
		var a4 = new Action(() => { c4 = w.Child("sea", "*Edit", 0, Wnd.ChildFlag.ControlText); });
		var a5 = new Action(() => { c5 = w.Child("Regex*"); });
		var a6 = new Action(() => { c6 = w.Child("Regex*", "Button"); });
		var a7 = new Action(() => { c7 = w.Child("Regex*", id: 1028); });
		Perf.ExecuteMulti(5, 100, a1, a2, a3, a4, a5, a6, a7);
		Output.WriteListSep("\n", c1, c2, c3, c4, c5, c6, c7);

		//Out(Process.GetProcessesByName("qm")[0].MainWindowTitle); //TB INTERNET

		//var w = Wnd.Find(null, "QM_Editor");
		//Out(Wnd.Get.NextMainWindow());
		//Out(Wnd.Get.NextMainWindow(w));
		//Out(Wnd.Get.NextMainWindow(w));

		//var a=Wnd.All.MainWindows();
		//foreach(Wnd w in a) {
		//	Out(w);
		//}

		//var x = new Wnd.WinProp() { owner = Wnd0, exStyle = 8 };
		//var y = new Wnd.WinProp(owner: Wnd0, style: 8);
		//TestProp(new Wnd.WinProp() { owner = Wnd0, exStyle = 8 });
		//TestProp(new Wnd.WinProp(owner: Wnd0, style: 8));

		//var a1 = new Action(() => { Wnd.AllWindows(e => { }); });
		//var a2 = new Action(() => { var a = Wnd.All.ThreadWindows(7192); /*Out(a == null); Out(a.Count);*/ });
		////a2();

		//Out("---");
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

		var a1 = new Action(() => { w1 = Wnd.FromXYRaw(Mouse.XY); });
		var a2 = new Action(() => { w2 = Wnd.FromMouse(); });
		//var a3 = new Action(() => { w3 = Wnd.FromXY2(Mouse.XY); });
		//var a4 = new Action(() => { w4 = Wnd.FromXY3(Mouse.XY); });

		for(;;) {
			Wait(1);
			if(Mouse.X == 0 && Mouse.Y == 0) break;
			//a4(); Out(w4); continue;
			Out("---------------------------");
			a1(); a2(); //a3(); a4();
			Perf.ExecuteMulti(3, 1, a1, a2);
			Out(w1);
			Out(w2);
			//Out(w3);
			//Out(w4);
		}

	}

	static void TestChildFromXY()
	{
		Wnd w1 = Wnd.Find("Options");
		Out(w1);
		Out(w1.ChildFromXY(43, 132));
		Out(w1.ChildFromXY(43, 132, true));
		Out(w1.ChildFromXY(1265, 1243, false, true));
		Out(w1.ChildFromXY(1, 1)); //coord not in a child
		Out(w1.ChildFromXY(43, 932)); //coord outside
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
		Perf.NextWrite();
		Out(R);
	}

	static void TestMemory()
	{
		Wnd w = Wnd.Find("", "QM_Editor");
		Out(w);
		//Out(w.Name);
		//Out(w.GetControlText());
		//return;

		while(Show.TaskDialog("test", style: "OC") == TDResult.OK) {
			for(int i = 0; i < 1; i++) {
				TestMemory3(w);
			}
			Show.TaskDialog("allocated 2 MB");
		}
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
		Out(s1);
		Out(s2);
		Out(s3);
		Out(s4);
		Out(s5);
		Out(s6);

		//Out(Folders.Virtual.ControlPanelFolder);
	}

	static void TestArrayAndList(IList<string> a)
	{
		Out(a.Count);
		//for(int)
	}

	static void TestWndFindAll()
	{
		//Out(Wnd.FindAll(0, "*I*"));
		Wnd w = Wnd.Find("Quick*");
		Out(w.ChildAll("", "QM*"));
	}

	static bool _Activate(Wnd w)
	{
		if(!Wnd.Misc.AllowActivate()) return false;
		Api.SetForegroundWindow(Wnd.ActiveWindow);
		Api.SetForegroundWindow(Wnd.ActiveWindow);
		Api.SetForegroundWindow(Wnd.ActiveWindow);
		Perf.First();
		if(!Api.SetForegroundWindow(w)) return false;
		Perf.Next();
		for(int i = 0; i < 10; i++) { Out(Wnd.ActiveWindow); WaitMS(1); }
		//WaitMS(100);
		Perf.Write();
		if(w.IsActive) return true;

		uint tid = w.ThreadId;
		if(tid != 0 && tid == Wnd.ActiveWindow.ThreadId && Api.SetForegroundWindow(Wnd.Get.DesktopWindow)) {
			Api.SetForegroundWindow(w);
			Wnd t = Wnd.ActiveWindow;
			Out(t);
			if(t.ThreadId == tid) return true;
		}

		return false;
	}

	static void TestWndActivateFocus()
	{
		//Wnd.ActiveWindow.ShowMinimized();
		//Out(Wnd.ActiveWindow);
		//Out(Wnd.SwitchActiveWindow(true));
		//return;

		//Wait(2);
		//var a = new Api.INPUTKEY[] { new Api.INPUTKEY(65, 0), new Api.INPUTKEY(65, 0, Api.IKFlag.Up), new Api.INPUTKEY(66, 0), new Api.INPUTKEY(66, 0, Api.IKFlag.Up)};
		//Api.SendInputKey(a);
		//Out("ok");
		//return;

		//Thread.CurrentThread.Priority = ThreadPriority.Highest;
		Wait(2);

		//Out(Wnd.LockActiveWindow(true));
		//return;

		//Wnd w=Wnd.Find("Quick*");
		//Wnd w=Wnd.Find("*Notepad");
		//Wnd w=Wnd.Find("[p]Paint");
		Wnd w = Wnd.Find("Options");
		//Wnd w=Wnd.Find(Wnd.Find("Microsoft Excel*").Name.EndsWith_("dictionary.xls") ? "Book1.xls" : "dictionary.xls");
		Out(w);
		//Out(w.ActivateRaw());
		w.Activate();
		//Out(Api.SetForegroundWindow(Wnd.Get.DesktopWindow));
		//WaitMS(100);
		//Out(Api.SetForegroundWindow(w));
		//Out(_Activate(w));
		//WaitMS(100);
		Out(Wnd.ActiveWindow);

		Wnd c = w.Child("Show*");
		c.FocusControl();
		Out(Wnd.FocusedControl);
		Out(Wnd.FocusedControlOfThisThread);

		//Show.TaskDialog("a");
		return;
		Wait(2);
		w = Wnd.Find("[p]Notepad");
		Out(w.ActivateRaw());
		Out(Wnd.ActiveWindow);
	}

	static void TestWndMinMaxRes()
	{
		//Wnd w = Wnd.Find("", "XLMAIN");
		//Wnd w = Wnd.Find("Book1.xls");
		//Wnd w = Wnd.Find("[p]Dreamweaver");
		//Wnd w = Wnd.Find("app -*", "wndclass_desked_gsk");
		//Wnd w = Wnd.Find("* Notepad");
		//Out(w);

		//w.Activate(); Wait(1); //return;

		Wnd w = Wnd.Find("Registry*");
		//if(!w.ShowMinimized(Wnd._MinMaxMethod.Auto)) Out(ThreadError.ErrorText);
		//if(!w.ShowMinimized(Wnd._MinMaxMethod.LikeProgrammer)) Out(ThreadError.ErrorText);
		//if(!w.ShowMinimized(Wnd._MinMaxMethod.NoAnimation)) Out(ThreadError.ErrorText);
		//return;

		if(false) {
			w.ShowNotMinMax();
			Wait(1);
			w.ShowMaximized();
			return;

			w.ShowMinimized();
			//Out(w.IsMinimized);
			Out(Wnd.ActiveWindow);
			Wait(1);
			w.ShowNotMinimized();
			//Out(w.IsMinimized);
			Out(Wnd.ActiveWindow);

			//w.ShowMaximized();
			//w.ShowNotMinMax();
		} else {
			//var m =Wnd._MinMaxMethod.NoAnimation;
			var m = Wnd._MinMaxMethod.LikeProgrammer;

			//Out(w.ShowNotMinMax(m));
			//Wait(1);
			//Out(w.ShowMaximized(m));
			//return;

			Out(w.ShowMinimized(m));
			//Out(w.IsMinimized);
			Out(Wnd.ActiveWindow);
			Wait(1);
			Out(w.ShowNotMinimized(m));
			//Out(w.IsMinimized);
			Out(Wnd.ActiveWindow);

			//Out(w.ShowMaximized(m));
			//Out(w.ShowNotMinMax(m));
		}

		Out("ok");
	}

	static bool IsVisible(Wnd w)
	{
		ThreadError.Clear();
		return Api.IsWindowVisible(w) || ThreadError.SetWinError();
	}

	static void TestThreadError()
	{
		//Script.Option.speed = 10;
		var w = Wnd.FindRaw("QM_Editor");
		//Perf.ExecuteMulti(5, 1, ()=> { Time.AutoDelay(w); });
		Out(w);

		w = Wnd.Misc.SpecHwnd.Bottom;

		var a1 = new Action(() => { Api.GetCurrentThreadId(); });
		bool yes = false;
		var a2 = new Action(() => { yes = Api.IsWindow(w); });
		var a3 = new Action(() => { yes = Api.IsWindowVisible(w); });
		var a4 = new Action(() => { yes = w.Visible; });
		var a5 = new Action(() => { yes = IsVisible(w); });

		Perf.ExecuteMulti(5, 1, a1, a2, a3, a4, a5);


		//ThreadError.Set("Failed to activate window.");
		ThreadError.Set(5, "Failed to activate window.");
		//ThreadError.Set(5, "");
		//ThreadError.Set(5555, "Failed to activate window.");

		Exception e = ThreadError.Exception;
		//System.ComponentModel.Win32Exception e = ThreadError.Get() as System.ComponentModel.Win32Exception;
		if(e == null) Out("null");
		else {
			Out(e);
			Out(e.Message);
			Out(ThreadError.WinErrorCode);

			//try { throw e; } catch(Exception ee) { Out(ee); }
			//try { ThreadError.ThrowIfError(); } catch(Exception ee) { Out(ee); }
			//try { IsVisible(w) || ThreadError.ThrowIfError(); } catch(Exception ee) { Out(ee); } //cannot do it
			try { if(!IsVisible(w)) ThreadError.ThrowIfError(); } catch(Exception ee) { Out(ee); }

		}
	}

	static void TestWindowDimensions()
	{
		//Wnd w=Wnd.Find("Quick*", "QM_*");
		Wnd w = Wnd.Find("* Notepad");
		//Wnd w=Wnd.Find("Registry*");
		//Wnd w=Wnd.Find(null, "Dwm", flags:Wnd.WinFlag.HiddenToo);
		Out(w);

		//Out(w.MoveInScreen(0, 0));
		//Out(w.MoveToScreenCenter(2));

		//Out(w.Child("", "*Tree*", prop: new Wnd.ChildProp() { y=0.5 }));
		//Out(Wnd.Find("", "QM_*", prop: new Wnd.WinProp() { x=0.5 }));

		w.Activate();
		//w.MoveRaw(300, 100, 600, 200);
		//w.Width = 500;
		//w.MoveRaw(100, 30, 500, 300);
		//w.MoveRaw(100, 30, null, null);
		//w.MoveRaw(null, null, 500, 300);
		//w.MoveRaw(null, null, 500, null);
		//w.MoveRaw(null, null, null, 500);
		//w.MoveRaw(100, null, null, null);
		//w.MoveRaw(null, 100, null, null);
		//w.MoveRaw(null, 100, null, 300);
		//w.MoveRaw(300, 100);
		//w.MoveRaw(null, 100);
		//w.ResizeRaw(300, 100);
		//w.ResizeRaw(null, 100);
		//w.ResizeRaw(300, null);

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
		//Out(w.Height);
		//Out(w.IsNotMinMax);
		//Out(ThreadError.ErrorText);


		OutList(w.X, w.Y, w.Width, w.Height);
		OutList(w.ClientWidth, w.ClientHeight);

		//w.X = 500;
		//w.Y = 500;
		//w.Width = 500;
		//w.Height = 500;

		//RECT r = w.Rect;
		//r.Inflate(20, 20);
		//w.Rect = r;

		//RECT r = w.ClientRect;
		//Out(r);
		//r.Inflate(20, 20);
		//w.ClientRect = r;

		//w.ClientWidth = 300;
		//Out(w.ClientWidth);
		//w.ClientHeight = 300;
		//Out(w.ClientHeight);

		//RECT rw, rc;
		//if(w.GetWindowAndClientRectInScreen(out rw, out rc)) OutList(rw, rc);
	}

	static void TestWndtaskbarButton()
	{
		//Wnd w=Wnd.Find("Quick*", "QM_*");
		Wnd w = Wnd.Find("* Notepad");
		//Wnd w=Wnd.Find("Registry*");
		//Wnd w=Wnd.Find(null, "Dwm", flags:Wnd.WinFlag.HiddenToo);
		Out(w);

		//Wnd.Misc.TaskbarButton.Flash(w, 0);
		//Wait(3);
		//Wnd.Misc.TaskbarButton.Flash(w, 1);

		//Wnd.Misc.TaskbarButton.SetProgressState(w, Wnd.Misc.TaskbarButton.ProgressState.Error);
		//for(int i = 0; i < 100; i++) {
		//	Wnd.Misc.TaskbarButton.SetProgressValue(w, i + 1);
		//	WaitMS(100);
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
		Out(w);
		//Script.Speed = 200;
		Out(w.Close());
		//Wnd.CloseAll("*Notepad*");
	}

	static void TestWndArrange()
	{
		Wnd w = Wnd.Find("*Notepad");
		//Out(w);
		Wnd.Misc.Arrange.ShowDesktop();
		Out("ok");
		Wait(1);
		Wnd.Misc.Arrange.ShowDesktop();
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
		Wnd w = Wnd.FindH("*Notepad");

		var a1 = new Action(() => { vis = w.Visible; });
		var a2 = new Action(() => { Api.ShowWindow(w, vis ? 0 : Api.SW_SHOWNA); });
		//var a2 = new Action(() => { Api.ShowWindow(w, vis?0:Api.SW_SHOW); });
		var a3 = new Action(() => { Api.SetWindowPos(w, Wnd0, 0, 0, 0, 0, (uint)(vis ? Api.SWP_HIDEWINDOW : Api.SWP_SHOWWINDOW) | Api.SWP_NOMOVE | Api.SWP_NOSIZE | Api.SWP_NOZORDER | Api.SWP_NOOWNERZORDER | Api.SWP_NOACTIVATE); });
		//var a3 = new Action(() => { Api.SetWindowPos(w, Wnd0, 0, 0, 0, 0, (uint)(vis ? Api.SWP_HIDEWINDOW: Api.SWP_SHOWWINDOW)|Api.SWP_NOMOVE|Api.SWP_NOSIZE|Api.SWP_NOZORDER|Api.SWP_NOOWNERZORDER|Api.SWP_NOACTIVATE|Api.SWP_NOSENDCHANGING); });

		//Perf.ExecuteMulti(5, 1000, a1, a2);

		Wnd[] a = new Wnd[10];
		var f = new Form();
		for(int i = 0; i < 10; i++) {
			var b = new Button();
			b.Location = new POINT(0, i * 20);
			b.Size = new SIZE(50, 18);
			f.Controls.Add(b);
			a[i] = (Wnd)b.Handle; Out(a[i]);
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

		Out("---- int ----");

		ok = Registry_.SetInt(5, "ii", "Test");
		Out(ok);
		if(!ok) { Out(ThreadError.ErrorText); return; }

		int i;
		ok = Registry_.GetInt(out i, "ii", "Test");
		Out(ok);
		if(!ok) { Out(ThreadError.ErrorText); return; }
		Out(i);

		Out("---- long ----");

		ok = Registry_.SetLong(5, "LLL", "Test");
		Out(ok);
		if(!ok) { Out(ThreadError.ErrorText); return; }

		long L;
		ok = Registry_.GetLong(out L, "LLL", "Test");
		Out(ok);
		if(!ok) { Out(ThreadError.ErrorText); return; }
		Out(L);

		Out("---- string ----");

		ok = Registry_.SetString("stttttttttttrrrrrr", "SSS", "Test");
		Out(ok);
		if(!ok) { Out(ThreadError.ErrorText); return; }

		string s;
		ok = Registry_.GetString(out s, "SSS", "Test");
		Out(ok);
		if(!ok) { Out(ThreadError.ErrorText); return; }
		Out(s);

		Out("---- multi string ----");

		ok = Registry_.SetStringArray(new string[] { "one", "two", "three" }, "AAA", "Test");
		Out(ok);
		if(!ok) { Out(ThreadError.ErrorText); return; }

		string[] a;
		ok = Registry_.GetStringArray(out a, "AAA", "Test");
		Out(ok);
		if(!ok) { Out(ThreadError.ErrorText); return; }
		Out(a);

		Out("---- binary ----");

		var r = new RECT(1, 2, 3, 4, false);
		int n = Marshal.SizeOf(r);
		ok = Registry_.SetBinary(&r, n, "BB", "Test");
		Out(ok);
		if(!ok) { Out(ThreadError.ErrorText); return; }
		//ok=Registry_.SetBinary(&r, n, "rect2", @"HKEY_CURRENT_USER\Test");
		//Out(ok);
		//if(!ok) { Out(ThreadError.ErrorText); return; }
		//ok = Registry_.SetBinary(&r, n, "rect2", @"HKEY_LOCAL_MACHINE\Software\Test");
		//Out(ok);
		//if(!ok) { Out(ThreadError.ErrorText); return; }

		RECT r2;
		n = Registry_.GetBinary(&r2, n, "BB", "Test");
		Out(n);
		if(n <= 0) { Out(ThreadError.ErrorText); return; }
		Out(r2);
	}

	static void TestWndRegistrySaveRestore()
	{
		Wnd w = Wnd.FindH("*Notepad", "Notepad");
		Out(w);
		Out(w.RegistrySave("WndSR", "Test", true));
		Show.TaskDialog("move etc Notepad");
		w.Visible = false;
		Wnd.FindRaw("QM_Editor").Activate();
		Wait(0.2);
		Out(w.RegistryRestore("WndSR", "Test", true, true));
		Wait(1);
		w.Visible = true;
	}

	static void TestWndTransparency()
	{
		Wnd w = Wnd.FindH("*Notepad", "Notepad");
		//w = w.Child(15);
		Out(w);
		//Out(w.Transparency(true, null, 0));
		//Out(w.Transparency(true, 80));
		//Out(w.Transparency(true, 80, 0));
		//Out(w.Transparency(true));
		Out(w.Transparency(false));
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

		Out(d, "");
	}

	static void TestStrToI()
	{
		var s = "1234567";
		//var s = "0xffffffff";
		var c = s.ToCharArray();

		unsafe
		{
			fixed (char* p = c)
			{
				Out(s.ToInt_());
				char* t = p, e;
				Out(Api.strtoi(t, out e));
				Out(Api.strtoui(t, out e));
				Out(Api.strtoi64(t, out e));
				int n;
				Out(Api.strtoi(s, 0, out n)); Out(n);
				Out(Api.strtoui(s, 0, out n)); Out(n);
				Out(Api.strtoi64(s, 0, out n)); Out(n);

				var a1 = new Action(() => { s.ToInt_(); });
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
		//Out(Api.strtoi(s, 0, out len));
		//Out(len);
		//Out(Api.strtoi(s));

		//Out(Api.strtoi("0x7ffffffe"));
		//Out(Api.strtoi("0x8ffffffe"));
		//Out(Api.strtoi("-0x8ffffffe"));
		//OutHex(Api.strtoui("0xfffffffe"));
		//OutHex(Api.strtoui("0xffffffff1"));
		//OutHex(Api.strtoui("-2"));
		//OutHex(Api.strtoui("-0x7fffffff"));
		//OutHex(Api.strtoui("-0x80000000"));

		Out(Api.strtoi64("0x7ffffffffffffffe"));
		Out(Api.strtoi64("0x8000000000000000"));
	}

	static void TestRegexAgain()
	{
		//string p = @"(\d)\d+", r = "$1R";
		//string s = "aaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mmaaa 45 fff 877 mm";

		////Out(s.RegexReplace_(out s, p, r));
		////Out(s);

		//string s2 = null; int n = 0;
		//var a1 = new Action(() => { s2 = s.RegexReplace_(p, r); });
		//var a2 = new Action(() => { n = s.RegexReplace_(out s2, p, r); });

		//Perf.ExecuteMulti(5, 1000, a1, a2);

		//OutList(n, s2);

		//var k = new UNS.TDARRAY();
		////k.a = new int[5];
		//k[0] = 5;
		//Out(k[0]);
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
		//	OutList(sizeof(UNS.HASARRAY), Marshal.SizeOf(typeof(UNS.HASARRAY)));

		//	var k = new UNS.HASARRAY();

		//	Out(k.a[0]);
		//	k.a[0]=5;
		//	Out(k.a[0]);

		//	//Out(k.a); //error
		//	//Out(k.s); //error

		//	//char[] s = k.s; //error
		//	k.s[0] = 'A';
		//	string s = new string(k.s, 0, 5);
		//	Out(s);
	}

	[DllImport(@"Q:\app\Catkeys\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void TestSimple();

	[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void TestExceptions()
	{
		Out(1);
		try {
			TestSimple();
		}
		catch { Out("exc"); }
		Out(2);

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
		Out("fin");
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
		Out("fin");
	}

	static void TestWndGetIcon()
	{
		//Wnd w = Wnd.Find("*Notepad");
		Wnd w = Wnd.Find("Calculator");
		if(w.Is0) { //on Win8 cannot find window, probably must be uiAccess. Find in QM and copy-paste.
			string s;
			if(!Show.InputDialog(out s, "hwnd")) return;
			w = (Wnd)s.ToInt_();
		}
		Out(w);
		IntPtr hi16 = w.GetIconHandle();
		IntPtr hi32 = w.GetIconHandle(true);
		OutList(hi16, hi32);
		if(hi32 == Zero) return;
		var d = new TaskDialogObject("big icon", style: hi32);
		d.SetFooterText("small icon", hi16);
		d.Show();
		Api.DestroyIcon(hi16);
		Api.DestroyIcon(hi32);

		var a2 = new Action(() => { hi32 = w.GetIconHandle(true); Api.DestroyIcon(hi32); });
		Perf.ExecuteMulti(5, 10, a2);
	}

	static void TestFileIcon()
	{
		//IntPtr hi = Files._IconCreateEmpty(16, 16);

		string s;

		//s = @"q:\app\qm.exe,-133";
		//int i = Files._IconGetIndex(ref s);
		//OutList(i, s);
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

		IntPtr hi = Files.GetIconHandle(s, 32);
		//IntPtr hi = Files.GetIconHandle(Folders.VirtualITEMIDLIST.ControlPanelFolder, 32);
		Out(hi);
		if(hi == Zero) return;
		Show.TaskDialogEx("text", style: hi);
		Api.DestroyIcon(hi);
	}

	//static void TestCoord(int x, int y)
	//{
	//	Out("int");
	//}

	static void TestCoord(float x, float y)
	{
		OutList("float", x > 0.0 && x < 1.1);
	}

	//static void TestCoord(double x, double y)
	//{
	//	Out("double");
	//}

	static void TestCoord()
	{
		var w = Wnd.Find("*Notepad");
		w.Activate();
		w.Move(0.5f, 1f);
	}

	//static IEnumerable<int> TestYield()
	//{
	//	for(int i=0; i<5; i++) {
	//		int k = ToInt("5");
	//		yield return i+k;
	//	}
	//}

	[DllImport("user32.dll", EntryPoint = "CharUpperBuffW")]
	public static extern uint CharUpperBuff(string lpsz, uint cchLength);

	static unsafe void TestStringZeroTerm(string s)
	{
		fixed (char* p = s)
		{
			if((int)p[s.Length] != 0 || (int)p[s.Length + 1] != 0 || (int)p[s.Length + 2] != 0 || (int)p[s.Length + 3] != 0)
				OutList((long)p, (int)p[s.Length], (int)p[s.Length + 1], (int)p[s.Length + 2], (int)p[s.Length + 3]);
		}
	}

	static void TestStringMisc()
	{
		//Out(int.Parse("+16"));

		//TestStringZeroTerm("aaaaaaaa");
		//TestStringZeroTerm("aabaaaaa");
		//TestStringZeroTerm("aacaaaaa");
		//TestStringZeroTerm("aadaaaaa");
		//TestStringZeroTerm($"aa{1}aaaaa");
		//TestStringZeroTerm($"aa{2}aaaaa");

		//for(int i=1; i<100000000; i++) {
		//	TestStringZeroTerm(new string('a', i&0xff));
		//      }
		//Out("fin");

		//foreach(int i in TestYield()) {
		//	Out(i);
		//}
		//Out("fin");

		//string s = "abc";
		//fixed(char* p= s)
		//{
		//	p[0] = 'A';
		//}
		//Out(s);

		//string s = "abc";
		//CharUpperBuff(s, 2);
		//Out(s);

		OutList(int.MaxValue, uint.MaxValue, long.MaxValue, ulong.MaxValue);

		string s;
		s = "mm  18446744073709551615 kk";
		//s = "mm  9999999999999999999 kk";
		//s = "mm  0xFfffffffffffffff kk";

		s = "mm  4294967295 kk";
		//s = "mm  999999999 kk";
		//s = "mm  0xFfffffff kk";

		int iEnd;
		int R = s.ToInt_(2, out iEnd);
		uint u = (uint)R;
		//long R = s.ToInt64_(2, out iEnd);
		//ulong u = (ulong)R;
		OutList(s, R, u, iEnd);


		//int i1 = 0, i2 = 0, i3 = 0, i4=0;

		////i3 = ToInt(s);

		//Perf.SpinCPU(200);
		//Action a1 = new Action(() => { i1 = int.Parse(s); });
		//Action a2 = new Action(() => { i2 = s.ToInt_(); });
		//Action a3 = new Action(() => { i3 = ToInt(s); });
		//Action a4 = new Action(() => { i4 = ToInt2(s); });

		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);
		//OutList(i1, i2, i3, i4);

	}

	static void TestWndMisc()
	{
		Wnd w;
		Wnd w2 = Wnd.Find("Quick*");
		//w = Wnd.Get.DesktopWindow;
		//Out(w);
		//w = Wnd.Get.Desktop;
		//Out(w);
		//w = Wnd.Get.DesktopListview;
		//Out(w);
		Out(w2.IsTopmost);

		Action a1 = new Action(() => { w = Wnd.Get.DesktopWindow; });
		Action a2 = new Action(() => { w = Wnd.Get.Desktop; });
		Action a3 = new Action(() => { w = Wnd.Get.DesktopListview; });
		Action a4 = new Action(() => { bool fs = w2.IsFullScreen; });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		//w = Wnd.Find("Quick*");
		//w = Wnd.Find("My QM");
		//w = Wnd.Get.Desktop;
		//w = Wnd.Get.DesktopListview;
		//w = Wnd.Find("* Firefox");
		//w = Wnd.Get.DesktopWindow;
		////w = Wnd.Get.ShellWindow;
		//w = Wnd.Find("*- Google Chrome");
		//Out(w);
		////OutList(w.IsOfShellThread, w.IsOfShellProcess);
		//Out(w.IsFullScreen);

		//while(true) {
		//	Wait(1);
		//	w = Wnd.FromMouse(false);
		//	if(w.ClassNameIs("WorkerW")) break;
		//	Out(w);
		//	OutList(w.IsFullScreen, w.Rect);
		//}
	}

	#region TestWndRegisterClass
	static Wnd.Misc.RegisterClass _wndRC, _wndRCSuper;
	static void TestWndRegisterClass()
	{
		_wndRC = new Wnd.Misc.RegisterClass();
		_wndRC.Register("Cat_Test", Cat_Test_WndProc, IntPtr.Size);
		//_wndRC.Register("Cat_Test", Cat_Test_WndProc, IntPtr.Size, Api.CS_GLOBALCLASS);
		Wnd w = Api.CreateWindowEx(0, "Cat_Test", "Cat_Test", Api.WS_OVERLAPPEDWINDOW | Api.WS_VISIBLE, 300, 100, 300, 200, Wnd0, 0, Zero, 0);
		if(w.Is0) return;

		//_wndRCSuper = new Wnd.RegisterClass();
		////_wndRCSuper.Superclass("Cat_Test", "Cat_Test_Super", Cat_Test_WndProcSuper, IntPtr.Size);
		//_wndRCSuper.Superclass("Cat_Test", "Cat_Test_Super", Cat_Test_WndProcSuper, IntPtr.Size, false, Api.GetModuleHandle(null));
		//Wnd w2 = Api.CreateWindowEx(0, "Cat_Test_Super", "Cat_Test_Super", Api.WS_OVERLAPPEDWINDOW | Api.WS_VISIBLE, 300, 500, 300, 100, Wnd0, 0, Zero, 0);
		//if(w2.Is0) return;

		_wndRCSuper = new Wnd.Misc.RegisterClass();
		_wndRCSuper.Superclass("Edit", "Edit_Super", Cat_Test_WndProcSuper, IntPtr.Size);
		Wnd w2 = Api.CreateWindowEx(0, "Edit_Super", "Edit_Super", Api.WS_CHILD | Api.WS_VISIBLE, 0, 0, 200, 30, w, 3, Zero, 0);
		if(w2.Is0) return;

		//Out(Wnd.RegisterClass.GetClassAtom("Cat_Test", Api.GetModuleHandle(null)));

		//Api.SetTimer(w, 1, 1000, null);

		Api.MSG m;
		while(Api.GetMessage(out m, Wnd0, 0, 0) > 0) {

			//if(m.message == Api.WM_TIMER && m.hwnd==w) {
			//	Out("timer");
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
			Out(_wndRC.GetMyLong(w));
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
			Out(_wndRCSuper.GetMyLong(w));
			break;
		}

		return Api.CallWindowProc(_wndRCSuper.BaseClassWndProc, w, msg, wParam, lParam);
	}
	#endregion

	static void TestIsGhost()
	{
		Wnd w = Wnd.Find("Hung");
		Out(w);
		Out(w.IsHung);
		w = Wnd.Find("Hung*", "Ghost");
		Out(w);
		OutList(w.IsHung, w.IsHungGhost);

		//bool y;
		//var a1 = new Action(() => { y = w.ClassNameIs("Ghost"); });
		//var a2 = new Action(() => { y = w.ProcessName.Equals_("DWM", true); });
		//var a3 = new Action(() => { y = w.IsHung; });
		//Perf.ExecuteMulti(5, 100, a1, a2, a3);

		//Perf.SpinCPU(100);
		//var a = Wnd.AllWindows(null, true);
		//foreach(Wnd t in a) {
		//	Out(t);
		//	Perf.First();
		//	y = t.IsHung;
		//	Perf.Next();
		//	y=t.ClassNameIs("Ghost");
		//	Perf.NextWrite();
		//}
	}

	static void TestWndGetPropList()
	{
		foreach(Wnd w in Wnd.AllWindows(null, true)) {
			Out(w);
			foreach(var k in w.GetPropList()) {
				Out(k);
			}
			Out("---------");
		}
	}

	static void TestWndMapPoints()
	{
		Wnd w = Wnd.Find("", "QM_Editor");
		Out(w);
		RECT r = new RECT(1, 2, 3, 4, false);
		RECT rr;
		rr = r; w.MapClientToScreen(ref rr);
		Out(rr);
		rr = r; w.MapScreenToClient(ref rr);
		Out(rr);

		Wnd c = w.Child("", "QM_Code");
		Out(c);
		//rr = c.Rect;
		rr = r; c.MapClientToClientOf(w, ref rr);
		Out(rr);

		c.GetRectInClientOf(w, out rr);
		Out(rr);

		Out(Wnd.FromXY(1460, 1400));

		//rr = r; c.MapClientToClientOf(w, ref rr);
		//Out(rr);
		//rr = r; c.MapClientToClientOf(w, ref rr);
		//Out(rr);
		POINT p = new POINT(1, 2), pp;
		pp = p; w.MapClientToWindow(ref pp);
		Out(pp);
		pp = p; w.MapWindowToClient(ref pp);
		Out(pp);

		rr = r; w.MapClientToWindow(ref rr);
		Out(rr);
		rr = r; w.MapWindowToClient(ref rr);
		Out(rr);

		Out(Wnd.FromMouse().MouseClientXY);
	}

	static void TestWndIsAbove()
	{
		Wnd w1 = Wnd.Find("", "QM_Editor");
		Wnd w2 = Wnd.Find("", "CabinetWClass");
		OutList(w1.IsZorderedBefore(w2), w2.IsZorderedBefore(w1));
	}

	static void TestWndSetParent()
	{
		//note: SetParent removed, it did not work well, anyway probably will need to change parent only of form controls, then Form.Parent should be used and works well, even with topmost windows.

		//Wnd w1 = Wnd.Find("", "QM_Editor");
		//Wnd w2 = w1.Child("Running items");
		//Wnd w3 = w2.DirectParentOrOwner;
		//Out(w3);

		//OutList("SetParent", w2.SetParent(Wnd0));
		//Show.TaskDialog("");
		//OutList("SetParent", w2.SetParent(w3));

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
			//Out(b.Handle);

			b.Parent = f2;
			//Out(b.Handle);
			//Wnd w = (Wnd)b;
			//w.SetParent(Wnd0, true);
			Show.TaskDialog("");
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
		Out(w);
		Out(Wnd.Misc.BorderWidth(w));
		Out(Wnd.Misc.BorderWidth(w.Style, w.ExStyle));

		RECT r = w.ClientRect;
		Out(r);
		Wnd.Misc.WindowRectFromClientRect(ref r, w.Style, w.ExStyle, false);
		Out(r);
	}

	static void TestWndStoreApp()
	{
		foreach(Wnd w in Wnd.AllWindows()) {
			bool isWin8Metro = w.IsWin8MetroStyle;
			int isWin10StoreApp = w.IsWin10StoreApp;
			string prefix = null, suffix = null;
			if(isWin8Metro || isWin10StoreApp != 0) { prefix = isWin8Metro ? "<><c 0xff>" : "<><c 0xff0000>"; suffix = "</c>"; }
			Out($"{prefix}metro={isWin8Metro}, win10={isWin10StoreApp}, cloaked={w.IsCloaked},    {w.ProcessName}  {w}  {w.Rect}{suffix}");
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
				Out(c == null); //False
			} else {
				Wnd t = Api.CreateWindowEx(0, "Edit", "Edit", Api.WS_CHILD | Api.WS_VISIBLE, 0, 0, 200, 30, (Wnd)f, 3, Zero, 0);
				if(t.Is0) return;
				var c = (Control)t;
				Out(c == null); //True
								//Out(c.Bounds);
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
		Out(w);
		RECT r;

		r = w.Rect;
		Out(r);
		//w.Rect = new RECT(100, 1300, 300, 500, true);
		//w.Rect = new RECT(100, 320, 80, 50, true);

		r = w.ClientRect;
		Out(r);
		//r.Inflate(2, 2); w.ClientRect=r;

		//r =w.ClientRectInScreen;
		//Out(r);
		//w.ClientRectInScreen = new RECT(100, 320, 80, 50, true);

		//r.Inflate(2, 2);
		//w.ClientRectInScreen = r;

		r = w.RectInParent;
		Out(r);
	}

	static void TestWndMoveMisc()
	{
		var w = Wnd.Find("*Notepad");
		//w.Move(200, 1500, 400, 200);

		//w.MoveToScreenCenter(2);
		w.MoveInScreen(-100, -20, 2);
		w.ActivateRaw();
	}

	static void _TestWndZorder(Wnd w, Wnd w2)
	{
		//Out(w.ZorderTop());
		//Out(w.ZorderBottom());

		//Out(w.ZorderTopmost());
		////Show.TaskDialog("");
		//Wait(1);
		//Out(w.ZorderNotopmost(true));
		////Out(w.ZorderBottom());

		Out(w.ZorderAfter(w2));
		w2.ActivateRaw(); WaitMS(500);
		Out(w.ZorderBefore(w2));

	}

	static void TestWndZorder()
	{
		var w = Wnd.Find("*Notepad");
		//var w = Wnd.Find("*WordPad");
		//var w = Wnd.Find("Options");
		//var w = Wnd.Find("Font");
		//var w = Wnd.Find("", "QM_Editor");
		var w2 = Wnd.Find("* - Paint");
		//OutList(w, w2);

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
			if(!Show.InputDialog(out i, "Handle")) return;
			w = (Wnd)i;
		}

		Out(w);
		Out(w.IsUacAccessDenied);
		//OutList(w.IsUacAccessDenied, w.IsUacAccessDenied2);
		//Out(w.SetProp("abc", 1)); //fails
		//Out(w.RemoveProp("abcde"));
		//Out(w.SetWindowLong());
		//Out(w.)


		//var a1 = new Action(() => { bool b = w.IsUacAccessDenied; });
		//var a2 = new Action(() => { bool b = w.IsUacAccessDenied2; });
		//Perf.ExecuteMulti(5, 1000, a1, a2);
	}

	static void TestTaskDialogActivationEtc()
	{
		Wait(3);
		//Script.Option.dialogTopmostIfNoOwner = true;
		//Script.Option.dialogAlwaysActivate = true;
		//Out(Wnd.AllowActivate());
		//Out(Wnd.Find("*Notepad").ActivateRaw());
		//Wait(1);
		//Out(Wnd.ActiveWindow);
		Show.TaskDialog("test", "", "i");
		//Wait(3);
		//Script.Option.dialogAlwaysActivate = false;
		//Show.TaskDialog("test", "", "a");
	}

	static void TestWndNextMainWindow()
	{
		int f = Show.ListDialog("1 default|2 allDesktops|3 likeAltTab|4 retryFromTop|5 skipMinimized");
		if(f == 0) return;
		Wnd w = Wnd.Get.FirstToplevel();
		int n = 0;
		while(!w.Is0) {
			Out(w);
			switch(f) {
			case 1:
				w = Wnd.Get.NextMainWindow(w);
				break;
			case 2:
				w = Wnd.Get.NextMainWindow(w, allDesktops: true);
				break;
			case 3:
				w = Wnd.Get.NextMainWindow(w, likeAltTab: true);
				break;
			case 4:
				w = Wnd.Get.NextMainWindow(w, retryFromTop: true); if(++n > 20) return;
				break;
			case 5:
				w = Wnd.Get.NextMainWindow(w, skipMinimized: true);
				break;
			}
		}
	}

	static void TestWndFindRaw()
	{
		Wnd w = Wnd0;
		for(;;) {
			w = Wnd.FindRaw("QM_Toolbar", null, w);
			if(w.Is0) break;
			Out(w);
		}
		Out(Wnd.FindRaw("QM_Editor").ChildRaw("QM_Scc"));
	}

	static void TestWndGetGUIThreadInfo()
	{
		Wait(3);
		Out(Wnd.FocusedControl);
		RECT r;
		Out(Input.GetTextCursorRect(out r));
		Out(r);
	}

	static class TestStaticInitClass
	{
		static int _x = _InitX();
		static int _InitX() { OutFunc(); return 1; }
		public static int GetX()
		{
			OutFunc();
			return _x;
		}
		public static int Other()
		{
			OutFunc();
			return -1;
		}
	}

	static void TestStaticInit1()
	{
		OutFunc();
		Out(TestStaticInitClass.GetX());
	}

	static void TestStaticInit2()
	{
		OutFunc();
		Out(TestStaticInitClass.Other());
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
		//Out(s);
		////s = Calc.BytesToHexString2(b);
		////Out(s);
		//s = BytesToHexString_BitConverter(b);
		//Out(s);

		//var a1 = new Action(() => { s = BytesToHexString_BitConverter(b); });
		//var a2 = new Action(() => { s = Calc.BytesToHexString(b); });
		////var a3 = new Action(() => { s = Calc.BytesToHexString2(b); });
		//Perf.ExecuteMulti(5, 1000, a1, a2);


		//var b = new byte[] { 0, 1, 0xA, 0xF, 0xBE, 0x59, 0, 0xff, 0x58, 0xD7, 0, 1, 0xA, 0xF, 0xBE, 0x59, 0, 0xff, 0x58, 0xD7 };
		//var s = Calc.BytesToHexString(b);
		//Out(s);
		//Out(Calc.BytesToHexString(Calc.BytesFromHexString(s)));
		//string sB64 = Convert.ToBase64String(b);
		//Out(sB64);
		//Out(Convert.ToBase64String(Convert.FromBase64String(sB64)));

		//var a1 = new Action(() => { Calc.BytesToHexString(b); });
		//var a2 = new Action(() => { Calc.BytesFromHexString(s); });
		//var a3 = new Action(() => { Convert.ToBase64String(b); });
		//var a4 = new Action(() => { Convert.FromBase64String(sB64); });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);


		string s = @"Q:\app\catkeys\tasks\CatkeysTasks.exe";
		string hash;
		Perf.First();
		hash = Calc.HashMD5Hex(s);
		Perf.NextWrite(); //1700 (.NET 3700)
		Out(hash);
		hash = Calc.HashHex(s, "MD5");
		Out(hash);
		hash = Calc.HashHex(s, "SHA256");
		Out(hash);
		int hashInt = Calc.HashFnv1(s);
		Out(hashInt);
		unsafe { fixed (char* p = s) { hashInt = Calc.HashFnv1(p, s.Length); } }
		Out(hashInt);
		unsafe { fixed (char* p = s) { hashInt = Calc.HashFnv1((byte*)p, s.Length * 2); } }
		Out(hashInt);

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
		//WaitMS(1);
		//Perf.Next();
		//WaitMS(10);
		//Perf.NextWrite();

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

	#endregion

	public class CatMenu :ContextMenuStrip
	{
		public CatMenu() : base()
		{
		}

		public CatMenu(System.ComponentModel.IContainer container) : base(container)
		{
		}

		/// <summary>
		/// Gets ToolStripItem of the last added item.
		/// For submenu-item you can cast it to ToolStripMenuItem if need.
		/// </summary>
		public ToolStripItem LastItem { get; private set; }
		//public ToolStripMenuItem CurrentSubmenu { get { return _submenuStack.Peek(); } } //can instead use LastItem or SubmenuBlock.Submenu

		public ItemHandler this[string name, object icon = null, bool disabled = false, string tooltip = null]
		{
			set { Add(name, value, icon, disabled, tooltip); }
		}

		public ToolStripItem Add(string name, ItemHandler code, object icon = null, bool disabled = false, string tooltip = null)
		{
			var item = _Items.Add(name, null, _eh);
			item.Tag = code;
			_SetItemProp(item, icon, disabled, tooltip);
			LastItem = item;
			return item;
		}

		void _SetItemProp(ToolStripItem item, object icon, bool disabled, string tooltip)
		{
#if true
			if(icon != null) {
				var s = icon as string;
				if(s != null) {
					if(s != "") {
						bool setSubmenuIL = false;
						var il = item.Owner.ImageList;
						if(il == null) {
							il = this.ImageList;
							if(il != null) setSubmenuIL = true;
						}
						if(il != null && il.Images.ContainsKey(s)) {
							item.ImageKey = s;
							if(setSubmenuIL) item.Owner.ImageList = il;
						} else {
							//TODO: extract on demand, eg for submenus or offscreen items
							IntPtr hi = Files.GetIconHandle(s);
							if(hi != Zero) {
								Icon ic = Icon.FromHandle(hi);
								item.Image = ic.ToBitmap();
								ic.Dispose();
								Api.DestroyIcon(hi); //note: fails if this is immediately after 'Icon.FromHandle(hi)', although MSDN says need to call DestroyIcon() which implies that FromHandle() copies it.
							}
						}
					} else if(icon is int) {
						int i = (int)icon;
						if(i >= 0) {
							item.ImageIndex = i;
							if(item.Owner.ImageList == null) item.Owner.ImageList = this.ImageList; //if submenu ImageList not set, use common ImageList for it
						}
					} else if(icon is Image) {
						item.Image = icon as Image;
					} else if(icon is Icon) {
						item.Image = (icon as Icon).ToBitmap();
					} else if(icon is IntPtr) {
						item.Image = Icon.FromHandle((IntPtr)icon).ToBitmap();
					} else {
						throw new ArgumentException("", "icon");
					}
				}
			}
#endif
			if(disabled) item.Enabled = false;
			if(tooltip != null) item.ToolTipText = tooltip;
		}

		/// <summary>
		/// Adds item of any type, for example ToolStripLabel, ToolStripTextBox, ToolStripComboBox, ToolStripProgressBar, ToolStripButton.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="icon"></param>
		/// <param name="disabled"></param>
		/// <param name="tooltip"></param>
		/// <param name="code"></param>
		public void Add(ToolStripItem item, object icon = null, bool disabled = false, string tooltip = null, ItemHandler code = null)
		{
			_Items.Add(item);
			if(code != null) item.Tag = code;
			_SetItemProp(item, icon, disabled, tooltip);
			//activate menu window on click, or something may not work, eg cannot enter text in Edit control
			if(!(item is ToolStripMenuItem || item is ToolStripLabel || item is ToolStripSeparator || item is ToolStripProgressBar)) {
				item.MouseDown += _Item_MouseDown;
			}
			LastItem = item;
		}

		void _Item_MouseDown(object sender, MouseEventArgs e)
		{
			//OutFunc();
			var t = sender as ToolStripItem;
			var w = (Wnd)t.Owner.Handle;
			w.ActivateRaw();
		}

		public ToolStripSeparator AddSeparator()
		{
			var item = new ToolStripSeparator();
			_Items.Add(item);
			LastItem = item;
			return item;
		}

		public SubmenuBlock Submenu(string name, object icon = null, bool disabled = false, string tooltip = null, ItemHandler code = null)
		{
			var item = Add(name, code, icon, disabled, tooltip);
			var sm = item as ToolStripMenuItem;
			_submenuStack.Push(sm);
			sm.DropDown.HandleCreated += _DropDown_HandleCreated;
			return new SubmenuBlock(this, sm);
		}

		public void EndSubmenu()
		{
			if(_submenuStack.Count > 0) _submenuStack.Pop();
		}

		public class SubmenuBlock :IDisposable
		{
			CatMenu _menu;
			public readonly ToolStripMenuItem Submenu;

			public SubmenuBlock(CatMenu menu, ToolStripMenuItem submenu) { _menu = menu; Submenu = submenu; }
			//public static implicit operator ToolStripMenuItem(SubmenuBlock smb) { return smb.Submenu; } //no, then does not work with using()

			public void Dispose() { _menu.EndSubmenu(); }
		}

		Stack<ToolStripMenuItem> _submenuStack = new Stack<ToolStripMenuItem>();

		ToolStripItemCollection _Items
		{
			get { return _submenuStack.Count > 0 ? _submenuStack.Peek().DropDownItems : Items; }
		}

		private void _DropDown_HandleCreated(object sender, EventArgs e)
		{
			var sm = sender as ToolStripDropDown;
			_SetWindowStyle((Wnd)sm.Handle);
		}

		static EventHandler _eh = _EventHandler;

		static void _EventHandler(object sender, EventArgs args)
		{
			var k = sender as ToolStripItem;
			var t = k.Tag as ItemHandler;
			if(t != null) t(new ItemData(k));

			//TODO: this is also called on right-click (not on middle-click)
		}

		public class ItemData
		{
			public readonly ToolStripItem MenuItem;

			public ItemData(ToolStripItem item) { MenuItem = item; }
		}

		public delegate void ItemHandler(ItemData d);

		public new void Show()
		{
			_Show(0);
		}

		public new void Show(Point screenLocation)
		{
			_Show(1, screenLocation);
		}

		public new void Show(Control control, Point position)
		{
			_Show(2, position, 0, control);
		}

		public new void Show(int x, int y)
		{
			_Show(3, new Point(x, y));
		}

		public new void Show(Point position, ToolStripDropDownDirection direction)
		{
			_Show(4, position, direction);
		}

		public new void Show(Control control, Point position, ToolStripDropDownDirection direction)
		{
			_Show(5, position, direction, control);
		}

		public new void Show(Control control, int x, int y)
		{
			_Show(6, new Point(x, y), 0, control);
		}

		void _Show(int overload, Point p = default(Point), ToolStripDropDownDirection direction = 0, Control control = null)
		{
			Wnd w = (Wnd)Handle; //creates handle
			_SetWindowStyle(w);
			Perf.Next();

			switch(overload) {
			case 0: base.Show(Mouse.XY); break;
			case 1: base.Show(p); break;
			case 2: base.Show(control, p); break;
			case 3: base.Show(p.X, p.Y); break;
			case 4: base.Show(p, direction); break;
			case 5: base.Show(control, p, direction); break;
			case 6: base.Show(control, p.X, p.Y); break;
			}
			Perf.NextWrite();

			//w.ActivateRaw(); //then keyboard works, eg arrows/Enter to select, Esc to close

			if(!NonModal) {
#if true
				_idTimer = Api.SetTimer(Wnd0, w.Handle, 100, null);

				Api.MSG u;
				while(Api.GetMessage(out u, Wnd0, 0, 0) > 0) {
					//100 ms timer
					if(u.message == Api.WM_TIMER && u.wParam == _idTimer && u.hwnd == Wnd0) {
						if(u.lParam == 1) break; //posted from OnClosed
						if(!w.IsValid) break; //when menu window closed from outside, OnClosedOnClosed isn't called and even m.Visible is true etc

						//TODO: finally test GC:
						//GC.Collect();
						continue;
					}
					//if(u.message != Api.WM_TIMER && u.message != Api.WM_MOUSEMOVE) Out(u.message);
					Api.TranslateMessage(ref u);
					Api.DispatchMessage(ref u);
				}

				Api.KillTimer(Wnd0, _idTimer);
#else
				//Application.Run();
#endif
			}
		}

		public bool NonModal { get; set; }

		void _SetWindowStyle(Wnd w)
		{
			w.SetExStyleAdd(Api.WS_EX_NOACTIVATE); //prevents click-activation and adding taskbar button
		}

		LPARAM _idTimer;

		protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
		{
			Wnd0.Post(Api.WM_TIMER, _idTimer, 1);
			base.OnClosed(e);
		}
	}

	static void TestCatMenu()
	{
		//TODO: sometimes a submenu does not open at first.
		//TODO: 'wait' cursor when opening a submenu. Also when mouse enters the main context menu. Also when showing main.

		//Application.EnableVisualStyles();

		Perf.First();
		var m = new CatMenu();

		var il = new ImageList();
		IntPtr hi = Files.GetIconHandle(@"q:\app\browse.ico");
		//il.Images.Add("k1", Icon.FromHandle(hi));
		il.Images.Add("k0", Icon.FromHandle(hi).ToBitmap());
		Api.DestroyIcon(hi);
		//il.Images.Add(SystemIcons.Exclamation); //distorted
		il.Images.Add(new Icon(SystemIcons.Exclamation, 16, 16)); //distorted, the same
																  //il.Images.Add(Catkeys.Tasks.Properties.Resources.qm_running); //distorted, as well as with ToBitmap(), because the resource manager adds big icon
		m.ImageList = il;

		m["One"] = o => Out("-one-");
		m["Two"] = o => { Out(o.MenuItem); };
		m.Submenu("Sub");
		{
			m["Three"] = o => Out("-three-");
			m["Four"] = o => Out(o.MenuItem);
			m.Submenu("Sub2", code: o => Out(o.MenuItem));
			{
				m["Five"] = o => Out(o.MenuItem);
				m.EndSubmenu();
			}
			m.Submenu("Sub2", code: o => Out(o.MenuItem));
			{
				m["Five"] = o => Out(o.MenuItem);
				m.EndSubmenu();
			}
			m["Six"] = o => Out(o.MenuItem);
			m.LastItem.ForeColor = Color.BlueViolet;
			m.EndSubmenu();
		}
		using(m.Submenu("Sub with using")) {
			m["Three"] = o => Out("-three-");
			m.LastItem.Font = new Font(m.LastItem.Font, FontStyle.Bold);
			m["Four"] = o => Out(o.MenuItem);
			m.LastItem.Font = new Font("Tahoma", 25);
			using(m.Submenu("Sub2", code: o => Out(o.MenuItem))) {
				m["Five"] = o => Out(o.MenuItem);
			}
			m.Submenu("Sub2", code: o => Out(o.MenuItem));
			{
				m["Five"] = o => Out(o.MenuItem);
				m.EndSubmenu();
			}
			using(var smb = m.Submenu("Sub with new tooltip", code: o => Out(o.MenuItem))) {
				smb.Submenu.ToolTipText = "new tooltip";
				m["Five"] = o => Out(o.MenuItem);
			}
			m["Six"] = o => Out(o.MenuItem);
		}
		m["Disabled", disabled: true] = null;
		m["Tooltip", tooltip: "ttttt"] = null;
		m.Add("Method", o => Out(o.MenuItem));
		var mi = m.Add("Method2", o => Out(o.MenuItem)); mi.BackColor = Color.AliceBlue; mi.ForeColor = Color.Orchid;
		m["Icon", @"q:\app\Cut.ico"] = o => Out(o.MenuItem);
		m["Icon", @"q:\app\Copy.ico"] = o => Out(o.MenuItem);
		m["Icon", @"q:\app\Paste.ico"] = o => Out(o.MenuItem);
		m["Icon", @"q:\app\Run.ico"] = o => Out(o.MenuItem);
		m["Icon", @"q:\app\Tip.ico"] = o => Out(o.MenuItem);
		//m["Icon resource", 1] = o => Out(o.MenuItem);
		m["Imagelist icon name", "k0"] = o => Out(o.MenuItem);
		m["Imagelist icon index", 1] = o => Out(o.MenuItem);
		using(m.Submenu("Sub3")) {
			m.LastItem.ForeColor = Color.Red;
			//m.LastItem.DropDown.ImageList = il;
			//m.LastItem.Margin=new Padding(8);
			m["Simple"] = o => Out(o.MenuItem);
			m["Icon in submenu", @"q:\app\Paste.ico"] = o => Out(o.MenuItem);
			m["Imagelist icon name in submenu", "k0"] = o => Out(o.MenuItem);
			m["Imagelist icon index in submenu", 1] = o => Out(o.MenuItem);
			using(m.Submenu("Sub4", "k0")) {
				m.LastItem.BackColor = Color.Bisque;
				m["Simple"] = o => Out(o.MenuItem);
			}
			m.Submenu("Sub5", 1);
			{
				m["Simple"] = o => Out(o.MenuItem);
				m.EndSubmenu();
			}
		}
#if false
		m.AddSeparator();
		//if(item is ToolStripTextBox || item is ToolStripDropDown || item is ToolStripComboBox || item is ToolStripControlHost || item is ToolStripButton )
		m.Add(new ToolStripLabel("Label"));
		m.Add(new ToolStripTextBox("txt"));
		m.Add(new ToolStripComboBox("cb"));
		m.Add(new ToolStripProgressBar("pb"));
		m.Add(new ToolStripButton("Button"));
		m.Add(new ToolStripDropDownButton("DD button"));
		m.Add(new ToolStripSplitButton("Split button"));
		m.Add(new ToolStripStatusLabel("Status label"));
		m.Add(new ToolStripMenuItem("Menu item"));
		m.Add(new ToolStripSeparator());

		//this code works, but the control width is several pixels
		//var ed =new TextBox();
		//ed.Width = 100;
		//var host =new ToolStripControlHost(ed, "host");
		//host.Width = 100;
		//m.Add(host);

		//test overflow
		//for(int i=0; i<30; i++) m[$"Overflow {i}"] = o => Out(o.MenuItem);
#endif
		m["Last"] = o => { Out(o.MenuItem); };
		Perf.Next();

		Wait(1);
		Out(1);
		Perf.Next();
		m.Show(500, 300);
		//m.Show();
		//m.Show(Mouse.X + 10, Mouse.Y + 10);
		Out(2);
		//m.Show(new Point(600, 400));
		//Out(3);
		m.Dispose();
	}

	static void TestCatMenuSimplest()
	{
		var m = new CatMenu();
		m["One"] = o => Out("one");
		m["Two"] = o => Out("two");
		using(m.Submenu("Submenu")) {
			m["Three"] = o => Out("three");
			m["Four"] = o => Out("four");
			for(int i = 0; i < 30; i++) m[$"More {i}"] = o => Out(o.MenuItem);
		}
		m.Show();
	}

	static void TestCatMenuSpeed()
	{
		bool suspend = true; //makes faster 37 -> 15 ms (26 if ResumeLayout(true))
		var speed = new Perf.Inst();
		speed.First();
		//System.ComponentModel.IContainer components = new System.ComponentModel.Container();
		Perf.First();
		var m = new CatMenu();
		//var m = new CatMenu(components);
		Perf.Next();
		if(suspend) m.SuspendLayout();
		Perf.Next();
		//m["One"] = o => Out("one");
		m.Items.Add("text");
		Perf.Next();
		//m["Two"] = o => Out("two");
		//m.Items.Add("text");
		for(int i = 0; i < 30; i++) m[$"More {i}"] = o => Out(o.MenuItem);
		Perf.Next();
		using(var sm=m.Submenu("Submenu")) {
			if(suspend) sm.Submenu.DropDown.SuspendLayout();
			m["Three"] = o => Out("three");
			m["Four"] = o => Out("four");
			Perf.Next();
			for(int i = 0; i < 30; i++) m[$"More {i}"] = o => Out(o.MenuItem);
			Perf.Next();
			if(suspend) sm.Submenu.DropDown.ResumeLayout(false);
		}
		Perf.Next();
		if(suspend) m.ResumeLayout(false);
		Perf.Next();
		speed.NextWrite();
		Wait(1);
		Perf.Next();
		//m.Show();
		m.Show(Mouse.X + 10, Mouse.Y + 10);
	}

	static void TestCatMenuArray()
	{
		Perf.First();
		var m = new CatMenu();
		Perf.Next();
		var a = new ToolStripItem[30];
		for(int i = 0; i < a.Length; i++) a[i] = new ToolStripMenuItem("text");
		Perf.Next();
		m.Items.AddRange(a);
		Perf.Next();
		using(m.Submenu("Submenu")) {
			m["Three"] = o => Out("three");
			m["Four"] = o => Out("four");
		}
		Perf.Next();
		Wait(1);
		Perf.Next();
		//m.Show();
		m.Show(Mouse.X + 10, Mouse.Y + 10);
	}

	public partial class Form1 :Form
	{
		System.ComponentModel.IContainer components;
		CatMenu contextMenuStrip1;

		public Form1()
		{
			Perf.First();
			this.components = new System.ComponentModel.Container();
			this.contextMenuStrip1 = new CatMenu(this.components);
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();

			this.contextMenuStrip1.ResumeLayout(false);
			this.contextMenuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

			this.Click += Form1_Click;
			Perf.NextWrite();
		}

		private void Form1_Click(object sender, EventArgs e)
		{
			Perf.First();
			//var m = new CatMenu();
			var m = contextMenuStrip1;
			m["One"] = o => Out("one");
			Perf.Next();
			m["Two"] = o => Out("two");
			for(int i = 0; i < 30; i++) m[$"More {i}"] = o => Out(o.MenuItem);
			using(m.Submenu("Submenu")) {
				m["Three"] = o => Out("three");
				m["Four"] = o => Out("four");
			}
			Perf.NextWrite();
			m.NonModal = true;
			//m.Show();
			//m.Show(Mouse.X + 10, Mouse.Y + 10);
			m.Show(this, 0, 0);
		}
	}

	static void TestCatMenuWithForm()
	{
		Application.Run(new Form1());
	}

	public unsafe Test()
	{
		//Wait(1);
		//TestCatMenu();
		//TestCatMenuSimplest();
		//TestCatMenuArray();
		TestCatMenuSpeed();
		//TestCatMenuWithForm();

		#region call_old_test_functions
		//TestNewPerf();
		//TestHash();
		//OutFunc(); if((Time.Milliseconds&0x4)==0) TestStaticInit1(); else TestStaticInit2();
		//TestWndGetGUIThreadInfo();
		//TestWndFindRaw();
		//TestWndNextMainWindow();
		//TestTaskDialogActivationEtc();
		//TestWndIsUacDenied();
		//TestWndZorder();
		//TestWndMoveMisc();
		//TestWndRect();
		//TestWndControlCast();
		//OutList(Calc.AngleDegrees(1, 1), Calc.AngleDegrees(0, 1), Calc.AngleDegrees(1, 0));
		//TestWndStoreApp();
		//TestWndBorder();
		//TestWndSetParent();
		//TestWndIsAbove();
		//TestWndMapPoints();
		//TestWndGetPropList();
		//TestIsGhost();
		//TestWndRegisterClass();
		//TestWndMisc();
		//TestStringMisc();
		//TestCoord();
		//TestCoord(1, 2);
		//TestCoord(1.1f, 2.2f);
		//TestCoord(3.3, 4.4);
		//TestFileIcon();
		//TestWndGetIcon();
		//TestOutAnyInterface();
		//TestExceptions();
		//TestStrToI2();
		//TestStringFold();
		//TestWndTransparency();
		//TestWndRegistrySaveRestore();
		//TestRegistry();
		//TestWndShowHide();
		//TestWndArrange();
		//TestWndClose();
		//TestWndtaskbarButton();
		//TestWindowDimensions();
		//TestThreadError();
		//TestWndMinMaxRes();
		//TestWndActivateFocus();
		//TestArrayAndList(new string[] { "one", "two" });
		//TestArrayAndList(new List<string> { "three", "four", "five" });
		//TestWndFindAll();
		//TestWndAll();
		//TestStringPlusConcatInterpolation();
		//TestMemory();
		//TestChildFromXY();
		//TestWndFromXY();
		//TestWndFind();
		//TestWildNot();
		//TestDotNetControls();
		//TestProcessMemory();
		//TestRegexSpeed();
		//TestStringEmpty("df");
		//TestWildString();
		//TestProcessUacInfo();
		//TestProcesses();
		//TestSerialization();
		//TestCsv();
		//TestCsvSerialization();
		//TestShow();
		//TestCurrentCulture();
		#endregion
	}

	public static unsafe void TestMain()
	{
		Output.Clear();
		WaitMS(100);

		//TestX();
		//TestInScriptDomain();
		var t = new Thread(() =>
		{
			var domain = AppDomain.CreateDomain("Test");
			try {
				//domain.ExecuteAssembly(@"..\Test Projects\ScriptClass\bin\Debug\ScriptClass.exe");

				//domain.DoCallBack(TestX);
				//Out("after domain.DoCallBack");

				domain.CreateInstance("CatkeysTasks", "Test");
				//domain.CreateInstanceFrom("CatkeysTasks.exe", "Test");
				//Out("after domain.CreateInstance");
			}
			finally {
				AppDomain.Unload(domain);
				//Out("after AppDomain.Unload(domain)");
			}
		}
		);
		t.SetApartmentState(ApartmentState.STA); //must be STA, or something will not work, eg some COM components, MSAA in TaskDialog.
		t.Start();
		t.Join();
		//Show.TaskDialog("after all");
		//Out("after all");
	}
}
