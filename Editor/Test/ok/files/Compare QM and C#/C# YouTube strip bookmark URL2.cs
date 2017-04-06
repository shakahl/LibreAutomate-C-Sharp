#region begin_script
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
//using System.Reflection;
//using System.Windows.Forms;

//using qm;
//using static qm.Output;
using Util;
using static Util.Static;
using Winapi;

public static class ThisScript3 {
[STAThread]static void Main(string[] args) {
#endregion

OnScreenDisplay("Right click the bookmark", 30, 0, 0, 0, 0, 0, 8);
try { WaitFor.MouseRightUp(30); } catch { return; }
Key("i");
WaitFor.WindowActive(30, "Properties for", "MozillaDialogClass");
Key("Alt+l");
string s=Selection.GetText();
s=Regex.Replace(s, "&.+");
Selection.SetText(s);
Key("Enter");
}
}
