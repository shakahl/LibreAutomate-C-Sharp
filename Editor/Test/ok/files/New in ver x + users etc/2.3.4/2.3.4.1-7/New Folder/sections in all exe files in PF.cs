out
Dir d
foreach(d "$Program Files$\*.exe" FE_Dir 4|8)
	str path=d.FileName(1)
	out path
	str data.getfile(d.FileName(1));; err ...
	IMAGE_DOS_HEADER* dh=data
	IMAGE_NT_HEADERS* nh=data+dh.e_lfanew
	int i nsec=nh.FileHeader.NumberOfSections; err out "<><c 0xff>%i</c>" dh.e_lfanew; continue
	str s.fix(0)
	IMAGE_SECTION_HEADER* sp=+(nh+sizeof(IMAGE_NT_HEADERS))
	for i 0 nsec
		IMAGE_SECTION_HEADER& sec=sp[i]
		s.formata(" %.7s" &sec.Name); err continue
	out s
	
