 /dlg_apihook
function# GDIP.GpGraphics*graphics @*string length GDIP.GpFont*font GDIP.RectF*layoutRect GDIP.GpStringFormat*stringFormat GDIP.GpBrush*brush

int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
int R=call(fnGdipDrawString graphics string length font layoutRect stringFormat brush)
t_inAPI-1; if(inAPI and !t_all) ret R

 ret R
if(R) ret R

RECT r
r.left=layoutRect.X; r.top=layoutRect.Y; r.right=layoutRect.X+layoutRect.Width; r.bottom=layoutRect.Y+layoutRect.Height

int hdc; GDIP.GdipGetDC(graphics &hdc)

CommonTextFunc 5 hdc string length r

GDIP.GdipReleaseDC(graphics hdc)

ret R
