#sub BitmapToResource
function# hbm str&resName

str b
__Stream is.CreateOnHglobal
if(!SaveBitmap(hbm +is 1)) ret
is.ToStr(b 0 2)
is=0
b.encrypt(32 b)

if(!resName.len) resName.format("h%08X" Crc32(b b.len))
if(!resName.beg("image:")) resName-"image:"

_qmfile.ResourceAdd(+-3 F"{resName}" b b.len)

ret 1
err+


#sub CanGetPixels
function w hDlg idCheck

int h(id(idCheck hDlg)) checked=but(h)
if(!checked) OsdHide "TO_Scan"; ret
str s
if !IsWindow(w)
else if(DpiIsWindowScaled(w)) s="Cannot get pixels from this window, it is DPI-scaled.[]You can disable scaling:[]    for that program, in its Properties dialog -> Compatibility;[]    for all programs, in Control Panel -> Display."
else if(GetWinStyle(w 1)&WS_EX_NOREDIRECTIONBITMAP) s="Cannot get pixels from this window."
else
	__MemBmp m.Create(4 4)
	if(!PrintWindow(w m.dc 1)) s="Cannot get pixels from this window when Aero is not enabled."
	else sel(_s.getwinexe(w) 1) case "IEXPLORE" s="Often cannot get pixels from Internet Explorer."
if(s.len) sub_sys.TooltipOsd s 9 "TO_Scan" 0 0 0 h
err+


#sub ViewPixels
function hwnd scanFlags ;;scanFlags: 16 client, 0x100 PrintWindow, 0x1000 GetDC

type ___SCANVIEWPIXELS cx cy __MemBmp'b
___SCANVIEWPIXELS d

OsdHide "TO_Scan"
if(scanFlags&0x1100=0 or !IsWindow(hwnd)) ret
str k
if(scanFlags&0x1100) if(DpiIsWindowScaled(hwnd)) scanFlags~0x1100; k+" It is DPI-scaled."
if(scanFlags&0x1000) if(_winver<0x600 or DwmIsCompositionEnabled(&_i) or !_i) scanFlags~0x1000; k=" Aero is unavailable or disabled."; else scanFlags~0x100
if(scanFlags&0x1100=0) mes F"This 'background' option will not be applied for this window on this computer.{k}" "" "i"; ret

int client(scanFlags&16 and !(_winver<0x600 and scanFlags&0x1000=0))
RECT r; DpiGetWindowRect hwnd &r client*4; OffsetRect &r -r.left -r.top; r.right+2; r.bottom+2; d.cx=r.right; d.cy=r.bottom
if(!d.b.Create(d.cx d.cy)) mes "Failed"; ret
if scanFlags&0x100
	 out "PrintWindow"
	if(!PrintWindow(hwnd d.b.dc client)) mes "Failed"; ret
else
	 out "GetDC"
	__Hdc dc=iif(client GetDC(hwnd) GetWindowDC(hwnd))
	BitBlt d.b.dc 0 0 d.cx-2 d.cy-2 dc 0 0 SRCCOPY
	dc.Release

 draw red rect
ScrollDC d.b.dc 1 1 0 0 0 0
__GdiHandle redBrush=CreateSolidBrush(0xff)
FrameRect d.b.dc &r redBrush

str dd=
 BEGIN DIALOG
 0 "" 0x90FF02C8 0x8 0 0 347 233 "Captured window pixels"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

ShowDialog(dd &sub.__DlgProcVP 0 0 0x100 0 0 &d)


#sub __DlgProcVP
function# hDlg message wParam lParam

___SCANVIEWPIXELS& d=+DT_GetParam(hDlg)
sel message
	case WM_INITDIALOG sub.__SetScoll hDlg d
	case WM_SIZE sub.__SetScoll hDlg d
	
	case WM_PAINT
	PAINTSTRUCT ps; BeginPaint(hDlg &ps)
	BitBlt ps.hDC 0 0 d.cx d.cy d.b.dc 0 0 SRCCOPY
	EndPaint hDlg &ps
	
	case [WM_VSCROLL,WM_HSCROLL] sub.__Scroll(hDlg message wParam d)
	case WM_COMMAND ret 1


#sub __SetScoll
function hDlg ___SCANVIEWPIXELS&d
if(IsIconic(hDlg)) ret

RECT r; GetClientRect(hDlg &r)

SCROLLINFO si.cbSize=sizeof(si)
si.fMask=SIF_RANGE|SIF_PAGE|SIF_POS|SIF_DISABLENOSCROLL ;;SIF_DISABLENOSCROLL is a workaround for: messed scrollbars after maximize/restore.
si.nMax=d.cy; si.nPage=r.bottom; SetScrollInfo hDlg SB_VERT &si 0
si.nMax=d.cx; si.nPage=r.right; SetScrollInfo hDlg SB_HORZ &si 0

SetViewportOrgEx d.b.dc 0 0 0
InvalidateRect hDlg 0 0


#sub __Scroll
function hDlg message wParam ___SCANVIEWPIXELS&d

int H(message=WM_HSCROLL) SB scpage scmax x y code(wParam&0xffff) pos(wParam>>16) ppos

RECT r; GetClientRect(hDlg &r)
if(H) SB=SB_HORZ; scpage=r.right; scmax=d.cx-scpage
else SB=SB_VERT; scpage=r.bottom; scmax=d.cy-scpage

ppos=GetScrollPos(hDlg SB)
sel code
	case SB_THUMBTRACK
	case SB_LINEDOWN pos=ppos+16
	case SB_LINEUP pos=ppos-16
	case SB_PAGEDOWN pos=ppos+scpage
	case SB_PAGEUP pos=ppos-scpage
	case else ret
if(pos<0) pos=0; else if(pos>scmax) pos=scmax

SetScrollPos(hDlg SB pos 1); pos=GetScrollPos(hDlg SB) ;;setscrollpos may not set correctly
_i=pos-ppos; if(H) x=_i; else y=_i
if x|y
	OffsetViewportOrgEx d.b.dc x y 0
	InvalidateRect hDlg 0 0
