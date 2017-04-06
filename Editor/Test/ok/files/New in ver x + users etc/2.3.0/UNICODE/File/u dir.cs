out
Dir d
foreach(d "$qm$\*" FE_Dir)
 foreach(d "*" FE_Dir)
 foreach(d "\*" FE_Dir)
	str sPath=d.FileName(1)
	out sPath
	
