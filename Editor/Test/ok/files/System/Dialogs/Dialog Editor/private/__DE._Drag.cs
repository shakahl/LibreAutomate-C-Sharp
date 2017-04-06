 \Dialog_Editor
function# msg xy

SetFocus _hwnd
int h hprev swp(-1) multi noUndo resize axis
POINT p

if msg=WM_APP+1 ;;new control
	h=xy
	ClientToScreen h &p; SetCursorPos p.x p.y
	noUndo=1 ;;remove 1 undo
else ;;get control from mouse, or form
	p.x=xy&0xffff; p.y=xy>>16
	h=subs.ControlFromPoint(p); if(!h) ret

if(h!=_hsel) hprev=_hsel
_Select(h)

int mod=GetMod
sel msg
	case WM_LBUTTONDOWN
	sel(mod) case [0,4] axis=_edge&3; if(_edge&4) resize=1
	if(h=_hform and !resize) ret
	sel mod
		case [0,4]
		case 1 if(WinTest(h "Button") and GetWinStyle(h)&BS_TYPEMASK=BS_GROUPBOX) multi=1; else swp=0
		case 2 swp=1
		case 3 if(hprev) swp=iif(hprev=_hform 0 hprev); else ret
		case else ret
	case WM_RBUTTONDOWN
	sel(mod) case [0,4] resize=1; case else ret

if swp!=-1 ;;Z-order
	_Undo
	SetWindowPos(h swp 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE)
	ret 1

if multi ;;collect controls for multi-move
	RECT r rr; GetWindowRect(h &r)
	ARRAY(int) adc ;;multi-drag control handles
	int c=GetWindow(_hform GW_CHILD)
	rep
		if(!c) break
		if(IsWindowVisible(c)) GetWindowRect(c &rr); if(PtInRect(&r rr.left rr.top)) adc[]=c
		c=GetWindow(c GW_HWNDNEXT)

int dragCounter=iif(axis 1 3)
__Drag g.Init(_hwnd 1+(msg=WM_RBUTTONDOWN))
rep
	if(!g.Next) break
	if(g.m.message!=WM_MOUSEMOVE) continue
	if dragCounter
		dragCounter-1; if(dragCounter) continue
		GetWindowRect(h &r)
		if resize
			if(h=_hform) GetClientRect(h &r); ClientToScreen h +&r.right
			SetCursorPos(r.right r.bottom)
		else SetCursorPos(r.left r.top)
		
		if(!noUndo) _Undo; noUndo=1
		POINT pp; if(axis) xm pp _hform 1
	else
		p=g.p; MapWindowPoints _hwnd _hform &p 1
		sub.OnMouseMove(h resize iif(axis=1 pp.x p.x) iif(axis=2 pp.y p.y) adc)
	g.cursor=5

ret 1


#sub OnMouseMove c
function h resize x y ARRAY(int)&adc

RECT r rc

if !resize
	GetClientRect _hform &r
	if(x<0) x=0; else if(x>=r.right) x=r.right-1
	if(y<0) y=0; else if(y>=r.bottom) y=r.bottom-1

GetWindowRect(h &r)
if resize
	int swp=SWP_NOMOVE|SWP_NOZORDER
	if h=_hform
		GetClientRect(h &rc)
		int xp(r.right-r.left-rc.right) yp(r.bottom-r.top-rc.bottom)
		r.left=0; r.top=0
	else
		ScreenToClient _hform +&r
		swp|SWP_NOCOPYBITS ;;would not redraw correctly some controls. Form flickers with this flag.
	x-r.left; if(x<1) x=1
	y-r.top; if(y<1) y=1
	sub.Grid(x y 1)
	SetWindowPos(h 0 0 0 x+xp y+yp swp)
	if(h=_hform) subs.AutoSizeEditor
else
	sub.Grid(x y 0)
	if(adc.len) sub.MultiMove(adc r x y)
	SetWindowPos(h 0 x y 0 0 SWP_NOSIZE|SWP_NOZORDER|SWP_NOCOPYBITS)
	subs.SetMark


#sub Grid c
function &x &y resize

int grid=iif(ifk(A) 1 _grid); if(!grid) ret
if(resize and grid>2) grid=2

int X=MulDiv(x 4 _dbx)
int Y=MulDiv(y 8 _dby)
X+grid*0.4
Y+grid*0.4
x=X-(X%grid)
y=Y-(Y%grid)
if resize
	if(x<1) x=1
	if(y<1) y=1
x=MulDiv(x _dbx 4)
y=MulDiv(y _dby 8)


#sub MultiMove c
function ARRAY(int)&adc RECT&r &x &y

int i h dx dy
RECT rr
dx=r.left; dy=r.top; ScreenToClient(_hform +&dx)
dx=x-dx; dy=y-dy

for i 0 adc.len
	h=adc[i]
	GetWindowRect(h &rr); rr.left+dx; rr.top+dy
	ScreenToClient(_hform +&rr)
	SetWindowPos(h 0 rr.left rr.top 0 0 SWP_NOSIZE|SWP_NOZORDER|SWP_NOCOPYBITS)
