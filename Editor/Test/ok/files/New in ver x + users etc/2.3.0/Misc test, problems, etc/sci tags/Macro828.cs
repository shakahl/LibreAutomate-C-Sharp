out
str s="aaa <b>hh<c>kkk</c>jj</b> zz"
ARRAY(str) a
if(!findrx(s "<(\w+)>(.+?)</\1>" 0 4 a)) ret
int i
for i 0 a.len
	out a[0 i]

if(!s.replacerx("<(\w+)>(.+?)</\1>" "$2")) ret
out s
