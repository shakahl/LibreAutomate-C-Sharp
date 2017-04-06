 /dlg_apihook
function# hDlg message wParam lParam
if(hDlg) goto messages

ret
 messages
BSTR s
sel message
	case WM_INITDIALOG
	case WM_ERASEBKGND
	s="on WM_ERASEBKGND"; out s; ExtTextOutW wParam 0 80 0 0 s s.len 0
	ret DT_Ret(hDlg 1)
	
	case WM_PAINT
	out "p"
	 ret
	ValidateRect hDlg 0
	__Hdc dc.FromWindowDC(hDlg)
	s="on WM_PAINT no BeginPaint"; out s; ExtTextOutW dc.dc 0 140 0 0 s s.len 0
	ret DT_Ret(hDlg 0)
	
	PAINTSTRUCT ps
	int hdc=BeginPaint(hDlg &ps)
	s="on WM_PAINT"; out s; ExtTextOutW hdc 0 110 0 0 s s.len 0
	EndPaint hDlg &ps
	
	case WM_COMMAND goto messages2
ret
 messages2
ret 1
