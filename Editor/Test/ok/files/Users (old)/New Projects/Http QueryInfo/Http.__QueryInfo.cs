function# $_file what str&info

 Retrieves specified response header for _file.
 what is one of HTTP_QUERY_x flags. Use HTTP_QUERY_RAW_HEADERS_CRLF to retrieve all headers.

 EXAMPLES
 Http h.Connect("www.google.com")
 str s
 if(h.QueryInfo("about.html" HTTP_QUERY_RAW_HEADERS_CRLF s))
	 out s
 
 if(h.QueryInfo("about.html" HTTP_QUERY_LAST_MODIFIED s))
	 out s
 
 if(h.QueryInfo("about.html" HTTP_QUERY_LAST_MODIFIED|HTTP_QUERY_FLAG_SYSTEMTIME s))
	 SYSTEMTIME& st=+s
	 DATE d.fromsystemtime(st)
	 out d


def ERROR_INSUFFICIENT_BUFFER  122

int hr=HttpOpenRequest(m_hi 0 _file 0 0 0 INTERNET_FLAG_RELOAD 0); if(!hr) goto e
if(!HttpSendRequest(hr 0 0 0 0)) goto e
_i=1000
if(!HttpQueryInfo(hr what info.all(_i) &_i 0))
	if(GetLastError!=ERROR_INSUFFICIENT_BUFFER) goto e
	if(!HttpQueryInfo(hr what info.all(_i) &_i 0)) goto e
info.fix(_i)
InternetCloseHandle hr
ret 1
 e
if(hr) InternetCloseHandle hr
Error
