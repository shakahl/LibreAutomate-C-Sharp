 /
function'DateTime $url [inetflags]  ;;flags: convert UTC/GMT to local time

 Returns internet file Last-Modified date.

 url - file URL.
 inetflags - optional dwFlags argument for InternetOpenUrl API.

 REMARKS
 Sends request to download the file, but gets only response headers, not whole file.
 Error if fails, or if there is no Last-Modified header.
 The date is in GMT time zone, which is the same as UTC. Use UtcToLocal if need your local time, see example.

 EXAMPLE
 DateTime t=IntGetLastModified("http://www.quickmacros.com/support.html")
 t.UtcToLocal
 out t.ToStr


#opt nowarnings 1
Http http
__HInternet hi=InternetOpenUrl(http.hinternet(1) url 0 0 inetflags 0); if(!hi) end "failed"
SYSTEMTIME st; _i=sizeof(st)
if(!HttpQueryInfo(hi HTTP_QUERY_LAST_MODIFIED|HTTP_QUERY_FLAG_SYSTEMTIME &st &_i 0)) end "unknown Last-Modified"
DateTime r.FromSYSTEMTIME(st)
ret r
