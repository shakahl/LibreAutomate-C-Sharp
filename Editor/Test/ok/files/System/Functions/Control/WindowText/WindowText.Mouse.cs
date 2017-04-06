function button WTI*t [offsetx] [offsety] ;;button: 0 move, 1 left, 2 right, 3 middle, 4 left double

 Moves mouse pointer to a text item, and optionally clicks.

 button - if not 0, clicks a mouse button. See above.
 t - address of a text item variable. You can pass the return value of Find() or Wait(). Or pass address of an element of member array a of this variable.
 offsetx, offsety - move mouse to this offset from top-left corner of the text item (of its text rectangle). If not used, moves to the center of the visible part of its text rectangle.

 REMARKS
 This function does not capture or find text. Find(), Wait() or Capture() must be called before.

 EXAMPLE
  click text "findme" in Notepad
 int w=id(15 win("Notepad" "Notepad")) ;;get handle of Notepad edit control
 WindowText x.Init(w)
 x.Mouse(1 x.Find("findme"))


if(!t) end ERR_OBJECT

int userOffs=getopt(nargs)>2
RECT& r=iif((userOffs or t.flags&WTI_INVISIBLE) t.rt t.rv) ;;info: Find() by default skips invisible items; if user wants to find anyway, we must obey.
if(!userOffs) offsetx=r.right-r.left/2; offsety=r.bottom-r.top/2

int w(t.hwnd) x(r.left+offsetx) y(r.top+offsety)
if(m_flags&WT_SINGLE_COORD_SYSTEM) w=m_hwnd

spe -1
opt slowmouse -1
sel button
	case 0 mou x y w 1
	case 1 lef x y w 1
	case 2 rig x y w 1
	case 3 mid x y w 1
	case 4 dou x y w 1
	case else end ERR_BADARG

err+ end _error
