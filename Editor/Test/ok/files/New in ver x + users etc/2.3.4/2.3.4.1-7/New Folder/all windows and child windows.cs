out
ARRAY(int) w c
opt hidden 1
win "" "" "" 0 "" w
int i j
for i 0 w.len
	_s.getwinclass(w[i]); if(findcs(_s "*?")>=0) outw w[i] ;;wildcard
	_s.getwinclass(w[i]); if(findrx(_s "[\x80-\xff]")>=0) outw w[i] ;;unicode
	child "" "" w[i] 0 "" c
	for j 0 c.len
		_s.getwinclass(c[j]); if(findcs(_s "*?")>=0) outw c[j]
		_s.getwinclass(c[j]); if(findrx(_s "[\x80-\xff]")>=0) outw c[j]
		