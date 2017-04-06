ClearOutput
str s
s="$desktop$\e.bmp"
RECT r
 r.left=1
 r.top=150
 r.right=ScreenWidth
 r.bottom=ScreenHeight

 r.left=1024
 r.top=0
 r.right=200
 r.bottom=200

int t1=perf
 int z=scan(s 0 r 0)
int z=scan(s _hwndqm r 64)
int t2=perf
out t2-t1

if(z) out "%i %i" r.left r.top
