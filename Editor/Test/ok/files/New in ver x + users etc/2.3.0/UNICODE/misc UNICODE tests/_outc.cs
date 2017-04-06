 /
function~ ch

str s
if(_unicode)
	lpstr ss=+&ch
	s.unicode(ss 0)
	s.ansi
else
	s=" "
	s[0]=ch

ret s
