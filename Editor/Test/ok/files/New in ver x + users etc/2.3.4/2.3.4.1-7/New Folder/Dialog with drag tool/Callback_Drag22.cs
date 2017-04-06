 /
function# button param

 Callback function for Drag.

 button - mouse button: 0 while dragging, 1 left up, 2 right up.
 param - param passed to Drag().

 Return:
   If button 0 (mouse move), return cursor handle, or 0 to not change cursor, or 1-3 to use standard cursors: 1 move, 2 copy, 3 no operation.
   Else (mouse button up), can return any value. Drag() returns it.


sel button
	case 0
	Acc a.FromMouse; err ret
	RECT r; a.Location(r.left r.top r.right r.bottom); err ret
	r.right+r.left; r.bottom+r.top
	OnScreenRect 0 &r
	ret LoadCursor(0 +IDC_CROSS)
	
	case else
	OnScreenRect 2 &r
	ret button
