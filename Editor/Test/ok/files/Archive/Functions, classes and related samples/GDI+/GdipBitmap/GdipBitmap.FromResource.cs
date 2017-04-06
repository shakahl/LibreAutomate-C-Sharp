function'GDIP.GpBitmap* resId [hInstance]

 Create this bitmap from a bitmap resource.
 Can be useful in exe.

 resId - bitmap resource id.
 hInstance - handle of module containing the resource. If 0, uses exe resource module handle.


if(!GdipInit) ret
Delete

if(!hInstance) hInstance=GetExeResHandle

_hresult=GDIP.GdipCreateBitmapFromResource(hInstance +resId +&m_i)
ret +m_i
