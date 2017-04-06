out
dll "qm.exe" #__LoadBitmapFromMemory $mem memSize flags

mkdir "$temp qm$\bmp"
Dir d
 foreach(d "$Program Files$\*.bmp" FE_Dir 4)
 foreach(d "Q:\ico and bmp\bmp\*.bmp" FE_Dir 0)
foreach(d "Q:\ico and bmp\bmp\nvcpl_152*.bmp" FE_Dir 0)
 foreach(d "Q:\ico and bmp\bmp\nvcpl_1514*.bmp" FE_Dir 0)
 foreach(d "Q:\ico and bmp\bmp\nvcpl_*.bmp" FE_Dir 0)
	str path=d.FullPath
	_s.getfile(path)
	int hb=__LoadBitmapFromMemory(_s _s.len 0)
	if(hb)
		 out path
		 str s1.encrypt(2|8 path) s2.from("$temp qm$\bmp\" s1 ".bmp")
		 if SaveBitmap(hb s2)
			 out F"<><image ''{s2}''>{path}</image>"
		 else out F"<><c 0xff0000>{path}</c>"
		DeleteObject hb
	else out F"<><c 0xff>{path}</c>"
	 break

 Q:\ico and bmp\bmp\nvcpl_15218.bmp
 Q:\ico and bmp\bmp\nvcpl_15218_1.bmp
