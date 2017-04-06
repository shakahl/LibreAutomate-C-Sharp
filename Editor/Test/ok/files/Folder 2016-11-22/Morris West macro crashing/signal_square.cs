type OSDCOLORANDBORDER color border
OSDCOLORANDBORDER a b
a.color=ColorFromRGB(255 0 0)
a.border=20

b.color=ColorFromRGB(0 0 0)
b.border=20

int h
rep 1
	h=OnScreenDraw(720 425 50 50 &OSD_ProcExample2 &a)
	0.2
	OnScreenDrawEnd(h)
	0.02
	h=OnScreenDraw(720 425 50 50 &OSD_ProcExample2 &b)
	0.2
	OnScreenDrawEnd(h)
	0.02
