 /
function# int&w [RECT&r]

 Allows the user to capture handle of a window or control in screen, and optionally a rectangle in it.
 Returns:
   0 not captured. User pressed Esc. w and r not changed.
   1 captured only window.
   2 captured window and rectangle.

 w - variable that receives window handle.
 r - variable that receives rectangle in the window, in client coordinates. Optional.

 REMARKS
 User selects the window with mouse and Shift.

 Added in: QM 2.3.4.


if(&r) _s="[]You can Shift+move mouse to draw rectangle."
sub_sys.TooltipOsd F"Move mouse to a window or control, and press Shift.{_s}" 4|8 "CWAR" -1

int R=sub.Capture(w r)

OsdHide "CWAR"

ret R


#sub Capture
function# int&w [RECT&r]

 wait for Shift or Esc, and draw window/control rect

opt waitmsg 1 ;;caller may be dialog etc

int captured
RECT rw
__OnScreenRect osr osrd
rep
	0.01
	int _w=child(mouse 1)
	DpiGetWindowRect _w &rw; osr.Show(0 &rw)
	ifk(S) captured=1; break
	ifk(Z) break

if(!captured) ret
w=_w
if(!&r) ret 1

 wait for Shift up, and draw rect

int wMain=GetAncestor(w 2)
DpiGetWindowRect wMain &rw; ClipCursor &rw

osrd.SetStyle(0xff0000 1)
int drag; RECT rd
xm +&rd
rep
	0.01
	ifk-(S) break
	xm +&rd.right
	if(rd.right<rd.left) rd.left=rd.right
	if(rd.bottom<rd.top) rd.top=rd.bottom
	if(!IsRectEmpty(&rd)) drag=1
	if(drag) osrd.Show(0 &rd)

ClipCursor 0

if(!drag or IsRectEmpty(&rd)) ret 1

int w2=child(mouse); if(w2!=w) w=wMain ;;if rect spans multiple controls, get top-level window
 DpiMapWindowPoints 0 w +&rd 2
DpiScreenToClient w +&rd 0x100
r=rd
ret 2
