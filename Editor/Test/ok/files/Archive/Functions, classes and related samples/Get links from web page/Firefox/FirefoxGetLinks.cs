 /
function hwnd [ARRAY(str)&aURL] [ARRAY(str)&aText] [ARRAY(Acc)&aObj]

 Gets links in Firefox.

 hwnd - Firefox window handle.
 aURL - receives href attribute of links. It is full URL, not relative as in HTML source. Can be 0 if you dont need it.
 aText - receives text of links. Can be 0 if you dont need it.
 aObj - receives accessible object of links. Can be 0 if you dont need it.

 EXAMPLE
 out
 int w=win("Firefox" "Mozilla*WindowClass" "" 0x804)
 ARRAY(str) a at; int i
 FirefoxGetLinks w a at
 for i 0 a.len
	 out F"{at[i]%%-35s} {a[i]}"


if(&aURL) aURL=0
if(&aText) aText=0
if(&aObj) aObj=0

FFNode f.FindFF(hwnd "A" "" "" 0 0 0 &__FGL_Proc &hwnd)

err+ end _error
