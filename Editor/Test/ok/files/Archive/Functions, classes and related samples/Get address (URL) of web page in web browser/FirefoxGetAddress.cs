 /
function~

 Returns URL from address bar of Firefox browser.
 Error if fails.
 Tested with Firefox 3.6.8 - 21.0. May fail with other versions.
 Fails if address bar is hidden.

 EXAMPLE
 str s=FirefoxGetAddress
 sel s 3
	 case "http://www.google.*"
	 out "Google"
	 
	 case "http://www.quickmacros.com*"
	 out "Quick Macros"
	 
	 case else
	 out s


int w=win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
Acc a=acc("Search or enter address|Go to a Website|Go to a Web Site|Search Bookmarks and History|Location" "TEXT" w "" "" 0x1802 0x0 0x20000040 "" 1)
ret a.Value

err+ end _error
