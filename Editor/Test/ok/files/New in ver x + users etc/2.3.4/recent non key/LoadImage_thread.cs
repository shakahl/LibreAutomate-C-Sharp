rep 10000
	str imgfile="$my qm$\item1.bmp"
	
	str sf
	if(!sf.searchpath(imgfile)) end "file not found"
	
	__GdiHandle hbitmap=LoadImageW(0 @sf IMAGE_BITMAP 0 0 LR_LOADFROMFILE)
	if(!hbitmap) end _s.dllerror("LoadImageW error:")
	 0.001
	hbitmap.Delete
