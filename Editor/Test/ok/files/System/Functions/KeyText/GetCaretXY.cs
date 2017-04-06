 /
function# int&x int&y [int&width] [int&height] [flags] ;;flags: 1 relative to container's client area.

 Gets caret (text cursor) position.
 Returns handle of child window that contains it.
 Works not in all windows. If fails, returns 0.

 x, y - variables that receive caret position.
 width, height - variables that receive caret width and height. Can be 0.

 EXAMPLE
 int x y
 GetCaretXY x y
 out F"{x} {y}"


GUITHREADINFO g.cbSize=sizeof(GUITHREADINFO)
if(!GetGUIThreadInfo(0 &g)) ret
int h=g.hwndCaret; if(!h) ret
RECT& r=g.rcCaret
if(_winver<0x603 && DpiIsWindowScaled(h)) DpiScale +&r 2
if(&width) width=r.right-r.left
if(&height) height=r.bottom-r.top
if(flags&1=0) DpiClientToScreen h +&r
x=r.left
y=r.top
ret h
