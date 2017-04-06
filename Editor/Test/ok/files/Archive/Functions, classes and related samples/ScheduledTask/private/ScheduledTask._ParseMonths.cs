function# $s

 Creates flags from months string like "1 2 3 4 5 6".
 If empty, all months.

if(empty(s)) ret 0xfff

ARRAY(lpstr) a
tok s a -1 " "
int f i k
for i 0 a.len
	k=val(a[i]); if(k<1 or k>12) goto ge
	f|1<<(k-1)
if(f) ret f
 ge
end "invalid months string"
