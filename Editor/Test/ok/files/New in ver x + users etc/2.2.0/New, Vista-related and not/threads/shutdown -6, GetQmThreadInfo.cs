int h=mac("Function42")
 out "---"
 out h

1
QMTHREAD q
 if(GetQmThreadInfo(0 &q))
if(GetQmThreadInfo(h &q))
	out q.qmitemid
	out q.threadhandle
	out q.threadid
	out q.tuid

shutdown -6 0 "" h
 shutdown -6 0 "Function42" h
 shutdown -6 0 "" q.threadhandle
 shutdown -6 1 "" q.threadid
 shutdown -6 2 "" q.tuid
