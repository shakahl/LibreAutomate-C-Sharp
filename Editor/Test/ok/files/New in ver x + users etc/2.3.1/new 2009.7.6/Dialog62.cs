 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog62" &Dialog62)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 14 18 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030100 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	_i/0
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog62
 exe_file  $my qm$\Dialog62.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {0E1D7672-1E3B-437B-B479-19EB662230B4}
 END PROJECT
