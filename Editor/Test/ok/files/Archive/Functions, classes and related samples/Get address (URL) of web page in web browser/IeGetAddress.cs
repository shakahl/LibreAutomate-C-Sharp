 /
function~

 Returns URL from address bar of Internet Explorer browser.
 Error if fails.
 Tested with IE 5, 6, 7, 8. May fail with other versions.
 Fails if address bar is hidden.

 EXAMPLE
 str s=IeGetAddress
 sel s 3
	 case "http://www.google.*"
	 out "Google"
	 
	 case "http://www.quickmacros.com*"
	 out "Quick Macros"
	 
	 case else
	 out s


Acc a=acc("Address" "TEXT" win(" Internet Explorer" "IEFrame") "Edit" "" 0x1801 0x0 0x20000040)
ret a.Value

err+ end _error
