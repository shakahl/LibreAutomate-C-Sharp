 /
function $url $data [str&responsepage] [$headers]

 Posts web form data.
 Error if failed.

 url - full URL.
 data, responsepage, headers - see Http.Post.

 See also: <Http.Post>, <Http.PostFormData>.


Http h
 get server and path from url
URL_COMPONENTS c.dwStructSize=sizeof(c)
c.dwHostNameLength=1
c.dwUrlPathLength=1
if(!InternetCrackUrl(url len(url) 0 &c) or !c.lpszUrlPath) end ERR_BADARG
str s.left(c.lpszHostName c.lpszUrlPath-c.lpszHostName)
if(!c.dwUrlPathLength) c.lpszUrlPath=0
 connect and post
h.Connect(s); err end _error
if(!h.Post(c.lpszUrlPath data responsepage headers)) end ERR_FAILED
