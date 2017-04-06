out
str s="a 'b c' d"
ARRAY(str) a
tok s a -1 "' " 128
int i
for i 0 a.len
	out a[i]
	