 /out pixels from mouse
function RECT&r

 copy the rectangle from screen to memory dc
int wid(r.right-r.left) hei(r.bottom-r.top)
int dc=GetDC(0)
__MemBmp mb.Create(wid hei dc r.left r.top)
ReleaseDC 0 dc

 get pixel colors
ARRAY(int) a.create(wid*hei)
BITMAPINFOHEADER h.biSize=sizeof(h)
h.biBitCount=32; h.biWidth=wid; h.biHeight=-hei; h.biPlanes=1
if(GetDIBits(mb.dc mb.bm 0 hei &a[0] +&h DIB_RGB_COLORS)!=hei) ret

 show pixel colors in hex format. Note that R and B are swapped, ie the least significant byte (at the right) is B, not R.
str s
int row i
for row 0 hei 
	for i 0 wid
		s.formata("%06X " a[row*wid+i]&0xffffff)
	s+"[]"
out s
