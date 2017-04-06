function# $httpfile str&data minimumNbytes [usecache] [inetflags];;usecache: 0 download, 1 download if modified, 2 download if not in cache, 3 download if online;  inetflags: look in Http.GetUrl.

 Downloads file to str variable.
 Returns 1 if successful, 0 if fails.
 This is lower-level function than GetUrl, and works only in http protocol.
 At first you must call Connect to connect to web server.


data.flags|=2; data.fix(0)
int size
if(inetflags=1) inetflags=INTERNET_FLAG_KEEP_CONNECTION ;;for backward compatibility
sel usecache
	case 0
	if(!IntIsOnline) ret
	inetflags|INTERNET_FLAG_RELOAD|INTERNET_FLAG_NO_CACHE_WRITE
	case 1 inetflags|INTERNET_FLAG_RESYNCHRONIZE
	case 3 inetflags|INTERNET_FLAG_RELOAD|INTERNET_FLAG_NO_CACHE_WRITE

int hiopen = HttpOpenRequest(m_hi "GET" httpfile 0 0 0 inetflags 0)
if(!hiopen) goto error
if(!HttpSendRequest(hiopen 0 0 0 0)) goto error

rep
	if(!InternetQueryDataAvailable(hiopen &size 0 0)) goto error
	if(size)
		data.all(data.len+size 1)
		if(!InternetReadFile(hiopen data+data.len size &size)) goto error
	data.fix(data.len+size)
	if(size=0) break
	if(data.len>=minimumNbytes) break

InternetCloseHandle(hiopen)
ret 1
 error
if(hiopen) InternetCloseHandle(hiopen)
Error
