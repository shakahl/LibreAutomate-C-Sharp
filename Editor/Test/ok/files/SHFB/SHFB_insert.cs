 /
function ~s [flags] ;;flags: 1 ignore selection, 2 is block element, 4 key CSL, 8 key B

if(!s.len) ret
int isBlock=flags&2

int nr i=find(s "##")
if(i<0)
	i=s.len+2
	s=F"<{s}></{s}>"
else nr=2

str c
if(flags&1=0) c.getsel
int isSel=(c.len and !c.end("[10]"))
if(isSel)
	s.replace(c i nr)
	i+c.len
else
	if(nr) s.remove(i nr)
	if(isBlock) s+"[]"

paste s

if(flags&8) key B

if(!isSel or isBlock)
	int j n
	for(j i s.len) if(s[j]!=13) n+1
	key L(#n)
	if(flags&4) key CSL
