There are 2 item types that can contain code: script and library.

SCRIPT
Scripts are executable.
Each script runs in a separate AppDomain. Or can be converted to exe.
Each AppDomain or exe can have 1 or more its private threads.
Classes and namespaces defined in a script are private to the scipt (cannot be shared with other scripts).
To share global variables between domains, use InterDomain class or shared memory.

LIBRARY
Libraries contain classes and namespaces that can be used in scripts and other libraries.
A library is compiled to a dll, which can be a temporary dll (used in scripts running in QM) or normal dll (can be used in exe).
Libraries are not executable.

 _________________________________________________________

Triggers, menus, toolbars and autotexts are defined in scripts. To make esier to define, use attributes, tools, maybe a custom preprocessor.
Other item types - folder and file-link. Maybe also resource, table.
At startup load assemblies of scripts containing triggers etc.

 _________________________________________________________

Scripts and libraries are separate text files.
Scripts and libraries can have multiple files: menu New -> Add file to this script/library.
A script file can have multiple executable scripts (functions). Each of them can have a trigger.
The main file just contains links to these files. Also the folder structure, some settings, cached data, user-defined tables, maybe some resources.

 _________________________________________________________

To make scripting easier, when compiling (or/and when editing):
If something not found, automatically find the namespace/reference and add to the script.
Automatically insert header (class, Main) and foother (}}). Also use templates with it.
When 'public' missing, show more informative error description.

For example, we could post to the forum just script body. On paste or first-time-compile would be automatically added references, usings, and created class and Main().

 _________________________________________________________

//C# script example:

#region begin_script
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
//using System.Linq;
//using System.IO;
//using System.Runtime.InteropServices;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Automation;
using Auto = Catkeys.Automation;
using static Catkeys.Automation.NoClass;
using Catkeys.Triggers;

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
