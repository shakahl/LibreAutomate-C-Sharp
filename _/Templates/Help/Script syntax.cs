/*/ ifRunning restart; r System.Numerics; /*/ //{{
//{{ using
using System; using System.Collections.Generic; using System.Text; using System.Text.RegularExpressions; using System.Diagnostics; using System.Runtime.InteropServices; using System.IO; using System.Threading; using System.Threading.Tasks; using System.Windows.Forms; using System.Drawing; using System.Linq; using Au; using Au.Types; using static Au.NoClass; using Au.Triggers; //}}

using System.Numerics; //example. Add usings and assembly/module attributes here.

//{{ main
unsafe partial class Script :AuScript { [STAThread] static void Main(string[] args) { new Script()._Main(args); } void _Main(string[] args) { //}}//}}//}}//}}

/*
To create automation scripts in this program you use C# as the programming language.
Scripts actually are regular C# programs. Click the small [+] box at the top-left,
and you can see code familiar to C# programmers - several usings, class Script and
function Main (from it starts script execution). To avoid using 'static' everywhere,
function Main creates a class instance and calls function _Main, where is your main
script code. Usually don't need to edit the folded (hidden) code. Let it be hidden,
and write your script below it. Your script can optionally end with one or two },
to end function _Main and class Script.

The //{{ and //}} are used to fold (hide) code lines. Like #region and #endregion.

This program saves script properties and used references in /*​/ meta comments /*​/,
at the very start of script file. You can change them in the Properties dialog.

You can add more functions to the script in several ways:
1. Directly in your script (function _Main), as nested functions.
2. Below function _Main, as class member functions. Insert } to end function _Main.
3. In class files. Create a class file (menu File -> New ...) and use the Properties
dialog of the script to add it. Or create a script project (menu File -> New ...)
and add class files to the project folder.

Below function _Main and } you can also add fields, nested classes, [DllImport], etc.

You can add 'using' directives, assembly/module attributes and #define below or above
the '//{{ using' code block. Click the [+] box to show it.
*/

Print("Main script code is here.");
ExampleFunction("function"); //call function
int localVariable = 2;
ExampleNestedFunction("nested function"); //call nested function
var v = new NestedClass(); v.Function2(); //call function of other class
Print(GetTickCount()); //call the declared dll function

//Nested function example. Can use local variables declared above.
void ExampleNestedFunction(string parameter) {
	Print(parameter, localVariable);
}

//{{
} //ends function _Main. The comment line above is recommended to balance folding.

//Field examples. These fields are member variables of class Script.
int _field = 3; //visible in all functions of class Script, but not directly visible in nested classes
static int s_staticField = 10; //static and const fields are directly visible in nested classes too

//Function example. It is a member of class Script. Can use fields, but not local variables of _Main.
void ExampleFunction(string parameter) {
	Print(parameter, _field);
}

//Nested class example.
class NestedClass {
	public void Function2() {
		Print(s_staticField);
	}
}

//Other declaration example.
[DllImport("kernel32.dll")]
static extern int GetTickCount();

//{{
} //ends class Script. Optional. Below you can add more classes.

//When compiling, this program adds:
//1. References: mscorlib, System, System.Core, System.Windows.Forms, System.Drawing, Au.dll.
//2. Attributes: [module: DefaultCharSet(CharSet.Unicode)], some version attributes.

//This program uses C# version 7.3 and .NET Framework version 4.7.2.
