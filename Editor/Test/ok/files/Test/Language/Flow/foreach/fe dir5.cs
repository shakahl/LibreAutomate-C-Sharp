ClearOutput

int i t=GetTickCount

Dir d
foreach d "$qm$\*.html" FE_Dir 8
 foreach d "$qm$\*.txt" FE_Dir 0
 foreach d "e:\*" FE_Dir
 foreach d "*" FE_Dir
	 out d.path
	 d.FileName()
	 out d.FileName()
	out d.FileName(1)
	i+1

out GetTickCount-t
 out i
