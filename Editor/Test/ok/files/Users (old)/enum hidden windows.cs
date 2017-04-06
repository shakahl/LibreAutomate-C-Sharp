out
ARRAY(int) a; int i
GetWindowList(0 0 0 0 0 a)
for i 0 a.len
	if(IsWindowVisible(a[i])) continue
	str sc st
	sc.getwintext(a[i])
	st.getwintext(a[i])
	out "%i ''%s'' ''%s''" a[i] sc st
	
	