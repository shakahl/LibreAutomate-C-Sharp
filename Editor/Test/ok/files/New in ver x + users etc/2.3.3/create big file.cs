__HFile f.Create("$Desktop$\test1Gb.txt" CREATE_ALWAYS)
_s.all(1024*1024 2 '.')
int i
for i 0 1024
	if(!WriteFile(f _s _s.len &_i 0)) end "failed"

