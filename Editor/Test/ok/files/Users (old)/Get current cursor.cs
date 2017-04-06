CURSORINFO ci.cbSize=sizeof(CURSORINFO)
if(!GetCursorInfo(&ci)) ret

if(ci.hCursor=LoadCursor(0 +IDC_WAIT)) out "wait"
