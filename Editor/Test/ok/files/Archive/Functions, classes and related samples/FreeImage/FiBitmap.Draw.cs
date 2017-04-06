function hDC RECT&r

 Draws image into a Windows device context (in a dialog etc).

SetStretchBltMode(hDC COLORONCOLOR)
StretchDIBits(hDC r.left r.top r.right-r.left r.bottom-r.top 0 0 FIMG.FreeImage_GetWidth(b) FIMG.FreeImage_GetHeight(b) FIMG.FreeImage_GetBits(b) FIMG.FreeImage_GetInfo(b) DIB_RGB_COLORS SRCCOPY)

 info: from FreeImage FAQ
