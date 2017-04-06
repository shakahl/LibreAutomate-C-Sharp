int w=win
if(!wintest(w "Paint" "MSPaintApp")) ret
Acc a.Find(w "PUSHBUTTON" "Pencil" "class=NetUIHWND" 0x1005); err end "Pencil button not found" 8; ret
if(a.State&STATE_SYSTEM_PRESSED=0) ret

OnScreenDisplay "macro"
