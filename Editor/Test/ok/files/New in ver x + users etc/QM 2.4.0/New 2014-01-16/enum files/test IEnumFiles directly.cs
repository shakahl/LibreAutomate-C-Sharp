
 Dir d
 FE_Dir
out

IEnumFiles e=CreateEnumFiles
PF
 e.Begin("Q:\My QM\test\*" 2|4)
 e.Begin("Q:\My QM\test\export.qmL" 2|4)
e.Begin("Q:\My QM\test\*.qmL")
 e.Begin("Q:\My QM\test\*.qmL" 2|4)
 e.Begin("Q:\app\*.qmL")
 e.Begin("Q:\app\*")
rep
	lpstr s=e.Next; if(!s) break
	WIN32_FIND_DATAU& r=e.Data
	 outx r.dwFileAttributes
	out "<><c %i>%.*m%s</c>" iif(r.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY 0xff0000 0) e.Level 9 s
	  out e.FullPath
PN;PO
