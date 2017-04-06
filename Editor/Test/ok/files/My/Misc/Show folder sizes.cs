out
ARRAY(STRINT) a
Dir d dd
 foreach(d "C:\Users\G\*" FE_Dir 0x1) ;;32-bit
 foreach(d "$Program Files$\*" FE_Dir 0x1) ;;32-bit
 foreach(d "C:\Program Files\*" FE_Dir 0x1) ;;64-bit
 foreach(d "$Windows$\*" FE_Dir 0x1)
	str sf=d.FileName(1)
	int n=GetFileOrFolderSize(sf)/(1024*1024)
	if(n<10) continue
	STRINT& r=a[]; r.i=n; r.s=sf.getfilename(sf 1)
a.sort(1 sub.Callback_ARRAY_sort)
int i
for i 0 a.len
	&r=a[i]
	out "%-25s  %i" r.s r.i


#sub Callback_ARRAY_sort
function# param STRINT&a STRINT&b

if(a.i<b.i) ret -1
else if(a.i>b.i) ret 1
