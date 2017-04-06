\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_menu_help" &dlg_menu_help)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 14 6 48 14 "menu"
 4 Static 0x54000000 0x0 2 94 218 12 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_MENUSELECT
	sel wParam&0xffff
		case 100 _s="help for one"
		case 101 _s="help for two"
	
	_s.setwintext(id(4 hDlg))
	
	int x y w h=win("" "#32768"); if(!h) ret
	GetWinXY h x y w
	OnScreenDisplay _s -1 x+w y 0 10 1 1|4|8|16 "menu_tooltip943" 0xe0ffff
ret
 messages2
sel wParam
	case 3
	ShowMenu("100 one[]101 two" hDlg)
	OsdHide "menu_tooltip943"
	
	case IDOK
	case IDCANCEL
ret 1
