function [nocheckm_hi]

if(!m_hi and !nocheckm_hi) lasterror="not connected"; ret
int e j=1000

e=GetLastError
if e=ERROR_INTERNET_EXTENDED_ERROR
	 g1
	BSTR b.alloc(j)
	if(InternetGetLastResponseInfoW(&_i b &j)) lasterror.ansi(b); lasterror.rtrim
	else if(GetLastError=ERROR_INSUFFICIENT_BUFFER) goto g1
	else lasterror="unknown error"
else if e
	lasterror.dllerror("" "wininet.dll" e)
	if(!lasterror.len) lasterror.dllerror("" "" e)
else lasterror="unknown error"
