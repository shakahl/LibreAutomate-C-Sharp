 /exe 1
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

rep 4
	4
	ShowDialog("Dialog100" &Dialog100)

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030307 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 2000 0
	case WM_TIMER
	clo hDlg
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog100
 exe_file  $my qm$\Dialog100.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {D6D03622-66A4-4BD7-BACB-B0A2F3C32757}
 END PROJECT
