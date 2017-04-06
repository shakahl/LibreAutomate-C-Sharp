out
Dir d
foreach(d "$My QM$\test\*" FE_Dir 2|4)
 foreach(d "$My QM$\test\*" FE_Dir 2|4 "2013.01.01" "2013.10.01")
 foreach(d "*" FE_Dir 2|4|32)
	str path=d.FullPath
	out d.FileName
	out d.RelativePath
	out path
	 out d.Level
	 d.SkipChildren
	if d.FileAttributes&FILE_ATTRIBUTE_REPARSE_POINT
		outx d.fd.dwReserved0
