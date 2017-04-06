\Dialog_Editor
#compile "__Gdip"

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 328 168 "Dialog"
 3 Static 0x5400000E 0x0 242 102 66 58 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030201 "*" "" ""

str controls = "3"
str sb3
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	GdipImage-- t_im
	
	case WM_PAINT
	if(!t_im and !t_im.FromFile("q:\test\app_55.png")) ret
	
	PAINTSTRUCT ps
	BeginPaint hDlg &ps
	t_im.Draw(ps.hDC 150 100)
	EndPaint hDlg &ps
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
