#region begin_script
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
//using System.Reflection;
//using System.Windows.Forms;

using Catkeys.Automation;
using static Catkeys.Automation.NoClass;
using Catkeys.Winapi;

public static class ThisScript3 {
	[STAThread]
	static void Main(string[] args) {
#endregion

OnScreenDisplay("Right click the bookmark", 30, 0, 0, 0, 0, 0, 8);
try { WaitFor.MouseRightUp(30); } catch { return; }
Key("i"); //or Keys.I.Send();
int w=WaitFor.WindowActive(30, "Properties for", "MozillaDialogClass");
Key("Alt+l"); //or Keys.Alt.L.Send();
string s=Selection.GetText();
s=Regex.Replace(s, "&.+", "");
Selection.SetText(s);
Key("Enter");


string s=Selection.GetText();
Selection.SetText(s);
//or
string s=Selection.Text;
Selection.Text=s;
//also
Selection.SetAllText(s); //at first presses Ctrl+A
Selection.SetLineText(s); //at first presses Home Shift+End
//or
string s=StringExt.Copy();
s.Paste(); //String extension method implemented in StringExt
}
}
