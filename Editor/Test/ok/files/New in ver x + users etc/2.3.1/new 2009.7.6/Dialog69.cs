 /exe 1
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

tls5=1
if(!ShowDialog("Dialog69" &Dialog69)) ret

 BEGIN DIALOG
 0 "" 0x90C80A40 0x110 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030100 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_DROPFILES
	out 1
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog69
 exe_file  $my qm$\Dialog69.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {230AAD89-F219-4216-B01F-4AEE4E193B14}
 END PROJECT
