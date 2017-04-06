function'GDIP.GpGraphics* hWnd

 Creates this graphics and sets to draw in the window.


if(!GdipInit) ret
Delete

_hresult=GDIP.GdipCreateFromHWND(hWnd &m_g)
SetDefProp
ret m_g
