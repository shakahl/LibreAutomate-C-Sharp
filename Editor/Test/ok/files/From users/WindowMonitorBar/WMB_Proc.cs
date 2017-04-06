
function# hWnd message wParam lParam

def TB_GETIMAGELIST (WM_USER + 49)
def TB_CHANGEBITMAP (WM_USER + 43)
dll user32 #UpdateWindow hWnd
type TBBLINK htb iic is

int i hil hic
TBBLINK* p
str+ icon

sel message
	
	case WM_CREATE
		SetProp hWnd "p" p._new
	
	case WM_DESTROY
		p=+GetProp(hWnd "p"); p._delete

	case WM_APP
		p=+GetProp(hWnd "p")
		p.htb=id(9999 hWnd)
		hil=SendMessage(p.htb TB_GETIMAGELIST 0 0)
		hic=GetIcon(icon)
		p.iic=ImageList_ReplaceIcon(hil -1 hic)
		SendMessage p.htb TB_CHANGEBITMAP wParam p.iic
		UpdateWindow p.htb		
		DestroyIcon hic
