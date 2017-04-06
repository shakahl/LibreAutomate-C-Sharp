 /
function~ ch [codepage]

 Converts ANSI character to string.
 In Unicode mode the string will be UTF-8.


if(ch&0xffffff00) end ERR_BADARG

str s
if(_unicode)
	lpstr ss=+&ch
	s.unicode(ss codepage)
	s.ansi
else
	s=" "
	s[0]=ch

ret s
