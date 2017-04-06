 /exe
\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030506 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam
__RegisterHotKey- t_hk1 t_hk2
sel message
	case WM_INITDIALOG
	if(!t_hk1.Register(hDlg 1 MOD_ALT 'Z')) end "failed to register a hotkey"
	if(!t_hk2.Register(hDlg 2 MOD_ALT 'X')) end "failed to register a hotkey"
	
	case WM_HOTKEY
	sel wParam
		case 1 sub.WaitNoKeys; out "Alt+Z"
		case 2 sub.WaitNoKeys; out "Alt+X"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub WaitNoKeys

 Waits until released Ctrl, Shift, Alt and Win keys of a hotkey.

opt waitmsg 1
rep
	if(!GetMod) break
	0.01

 BEGIN PROJECT
 main_function  dialog_hotkeys2
 exe_file  $my qm$\dialog_hotkeys2.qmm
 flags  6
 guid  {874CCE74-343D-418B-96BF-C9F95D7E6698}
 END PROJECT
