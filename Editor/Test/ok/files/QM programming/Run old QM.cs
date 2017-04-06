#if EXE
str rf; rget rf "file" "Software\GinDi\QM2\settings"
rset "Q:\app\ok old-QM2.3.qml" "file" "Software\GinDi\QM2\settings"

run "Q:\app - replace CDataArray to QmItems\qm.exe" "v"
 note: don't use run flag 0x400 because restarts to elevate
int w=wait(30 WC win("" "QM_Editor"))
wait 0 WP w

err+
rset rf "file" "Software\GinDi\QM2\settings"

 BEGIN PROJECT
 main_function  Run old QM
 exe_file  $my qm$\Run old QM.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  4
 guid  {DE85A468-9CBA-4AC8-B727-DBF49B0929FA}
 END PROJECT
