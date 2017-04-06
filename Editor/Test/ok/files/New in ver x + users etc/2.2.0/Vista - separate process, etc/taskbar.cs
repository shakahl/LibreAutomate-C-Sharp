 /exe 1
Acc a=acc("Safely Remove Hardware" "PUSHBUTTON" win("" "Shell_TrayWnd") "ToolbarWindow32" "" 0x1001)
a.Mouse(1)

 BEGIN PROJECT
 main_function  Macro455
 exe_file  $my qm$\Macro455.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {C4B8D05B-B1BB-44AE-BE2A-49930EBCAB2D}
 END PROJECT
