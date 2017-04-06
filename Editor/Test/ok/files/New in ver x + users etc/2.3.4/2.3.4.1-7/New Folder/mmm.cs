 /exe
out _s.getmacro("mmm")

 BEGIN PROJECT
 main_function  mmm
 exe_file  $my qm$\mmm.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {90B8BEF8-B93F-4E2C-B735-7100E1B7C6CD}
 END PROJECT
type TYRE ~controls ~e3
TYRE d.controls="3"
if(!ShowDialog("Dialog157" 0 &d)) ret
