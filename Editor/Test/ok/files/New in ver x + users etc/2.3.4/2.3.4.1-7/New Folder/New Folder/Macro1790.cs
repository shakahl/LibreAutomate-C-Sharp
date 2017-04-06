out
WindowText x
int w=id(15 win("Notepad" "Notepad")) ;;get handle of Notepad edit control

 if text "findme" exists...
if x.Capture(w 0 "findme")
	out "text ''findme'' found"

 click text "findme"
x.Mouse(1 x.Capture(w 0 "findme"))

 show rectangle of text "findme"
WTI* t=x.Capture(w 0 "findme" 0x1000)
RECT r=t.rv; MapWindowPoints t.hwnd 0 +&r 2 ;;from t.hwnd client to screen
OnScreenRect 1 &r; 1; OnScreenRect 2 &r

 get all items
x.Capture(w)
int i
for i 0 x.n
	out x.a[i].txt
