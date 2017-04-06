int w1=win("Last.fm" "QWidget")
RECT r

 scan "Macro1376.bmp" 0 r 0x3
 scan "Macro1376.bmp" 0 r 0x3|16

 scan "Macro1376.bmp" w1 r 0x3
 scan "Macro1376.bmp" w1 r 0x3|16
 scan "Macro1376.bmp" w1 r 0x3|0x100
 scan "Macro1376.bmp" w1 r 0x3|0x100|16

 r.top=900
 r.bottom=800
 r.top=1900
 r.bottom=5000
 r.top=-1000
 r.bottom=-1000
 r.top=1; r.bottom=1
 r.top=-1; r.bottom=-1

 scan "Macro1376.bmp" 0 r 0x3
 scan "Macro1376.bmp" 0 r 0x3|16

 r.top=500
 r.bottom=800
 r.top=500; r.bottom=500
 r.top=500; r.bottom=499

 scan "Macro1376.bmp" w1 r 0x3
 scan "Macro1376.bmp" w1 r 0x3|16
 scan "Macro1376.bmp" w1 r 0x3|0x100
scan "Macro1376.bmp" w1 r 0x3|0x100|16

zRECT r
1; mou
