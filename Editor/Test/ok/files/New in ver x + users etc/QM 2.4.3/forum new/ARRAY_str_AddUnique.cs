 /
function# ARRAY(str)&a $s [flags] ;;flags: 1 case insensitive

 Adds string s to array a. Does not add if a already contains the string.
 Returns index of found or new element.


int i ci(flags&1)
for i 0 a.len
	if(!StrCompare(a[i] s ci)) ret i
a[]=s
ret a.ubound
