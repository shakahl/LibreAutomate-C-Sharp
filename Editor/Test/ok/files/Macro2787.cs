out
ARRAY(int) a b; int i j
win "" "" "" 0 "" a
for i 0 a.len
	int w=a[i]
	if(window.IsWindowCloaked(w)) continue
	child "" "" w 0 "" b
	for j 0 b.len
		a[]=b[j]
 out a.len ;;1924
0.1
int na=1000
str s.all(na) s2
PF
rep 3
	 for i 0 a.len
		 int k2 c=a[i]
		 k2=SendMessage(c WM_GETTEXT na s)
	for i 0 a.len
		int k1 k2 c=a[i]
		k1=SendMessage(c WM_GETTEXTLENGTH 0 0); if(k1<1) continue
		if k1<na
			k2=SendMessage(c WM_GETTEXT na s)
		else
			s2.all(k1)
			k2=SendMessage(c WM_GETTEXT k1+1 s2)
			s2.all
	PN
PO

 80 105 130
