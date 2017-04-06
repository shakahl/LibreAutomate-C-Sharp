 This function runs when you press Scroll Lock.
 It works only when Scroll Lock is toggled on.


if(getopt(nthreads)>1) ret

rep
	0.5
	
	 is Scroll Lock is on?
	ifk-(J 1) ret
	
	 get focused control and caret pos in it
	POINT p
	int h=GetCaretXY(p.x p.y 0 0 1)
	if(!h) continue
	
	 get h client rect
	RECT r
	GetClientRect h &r
	if(r.bottom<150) continue
	
	 is cursor near bottom?
	if(p.y<r.bottom-100) continue
	
	 is Ctrl etc pressed?
	if(GetMod) continue
	
	 is mouse button pressed?
	ifk((1)) continue
	ifk((2)) continue
	ifk((3)) continue
	
	 scroll 1 tick down
	MouseWheel -1 h
	 out "scrolled"

 z
 z
 z
 z
 z
 z
 z
 z
 z
 z
 z
 z
 z
 z
 z
 z
 z
 z
