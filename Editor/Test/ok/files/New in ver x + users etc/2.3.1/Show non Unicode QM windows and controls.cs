out
ARRAY(int) aw ac; int i j hw hc
opt hidden 1
win "" "" "qm" 0 0 0 aw
for i 0 aw.len
	hw=aw[i]
	if(GetWindowThreadProcessId(hw 0)!=GetWindowThreadProcessId(_hwndqm 0)) continue
	if(!IsWindowUnicode(hw)) zw hw;; RECT r; GetWindowRect hw &r; zRECT r
	child "" "" hw 0 0 0 ac
	for j 0 ac.len
		hc=ac[j]
		if(!IsWindowUnicode(hc)) zw hc "child: "
		