 EnumQmThreads
 GetQmThreadInfo

 int i n=EnumQmThreads(0 0 0 0)
 ARRAY(QMTHREAD) a.create(n)
 for i 0 EnumQmThreads(&a[0] n 0 0)
	 out _s.getmacro(a[i].qmitemid 1)

if(EnumQmThreads(0 0 0 "SpamFilter")) out "running"
