 /
function# $computer str&password

ARRAY(str) a; int i
RegGetValues a "\NetShare" 0 1
for(i 0 a.len) if(a[0 i]~computer) break
if(i=a.len) ret
password=a[1 i]
ret 1
