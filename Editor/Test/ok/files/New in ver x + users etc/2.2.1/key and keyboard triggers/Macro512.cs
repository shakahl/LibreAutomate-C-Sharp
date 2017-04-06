int uni=0
str s=iif(_win64 "q:\app\x64\qmshex.dll" "q:\app\qmshex.dll")
if(!RegisterComComponent(s uni|6)) mes- "Failed to %sregister %s." "" "!" iif(uni "un" "") s

 BEGIN PROJECT
 main_function  Macro512
 exe_file  $qm$\x64\regqmshex.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {05C23ACF-6B8E-4F3A-960F-6DB67A925309}
 END PROJECT
