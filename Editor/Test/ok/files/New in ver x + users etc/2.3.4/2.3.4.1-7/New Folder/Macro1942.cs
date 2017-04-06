out
out "----- access -----"

int i
str s2 s3
s2.expandpath("$desktop$\test\")
for i 0 1000000000
	lpstr filename=dir(F"{s2}{i+1}_*" 1); if(!filename) break
	s3=F"{s2}{filename}"
	out s3
