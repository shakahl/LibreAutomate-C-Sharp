out

 POINT p; int h
 PF
 GetPhysicalCursorPos &p
 PN
 h=WINAPIV.WindowFromPhysicalPoint(p)
 PN
 WINAPIV.PhysicalToLogicalPoint h &p
 PN
 int hh=WindowFromPoint(p.x p.y)
 PN; PO
 outw h
 outw hh
 
 #ret
 int hm=SendMessage(h MN_GETHMENU 0 0)
 out hm
  GetCursorPos &p ;;broken. Sometimes gets logical, but most of the times physical.
 WINAPIV.PhysicalToLogicalPoint(h &p)
 int i=MenuItemFromPoint(0 hm p)
 out i
  for p.y 0 ScreenHeight 10
	  for p.x 0 ScreenWidth 30
		  if(MenuItemFromPoint(0 hm p)>=0) out "%i %i" p.x p.y; ret
 #ret
 
  out ym
  POINT p; GetCursorPos &p; out p.y
   CURSORINFO c.cbSize=sizeof(c); GetCursorInfo &c; out c.ptScreenPos.y
  GetPhysicalCursorPos &p; out p.y
 
 int w=win("Dialog" "#32770")
 if(!w) mac "Dialog131"; 1; w=win("Dialog" "#32770")
  w=id(3 w)
   int w=_hwndqm
   hid w; 0.5
  POINT p.y=-5; ClientToScreen w &p
  out LogicalToPhysicalPoint(w &p); out p.y
   hid- w
 
 RECT r
 PF
  _i=DpiIsWindowScaled(w)
 _i=DpiGetWindowRect(w &r 2)
 PN; PO
 out _i
 zRECT r
 #ret

PF
int x y w h
int hwnd=GetCaretXY(x y w h)
PN
IAccessible a
int hr=AccessibleObjectFromWindow(hwnd OBJID_CARET uuidof(IAccessible) &a)
 outx hr ;;error if hwnd 0
if(!hr)
	VARIANT v=0
	int x2 y2 w2 h2
	a.Location(x2 y2 w2 h2 v)
PN; PO
out "%i %i  %i %i" x y w h
out "%i %i  %i %i" x2 y2 w2 h2
 PostMessage _hwndqm WM_USER+33 0 0
 OutWinProps hwnd
