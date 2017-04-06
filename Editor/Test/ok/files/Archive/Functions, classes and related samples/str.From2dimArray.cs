function$ ARRAY(str)&a [flags] ;;flags: 1 no row#

fix(0)
int c r
for r 0 a.len(2)
	if(!(flags&1)) formata("--- row %i ---[]" r)
	for c 0 a.len(1)
		addline(a[c r])
ret this
