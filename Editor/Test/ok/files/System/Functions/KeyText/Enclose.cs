 /
function# $prefix $suffix [rtrimspace] [encloseempty]

 Encloses selected text.
 Returns selected text length.

 rtrimspace - if nonzero, when selection ends with spaces, does not enclose the spaces.
 encloseempty - if nonzero, encloses empty selection too.

 EXAMPLES
 Enclose "''" "''"

 if(Enclose("<i>" "</i>" 0 1)=0) key LLLL


str s.getsel; if(s.len=0 and encloseempty=0) ret
int sellen=s.len
if(suffix)
	if(rtrimspace)
		int i
		for(i s.len 0 -1) if(s[i-1]>32) break
		s.insert(suffix i)
	else s+suffix
if(prefix) s-prefix
s.setsel
ret sellen
