out
Dir d
 out d.dir("$Desktop$\*.exe")
Q &q
foreach(d "$Desktop$\*.exe" FE_Dir 0x4)
	str sPath=d.FileName(1)
	 out sPath
	 out d.dir
	 break
Q &qq; outq
