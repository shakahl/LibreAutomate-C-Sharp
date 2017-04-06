 out SetPrivilege("SeDebugPrivilege")

 int w=win("Notepad")
 rep(10) out GetProcessCpuUsage(w 500 0) ;;works, although SetPrivilege("SeDebugPrivilege") fails. Probably because of PROCESS_QUERY_X_INFORMATION.

 sys.Dde - not tested

 sys.RegKey rk ;;tested using the following
 foreach(_s "Software\GinDi\QM2\settings" FE_RegKey HKEY_LOCAL_MACHINE) out _s
 str s
 foreach s "Software" FE_RegKey HKEY_LOCAL_MACHINE 1
	 out s

 sys.SetDefaultPrinter("Fax") ;;works, on nonadmin too
 sys.DisplaySettings ;;tested to enum, not tested to change

 sys.system("start notepad.exe")

 sys._putenv("test=80")
 out sys.getenv("test")
 out sys.getenv("PATH")

 out share("+ConsoleWindowClass") ;;does not work with console, works with others, including admin and 64-bit

 others tested
