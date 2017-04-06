str s="a b"
int i
for i 0 256
	s[1]=i
	str ss=s
	ss.escape(9|2)
	str sss=ss; sss.escape(8|2)
	out "%i %c %s %s" i i ss sss
	