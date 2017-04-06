 /
function~

 Returns URL from address bar of Opera browser.
 Error if fails.
 Tested with Opera 10.61. May fail with other versions.
 Fails if address bar is hidden.

 EXAMPLE
 str s=OperaGetAddress
 sel s 3
	 case "http://www.google.*"
	 out "Google"
	 
	 case "http://www.quickmacros.com*"
	 out "Quick Macros"
	 
	 case else
	 out s


Acc a=acc("" "TEXT" win(" - Opera" "OperaWindowClass") "" "Enter address or search" 0x1C04 0x0 0x20000040)
ret a.Value

err+ end _error
