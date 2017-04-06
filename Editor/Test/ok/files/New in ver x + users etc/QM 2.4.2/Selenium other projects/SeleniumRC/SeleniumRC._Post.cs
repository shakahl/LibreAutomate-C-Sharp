function $cmd [str&response]

opt noerrorshere 1
typelib WinHttp {662901FC-6951-4854-9EB2-D9A2570F2B2E} 5.1
WinHttp.WinHttpRequest r._create
str s=F"http://localhost:4444/selenium-server/driver/?cmd={cmd}"
if(m_sessionId.len) s+"&sessionId="; s+m_sessionId
r.Open("POST" s)
r.Send
r.WaitForResponse(10000)
str sr=r.ResponseText
 out sr
if(&response) response=sr
if(!sr.beg("OK")) end sr
