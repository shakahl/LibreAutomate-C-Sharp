function! RECT&r

 Calculates MD5 of pixels in specified rectangle on screen, and stores into this str variable as hex string.
 Returns 1 if successful, 0 if fails.

 EXAMPLE
 RECT r
 r.left=10; r.top=10; r.right=100; r.bottom=100
 str md5
 md5.ScreenRectMD5(r)
 out md5


 copy the rectangle from screen to memory dc
int wid(r.right-r.left) hei(r.bottom-r.top)
int dc=GetDC(0)
__MemBmp mb.Create(wid hei dc r.left r.top)
ReleaseDC 0 dc

 get pixel colors
_s.all(wid*hei*4 2)
BITMAPINFOHEADER h.biSize=sizeof(h)
h.biBitCount=32; h.biWidth=wid; h.biHeight=-hei; h.biPlanes=1
if(GetDIBits(mb.dc mb.bm 0 hei _s +&h DIB_RGB_COLORS)!=hei) ret

 md5
this.encrypt(10 _s)
ret 1
