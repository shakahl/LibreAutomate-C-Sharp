/*/ ifRunning warn_restart; /*/ //.
using Au; using Au.Types; using System; using System.Collections.Generic;
class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;

/*
This file is an example of an automation script.

The programming language is C#.

In scripts you can use classes/functions of the automation library provided by
this program, as well as of .NET Core and everything that can be used in C#.
Also you can create and use new functions, classes, .NET libraries, .exe programs.

This program saves script properties in meta comments /*/ /*/, at the very start
of script code. You can change them in the Properties dialog.

Like all C# programs, a script starts with standard code: several 'using', a class
and function Main (from it starts program execution). Click the small [+] box at
the top-left to show and edit that code when need (add more 'using' etc).

To avoid 'static' everywhere, function Main creates a class instance. Your script
code is in the constructor function Script(string[] args). Finally the function
and the class end with } and }.

The //. and //; are used to fold (hide) code lines like #region and #endregion.

To run a script, you can click the Run button on the toolbar, or use command line,
or launch from a script like ATask.Run("script5.cs"). There are no script triggers
like hotkey; instead a script can contain many such triggers that work only when
the script is running. Example: Example: script "Hotkeys and other triggers".
*/

//Examples of automation functions.

AOutput.Write("Main script code.");

ADialog.Show("Message box.");

AExec.Run(AFolders.System + "notepad.exe");
var w = AWnd.Wait(0, true, "* - Notepad");
AKeys.Key("F5 Enter*2");
AKeys.Text(w.Name);
2.s();
w.Close();
var w2 = AWnd.Wait(-3, true, "Notepad", "#32770");
if(!w2.Is0) {
	500.ms();
	var c = w2.Child(null, "Button", skip: 1).OrThrow(); // "Don't Save"
	AMouse.Click(c);
	500.ms();
}

//Examples of .NET functions.

string s = "Example";
var b = new System.Text.StringBuilder();
for(int i = 0; i < s.Length; i++) {
	b.Append(s[i]).AppendLine();
}
System.Windows.Forms.MessageBox.Show(b.ToString());

//Example of your function and shared variable.

_sharedVariable = 1;
FunctionExample("Example");
AOutput.Write(_sharedVariable);

} //end of main function

//Here you can add functions, shared variables (class fields), nested classes, struct, enum, [DllImport], etc.

void FunctionExample(string s) {
	AOutput.Write(s, _sharedVariable);
	_sharedVariable++;
}

int _sharedVariable;

} //end of class
