 /
function# hwnd

 note: don't need this func, use GetAncestor(hwnd 3). Normally for windows owned by desktop GA returns hwnd, except if the window has ws_child style, eg ComboLBox.

 Gets windows's root owner window.
 Returns owner's handle. If hwnd is not owned, returns hwnd.

 REMARKS
 A window's direct owner window itself may be owned. Unlike GetWindow(hwnd GW_OWNER), which gets the direct owner window, this function gets the ancestor owner window that is not owned.


int w=GetAncestor(hwnd 3)
if w!hwnd
	if(!w or w=GetDesktopWindow) w=hwnd
ret w
