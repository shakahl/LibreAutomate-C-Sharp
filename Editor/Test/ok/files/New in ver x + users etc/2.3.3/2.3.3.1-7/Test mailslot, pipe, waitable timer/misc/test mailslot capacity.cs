/exe
out
__Handle wt=CreateWaitableTimer(0 0 "Global\QM test timer")
__Handle ms=CreateFile("\\.\mailslot\qm test mailslot" GENERIC_WRITE FILE_SHARE_READ 0 OPEN_EXISTING FILE_ATTRIBUTE_NORMAL 0)
str s.all(1024*1-1 2 'k')

int i n=s.len+1
Q &q
for i 0 1024
	if(!WriteFile(ms s n &_i 0) or _i!n) break
Q &qq; outq
out i

long t=-1
SetWaitableTimer(wt +&t 0 0 0 0)
