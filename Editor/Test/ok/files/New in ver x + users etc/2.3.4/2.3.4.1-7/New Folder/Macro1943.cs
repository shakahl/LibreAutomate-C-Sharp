out
out "----- rename -----"

int i
ARRAY(str) a
GetFilesInFolder a "$desktop$\test" "" 1
a.sort(8)
for i 0 a.len
	str& s1=a[i]
	str s2 s3
	s2=s1
	s2.insert(F"{i+1}_" findcr(s2 '\')+1)
	out s2
	 ren s1 s2
