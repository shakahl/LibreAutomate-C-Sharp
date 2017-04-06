int hwnd=val(_command)
 outw hwnd
out "-------"

ret
0.1
RECT r; GetWindowRect hwnd &r
int dc=GetDC(0)
__MemBmp mb.Create(r.right-r.left r.bottom-r.top dc r.left r.top)
ReleaseDC 0 dc

str s="$temp$\flick.bmp"
SaveBitmap mb.bm s
run s
