\Dialog_Editor

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
	int hb=GdipLoadPictureFile("q:\test\app_55.png")
	SendMessage id(3 hDlg) STM_SETIMAGE IMAGE_BITMAP hb
	__GdiHandle-- _hb=SendMessage(id(3 hDlg) STM_GETIMAGE IMAGE_BITMAP 0); if(_hb!=hb) DeleteObject(hb)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
