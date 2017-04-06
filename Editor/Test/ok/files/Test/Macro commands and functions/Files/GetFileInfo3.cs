lpstr s=GetFileInfo("*" 0 0 &_i)
rep
	if(!s) break
	out "%s  %i" s _i
	s=GetFileInfo("" 3 0 &_i)
