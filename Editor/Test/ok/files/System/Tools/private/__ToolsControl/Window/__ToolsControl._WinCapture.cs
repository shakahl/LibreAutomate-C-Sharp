 /dialog_QM_Tools
function# [flags] [POINT&pt] ;;flags: 1 get window from pt and fill controls (don't capture). 2 Shift-capture. 4 pt is window or control handle (don't capture)

 Called on WM_LBUTTONDOWN on the Drag control. Captures a window etc and fills controls.
   At first sends __TWN_DRAGBEGIN to parent dialog. wParam is mw_what. If returns nonzero, just returns 0, eg parent can drag itself.
   Finally sends __TWN_DRAGEND to parent dialog. wParam is handle (window, control or 0), lParam is POINT* in screen coord.
 Also can be called with flag 1 or 4 from everywhere to just fill controls.
   Then does not send __TWN_DRAGBEGIN/__TWN_DRAGEND.
   If flag 1, the required window/control must be visible.
 Parent dialog can send __TWM_DRAGDROP(flags pt) to call this func.
 Returns captured window handle (window, control or 0) or -1 on error.


if(mw_capturing) ret -1

int captured; POINT pCaptured
__OnScreenRect osr

if flags&1
	captured=sub.CaptureRect(6 osr pt pCaptured)
	goto gDropped ;;parent ended drag drop
else if flags&4 ;;note: this code does not check mw_what/mw_lock, and not tested all possible cases; currently don't need it; _WinSelect will correct if wrong
	_i=&pt; mw_captW=GetAncestor(_i 2); mw_captC=iif(mw_captW=_i 0 _i)
	if(!mw_captW) captured=3; else captured=iif(mw_captC 2 1)
	goto gDropped

if(SendMessageW(m_hparent __TWN_DRAGBEGIN mw_what 0)) ret

 drag-capture
__MinimizeDialog m.Minimize(m_hparent)
mw_capturing=1
if flags&2
	opt waitmsg 1
	rep
		0.01
		ifk(S) captured=sub.CaptureRect(2 osr 0 pCaptured); break; else ifk(Z) break
		POINT p pp; xm p; if(memcmp(&p &pp 8)) pp=p; else continue
		sub.CaptureRect(0 osr)
else
	__Drag d.Init(m_hwnd 1)
	rep() sub.CaptureRect(0 osr); if(!sub_to.DragTool_Loop(d)) break
	if(d.dropped) captured=sub.CaptureRect(2 osr 0 pCaptured)
mw_capturing=0

 gDropped
if(!captured) ret -1

str sw sc c1 c2
str* pc2; if(m_flags&0x400) pc2=&c2
mw_comments.all
if mw_captC
	RecGetWindowName(mw_captC &sc 0 &sw &c1 pc2)
	if(c1.len) mw_comments.from(" ;;" c1); if(pc2 and c2.len) mw_comments+F", {c2}"
else if mw_captW
	RecGetWindowName(mw_captW &sw 0 0 0 pc2)
	if(pc2 and c2.len) mw_comments.from(" ;;" c2)

sub_to.SetTextNoNotify mw_heW sw
sub_to.SetTextNoNotify mw_heC sc
_WinSelect(captured)
if(mw_what) SendMessageW m_hparent __TWN_WINDOWCHANGED mw_captW mw_heW

int h; sel(mw_what) case 1 h=mw_captW; case 2 h=mw_captC
if(flags&5=0) SendMessageW m_hparent __TWN_DRAGEND h &pCaptured ;;notify parent
ret h


#sub CaptureRect c
function# flags __OnScreenRect&osr [POINT&p] [POINT&pCaptured] ;;flags: 0 begin/move, 2 end, 3 temp hide, 4 don't draw.

 Gets window from mouse or p, and draws rect.
 Returns: 0 not captured, 1 window, 2 control, 3 screen.
 If flags&3=2 and successfully captured, fills mw_capX and returns nonzero. Else returns 0.

int captured h hw hc noDraw=flags&4
flags&3

POINT _p; if(!&p) &p=_p; xm p
hc=child(p.x p.y 0)
hw=iif(hc GetAncestor(hc 2) win(p.x p.y))

sel mw_what
	case [-1,2] h=hc; if(!h and mw_lock!2) h=hw
	case 1 h=hw
	case else noDraw=1

if flags=2 and (h or !mw_what)
	if(!h) captured=3; else if(h=hc) captured=2; else captured=1
	mw_captW=hw; mw_captC=hc; if(&pCaptured) pCaptured=p

if !noDraw
	if(h) RECT r; DpiGetWindowRect(h &r); osr.Show(flags r)
	else osr.Show(iif(flags=2 2 3))

ret captured
