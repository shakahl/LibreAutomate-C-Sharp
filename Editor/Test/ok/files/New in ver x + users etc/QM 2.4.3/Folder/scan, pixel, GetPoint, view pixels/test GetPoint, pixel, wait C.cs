int w=win("Registry Editor" "RegEdit_RegEdit")
int c=id(1 w)
 mou 10 30 w 1
 opt slowmouse 1
 rig 10 30 w 1|2|4
 outx pixel(95 25 w 1|0x1000)
 outx pixel(116 36 w 1|0x1000)
 outx pixel(116 -36 0 2)
 outx pixel(116 -36 w 2)
 hid w
 min w
 out IsWindowVisible(c)
 outx pixel(116 136 w 1|0x1000)
 outx pixel(116 136 w 1|0)
 hid- w
 res w
 wait 0 C 0xDB903A 116 136 w 1|0|0x1000

 wait 0 C 0 0 0 win() 1
 wait 0 C 0 0 0 win()
 wait 0 C 0 0 0
 wait 0 C 0 0 0 win() 1|2
 wait 0 C 0 0 0 win() 0x1000
 wait 0 C 0 0 0 win() 1|0x1000
 wait 0 C 0 0 0 win() 1|2|0x1000
 wait 0 C 0 0 0 win() 2|0x1000
 int w1=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 wait 0 C 0x4BBBF0 1673 300 w1 1|0x1000
