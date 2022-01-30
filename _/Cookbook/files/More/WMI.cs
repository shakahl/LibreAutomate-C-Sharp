/// Use assembly System.Management.dll. It's in the editor's folder. <google C# System.Management Namespace>More info</google>.

/*/ r System.Management.dll; /*/
using System.Management;

/// Create process.

var pr = new ManagementClass("Win32_Process");
pr.InvokeMethod("Create", new[] { "Notepad.exe" });

/// Get properties of all processes.

print.clear();
var scope = new ManagementScope(); scope.Connect();
var query = new ObjectQuery("SELECT * FROM Win32_Process");
var searcher = new ManagementObjectSearcher(scope, query);
foreach (var m in searcher.Get()) {
	print.it($"{m["Name"],-30}  {m["CommandLine"]}");
}

/// Watch process start events.

using var watcher = new ManagementEventWatcher("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance isa \"Win32_Process\"");
watcher.Options.Timeout = TimeSpan.FromSeconds(30);
using var osd = osdText.showTransparentText("Open 2 applications to trigger events", -1);
for (int i = 0; i < 2; i++) {
	var e = watcher.WaitForNextEvent();
	print.it(((ManagementBaseObject)e["TargetInstance"])["Name"]);
}
