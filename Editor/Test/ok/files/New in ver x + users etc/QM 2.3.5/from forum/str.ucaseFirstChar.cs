
 Makes first character uppercase.


if(!this.len) ret
if this[0]<128
	this[0]=toupper(this[0])
else
	BSTR b=this ;;should get just first char, but difficult with UTF-8. This func probably will not be used with long strings.
	b[0]=CharUpperW(+b[0])
	this=b
