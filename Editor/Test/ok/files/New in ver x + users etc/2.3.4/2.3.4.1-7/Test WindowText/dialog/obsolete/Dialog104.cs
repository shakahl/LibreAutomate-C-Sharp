 /exe
 \Dialog_Editor

 this was used to test capturing of menu items


function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog104" &Dialog104 0 0 0 0 0 0 -1 0 0 "Dialog104")) ret

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_LBUTTONUP
	clo hDlg
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN DIALOG
 0 "" 0x90080AC8 0x0 0 0 227 150 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

 BEGIN MENU
 Normal : 101 0 0
 Disabled : 102 0 3
 END MENU




 BEGIN PROJECT
 main_function  Dialog104
 exe_file  $qm$\qmtc_debug.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {5056D1A6-F475-4B0F-B9A6-6973DBB7E73D}
 END PROJECT
