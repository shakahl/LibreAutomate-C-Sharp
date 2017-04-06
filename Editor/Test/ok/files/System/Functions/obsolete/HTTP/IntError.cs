 /

 Obsolete.

int e i j=400; str s
e=GetLastError

if(e=ERROR_INTERNET_EXTENDED_ERROR)
	BSTR b.alloc(j)
	if(InternetGetLastResponseInfoW(&i b &j)) s.ansi(b); s.rtrim
else s.dllerror("" "wininet.dll" e)

out "Internet error: %s" s
