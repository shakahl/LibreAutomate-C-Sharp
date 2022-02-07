/// Most "wait for" functions are in separate classes. For example "wait for window" is in <b>wnd</b>, "wait for key" is in <b>keys</b>. To wait for other events can be used class <see cref="wait"/>.

/// Wait for toggled CapsLock.

wait.forCondition(0, () => keys.isCapsLock);
print.it("continue");

/// Wait for file. Then wait max 10 s until it does not exist; on timeout throw exception.

var file = @"C:\Test\file.txt";
wait.forCondition(0, () => filesystem.exists(file));
print.it("created");
wait.forCondition(10, () => !filesystem.exists(file));
print.it("deleted");

/// Wait max 5 s for process; on timeout exit. Then wait until does not exist.

if (!wait.forCondition(-5, () => 0 != process.getProcessId("notepad.exe"))) return;
print.it("started");
wait.forCondition(0, () => 0 == process.getProcessId("notepad.exe"));
print.it("ended");

/// Wait for variable.

bool m = false;
Task.Run(() => { 2.s(); m = true; }); //an example that executes a task in another thread and sets the variable when finished
wait.forVariable(0, m);
print.it("continue");
