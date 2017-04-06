 Copy constant etc and go to definition in Visual Studio
dou
key Cc
int w1=act(win("Visual C" "wndclass_desked_gsk"))
1
int hwnd=child("" "RichEdit20W" w1 0x5)
int x y; GetWinXY hwnd x y
lef 5 5 hwnd
0.5
'Cv
'F12
 Wait with listbox
sel(list("Continue" "" "" -1 0))
	case 1
	case else ret
 When definition is selected, paste it in QM
key Cc
act _hwndqm
key CH Cv
