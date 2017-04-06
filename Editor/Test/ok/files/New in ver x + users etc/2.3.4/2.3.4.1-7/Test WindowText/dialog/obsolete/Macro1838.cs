out
int w=win("dlg_apihook" "#32770")
int c=id(100 w)
RECT rc
GetClientRect c &rc
zRECT rc

MapWindowPoints c w +&rc 2
zRECT rc

 mou 10 10 c
mou 100 0 c 1
POINT P; xm P; outw WindowFromPoint(P.x P.y)
1; ret
POINT pp.x=10; pp.y=20; ClientToScreen c &pp;; mou pp.x pp.y; 1
RECT rw; GetWindowRect c &rw
 MapWindowPoints
pp.x-rw.left; pp.y-rw.top
mou pp.x pp.y c

 Acc a=acc(c)
 int X W
 a.Location(X 0 W)
 out "%i %i" X W

 GetWindowRect c &rc
 zRECT rc
 MapWindowPoints 0 w +&rc 2
  MapWindowPoints 0 c +&rc 2
 zRECT rc

__Hdc dc.FromWindowDC(c)
POINT p; if(GetDCOrgEx(dc.dc &p)) out "%i %i" p.x p.y
POINT _p; ClientToScreen c &_p; out "%i %i" _p.x _p.y
 RECT _r; MapWindowPoints c 0 +&_r 2; zRECT _r
RECT r; SetRect &r p.x p.y p.x p.y; MapWindowPoints 0 c +&r 2; zRECT r
