 /
function# __MemBmp&mb [POINT*hotspot]

 Gets memory bitmap of the current cursor (mouse pointer).
 The transparent parts will be white.
 Also can optionally get hotspot position within the cursor.
 Returns cursor handle on success, 0 on failure.


CURSORINFO ci.cbSize=sizeof(CURSORINFO)
if(!GetCursorInfo(&ci)) ret

if(hotspot)
	ICONINFO ii
	if(!GetIconInfo(ci.hCursor &ii)) ret
	hotspot.x=ii.xHotspot; hotspot.y=ii.yHotspot

mb.Create(32 32)
RECT r.right=32; r.bottom=32; FillRect mb.dc &r GetStockObject(WHITE_BRUSH)
if(!DrawIconEx(mb.dc 0 0 ci.hCursor 32 32 0 0 DI_NORMAL)) ret
ret ci.hCursor
