out
int w=win("dlg_apihook" "#32770")
int c=child("D1" "#32770" w)

RECT rw rc
GetWindowRect(c &rw)
GetClientRect(c &rc)
MapWindowPoints(c 0 +&rc 2)
zRECT rw
zRECT rc

__Hdc dc.FromWindowDC(c)
POINT p; if(GetDCOrgEx(dc.dc &p)) out "%i %i" p.x p.y
