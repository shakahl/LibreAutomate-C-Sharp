 Declares GdipX classes.

#opt hidedecl 1
class GdipGraphics -GDIP.GpGraphics*m_g
class GdipImage -GDIP.GpImage*m_i
class GdipBitmap :GdipImage'__ ;;the native GDI+ object type actually is GpBitmap
class GdipPen -GDIP.GpPen*m_p
class GdipBrush -GDIP.GpBrush*m_b
class GdipFont -GDIP.GpFont*m_f
