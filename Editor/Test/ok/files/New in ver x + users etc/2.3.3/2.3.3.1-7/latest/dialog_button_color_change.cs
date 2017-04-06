\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 4 Button 0x54032000 0x0 4 4 58 14 "Random color"
 3 Button 0x5403200B 0x0 72 4 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2040201 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

__GdiHandle- t_brush
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_CTLCOLORBTN
	sel GetDlgCtrlID(lParam)
		case 3
		SetBkMode wParam 1 ;;draw text transparently
		SetTextColor wParam 0xff
		ret t_brush
ret
 messages2
sel wParam
	case 4 ;;Random color
	t_brush.Delete
	t_brush=CreateSolidBrush(RandomInt(0x404040 0xFFFFFF)) ;;create brush of random color
	InvalidateRect id(3 hDlg) 0 1 ;;redraw; then it sends WM_CTLCOLORSTATIC
	
	case IDOK
	case IDCANCEL
ret 1
