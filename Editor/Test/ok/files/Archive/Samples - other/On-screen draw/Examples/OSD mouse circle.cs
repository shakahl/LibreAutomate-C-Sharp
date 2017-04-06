 As you move the mouse, draws circle around mouse pointer. Click to stop.

int rad=32 ;;change this
 _________________________

POINT p pp
rep
	ifk((1)) break
	0.01
	xm p
	if(!memcmp(&pp &p sizeof(POINT))) continue
	pp=p
	int h=OnScreenDraw(p.x-rad p.y-rad rad*2 rad*2 &OSD_ProcExample3 0 80 1 0 h)
