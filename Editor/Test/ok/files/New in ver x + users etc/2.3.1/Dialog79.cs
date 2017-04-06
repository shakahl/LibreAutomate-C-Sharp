 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog79" &Dialog79)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 28 44 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030103 "" "" ""

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
	mac "Function172"
	 out 1
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog79
 exe_file  $my qm$\Dialog79.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {7B6222C7-AB21-4792-B8D0-C3077869B5D1}
 END PROJECT
