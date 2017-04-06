 Gets list of currently running visible process paths for macro "Run many programs for testing UI".

out
ARRAY(int) a; int i
win "" "" "" 0 "" a
 out a.len
IStringMap m._create
m.Flags=1
for i 0 a.len
	int w=a[i]
	if(window.IsWindowCloaked(w)) continue
	 outw w
	str s.getwinexe(w 1)
	sel s 3
		case ["*\explorer.exe","*\qm.exe"] continue
	m.Add(s); err continue
	out s
	