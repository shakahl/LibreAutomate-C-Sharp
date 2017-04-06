out
 scan "Macro1378.bmp" 0 0 0x3 10 100

 if(!CaptureImageOrColor(_i 1)) ret
 outx _i

 scan "color:0xDEDAF5" 0 0 0x3
 scan F"color:{0xDEDAF5}" 0 0 0x3
 int c=0xDEDAF5
int c=0x82b665
 scan F"color:{c}" 0 0 0x3

int w1=win("Last.fm" "QWidget")
 scan F"color:{c}" w1 0 0x3|0x100

 wait 0 S F"color:{c}" w1 0 0x3|0x400
 wait 0 -S F"color:{c}" w1 0 0
