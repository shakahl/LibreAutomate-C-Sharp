dll msvcrt #_utime $filename !*times
Dir d; DATE da.getclock; str sPath
foreach(d "E:\MyProjects\app\*" FE_Dir 0x0 da)
	sPath=d.FileName(1)
	out sPath
	_utime sPath 0