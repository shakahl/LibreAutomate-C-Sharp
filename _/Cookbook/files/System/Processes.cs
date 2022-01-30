/// A process is a running program. It can have visible windows or not. Also known as <i>task</i>. Use class <help>process<>.

/// Print names of all processes of this user session.

print.clear();
var a = process.allProcesses(ofThisSession: true);
foreach (var v in a) {
	print.it(v.Name);
}

/// Terminate, suspend and resume all "notepad.exe" processes (end tasks).

process.terminate("notepad.exe");
process.suspend(true, "notepad.exe");
process.suspend(false, "notepad.exe");

/// If process does not exist.

if (0 == process.getProcessId("notepad.exe")) {
	print.it("does not exist");
}

/// Wait for a "notepad.exe" process.

wait.forCondition(0, () => 0 != process.getProcessId("notepad.exe"), new OWait(1));

/// Wait until there are no "notepad.exe" processes.

wait.forCondition(0, () => 0 == process.getProcessId("notepad.exe"));

/// Get window process id and terminate its process.

var w1 = wnd.find(1, "*- Notepad", "Notepad");
int pid = w1.ProcessId;
process.terminate(pid);

/// Get window process name.

var w2 = wnd.find(1, "*- Notepad", "Notepad");
var program = w2.ProgramName;

/// Get name and path of current process (the script process).

print.it(process.thisExeName, process.thisExePath);

/// Get script name.

print.it(script.name);

/// When need to get more process properties, use class <google C# class Process>Process</google>.
