out
Dir d
foreach(d "$desktop$\*" FE_Dir2 4|2|32)
	str path=d.FileName(1)
	out path
