 /
function x y [VARIANT'Window]

int h
if(getopt(nargs)>2)
	int h=VariantToHwnd(Window)
	RECT r; GetWindowRect(h &r)
	x+r.left; y+r.top
SetCursorPos x y
0.025
