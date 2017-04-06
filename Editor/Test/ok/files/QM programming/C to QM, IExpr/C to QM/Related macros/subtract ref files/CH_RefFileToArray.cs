 /
function $refFile ARRAY(str)&a

a=0
str s.getfile(refFile)
ARRAY(str) aa=s
int i
for i 0 aa.len
	str& r=a[]
	r=aa[i]
	for i i aa.len-1
		if(aa[i+1].beg("[9]") or aa[i+1].beg(" ")) r+"[]"; r+aa[i+1]
		else break
