 /
function# hWnd message wParam lParam

def TB_HITTEST (WM_USER + 69)
int hx hy hcx hcy
int bx by bcx bcy
int+ g_tb_move_htb g_tb_move_hfore
POINT+ g_tb_move_pos
POINT p

sel message
	case WM_INITDIALOG
		 GetWinXY win("THUMB_MIN" "QM_toolbar") hx hy hcx hcy
		 GetWinXY id(9999 win("THUMB_MIN" "QM_toolbar")) bx by bcx bcy
	 MoveWindow id(9999 win("THUMB_MIN" "QM_toolbar")) -4 -4 bcx bcy 1 
	
	case WM_SETCURSOR
	if(lParam=WM_LBUTTONDOWN<<16|HTCLIENT)
		if(wParam=hWnd)
		else if(GetDlgCtrlID(wParam)=9999)
			GetCursorPos &p; ScreenToClient wParam &p
			int i=SendMessage(wParam TB_HITTEST 0 &p)
			if(i!=1) ret
		else ret
		 PostMessage hWnd WM_APP 0 0
		GetCursorPos &g_tb_move_pos
		SetTimer hWnd 547 300 0
		
	 case WM_APP
	 GetCursorPos &g_tb_move_pos
	 g_tb_move_htb=hWnd
	 g_tb_move_hfore=win
	 act hWnd ;;for SetCapture
	 SetCapture hWnd
	case WM_TIMER
	if(wParam=547)
		KillTimer hWnd wParam
		ifk (1) ;;still pressed, so lets drag
			g_tb_move_htb=hWnd
			g_tb_move_hfore=win
			 act hWnd ;;for SetCapture. Try to remove this. On Vista works well anyway.
			SetCapture hWnd
	
	case [WM_LBUTTONUP,WM_CANCELMODE]
	g_tb_move_htb=0
	SetCapture 0
	act g_tb_move_hfore; err
	
	case WM_MOUSEMOVE
	if(g_tb_move_htb=hWnd and GetCapture=hWnd)
		RECT r; GetWindowRect hWnd &r
		GetCursorPos &p
		mov r.left+p.x-g_tb_move_pos.x r.top+p.y-g_tb_move_pos.y hWnd
		g_tb_move_pos=p