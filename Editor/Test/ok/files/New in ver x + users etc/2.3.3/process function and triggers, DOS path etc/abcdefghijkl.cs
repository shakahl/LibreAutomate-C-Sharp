 /exe
\Dialog_Editor

if(!ShowDialog("abcdefghijkl" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

 BEGIN PROJECT
 main_function  abcdefghijkl
 exe_file  $my qm$\abcdefghijkl.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {330429B5-386D-4519-9D75-6A85B10EB3A2}
 END PROJECT
