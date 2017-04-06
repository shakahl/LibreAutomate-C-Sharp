\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog52" &Dialog52)) ret

 BEGIN DIALOG
 1 "" 0x90C80A44 0x100 0 0 223 135 "Dialog ᵮ"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 6 8 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030000 "" "" ""

ret
 messages
sel message
	case WM_SETTEXT
	 lpstr ss=+lParam
	 out ss
	word* w=+lParam
	out _s.ansi(w)
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	str s="ᵮᵮ"
	s.setwintext(hDlg)
	case IDOK
	case IDCANCEL
ret 1

