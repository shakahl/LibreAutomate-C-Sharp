 /
function'long $url [inetflags]

 Returns internet file size.

 url - file URL.
 inetflags - optional dwFlags argument for InternetOpenUrl API.

 REMARKS
 Sends request to download the file, but gets only response headers, not whole file.
 Error if fails, or if file size is not specified in response headers (no Content-Length header).


#opt nowarnings 1
Http http
__HInternet hi=InternetOpenUrl(http.hinternet(1) url 0 0 inetflags 0); if(!hi) end "failed"
_s.all(30); _i=30
if(!HttpQueryInfo(hi HTTP_QUERY_CONTENT_LENGTH _s &_i 0)) end "unknown size"
ret val(_s.fix 1)
