ClearOutput
 QMTHREAD
 qm.EnumQmThreads

ARRAY(int) a
ARRAY(str) b
int t1=perf
EnumProcessesEx &a &b 1
 EnumProcessesEx &a &b
 EnumProcessesEx &a
int t2=perf
out t2-t1

int i
for i 0 a.len
	out "%i %s" a[i] b[i]
	 out "%i" a[i]
