out
RECT r
int h=id(2202 _hwndqm)
GetClientRect h &r
MapWindowPoints h 0 +&r 2
ARRAY(int) a
Q &q
GetRectPixels r a 0
Q &qq
 int* p=&a[0 0]; int c i n=a.len(1)*a.len(2)
 for(i 0 n) c=p[i]; p[i]=(c&0xff00) | (c&0xff<<16) | (c&0xff0000>>16)
ColorArrayARGBtoBGR a
Q &qqq
outq

 int i
 for i 0 a.len
	 out "

 outb &a[0 0] a.len(1)*a.len(2)*4
