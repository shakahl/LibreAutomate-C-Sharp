\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str si3
si3="&$system$\notepad.exe,0"
if(!ShowDialog("dlg_static_text_transparent" &dlg_static_text_transparent &controls)) ret

 note: static control: remove WS_CLIPSIBLINGS style, and add WS_EX_TRANSPARENT.

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 4 Static 0x50000000 0x20 4 8 48 12 "Text"
 3 Static 0x54000003 0x0 10 4 16 16 ""
 5 Button 0x54032000 0x0 2 38 56 14 "Change text"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_CTLCOLORSTATIC
	sel GetWinId(lParam)
		case 4
		SetBkMode wParam TRANSPARENT
		ret GetStockObject(NULL_BRUSH) ;;transparent brush
ret
 messages2
sel wParam
	case IDOK
	case 5 ;;Change text
	int h=id(4 hDlg)
	RECT r; GetClientRect h &r; MapWindowPoints h hDlg +&r 2; InvalidateRect hDlg &r 1 ;;erase background, because, when using transparent brush, setwintext does not erase
	_s="new text"
	_s.setwintext(h)
ret 1
