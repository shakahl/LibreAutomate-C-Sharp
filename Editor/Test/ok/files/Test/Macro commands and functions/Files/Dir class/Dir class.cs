ClearOutput
Dir d
 out d.dir("$qm$\*")
 out d.FileAttributes
 out d.dir("$qm$\*" 4)
 out d.dir("." 2|4)

lpstr s=d.dir("..\*" 2|4)
rep
	if(!s) break
	out s
	s=d.dir
