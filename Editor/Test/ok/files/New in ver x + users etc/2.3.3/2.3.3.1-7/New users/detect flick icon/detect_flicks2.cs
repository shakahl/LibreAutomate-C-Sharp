int hwnd=val(_command)
 outw hwnd
out "-------"
 ARRAY(int) a; child "" "" hwnd 0 0 0 a
 int i; for(i 0 a.len) outw a[i]
 RECT r; GetWindowRect hwnd &r; zRECT r

 Acc a.Find(hwnd "" "" "" 16 0 0 "" &detect_flicks_acc_callback)

if(_winnt<6) ret
dll- user32 #PrintWindow hwnd hdcBlt nFlags

RECT r; GetWindowRect hwnd &r
__MemBmp mb.Create(r.right-r.left r.bottom-r.top)
if(!PrintWindow(hwnd mb.dc 0)) ret

str s="$temp$\flick.bmp"
SaveBitmap mb.bm s
run s
