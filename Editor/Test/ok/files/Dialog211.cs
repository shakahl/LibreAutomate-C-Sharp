
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040500 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	__ImageList- il.Create(0 24)
	__Hicon hi=GetFileIcon("Q:\Test\application_edit.ico")
	ImageList_ReplaceIcon(il -1 hi)
	
	case WM_PAINT
	PAINTSTRUCT ps
	int dc=BeginPaint(hDlg &ps)
	FillRect dc &ps.rcPaint GetSysColorBrush(COLOR_WINDOW)
	ImageList_Draw il 0 dc 4 4 0
	EndPaint hDlg &ps
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
