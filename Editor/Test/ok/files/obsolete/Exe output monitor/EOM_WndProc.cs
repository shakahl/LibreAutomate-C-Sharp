 /exe
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	RegWinPos hWnd "wndpos" "\QM EOM" 0
	LoadLibrary("Riched20")
	int-- t_hEd=CreateControl(0 "RichEdit20W" 0 WS_VSCROLL|ES_AUTOVSCROLL|ES_DISABLENOSCROLL|ES_MULTILINE|ES_WANTRETURN|ES_READONLY 0 0 100 100 hWnd 3)
	
	case WM_DESTROY
	RegWinPos hWnd "wndpos" "\QM EOM" 1
	PostQuitMessage 0
	
	case WM_SIZE
	RECT r; GetClientRect hWnd &r; MoveWindow t_hEd 0 0 r.right r.bottom 1
	
	case WM_SETTEXT
	if(wParam) goto g1

ret DefWindowProcW(hWnd message wParam lParam)

 g1
if(wParam!-1) ret

if !lParam ;;clear
	_s.setwintext(t_hEd)
	ret

 remove tags
word* w=+lParam
if w[0]='<' and w[1]='>'
	_s.ansi(w+4)
	_s.replacerx("<.+?>")
	lParam=_s.unicode

 add to the rich edit control
SendMessageW t_hEd EM_SETSEL -1 -1
SendMessageW t_hEd EM_REPLACESEL 0 lParam
SendMessageW t_hEd EM_REPLACESEL 0 L"[]"
SendMessageW t_hEd WM_VSCROLL SB_BOTTOM 0
