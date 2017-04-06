 This macro shows all symbolic links in C:\.
 If black, FE_Dir will get files in the target folder. If red, will not get.

out
Dir d
foreach(d "C:\*" FE_Dir 1|4)
	int a=d.FileAttributes
	if(a&FILE_ATTRIBUTE_REPARSE_POINT)
		str s=d.FileName(1)
		WIN32_FIND_DATA fd
		int fh=FindFirstFile(F"{s}\*" &fd)
		if(fh=-1) out F"<><c 0xff>{s}        ({_s.dllerror})</c>"
		else FindClose(fh); out s
