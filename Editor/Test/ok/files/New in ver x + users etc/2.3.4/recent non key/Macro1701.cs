str imgfile="$my qm$\item1.bmp"

str sf
if(!sf.searchpath(imgfile)) end "file not found"

__GdiHandle hbitmap=LoadImageW(0 @sf IMAGE_BITMAP 0 0 LR_LOADFROMFILE)
if(!hbitmap) end _s.dllerror("LoadImageW error:")

 int ok=scan("$common documents$\My QM Share\QM BMPs\Sample.bmp" 0 DRSBox_MonB 4)
 err
	 out _s.dllerror("dllerror:")
	 out _error.description
	 ret
