out
int hp=win("Find window or control")
int i
for i 1 10000
	Acc a=acc("" "" hp "" "" 0 0 0 "" 0 i)
	if(!a.a) break
	out a.Name
