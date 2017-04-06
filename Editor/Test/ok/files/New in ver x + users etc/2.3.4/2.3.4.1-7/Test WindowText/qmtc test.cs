 /exe
out
dll- "$qm$\qmtc32.dll"
	%__QMTC_Test

int w=win("" "QM_Editor")
SetProp(w "qmtc_debug_output" 1)
__QMTC_Test

 BEGIN PROJECT
 main_function  qmtc test
 exe_file  $qm$\qmtc_debug.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {5056D1A6-F475-4B0F-B9A6-6973DBB7E73D}
 END PROJECT
