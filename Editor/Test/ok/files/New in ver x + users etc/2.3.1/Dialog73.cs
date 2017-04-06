 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("Dialog73" &Dialog73 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 40 62 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030102 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog73
 exe_file  $my qm$\Dialog73.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {D58CC428-7FB6-4AA2-BF06-209B3F67EA57}
 END PROJECT
