\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog44" &Dialog44)) ret

 BEGIN DIALOG
 1 "" 0x90C80A44 0x100 0 0 223 135 "ąč ﯔ"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 6 6 48 14 "ąč ﯔ"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_SETTEXT
	lpstr ss=+lParam
	out ss
	outb ss 3 1
	 ret 1
ret
 messages2
sel wParam
	case 3
	str s="ąč ﯔﮥ qww"
	s.setwintext(hDlg)
	 WINAPI2.DefWindowProcW hDlg WM_SETTEXT 0 s
	case IDOK
	case IDCANCEL
ret 1

