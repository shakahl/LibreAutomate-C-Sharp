 /exe
\Dialog_Editor
typelib Project2 {EB35919D-393D-4471-B747-C28F5DED9BCF} 2.0

 Simple control that I created with VB6.

 #exe addactivex "Project2.UserControl1 {E46F9BB9-8BD9-4AAA-BD7D-1DFD779EC13F}"

if(!ShowDialog("Dialog_VB_AX_Simple" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 12 18 48 12 "Text"
 4 ActiveX 0x54030000 0x0 0 87 96 48 "Project2.UserControl1 {E46F9BB9-8BD9-4AAA-BD7D-1DFD779EC13F}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

 BEGIN PROJECT
 main_function  Dialog_VB_AX_Simple
 exe_file  $my qm$\Dialog32.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  7
 end_hotkey  0
 guid  {51C87514-6B08-4A71-A86F-8F4413BE7A45}
 END PROJECT
