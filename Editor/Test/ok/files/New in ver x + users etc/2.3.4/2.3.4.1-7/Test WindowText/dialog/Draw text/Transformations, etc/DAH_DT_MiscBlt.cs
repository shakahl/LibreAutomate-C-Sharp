 /dlg_apihook
function hdc CLogicalCoord&c

__MemBmp m.Create(100 100)
RECT r; SetRect &r 0 0 100 100; FillRect m.dc &r GetStockObject(WHITE_BRUSH)

ExtTextOutW(m.dc 20 20 0 0 L"Text" 4 0)

 BitBlt hdc 10 10 100 100 m.dc -50 0 SRCCOPY
StretchBlt hdc 100 100 150 300 m.dc 10 -10 100 100 SRCCOPY
 StretchBlt hdc 50 50 200 150 m.dc 0 0 100 100 SRCCOPY
 StretchBlt hdc 50 50 200 150 m.dc 10 10 100 100 SRCCOPY
 StretchBlt hdc 50 50 200 -150 m.dc 0 0 100 100 SRCCOPY

 BitBlt hdc 0 100 100 -100 m.dc 0 0 SRCCOPY
 StretchBlt hdc 0 100 100 -100 m.dc 0 0 100 100 SRCCOPY
 StretchBlt hdc 200 200 -100 -100 m.dc 20 20 100 100 SRCCOPY
 StretchBlt hdc 100 100 100 100 m.dc 0 0 -100 -100 SRCCOPY ;;no
