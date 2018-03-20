#region begin_script
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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;
//using System.Xml.XPath;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Triggers;

//[Script.Options(reuseAppdomain=true, ...)]
class ScriptClass :Script
{
	//static ScriptClass() {} //script appdomain initialization
	//ScriptClass() {} //script instance initialization
	[STAThread] static void Main() { new ScriptClass().CallFirstMethod(); } //runs in exe or when script launched not with a trigger
	#endregion

	[Trigger.Hotkey("Ctrl+K")]
	void Function1(Trigger.HotkeyData t)
	{
		Out("Hello World!");
	}

} //end of ScriptClass
