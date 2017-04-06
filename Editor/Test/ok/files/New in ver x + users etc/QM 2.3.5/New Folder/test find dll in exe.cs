 /exe
 /exe 2

SetCurDir "c:\windows"
str p; GetEnvVar "PATH" p; out p

dll- "$qm$\mailbee.dll" DllGetClassObject
dll- inpout32 Out32

out &DllGetClassObject
out &Out32

 BEGIN PROJECT
 main_function  Macro2092
 exe_file  $my qm$\Macro2092.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {8E8301C0-779A-4B11-83DD-3D9E8781B101}
 END PROJECT
