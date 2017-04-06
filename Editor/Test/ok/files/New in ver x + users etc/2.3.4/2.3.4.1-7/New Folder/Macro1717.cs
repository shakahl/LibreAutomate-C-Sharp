__GdiHandle hb=LoadPictureFile("$documents$\foto\kamane.JPG")
if(!hb) end "failed"
str sTempFile="$temp$\qm_bitmap.bmp"
if(!SaveBitmap(hb sTempFile)) end "failed"

act "Paint"
sTempFile.setsel(CF_BITMAP)
