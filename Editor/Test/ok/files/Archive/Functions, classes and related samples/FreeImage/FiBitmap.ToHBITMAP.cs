function#

 Creates Windows bitmap from FIBITMAP of this variable.
 Returns handle. Will need to delete it.
 Error if failed.


int dc=GetDC(0)
int R=CreateDIBitmap(dc FIMG.FreeImage_GetInfoHeader(b) CBM_INIT FIMG.FreeImage_GetBits(b) FIMG.FreeImage_GetInfo(b) DIB_RGB_COLORS)
ReleaseDC(0 dc)

if(!R) end ERR_FAILED
ret R

 info: from FreeImage FAQ
