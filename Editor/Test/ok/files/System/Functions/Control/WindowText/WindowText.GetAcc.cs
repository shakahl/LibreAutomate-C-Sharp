function'Acc WTI*t [offsetx] [offsety]

 Gets accessible object from text item.

 t - address of a text item variable. You can pass the return value of Find() or Wait(). Or pass address of an element of member array a of this variable.
 offsetx, offsety - get object that is at this offset from top-left corner of the text item (of its text rectangle).

 REMARKS
 This function does not capture or find text. Find(), Wait() or Capture() must be called before.

 EXAMPLE
 int w=win("Options" "#32770")
 WindowText x.Init(w)
 Acc a=x.GetAcc(x.Find("Run"))
 out a.Name


if(!t) end ERR_OBJECT

int userOffs=getopt(nargs)>1
RECT& r=iif((userOffs or t.flags&WTI_INVISIBLE) t.rt t.rv) ;;info: Find() by default skips invisible items; if user wants to find anyway, we must obey.
if(!userOffs) offsetx=r.right-r.left/2; offsety=r.bottom-r.top/2

int w(t.hwnd) x(r.left+offsetx) y(r.top+offsety)
if(m_flags&WT_SINGLE_COORD_SYSTEM) w=m_hwnd

ret acc(x y w 1)

err+ end _error
