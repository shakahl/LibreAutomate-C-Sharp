 /
function# int&result flags [hwndmin] [$bmpfile] [RECT&rect] ;;flags: 0 image, 1 color, 2 minimize owner, 4 disable hwndmin, 8 don't restore if ok, 16 no tooltip, 32 no menu

 Allows the user to capture an image or a color on the screen.
 Returns 1 if the user clicks OK and everything goes without errors. Returns 0 otherwise.

 result - variable that will receive color value or bitmap handle. Later the bitmap must be deleted using DeleteObject. Can be 0.
 hwndmin - handle of a window to minimize/restore. Can be 0.
 bmpfile - if used, saves the image to the file. Returns 0 if fails to save. Can be "".
 rect - receives rectangle/point coordinates. Can be 0.

 REMARKS
 Unlike <help>CaptureImageOnScreen</help>, this function interacts with the user, and returns only when the user clicks OK or Cancel or presses Esc.
 If flags contains 1, captures color, else captures image.
 Flags 16 and 32 added in QM 2.3.3.


type ___CIOC POINT'p RECT'r xs ys cxs cys xmagn flags __MemBmp'bms __MemBmp'bmc
___CIOC d.flags=flags
int ho R

opt err 1
opt waitmsg 1 ;;caller may be dialog etc
spe 50
if(hwndmin)
	if(flags&2) ho=GetWindow(hwndmin GW_OWNER)
	if(!ho) ho=hwndmin
	min ho
	if(flags&4) EnableWindow hwndmin 0
	0.5

str dd=
 BEGIN DIALOG
 0 "" 0x90000248 0x88 0 0 227 151 "Dialog"
 END DIALOG

 g1
GetVirtualScreen d.xs d.ys d.cxs d.cys
if !d.bms.Create(d.cxs d.cys 2)
	 if fails to create bitmap for whole virtual screen, try only primary monitor
	GetWorkArea d.xs d.ys d.cxs d.cys 1
	if(!d.bms.Create(d.cxs d.cys 1)) ret

sel(ShowDialog(dd &sub.Dlg 0 0 0 0 0 &d))
	case 1 ;;OK
	if(flags&1) ;;color
		R=1
		if(&result) result=GetPixel(d.bms.dc d.p.x d.p.y)
		if(&rect) rect.left=d.p.x; rect.top=d.p.y; rect.right=rect.left; rect.bottom=rect.top; OffsetRect &rect d.xs d.ys
	else if(d.bmc.bm)
		R=1
		if(!empty(bmpfile)) R=SaveBitmap(d.bmc.bm bmpfile); if(!R) &result=0
		if(&result) result=d.bmc.Detach
		if(&rect) rect=d.r; OffsetRect &rect d.xs d.ys
	case 3 ;;Take another snapshot
	d.bmc.Delete
	sub_to.WaitForShift "Press Shift when ready to take another snapshot.[][]Note: This is NOT a screen snapshot. You can rearrange windows, etc."
	goto g1

OsdHide
if(hwndmin)
	if(flags&4) EnableWindow hwndmin 1
	if(flags&8=0 or !R)
		if(min(ho)) res ho; act ho
		act hwndmin

0.01 ;;windows may be incorrectly ordered, eg like the max window is over taskbar
ret R


#sub Dlg
function# hDlg message wParam lParam

___CIOC* d=+DT_GetParam(hDlg)
sel message
	case WM_INITDIALOG
	MoveWindow hDlg d.xs d.ys d.cxs d.cys 0 ;;cover whole screen
	
	case WM_SHOWWINDOW ;;if on WM_INITDIALOG, hDlg may cover the tooltip
	str tt=
	;
	;
	 Note: This is a screen snapshot, not real windows.
	 You can click anywhere to take another snapshot.
	if(wParam and d.flags&16=0) sub_sys.TooltipOsd _s.from(iif(d.flags&1 "Click anywhere to pick the color." "Draw a rectangle with the mouse.") tt) 4|8 "" 10
	
	case [WM_LBUTTONDOWN,WM_MOUSEMOVE]
	sub.Draw hDlg message d
	
	case WM_PAINT
	PAINTSTRUCT ps
	BitBlt BeginPaint(hDlg &ps) 0 0 d.cxs d.cys d.bms.dc 0 0 SRCCOPY ;;draw whole screen in hDlg
	EndPaint hDlg &ps
	ret 1
	
	case WM_COMMAND ret 1
	
	case WM_SETCURSOR
	if(GetCapture) ret
	SetCursor sub_to.LoadCursor(4)
	ret 1


#sub Draw
function hDlg message ___CIOC&d

RECT r rr; POINT p; int dc z
r.right=66; r.bottom=86; OffsetRect &r -d.xs+d.xmagn -d.ys ;;magnifier rect
xm p hDlg 1

if message=WM_MOUSEMOVE
	 move magnifier away from cursor
	rr=r; InflateRect &rr 8 8
	if PtInRect(&rr p.x p.y)
		d.xmagn=iif(d.xmagn 0 300)
		InvalidateRect hDlg &r 0
		ret
	 draw magnifier/color
	dc=GetDC(hDlg)
	z=GetStockObject(BLACK_BRUSH); FrameRect dc &r z; r.top+65; FrameRect dc &r z ;;draw frames
	StretchBlt(dc r.left+1 r.top-64 64 64 d.bms.dc p.x p.y 16 16 SRCCOPY) ;;draw magnifier
	 draw color
	_s.format("0x%06X" GetPixel(d.bms.dc p.x p.y))
	InflateRect &r -1 -1; FillRect dc &r 1+COLOR_WINDOW; r.left+2; r.top+2
	z=SelectObject(dc _hfont); DrawTextW dc @_s -1 &r 0; SelectObject(dc z)
	ReleaseDC hDlg dc
	ret

OsdHide
d.p=p ;;save mouse down coordinates
InvalidateRect hDlg &r 0 ;;erase magnifier
 drag
__Drag g.Init(hDlg 1)
if d.flags&1 ;;capturing color, just wait for lbuttonup
	rep() if(!g.Next) break
else ;;draw focus rect
	int next erase
	dc=GetDC(hDlg)
	rep
		next=g.Next
		if(erase) DrawFocusRect dc &d.r ;;erase prev
		if(!next) break
		p=g.p
		SetRect &d.r iif(p.x>=d.p.x d.p.x p.x) iif(p.y>=d.p.y d.p.y p.y) iif(p.x<d.p.x d.p.x p.x) iif(p.y<d.p.y d.p.y p.y) ;;calc new
		erase=DrawFocusRect(dc &d.r) ;;draw new
	ReleaseDC hDlg dc
if(!g.dropped) ret

_s="1OK[]2Cancel[]3Take another snapshot[]4Retry in current snapshot"
if d.flags&1=0
	if(erase and !IsRectEmpty(&d.r)) d.bmc.Create(d.r.right-d.r.left d.r.bottom-d.r.top d.bms.dc d.r.left d.r.top) ;;capture and save the rectangle
	else _s.remove(0 5); int noOK=1 ;;remove OK
if(d.flags&32) z=1+noOK; else z=ShowMenu(_s hDlg)
sel z
	case 1 DT_Ok hDlg
	case 2 DT_Cancel hDlg
	case 3 DT_Ok hDlg 3
