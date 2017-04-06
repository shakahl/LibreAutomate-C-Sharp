 /
function WTI*ta n hwnd qmtc_flags

 Shows visual text capturing results window.
 It is a partially transparent window over hwnd.
 Draws rectangles for captured text items.
 Red - visible part (rv), yellow - whole text (rt).
 To close the window, click or call WT_ResultsVisualClose.


if(n<1) ret

type TCRESULTS WTI*ta n hwnd qmtc_flags RECT'r
TCRESULTS z.ta=ta; z.n=n; z.hwnd=hwnd; z.qmtc_flags=qmtc_flags

DpiGetWindowRect hwnd &z.r; InflateRect &z.r 2 2

str dd=
 BEGIN DIALOG
 0 "" 0x80000048 0x8 0 0 227 152 "WT_ResultsDlg"
 END DIALOG

if(!ShowDialog(dd &sub.DlgProc 0 0 64|128 0 0 &z)) ret


#sub DlgProc
function# hDlg message wParam lParam

TCRESULTS& z=+DT_GetParam(hDlg)
sel message
	case WM_INITDIALOG
	Transparent hDlg 160
	MoveWindow hDlg z.r.left z.r.top z.r.right-z.r.left z.r.bottom-z.r.top 0
	hid- hDlg
	
	case WM_PAINT goto g1
	case WM_LBUTTONUP clo hDlg
	case WM_COMMAND ret 1
ret

 g1
RECT rc; GetClientRect hDlg &rc
__MemBmp m.Create(rc.right rc.bottom)
FillRect(m.dc &rc GetStockObject(LTGRAY_BRUSH))

int isdc=SaveDC(m.dc)
SelectObject(m.dc GetStockObject(NULL_BRUSH))
SelectObject(m.dc GetStockObject(DC_PEN))
SetTextColor m.dc 0xff0000;; SetBkMode hdc TRANSPARENT
__Font f.Create("Arial" 7); SelectObject(m.dc f)

int i
for i 0 z.n
	WTI& t=z.ta[i]
	RECT rt(t.rt) rv(t.rv)
	if(t.hwnd!=z.hwnd and z.qmtc_flags&WT_SINGLE_COORD_SYSTEM=0) DpiMapWindowPoints t.hwnd z.hwnd +&rt 4
	DpiMapWindowPoints z.hwnd hDlg +&rt 4
	sub.DrawRect m.dc rt rv i t.flags

PAINTSTRUCT ps
BitBlt BeginPaint(hDlg &ps) 0 0 rc.right rc.bottom m.dc 0 0 SRCCOPY
EndPaint hDlg &ps

RestoreDC(m.dc isdc)


#sub DrawRect
function# hdc RECT&rt RECT&rv counter wtiFlags

 draw text rect
SetDCPenColor hdc 0xffff
Rectangle(hdc rt.left rt.top rt.right+((rt.right=rt.left)*2) rt.bottom+((rt.bottom=rt.top)*2))

if(wtiFlags&WTI_INVISIBLE) ret

 draw visible part rect
SetDCPenColor hdc 0xff
Rectangle(hdc rv.left rv.top rv.right rv.bottom)
 ret

 draw WTI index
BSTR b=counter
TextOutW hdc rv.right+2 rv.top b b.len
