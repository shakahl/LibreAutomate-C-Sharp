 /
function hwnd hdc cx cy OSDGRID&g

int hpen oldpen

hpen=CreatePen(0 g.lineThickness g.lineColor); oldpen=SelectObject(hdc hpen)

ARRAY(int) ai
ARRAY(POINT) a
int i k

 horz lines

ai.create(cy/g.gridSize)
a.create(ai.len*2)
k=0
for i 0 a.len
	ai[i/2]=2
	k+g.gridSize
	a[i].y=k
	i+1
	a[i].y=k
	a[i].x=cx

PolyPolyline(hdc &a[0] &ai[0] ai.len)

 vert lines

ai.create(cx/g.gridSize)
a.create(ai.len*2)
k=0
for i 0 a.len
	ai[i/2]=2
	k+g.gridSize
	a[i].x=k
	i+1
	a[i].x=k
	a[i].y=cy

PolyPolyline(hdc &a[0] &ai[0] ai.len)

DeleteObject SelectObject(hdc oldpen)
