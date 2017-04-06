__Handle ms=CreateFile("\\.\mailslot\qm test mailslot" GENERIC_WRITE FILE_SHARE_READ 0 OPEN_EXISTING FILE_ATTRIBUTE_NORMAL 0)
str s.all(1024*1-1 2 'a')

int i n=s.len+1
for i 0 1024
	if(!WriteFile(ms s n &_i 0) or _i!n) break
