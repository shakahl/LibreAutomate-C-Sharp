out
 _s.setfile("Q:\My QM\test\test.tes-wik")
 _s.setfile("Q:\My QM\test\Dest.tes")
 _s.setfile("Q:\My QM\test\Dest long fn.tes")

str s="Q:\My QM\test\*.tes"
 str s="Q:\My QM\test\*1*"
 str s="Q:\My QM\test\ARCHIV~1.QML"

lpstr k=dir(s)
rep
	if(!k) break
	out k
	out "<><c 0x8000>%s</c>" &_dir.fd.cAlternate
	k=dir

 test also: CompareFilesInFolders, GetFilesInFolder, FileCopy/Move/Delete (System)
2
#ret
FE_Dir