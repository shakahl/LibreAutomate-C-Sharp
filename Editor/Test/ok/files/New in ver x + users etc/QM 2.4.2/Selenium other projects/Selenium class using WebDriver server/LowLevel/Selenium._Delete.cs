function! $path [str&response] [flags] ;;flags: 1 don't throw.

 Sends DELETE request and optionally gets raw response JSON.
 Returns: 1. Error if failed, unless flag 1.

 path - command as documented in Selenium WebDriver Wire Protocol. If does not begin with "/", prepends "/session/{m_sessionId}/".

 REMARKS
 Passes all errors to the first non-member.


ret _HttpRequest("DELETE" path 0 response flags)
