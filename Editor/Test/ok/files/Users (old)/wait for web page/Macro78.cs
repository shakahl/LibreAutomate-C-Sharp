 This probably will succeed
web "http://www.quickmacros.com/" 0x11 0 "*"
err
	mes "url 1 failed"

 This probably will fail, and message box will be displayed
web "http://www.quickmacros.com/hkhkhkhlkh.htm" 0x11 0 "*"
err
	mes "url 2 failed"

