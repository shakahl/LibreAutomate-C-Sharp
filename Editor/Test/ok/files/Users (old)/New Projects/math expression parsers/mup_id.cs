 /
function[c]# $s &i double&v

 out s
 if((s[0]='x' or s[0]='X') and s[-1]='0')
	  out s
	 int j
	 v=val(s-1 0 j)
	 out v
	 out j
	 i+j-1
	 ret 1

if(s[0]='x' or s[0]='X')
	 out s
	int j
	str ss.from(0 s)
	v=val(ss 0 j)
	out v
	out j
	i+j-1
	ret 1
