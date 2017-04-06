 int w=win(mouse)

 out ym(0 win(mouse))
 lef 100 0 w 2

 RECT r; GetWindowRect w &r
 out
  r.top+4
 out r.top
 out MulDiv(r.top 120 96)
 double d=120.0/96.0
 out r.top*d

 int hdc=GetDC(w)
 out hdc
 if(!hdc) ret
 POINT p
 out GetDCOrgEx(hdc &p)
 ReleaseDC w hdc
 out p.y

 RECT r
 GetWindowRect w &r
  GetClientRect w &r; MapWindowPoints w 0 +&r 2
 r.left=MulDiv(r.left 120 96)
 r.right=MulDiv(r.right 120 96)
 r.top=MulDiv(r.top 120 96)
 r.bottom=MulDiv(r.bottom 120 96)
 OnScreenRect 1 r
 3

out
POINT p
xm p; out "%i %i" p.x p.y
xm p win(mouse); out "%i %i" p.x p.y
xm p win(mouse) 1; out "%i %i" p.x p.y

 lef 0.9 0.9 child(mouse) 0

 int x y cx cy
 GetWinXY win(mouse) x y cx cy
 out "%i %i %i %i" x y cx cy
