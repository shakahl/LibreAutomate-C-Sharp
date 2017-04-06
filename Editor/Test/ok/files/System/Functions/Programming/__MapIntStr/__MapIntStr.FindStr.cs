function# $strVal [flags] ;;flags: 1 insens

 Finds a string. Returns its index, or -1 if not found.


int i ins=flags&1
for(i 0 a.len) if(!StrCompare(a[i].s strVal ins)) ret i
ret -1
