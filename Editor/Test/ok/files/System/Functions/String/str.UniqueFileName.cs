function!

 Creates unique filename from specified.
 For example, if this variable contains "c:\file.txt", and the file exists, converts it to "c:\file (n).txt", where n is a unique number.
 Returns 1 if exists, 0 if not.

 Added in: QM 2.3.3.

 EXAMPLE
 str s="$desktop$\test.txt"
 s.UniqueFileName
 out s


if(!this.len) ret
if(!FileExists(this 2)) ret
int i
if(_dir.fd.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY) i=this.len
else i=findcr(this '.'); if(i<=findcr(this '\')) i=this.len

str s1.left(this i) s2(this+i)
for i 2 1000000000
	this.format("%s (%i)%s" s1 i s2)
	if(!FileExists(this 2)) ret 1
