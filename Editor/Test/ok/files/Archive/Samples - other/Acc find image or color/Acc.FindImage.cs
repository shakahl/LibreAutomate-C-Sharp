function# $bmpFile [RECT&foundLocation] [scanFlags] [handle] [RECT&margins]

 Finds image within the accessible object.
 Uses <help "::/Functions/IDP_SCAN.html">scan</help>.
 Returns 1 if found, 0 if not found, or throws error thrown by <color "0xff0000">scan</color> or Acc.Location.
 To capture the image and get values for other arguments you can use the 'Find Image' dialog from the floating toolbar.

 bmpFile - bitmap or icon file that contains image to find. Same as with <color "0xff0000">scan</color>, except that handle cannot be used.
 foundLocation - optional RECT variable that will receive screen coordinates of the image. Use 0 if not needed. Note that it is not used to limit the search area. It only receives results.
 scanFlags - same as flags argument of <color "0xff0000">scan</color>.
 handle - bitmap or icon handle to be used instead of bmpFile.
 margins - optional RECT variable that contains search area margins. For example, if margins.left is 20, the function will not search in first 20 pixels in the left of the object. Or, if margins.left is -30, the function also will search in 30 pixels to the left from the object.

 EXAMPLE
 Acc a=acc("Notification Area" "TOOLBAR" win("" "Shell_TrayWnd") "ToolbarWindow32" "" 0x1001)
 if(!a.FindImage("Macro517.bmp" 0 0x3)) ret
 out "found"


RECT r
this.Location(r.left r.top r.right r.bottom)
r.right+r.left; r.bottom+r.top
if(&margins)
	r.left+margins.left; r.top+margins.top; r.right-margins.right; r.bottom-margins.bottom
	if(r.left>=r.right or r.top>=r.bottom) ret

int ok
if(handle) ok=scan(handle 0 r scanFlags)
else ok=scan(bmpFile 0 r scanFlags)
if(!ok) ret
if(&foundLocation) foundLocation=r
ret 1

err+ end _error
