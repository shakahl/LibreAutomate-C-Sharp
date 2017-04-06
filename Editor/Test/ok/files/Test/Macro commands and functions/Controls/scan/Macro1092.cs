ClearOutput
str s
 s="qm.exe"
s="$desktop$\e.bmp"
int h=id(2210 _hwndqm)
 int h=_hwndqm
RECT r
 r.left=1
 r.top=150
 r.right=ScreenWidth
 r.bottom=ScreenHeight

 r.left=100
 r.top=100
 r.right=200
 r.bottom=200
 spe
wait 0 S s 0 r 0
 int z=scan(s _hwndqm r 0)

out "%i %i %i %i" r.left r.top r.right r.bottom
