 /Selenium reuse session
function $browser [$openURL] [flags] ;;flags: 1 start Selenium server if not running, 2 activate browser window, 4 call End when destroying variable

 REMARKS


opt noerrorshere 1
if(m_sessionId.len) end "Call End() with this variable."

ARRAY(str) a
 _GetValueArray("/sessions" a "id" 1); err
_GetValueArray("/sessions" a); err
int i found; str s1 s2 sid se
for i 0 a.len
	str& s=a[i]
	if(!_JsonGetValue(s "browserName" s1 1) or s1=browser=0) continue
	if(!_JsonGetValue(s "id" sid 1)) continue
	out sid
	
	if(_Get(F"/session/{sid}" s2 "" 0 1)) out s2; else out "no session info" ;;succeeds for invalid sessions
	
	if !_Get(F"/session/{sid}/window_handle" s2 "value" 1 1) ;;fails for invalid sessions
		 out s2
		if _JsonGetValue(s2 "message" se 1)
			out se
			sel se 2
				case ["* It may have died.*", "Session not found:*", "* cannot be used after quit() was called.*"]
				out "Deleting Selenium session %s" sid
				rep(3) if(_Delete(F"/session/{sid}" 0 1)) break
		continue
	out s2
	found=1; break

if found
	m_sessionId=sid
	if(!empty(openURL)) urlOpen(openURL)
else
	end "no sessions"


#ret

m_flags=flags
if(flags&3) Selenium_FindServer(flags&3)

str sr
lpstr cap
if(browser[0]='{') cap=browser
else cap=_JsonPair("desiredCapabilities" _JsonPairStr("browserName" browser))

_Post("/session" cap sr)
if(!_JsonGetValue(sr "sessionId" m_sessionId 1)) end sr

if(!empty(openURL)) urlOpen(openURL)
