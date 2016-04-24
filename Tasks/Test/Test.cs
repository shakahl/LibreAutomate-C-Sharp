//#define NETSM

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

#pragma warning disable 162 //unreachable code

#if !NETSM

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
			Speed.First();
			Speed.Execute(1000, a1);
			Speed.Write();
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
			Speed.First();
			Speed.Execute(1000, a1);
			Speed.Write();
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
		////Speed.First();
		////td.ThreadWaitOpen();
		////Speed.NextWrite();
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

		//Speed.First();
		TDResult r = d.Show();
		//Speed.NextWrite();

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
		//	Speed.First();
		//	Speed.Execute(1000, a1);
		//	Speed.Execute(1000, a2);
		//	Speed.Execute(1000, a3);
		//	Speed.Execute(1000, a4);
		//	Speed.Execute(1000, a5);
		//	Speed.Execute(1000, a6);
		//	Speed.Execute(1000, a7);
		//	Speed.Write();
		//}

		//Task.Run(()=> Directory.SetCurrentDirectory("c:\\windows\\system32"));
		//Wait(1);
		//Out(Path.GetFullPath("..\\nnn.ttt"));

		//Out(File.Exists(@"%windir%\System32\notepad.exe"));
		//Out(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
	}

	static void TestWndAll()
	{
		//Wnd wq = Wnd.Find(null, "QM_Editor");
		//Out(wq.GetClassLong(Api.GCW_ATOM));

		//Out(Util.Window.RegWinClassHidden("QM_Editor", _WndProcCompiler));

		//Out(Wnd.GetClassAtom("Edit"));
		//Out(Wnd.GetClassAtom("QM_Editor"));
		//Out(Wnd.GetClassAtom("QM_Editor", Util.Misc.GetModuleHandleOfExe()));

		//var ac = new string[] { "IME", "MSCTFIME UI", "tooltips_class32", "ComboLBox", "WorkerW", "VBFloatingPalette" };
		//foreach(string s in ac) Out(Wnd.GetClassAtom(s));

		//return;

		//List<Wnd> a = null;

		////var a1 = new Action(() => { a = Wnd.All.Windows(); });
		////var a1 = new Action(() => { a = Wnd.All.Windows(null, true); });
		////var a1 = new Action(() => { a = Wnd.All.Controls(wy); });
		////var a2 = new Action(() => { a = Wnd.All.Controls2(wy); });
		////var a2 = new Action(() => { Wnd.All.Controls((c, e) => { }, wy); });
		//var a1 = new Action(() => { a = Wnd.All.Windows(); });
		//var a2 = new Action(() => { Wnd.All.Windows((w3, e) => { }); });

		////Wnd.All.Controls((c, e)=> { Out(c); }, wy, null, true); return;
		////Wnd.All.Windows((c, e)=> { Out(c); }, "QM_*"); return;
		////Wnd.All.Windows((c, e)=> { Out(c); }, null, true); return;
		////foreach(Wnd w3 in Wnd.All.Windows(onlyVisible:true)) { Out(w3); }; return;
		////a1();

		////var a2 = new Action(() =>
		////{
		////	foreach(Wnd t in a) {
		////		string s = t.ClassName;
		////	}
		////});

		//int n = 10;

		//for(int k = 0; k < 5; k++) {
		//	Speed.First();
		//	Speed.Execute(n, a1);
		//	Speed.Execute(n, a2);
		//	Speed.Write();
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

		//	var a1 = new Action(() => { a=Wnd.All.Controls(hform); });
		//	//var a1 = new Action(() => { a=Wnd.All.Controls(wq); });
		//	//a1();

		//	for(int k=0; k<5; k++) {
		//		Speed.First();
		//		Speed.Execute(1000, a1);
		//		Speed.Write();
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

		////List<Wnd> e1 = Wnd.All.Controls((Wnd)1245464);
		////Out(e1.Count);
		//////IEnumerable<Wnd> e= Wnd.All.DirectChildControlsEnum((Wnd)1245464);
		////List<Wnd> e2 = Wnd.All.DirectChildControlsFastUnsafe((Wnd)1245464);
		////Out(e2.Count);

		////Speed.First(true);
		////for(int uu=0; uu<5; uu++) {
		////	Speed.First();
		////	Speed.Execute(1000, () => { e2 = Wnd.All.DirectChildControlsFastUnsafe((Wnd)1245464); });
		////	Speed.Execute(1000, () => { e1 = Wnd.All.Controls((Wnd)1245464); });
		////	Speed.Execute(1000, () => { e1 = Wnd.All.Controls((Wnd)1245464, "Button", true); });
		////	Speed.Execute(1000, () => { Wnd.All.Controls(c=> { /*Out(c);*/ return false; }, (Wnd)1245464); });
		////	Speed.Write();
		////}

	}

	static void TestProcesses()
	{
		Wnd w = Wnd0;
		bool hiddenToo = true;

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
		//Speed.ExecuteMulti(5, 1000, a11, a12);
		//Out(pn1); Out(pn2);
		//return;

		var a1 = new Action(() => { w = Wnd.Find("*Notepad", hiddenToo: hiddenToo); });
		var a2 = new Action(() => { w = Wnd.Find("*Notepad", "Notepad", hiddenToo: hiddenToo); });
		var a3 = new Action(() => { w = Wnd.Find("*Notepad", "Notepad", "Notepad", hiddenToo: hiddenToo); });
		var a4 = new Action(() => { w = Wnd.Find("*Notepad", "Notepad", "NotepaD.exE", hiddenToo: hiddenToo); });
		var a5 = new Action(() => { w = Wnd.Find("*Notepad", "Notepad", @"c:\windows\syswow64\Notepad.exe", hiddenToo: hiddenToo); });
		//var a6 = new Action(() => { w = Wnd.Find("", "", "NotepaD.exE", hiddenToo: hiddenToo); });
		//var a7 = new Action(() => { w = Wnd.Find("", "", @"c:\windows\syswow64\Notepad.exe", hiddenToo: hiddenToo); });
		var a6 = new Action(() => { w = Wnd.Find("", "", "no NotepaD.exE", hiddenToo: hiddenToo); });
		var a7 = new Action(() => { w = Wnd.Find("", "", @"c:\no windows\syswow64\Notepad.exe", hiddenToo: hiddenToo); });
		var a8 = new Action(() => { w = Wnd.Find("", "", "youtubedownloader", hiddenToo: hiddenToo); });

		//a6(); Out(w); return;

		a1(); Out(w);
		a2(); Out(w);
		a3(); Out(w);
		a4(); Out(w);
		a5(); Out(w);
		a6(); Out(w);
		a7(); Out(w);
		a8(); Out(w);

		Speed.ExecuteMulti(5, 1, a1, a2, a3, a4, a5, a6, a7, a8);

		//Wnd w = (Wnd)12582978; //Notepad

		//string s = null;

		//s = w.ProcessName;
		//Out(s);
		//s = w.ProcessPath;
		//Out(s);

		//var a1 = new Action(() => { s = w.ProcessName; });
		//var a2 = new Action(() => { s = w.ProcessPath; });

		//Speed.ExecuteMulti(5, 1, a1, a2);

		//Process[] a=null;

		//Speed.First(true);
		//a = Process.GetProcesses();
		//Speed.Next();
		//a = Process.GetProcesses();
		//Speed.Next();
		//a = Process.GetProcesses();
		//Speed.Next();
		//a = Process.GetProcessesByName("NoTepad");
		//Speed.Next();
		//a = Process.GetProcessesByName("NoTepad");
		//Speed.Next();
		//a = Process.GetProcessesByName("NoTepad");
		//Speed.Next();
		//Speed.Write();

		//int pid = a[0].Id;

		//Process u = null;
		//string name = null;

		//Speed.First();
		//u=Process.GetProcessById(pid);
		//Speed.Next();
		//name = u.ProcessName;
		//Speed.Next();
		//u=Process.GetProcessById(pid);
		//Speed.Next();
		//name = u.ProcessName;
		//Speed.Next();
		//u=Process.GetProcessById(pid);
		//Speed.Next();
		//name = u.ProcessName;
		//Speed.Next();
		//Speed.Write();

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
		//Speed.SpinCPU(200);
		//Speed.ExecuteMulti(5, 1, () => { is1 = UacInfo.IsThisProcessAdmin; }, () => { is2 = Api.IsUserAnAdmin(); }, () => { is3 = IsUserAdmin; });
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

			Speed.First();
			if(i < 0) p = UacInfo.ThisProcess;
			else {
				x = a[i];
				Out($"<><Z 0x80c080>{x.ProcessName}</Z>");
				p = UacInfo.GetOfProcess((uint)x.Id);
			}
			if(p == null) { Out("failed"); continue; }
			Speed.Next();
			var elev = p.Elevation; if(p.Failed) Out("<><c 0xff>Elevation failed</c>");
			Speed.Next();
			var IL = p.IntegrityLevel; if(p.Failed) Out("<><c 0xff>IntegrityLevel failed</c>");
			Speed.Next();
			var IL2 = p.IntegrityLevelAndUIAccess;
			Speed.Next();
			bool uiaccess = p.IsUIAccess; if(p.Failed) Out("<><c 0xff>IsUIAccess failed</c>");
			Speed.Next();
			bool appcontainer = p.IsAppContainer; if(p.Failed) Out("<><c 0xff>IsAppContainer failed</c>");
			Speed.Next();
			Out($"elev={elev}, uiAccess={uiaccess}, AppContainer={appcontainer}, IL={IL}, IL+uiaccess={IL2}");
			//Out($"uiAccess={uiaccess}, IL={IL}");
			//Speed.Write();
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

		//if(excep) Speed.First(100, a2, a2);
		//else Speed.First(100, a1, a2, a2);
		//for(int i = 0; i < 4; i++) {
		//	Speed.First();
		//	if(!excep) Speed.Execute(1000, a1);
		//	Speed.Execute(1000, a2);
		//	Speed.Execute(1000, a3);
		//	//for(int j = 0; j < 1000; j++) s.LikeEx_(p);
		//	//Speed.Next();
		//	//for(int j = 0; j < 1000; j++) x.Match(s);
		//	//Speed.Next();
		//	Speed.Write();
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

		Speed.ExecuteMulti(5, 1000, a1, a2, a1i, a2i, a1o, a2o, a1oi, a2oi);

		//compiling: Regex 4500, PCRE 450.
		//match (invariant, match case): Regex 1200, PCRE 300.
		//match (invariant, ignore case): Regex 1700, PCRE 300.
	}

	static void TestProcessMemory()
	{
		//var w = Wnd.FindByClassName("QM_Editor");
		var w = Wnd.FindByClassName("Notepad");
		Out(w);
		ProcessMemory x = null;
		try {
			x = new ProcessMemory(w, 1000);
			//x = new ProcessMemory(w, 0);

		} catch(CatkeysException e) { Out(e); return; }

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

		var a = Wnd.All.Controls(w);
		foreach(Wnd k in a) {
			Out("---");
			Out(k);
			Out(WindowsFormsControlNames.IsDotNetWindow(k));
			Out(x.GetControlName(k));
		}
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
		Wnd w = Wnd0, c=Wnd0;
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
		//w = Wnd.Find(null, "QM_*", prop:new Wnd.WinProp() {childClass="QM_Code" } ); //TODO: test when Child implemented
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
		w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childId=2202 });
		w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childClass = "Button", childName= "Te&xt" });
		w = Wnd.Find(null, "QM_*", prop: new Wnd.WinProp() { childClass = "*Edit", childName= "sea" });
		w = Wnd.Find(null, "", prop: new Wnd.WinProp() { child=new Wnd.ChildDefinition("*ame") });
		w = Wnd.Find("Free YouTube Downloader", "*.Window.*");
		//w = Wnd.Find("Keyboard Layout*", "*.Window.*");
		w = Wnd.Find("?*", prop: new Wnd.WinProp() { x=1, xFromRight=true, y=10 });
		//w = Wnd.FromXY(100, 100, null, true, true);
		//w = Wnd.FromXY(0.1, 0.1, null, true, true);

		//for(int i=0; i<30; i++) {
		//	Wait(1);
		//	Out(Wnd.FromMouse(false));
		//}

		//Speed.ExecuteMulti(5, 100, () => { Wnd.FindByClassName("QM_Editor"); }, () => { Wnd.Find(null, "QM_Editor"); });

		w = Wnd.FindByClassName("QM_Editor");
		//w = Wnd.Find("Catkeys -*");
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
		c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildProp() { x=1});
		c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildProp() { x=1276});
		c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildProp() { x=1, xFromRight=true });
		c = w.Child(null, "SysTreeView32", prop:new Wnd.ChildProp() { x=w.RectClient.Width-1 });
		//c = w.Child(null, "QM_*", prop:new Wnd.ChildProp() { y=0.02, yFromBottom=true });
		//c = w.Child("Find &Previous");
		//c = w.Child("Find Previous");

		Out(c);
		return;

		Wnd c1 = Wnd0, c2 = Wnd0, c3 = Wnd0, c4 = Wnd0, c5 = Wnd0, c6 = Wnd0, c7 = Wnd0;
		var a1 = new Action(() => { c1 = w.Child("sea"); });
		var a2 = new Action(() => { c2 = w.Child(Wnd.ChildFlag.ControlText, "sea"); });
		var a3 = new Action(() => { c3 = w.Child("sea", "*Edit"); });
		var a4 = new Action(() => { c4 = w.Child(Wnd.ChildFlag.ControlText, "sea", "*Edit"); });
		var a5 = new Action(() => { c5 = w.Child("Regex*"); });
		var a6 = new Action(() => { c6 = w.Child("Regex*", "Button"); });
		var a7 = new Action(() => { c7 = w.Child("Regex*", id:1028); });
		Speed.ExecuteMulti(5, 100, a1, a2, a3, a4, a5, a6, a7);
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

		//var a1 = new Action(() => { Wnd.All.Windows(e => { }); });
		//var a2 = new Action(() => { var a = Wnd.All.ThreadWindows(7192); /*Out(a == null); Out(a.Count);*/ });
		////a2();

		//Out("---");
		//Speed.ExecuteMulti(5, 1000, a1, a2);

		//Wnd w = (Wnd)1705264;

		//var a1 = new Action(() => { RECT r = w.Rect; });
		//var a2 = new Action(() => { uint st = w.Style; });
		//var a3 = new Action(() => { uint est = w.ExStyle; });
		//var a4 = new Action(() => { string name = w.Name; });
		//var a5 = new Action(() => { string cn = w.ClassName; });
		//var a6 = new Action(() => { uint tid = w.ThreadId; });
		//var a7 = new Action(() => { Wnd own = w.Owner; });
		//var a8 = new Action(() => { bool clo=w.Cloaked; });

		//Speed.ExecuteMulti(5, 1000, a1, a2, a3, a4, a5, a6, a7, a8);
	}

	//[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
	static void TestX()
	{
		TestWndFind();
		//TestWildNot();
		//TestDotNetControls();
		//TestProcessMemory();
		//TestRegexSpeed();
		//TestStringEmpty("df");
		//TestWndAll();
		//TestWildString();
		//TestProcessUacInfo();
		//TestProcesses();
		//TestSerialization();
		//TestCsv();
		//TestCsvSerialization();
		//TestShow();
		//TestCurrentCulture();
	}

	//static void ScriptDomainThread()
	//{

	//}

	public Test()
	{
		WaitMS(100);
		TestX();
	}

	static void TestInScriptDomain()
	{
		var domain = AppDomain.CreateDomain("Test");

		try {

			//domain.ExecuteAssembly(@"..\Test Projects\ScriptClass\bin\Debug\ScriptClass.exe");

			//domain.DoCallBack(TestX);
			//Out("after domain.DoCallBack");

			domain.CreateInstance("CatkeysTasks", "Test");
			//domain.CreateInstanceFrom("CatkeysTasks.exe", "Test");
			//Out("after domain.CreateInstance");

		} finally {
			AppDomain.Unload(domain);
			//Out("after AppDomain.Unload(domain)");
		}
	}
	//static void TestInScriptDomain()
	//{
	//	var domain = AppDomain.CreateDomain("Test");
	//	//domain.ExecuteAssembly(@"..\Test Projects\ScriptClass\bin\Debug\ScriptClass.exe");
	//	domain.DoCallBack(TestX); //Assembly.GetEntryAssembly() fails. OK if ExecuteAssembly.
	//	Out("after domain.DoCallBack");
	//	AppDomain.Unload(domain);
	//	Out("after AppDomain.Unload(domain)");
	//}


	static unsafe void TestAny()
	{
		Output.Clear();

		//TestX();
		//TestInScriptDomain();
		var t = new Thread(TestInScriptDomain);
		t.SetApartmentState(ApartmentState.STA); //must be STA, or something will not work, eg some COM components, MSAA in TaskDialog.
		t.Start();
		t.Join();
		//Show.TaskDialog("after all");
		//Out("after all");
	}

	#region old

	public static unsafe void _Main()
	{
		TestAny();
		return;



		//Speed.SpinCPU(); //does nothing
		long t1 = Stopwatch.GetTimestamp();

		//TODO: instead could simply allocate unmanaged memory with Marshal methods and pass to domains with childDomain.SetData
		_sm = new OurSharedMemory();
		_sm.Create("Catkeys_SM_Tasks", 1024 * 1024);

		if(true) //compiler
		{
			_sm.x->perf.AddTicksFirst(t1);
			_sm.x->perf.Next();

			IntPtr ev = Api.CreateEvent(Zero, false, false, null);

			_sm.x->eventCompilerStartup = ev;

			//Mes("before");

			var thr = new Thread(_AppDomainThread);
			thr.Start();

			_sm.x->perf.Next();

			Api.WaitForSingleObject(ev, ~0U);
			//Thread.Sleep(100);
			Api.CloseHandle(ev);

			_sm.x->perf.Next();
			_sm.x->perf.Write();

			_hwndCompiler = _sm.x->hwndCompiler;

			for(int i = 0; i < 1; i++) {
				_hwndCompiler.Send(Api.WM_USER, Zero, Marshal.StringToBSTR("test"));
			}

			//Mes("in");

			_hwndCompiler.Send(Api.WM_CLOSE);
			//Environment.Exit(0);

			//Mes("after");
			//return;
		}

		//Thread.Sleep(100);

		//for(int i = 0; i<5; i++) {
		//	var thr2 = new Thread(_AppDomainThread2);
		//	thr2.Start();
		//	Thread.Sleep(100);
		//	if(i%10!=9) continue;
		//	//Speed.First();
		//	thr2=null;
		//	GC.Collect(); //releases a lot. Without it, GC runs when Task Manager shows 100 MB.
		//				  //GC.Collect(2, GCCollectionMode.Optimized); //collects at 26 MB; without - at 36 MB
		//}
		//Mes("exit");
	}

	static void _AppDomainThread2()
	{
		Speed.First();
		var domain = AppDomain.CreateDomain("Compiler");
		Speed.Next();
		domain.ExecuteAssembly(@"C:\Test\test1.exe");
		Speed.Next();
		AppDomain.Unload(domain);
		Speed.NextWrite();
	}

	static void _AppDomainThread()
	{
		//_DomainCallback();

		var domain = AppDomain.CreateDomain("Compiler");
		//var domain=AppDomain.CreateDomain("Compiler", AppDomain.CurrentDomain.Evidence, new AppDomainSetup { LoaderOptimization = LoaderOptimization.MultiDomain }); //by default makes faster, but makes much slower when we use LoaderOptimization attribute on Main(). Assemblies will not be unloaded when appdomain unloaded (will use many MB of memory).
		//System.IO.Pipes.AnonymousPipeClientStream
		//childDomain.SetData("hPipe", handle.ToString());
		unsafe { _sm.x->perf.Next(); }

		domain.DoCallBack(_DomainCallback);
		//domain.ExecuteAssembly(Paths.CombineApp("Compiler.exe"));
		//domain.DoCallBack(Compiler.Compiler.Main); //faster than ExecuteAssembly by 3-4 ms
		AppDomain.Unload(domain);
		domain = null;
		//Out("_AppDomainThread() ended");
		GC.Collect(); //releases a lot
					  //Mes("MinimizeMemory");
					  //Misc.MinimizeMemory(); //does nothing

		//tested:
		//Currently speed and memory is similar in both cases - when compiler is in this assembly and when in another.
		//But will need to test later, when this assembly will be big.
		//Not using LoaderOptimization.MultiDomain, because then does not unload assemblies of unloaded domains (then uses much memory, and there is no sense to execute compiler in a separate domain).
	}

	//[System.Runtime.CompilerServices.MethodImpl(MethodImplOptions.NoOptimization)]
	static unsafe void _DomainCallback()
	{
		//if(AppDomain.CurrentDomain.FriendlyName!="Compiler") return;
		long t1 = Stopwatch.GetTimestamp();

		_sm = new OurSharedMemory();
		_sm.Open("Catkeys_SM_Tasks");

		_sm.x->perf.AddTicksNext(t1);
		_sm.x->perf.Next();

		//=AppDomain.CurrentDomain.GetData("hPipe")

		Util.Window.RegWinClassHidden("Catkeys_Compiler", _wndProcCompiler);

		_sm.x->perf.Next();

		Wnd w = Api.CreateWindowEx(0, "Catkeys_Compiler", null, Api.WS_POPUP, 0, 0, 0, 0, Wnd.Spec.Message, Zero, Zero, Zero);

		_sm.x->perf.Next();

		_SHMEM* x = _sm.x;
		x->hwndCompiler = w;
		Api.SetEvent(x->eventCompilerStartup);

		//message loop
		//Application.Run(); //By default would add several ms to the startup time. Same speed if Main() has the LoaderOptimization attribute. Also may be not completely compatible with native wndproc. Also in some cases adds several MB to the working set.
		Api.MSG m;
		while(Api.GetMessage(out m, Wnd0, 0, 0) > 0) { Api.DispatchMessage(ref m); }
	}

	unsafe static LPARAM _WndProcCompiler(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam)
	{
		switch(msg) {
		//case WM.NCCREATE:
		//	_hwndAM=hWnd;
		//	break;
		//case WM.CREATE:
		//	Speed.Next();
		//	break;
		//case WM.COPYDATA: //TODO: ChangeWindowMessageFilter
		//	_OnCopyData(wParam, (api.COPYDATASTRUCT*)lParam);
		//	break;
		//case WM.DESTROY:
		//	Out("destroy");
		//	break;
		case Api.WM_USER:
			//Out(Marshal.PtrToStringBSTR(lParam));
			Marshal.FreeBSTR(lParam);
			TestRoslyn();
			return Zero;
		}

		LPARAM R = Api.DefWindowProc(hWnd, msg, wParam, lParam);

		switch(msg) {
		case Api.WM_NCDESTROY:
			Api.PostQuitMessage(0); //Application.Exit[Thread](); does not work
			break;
		}
		return R;

		//tested: .NET class NativeWindow. It semms its purpose is different (to wrap/subclass an existing class).
	}

	static void TestRoslyn()
	{
		//Out("test");
		//TODO: auto-ngen compiler. Need admin.

		Speed.First();
		//System.Runtime.ProfileOptimization.SetProfileRoot(@"C:\Test");
		//System.Runtime.ProfileOptimization.StartProfile("Startup.Profile"); //does not make jitting the C# compiler assemblies faster
		//Speed.Next();

		//Assembly a = Assembly.LoadFile(@"Q:\Test\Csc\csc.exe"); //error
		//Assembly a = Assembly.LoadFile(@"C:\Program Files (x86)\MSBuild\14.0\Bin\csc.exe"); //error
		Assembly a = Assembly.LoadFile(Folders.App + "csc.exe"); //ok
																 //Assembly a = Assembly.Load("csc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"); //works, same speed as LoadFile, but VS shows many warnings if this project uses different .NET framework version than csc (which is added to project references). Also, possibly could make the app start slower, especially if HDD. Better to load on demand through reflection.
		MethodInfo m = a.EntryPoint;
		string[] g = new string[] {null, "/nologo", "/noconfig", "/target:winexe",
		"/r:System.dll", "/r:System.Core.dll", "/r:System.Windows.Forms.dll",
		  @"C:\Test\test.cs"};
		object[] p = new object[1] { g };

		//g[0]="/?";
		Speed.Next(); //16 ms
		for(int i = 1; i <= 4; i++) {
			g[0] = $@"/out:C:\Test\test{i}.exe";
			int r = (int)m.Invoke(0, p); //works, 22 ms, first time ~300 ms on Win10/.NET4.6 and ~600 on older Win/.NET.
			if(r != 0) Out(r);
			Speed.Next();
			//GC.Collect(); //4 ms, makes working set smaller 48 -> 33 MB
			//Speed.Next();
		}
		Speed.Write();

		//Mes("now will GC.Collect");
		GC.Collect(); //releases a lot

		OutLoadedAssemblies();
		Show.TaskDialog("exit");
	}

	static void OutLoadedAssemblies()
	{
		AppDomain currentDomain = AppDomain.CurrentDomain;
		Assembly[] assems = currentDomain.GetAssemblies();
		foreach(Assembly assem in assems) {
			Out(assem.ToString());
			Out(assem.CodeBase);
			Out(assem.Location);
			Out("");
		}
	}

	#region misc

	struct _SHMEM
	{
		public IntPtr eventCompilerStartup;
		public Wnd hwndCompiler;
		public Speed.PerformanceCounter perf;
	}

	//We don't use MemoryMappedFile because it is very slow. Creating/writing is 1500, opening/reading is 5000.
	//With this class - 1300 and 600 (because of JIT). With ngen - 60 and 20 (same as in C++).
	unsafe class OurSharedMemory :Util.SharedMemoryFast
	{
		public _SHMEM* x { get { return (_SHMEM*)_mem; } }
	}

	static OurSharedMemory _sm;

	static Wnd _hwndCompiler;
	static Api.WNDPROC _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate


	//class DialogVariables { public string lb3, c4; public string[] au; }
	//class DialogVariables { public object lb3, c4, au; }
	//here class is better than struct, because:
	//Don't need ref;
	//Can be used with modeless dialogs.

	//const string S1="one"+NL+"two"; //ok
	//const string S2=$"one{NL}two"; //error


	static void ShowDialog(object v)
	{
		FieldInfo[] a = v.GetType().GetFields();
		foreach(FieldInfo f in a) {
			Out(f.Name);
			//Out(f.FieldType.Equals(typeof(string)));
			switch(Type.GetTypeCode(f.FieldType)) {
			case TypeCode.String: Out("string"); break;
			case TypeCode.Object: Out("object"); break;
			}
		}
	}

	//delegate void Dee(GCHandle x);

	//static void Mee(GCHandle x)
	//{
	//Out("here"); return;
	////Out(x.IsAllocated);
	//Out(GCHandle.ToIntPtr(x));
	//if(GCHandle.ToIntPtr(x)==Zero) Out("null");
	//else {
	//string s=(x.Target as string);
	//Out(s);
	//}
	//}
	delegate void Dee(object x);

	[DllImport("UnmanagedDll", CallingConvention = CallingConvention.Cdecl)]
	static extern void TestUnmanaged();


	delegate void Del(int t);
	//delegate void Del0();

	class TestIndexers
	{
		//public int this[int i]
		//{
		//get { return i*i; }
		//set { Out($"{i} {value}"); }
		//}
		//public int this[int i, int j=1]
		//{
		//	get { return i*j; }
		//	set { Out($"{i} {j} {value}"); }
		//}
		public string this[string s]
		{
			get { return s + " ?"; }
			set { Out($"{s} {value}"); }
		}

		//static TestIndexers ti_=new TestIndexers();
		//public static TestIndexers Hotkey => ti_;
		//or
		public static readonly TestIndexers Hotkey = new TestIndexers();
	}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static void TestCallersVariables()
	//{
	//	Speed.First();
	//	StackFrame frame = new StackFrame(1);
	//	var method = frame.GetMethod();
	//	MethodBody mb=method.GetMethodBody();
	//	int n=0;
	//	foreach(LocalVariableInfo v in mb.LocalVariables) {
	//		//Out(v.LocalType.ToString());
	//		if(v.LocalType.Name=="Wnd") {
	//			n++;
	//			//v.
	//		}
	//	}
	//	Out(n);
	//}


	[Trigger.Hotkey("Ctrl+K")]
	public static void Function1(HotkeyTriggers.Message m) { Out("script"); }

	#endregion
}


#endregion

#else

	class Test
{
	static MemoryMappedFile _mmf;
	static MemoryMappedViewAccessor _m;
	//0 event
	//8 _hwndCompiler

	static Wnd _hwndCompiler;
	static api.WndProc _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate


	public static void _Main()
	{
	Speed.First();
		IntPtr ev=api.CreateEvent(Wnd0, false, false, null);

		_mmf=MemoryMappedFile.CreateNew("Catkeys_SM_Tasks", 1024*1024);
		_m=_mmf.CreateViewAccessor();

		_m.Write(0, ref ev);
		
		var thr=new Thread(_AppDomainThread);
		thr.Start();

		api.WaitForSingleObject(ev, ~0U);
		//Thread.Sleep(100);
		api.CloseHandle(ev);

		_m.Read(8, out _hwndCompiler);
	Speed.Next();
		
		_hwndCompiler.Send(WM.USER, Zero, Marshal.StringToBSTR("test"));

		Msg("exit");
		
		_hwndCompiler.Send(WM.CLOSE);
		//Environment.Exit(0);
	}

	static void _AppDomainThread()
	{
		var domain=AppDomain.CreateDomain("Compiler");
		//System.IO.Pipes.AnonymousPipeClientStream
		//childDomain.SetData("hPipe", handle.ToString());
		domain.DoCallBack(_DomainCallback);
		AppDomain.Unload(domain);
		//Out("_AppDomainThread() ended");
	}

	static void _DomainCallback()
	{
		//=AppDomain.CurrentDomain.GetData("hPipe")

		Util.Window.RegWinClassHidden("Catkeys_Compiler", _wndProcCompiler);

		Wnd w=api.CreateWindowExW(0, "Catkeys_Compiler",
			null, WS.POPUP, 0, 0, 0, 0, Wnd.Spec.Message,
			Zero, Zero, Zero);

		_mmf=MemoryMappedFile.OpenExisting("Catkeys_SM_Tasks"); //1.5 ms
		_m=_mmf.CreateViewAccessor(); //3.5 ms. Why it is so slow? CreateOrOpen is even slower.

		_m.Write(8, ref w);
		IntPtr ev; _m.Read(0, out ev);
		api.SetEvent(ev);

		Application.Run(); //message loop
	}

	unsafe static LPARAM _WndProcCompiler(Wnd hWnd, WM msg, LPARAM wParam, LPARAM lParam)
	{
		switch(msg) {
		//case WM.NCCREATE:
		//	_hwndAM=hWnd;
		//	break;
		//case WM.CREATE:
		//	Speed.Next();
		//	break;
		//case WM.COPYDATA: //TODO: ChangeWindowMessageFilter
		//	_OnCopyData(wParam, (api.COPYDATASTRUCT*)lParam);
		//	break;
		//case WM.DESTROY:
		//	Out("destroy");
		//	break;
		case WM.USER:
			Out(Marshal.PtrToStringBSTR(lParam));
			Marshal.FreeBSTR(lParam);
			return Zero;
		}

		LPARAM R = api.DefWindowProcW(hWnd, msg, wParam, lParam);

		switch(msg) {
		case WM.NCDESTROY:
			api.PostQuitMessage(0); //Application.Exit[Thread](); does not work
			break;
		}
		return R;

		//tested: .NET class NativeWindow. It semms its purpose is different (to wrap/subclass an existing class).
	}
}



#endif













#region commented

//TestUnmanaged();

//StackTrace stackTrace = new StackTrace();
//Out(stackTrace.GetFrame(1).GetMethod().Name);
////Out(stackTrace.GetFrame(1).GetFileLineNumber()); //always 0, even in Debug build
//Out(stackTrace.GetFrame(1).GetFileName()); //null
////Out(stackTrace.GetFrame(1).GetMethod(). //nothing useful




//Dee f=Mee;
////f(GCHandle.Alloc("test"));
////f(GCHandle.FromIntPtr(Zero));
//f("test");
//f(5);


//return;

//UIntPtr ki=1;

//switch(2)
//{
//	case 1:
//	int hdh=8;
//	break;
//	case 2:
//	int koop=8;
//Out(hdh);
//	break;
//}

//Out(hdh);

//Out($"one{_}two");
//Out("three"+_+"four");
//Out("one" RN "two"); //error
//Out($"one{}two"); //error


////str controls="3"
////var d=new DialogVariables("3") { lb3="oooo" };
////var d=new DialogVariables("3");
//var d=new DialogVariables();
//d.lb3="oooo";
//d.c4=7;
//d.au=new string[] { "one", "two" };
////d.au={ "one", "two" }; //error
//ShowDialog(d);
//return;


//static void AnotherThread()
//{
//	//Show.TaskDialog("another thread", "", "", x:1);
//	MessageBox.Show("another thread");
//	Out("after msgbox in another thread");
//}

//[DllImport("comctl32.dll", EntryPoint = "TaskDialog")]
//static extern int _TaskDialog(Wnd hWndParent, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, TDButton dwCommonButtons, LPARAM pszIcon, out int pnButton);

//static int TD(string s, bool asy)
//{
//	int r = 0;
//	for(int i = 0; i < 100; i++) {
//		int hr = _TaskDialog(Wnd0, Zero, "Test", s, null, TDButton.Cancel, 0, out r);
//		//OutList(hr, r, asy);
//		if(hr == 0 || hr == Api.E_INVALIDARG) break;
//		Time.WaitMS(20);
//	}
//	return r;
//}



////string ss = "gggggg.txt";
////Out(ss.Like_(false, "*.exe", "*.txt"));

////string ss = "5ggggg.txt";
////Out(ss.LikeEx_(false, "#*.exe", "#*.txt"));

////string ss = "5ggggg.txt";
////Out(ss.Equals_(false, "moo.exe", "5ggggg.txt"));

////string ss = "5ggggg.txt";
////Out(ss.EndsWith_(false, ".exe", ".txt"));

////string ss = "file.txt";
////Out(ss.StartsWith_(false, "kkk", "file."));

//string ss = "file.txt";
////Out(ss.RegexIs_(".*.TXT"));
////Out(ss.RegexIs_(".*.txt", RegexOptions.IgnoreCase));
////Out(ss.RegexIs_(0, ".*.TXT", ".*.txt"));
////Out(ss.RegexIs_(RegexOptions.IgnoreCase, ".*.TXT", ".*.txt"));
//ss = "gggg.exe\naaa.txt\nbbb.txt";
////Out(ss.RegexMatch_(".*.TXT"));
////Out(ss.RegexMatch_(".*.TXT", RegexOptions.IgnoreCase));
////Output.Write(ss.RegexMatches_(".*.TXT"));
////Output.Write(ss.RegexMatches_(".*.TXT", RegexOptions.IgnoreCase));

//Out(ss.RegexReplace_(@"\.txt\b", ".moo"));
//Out(ss.RegexReplace_(@"\.txt\b", m=> { return ".boo"; }));

////var au = new int[] {1, 2, 3 };
////var au = new List<int> {1, 2, 3 };
////var au = new Dictionary<string, string>() { { "A", "B" }, { "C", "D" } };
////Output.Write(au, ", ");

//return;




//Out(AppDomain.CurrentDomain.FriendlyName);

//new Thread(AnotherThread).Start();

//Show.TaskDialog("appdomain primary thread", "", "e");

////Thread.CurrentThread.Abort();
////AppDomain.Unload(AppDomain.CurrentDomain);
//Out("after TaskDialog");

//return;

//Show.TaskDialog("", "<a href=\"test\">test</a>", onLinkClick: ed =>
//{
//	Wnd z = ed.hwnd;
//	string s = null;
//	Speed.First(true);
//	for(int j = 0; j<8; j++) {
//		for(int i=0; i<1000; i++) s= z.ClassName;
//		//for(int i=0; i<1000; i++) s= z.Name;
//		//for(int i = 0; i<1000; i++) s= z.ControlText;
//		//for(int i = 0; i<1000; i++) s= z.ControlTextLength;
//		Speed.Next();
//	}
//	Speed.Write();
//	Out(s);
//}
//);

//return;

////Time.Wait(1);
//Wnd z = Wnd.Find("Untitled - Notepad");
////z=(Wnd)2098486; //Inno
////z=(Wnd)395896; //Static
////z=(Wnd)1510052; //Edit

////z.Name = "MMMMMMMMGGGG"; return;

////string m = z.Name;
////string m = z.GetControlText();
////OutList(m==null, m=="", m);
////return;

//Out(z);

//string cn = null;
////cn= z.ControlText; OutList(cn.Length, cn); return;
//int nt = 0;

//Speed.First(true);
//for(int j = 0; j<8; j++) {
//	//for(int i=0; i<1000; i++) cn= z.ClassName;
//	//for(int i=0; i<1000; i++) cn= z.Name;
//	for(int i = 0; i<1000; i++) cn= z.GetControlText();
//	Speed.Next();
//}
//Speed.Write();
//Out(cn);
//Out(nt);

//Wnd ww = Wnd.Find("Untitled - Notepad");
//ww.MoveInScreen(100, 100);
//return;

//OutFunc();
//Out(FunctionName());
//Output.WriteHex((sbyte)(-10));
//OutList(1, "mmm", true, null, 5.6);
//Out("ff");
//Print(5);
//PrintList(1, 2, 3);
//return;

//Output.Clear();
//Screen s = null;
////Output.Write(s);
////Console.WriteLine(s);
//Info("stri");
//Info(5);
//Info(true);
//Info(new char[] { 'a', 'b' });
//Info(new string[] { "aaa", "bbb" });
//Info(new int[] { 'a', 'b' });
//Info(new Dictionary<string, string>() { { "A", "B" }, { "C", "D" } });
//Out("----------");
//Print("stri");
//Print(5);
//Print(true);
//Print(new char[] { 'a', 'b' });
//Print(new string[] { "aaa", "bbb" });
//Print(new int[] { 'a', 'b' });
//Print(new Dictionary<string, string>() { { "A", "B" }, { "C", "D" } });

////Speed.First(true);
////for(int j=0; j<5; j++) {
////	for(int i = 0; i<1000; i++) Info2("ff");
////	Speed.Next();
////}
////Speed.Write();

//return;




//Wait(2);
//Info("bla");
//Out("bla");
//Say("bla");
//Print("bla");
//OW("bla");

//Info("bla"); Warning("bla"); Error("bla");



//Wnd w = Wnd.Find("Untitled - Notepad");
////Wnd w2 = Wnd.Find("Registry Editor");

////w.MoveInScreen(0, 0, null, limitSize:true, rawXY:false);
////w.EnsureInScreen(null, limitSize:true, workArea:true);

//RECT k=new RECT(0, 1700, 5000, 400, true);
////RECT k=new RECT(-1, -1, 500, 400, true);

////Wnd.RectMoveInScreen(ref k, limitSize:true);
//Wnd.RectEnsureInScreen(ref k, limitSize:true);
//Out(k);

////w=Wnd.Spec.NoTopmost;
//Out(Screen_.FromObject(w));

////Screen s1 = Screen_.FromObject(w);
////Screen s2 = Screen_.FromObject(w);
//Screen s1 = Screen.PrimaryScreen;
//Screen s2 = Screen.PrimaryScreen;
//Out(s1==s2);
//Out(s1.Equals(s2));


//IntPtr hm = DisplayMonitor.GetHandle(w);
////hm=DisplayMonitor.GetHandle(2);
////hm=DisplayMonitor.GetHandle(DisplayMonitor.OfMouse);
////hm=DisplayMonitor.GetHandle(new POINT(2000, 2000));
////hm=DisplayMonitor.GetHandle(new RECT(2000, 2000, 100, 100, true));
//Out(hm);

//for(int z=0; z<2; z++) {
//	Screen[] ad = Screen.AllScreens;
//	foreach(Screen k in ad) {
//		RECT rr = k.Bounds;
//		Out(rr);
//	}
//	Show.MessageDialog("aaa");
//}

//Speed.First(true);
//for(int rep1=0; rep1<5; rep1++) {
//	//for(int rep2=0; rep2<100; rep2++) { RECT u1 = DisplayMonitor.GetRectangle(2); }
//	for(int rep2=0; rep2<100; rep2++) { RECT u2 = ScreenFromIndex(2).Bounds; }
//	Speed.Next();
//}
//Speed.Write();

//Screen k = ScreenFromIndex(1);
//Out(k.Bounds);


//return;

//RECT r1; System.Drawing.Rectangle r2;

//Api.GetWindowRect(w, out r1);
//GetWindowRect(w, out r2);

//OutList(r1, r2);

//return;


//var r1 = new RECT();
//var r2 = r1;
//var r3 = RECT.LTRB(1, 8, 10, 50);
//var r4 = RECT.LTWH(1, 8, 10, 50);
//var r5 = new RECT() { left=2, top=20, Width=2, Height=10 };

//Out(r2==r1);

//Out(r3);
//Out(r4);
//Out(r5);

//return;

//Wnd w = Wnd.Spec.Bottom;
//Out(w.Equals(Wnd.Spec.Bottom));
//Wnd w2 = w;
//Out(w.Equals(w2));
////Wnd w = Wnd0;
////Out(w.Equals(Wnd0));

//Wnd wg = Wnd.Get.FirstToplevel();

//return;

//int eon, x = "ab 99 hjk".ToInt_(2, out eon);
//OutList(x, eon);
//int x = "ab 99 hjk".ToInt_(2);
//int x = " 99 hjk".ToInt_();
//OutList(x);
//int eon, x = " 99 hjk".ToInt_(out eon);
//OutList(x, eon);

//return;

//#if NEWRESULT
//try {
//Thread.Sleep(5000);

//Api.MessageBox(Wnd0, "dd", "ggg", 0x40000);
//return;

//Script.Option.dialogRtlLayout=true;
//Script.Option.dialogTopmostIfNoOwner=true;

//var asm = Assembly.GetEntryAssembly(); //fails if DoCallBack or CreateInstance, OK if ExecuteAssembly
//var asm = Assembly.GetExecutingAssembly(); //OK
//Out(asm!=null);
//Out(asm.Location);
////var rm = new System.Resources.ResourceManager("", asm);
////Out(rm);
//return;

//ScriptOptions.DisplayName="Script display name";
//Out(Assembly.GetEntryAssembly().FullName); //exception

//Wnd ko = Wnd0;
////ko = Wnd.Spec.Topmost;
//Out(ko == null);
//Out(null==ko);
//Wnd? mo = null, mo2=null;
//Out(ko == mo);
//Out(mo == mo2);
//POINT po = new POINT();
//Out(po == null);
////int io = 0;
////Out(io == null);
//IntPtr pi = Zero;
//Out(pi == null);
//return;

//Out(sizeof(WPARAM));

////void* b = (void*)1000000;
////IntPtr b = (IntPtr)(-1);
////UIntPtr b = (UIntPtr)(0xffffffff);
//int b = -1;
////uint b = 0xffffffff;
////byte b = 5;
////sbyte b = -5;
////ushort b = 5;
////short b = -5;
////char b = 'A';
////WPARAM b = -1;

////IntPtr b=(IntPtr)(-1);
////UIntPtr b=(UIntPtr)(0xffffffff);
////uint b = 5;
////char b = 'A';

//LPARAM x;
//x=b;
////x=1000;
//b=x;

////uint u =0xffffffff;
////int x = (int)u;
////WPARAM y = u;

//Out("OK");
//Out($"{x} {b}");
////Out($"{x} {((int)b).ToString()}");
////Out(x==-1);
////Out(x+4);


////string s = " 10 more";
//string s = "-10 more";
////string s = "0x10 more";
////string s = "-0x10";
//s=" 15 text";

////Out(Convert.ToInt32(s));
////Out(int.Parse(s));
////Out(SplitNumberString(ref s));
////Out($"'{s}'");

//int len, r = s.ToInt_(out len);
//Out($"{r} 0x{r:X} {len}");

//string tail;
//r=s.ToInt_(out tail);
//Out($"{r} 0x{r:X} '{tail}' {tail==null}");

//Speed.SpinCPU();
//int i, j, n1=0, n2=0;
//for(j=0; j<4; j++) {
//	Speed.First();
//	for(i=0; i<1000; i++) n1+=int.Parse(s);
//	Speed.Next();
//	for(i=0; i<1000; i++) n2+=ToInt_(s, out len);
//	Speed.NextWrite();
//}
//Out($"{n1} {n2} {len}");

//string[] a = { "one", "two" };
//Out(a);

//var d = new Dictionary<int, int>() { { 1, 1 }, { 2, 2 } };
//Out(d);

//var k = new Dictionary<string, string>() { { "A", "B" }, { "C", "D" } };
////Out(k);
//Output.Write(k);

//Redirect();
//Thread.Sleep(100);

//Output.Writer=new MooWriter();

//Speed.SpinCPU();
//int i, n=10;
//for(int j=0; j<3; j++) {

//	Speed.First();
//	for(i=0; i<n; i++) Out("out");
//	Speed.Next();
//	for(i=0; i<n; i++) Console.WriteLine("con");
//	//Speed.Next();
//	//for(i=0; i<n; i++) Trace.WriteLine("tra");
//	Speed.NextWrite();
//}
//speed: Write unbuffered 35, Console.WriteLine 30, Trace.WriteLine (debug mode) 900

////Speed.First(true);
////Output.AlwaysOutput=true;
//Output.RedirectConsoleOutput();
//Output.RedirectDebugOutput();
////Speed.NextWrite();

////Console.Write("{0} {1}", 1, true);
////return;
//Out("out");
//Console.WriteLine("con");
//Trace.WriteLine("tra");
////Thread.Sleep(1000); try { Console.Clear(); } catch { Out("exc"); }
//Debug.WriteLine("deb");

////Output.Clear();


//var e = new Exception("failed");
//var e = new ArgumentException(null, "paramName");
//var e = new FormatException();
//var e = new InvalidOperationException();
//var e = new NotImplementedException();
//var e = new NotSupportedException();
//var e = new OperationCanceledException();
//var e = new TimeoutException();
//var e = new WaitTimeoutException();
//var e = new WaitTimeoutException(null, new Exception("inner"));
//var e = new WaitTimeoutException();
//Out(e.Message);


//Out(1);
//Input.Key("Ctrl+K");
//Cat.Key("Ctrl+K");

//Show.MessageDialog("dddd");
//Meow.MessageDialog("dddd");

//Test_str();

//Out($"{(int)Control.ModifierKeys:X}");
//Out($"{(int)Keys.Control:X}");
//Out($"{(int)K.Control:X}");
//Keys("");
//Key("");
//SendKeys


//Out((IntPtr)WndSpec.NoTopmost);
//Out(Wnd.Find("Untitled - Notepad"));

//TestUtil.Test_str();

//string s = "file.txt";
//Out(s.likeS("*.txt"));

////Out(s.Reverse());

//switch(s) {
//case "*.txt":
//	Out("txt");
//	break;
//case "*.moo":
//	Out("moo");
//	break;
//default:
//	Out("none");
//	break;
//}

//if(s.likeI("*.txt")) {
//	Out("txt");
//} else if(s.likeI("*.moo")) {
//	Out("moo");
//} else {
//	Out("none");
//}

//if(s.endsWithI(".txt")) {
//	Out("txt");
//} else if(s.endsWithI(".moo")) {
//	Out("moo");
//} else {
//	Out("none");
//}

//if(Regex.IsMatch(s, "one")) {
//	Out("txt");
//} else if(Regex.IsMatch(s, "two")) {
//	Out("moo");
//} else {
//	Out("none");
//}


//Out(K.A);
//Keys("dd");
//Text("uu");

//Trigger.Hotkey["Ctrl+K"] =O=> { Out("lambda"); };
//Trigger.Hotkey["Ctrl+K"] = delegate(HotkeyTriggers.Message m) { Out("delegate"); };

//HotkeyTriggers.TestFireTrigger();

//var k=new TestIndexers();
//Out(k[3]); k[7]=5;
//Out(k[3, 4]); k[7, 2]=5;
//Out(k[3]); k[7]=5;
//Out(k["AAA"]); k["BBB"]="C";
//TestIndexers.Hotkey["test"]="moo";



////var thr=new Thread(AppDomainThread);
////thr.Start();
//AppDomainThread();
////Uuoo(1);
////Uuoo(2);
////Uuoo(3);
//MessageBox.Show("main domain, tid="+Thread.CurrentThread.ManagedThreadId.ToString());


//System.AppDomain.CreateDomain(
//System.Collections.ArrayList k=new System.Collections.ArrayList();
//k.Add(object
//System.Collections.Hashtable t=new System.Collections.Hashtable();
//t.Add(
//System.Collections.Generic.HashSet<

//		return;
//			//Out(OptParam(b:5));

//			//try { Out(1); }catch {}

//			//for(int j=0; j<5; j++)
//			//{
//			//	TestLocal();
//			//	//Out("returned");
//			//	//GC.Collect(0, GCCollectionMode.Forced, true);
//			//	Speed.First();
//			//	GC.Collect();
//			//	Speed.Next();
//			//	//Out("collected");
//			//	GC.WaitForFullGCComplete();
//			//	//Out("waited");
//			//}

//			//long g1, g2;
//			//g1=Stopwatch.GetTimestamp();
//			//Speed.First();
//			////Thread.Sleep(1000);
//			//g2=Stopwatch.GetTimestamp();

//			//Out(g2-g1);
//			//return;

//			string script = @"
//import System
////import System.Runtime.InteropServices
//import Moo
//import Catkeys.Winapi

////[DllImport(""user32.dll"")]
////def MessageBox(hWnd as int, text as string, caption as string, type as int) as int:
////	pass

//static def Main():
//	i =8
//	print ""Hello, World!""
//	api.MessageBox(0, ""text $(i)"", ""cap"", 0);

//	print Class1.Add(1, 2);

//	//print ""Press a key . . . mm""; Console.ReadKey(true)
//";
////static def stringManip(item as string):
////	return ""'${item}' ? What the hell are you talking about ? ""
////";

//			for (int i = 0; i < 1; i++)
//			{
//				Stopwatch sw = new Stopwatch();
//				long t1, t2 = 0, t3 = 0, t4 = 0, t5 = 0;

//				sw.Start();
//				BooCompiler compiler = new BooCompiler();
//				//compiler.Parameters.Input.Add(new StringInput("_script_", "print('Hello!')"));
//				compiler.Parameters.Input.Add(new StringInput("Script", script + "//" + i.ToString()));
//				compiler.Parameters.Pipeline = new CompileToMemory();
//				//compiler.Parameters.Pipeline = new CompileToFile();
//				//compiler.Parameters.Ducky = true;
//				//Out(compiler.Parameters.BooAssembly.FullName);
//				//Out(compiler.Parameters.Debug);
//				compiler.Parameters.Debug = false; //default true; 20% faster when Release
//				//compiler.Parameters.Environment.Provide.
//				//compiler.Parameters.GenerateInMemory=false; //default is true even if new CompileToFile()
//				//Out(compiler.Parameters.GenerateInMemory);
//				//Out(compiler.Parameters.OutputAssembly);
//				//compiler.Parameters.OutputAssembly=@"q:\test\boo.dll";

//				compiler.Parameters.AddAssembly(Assembly.LoadFile(@"Q:\test\Moo.dll")); //ok
//				//compiler.Parameters.LoadAssembly(@"Q:\test\Moo.dll", true); //no effect
//				//compiler.Parameters.LoadReferencesFromPackage(@"Q:\test\Moo.dll"); //error
//				//compiler.Parameters.References.Add(Assembly.LoadFile(@"Q:\test\Moo.dll")); //ok
//				compiler.Parameters.AddAssembly(Assembly.LoadFile(@"C:\Users\G\Documents\SharpDevelop Projects\Test\Winapi\bin\Release\Winapi.dll"));

//				CompilerContext context = compiler.Run();
//				t1 = sw.ElapsedTicks;
//				//Note that the following code might throw an error if the Boo script had bugs.
//				//Poke context.Errors to make sure.
//				if (context.GeneratedAssembly != null)
//				{
//					//SaveAssembly(context.GeneratedAssembly, @"q:\test\boo.exe");
//					//Out(context.GeneratedAssembly.FullName);
//					//Out(context.GeneratedAssembly.EntryPoint.ToString()); //void Main()
//					//Out(context.GeneratedAssembly.);

//					Type scriptModule = context.GeneratedAssembly.GetType("ScriptModule");
//					//Out(scriptModule == null);
//					MethodInfo met = scriptModule.GetMethod("Main");

//					//MethodInfo[] a = scriptModule.GetMethods();
//					//foreach(MethodInfo m in a)
//					//{
//					//Out(m.Name);
//					//}

//					met.Invoke(null, null);

//					//string output = (string)stringManip.Invoke(null, new object[] { "Tag" });
//					//Out(output);
//				}
//				else
//				{
//					foreach (CompilerError error in context.Errors)
//						Out(error);
//				}

//				double f = Stopwatch.Frequency / 1000000.0;
//				Out("speed: {0} {1} {2} {3} {4}", (long)(t1 / f), (long)((t2 - t1) / f), (long)((t3 - t2) / f), (long)((t4 - t3) / f), (long)((t5 - t4) / f));
//			}
//			Out("Press a key . . . ");
//			Console.ReadKey(true);
//}

//static void SaveAssembly(Assembly a, string file)
//{
//	using (FileStream stream = new FileStream(file, FileMode.Create))
//	{
//		BinaryFormatter formatter = new BinaryFormatter();

//		formatter.Serialize(stream, a); //error, assembly not marked as serializable
//	}
//}

#endregion

