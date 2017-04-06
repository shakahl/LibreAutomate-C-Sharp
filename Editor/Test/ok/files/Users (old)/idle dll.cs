 detects time of keyboard/mouse last input event
 on Win2K+, use GetLastInputInfo instead

dll "IdleTrac.dll"
	[1]#IdleTrackerGetLastTickCount
	[2]#IdleTrackerInit
	[3]IdleTrackerTerm

if(!IdleTrackerInit) ret
atend IdleTrackerTerm

rep 10
	1
	out IdleTrackerGetLastTickCount
