 As you move the mouse, draws red line from initial to current mouse position, until you click or release left mouse button.

int color=ColorFromRGB(255 0 0)
POINT p1 p2 pp
xm p1; pp=p1
int h m
rep
	ifk((1)) m=1; else if(m) break
	0.01
	xm p2
	if(!memcmp(&pp &p2 sizeof(POINT))) continue
	h=OnScreenLine(p1.x p1.y p2.x p2.y color 4 h)
	pp=p2
