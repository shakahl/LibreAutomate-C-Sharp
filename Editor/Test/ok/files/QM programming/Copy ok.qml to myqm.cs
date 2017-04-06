#if EXE=1

men 2003 _hwndqm ;;Save All Now

str s1="\\vmware-host\Shared Folders\Q\app\ok.qml"

OnScreenDisplay "copying..." -1

cop- s1 "$my qm$"
s1+"-wal"; if(FileExists(s1)) cop- s1 "$my qm$"
OsdHide

sel list("Run QM in PF[]Run QM in app")
	case 1
	run "$pf$\quick macros 2\qm.exe" "v"
	case 2
	s1.getpath; s1+"qm.exe"
	run s1 "v"


 BEGIN PROJECT
 main_function  Copy ok.qml to myqm
 exe_file  $desktop$\Copy ok.qml to myqm.exe
 icon  $qm$\qm.exe,0
 manifest  $qm$\default.exe.manifest
 flags  4
 guid  {E4FAC339-FCA5-4666-A6FC-8E7E32317B25}
 END PROJECT
