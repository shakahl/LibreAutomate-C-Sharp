 /
function~

 Returns URL from address bar of Google Chrome browser.
 Error if fails.
 Tested with Chrome 5.0. May fail with other versions.
 Fails if address bar is hidden.

 EXAMPLE
 str s=ChromeGetAddress
 sel s 3
	 case "http://www.google.*"
	 out "Google"
	 
	 case "http://www.quickmacros.com*"
	 out "Quick Macros"
	 
	 case else
	 out s


Acc a=acc("Address" "GROUPING" win(" Chrome" "Chrome_WidgetWin_0") "" "" 0x1801 0x0 0x20000040)
ret a.Value

err+ end _error
