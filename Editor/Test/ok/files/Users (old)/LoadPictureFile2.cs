 /
function# $_file

 Loads image file of bmp, gif or jpg type.
 Returns bitmap handle. Later you must free it with DeleteObject.

 obsolete


dll oleaut32 #OleLoadPictureFile VARIANT'varFileName IDispatch*lplpdispPicture
dll user32 #CopyImage handle un1 n1 n2 un2

str s.searchpath(_file)
if(!s.len) ret

IDispatch disp
if(OleLoadPictureFile(s &disp)) ret
stdole.IPicture pict=disp
ret CopyImage(pict.Handle IMAGE_BITMAP 0 0 0)
err+
