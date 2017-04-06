 /dlg_apihook
function# GDIP.GpGraphics*graphics @*text length GDIP.GpFont*font GDIP.GpBrush*brush GDIP.PointF*positions flags GDIP.Matrix*matrix

int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
int R=call(fnGdipDrawDriverString graphics text length font brush positions flags matrix)
t_inAPI-1; if(inAPI and !t_all) ret R

 ret R
if(R or !(flags&GDIP.DriverStringOptionsCmapLookup)) ret R

RECT r
int i
for i 0 length
	GDIP.PointF& p=positions[i]
	if i=0
		r.left=p.X; r.top=p.Y; r.right=r.left; r.bottom=r.top
	else
		if(p.X<r.left) r.left=p.X
		if(p.X>r.right) r.right=p.X
		if(p.Y<r.top) r.top=p.Y
		if(p.Y>r.bottom) r.bottom=p.Y
r.right+12; r.bottom+12

int hdc; GDIP.GdipGetDC(graphics &hdc)

CommonTextFunc 6 hdc text length r

GDIP.GdipReleaseDC(graphics hdc)

ret R
