 /
function# cursor ;;cursor: one of IDC_x constants

 Returns 1 if cursor matches the current mouse cursor (pointer), or 0 if not.

 cursor - one of system cursor constants:
	 IDC_APPSTARTING - Standard arrow and small hourglass
	 IDC_ARROW - Standard arrow
	 IDC_CROSS - Crosshair
	 IDC_HAND - Hand
	 IDC_HELP - Arrow and question mark
	 IDC_IBEAM - I-beam
	 IDC_NO - Slashed circle
	 IDC_SIZEALL - Four-pointed arrow pointing north, south, east, and west
	 IDC_SIZENESW - Double-pointed arrow pointing northeast and southwest
	 IDC_SIZENS - Double-pointed arrow pointing north and south
	 IDC_SIZENWSE - Double-pointed arrow pointing northwest and southeast
	 IDC_SIZEWE - Double-pointed arrow pointing west and east
	 IDC_UPARROW - Vertical arrow
	 IDC_WAIT - Hourglass

 EXAMPLE
 if(IsMouseCursor(IDC_ARROW))
	 out "arrow"
 else if(IsMouseCursor(IDC_IBEAM))
	 out "I beam"
 else out "other"


cursor=LoadCursor(0 +(cursor&0xffff)); if(!cursor) ret

CURSORINFO ci.cbSize=sizeof(CURSORINFO)
if(!GetCursorInfo(&ci)) ret
ret cursor=ci.hCursor
