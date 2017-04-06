\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_buttons_with_icons" &dlg_buttons_with_icons 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 22 46 54 14 "Button"
 4 Button 0x54032400 0x0 80 46 36 26 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x203000C "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__Hicon-- t_hi1=GetFileIcon("shell32.dll,15")
	SendMessage id(3 hDlg) BM_SETIMAGE IMAGE_ICON t_hi1
	SendMessage id(4 hDlg) BM_SETIMAGE IMAGE_ICON t_hi1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
