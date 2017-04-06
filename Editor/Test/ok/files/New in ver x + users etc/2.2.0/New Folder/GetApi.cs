 /
function# $name str&decl

 Retrieves Windows API declaration from database.

 name - identifier.
 decl - receives declaration.

 Returns:
  0 - found
  1 - not found
  -1 - cannot find or open file
  other negative values - other error, eg bad format.

 Database file winapi.dat can be downloaded from http://www.quickmacros.com/winapi.zip. Function InsertApi can download it.


if(!len(name)) ret -10

type CH_FILE_HEADER magic @version @oIndex nItems
CH_FILE_HEADER hd
ARRAY(POINT) a
__HFile f
int n i crc=Crc32(name -1)
str b.all(128 2); lpstr sz

 open file, read header and index
if(!f.Create("$qm$\\winapiqm.dat" OPEN_EXISTING GENERIC_READ FILE_SHARE_READ)) ret -1
if(!ReadFile(f &hd sizeof(CH_FILE_HEADER) &n 0) or n!=sizeof(CH_FILE_HEADER)) ret -2
if(memcmp(&hd.magic "QMAP" 4)) ret -3
if(hd.version!=1) ret -4
a.create(hd.nItems)
if(!ReadFile(f &a[0] a.len*sizeof(POINT) &n 0) or n!=a.len*sizeof(POINT)) ret -5

 find crc in sorted array
int j from to=a.len
rep
	if(from>=to) ret 1 ;;not found
	i=from+to/2; j=a[i].x
	if(crc<j) to=i; else if(crc>j) from=i+1; else break
for(i i 0 -1) if(a[i-1].x!=crc) break ;;find first crc (several duplicate crc exist)

 read data
rep
	decl.fix(0)
	if(SetFilePointer(f a[i].y 0 FILE_BEGIN)=0xffffffff) ret -6
	rep
		if(!ReadFile(f b b.len &n 0) or !n) ret -7
		sz=memchr(b 0 b.len)
		if(sz) decl.geta(b 0 sz-b); break
		decl.geta(b 0 b.len)
	
	i+1; if(i=a.len or a[i].x!=crc or find(decl name)>=0) break
ret
