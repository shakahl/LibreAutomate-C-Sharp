out
int h hp=win("Find window or control")
int i
for i 1 10000
	h=child("" "" hp 0 0 0 i)
	if(!h) break
	zw h
