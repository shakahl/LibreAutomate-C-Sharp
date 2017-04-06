
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	__Font-- t_f.Create("Segoe UI" 9 0 270)
	
	case WM_PAINT
	PAINTSTRUCT ps
	int dc=BeginPaint(hDlg &ps)
	
	BSTR s="Output"
	int of=SelectObject(dc t_f)
	RECT r; SetRect &r 100 100 200 200
	SetBkMode dc 1
	ExtTextOutW(dc 120 0 ETO_CLIPPED 0 s s.len 0)
	SelectObject(dc of)
	
	EndPaint(hDlg &ps)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
