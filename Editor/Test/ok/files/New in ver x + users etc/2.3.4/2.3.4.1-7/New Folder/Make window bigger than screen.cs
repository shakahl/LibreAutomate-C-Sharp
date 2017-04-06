int w=win("Notepad" "Notepad")
int st=GetWinStyle(w); SetWinStyle w st|WS_POPUP~WS_THICKFRAME
siz 5000 3000 w
int cx cy; GetWinXY w 0 0 cx cy; out F"{cx} {cy}"
10
siz 500 300 w
SetWinStyle w st
