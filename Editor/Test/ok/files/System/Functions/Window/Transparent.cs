 /
function# hwnd opacity [color] ;;opacity: 0-255

 Makes window transparent.
 Returns 1 if the window was transparent previously, 0 if not.

 hwnd - window handle. Cannot be child window.
 opacity - a value from 0 (completely transparent) to 255 (completely opaque). If 256 or more, removes transparency. If negative - toggles transparency.
 color - if used, this color will be 100% transparent.
   100% transparent parts are also transparent to mouse messages.
   Use opacity 255 to apply only color.

 EXAMPLE
 Transparent(win("Notepad") 128)


int e(GetWindowLong(hwnd GWL_EXSTYLE)) t(e&WS_EX_LAYERED!=0)
if(t and (opacity>255 or opacity<0))
	SetLayeredWindowAttributes(hwnd 0 255 2)
	SetWindowLong(hwnd GWL_EXSTYLE e~WS_EX_LAYERED)
else if(opacity<=255)
	if(!t) SetWindowLong(hwnd GWL_EXSTYLE e|WS_EX_LAYERED)
	if(opacity<0) opacity=-opacity
	int fl
	if(opacity<255) fl|2
	if(getopt(nargs)>2) fl|1
	SetLayeredWindowAttributes(hwnd color opacity fl)
ret t
