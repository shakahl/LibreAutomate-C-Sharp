 /dlg_DlgSplitter
function# hWnd message wParam lParam

int i j h
RECT rs
int* ip

sel message
	case WM_CREATE
	m_hWnd=hWnd
	m_hParent=GetParent(hWnd)
	GetWinXY(hWnd 0 0 i j); m_horz=i>j
	m_hCursor=LoadCursor(0 +iif(m_horz IDC_SIZENS IDC_SIZEWE))
	h=GetParent(m_hParent); m_disabled=h and wintest(h "" "QM_DE_class")
	
	case WM_SETCURSOR
	if(!m_disabled and wParam=hWnd)
		SetCursor m_hCursor
		ret 1
	
	case [WM_COMMAND,WM_NOTIFY]
	ret SendMessage(GetParent(hWnd) message wParam lParam)
	
	case WM_LBUTTONDOWN
	if(!m_disabled)
		j=GetWinStyle(m_hParent)&WS_CLIPCHILDREN; if(!j) SetWinStyle m_hParent WS_CLIPCHILDREN 1 ;;reduce flickering
		xm m_ptDrag hWnd; SwapPOINT(m_ptDrag)
		Drag(hWnd &SPL_DragProc &this)
		if(!j) SetWinStyle m_hParent WS_CLIPCHILDREN 2
		InvalidateRect m_hParent 0 1
	
	case WM_ERASEBKGND
	int hb=SendMessageW(GetParent(hWnd) WM_CTLCOLORSTATIC wParam hWnd)
	if hb
		RECT r; GetClientRect hWnd &r; FillRect wParam &r hb
		ret 1
	
	case WM_WINDOWPOSCHANGING
	if(!m_disabled) OnWindowposchanging(+lParam)
	
	case SPM_ENABLE
	m_disabled=wParam=0
	
	case SPM_ATTACH
	Attach(!wParam)
	
	case SPM_GETBOUNDS
	if(wParam) ip=+wParam; *ip=GetMinMax(0)
	if(lParam) ip=+lParam; *ip=GetMinMax(1)
	
	case SPM_GETPOS
	GetWinRect(m_hWnd rs m_hParent)
	i=GetMinMax(0)
	if(wParam) ip=+wParam; *ip=GetMinMax(1)-i-(rs.right-rs.left)
	ret rs.left-i
	
	case SPM_SETPOS
	GetWinRect(m_hWnd rs m_hParent)
	rs.left=wParam+GetMinMax(0)
	SwapRECT(rs)
	SetWindowPos m_hWnd 0 rs.left rs.top 0 0 SWP_NOSIZE|SWP_NOZORDER|SWP_NOACTIVATE

ret DefWindowProcW(hWnd message wParam lParam)
