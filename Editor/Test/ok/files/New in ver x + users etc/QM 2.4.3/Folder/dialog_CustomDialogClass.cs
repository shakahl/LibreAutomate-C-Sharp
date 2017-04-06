\Dialog_Editor

__RegisterWindowClass+ __MyDialogClass; if(!__MyDialogClass.atom) __MyDialogClass.Superclass("#32770" "MyDialogClass" &DefDlgProcW)
 ret

str dd=
 BEGIN DIALOG
 1 "MyDialogClass" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54012003 0x0 12 20 48 10 "Check"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "3"
str c3Che
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	_s="Test"; _s.setwintext(hDlg)
	
	case WM_SETTEXT ;;note: the dialog is always Unicode, ie the dialog procedure receives UTF-16 text in string paramaters
	 lpstr s=+lParam; out s
	 word* w=+lParam; out _s.ansi(w)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
