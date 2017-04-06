out
Http h.Connect("www.quickmacros.com")
str srh
h.FileGet("support.html" _s 0 0 0 0 srh)
out srh
out _s
