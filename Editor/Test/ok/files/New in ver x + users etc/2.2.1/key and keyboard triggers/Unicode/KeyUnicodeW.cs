 /
function word*text tlen [altnum]

int i c
for i 0 tlen
	c=text[i]
	 out "0x%X %i" c c
	if(c&0xff00)
		if(!altnum) c|0x40000
		key (c)
	else
		_s.fix(0)
		for(i i tlen) if(text[i]&0xff00) i-1; break; else _s.set(text[i] _s.len 1)
		key (_s)
