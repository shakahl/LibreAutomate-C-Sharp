function hBitmap

 Creates FIBITMAP from Windows bitmap and sets this variable to manage the FIBITMAP.
 Error if failed.


Delete

BITMAP bm
if(!GetObjectW(hBitmap sizeof(BITMAP) &bm)) end ERR_BADARG
b=FIMG.FreeImage_Allocate(bm.bmWidth bm.bmHeight bm.bmBitsPixel 0 0 0)
int nColors=FIMG.FreeImage_GetColorsUsed(b)
int dc=GetDC(0)
int R=GetDIBits(dc hBitmap 0 FIMG.FreeImage_GetHeight(b) FIMG.FreeImage_GetBits(b) FIMG.FreeImage_GetInfo(b) DIB_RGB_COLORS)
ReleaseDC(0 dc)
BITMAPINFOHEADER* bih=FIMG.FreeImage_GetInfoHeader(b)
bih.biClrUsed=nColors
bih.biClrImportant=nColors

if(!R) end ERR_FAILED

 info: from FreeImage FAQ
