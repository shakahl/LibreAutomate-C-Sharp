function'GDIP.GpBrush* color ;;color: 0xAARRGGBB or ColorARGB(red green blue alpha)

 Creates this brush as solid brush.


if(!GdipInit) ret
Delete

_hresult=GDIP.GdipCreateSolidFill(color +&m_b)
ret m_b
