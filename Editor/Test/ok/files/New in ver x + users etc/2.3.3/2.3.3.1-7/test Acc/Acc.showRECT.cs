

int x y cx cy
Location(x y cx cy); err out _error.description; ret
RECT r; SetRect &r x y x+cx y+cy
outRECT r
if r.right>r.left
	OnScreenRect 0 r
	1
	OnScreenRect 2 r
