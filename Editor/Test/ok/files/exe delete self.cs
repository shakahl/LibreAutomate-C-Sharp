
mes "macro"

 delete this exe
#if EXE=1
str s=
F
 timeout 2
 del "{ExeFullPath}"
 del %0
str tempFile="$temp$\delete my exe.bat"
s.setfile(tempFile)
run tempFile "" "" "" 16
#endif

 BEGIN PROJECT
 main_function  Macro2829
 exe_file  $my qm$\Macro2829.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {CE7FAA90-E127-489B-AC9C-D498D265195B}
 END PROJECT
