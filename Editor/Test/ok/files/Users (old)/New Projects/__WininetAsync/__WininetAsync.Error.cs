function [nocheckm_hi]

if(!m_hi and !nocheckm_hi) end ERR_INIT 2
int e i j=400
str s

e=GetLastError
if(e=ERROR_INTERNET_EXTENDED_ERROR)
	BSTR b.alloc(j)
	if(InternetGetLastResponseInfoW(&i b &j)) lasterror.ansi(b); lasterror.rtrim
	else lasterror="unknown error"
else lasterror.dllerror("" "wininet.dll" e)
