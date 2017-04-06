RECT r; r.left=500; r.top=500; r.right=600; r.bottom=600
OnScreenRect 1 &r
rep 20
	0.1
	OffsetRect &r 10 10
	OnScreenRect 0 &r
OnScreenRect 2 &r
