out
RECT r; SetRect &r 200 200 300 300

 OnScreenRect 1 r
 1
 OnScreenRect 3 r
 1
 OnScreenRect 0 r
 1
  OnScreenRect 2 r

__OnScreenRect osr
 osr.SetStyle(0xff0000)
osr.SetStyle(0xff0000 1)

osr.Show(1 r)
5
osr.Show(3 r)
1
osr.Show(1 r)
1
