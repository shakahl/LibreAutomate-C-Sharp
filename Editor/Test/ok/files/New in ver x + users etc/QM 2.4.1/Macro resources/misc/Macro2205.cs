 /exe
 mes "" "" "q"



 BEGIN PROJECT
 main_function  Macro2205
 exe_file  $my qm$\Macro2205.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  7
 guid  {183D2381-1CBE-4031-81F0-6C8F607BB6F6}
 END PROJECT
 icon  resource:<Macro2205>1.ico
_s="resource:1 info.ico"
 _s="resource:<Macro2202>a3d_dll_0.ico"
 _s="resource:<{96668C01-53AA-4C5B-AF22-AD939C277248}Macro2202>a3d_dll_0.ico"

if(!ShowDialog("Macro2205" 0)) ret
 if(!ShowDialog("Macro2205" 0 0 0 0 0 0 0 0 0 "resource:<Macro2202>a3d_dll_0.ico")) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "*" "" "" ""
