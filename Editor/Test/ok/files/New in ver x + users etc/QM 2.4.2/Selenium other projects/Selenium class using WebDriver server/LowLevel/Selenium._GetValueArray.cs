function! $path ARRAY(str)&a [$getValueOf] [valueType] [flags] ;;valueType: 0 any, 1 string or null, 2 number, 3 bool, 4 object, 5 array.  flags: 1 don't throw.

 Sends GET request, finds "value":[array] in the response JSON, and gets array elements.
 If getValueOf, for each element calls _JsonGetValue with valueType. Else gets raw elements.
 Returns: 1. Error if failed, unless flag 1.

 path - command as documented in Selenium WebDriver Wire Protocol. If does not begin with "/", prepends "/session/{m_sessionId}/".

 REMARKS
 Passes all errors to the first non-member.


str sr
if(!_HttpRequest("GET" path 0 sr)) ret
if !_JsonGetArray(sr "value" a getValueOf valueType)
	if(flags&1) ret
	end sr 2
ret 1
