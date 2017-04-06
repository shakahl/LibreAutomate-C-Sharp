out
ARRAY(int) a
win "" "" "" 0 "" a
int i
for i 0 a.len
	int h=a[i]
	outw h
	int hi=GetWindowIcon(h 8)
	out hi
	if(hi) DestroyIcon hi
	