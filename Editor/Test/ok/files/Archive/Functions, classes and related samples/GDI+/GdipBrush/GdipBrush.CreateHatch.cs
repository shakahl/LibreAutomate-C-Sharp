function'GDIP.GpBrush* hatchStyle foreColor [backColor] ;;hatchStyle: one of GDIP.HatchStyleX constants.  colors: 0xAARRGGBB or ColorARGB(red green blue alpha)

 Creates this brush as hatch brush.


if(!GdipInit) ret
Delete

_hresult=GDIP.GdipCreateHatchBrush(hatchStyle foreColor backColor +&m_b)
ret m_b
