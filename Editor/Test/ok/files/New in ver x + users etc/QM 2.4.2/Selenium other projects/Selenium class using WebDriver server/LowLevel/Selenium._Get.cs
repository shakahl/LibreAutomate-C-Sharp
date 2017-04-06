function! $path str&response [$getValueOf] [valueType] [flags] ;;valueType: 1 string or null, 2 number, 3 bool, 4 object, 5 array.  flags: 1 don't throw

 Sends GET request and returns its response JSON.
 If getValueOf, calls _JsonGetValue; error if not found. Else gets whole raw JSON.
 Returns: 1. Error if failed, unless flag 1.

 path - command as documented in Selenium WebDriver Wire Protocol. If does not begin with "/", prepends "/session/{m_sessionId}/".

 REMARKS
 Passes all errors to the first non-member.


if(!_HttpRequest("GET" path 0 response flags)) ret
if !empty(getValueOf)
	if !_JsonGetValue(response getValueOf _s valueType)
		if(flags&1) ret
		end response 2
	_s.swap(response)
ret 1
