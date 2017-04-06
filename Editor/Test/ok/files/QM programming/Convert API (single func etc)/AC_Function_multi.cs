str s.getsel; if(!s.len) ret
AC_Prepare s
ARRAY(str) a
str p="^ *.+\((?s).*?\);"
if(findrx(s p 0 4|8 a)<0) ret
int i; str ss
for i 0 a.len
	s=a[0 i]
	if(!AC_Function2(s "" 1 1)) continue
	ss+s
ss.setsel
