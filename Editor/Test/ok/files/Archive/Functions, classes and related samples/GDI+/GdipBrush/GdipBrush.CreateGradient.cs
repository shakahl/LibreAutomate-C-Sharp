function'GDIP.GpBrush* x1 y1 x2 y2 color1 color2 [wrapMode] ;;colors: 0xAARRGGBB or ColorARGB(red green blue alpha).  wrapMode one of GDIP.WrapModeX constants.

 Creates this brush as linear gradient brush.

 x1 y1 - starting point.
 x2 y2 - ending point.
 color1 color2 - colors at starting and ending points.


if(!GdipInit) ret
Delete

GDIP.Point p1 p2
p1.X=x1; p1.Y=y1; p2.X=x2; p2.Y=y2

_hresult=GDIP.GdipCreateLineBrushI(&p1 &p2 color1 color2 wrapMode +&m_b)
ret m_b
