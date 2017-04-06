 DATE dFuture="01/01/2010"
DATE dFuture.getclock; dFuture=dFuture+5

rep
	DATE dNow.getclock
	if(dNow>=dFuture) break
	
	SYSTEMTIME t
	int days=dFuture.diff(dNow t)
	
	str s.format("%i days %i hours %i minutes %i seconds" days t.wHour t.wMinute t.wSecond)
	OnScreenDisplay s 2 0 0 0 0 0 5|8 "cd452"
	1

OsdHide "cd452"
