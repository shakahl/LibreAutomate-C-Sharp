out

str subject="''pns1'', ''pn, s2'': goto xxx"
str pattern="''([^'']+)''[\s,]*"
int i; ARRAY(str) a
if(findrx(subject pattern 0 4 a)<0) out "does not match"; ret
for i 0 a.len
	out a[1 i]
