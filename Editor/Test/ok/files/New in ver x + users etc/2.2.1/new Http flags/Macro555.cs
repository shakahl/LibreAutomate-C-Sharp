out
str srh
 Http h.GetUrl("http://www.quickmacros.com/support.html" _s 0 0 1 0 0 srh)
IntGetFile("http://www.quickmacros.com/support.html" _s 0 0 1 0 0 srh)
out srh
 out _s
 HTTP_QUERY_CONTENT_LOCATION
 HTTP_QUERY_LOCATION
 HTTP_QUERY_URI
