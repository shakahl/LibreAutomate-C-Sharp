 /
function# $s1 $s2 [flags] ;;flags: 1 case insensitive, 2 whole word

 Finds the last substring.
 Returns its first character index, or -1 if not found.


str a(s1) b(s2)
if(a.len<b.len or b.len=0) ret -1
strrev a
strrev b
int ins=flags&1
int i=iif(flags&2 findw(a b 0 "" flags&1|64) find(a b 0 flags&1))
if(i<0) ret i
ret a.len-i-b.len
