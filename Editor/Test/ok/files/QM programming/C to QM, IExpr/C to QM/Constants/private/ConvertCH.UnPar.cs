 /CtoQM
function str&s

 Unencloses expression, eg (a) -> a.
 Does not unenclose subexpressions.

rep
	if(s[0]!='(' or s[s.len-1]!=')') break
	lpstr s2=SkipEnclosed(s ')')
	if(s2!=s+s.len) break
	s.get(s 1 s.len-2); s.trim
