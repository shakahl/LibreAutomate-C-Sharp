 /
function# $name

 Returns the number of threads of the function or macro.

 name - name of the function or macro that started the thread.

 EXAMPLE
 if(IsFunctionRunning("MyFunc"))
	 out "MyFunc is running"


int i n=EnumQmThreads(0 0 0 0)
ARRAY(QMTHREAD) a.create(n)
for i 0 EnumQmThreads(&a[0] n 0 0)
	_s.getmacro(a[i].qmitemid 1)
	if(_s=functionname) ret 1
