 /
function# $url str&s [str&responseHeaders] [$requestHeaders]

 Downloads web page or other file from the Internet.
 Returns HTTP status code (200 is OK).
 Error if failed.

 url - file to download. Example: "http://www.x.com/index.htm"
 s - variable that receives file data.
 responseHeaders - variable that receives response headers.
 requestHeaders - headers to send, like "name1: value1[]name2: value2[]...".

 REMARKS
 There are 2 ways specify proxy settings:
   1. Call <help>IntSettings</help> with useproxy = 1 and proxy_name etc. The this function will call <link "https://msdn.microsoft.com/en-us/library/windows/desktop/aa384059%28v=vs.85%29.aspx">WinHttpRequest.SetProxy</link> with these settings.
   2. Run WinHTTP proxy configuration utility <link "https://msdn.microsoft.com/en-us/library/windows/desktop/aa384069%28v=vs.85%29.aspx">Netsh.exe or ProxyCfg.exe</link>. These proxy settings are used by this function and WinHTTP functions, and are not those used by Internet Explorer and WinInet functions that are used by IntGetFile and other QM functions.
 To not use a proxy, call IntSettings with useproxy = 2, or don't configure proxy settings with the configuration utilities.

 EXAMPLE
 str s
 int status=IntGetFileWH("http://www.quickmacros.com/index.html" s)
 if(status!=200) end F"failed, status={status}"
 out s


opt noerrorshere 1
typelib WinHttp {662901FC-6951-4854-9EB2-D9A2570F2B2E} 5.1

WinHttp.WinHttpRequest r._create

 apply IntSettings
sel __intsett.useproxy
	case 1 r.SetProxy(2 __intsett.proxy_name __intsett.proxy_bypass)
	case 2 r.SetProxy(1)

r.Open("GET" url)
foreach _s requestHeaders
	str hName hValue
	if(tok(_s &hName 2 ":" 0x2002)<2) end "requestHeaders must be: ''name1: value1[91]]name2: value2[91]]...''"
	r.SetRequestHeader(hName hValue)
	int isUA; if(matchw(hName "User-Agent" 1)) isUA=1
if(!isUA) r.SetRequestHeader("User-Agent" iif(__intsett.user_agent.len __intsett.user_agent "Quick Macros"))
r.Send
 r.WaitForResponse
if(&responseHeaders) responseHeaders=r.GetAllResponseHeaders
ARRAY(byte) a=r.ResponseBody
s.fromn(&a[0] a.len)
ret r.Status
