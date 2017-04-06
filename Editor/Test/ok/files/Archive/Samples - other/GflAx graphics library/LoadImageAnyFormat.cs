 /
function# $_file

 Loads image of bmp, gif, jpg or other supported type.
 Returns bitmap handle. Later you must free it with CloseHandle.

 Note: you can use LoadPictureFile (QM 2.1.8.4) instead.


typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0

str s.searchpath(_file)
if(!s.len) end "file does not exist"

if(s.endi(".bmp")) ret LoadImage(0 s IMAGE_BITMAP 0 0 LR_LOADFROMFILE)

GflAx.GflAx g._create
g.EnableLZW=TRUE ;;needed for gif 
g.LoadBitmap(s)
IPicture p=g.GetPicture
int h; p.get_Handle(h)
ret CopyImage(h IMAGE_BITMAP g.width g.height 0)
