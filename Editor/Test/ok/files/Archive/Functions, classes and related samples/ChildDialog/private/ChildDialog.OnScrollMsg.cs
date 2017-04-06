function hDlg message wParam lParam

int H(message=WM_HSCROLL) SB scpage scmax x y
if H
	SB=SB_HORZ
	scpage=m_scpageh
	scmax=m_scmaxh
else
	SB=SB_VERT
	scpage=m_scpagev
	scmax=m_scmaxv

int code=wParam&0xffff
int pos=wParam>>16
int ppos=GetScrollPos(hDlg SB)
 out "%i %i" code pos
sel code
	case SB_THUMBTRACK
	case SB_LINEDOWN pos=ppos+16
	case SB_LINEUP pos=ppos-16
	case SB_PAGEDOWN pos=ppos+scpage
	case SB_PAGEUP pos=ppos-scpage
	case else ret
if(pos<0) pos=0; else if(pos>scmax) pos=scmax
SetScrollPos hDlg SB pos 1
_i=ppos-pos; if(H) x=_i; else y=_i
ScrollWindowEx hDlg x y 0 0 0 0 SW_SCROLLCHILDREN|SW_ERASE|SW_INVALIDATE
