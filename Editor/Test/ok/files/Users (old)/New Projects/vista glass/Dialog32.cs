\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

ref WINAPI2

if(!ShowDialog("Dialog32" &Dialog32)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x202000A "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DWM_BLURBEHIND b
	b.dwFlags=DWM_BB_ENABLE
	b.fEnable=1
	DwmEnableBlurBehindWindow(hDlg &b)

	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
