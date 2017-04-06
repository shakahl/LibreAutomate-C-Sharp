function! hDc [x] [y]

 Draws this image in the device context.
 Returns 1 on success, 0 if failed.


GdipGraphics g
if(!g.FromHDC(hDc)) ret
_hresult=GDIP.GdipDrawImageI(g m_i x y)
ret !_hresult
