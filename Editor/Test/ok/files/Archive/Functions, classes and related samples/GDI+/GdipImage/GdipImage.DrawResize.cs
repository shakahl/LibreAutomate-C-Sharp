function! hDc x y _width _height [quality] ;;quality: 0 default, 1 low, 2 high, 3 bilinear, 4 bicubic, 5 nearest neighbor, 6 high bilinear, 7 high bicubic

 Draws this image in the device context and resizes if need.
 Returns 1 on success, 0 if failed.

 If _height or _width is 0, calculates it to preserve aspect ratio. If both 0, uses original size.


if(!_width and !_height) ret Draw(hDc x y)

GdipGraphics g
if(!g.FromHDC(hDc)) ret

if(!_height) _height=MulDiv(this.height _width this.width)
if(!_width) _width=MulDiv(this.width _height this.height)

if(quality) GDIP.GdipSetInterpolationMode(g quality)

_hresult=GDIP.GdipDrawImageRectI(g m_i x y _width _height)
ret !_hresult
