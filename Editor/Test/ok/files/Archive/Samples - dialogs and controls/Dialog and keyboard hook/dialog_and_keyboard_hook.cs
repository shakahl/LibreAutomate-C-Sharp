\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_and_keyboard_hook" &dialog_and_keyboard_hook)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x54000000 0x0 8 10 48 12 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030506 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int- t_hdlg t_kh
	t_hdlg=hDlg
	t_kh=SetWindowsHookEx(WH_KEYBOARD_LL &DAKH_HookProc _hinst 0)
	
	case WM_DESTROY
	UnhookWindowsHookEx t_kh
	
	case WM_APP+10
	FormatKeyString lParam 0 &_s
	_s.setwintext(id(3 hDlg))
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
