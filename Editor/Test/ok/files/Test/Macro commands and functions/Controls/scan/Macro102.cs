ClearOutput
str s="$desktop$\test.bmp"
 str s="$desktop$\test2.bmp"
 str s="$desktop$\test 200x200.bmp"
 str s="$desktop$\test.ico"
 str s="$desktop$\test2.ico"

int hwnd=child("Notification Area" "ToolbarWindow32" "+Shell_TrayWnd" 0x1)
 int hwnd=win("" "Shell_TrayWnd")
RECT r

long t1=perf

 int z=scan(s)
 int z=scan(s _hwndqm)

 int z=scan(s hwnd)

 r.left=850
 r.right=900
 int z=scan(s 0 r)

int z=scan(s 0 r 0)

long t2=perf
if(z) out "%I64i  %i %i" t2-t1 r.left r.top
