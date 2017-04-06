 /
function# hDlg message wParam lParam

sel message
	case [WM_VSCROLL,WM_HSCROLL] OnScrollMsg(hDlg message wParam lParam)

int r=CallWindowProc(m_subc hDlg message wParam lParam)

sel message
	case [WM_CTLCOLORDLG,WM_CTLCOLORBTN,WM_CTLCOLORSTATIC]
	if(m_bcolor)
		SetBkColor wParam m_bcolor
		ret m_bcbrush
	case WM_NCDESTROY RemoveProp(hDlg "this")

ret r
