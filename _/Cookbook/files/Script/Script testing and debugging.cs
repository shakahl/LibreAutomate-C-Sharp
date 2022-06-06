/// When creating, testing and debugging scripts and other code, often you want to see what code parts are executed, what are values of variables there, etc. In most cases for it can be used <see cref="print.it"/>. You can insert it temporarily, and later delete or disable the code line.

print.it("Script started");
Test(5);
print.it("Script ended");

void Test(int i) {
	print.it("Test", i);
	//print.it("disabled");
}

/// To get current stack of functions, use <see cref="StackTrace"/>.

F1();
void F1() { F2(); }
void F2() { F3(); }
void F3() { print.it(new StackTrace(0, true)); }

/// Sometimes it's difficult to debug with just the above functions. You may want to execute some script parts in step mode (one statement at a time), and in each step see variables, stack, etc. Then need a debugger. This program does not have a debugger, but you can use debuggers of Visual Studio or Visual Studio Code. At first need to attach a debugger to the script's process. Let the script call <see cref="script.debug"/> (<u>click the link for more info<>) or <see cref="Debugger.Launch"/> (only with Visual Studio). Then to start step mode call <see cref="Debugger.Break"/>. To insert code can be used menu Run -> Debugger. When the debugger is attached, it displays script code, and you can click its debug toolbar buttons to execute steps etc. You can click its left margin to set breakpoints. Also it will break on exceptions.

script.debug(); //prints process name and id, and waits until you attach a debugger
Debugger.Break(); //starts step mode
print.it(1);
print.it(2);
print.it(3);

/// See also classes in namespace <see cref="System.Diagnostics"/> and its child namespaces.

script.setup(debug: true);
// ...
Debug.Assert(false);
