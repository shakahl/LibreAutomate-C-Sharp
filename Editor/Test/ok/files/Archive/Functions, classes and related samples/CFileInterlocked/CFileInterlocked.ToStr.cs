function str&s

 Stores file data to s.


int n=GetFileSize(handle &_i); if(_i or n>1024*1024*1024) end ES_FAILED ;;>1GB
s.all(n)
if(n)
	SetFilePointer handle 0 0 0
	if(!ReadFile(handle s n &_i 0) or _i!=n) end ES_FAILED
	s.fix(n)
