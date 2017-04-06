act "Word"
spe 10
Dir d
foreach(d "$My QM$\*.txt" FE_Dir)
	str path=d.FullPath
	out path
	str data.getfile(d.FullPath);; err ...
	
	key Cb (d.FileName) Y Cb
	paste data
	key CY
