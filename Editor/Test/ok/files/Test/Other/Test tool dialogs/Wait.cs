 0.1
 10

 0 "Untitled - Notepad"
 0 id(1 "Font")
 50 WN "Untitled - Notepad"
 50 WN id(1001 "Font")
 0 WC "Calc"
 0 WD "Font"
 0 WP "Font"
 0 WV "+Shell_TrayWnd"
 0 WE "Untitled - Notepad"
 5 WP "Notepad"

 int v=wait(0 K k)
 out v
 5 KF (VK_TAB)
 int v
 v=wait(0 KF a); err
 out v

 0 ML
 5 MR
 0 MM
 int v5=wait(0 M)
 out v5

 1
 0 P 3

 int+ _ho=0
 int vv=wait(0 V _ho)
 out vv

 0 H _wait
 WaitPixelColor(0 0xFFFFFF 820 91)
 WaitPixelColor(0 0xA6A6A6 601 12 win("app - Microsoft Visual C++ [design] - RunWait.cpp"))
 WaitPixelColor(0 0x0 5 10 id(15 "Untitled - Notepad"))

 IeWait(0)
 IeWait(0 0 "url"); err ErrMsg(1)

 spe 40
