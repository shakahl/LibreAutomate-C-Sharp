/// The <link https://docs.microsoft.com/en-us/dotnet/csharp/>C# programming language<> is used to create scripts in this program. It is one of the <google programming language popularity>most popular<> languages.
/// 
/// Don't need to learn C# to start creating automation scripts. Use the input recorder and other tools in the Code menu. But with some C# knowledge you can do much more.
/// 
/// This script displays string "example" in the program's Output panel. It calls function <b>it</b> of class <b>print</b>.

print.it("example");

/// This script contains 2 statements. The //text is comments.

var s = "Some text."; //create variable s
dialog.show("Example", s); //show message box

/// Example with statements <.k>if<>, <.k>return<> (exit) and operator <.k>!<> (NOT).

if (!dialog.showOkCancel("Example", "Continue?")) {
	print.it("Cancel");
	return;
}
print.it("OK, let's continue.");

/// Use the <.k>for<> statement to execute code more than once.

for (int i = 0; i < 3; i++) {
	print.it(i);
}

/// Another way to execute code more than once - user-defined functions.

//call function Example 2 times
Example("one", 1);
Example("two", 2);

//this is the function
void Example(string s, int i) {
	print.it(s + " " + i);
}
