out
str url="http://www.quickmacros.com/test/test.php"
str html headers
IntGetFile url html 0 INTERNET_FLAG_NO_AUTO_REDIRECT 0 0 0 headers
 out html
out headers

str location
if findrx(headers "^HTTP/\S+ 301\b(?sm).+^Location: *([^\r\n]+)" 0 1 location 1)>=0
	out F"Redirected to {location}[][]"
	
	IntGetFile url html
	out html


 INTERNET_FLAG_NO_AUTO_REDIRECT
 IntGetFile "http://www.quickmacros.com/history.html" s 0 0 0 0 0 _s
