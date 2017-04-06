int w=win
if(!w or !wintest(w "Paint" "MSPaintApp")) ret
if(!scan("macro:mspaint pencil pressed.bmp" child("" "NetUIHWND" w 0x1) 0 16 10)) ret

OnScreenDisplay "macro"
