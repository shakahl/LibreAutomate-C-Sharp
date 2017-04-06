ClearOutput
str s
 s="$qm$\qm.exe"
 s.getwinexe(win("Macromedia") 1)

 0.3
 key (VK_SNAPSHOT)
 0.3
 s="$desktop$\temp.bmp"
 s.getclip(CF_BITMAP)

 s="$desktop$\M.bmp"
 s="$desktop$\e.bmp"
 s="$desktop$\e.ico"
 s="$desktop$\e magenta.bmp"
 s="$desktop$\test2.bmp"
 s="$desktop$\test.ico"
 s="$desktop$\test2.ico"
 s="$desktop$\notepad.ico"
 s="$desktop$\book.ico"
s="TO_Scan.bmp"

 int h=win("Display Properties")
 int h=win("" "Shell_TrayWnd")
RECT r
 r.left=1
 r.top=150
 r.right=ScreenWidth
 r.bottom=ScreenHeight

 int t1=perf
int z=scan(s 0 r 1)
lef
 int t2=perf
 out t2-t1

if(z) out "%i %i" r.left r.top
