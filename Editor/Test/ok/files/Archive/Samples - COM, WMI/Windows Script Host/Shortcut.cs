 Creates shortcut to the WINDOWS folder on the desktop

str s.expandpath("$desktop$\WSH Windows.lnk")
Wsh.IWshShell_Class shell._create
Wsh.IWshShortcut a=shell.CreateShortcut(s)
a.TargetPath=s.expandpath("$windows$")
a.Save
