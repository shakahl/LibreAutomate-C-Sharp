out
Dir d
lpstr s=d.dir("$My QM$\test\*" 2|4)
 lpstr s=d.dir("*" 2|4)
rep
	if(!s) break
	out s
	 out d.FileName
	s=d.dir
