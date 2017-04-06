function~ $path [valueType] ;;valueType: 1 string or null, 2 number, 3 bool, 4 object, 5 array

 Sends GET request and returns value of "value" from its response JSON.
 Same as _Get(path "value" valueType).

 path - command as documented in Selenium WebDriver Wire Protocol. If does not begin with "/", prepends "/session/{m_sessionId}/".

 REMARKS
 Passes all errors to the first non-member.


str sr
_HttpRequest("GET" path 0 sr)
if(!_JsonGetValue(sr "value" _s valueType)) end sr 2
ret sr
