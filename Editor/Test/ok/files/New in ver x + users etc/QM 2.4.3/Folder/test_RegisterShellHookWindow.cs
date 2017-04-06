\Dialog_Editor

 Very unreliable. Duplicate messages, incorrect messages, invalid handles, etc.
 This fails, probably must be dll: int hh=SetWindowsHookEx(WH_SHELL &sub.Hook_WH_GETMESSAGE _hinst 0)

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

out
if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	if(!RegisterShellHookWindow(hDlg)) out "failed"; ret
	int- t_msg=RegisterWindowMessage("SHELLHOOK")
	case WM_DESTROY
	DeregisterShellHookWindow(hDlg)
	case WM_COMMAND goto messages2
	case else
	if message=t_msg
		sel wParam
			case HSHELL_GETMINRECT SHELLHOOKINFO& x1=+lParam; outw2 x1.hwnd "GETMINRECT  "
			case HSHELL_WINDOWACTIVATED outw2 lParam "WINDOWACTIVATED  "
			case HSHELL_RUDEAPPACTIVATED outw2 lParam "RUDEAPPACTIVATED  "
			case HSHELL_WINDOWREPLACING outw2 lParam "WINDOWREPLACING  "
			case HSHELL_WINDOWREPLACED outw2 lParam "WINDOWREPLACED  "
			case HSHELL_WINDOWCREATED outw2 lParam "WINDOWCREATED  "
			case HSHELL_WINDOWDESTROYED outw2 lParam "WINDOWDESTROYED  "
			case HSHELL_ACTIVATESHELLWINDOW out "ACTIVATESHELLWINDOW  "
			case HSHELL_TASKMAN out "TASKMAN  "
			case HSHELL_REDRAW outw2 lParam "REDRAW  "
			case HSHELL_FLASH outw2 lParam "FLASH  "
			case HSHELL_ENDTASK outw2 lParam "ENDTASK  "
			case HSHELL_APPCOMMAND out "APPCOMMAND  "
			case 16 outw2 lParam "MONITORCHANGED  "
			case HSHELL_LANGUAGE out "LANGUAGE" ;;undocumented and does not work
			case HSHELL_SYSMENU out "SYSMENU"
			case HSHELL_ACCESSIBILITYSTATE out "ACCESSIBILITYSTATE"
			case else out "unknown  0x%X" wParam
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
