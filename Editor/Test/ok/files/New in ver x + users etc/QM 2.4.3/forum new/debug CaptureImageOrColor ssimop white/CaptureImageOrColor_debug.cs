 /
function# int&result flags [hwndmin] [$bmpfile] [RECT&rect] ;;flags: 0 image, 1 color, 2 minimize owner, 4 disable hwndmin, 8 don't restore if ok, 16 no tooltip, 32 no menu

 A debug version of CaptureImageOrColor.


type ___CIOC POINT'p0 RECT'r xs ys cxs cys xmagn !flags !retry !erase !inmenu __MemBmp'bms __MemBmp'bmc
___CIOC d.flags=flags
int i ho R

 opt err 1
opt waitmsg 1 ;;caller may be dialog etc
spe 50
if(hwndmin)
	if(flags&2) ho=GetWindow(hwndmin GW_OWNER)
	if(!ho) ho=hwndmin
	min ho
	if(flags&4) EnableWindow hwndmin 0
	0.5

 g1
GetVirtualScreen d.xs d.ys d.cxs d.cys
d.bms.Create_debug(d.cxs d.cys 2) ;;take snapshot

str debugFile="$temp$\CaptureImageOrColor_debug.bmp"
_i=SaveBitmap(d.bms.bm debugFile)
if(_i) out F"<>Whole screen bitmap saved here: <link>{debugFile}</link>. Is it white?"
else out "Failed to save whole screen bitmap"

sel(ShowDialog("" &sub.Dlg 0 0 0 0 0 &d))
	case 1 ;;OK
	if(flags&1)
		R=1
		if(&result) result=GetPixel(d.bms.dc d.p0.x d.p0.y)
		if(&rect) rect.left=d.p0.x; rect.top=d.p0.y; rect.right=rect.left; rect.bottom=rect.top; OffsetRect &rect d.xs d.ys
	else if(d.bmc.bm)
		R=1
		if(!empty(bmpfile)) R=SaveBitmap(d.bmc.bm bmpfile); if(!R) &result=0
		if(&result) result=d.bmc.Detach
		if(&rect) rect=d.r; OffsetRect &rect d.xs d.ys
	case 3 ;;Take another snapshot
	d.bmc.Delete; d.erase=0; d.retry=1
	TO_TooltipOsd "Press Shift when ready to take another snapshot.[][]Note: This is NOT a screen snapshot. You can rearrange windows, etc." 8 "" -1
	 wait for Shift
	for(i 0 1000000)
		0.003; ifk(S) break
		if(i=20000) i=0; TO_TooltipOsd "Still waiting for Shift..." 0 "" 0 0 0 0 0xff
	OsdHide; 0.1
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
POINT p; RECT r; int dc z
sel message
	case WM_INITDIALOG
	MoveWindow hDlg d.xs d.ys d.cxs d.cys 0 ;;cover whole screen
	
	case WM_SHOWWINDOW ;;if on WM_INITDIALOG, hDlg may cover the tooltip
	if(wParam and d.flags&16=0) TO_TooltipOsd _s.from(iif(d.flags&1 "Click anywhere to pick the color." "Draw a rectangle with the mouse.") "[][]Note: This is a screen snapshot, not real windows.[]You can click anywhere to take another snapshot.") 0 "" 10
	
	case WM_LBUTTONDOWN
	OsdHide
	xm d.p0 hDlg 1 ;;save mouse down coordinates
	SetCapture hDlg
	r.right=66; r.bottom=86; OffsetRect &r -d.xs+d.xmagn -d.ys; InvalidateRect hDlg &r 0 ;;erase magnifier
	
	case WM_MOUSEMOVE
	xm p hDlg 1
	dc=GetDC(hDlg)
	if(hDlg!=GetCapture) ;;draw magnifier/color
		r.right=66; r.bottom=86; OffsetRect &r -d.xs+d.xmagn -d.ys ;;calc frame
		if(PtInRect(&r p.x p.y)) d.xmagn=iif(d.xmagn 0 300); InvalidateRect hDlg &r 0; goto gr ;;move away
		z=GetStockObject(BLACK_BRUSH); FrameRect dc &r z; r.top+65; FrameRect dc &r z ;;draw frames
		StretchBlt(dc r.left+1 r.top-64 64 64 d.bms.dc p.x p.y 16 16 SRCCOPY) ;;draw magnifier
		 draw color
		_s.format("0x%06X" GetPixel(d.bms.dc p.x p.y))
		InflateRect &r -1 -1; FillRect dc &r 1+COLOR_WINDOW; r.left+2; r.top+2
		z=SelectObject(dc _hfont); DrawTextW dc @_s -1 &r 0; SelectObject(dc z)
	else if(d.flags&1=0) ;;draw focus rect
		if(d.erase) DrawFocusRect dc &d.r; d.erase=0 ;;erase prev
		SetRect &d.r iif(p.x>=d.p0.x d.p0.x p.x) iif(p.y>=d.p0.y d.p0.y p.y) iif(p.x<d.p0.x d.p0.x p.x) iif(p.y<d.p0.y d.p0.y p.y) ;;calc new
		d.erase=DrawFocusRect(dc &d.r) ;;draw new
	 gr
	ReleaseDC hDlg dc
	
	case WM_LBUTTONUP
	if(hDlg!=GetCapture) ret
	ReleaseCapture
	_s="OK[]Cancel[]Take another snapshot[]Retry in current snapshot"
	if(d.flags&1=0)
		if(d.erase and !IsRectEmpty(&d.r)) d.bmc.Create(d.r.right-d.r.left d.r.bottom-d.r.top d.bms.dc d.r.left d.r.top) ;;capture and save the rectangle
		else _s.get(_s 4); _i=1 ;;remove OK from the menu
	if(d.flags&32) DT_Ok hDlg; ret
	d.inmenu=1
	z=ShowMenu(_s hDlg 0 2)
	d.inmenu=0
	if(!z) ret
	sel z+_i
		case 1 DT_Ok hDlg
		case 2 DT_Cancel hDlg
		case 3 DT_Ok hDlg 3
		
	case WM_PAINT
	PAINTSTRUCT ps
	dc=BeginPaint(hDlg &ps)
	if(!dc) end "BeginPaint" 17
	_i=BitBlt(dc 0 0 d.cxs d.cys d.bms.dc 0 0 SRCCOPY) ;;draw whole screen in hDlg
	if(!_i) end "BitBlt" 17
	EndPaint hDlg &ps
	ret 1
	
	case WM_COMMAND ret 1
	
	case WM_SETCURSOR
	if(d.inmenu) ret
	SetCursor sub_to.LoadCursor(4)
	ret 1

 BEGIN DIALOG
 0 "" 0x90000248 0x88 0 0 227 151 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""
