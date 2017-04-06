out
rep 20
	0.001
	 out timeGetTime
	 out GetTickCount
	long t
	GetSystemTimeAsFileTime +&t
	out t/10000
	