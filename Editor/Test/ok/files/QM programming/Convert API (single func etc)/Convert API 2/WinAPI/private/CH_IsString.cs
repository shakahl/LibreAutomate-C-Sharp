 /
function str&s i
int j l1 l2 q
l1=findcr(s 10 i)+1
l2=findc(s 13 i); if(!l2) l2=s.len
for(j l1 i)
	if(q)
		if(s[j]=34 and s[j-1]!='\') q=0
	else
		if(s[j]=34) q=1
ret q
