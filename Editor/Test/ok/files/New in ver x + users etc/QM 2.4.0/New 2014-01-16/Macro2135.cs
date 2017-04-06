int w=win("Window Name")
__ProcessMemory m.Alloc(w 0)
double d ;;or FLOAT d, it's 4 bytes, double is 8 bytes
rep 1
	m.ReadOther(&d address sizeof(d))
	out d
	if d!=3.5
		d=3.5
		m.WriteOther(address &d sizeof(d))
	0.1
