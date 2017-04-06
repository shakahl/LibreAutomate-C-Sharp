Wsh.WshShell s._create
Wsh.WshShortcut a=+s.CreateShortcut(_s.expandpath("$desktop$\lo.lnk"))
a.TargetPath="c:\windows"
a.Save
