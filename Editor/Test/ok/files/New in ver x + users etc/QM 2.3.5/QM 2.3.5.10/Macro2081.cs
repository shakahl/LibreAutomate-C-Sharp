 IntGetFile "http://www.quickmacros.com" _s
 out _s

 Http h.GetUrl("http://www.quickmacros.com" _s)
 out _s

 Http h.Connect("www.quickmacros.com")
 h.Get("/index.html" _s)
 out _s

 Ftp f.Connect("ftp.quickmacros.com" "quickmac" "*")
 f.FileGetStr("/public_html/index.html" _s)
 out _s
