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
	 outx _s.len
	int nb=_s.len+0x1000-1~(0x1000-1)
	 outx nb
	 continue
	byte* p=VirtualAlloc(0 nb+(4096*1) MEM_COMMIT|MEM_RESERVE PAGE_READWRITE)
	if(!VirtualProtect(p+nb 4096 PAGE_NOACCESS &_i)) out "FAILED"
	byte* pp=p+nb-_s.len
	memcpy pp _s _s.len
	int hb=__LoadBitmapFromMemory(pp _s.len 0)
	if(hb)
		if(hb=-1) goto g1
		 out path
		str s1.encrypt(2|8 path) s2.from("$temp qm$\bmp\" s1 ".bmp")
		if SaveBitmap(hb s2)
			out F"<><image ''{s2}''>{path}</image>"
		else out F"<><c 0xff0000>{path}</c>"
		DeleteObject hb
	else out F"<><c 0xff>{path}</c>"
	 g1
	VirtualFree p 0 MEM_RELEASE
	 break

 Q:\ico and bmp\bmp\nvcpl_15218.bmp
 Q:\ico and bmp\bmp\nvcpl_15218_1.bmp
