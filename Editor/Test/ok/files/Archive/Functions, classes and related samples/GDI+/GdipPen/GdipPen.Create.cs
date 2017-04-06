function'GDIP.GpPen* [color] [^width] ;;color: 0xAARRGGBB or ColorARGB(red green blue alpha)

 Creates this pen.


if(!GdipInit) ret
Delete

if(!width) width=1

_hresult=GDIP.GdipCreatePen1(color width GDIP.UnitWorld &m_p)
ret m_p
