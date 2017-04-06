 Run this from QM taskbar button menu when QM hangs.
 Kills QM process. If it is beig debugged, also kills VS.

ShutDownProcess "qm"
err ;;fails if QM is being debugged. Then kill VS.
	mes "failed"
	ret
int pid=ProcessNameToId("qm")
if(!pid) ret
int w=win("app (Running) - Microsoft Visual Studio" "wndclass_desked_gsk" "" 0)
GetWindowThreadProcessId(w &_i)
if(!_i) mes- "VS not found"
ShutDownProcess _i 1

 BEGIN PROJECT
 main_function  shutdown_qm
 exe_file  $my qm$\shutdown_qm.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {26DC4C03-6B3A-4480-9A19-FC864D981E9F}
 END PROJECT
