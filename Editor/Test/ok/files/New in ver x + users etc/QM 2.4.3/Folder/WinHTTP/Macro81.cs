out
 IntGetFile "http://www.ihfhsjdhfjshdfjhjfk.com/file.html" _s
 out _s

 InternetOpenUrl
 InternetQueryDataAvailable
 InternetReadFile

str s
typelib WinHttp {662901FC-6951-4854-9EB2-D9A2570F2B2E} 5.1

WinHttp.WinHttpRequest r._create
r.Open("GET" "http://www.quickmacros.com/index.html")
r.Send
PF
r.WaitForResponse
PN
 s=r.ResponseText
 ARRAY(byte) a=r.ResponseBody
 s.fromn(&a[0] a.len)
__Stream k=r.ResponseStream
k.ToStr(s k.GetSize)
PN;PO
 out s
