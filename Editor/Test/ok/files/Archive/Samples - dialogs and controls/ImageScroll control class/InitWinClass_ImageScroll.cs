 Registers and implements window class "QM_ImageScroll".
 A control of "QM_ImageScroll" class displays an image and can have scrollbars. Supports .bmp, .png, .jpg, .gif files, and bitmap handle.
 Call this function when QM starts, eg in function "init2". In exe - in main function.
 Then controls of the class can be used in dialogs and other windows.

 To add a control to a dialog, in Dialog Editor click "Other controls", type "QM_ImageScroll", OK.
 To set image file path, use a dialog variable or str.setwintext.
 You can use a bitmap handle as control text. The control will delete the bitmap when destroyed. The bitmap must be not selected in a DC.


__RegisterWindowClass+ ___ImageScroll_class.Register("QM_ImageScroll" &sub.WndProc 4 0 CS_GLOBALCLASS)


#sub WndProc
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam

type __IMAGESCROLL __MemBmp'b cx cy
__IMAGESCROLL* d
sel message
	case WM_NCCREATE SetWindowLong hWnd 0 d._new
	case WM_NCDESTROY d._delete
	case else d=+GetWindowLong(hWnd 0)

sel message
	case WM_SETTEXT sub.SetText hWnd d +lParam
	case WM_SIZE sub.SetScoll hWnd d 0
	case [WM_VSCROLL,WM_HSCROLL] sub.Scroll(hWnd message wParam d)
	
	case WM_PAINT
	if d.b.bm
		PAINTSTRUCT ps; BeginPaint(hWnd &ps)
		BitBlt ps.hDC 0 0 d.cx d.cy d.b.dc 0 0 SRCCOPY
		EndPaint hWnd &ps

int R=DefWindowProcW(hWnd message wParam lParam)
ret R


#sub SetText
function hWnd __IMAGESCROLL&d word*wName
_s.ansi(wName)
 out _s

int hb=val(_s 0 _i) ;;handle
if(!(hb and _i=_s.len)) hb=LoadPictureFile(_s); if(!hb) ret

BITMAP _b; if(!GetObjectW(hb sizeof(BITMAP) &_b)) ret
d.b.Attach(hb)
d.cx=_b.bmWidth; d.cy=_b.bmHeight

sub.SetScoll hWnd d 1


#sub SetScoll
function hWnd __IMAGESCROLL&d !erase

RECT r; GetClientRect(hWnd &r)

SCROLLINFO si.cbSize=sizeof(si)
si.fMask=SIF_RANGE|SIF_PAGE|SIF_POS
si.nMax=d.cy; si.nPage=r.bottom; SetScrollInfo hWnd SB_VERT &si 0
si.nMax=d.cx; si.nPage=r.right; SetScrollInfo hWnd SB_HORZ &si 0

SetViewportOrgEx d.b.dc 0 0 0
InvalidateRect hWnd 0 erase


#sub Scroll
function hWnd message wParam __IMAGESCROLL&d

int H(message=WM_HSCROLL) SB scpage scmax x y code(wParam&0xffff) pos(wParam>>16) ppos

RECT r; GetClientRect(hWnd &r)
if(H) SB=SB_HORZ; scpage=r.right; scmax=d.cx-scpage
else SB=SB_VERT; scpage=r.bottom; scmax=d.cy-scpage

ppos=GetScrollPos(hWnd SB)
sel code
	case SB_THUMBTRACK
	case SB_LINEDOWN pos=ppos+16
	case SB_LINEUP pos=ppos-16
	case SB_PAGEDOWN pos=ppos+scpage
	case SB_PAGEUP pos=ppos-scpage
	case else ret
if(pos<0) pos=0; else if(pos>scmax) pos=scmax

SetScrollPos(hWnd SB pos 1); pos=GetScrollPos(hWnd SB) ;;setscrollpos may not set correctly
_i=pos-ppos; if(H) x=_i; else y=_i
if x|y
	OffsetViewportOrgEx d.b.dc x y 0
	InvalidateRect hWnd 0 0
