 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

int h=ShowDialog("" &exe_thread_2 0 0 1)
RegisterHotKey(h 1000 MOD_CONTROL|MOD_SHIFT VK_END)
MessageLoop
UnregisterHotKey(h 1000)

 BEGIN DIALOG
 0 "" 0xC80044 0x100 0 0 129 99 "Thread2Window"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_HOTKEY
	sel wParam
		case 1000
		out "hotkey"
		PostQuitMessage 0
		shutdown -6 0 "Macro297"
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
