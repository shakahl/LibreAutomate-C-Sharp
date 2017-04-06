function'GDIP.GpBrush* $imageFile [wrapMode] ;;wrapMode one of GDIP.WrapModeX constants, default WrapModeTile

 Creates this brush as texture brush.


if(!GdipInit) ret
Delete

GdipImage im
if(!im.FromFile(imageFile)) ret

ret CreateTextureFromImage(im wrapMode)
