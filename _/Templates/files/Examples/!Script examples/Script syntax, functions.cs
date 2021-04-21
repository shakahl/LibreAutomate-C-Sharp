/*/ ifRunning warn_restart; /*/ //.
using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
using System.Windows.Forms;
class Script { [STAThread] static void Main(string[] a) { ATask.Setup(); new Script(a); } Script(string[] args) { //;;;

/*
The programming language is C#.

In scripts you can use classes/functions of the automation library provided by
this program, as well as of .NET and everything that can be used in C#.
Also you can create and use new functions, classes, libraries and .exe programs.

Like all C# programs, a script starts with standard code: using directives,
class and function Main where the program starts. Click the small [+] box at
the top-left to see and edit that code when need. The //. and //; are used to
fold (hide) code.

To avoid 'static' everywhere, function Main creates a class instance. Your script
code is in the constructor function. The function and the class end with } and }.

Script properties are saved in /*/ /*/ comments at the very start of script.
You can change them in the Properties dialog or edit in script.
More properties can be set in code with various functions. For example, to
remove tray icon, replace ATask.Setup(); with ATask.Setup(trayIcon: false);.
To change default properties and code for new scripts: Options -> Templates.

To run a script, you can click the ► Run button on the toolbar, or use command line,
or call ATask.Run from another scrit, or in Options set to run at startup.

Triggers such as hotkeys, autotext, mouse and window are used to execute functions
in a running script. Also you can create custom toolbars and menus. To start
using them: menu File -> New -> Examples -> @Triggers and toolbars.
*/

//Examples of automation functions.

AOutput.Write("Main script code.");

ADialog.Show("Message box.");

AFile.Run(AFolders.System + "notepad.exe");
var w = AWnd.Wait(0, true, "*- Notepad");
AKeys.Key("F5 Enter*2");
AKeys.Text(w.Name);
2.s();
w.Close();
var w2 = AWnd.Wait(-3, true, "Notepad", "#32770");
if (!w2.Is0) {
	500.ms();
	var c = +w2.Child(null, "Button", skip: 1); // "Don't Save"
	AMouse.Click(c);
	500.ms();
}

//Examples of .NET functions.

string s = "Example";
var b = new System.Text.StringBuilder();
for (int i = 0; i < s.Length; i++) {
	b.Append(s[i]).AppendLine();
}
MessageBox.Show(b.ToString());

//Example of your function and how functions can share variables.

_sharedVariable = 1;
FunctionExample("Example");
AOutput.Write(_sharedVariable);

} //end of main function

//Here you can add functions, shared variables (fields), nested classes, struct, enum, [DllImport], etc.

void FunctionExample(string s) {
	AOutput.Write(s, _sharedVariable);
	_sharedVariable++;
}

int _sharedVariable;

} //end of class
