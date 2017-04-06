function $fileName

 Opens the file. If does not exist, creates empty file and opens.
 Locks the file so that it cannot be opened by other processes, threads, even same thread.
 If the file is locked, waits until it is unlocked.
 Error if cannot open for some other reason, eg invalid filename.
 While waiting, don't call this again in same thread, because then will lock itself, and you'll have to restart QM.


opt waitmsg -1
Close
str s.expandpath(fileName)

rep
	handle=CreateFileW(@s GENERIC_READ|GENERIC_WRITE 0 0 OPEN_ALWAYS FILE_ATTRIBUTE_NORMAL 0)
	if(handle!=INVALID_HANDLE_VALUE) ret handle
	handle=0
	if(GetLastError!=ERROR_SHARING_VIOLATION) end "cannot open file. %s" 0 _s.dllerror
	0.01
