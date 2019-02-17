/*/ ifRunning restart; r System.Numerics; */ //{{
//{{ using
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using Au;
using Au.Types;
using static Au.NoClass;
using Au.Triggers;
//}}
using System.Numerics;

//{{ class, Main
unsafe partial class App :AuApp { //}}
[STAThread] static void Main(string[] args) { new App()._Main(args); }
void _Main(string[] args) { //}}
//}}
//}}

/*
In this program is used standard C# syntax. You don't see the class, Main(), usings, etc because
it is folded (hidden) in the code editor. You can click the [+] box at the top-left to unhide it.
Everything else is optional. A script can have just single visible code line below the first line,
like Print("hello");. This script shows where to add more code, for example usings and functions.

This program saves script properties and references in meta comments, at the very start of code.
You can change them in the Properties dialog. The /*​/ ifRunning restart; ...*​/ is an example.

You can add 'using ...' and [assembly: ...] below the '//{{ using' code block in the folded
code (click the [+] box to show it). The 'using System.Numerics;' is an example.
You can add '#define ...' and 'extern alias ...' above the '//{{ using' code block.

As you can see, your main script code actually is in function _Main, which is in class App.
The closing } } below your script code are optional (this program adds them when compiling).

You can add more functions below or in main script code.
If below, insert single } between them (it ends the _Main function). The functions actually are
member function of class App. Fields (member variables) are shared by _Main and all functions.
Else the function is a nested function. Its local variables are shared with nested functions.

Below main script code you also can add fields, nested classes, [DllImport] declarations and
everything else that can be used at class level.

*/

Print("Main script code is here.");
ExampleFunction("function"); //call function
int localVariable = 2;
ExampleNestedFunction("nested function"); //call nested function
var v = new NestedClass(); v.Function2(); //call function of other class
Print(GetTickCount()); //call dll function

//Nested function example. Can use local variables declared above.
void ExampleNestedFunction(string parameter) {
	Print(parameter, localVariable);
}

//{{
} //ends the _Main function. The comment line above is recommended to balance folding.

//Field examples. These fields are member variables of class App.
int _field = 3; //visible in all functions of class App, but not directly visible in nested classes
static int s_staticField = 10; //static and const fields are directly visible in nested classes too

//Function example. It is a member of class App. Can use fields, but not local variables of _Main.
void ExampleFunction(string parameter) {
	Print(parameter, _field);
}

//Nested class example.
class NestedClass {
	public void Function2() {
		Print(s_staticField);
	}
}

//Other class-level declaration example.
[DllImport("kernel32.dll")]
static extern int GetTickCount();

//{{
} //ends class App. Optional. Below you can add more classes.

//When compiling, this program adds:
//1. References: mscorlib, System, System.Core, System.Windows.Forms, System.Drawing, Au.dll.
//2. Attributes: [module: DefaultCharSet(CharSet.Unicode)], some version attributes.

//This program uses C# version 7.3 and .NET Framework version 4.7.2.
