 /
function# hWnd message wParam lParam

def TB_HITTEST (WM_USER + 69)

int+ g_tb_move_htb g_tb_move_hfore
POINT+ g_tb_move_pos
POINT p

sel message
	case WM_SETCURSOR
	if(lParam=WM_LBUTTONDOWN<<16|HTCLIENT)
		if(wParam=hWnd)
		else if(GetDlgCtrlID(wParam)=9999)
			xm p wParam 1
			int i=SendMessage(wParam TB_HITTEST 0 &p)
			 if(i>=0) ret ;;button
			if(i>=0 and i!=2) ret ;;button, except button 2
		else ret
		PostMessage hWnd WM_APP 0 0
		
	case WM_APP
	GetCursorPos &g_tb_move_pos
	g_tb_move_htb=hWnd
	g_tb_move_hfore=win
	act hWnd ;;for SetCapture
	SetCapture hWnd
	
	case [WM_LBUTTONUP,WM_CANCELMODE]
	g_tb_move_htb=0
	SetCapture 0
	act g_tb_move_hfore; err
	
	case WM_MOUSEMOVE
	if(g_tb_move_htb=hWnd)
		RECT r; GetWindowRect hWnd &r
		GetCursorPos &p
		mov r.left+p.x-g_tb_move_pos.x r.top+p.y-g_tb_move_pos.y hWnd
		g_tb_move_pos=p
		
	