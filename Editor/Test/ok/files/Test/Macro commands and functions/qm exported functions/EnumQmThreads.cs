 Display all running threads.
int i n=EnumQmThreads(0 0 0 0)
QMTHREAD* a._new(n)
for i 0 EnumQmThreads(a n 0 0)
	out _s.getmacro(a[i].qmitemid 1)
a._delete
