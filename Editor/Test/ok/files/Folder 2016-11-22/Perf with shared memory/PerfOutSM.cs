#compile "_PerfSM"
__SM_CATKEYS* p=_PerfSM
long _f; QueryPerformanceFrequency(&_f)
double freq=1000000.0/_f
int i n=p.perfCounter
long* a=&p.perfArr
str s="speed:"
for i 0 n
	s+"  "
	_i=freq * (a[i + 1] - a[i]) - 0.5
	s+_i
out s
