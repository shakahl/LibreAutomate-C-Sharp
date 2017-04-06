 Trigger: #M 0x8

ClearOutput

int t0=GetTickCount
ARRAY(POINT) a
POINT p1 p2
int i dx dy
for i 0 1000
	GetCursorPos(&p2)
	 if(i and p2.x=p1.x and p2.y=p1.y) goto g1
	if(i) dx=p2.x-p1.x; dy=p2.y-p1.y; dx=iif(dx>=0 dx -dx); dy=iif(dy>=0 dy -dy); if(dx<3 and dy<3) goto g1
	a[a.redim(-1)]=p2
	p1=p2
	 g1
	ifk-((4)) break
	0.01

if(a.len>1)
	for i 0 a.len
		POINT& p=a[i]
		out "%i %i" p.x p.y
	ret

 cancel
mid
