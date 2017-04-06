dll "qm.exe" ColorArrayARGBtoBGR2 *a n

out
RECT r
int h=id(2202 _hwndqm)
GetClientRect h &r
MapWindowPoints h 0 +&r 2
ARRAY(int) a
Q &q
GetRectPixels r a 0
Q &qq
ColorArrayARGBtoBGR2 +a.psa.pvData a.len(1)*a.len(2)
Q &qqq
outq

 int i
 for i 0 a.len
	 out "

outb &a[0 0] a.len(1)*a.len(2)*4
