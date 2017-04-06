 /
function'str $url [inetflags]

 Returns internet file Last-Modified date as raw header string.

 url - file URL.
 inetflags - optional dwFlags argument for InternetOpenUrl API.

 REMARKS
 Sends request to download the file, but gets only response headers, not whole file.
 Error if fails, or if there is no Last-Modified header.


#opt nowarnings 1
Http http
__HInternet hi=InternetOpenUrl(http.hinternet(1) url 0 0 inetflags 0); if(!hi) end "failed"
_s.all(300); _i=300
if(!HttpQueryInfo(hi HTTP_QUERY_LAST_MODIFIED _s &_i 0)) end "unknown Last-Modified"
ret _s.fix
