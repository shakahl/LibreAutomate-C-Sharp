int ro
sel mes("Make System.qml read-only?[][]This will restart QM." "" "YNC")
	case 'Y' ro=1
	case 'N'
	case else ret

int w=win("" "QM_Editor")
if w
	PostMessage w WM_CLOSE 1 0
	wait 0 WP w

str sf="q:\app\system.qml"
SetAttr sf FILE_ATTRIBUTE_READONLY 2

Sqlite x.Open(sf)
x.Exec(F"PRAGMA journal_mode={iif(ro `DELETE` `WAL`)}")
x.Close

if(ro) SetAttr sf FILE_ATTRIBUTE_READONLY 1

if w
	run "q:\app\qm.exe" "v"

 BEGIN PROJECT
 main_function  System read-only
 exe_file  $qm$\System read-only.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  4
 guid  {76620DF9-3AC6-4306-A27C-7340D1F437AA}
 END PROJECT
