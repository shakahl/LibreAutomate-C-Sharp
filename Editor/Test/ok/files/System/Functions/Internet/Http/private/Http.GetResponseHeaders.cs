function# hi str&srh

_i=1000
if(HttpQueryInfo(hi HTTP_QUERY_RAW_HEADERS_CRLF srh.all(_i) &_i 0) or (GetLastError=ERROR_INSUFFICIENT_BUFFER and HttpQueryInfo(hi HTTP_QUERY_RAW_HEADERS_CRLF srh.all(_i) &_i 0)))
	srh.fix(_i)
	ret 1
