 /
function# int&x int&y [int&width] [int&height] [flags] ;;flags: 1 relative to container's client area.

 Gets caret (text cursor) position.
 Returns handle of child window that contains it.
 Works not in all windows. If fails, returns 0.

 x, y - variables that receive caret position.
 width, height - variables that receive caret width and height. Can be 0.


GUITHREADINFO g.cbSize=sizeof(GUITHREADINFO)
if(!GetGUIThreadInfo(0 &g)) ret
MapWindowPoints g.hwndCaret 0 +&g.rcCaret 2
if(_winnt>=6)
	if(!LogicalToPhysicalPoint(g.hwndCaret +&g.rcCaret)) out "fail 2"
	if(!LogicalToPhysicalPoint(g.hwndCaret +&g.rcCaret.right)) out "fail 2"
if(flags&1) MapWindowPoints 0 g.hwndCaret +&g.rcCaret 2
x=g.rcCaret.left
y=g.rcCaret.top
if(&width) width=g.rcCaret.right-g.rcCaret.left
if(&height) height=g.rcCaret.bottom-g.rcCaret.top
ret g.hwndCaret
