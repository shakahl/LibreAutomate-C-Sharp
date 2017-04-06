 /
function# [$url]

 Waits while Chrome is busy (loading page).
 Returns window handle.
 Error if fails, eg if Chrome closed while waiting.
 Tested with Google Chrome 7.0, 21.0, 41.0, 43.0. May not work with other versions.

 url - web page address to open.
   If empty, just waits while Chrome is busy.
   Can include command line, eg "--disable-extensions ''url''".

 May not wait if this function opens new window when a window already exists.
 Does not work if toolbar or its Reload button does not exist.

 To get "busy" state, uses accessible object properties of main toolbar and its Reload button.
 If your Chrome uses language other than English, the default values may not match. Then the function fails (error). Open this function and change variables. You can discover the values using the 'Find Accessible Object' dialog.

 With Chrome 43:
   Waits for pixel color 0x505050 in Reload button, at x=0.5 y=0.3 of the button Acc object rectangle. May need to change the color or coordinate.
   Activates Chrome window, because the Reload button must be visible on screen.


 variables to change
str toolbarName="Chrome|^main$" ;;regexp. In Chrome 39 it contains "Chrome"; in Chrome 41 it is "main".
str buttonName="Reload" ;;can be partial
str buttonDescriptionBeginsWith="Reload" ;;begins with

 ____________________________________________

int w=WB_Open("chrome" url "Chrome" "Chrome_WidgetWin_*" 0x804)

 get toolbar acc
Acc a=acc(toolbarName "TOOLBAR" w "" "" 0x1012 0 0 "" 30)
 get Reload button acc
a=acc(buttonName "PUSHBUTTON" a "" "" 16)

_s=a.Description
if _s.len
	 wait until description changes from "Stop*" to "Reload*"
	int n
	rep
		0.1
		_s=a.Description
		if(_s.begi(buttonDescriptionBeginsWith)) n+1; if(n=10) break
		else n=0
else ;;Chrome 43
	 get a 1-pixel rectangle in the circle-arrow ("Reload" image)
	RECT r rr
	int x y wid hei
	a.Location(x y wid hei)
	r.left=x+(wid/2); r.top=y+(hei*0.3) ;;may need to change. It must be a pixel from "Reload" image (circle-arrow), not from "Stop" image (X) and not from background.
	DpiScreenToClient w +&r
	r.right=r.left+1; r.bottom=r.top+1
	 out "%X" pixel(r.left r.top w 1) ;;show the color
	 mou r.left r.top w 1; ret ;;show the place
	
	 wait for color
	rep 8 ;;retry because button image can change several times
		0.1
		act w
		rr=r
		wait 0 S "color:0x505050" w rr 16 32 ;;may need to change color here. The 32 is allowed color difference.

ret w
err+ end ES_FAILED
