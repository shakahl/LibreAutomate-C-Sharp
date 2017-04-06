function# $remoteFile $localFile [str&responsepage]

 Uploads file. Returns 1 on success, 0 on failure.
 At first you must call Connect to connect to web server.

 I cannot test how this works because my web server does not support PUT, but similar code worked well with POST.

 EXAMPLE
 Http h.Connect("www.x.com")
 if(h.PutFile("put/test.txt" "$desktop$\test.txt" _s)) out _s


int hfile fsize size size2 bufsize(100) written r
str sf buf
INTERNET_BUFFERS b

if(!sf.searchpath(localFile)) end "file does not exist"
hfile=CreateFile(sf GENERIC_READ 0 0 OPEN_EXISTING 0 0)
if(hfile=INVALID_HANDLE_VALUE) end "cannot open file"
fsize=GetFileSize(hfile 0)

b.dwStructSize=sizeof(INTERNET_BUFFERS)
b.dwBufferTotal=fsize
buf.all(bufsize)

__HInternet hi=HttpOpenRequest(m_hi "PUT" remoteFile 0 0 0 INTERNET_FLAG_RELOAD 0); if(!hi) goto e
if(!HttpSendRequestEx(hi &b 0 0 0)) goto e

rep
	if(!ReadFile(hfile buf bufsize &size 0)) r=-1; goto e
	if(!InternetWriteFile(hi buf size &size2) or size2!=size) goto e
	written+size2; if(written>=fsize) break
	
if(!HttpEndRequest(hi 0 0 0)) goto e
CloseHandle hfile

if(&responsepage) r=Read(hi responsepage)
else r=1

ret r
 e
CloseHandle hfile
if(!r) Error
