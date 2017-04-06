 shows clock in top-right corner of specified monitor

_monitor=1
rep
	str s.time("%X")
	OnScreenDisplay s 1 -1 1 "" 15 0x00ffff 2 "tim4924"
