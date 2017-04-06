/exe 1
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages


if(!ShowDialog("Dialog31" &Dialog31)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Calculator"
 3 Edit 0x54030080 0x200 8 12 96 14 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 4 Button 0x54032000 0x0 8 34 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020006 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 SetTimer hDlg 1 300 0
	case WM_TIMER
	 KillTimer hDlg wParam
	 SetWindowPos hDlg 0 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 SetWindowPos hDlg 0 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOSENDCHANGING
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog31
 exe_file  $my qm$\Dialog31.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {16B51F4B-50CE-4015-9BC0-9C0C9D3832B6}
 END PROJECT
