 /
function'IPicture hBitmap

 Creates IPicture (OLE picture object) from Windows bitmap handle.


IPicture ip
PICTDESC pd.cbSizeofstruct=sizeof(pd)
pd.picType=PICTYPE_BITMAP
pd.bmp.hbitmap=hBitmap

OleCreatePictureIndirect(&pd uuidof(IPicture) 0 &ip)
ret ip
