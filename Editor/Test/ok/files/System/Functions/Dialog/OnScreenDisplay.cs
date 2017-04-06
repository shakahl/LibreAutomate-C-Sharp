 \
function $text [^timeS] [x] [y] [$fname] [fsize] [fcolor] [flags] [$osdid] [bcolor] [wrapwidth] [$pictfile] ;;flags: 1 nontransparent, 2 synchronous, 4 click to hide, 8 hide when macro ends, 16 use raw x y, 32 place by the mouse, 64 tile pict, 128 pict size, 0x100 bold, 0x200 italic

 Displays text or/and picture on the screen.

 text - on-screen display (OSD) text. Can be several lines.
 timeS - OSD time, seconds. Default or 0: depends on text length. -1 is infinite.
   You can use function OsdHide to close the window earlier. Or flag 8 to auto close when macro ends.
 x, y - coordinates. If flag 16 or 32 not used:
   If 0 (default), screen center.
   If > 0, relative to the work area.
   If < 0, relative to bottom/right of the work area.
   The OSD is displayed on the monitor specified by the <help>_monitor</help> variable.
 fname, fsize, fcolor - font name, size (default 24) and color (0 (default) is green, 1 is black).
 flags:
   1 - nontransparent.
   2 - synchronous. Caller waits timeS seconds until OSD disappears.
   4 - user can click to hide. Nontransparent.
   8 (QM 2.2.1) - hide when caller macro (thread) ends.
   16 (QM 2.2.1) - use raw x and y, relative to the primary monitor. Negative and 0 x y don't have special meaning. _monitor variable is ignored. The function still ensures that all text is in the screen. 
   32 (QM 2.2.1) - place by the mouse pointer. x and y are offsets from the mouse pointer position. The functions ensures that the text is in the same monitor as the mouse pointer. _monitor variable is ignored.
   64 (QM 2.3.0) - tile background picture.
   128 (QM 2.3.0) - use size of picture, not of text.
   0x100 (QM 2.3.2) - font bold.
   0x200 (QM 2.3.2) - font italic.
 osdid - some string that identifies the OSD window. If this OSD window already exists, displays text in it instead of creating new window.
   This improves performance when calling this function frequently.
   Also, later you can use win(osdid "QM_OSD_Class") to get OSD window handle, or OsdHide(osdid) to close it.
 bcolor - background color. Flag 1 also should be set, or the color will be transparent (which can be useful with picture).
 wrapwidth (QM 2.2.0) - if >0, wraps long text lines at this width. Default or 0: screen width.
 pictfile (QM 2.3.0) - background picture file. Can be bmp, jpg, gif. QM 2.3.4: also can be png.
   You also should use flag 1 or a background color (bcolor) that is not part of the picture. Otherwise parts of the picture may be transparent.
   Supports <help #IDP_RESOURCES>macro resources</help> (QM 2.4.1) and exe resources.

 REMARKS
 Creates a transparent window where it draws the text and picture.
 Note: By default (without flag 2) does not wait while showing OSD. In exe the OSD disappears as soon as exe process ends.

 EXAMPLES
 OnScreenDisplay "Text"

 OnScreenDisplay "Macro x is running." -1 0 -1 "Comic Sans MS" 0 0xff0000 8
 mes 1

 Dir d
 foreach(d "$my pictures$\*" FE_Dir)
	 str sPath=d.FullPath
	 sel(sPath 3) case ["*.jpg","*.gif","*.bmp"] case else continue
	 OnScreenDisplay d.FileName 1.5 0 0 "Comic sans ms" 16 0xc0c0 128|1 "test_id" 0 0 sPath
	 1


type ___OSDVAR
	~text ^_time x y ~fname fsize fcolor flags ~osdid bcolor wrapwidth ~pictfile
	!created hwnd masterthread monitor __Font'hfont __GdiHandle'hbmp

___OSDVAR* v._new
v.text=text
v._time=timeS
v.x=x; v.y=y
v.fname=fname
v.fsize=iif(fsize fsize 24)
v.fcolor=fcolor
v.flags=flags
v.osdid=osdid
v.bcolor=bcolor
v.wrapwidth=wrapwidth
v.pictfile=pictfile

if(flags&2=0) ;;not sync
	if(!empty(osdid)) v.hwnd=win(osdid "QM_OSD_Class"); else if(flags&8) osdid="no_osdid"
	
	if(flags&8) QMTHREAD q; GetQmThreadInfo 0 &q; v.masterthread=q.threadhandle
	
	if(!IsWindowVisible(v.hwnd))
		v.monitor=_monitor
		mac "sub.Main" "" v
		rep(1000) if(v.created) break; else 0.01 ;;wait for window created because caller may need it immediately
		ret
	 else sub.Main will set params and send msg

sub.Main v


#sub Main
function ___OSDVAR*v

if(v.monitor) _monitor=v.monitor

if(v._time>0) v._time*1000; if(v._time>2147483647.0) v._time=2147483647.0
else if(v._time<0) v._time=-1
else v._time=3000+(iif(v.text.len<=1000 v.text.len 1000)*100)

if(!v.fcolor) v.fcolor=0xc000; else if(v.fcolor=1) v.fcolor=0
if(v.flags&1=0) if(v.flags&4 or ScreenColors<8) v.flags|1
if(!v.bcolor) v.bcolor=GetSysColor(COLOR_BTNFACE) ;;note: when smooth font edges, they are nontransparent. Tried to use bcolor near to fcolor but it does not make better.
if(v.pictfile.len) v.hbmp=LoadPictureFile(v.pictfile)
v.hfont.Create(v.fname v.fsize v.flags>>8&3)

if(v.hwnd) SendMessageW v.hwnd WM_USER 0 v
else
	__RegisterWindowClass+ ___osd_class; if(!___osd_class.atom) ___osd_class.Register("QM_OSD_Class" &sub.WndProc)
	int st(WS_POPUP) exst(WS_EX_TOOLWINDOW|WS_EX_TOPMOST)
	if(v.flags&1) st|WS_BORDER; exst|WS_EX_NOACTIVATE; else exst|WS_EX_TRANSPARENT|WS_EX_LAYERED
	CreateWindowExW(exst +___osd_class.atom @v.osdid st 0 0 0 0 0 0 _hinst v)
	v.created=1
	MessageLoop
err+ ;;out _error.line


#sub WndProc
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	CREATESTRUCTW* cs=+lParam
	___OSDVAR* v=+cs.lpCreateParams
	SetProp hWnd "qm_win_obj" v
	if(v.flags&1=0) SetLayeredWindowAttributes(hWnd v.bcolor 0 1)
	SetTimer hWnd 3 500 0
	goto g1
	
	case WM_USER
	v=+GetProp(hWnd "qm_win_obj")
	v._delete; v=+lParam
	SetProp hWnd "qm_win_obj" v
	 g1
	if(v.masterthread) SetTimer hWnd 2 100 0
	int wid hei ww retry monf monitor
	if(v.flags&32) v.flags|16; monitor=-1; v.x+xm; v.y+ym; else if(v.flags&16=0) monitor=_monitor
	GetWorkArea 0 0 ww
	 g2
	RECT rt
	if(v.flags&128 and TO_GetBitmapRect(v.hbmp rt))
	else
		int hdc=GetDC(hWnd)
		int oldfont=SelectObject(hdc v.hfont)
		rt.right=iif(v.wrapwidth>0 v.wrapwidth ww)
		DrawTextW hdc @v.text -1 &rt DT_CALCRECT|DT_WORDBREAK|DT_EXPANDTABS|DT_NOPREFIX
		if(v.flags&0x200) rt.right+v.fsize/4 ;;avoid clipping part of last char when italic
		SelectObject hdc oldfont; ReleaseDC hWnd hdc
	if(v.flags&1) rt.right+8; rt.bottom+3
	wid=rt.right; hei=rt.bottom
	
	OffsetRect &rt v.x v.y
	AdjustWindowPos 0 &rt iif(v.flags&16 1|4|8 1)|monf monitor
	
	if(!retry and v.wrapwidth<=0) ;;maybe nonprimary monitor, different width
		monitor=MonitorFromRect(&rt 2); monf=32
		if(monitor!=MonitorFromWindow(0 1))
			MonitorFromIndex monitor 33 &rt
			ww=rt.right-rt.left
			RECT r0; rt=r0
			retry=1; goto g2
	
	if(IsWindowVisible(hWnd))
		SetWindowPos hWnd HWND_TOPMOST rt.left rt.top wid hei SWP_NOACTIVATE
		sub.Paint hWnd v 0
		MSG msg; PeekMessage &msg hWnd WM_TIMER WM_TIMER 1
	else SetWindowPos hWnd 0 rt.left rt.top wid hei SWP_NOACTIVATE|SWP_NOZORDER|SWP_SHOWWINDOW
	if(v._time!=-1) SetTimer hWnd 1 v._time 0
	
	case WM_PAINT
	sub.Paint hWnd +GetProp(hWnd "qm_win_obj") 1
	ret
	
	case WM_TIMER
	sel(wParam)
		case 1 DestroyWindow hWnd
		case 2
		v=+GetProp(hWnd "qm_win_obj")
		if(!v.masterthread) KillTimer hWnd 2
		else if(WaitForSingleObject(v.masterthread 0)!=WAIT_TIMEOUT) DestroyWindow hWnd
		case 3 SetWindowPos hWnd HWND_TOPMOST 0 0 0 0 SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE
	ret
	
	case WM_LBUTTONDOWN
	v=+GetProp(hWnd "qm_win_obj")
	if(v.flags&4) DestroyWindow hWnd
	
	case WM_DESTROY
	v=+RemoveProp(hWnd "qm_win_obj")
	v._delete
	PostMessage 0 2000 0 0
	
	case WM_SETTEXT
	ret

ret DefWindowProcW(hWnd message wParam lParam)


#sub Paint
function hWnd ___OSDVAR*v onwmpaint

PAINTSTRUCT ps; RECT r; int hdc

GetClientRect(hWnd &r)
if(onwmpaint) hdc=BeginPaint(hWnd &ps)
else ValidateRect hWnd 0; hdc=GetDC(hWnd)

__GdiHandle hbc=CreateSolidBrush(v.bcolor)
FillRect hdc &r hbc

if(v.hbmp)
	__GdiHandle br=CreatePatternBrush(v.hbmp)
	RECT ri=r
	if(v.flags&64=0) TO_GetBitmapRect(v.hbmp ri)
	FillRect hdc &ri br

if(v.flags&1) r.left+3
SetTextColor(hdc v.fcolor)
SetBkMode(hdc 1)
int oldfont=SelectObject(hdc v.hfont)
DrawTextW(hdc @v.text -1 &r DT_WORDBREAK|DT_EXPANDTABS|DT_NOPREFIX)
SelectObject(hdc oldfont)

if(onwmpaint) EndPaint(hWnd &ps) else ReleaseDC hWnd hdc
