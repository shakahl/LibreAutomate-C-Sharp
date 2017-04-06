function row ARRAY(str)&a str&s

out "--- row %i ---" row
s=""
int c
for c 1 a.ubound+1
	out a[c]
	s+a[c]
