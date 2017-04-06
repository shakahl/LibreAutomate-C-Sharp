out
 int w=_hwndqm
int w=win("Notepad")
int dcw dcc
Q &q
dcw=GetWindowDC(w)
dcc=GetDC(w)
out "%i %i" dcw dcc

 out GetCurrentObject(dcw OBJ_BITMAP)
 out GetCurrentObject(dcc OBJ_BITMAP)

POINT pc pw
GetDCOrgEx(dcc &pc); out pc.y
GetDCOrgEx(dcw &pw); out pw.y

 GetViewportOrgEx(dcc &pc); out pc.y
 GetViewportOrgEx(dcw &pw); out pw.y

 RECT rw rc
 GetClipBox dcw &rw; zRECT rw
 GetClipBox dcc &rc; zRECT rc

ReleaseDC w dcc
ReleaseDC w dcw
Q &qq; outq
