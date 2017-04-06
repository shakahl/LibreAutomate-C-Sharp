function! $path [$data] [str&response] [flags] ;;flags: 1 don't throw.

 Sends POST request and optionally gets raw response JSON.
 Returns: 1. Error if failed, unless flag 1.

 path - command as documented in Selenium WebDriver Wire Protocol, eg "/session". If does not begin with "/", prepends "/session/{m_sessionId}/".
 data - valid JSON. This function sends it raw.

 REMARKS
 Passes all errors to the first non-member.


ret _HttpRequest("POST" path data response flags)
