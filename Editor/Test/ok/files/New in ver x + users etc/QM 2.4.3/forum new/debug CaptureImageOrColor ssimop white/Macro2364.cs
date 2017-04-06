RECT r
str sout.from("$temp$\Image_" _s ".bmp")

__GdiHandle hBitmap
int i=CaptureImageOrColor_debug(hBitmap 0 0 sout r)
if !i
	out "Failed to capture"
	end

int q=OpenClipboard(_hwndqm)
if(q) EmptyClipboard; q=SetClipboardData(CF_BITMAP hBitmap)!0; CloseClipboard
if(q) hBitmap.handle=0

 _s.from("Image captured stored to clipboard - also stored to file " sout)
 OnScreenDisplay _s
