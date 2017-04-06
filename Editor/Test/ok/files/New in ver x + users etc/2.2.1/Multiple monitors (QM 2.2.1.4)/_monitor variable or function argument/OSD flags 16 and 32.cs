 _monitor=2
 str s="some text"
str s="AAAAAAA AAAAAAA AAAAAAAA AAAAAAAA BBBBBB BBBBBBB BBBBBBB BBBBBBB CCCCCC CCCCCCCC CCCCCCCC CCCCCC DDDDD DDDDDD DDDDDD DDDDDD EEEEEEEE EEEEEEE"
 OnScreenDisplay s 5 0 0 "" 0 0 0
 OnScreenDisplay s 5 0 0 "" 0 0 16

 OnScreenDisplay s 3 xm ym "" 0 0 16

 _monitor=-1
 OnScreenDisplay s 3 xm ym "" 0 0 16
 
 OnScreenDisplay s 3 0 0 "" 0 0 32
 OnScreenDisplay s 3 100 -100 "" 0 0 32
OnScreenDisplay s 3 0 0 "" 0 0 32 "" 0 300
