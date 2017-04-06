out
 1
Dir d
 foreach(d "$My QM$\*.*" FE_Dir)
 foreach(d "$pf$\*.*" FE_Dir 4)
 foreach(d "$pf$\*.bmp" FE_Dir 4)
 foreach(d "Q:\ico and bmp\bmp\q*.bmp" FE_Dir 4)
foreach(d "$pf$\*.png" FE_Dir 4)
 foreach(d "$pf$\*.jpg" FE_Dir 4)
 foreach(d "$pf$\*.gif" FE_Dir 4)
 foreach(d "$pf$\*.ico" FE_Dir 4)
 foreach(d "$windows$\Cursors\*.cur" FE_Dir 4)
 foreach(d "$windows$\Cursors\*.ani" FE_Dir 4)
	str path=d.FullPath
	 str name=d.FileName
	out F"<><c 0x808080><image ''{path}''>{path}</image></c>"
	 out F"<><image ''&{path}''>{path}</image>"
	 ifk(C) break

 foreach(d "$windows$\*.bmp" FE_Dir 4)
 foreach(d "$windows$\*.png" FE_Dir 4)
 foreach(d "$windows$\*.jpg" FE_Dir 4)
 foreach(d "$windows$\*.gif" FE_Dir 4)
 foreach(d "$windows$\*.ico" FE_Dir 4)

 foreach(d "$AppData$\*.bmp" FE_Dir 4)
 foreach(d "$AppData$\*.png" FE_Dir 4)
 foreach(d "$AppData$\*.jpg" FE_Dir 4)
 foreach(d "$AppData$\*.gif" FE_Dir 4)
 foreach(d "$AppData$\*.ico" FE_Dir 4)
