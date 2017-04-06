 /
function# x y [flags] ;;flags: 1 x y are relative to the hot spot

 Returns a color in the current cursor (mouse pointer).
 Returns -1 on failure.

 x, y - pixel coordinates relative to the top-left corner of the cursor. If flag 1 used - relative to the hot spot (which is at the mouse position) of the cursor.

 EXAMPLE
 rep
	 1
	 out "0x%X" GetColorInCursor(0 0 1) ;;color at the hot spot (mouse position)


__MemBmp mb; POINT p
if(!GetCursorBitmap(mb &p)) ret CLR_INVALID
if(flags&1) x+p.x; y+p.y
ret GetPixel(mb.dc x y)
