if(GetMod&2) _s="qmexe"; wait 0 K C
else _s="app"
VC_CompileProject _s

 BEGIN PROJECT
 main_function  Build_app
 exe_file  $my qm$\Build app or qmexe (Ctrl).exe
 icon  $qm$\icons\compile.ico
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {FE1A2FA3-F527-4832-8650-06A76F0C6642}
 END PROJECT
