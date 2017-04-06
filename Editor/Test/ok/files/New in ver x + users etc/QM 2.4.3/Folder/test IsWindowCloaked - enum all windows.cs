out
opt hidden 1
ARRAY(int) w c; int i j
win "" "" "" 0 "" w
 out w.len
for i 0 w.len
	int h=w[i]
	int r=IsWindowCloaked(h); if(r) outw2 h F"{r}  "
	child "" "" h 0 "" c
	for j 0 c.len
		int k=c[j]
		r=IsWindowCloaked(k); if(r) outw2 k F"    {r}  "
		
	out "---"
