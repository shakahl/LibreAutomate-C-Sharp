out
Dir d
foreach d "Q:\My QM\test\*" FE_Dir 2|4
 foreach d "Q:\My QM\test\*.qml" FE_Dir 2|4
 foreach d "$My QM$\test\*.qml" FE_Dir 2|4
 foreach d "Q:\My QM\test\export.qmL" FE_Dir 4
 foreach d "*.qml" FE_Dir
 foreach d "Q:\My QM\..\*" FE_Dir
 foreach d "" FE_Dir
	 out d.FullPath
	out d.FileName(3)

 out d.dir("Q:\My QM\test\*")
 out d.dir()
 lpstr s=d.dir("Q:\My QM\test\*" 2|4)
 rep
	 if(!s) break
	 out s
	 s=d.dir
	 

