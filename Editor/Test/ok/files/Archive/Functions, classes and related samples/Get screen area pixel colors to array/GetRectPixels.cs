 /
function! RECT&r ARRAY(int)&a [flags] ;;flags: 1 ARGB->BGR

 Gets colors of a rectangle from screen.
 Returns 1. On error returns 0.
 Faster than calling the pixel() function many times.

 r - rectangle coordinates.
 a - receives colors.
   The function creates 2-dim array where dimension 1 is for columns, dimension 2 is for rows.
   Color format is 0xAARRGGBB. It is different from the format that is used with various QM functions (0xBBGGRR).
   If flags contains 1, the function converts all pixels to the 0xBBGGRR format. However it slows down, especially when the rectangle is big.
   If you need 0xBBGGRR only for some pixels, use ColorARGBtoBGR.

 EXAMPLE
 out
 int hwnd=id(137 "Calculator") ;;button 7
 RECT r; ARRAY(int) a
 GetWindowRect hwnd &r
 if(!GetRectPixels(r a 0)) end "failed"
 int row col
 for row 0 a.len(2)
	 out "row %i" row
	 for col 0 a.len(1)
		 out "0x%X" a[col row]


 copy the rectangle from screen to memory dc
int wid(r.right-r.left) hei(r.bottom-r.top)
__MemBmp mb.Create(wid hei 1 r.left r.top)

 get pixel colors
a.create(wid hei)
BITMAPINFOHEADER h.biSize=sizeof(h)
h.biBitCount=32; h.biWidth=wid; h.biHeight=-hei; h.biPlanes=1
if(GetDIBits(mb.dc mb.bm 0 hei &a[0 0] +&h DIB_RGB_COLORS)!=hei) ret

 convert color format (slow)
if flags&1
	int* p=&a[0 0]
	int i n=wid*hei
	for i 0 n
		int c=p[i]
		p[i]=(c&0xff00) | (c&0xff<<16) | (c&0xff0000>>16)

ret 1
err+
