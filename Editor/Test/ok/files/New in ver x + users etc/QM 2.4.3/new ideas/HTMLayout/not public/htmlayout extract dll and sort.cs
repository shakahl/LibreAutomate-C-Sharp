_s.getmacro("htmlayout.txt")
ARRAY(str) a aa
if(!findrx(_s "'' \W*(\w+)" 0 4 a)) end "e"
int i
for i 0 a.len
	aa[]=a[1 i]

aa.sort(2)
out aa
