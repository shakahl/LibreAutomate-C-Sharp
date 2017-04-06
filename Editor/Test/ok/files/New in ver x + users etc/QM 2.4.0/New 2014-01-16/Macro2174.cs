out
_s.setfile("Q:\My QM\test\test.tes-wik")

 str s="Q:\My QM\test\*.tes"
str s="Q:\My QM\test\*1*"
WIN32_FIND_DATA f
int h=FindFirstFile(s &f)
if(h=-1) ret
rep
	lpstr k=&f.cFileName
	out k
	lpstr kk=&f.cAlternate
	out kk
	if(!FindNextFile(h &f)) break

FindClose h
