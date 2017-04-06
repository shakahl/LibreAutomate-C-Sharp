str s
Dir d
foreach(d "$System$\*.dll" FE_Dir)
	str sPath=d.FileName(1)
	if(ExtractIconEx(sPath 0 0 0 5)>4) s.addline(sPath)
out s
