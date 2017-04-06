 /Toolbar8
function# hWnd message wParam lParam

def TB_GETIMAGELIST (WM_USER + 49)
def TB_CHANGEBITMAP (WM_USER + 43)
dll user32 #UpdateWindow hWnd
type TBBLINK htb iic is

int i hil hic button icon
TBBLINK* p

button=2 ;;toolbar button index. This is line index, including comments.
icon=0 ;;real button index (not including comments)

sel message
	case WM_CREATE
	 PostMessage hWnd WM_APP 1 0
	SetProp hWnd "p" p._new
	case WM_DESTROY
	p=+GetProp(hWnd "p"); p._delete
	
	case WM_APP
	p=+GetProp(hWnd "p")
	if(!p.htb)
		p.htb=id(9999 hWnd)
		hil=SendMessage(p.htb TB_GETIMAGELIST 0 0)
		hic=GetIcon("$qm$\mouse.ico")
		p.iic=ImageList_ReplaceIcon(hil -1 hic)
		DestroyIcon hic
	if(wParam) SetTimer hWnd 0 300 0; p.is=0; goto g1
	else KillTimer hWnd 0; p.is=1; goto g1
	
	case WM_TIMER
	 g1
	p=+GetProp(hWnd "p")
	p.is^1
	SendMessage p.htb TB_CHANGEBITMAP button iif(p.is p.iic icon)
	UpdateWindow p.htb
	
	