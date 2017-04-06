out
int na=1000000
str s.all(na)
ARRAY(int) a b; int i j
win "" "" "" 0 "" a
for i 0 a.len
	int w=a[i]
	if(window.IsWindowCloaked(w)) continue
	child "" "" w 0 "" b
	 out b.len
	if(!b.len) continue
	str swn scn spn.getwinexe(w 1)
	outw w "" swn
	for j 0 b.len
		int c=b[j]
		int k1 k2
		k1=SendMessage(c WM_GETTEXTLENGTH 0 0)
		k2=SendMessage(c WM_GETTEXT k1+1 s)
		
		if k2=0 and k1=0
			 outw c
			continue
		 else outw c
		
		outw c "" scn
		int show=0
		show=k2!=k1
		
		 int k3=SendMessage(c WM_GETTEXT 0 0)
		 show=k3!=k2
		int k3=SendMessage(c WM_GETTEXT 2 s)
		show=k3<1
		
		if show
			out F"{k1} {k2} {k3}[][9]program: {spn}[][9]window: {swn}[][9]control: {scn}"
			