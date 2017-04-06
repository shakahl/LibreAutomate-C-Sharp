int w=win("Registry Editor" "RegEdit_RegEdit")
 w=id(2 w) ;;list
 act w
 0.5

PerfFirst
 _i=pixel(100 100 w 1|2)
_i=pixel(100 100 w 1|2|0x1000)
 _i=FastPixel(100 100 w 1)
 RECT r; SetRect &r 100 100 101 101; _i=scan("color:0xB9FF" w r 0x1010)
PerfNext;PerfOut
out F"0x{_i}"
 mou 100 100 w 1
