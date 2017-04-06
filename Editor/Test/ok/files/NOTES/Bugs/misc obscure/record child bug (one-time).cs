 Bug: garbage. Also, whole recording is strange (act, lef+, lef- etc are not where should be).
 Next time recorded OK, and played reliably.


int w1=act(win("PagrindinÄ— paieÅ¡ka - PaÅ¾intys - Draugas.lt - Mozilla Firefox" "MozillaUIWindowClass"))
lef 1220 509
act w1
lef- 814 425 child("" "MozillaWindowClass" w1 0 0 0 5)
lef+ 814 425 child("" "MozillaWindowClass" ÿÿw1) 0 0 0 5)
lef- 973 397 child("" "MozillaWindowClass" $ 0 0 0 5)
lef+ 973 397 child("" "MozillaWindowClass" ÿÿw1) 0 0 0 5)
int w2=win("" "MozillaWindowClass")
lef+ 140 41 w2
lef- 139 103 w2
lef 75 146 w2
act w1
lef 972 430 child("" "MozillaWindowClass" w1 0 0 0 5)
int w3=win("" "MozillaWindowClass")
lef+ 138 53 w3
lef- 137 152 w3
lef 75 154 w3
act w1
lef 970 517 child("" "MozillaWindowClass" w1 0 0 0 5)
lef 84 26 win("" "MozillaWindowClass")
