 /Macro92
function# $server $_file str&lastmodified

Http h.Connect(server); err end _error
int& hc=&h+4
int hr=HttpOpenRequest(hc 0 _file 0 0 0 INTERNET_FLAG_RELOAD 0); if(!hr) ret
out 1
if(!HttpSendRequest(hr 0 0 0 0)) ret
out 2
lastmodified.all(100); _i=100
if(!HttpQueryInfo(hr HTTP_QUERY_LAST_MODIFIED lastmodified &_i 0))
	out GetLastError
	ret
 if(!HttpQueryInfo(hr HTTP_QUERY_CONTENT_LENGTH lastmodified &_i 0)) ret
lastmodified.fix(_i)
ret 1
