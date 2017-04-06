 qm.Tray tested
 qm.deb tested
 qm.net tested
 qm.shutdown tested

 qm.Curtain "text"
 5

 out PopupMenuK("&one[]&two")
 rep(10) out GetCPU; 0.5
 rep(2) GetDiskUsage; 0.5 ;;requires admin. To test, run antivirus scan

 int w=win("Notepad")
 int hi=GetWindowIcon(w)
 out hi ;;OK with admin too
 CloseHandle hi

 ShutDownProcess "notepad" ;;OK with admin too

 qm.RegisterComComponent tested

 qm.SetPrivilege
