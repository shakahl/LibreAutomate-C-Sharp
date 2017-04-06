
 mes __FUNCTION__
 CreateProcessSimple "\\gintaras\q\app\Macro2468.exe"
CreateProcessSimple "c:\windows\system32\cmd.exe /c start \\gintaras\q\app\Macro2468.exe"
 ShellExecute 0 0 "c:\windows\system32\cmd.exe" "/c start \\gintaras\q\app\Macro2468.exe" 0 0

 BEGIN PROJECT
 main_function  Function298
 exe_file  $my qm$\Function298.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {650C1D82-D886-4AE1-956B-1E0DB35BE37A}
 END PROJECT
