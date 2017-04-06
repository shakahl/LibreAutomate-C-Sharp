 lef 630 432
 lef 14 118 "QM TOOLBAR"
 lef 242 10 id(2453 "MSDN Library - January 2002 - _msize") 1
 lef+
 lef- 768 289 "MSDN Library - January 2002 - _msize"
 dou 0.5 0.5 h
 rig 5 5 win("Notepad" "Notepad" "notepad")
 mid 426 72 "Find"; err ret
 mou 787 252 "MSDN Library - January 2002 - _msize"
 mou- 5 6
 POINT _m; xm _m
 mou+ 8 0
 1
 mou _m.x _m.y
 mou
 int xxx=xm()
 POINT p; xm(p "MSDN Library - January 2002 - _msize")
 MouseWheel(2)
 rig+ 0.5 0.5 "wind"; err
 rig- 0.5 0.5 "Mouse"; err ErrMsg(1)
 mid+; err ErrMsg
 mid- 1 1 "hh"; err ErrMsg(2)
 out 1
 int p=ym(0 child(1129 "&Find Text" "Button" "Find" 0x1) 1)
 err
	 out "err"
