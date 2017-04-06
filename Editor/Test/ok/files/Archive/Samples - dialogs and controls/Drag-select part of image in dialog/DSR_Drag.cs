 /dialog_select_rectangle
function# button DSR2387&d

 Callback function for Drag.

 button - mouse button: 0 while dragging, 1 left up, 2 right up.
 param - param passed to Drag().

 Return:
   If button 0 (mouse move), return cursor handle, or 0 to not change cursor, or 1-3 to use standard cursors: 1 move, 2 copy, 3 no operation, 4,5 (QM 2.3.4) red and blue cross.
   Else (mouse button up), can return any value. Drag() returns it.


if(!d.notFirstTime) d.notFirstTime=1
else if(!IsRectEmpty(&d.r)) DrawFocusRect d.hdc &d.r ;;restore previous rect

sel button
	case 0
	POINT p; xm p d.hwnd 1; memcpy &d.r.right &p 8
	if(!IsRectEmpty(&d.r)) DrawFocusRect d.hdc &d.r ;;draw new rect
	ret 4
	
	case else
	ret button
