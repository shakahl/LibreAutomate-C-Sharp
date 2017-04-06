 kills all threads except some important threads

int i n=EnumQmThreads(0 0 0 0)
ARRAY(QMTHREAD) a.create(n)
for i 0 EnumQmThreads(&a[0] n 0 0)
	if(a[i].threadid=GetCurrentThreadId) continue ;;don't kill itself
	str name.getmacro(a[i].qmitemid 1)
	out name
	sel name
		case ["ImportantThread","ImportantThread2"] ;;don't kill these
		case else
		shutdown -6 0 name
