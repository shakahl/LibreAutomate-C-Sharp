 /exe
 \Dialog_Editor
typelib test_licensed_control {46E38ED9-4630-4EE6-A5F8-69E310FD84CB} 6.0
typelib MSACAL {8E27C92E-1264-101C-8A2F-040224009C02} 7.0

 Simple control that I created with VB6.

if(!ShowDialog("Dialog_VB_AX_Licensed" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 12 18 48 12 "Text"
 15 ActiveX 0x54030000 0x0 6 50 96 48 "test_licensed_control.UserControl1 {D45F8166-E09C-4483-8C76-91122CDCA5E4} data:8EFB7E83010EA8D88985DC4D89DC041302359A260DAECFEC04"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "" "" ""
 14 ActiveX 0x54030000 0x0 122 8 96 48 "test_licensed_control.UserControl1 {D45F8166-E09C-4483-8C76-91122CDCA5E4} data:8"

 4 ActiveX 0x54030000 0x0 60 50 96 48 "test_licensed_control.UserControl1 {D45F8166-E09C-4483-8C76-91122CDCA5E4}"
 5 ActiveX 0x54030000 0x0 0 87 96 48 "SHDocVw.WebBrowser"
 5 ActiveX 0x54030000 0x0 126 2 96 48 "SHDocVw.WebBrowser"
 4 ActiveX 0x54030000 0x0 126 58 96 48 "MSACAL.Calendar {8E27C92B-1264-101C-8A2F-040224009C02}"

 The LICENSE line must be in macro where the control is created (where is ShowDialog, or CreateControl, etc).
 The macro must be encrypted, or the control will not be created.

 BEGIN PROJECT
 main_function  Dialog_VB_AX_Licensed
 exe_file  $my qm$\Dialog_VB_AX_Licensed.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {4C075123-47F1-4DB2-A69F-2C74CFC236FF}
 END PROJECT
