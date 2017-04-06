int w=win("Acc in-process" "#32770")
if(!w) end "no window"

int wParam=GetCurrentProcessId;; out wParam
int R=SendMessage(w WM_APP+1 wParam MakeInt(1571 540))
0.1
 out R
if(R<=0) ret
Acc a
int hr=ObjectFromLresult(R IID_IAccessible wParam &a.a); if(hr) out "error 0x%X" hr; ret
 out a.a
a.elem=3
out a.Name
a.Role(_s); out _s
int x y wid hei; a.Location(x y wid hei); RECT r; SetRect &r x y x+wid y+hei
if(_winver>=0x603) DpiScale +&r 2 ;;for objects returned like this (LresultFromObject/ObjectFromLresult), Location gives logical coord, different than normal objects
OnScreenRect 0 r; 1

 outx GetCurrentProcessId
 int xy=SendMessage(w WM_APP 1571 540)
 int x=xy&0xffff
 int y=xy>>16
 out "%i %i" x y

 Acc a
  a.FromWindow(w OBJID_CLIENT)
 a.FromXY(1571 540)
  POINT p.x=1571; p.y=540; VARIANT v; if(AccessibleObjectFromPoint(p &a.a &v)) ret; else a.elem=v.lVal
 0.1;out "----"
 int x y cx cy; a.Location(x y cx cy)

 UIA.IUIAutomationElement e=Uia(w "three")
 0.1
 out e
 out e.CurrentName
