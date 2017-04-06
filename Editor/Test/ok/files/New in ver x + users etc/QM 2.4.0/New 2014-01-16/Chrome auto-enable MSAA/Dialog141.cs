 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog141" &Dialog141)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040000 "" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	NotifyWinEvent(EVENT_SYSTEM_ALERT hDlg 1 0)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog141
 exe_file  $my qm$\Dialog141.qmm
 flags  6
 guid  {86A47326-BDF9-4507-B28D-CCA06B7395D2}
 END PROJECT
