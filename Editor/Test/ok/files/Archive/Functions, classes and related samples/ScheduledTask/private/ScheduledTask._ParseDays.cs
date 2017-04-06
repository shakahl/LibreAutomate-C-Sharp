function# $s

 Creates flags from week days string like "0 1 2 3 4 5 6".
 0 or 7 is Sunday.
 If s empty, returns Monday.

if(empty(s)) ret 2

ARRAY(lpstr) a
tok s a -1 " "
int f i k
for i 0 a.len
	k=val(a[i] 0 _i); if(!_i or k<0 or k>7) goto ge
	if(k=7) k=0
	f|1<<k
if(f) ret f
 ge
end "invalid week days string"
