
 out
 act win("Sample Pictures" "CabinetWClass")
 mou 500 300
str s
 s="qm.exe,0"
s="$system$\notepad.exe"
 s="$program files$\Mozilla Firefox\firefox.exe"
 s="$my qm$"
 s="$my qm$\file.txt" ;; 81137  13755  4824  3516 (+- 7600, noinline 13000)
 s="Macro1367.bmp"
 s="Macro1367.gif"
 s="Macro1367.jpg"
 s="Macro1367.png"
 s="Macro1367 (2).bmp"

 int w2=win("My QM" "CabinetWClass")
int w2=win("system32")
 int w2=win("Calculator")
 int w2=child("" "DirectUIHWND" win("My QM" "CabinetWClass") 0 0 0 3)

RECT r
 act
 scan s 0 0 3
 scan s 0 0 3 8
 scan s w2 0 3 8
 scan s w2 r 3 8
scan s w2 r 3|0x100|16 8
zRECT r
 RECT r.top=100
 scan s w2 r 3|0x100 8
 wait 0 S s 0 0 3 8
 wait 0 S s w2 0 3|0x100 8
 1; mou

  s="$myqm$\Macro1367 - explorer winth notepad icon.bmp"
 __GdiHandle h=LoadPictureFile("$myqm$\Macro1367 - explorer winth notepad icon.bmp")
 out h
 RECT r
 scan s h r 3|0x200 8
 zRECT r
  __MemBmp mb.Attach(h)
  scan s h r 3|0x200 8
  wait 10 S s h r 3|0x200 0
  zRECT r

 135  41  46  34  135  -88482576  -1  
