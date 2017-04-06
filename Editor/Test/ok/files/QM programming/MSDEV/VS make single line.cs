str s.getsel
if(!s.len) ret
str u t
foreach t s
	t.trim
	if(u.len) u+" "
	u+t
u+"[]"
if(u.beg("static extern ")) u-"public "
u.setsel
