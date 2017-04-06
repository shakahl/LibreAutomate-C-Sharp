out
lpstr s=dir("$My QM$\test\*" 2)
 lpstr s=dir("*" 2)
rep
	if(!s) break
	out s
	s=dir
