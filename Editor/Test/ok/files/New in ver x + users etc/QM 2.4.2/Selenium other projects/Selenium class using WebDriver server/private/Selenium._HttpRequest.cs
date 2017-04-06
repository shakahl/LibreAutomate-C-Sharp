function! $method $path [$data] [str&response] [flags] ;;flags: 1 don't throw

 Sends HTTP request and optinally gets raw response.
 Returns: 1. Error if failed, unless flag 1.

 method: "POST", "GET" or "DELETE".
 path - command as documented in Selenium WebDriver Wire Protocol, eg "/session". If does not begin with "/", prepends "/session/{m_sessionId}/".

 REMARKS
 Passes all errors to the first non-member.


if(path[0]!'/') path=F"/session/{m_sessionId}{`/`+empty(path)}{path}"
if(!m_seleniumURL.len) m_seleniumURL="http://localhost:4444/wd/hub"
str s=F"{m_seleniumURL}{path}"
 out s

if(!m_req) m_req._create
m_req.Open(method s)
 m_req.SetRequestHeader("Content-Type" "application/json;charset=UTF-8'")
 m_req.SetRequestHeader("Accept" "application/json")

m_req.Send(data)
 if(data) m_req.Send(data); else m_req.Send()
m_req.WaitForResponse(10000);;TODO

if m_req.Status!200
	_s=m_req.ResponseText
	if flags&1
		if(&response) _s.swap(response)
		ret
	end _s 2
if(&response) response=m_req.ResponseText

ret 1
err+
	if flags&1
		if(&response) response=_error.description
		ret
	end _error 2
