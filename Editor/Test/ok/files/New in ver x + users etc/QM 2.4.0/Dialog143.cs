\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str rea3
rea3=
 sel message
	 case WM_INITDIALOG
	 case WM_DESTROY
	 case WM_COMMAND goto messages2

if(!ShowDialog("Dialog143" &Dialog143 &controls)) ret

 BEGIN DIALOG
 1 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 0 0 224 52 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040001 "" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 500 0
	case WM_TIMER
	sel wParam
		case 1
		int cid=id(3 hDlg)
		POINT p; xm(p cid 1) ;;get mouse position into p.x and p.y
		int cursorPos = SendMessage(cid EM_CHARFROMPOS 0 &p)
		out cursorPos
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
